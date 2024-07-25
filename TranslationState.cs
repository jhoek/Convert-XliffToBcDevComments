
using System.Security.Cryptography;

namespace ConvertXliffToBcDevComments;

public enum TranslationState
{
    Final,
    NeedsAdaptation,
    NeedsLocalization,
    NeedsReviewAdaptation,
    NeedsReviewLocalization,
    NeedsReviewTranslation,
    NeedsTranslation,
    New,
    SignedOff,
    Translated
}

public static class TranslationStateParser
{
    public const string Final = "final";
    public const string NeedsAdaptation = "needs-adaptation";
    public const string NeedsLocalization = "needs-l10n";
    public const string NeedsReviewAdaptation = "needs-review-adaptation";
    public const string NeedsReviewLocalization = "needs-review-l10n";
    public const string NeedsReviewTranslation = "needs-review-translation";
    public const string NeedsTranslation = "needs-translation";
    public const string New = "new";
    public const string SignedOff = "signed-off";
    public const string Translated = "translated";

    public static TranslationState? Parse(string value) =>
        value switch
        {
            null => null,
            "" => null,
            Final => TranslationState.Final,
            NeedsAdaptation => TranslationState.NeedsAdaptation,
            NeedsLocalization => TranslationState.NeedsLocalization,
            NeedsReviewAdaptation => TranslationState.NeedsReviewAdaptation,
            NeedsReviewLocalization => TranslationState.NeedsReviewLocalization,
            NeedsReviewTranslation => TranslationState.NeedsReviewTranslation,
            NeedsTranslation => TranslationState.NeedsTranslation,
            New => TranslationState.New,
            SignedOff => TranslationState.SignedOff,
            Translated => TranslationState.Translated,
            _ => throw new ArgumentOutOfRangeException(nameof(value))
        };
}