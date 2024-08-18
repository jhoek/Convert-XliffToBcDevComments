Get-XliffTranslation -Path 'NakCore:/Translations/Naktuinbouw Extension.nl-NL.xlf'
| Set-XliffTranslationAsBcDevComment -ObjectPath NakCore:/src -Recurse -PassThru
| Remove-XliffTranslation -Verbose