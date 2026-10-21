using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using Babel.Core.Interfaces;
using Babel.Core.Ipc;

namespace Babel.Core.Services;

public class NamedPipeServer : IIpcServer, IAsyncDisposable, IDisposable
{
    public const string DefaultPipeName = "helldivers1_babel";

    private readonly string _pipeName;
    private NamedPipeServerStream? _pipeStream;
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private CancellationTokenSource? _cts;
    private Task? _listenerTask;
    private TaskCompletionSource<bool> _serverReadyTcs = new();
    private volatile bool _isRunning;
    private volatile bool _clientConnected;

    public bool IsClientConnected => _clientConnected && _pipeStream is { IsConnected: true };

    public event Action<IpcPacket>? PacketReceived;
    public event Action? ClientConnected;
    public event Action? ClientDisconnected;

    public NamedPipeServer(string pipeName = DefaultPipeName)
    {
        _pipeName = string.IsNullOrWhiteSpace(pipeName) ? DefaultPipeName : pipeName;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        if (_isRunning)
        {
            return;
        }

        _isRunning = true;
        _serverReadyTcs = new TaskCompletionSource<bool>();
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        _listenerTask = Task.Run(ListenLoopAsync);

        // Wait until server pipe instance is created
        await Task.WhenAny(_serverReadyTcs.Task, Task.Delay(2000, ct)).ConfigureAwait(false);
    }

    public async Task StopAsync()
    {
        if (!_isRunning)
        {
            return;
        }

        _isRunning = false;

        try
        {
            _cts?.Cancel();
        }
        catch { }

        if (_pipeStream != null)
        {
            try
            {
                _pipeStream.Close();
            }
            catch { }
        }

        if (_listenerTask != null)
        {
            try
            {
                await _listenerTask.ConfigureAwait(false);
            }
            catch { }
            _listenerTask = null;
        }

        try
        {
            _cts?.Dispose();
        }
        catch { }
        _cts = null;
    }

    public async Task SendPacketAsync(IpcPacket packet, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(packet);

        if (!_isRunning || !IsClientConnected || _pipeStream == null)
        {
            BabelLog.Warn("NamedPipeServer", $"Cannot send packet {packet.Type}: server running={_isRunning}, client connected={IsClientConnected}");
            return;
        }

        var jsonLine = packet.ToJsonLine();
        var bytes = Encoding.UTF8.GetBytes(jsonLine);
        BabelLog.Info("NamedPipeServer", $"Sending packet: {packet.Type} -> {jsonLine.TrimEnd()}");

        try
        {
            await _writeLock.WaitAsync(ct).ConfigureAwait(false);
        }
        catch (ObjectDisposedException)
        {
            return;
        }

        try
        {
            if (_isRunning && _pipeStream is { IsConnected: true })
            {
                await _pipeStream.WriteAsync(bytes, ct).ConfigureAwait(false);
                await _pipeStream.FlushAsync(ct).ConfigureAwait(false);
            }
        }
        catch (IOException ex)
        {
            BabelLog.Warn("NamedPipeServer", $"IOException sending packet {packet.Type}: {ex.Message}");
        }
        catch (ObjectDisposedException)
        {
            // Stream disposed during send
        }
        finally
        {
            try
            {
                _writeLock.Release();
            }
            catch { }
        }
    }

    private async Task ListenLoopAsync()
    {
        var token = _cts?.Token ?? CancellationToken.None;

        while (!token.IsCancellationRequested && _isRunning)
        {
            try
            {
                _pipeStream = new NamedPipeServerStream(
                    _pipeName,
                    PipeDirection.InOut,
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                _serverReadyTcs.TrySetResult(true);

                BabelLog.Info("NamedPipeServer", $"Waiting for client connection on pipe '{_pipeName}'...");
                await _pipeStream.WaitForConnectionAsync(token).ConfigureAwait(false);

                _clientConnected = true;
                BabelLog.Info("NamedPipeServer", $"Client connected to pipe '{_pipeName}'.");
                ClientConnected?.Invoke();

                using var reader = new StreamReader(
                    _pipeStream,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 4096,
                    leaveOpen: true);

                while (!token.IsCancellationRequested && _pipeStream.IsConnected)
                {
                    string? line;
                    try
                    {
                        line = await reader.ReadLineAsync(token).ConfigureAwait(false);
                    }
                    catch (IOException)
                    {
                        break;
                    }

                    if (line == null)
                    {
                        // EOF reached, client disconnected
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    try
                    {
                        var packet = IpcPacket.FromJsonLine(line);
                        if (packet != null)
                        {
                            BabelLog.Info("NamedPipeServer", $"Received packet: {packet.Type} -> {line.TrimEnd()}");
                            PacketReceived?.Invoke(packet);
                        }
                    }
                    catch (JsonException ex)
                    {
                        BabelLog.Warn("NamedPipeServer", $"Malformed JSON packet: {ex.Message} -> {line}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception ex)
            {
                BabelLog.Error("NamedPipeServer", "Exception in pipe listen loop", ex);
                if (token.IsCancellationRequested || !_isRunning)
                {
                    break;
                }
            }
            finally
            {
                bool wasConnected = _clientConnected;
                _clientConnected = false;

                if (_pipeStream != null)
                {
                    try
                    {
                        _pipeStream.Dispose();
                    }
                    catch { }

                    _pipeStream = null;
                }

                if (wasConnected)
                {
                    BabelLog.Info("NamedPipeServer", "Client disconnected from pipe.");
                    ClientDisconnected?.Invoke();
                }
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync().ConfigureAwait(false);
        try { _writeLock.Dispose(); } catch { }
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        StopAsync().GetAwaiter().GetResult();
        try { _writeLock.Dispose(); } catch { }
        GC.SuppressFinalize(this);
    }
}
