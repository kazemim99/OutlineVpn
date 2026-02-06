# V2Ray Application - Automated Deployment Script
# This script will build locally and deploy to the Ubuntu server

param(
    [string]$ServerIP = "109.94.164.46",
    [string]$Username = "root",
    [string]$Password = "!Q@W3e4r",
    [string]$Domain = "iransshvpn.com"
)

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "V2Ray Application - Automated Deployment" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Check if plink (PuTTY) or ssh is available
$sshAvailable = Get-Command ssh -ErrorAction SilentlyContinue
$plinkAvailable = Get-Command plink -ErrorAction SilentlyContinue

if (-not $sshAvailable -and -not $plinkAvailable) {
    Write-Host "ERROR: Neither SSH nor PuTTY (plink) found!" -ForegroundColor Red
    Write-Host "Please install one of the following:" -ForegroundColor Yellow
    Write-Host "1. Git for Windows (includes SSH)" -ForegroundColor Yellow
    Write-Host "2. PuTTY (https://www.putty.org/)" -ForegroundColor Yellow
    Write-Host "3. OpenSSH Client (Windows Feature)" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Or use manual deployment with WinSCP/FileZilla" -ForegroundColor Yellow
    pause
    exit 1
}

Write-Host "[1/5] Building Vue.js Frontend..." -ForegroundColor Yellow
Set-Location "$PSScriptRoot\vue\v2ray"
npm install
npm run build

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Vue.js build failed!" -ForegroundColor Red
    pause
    exit 1
}
Write-Host "Vue.js build completed!" -ForegroundColor Green
Write-Host ""

Write-Host "[2/5] Creating deployment package..." -ForegroundColor Yellow
Set-Location $PSScriptRoot
$deployDir = Join-Path $PSScriptRoot "deploy-package"

if (Test-Path $deployDir) {
    Remove-Item $deployDir -Recurse -Force
}
New-Item -ItemType Directory -Path $deployDir | Out-Null

# Copy files
Copy-Item -Path "V2Ray.Api" -Destination "$deployDir\V2Ray.Api" -Recurse -Force
Copy-Item -Path "nginx" -Destination "$deployDir\nginx" -Recurse -Force
Copy-Item -Path "docker-compose.yml" -Destination $deployDir -Force
Copy-Item -Path "deploy.sh" -Destination $deployDir -Force
Copy-Item -Path "DEPLOYMENT.md" -Destination $deployDir -Force

Write-Host "Deployment package created!" -ForegroundColor Green
Write-Host ""

Write-Host "[3/5] Transferring files to server..." -ForegroundColor Yellow
Write-Host "This may take a few minutes..." -ForegroundColor Yellow

# Create a batch file for WinSCP scripting
$wscpScript = @"
option batch abort
option confirm off
open sftp://${Username}:${Password}@${ServerIP}/
mkdir /var/www/v2ray
lcd "$deployDir"
cd /var/www/v2ray
put -delete *
exit
"@

$wscpScriptFile = Join-Path $PSScriptRoot "winscp-script.txt"
$wscpScript | Out-File -FilePath $wscpScriptFile -Encoding ASCII

Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "MANUAL TRANSFER REQUIRED" -ForegroundColor Yellow
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Automated SSH transfer requires additional setup." -ForegroundColor Yellow
Write-Host "Please use one of these methods to transfer files:" -ForegroundColor White
Write-Host ""
Write-Host "Option 1: WinSCP (Recommended)" -ForegroundColor Green
Write-Host "1. Download WinSCP from: https://winscp.net/" -ForegroundColor White
Write-Host "2. Connect to:" -ForegroundColor White
Write-Host "   Host: $ServerIP" -ForegroundColor Cyan
Write-Host "   Username: $Username" -ForegroundColor Cyan
Write-Host "   Password: $Password" -ForegroundColor Cyan
Write-Host "3. Upload contents of:" -ForegroundColor White
Write-Host "   $deployDir" -ForegroundColor Cyan
Write-Host "   to: /var/www/v2ray/" -ForegroundColor Cyan
Write-Host ""
Write-Host "Option 2: Git Bash / WSL" -ForegroundColor Green
Write-Host "Run this command:" -ForegroundColor White
Write-Host "scp -r `"$deployDir`"/* root@${ServerIP}:/var/www/v2ray/" -ForegroundColor Cyan
Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host "After transferring files, SSH to the server:" -ForegroundColor Yellow
Write-Host "ssh root@$ServerIP" -ForegroundColor Cyan
Write-Host ""
Write-Host "Then run these commands on the server:" -ForegroundColor Yellow
Write-Host "cd /var/www/v2ray" -ForegroundColor Cyan
Write-Host "chmod +x deploy.sh" -ForegroundColor Cyan
Write-Host "./deploy.sh" -ForegroundColor Cyan
Write-Host "docker-compose up -d" -ForegroundColor Cyan
Write-Host ""
Write-Host "For SSL certificate:" -ForegroundColor Yellow
Write-Host "docker-compose run --rm certbot certonly --webroot --webroot-path /var/www/certbot --email your-email@example.com --agree-tos --no-eff-email -d $Domain -d www.$Domain" -ForegroundColor Cyan
Write-Host ""
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Deployment package ready at:" -ForegroundColor Green
Write-Host "$deployDir" -ForegroundColor Cyan
Write-Host ""

# Clean up
Remove-Item $wscpScriptFile -Force -ErrorAction SilentlyContinue

pause
