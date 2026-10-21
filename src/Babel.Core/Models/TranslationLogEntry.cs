using System;

namespace Babel.Core.Models;

public class TranslationLogEntry
{
    public string Time { get; set; } = DateTime.Now.ToString("HH:mm:ss");
    public string Author { get; set; } = string.Empty;
    public string SourceLang { get; set; } = string.Empty;
    public string OriginalMessage { get; set; } = string.Empty;
    public string TranslatedMessage { get; set; } = string.Empty;
    public string Latency { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#FFD700";
}
