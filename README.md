# OneDrive Powershell Module
PowerShell Module to access OneDrive for Business using the Microsoft Graph API.

## Connect/Disconnect

### Connect using Azure AD Application Identity

* An application identity has access to all files of all users!

```powershell
Connect-OneDrive -ApplicationId $appId -ApplicationSecret $appSecret -TenantId $tenantId
```

### Disconnect

```powershell
Disconnect-OneDrive
```

## Get files inside a folder of a specific users OneDrive

```powershell
Get-OneDriveChildItem -Path "Documents/Images" -UserPrincipalName "jane.doe@contoso.com"
```

## Download files

```powershell
$itemsToDownload | Invoke-OneDriveItemDownload -Destination "C:\Temp"
```

## Delete files

```powershell
$itemsToDelete | Remove-OneDriveItem
```
