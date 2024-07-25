Get-ChildItem -Path NakCore:/Translations -Exclude *.g.xlf | Get-XliffTranslation | Where-Object { $_.Context.Length -ne (($_.RawContext.ToCharArray() -eq '-').Count + 1) }

# New-XliffTranslation -TargetLanguage nl-NL -Target 'My Translation' -RawContext 'Table Accorderingsroute - Field Accorderingsroute - Property Caption'
# | Set-XliffTranslationAsBcDevComment -ObjectPath NakCore:/ -Recurse

# # Note that the table name contains ' - ', which is also the element separator in the context string
# New-XliffTranslation -TargetLanguage nl-NL -Target 'My Translation' -RawContext 'Table SP - Appl. Multi-drop Exc. - Field Customer Id - Property Caption'
# | Set-XliffTranslationAsBcDevComment -ObjectPath NakCore:/ -Recurse