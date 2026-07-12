$ErrorActionPreference = "Stop"

$targetDir = "$HOME\.local\bin"
$binaryPath = "$targetDir\jartisan.exe"

if (-not (Test-Path $targetDir)) { 
    New-Item -ItemType Directory -Path $targetDir | Out-Null 
}

$url = "https://github.com/guiszlima/jartisan-cli/releases/latest/download/jartisan-windows-x64.zip"

Write-Host "📥 Downloading Jartisan for Windows (x64)..." -ForegroundColor Cyan

$zipPath = "$env:TEMP\jartisan.zip"
Invoke-WebRequest -Uri $url -OutFile $zipPath
Expand-Archive -Path $zipPath -DestinationPath $targetDir -Force
Remove-Item $zipPath

Write-Host "🔧 Configuring environment and aliases (jartisan & jart)..." -ForegroundColor Cyan

$batJartisanContent = "@echo off`n`""$binaryPath`"" %*"
Set-Content -Path "$targetDir\jartisan.bat" -Value $batJartisanContent -Force

$batJartContent = "@echo off`n`""$binaryPath`"" %*"
Set-Content -Path "$targetDir\jart.bat" -Value $batJartContent -Force

$currentPathEnv = [Environment]::GetEnvironmentVariable("Path", "User")
if ($currentPathEnv -notlike "*$targetDir*") {
    $newPathEnv = if ([string]::IsNullOrEmpty($currentPathEnv)) { $targetDir } else { $currentPathEnv.TrimEnd(';') + ";$targetDir" }
    [Environment]::SetEnvironmentVariable("Path", $newPathEnv, "User")
}

Write-Host "`n✨ Jartisan successfully installed to $binaryPath!" -ForegroundColor Green
Write-Host "👉 Please CLOSE and REOPEN your terminal/PowerShell window to refresh environment variables." -ForegroundColor Yellow
Write-Host "🚀 You can then use either 'jartisan' or 'jart' commands anywhere!" -ForegroundColor Green
