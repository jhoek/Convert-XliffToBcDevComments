# Convert-XliffToBcDevComments

PowerShell cmdlet for converting XLIFF translations to AL developer comments

## Index

| Command | Synopsis |
| ------- | -------- |
| [Get-XliffTranslation](#Get-XliffTranslation) | Retrieves the translations from one or more XLIFF files. |
| [New-XliffTranslation](#New-XliffTranslation) | Creates a user-defined translation entry. |
| [Remove-XliffTranslation](#Remove-XliffTranslation) | Removes translations from their respective translation files. |
| [Set-XliffTranslationAsBcDevComment](#Set-XliffTranslationAsBcDevComment) | Adds translations to AL objects (as developer comments in label properties). |

<a name="Get-XliffTranslation"></a>
## Get-XliffTranslation
### Synopsis
Retrieves the translations from one or more XLIFF files.
### Description
Retrieves the translations from one or more XLIFF files.

### Syntax
```powershell
Get-XliffTranslation [-Path] <string[]> [-Recurse] [<CommonParameters>]
```
### Parameters
#### Path &lt;String[]&gt;
    One or more XLIFF files to process. May also contain one or more directories to search for XLIFF files.
    
    Required?                    true
    Position?                    1
    Default value                
    Accept pipeline input?       true (ByValue, ByPropertyName)
    Aliases                      
    Accept wildcard characters?  false
#### Recurse [&lt;SwitchParameter&gt;]
    If -Path contains directories, specifies whether to also search subdirectories.
    
    Required?                    false
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
<a name="New-XliffTranslation"></a>
## New-XliffTranslation
### Synopsis
Creates a user-defined translation entry.
### Description
Creates a user-defined translation entry.

### Syntax
```powershell
New-XliffTranslation -TargetLanguage <string> -ID <string> -Target <string> -Context <string> [-XliffPath <string>] [-SourceLanguage <string>] [-Source <string>] [-TargetState <TranslationState>] [<CommonParameters>]
```
### Parameters
#### XliffPath [&lt;String&gt;]
    Path to the XLIFF file
    
    Required?                    false
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
#### SourceLanguage [&lt;String&gt;]
    Source language for the translation; defaults to &#39;en-US&#39;
    
    Required?                    false
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
#### TargetLanguage &lt;String&gt;
    Target language for the translation
    
    Required?                    true
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
#### ID &lt;String&gt;
    Unique ID for the translation
    
    Required?                    true
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
#### Source [&lt;String&gt;]
    Source text for the translation
    
    Required?                    false
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
#### Target &lt;String&gt;
    Target text for the translation
    
    Required?                    true
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
#### TargetState [&lt;Final|NeedsAdaptation|NeedsLocalization|NeedsReviewAdaptation|NeedsReviewLocalization|NeedsReviewTranslation|NeedsTranslation|New|SignedOff|Translated&gt;]
    State of the translation target text
    
    Required?                    false
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
#### Context &lt;String&gt;
    Context for the translation
    
    Required?                    true
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
<a name="Remove-XliffTranslation"></a>
## Remove-XliffTranslation
### Synopsis
Removes translations from their respective translation files.
### Description
Removes translations from their respective translation files.

### Syntax
```powershell
Remove-XliffTranslation [-InputObject] <XliffTranslation[]> [<CommonParameters>]
```
### Parameters
#### InputObject &lt;XliffTranslation[]&gt;
    Translation to remove
    
    Required?                    true
    Position?                    1
    Default value                
    Accept pipeline input?       true (ByValue)
    Aliases                      
    Accept wildcard characters?  false
<a name="Set-XliffTranslationAsBcDevComment"></a>
## Set-XliffTranslationAsBcDevComment
### Synopsis
Adds translations to AL objects (as developer comments in label properties).
### Description
Adds translations to AL objects (as developer comments in label properties).

### Syntax
```powershell
Set-XliffTranslationAsBcDevComment [-ObjectPath] <string[]> -Translations <XliffTranslation[]> [-Recurse] [-StateToProcess <TranslationState[]>] [-StateToEmit <TranslationState[]>] [-Force] [-PassThru] [<CommonParameters>]
```
### Parameters
#### ObjectPath &lt;String[]&gt;
    Path of the AL object file to update. May also contain one or more directories to search for AL objects.
    
    Required?                    true
    Position?                    1
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
#### Recurse [&lt;SwitchParameter&gt;]
    If -Path contains directories, specifies whether to also search subdirectories.
    
    Required?                    false
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
#### Translations &lt;XliffTranslation[]&gt;
    One or more descriptions as retrieved using Get-XliffTranslation or created using New-XliffTranslation
    
    Required?                    true
    Position?                    named
    Default value                
    Accept pipeline input?       true (ByValue)
    Aliases                      
    Accept wildcard characters?  false
#### StateToProcess [&lt;TranslationState[]&gt;]
    Translation target states to convert to AL developer comments; other states will be ignored
    
    Required?                    false
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
#### StateToEmit [&lt;TranslationState[]&gt;]
    Translation target states that will be emitted after processing.
    
    Required?                    false
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
#### Force [&lt;SwitchParameter&gt;]
    Overwrite developer comments if they already exist
    
    Required?                    false
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
#### PassThru [&lt;SwitchParameter&gt;]
    Output the translations that match the -StateToEmit filter to the PowerShell pipeline
    
    Required?                    false
    Position?                    named
    Default value                
    Accept pipeline input?       false
    Aliases                      
    Accept wildcard characters?  false
<div style='font-size:small'>License: https://github.com/jhoek/Convert-XliffToBcDevComments?tab=MIT-1-ov-file</div>
<div style='font-size:small'>(c) jhoek. All rights reserved.</div>
<div style='font-size:small; color: #ccc'>Generated 21-06-2025 10:20</div>
