# OneDrive Powershell Module
PowerShell Module to access OneDrive for Business using the Microsoft Graph API.

## Connect/Disconnect

### Connect using Azure AD Application Identity

⚠ An Azure AD Application Identity has access to all files of all users inside that Azure AD Tenant.

Required application permissions:
* Read-only commands: Microsoft Graph API / Files.Read.All
* Read & write commands: Microsoft Graph API / Files.ReadWrite.All

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

## Move files

```powershell
$itemsToMove | Move-OneDriveItem -Destination "Documents/ImageArchive"
```

## Download files

The following command downloads the files to C:\Temp but writes an error for each file that already exists:

```powershell
$itemsToDownload | Invoke-OneDriveItemDownload -Destination "C:\Temp"
```

The following command downloads the files to C:\Temp and overwrites existing files:

```powershell
$itemsToDownload | Invoke-OneDriveItemDownload -Destination "C:\Temp" -Force
```

## Delete files

```powershell
$itemsToDelete | Remove-OneDriveItem
```
