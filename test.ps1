# Get-ChildItem -Path NakCore:/Translations -Exclude *.g.xlf
# | Get-XliffTranslation
# | Select-Object -First 10000 -Property Context, Context, @{n = 'RawCount'; e = { ($_.Context.ToCharArray() | Where-Object { $_ -eq '-' }).length }}, @{ n = 'normalcount'; e = { ($_.Context).Count } }
# | Where-Object { ($_.RawCount + 1) -ne $_.NormalCount }

# $Translations = @(
#     (New-XliffTranslation -TargetLanguage nl-NL -Target 'Mijn veld' -Context 'Table My Table - Field My Field - Property Caption'),
#     (New-XliffTranslation -TargetLanguage nl-NL -Target 'Mijn andere veld' -Context 'Table My Table - Field My Other Field - Property Caption')
#     (New-XliffTranslation -TargetLanguage nl-NL -Target 'Reeds vertaald veldje' -Context 'Table My Table - Field Already TranslatedField - Property Caption')
#     (New-XliffTranslation -TargetLanguage nl-NL -Target 'Vertaald in andere taal' -Context 'Table My Table - Field Translated in Other Language - Property Caption')
# )

# $Translations | Set-XliffTranslationAsBcDevComment -ObjectPath ./test.al -Recurse -Verbose

''

# New-XliffTranslation -TargetLanguage nl-NL -Target 'Forceer hervertaling!!!' -Context 'Table My Table - Field Force Retranslate Field - Property Caption' |
#     Set-XliffTranslationAsBcDevComment -ObjectPath ./test.al -Recurse -Verbose -Force -PassThru


Get-XliffTranslation -Path 'NakCore:/Translations/Naktuinbouw Extension.nl-NL.xlf'
| Set-XliffTranslationAsBcDevComment -ObjectPath NakCore:/src/table/ApplicationCountry.table.al, NakCore:/src/table/ActiezinnenPerSoort.table.al -Recurse -PassThru
| Remove-XliffTranslation -Verbose

# Note that the table name contains ' - ', which is also the element separator in the context string
# New-XliffTranslation -TargetLanguage nl-NL -Target 'My Translation' -Context 'Table SP - Appl. Multi-drop Exc. - Field Customer Id - Property Caption'
# | Set-XliffTranslationAsBcDevComment -ObjectPath NakCore:/ -Recurse