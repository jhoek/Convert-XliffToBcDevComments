Get-ChildItem -Path NakCore:/Translations -Exclude *.g.xlf | Get-XliffTranslation | Select-Object -First 100 -Property RawContext, Context, @{ n = 'normalcount'; e = { ($_.Context).Count } }, @{n = 'RawCount'; e = { ($_.RawContext.ToCharArray() | Where-Object { $_ -eq '-' }).length } } | Where-Object { $_.RawCount + 1 -ne $_.NormalCount }

# New-XliffTranslation -TargetLanguage nl-NL -Target 'My Translation' -RawContext 'Table Accorderingsroute - Field Accorderingsroute - Property Caption'
# | Set-XliffTranslationAsBcDevComment -ObjectPath NakCore:/ -Recurse

# # Note that the table name contains ' - ', which is also the element separator in the context string
# New-XliffTranslation -TargetLanguage nl-NL -Target 'My Translation' -RawContext 'Table SP - Appl. Multi-drop Exc. - Field Customer Id - Property Caption'
# | Set-XliffTranslationAsBcDevComment -ObjectPath NakCore:/ -Recurse