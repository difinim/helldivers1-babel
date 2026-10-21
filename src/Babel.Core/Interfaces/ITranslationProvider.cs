using Babel.Core.Models;

namespace Babel.Core.Interfaces;

public interface ITranslationProvider
{
    string ProviderId { get; }
    string DisplayName { get; }
    Task<TranslationResult> TranslateAsync(TranslationRequest request, BabelConfig config, CancellationToken ct = default);
    Task<bool> ValidateCredentialsAsync(BabelConfig config, CancellationToken ct = default);
}
