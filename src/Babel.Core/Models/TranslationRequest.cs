namespace Babel.Core.Models;

public class TranslationRequest
{
    public string Text { get; set; } = string.Empty;
    public string SourceLanguage { get; set; } = "auto";
    public string TargetLanguage { get; set; } = "Portuguese (PT-BR)";
    public string PlayerName { get; set; } = string.Empty;
    public ColorDto PlayerColor { get; set; } = ColorDto.White;
    public bool IsOutgoing { get; set; }
}
