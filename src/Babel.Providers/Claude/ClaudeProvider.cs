using Babel.Core.Models;
using Babel.Providers.Base;

namespace Babel.Providers.Claude;

/// <summary>
/// Translation adapter stub for Anthropic Claude.
/// </summary>
public class ClaudeProvider : BaseTranslationProvider
{
    public const string Id = "Claude";

    public override string ProviderId => Id;
    public override string DisplayName => "Anthropic Claude";

    public ClaudeProvider(HttpClient? httpClient = null) : base(httpClient)
    {
    }

    public override Task<TranslationResult> TranslateAsync(
        TranslationRequest request,
        BabelConfig config,
        CancellationToken ct = default)
    {
        throw new NotImplementedException("Coming in next release");
    }

    public override Task<bool> ValidateCredentialsAsync(
        BabelConfig config,
        CancellationToken ct = default)
    {
        throw new NotImplementedException("Coming in next release");
    }

    protected override Task<TranslationResult> TranslateCoreAsync(
        TranslationRequest request,
        BabelConfig config,
        CancellationToken ct)
    {
        throw new NotImplementedException("Coming in next release");
    }
}
