namespace Babel.Core.Interfaces;

public interface IInjectorService
{
    bool IsInjected(int processId, string dllName = "BabelHook.dll");
    bool Inject(int processId, string dllPath);
}
