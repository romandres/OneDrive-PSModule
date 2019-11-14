# OneDrive Powershell Module
PowerShell Module to access OneDrive for Business using the Microsoft Graph API.

## Connect/Disconnect

### Connect using Azure AD Application Identity

```powershell
Connect-OneDrive -ApplicationId $appId -ApplicationSecret $appSecret -TenantId $tenantId
```

### Disconnect

```powershell
Disconnect-OneDrive
```

## Get files inside a folder

```powershell
Get-OneDriveChildItem -Path "Documents/Images" -UserPrincipalName "jane.doe@contoso.com"
```

## Download files

```powershell
$itemsToDownload | Invoke-OneDriveItemDownload -Destination $destinationPath
```

## Delete files

```powershell
$itemsToDelete | Remove-OneDriveItem
```
