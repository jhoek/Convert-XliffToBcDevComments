# Get-ChildItem -Path NakCore:/Translations -Exclude *.g.xlf | Get-XliffTranslation | Select-Object -ExpandProperty RawContext

# New-XliffTranslation -TargetLanguage nl-NL -Target 'My Translation' -RawContext 'Table Accorderingsroute - Field Accorderingsroute - Property Caption'
# | Set-XliffTranslationAsBcDevComment -ObjectPath NakCore:/ -Recurse

# # Note that the table name contains ' - ', which is also the element separator in the context string
# New-XliffTranslation -TargetLanguage nl-NL -Target 'My Translation' -RawContext 'Table SP - Appl. Multi-drop Exc. - Field Customer Id - Property Caption'
# | Set-XliffTranslationAsBcDevComment -ObjectPath NakCore:/ -Recurse

New-XliffTranslation -TargetLanguage nl-NL -Target 'My Translation' -RawContext 'Table Accorderingsroute - Field Accorderingsroute - Property Caption' | Select-Object -ExpandProperty Context

New-XliffTranslation -TargetLanguage nl-NL -Target 'My Translation' -RawContext 'Table SP - Appl. Multi-drop Exc. - Field Customer Id - Property Caption' | Select-Object -ExpandProperty Context