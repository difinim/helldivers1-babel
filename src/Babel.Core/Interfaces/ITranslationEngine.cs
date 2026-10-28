using Babel.Core.Ipc;
using Babel.Core.Models;

namespace Babel.Core.Interfaces;

public interface ITranslationEngine
{
    bool IsPaused { get; }
    string LastIncomingLanguage { get; set; }
    BabelConfig Config { get; set; }
    int LocalPlayerIndex { get; set; }
    string LocalPlayerName { get; set; }

    void TogglePause();
    void SetPaused(bool isPaused);
    void RegisterRecentBroadcast(string message);

    event Action<TranslationRequest>? TranslationStarted;
    event Action<ChatMessage, TranslationResult>? MessageTranslated;
    event Action<string>? TranslationFailed;
    event Action<TranslationRequest, string>? TranslationRequestFailed;
    event Action<bool>? StateChanged;

    Task HandlePacketAsync(IpcPacket packet, CancellationToken ct = default);
}
