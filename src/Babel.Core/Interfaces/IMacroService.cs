using Babel.Core.Models;

namespace Babel.Core.Interfaces;

public interface IMacroService
{
    bool IsPaused { get; }
    void SetPaused(bool paused);
    void TogglePause();
    void UpdateMacros(IEnumerable<QuickChatMacro> macros);
    Task ExecuteMacroAsync(int slotId);
    event Action<int, string>? MacroExecuted;
    event Action<bool>? StateChanged;
}
