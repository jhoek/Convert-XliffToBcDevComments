$ErrorActionPreference = 'Stop'
$VerbosePreference = 'Continue'

# Get-XliffTranslation -Path './test/Translations/test.nl-NL.xlf'
# | Set-XliffTranslationAsBcDevComment -ObjectPath ./test -Recurse -PassThru
# | Remove-XliffTranslation

Get-XliffTranslation -Path 'NakCore:/Translations/Naktuinbouw Extension.nl-NL.xlf'
| Set-XliffTranslationAsBcDevComment -ObjectPath NakCore:/src/enum/AddressSanatizationStatus.enum.al -PassThru
| Remove-XliffTranslation