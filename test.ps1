# $ErrorActionPreference = 'Stop'
# $VerbosePreference = 'Continue'

# Get-XliffTranslation -Path './test/Translations/test.nl-NL.xlf'
# | Set-XliffTranslationAsBcDevComment -ObjectPath ./test -Recurse -PassThru
# | Remove-XliffTranslation

Test-BcDevComment -ObjectPath ./test/test.al -ExpectedLanguage 'nl-NL', 'fr-FR' -DevCommentSeparator '|'