# Get-ChildItem -Path NakCore:/Translations -Exclude *.g.xlf
# | Get-XliffTranslation
# | Select-Object -First 10000 -Property RawContext, Context, @{n = 'RawCount'; e = { ($_.RawContext.ToCharArray() | Where-Object { $_ -eq '-' }).length }}, @{ n = 'normalcount'; e = { ($_.Context).Count } }
# | Where-Object { ($_.RawCount + 1) -ne $_.NormalCount }

$Translations = @(
    (New-XliffTranslation -TargetLanguage nl-NL -Target 'Mijn veld' -RawContext 'Table My Table - Field My Field - Property Caption'),
    (New-XliffTranslation -TargetLanguage nl-NL -Target 'Mijn andere veld' -RawContext 'Table My Table - Field My Other Field - Property Caption')
    (New-XliffTranslation -TargetLanguage nl-NL -Target 'Reeds vertaald veldje' -RawContext 'Table My Table - Field Already TranslatedField - Property Caption')
    (New-XliffTranslation -TargetLanguage nl-NL -Target 'Vertaald in andere taal' -RawContext 'Table My Table - Field Translated in Other Language - Property Caption')
)

$Translations | Set-XliffTranslationAsBcDevComment -ObjectPath ./test.al -Recurse -Verbose

New-XliffTranslation -TargetLanguage nl-NL -Target 'Forceer hervertaling!!!' -RawContext 'Table My Table - Field Force Retranslate Field - Property Caption' |
    Set-XliffTranslationAsBcDevComment -ObjectPath ./test.al -Force -Recurse -Verbose

# Note that the table name contains ' - ', which is also the element separator in the context string
# New-XliffTranslation -TargetLanguage nl-NL -Target 'My Translation' -RawContext 'Table SP - Appl. Multi-drop Exc. - Field Customer Id - Property Caption'
# | Set-XliffTranslationAsBcDevComment -ObjectPath NakCore:/ -Recurse