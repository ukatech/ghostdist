# GhostDist Manual Build Script (PowerShell)
# This script builds GhostDist in Release mode and creates a distribution ZIP

Write-Host "================================" -ForegroundColor Cyan
Write-Host "GhostDist Manual Build Script" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Check if NuGet is available
$nugetPath = Get-Command nuget -ErrorAction SilentlyContinue
if (-not $nugetPath) {
    Write-Host "ERROR: nuget.exe not found in PATH" -ForegroundColor Red
    Write-Host "Please download from: https://www.nuget.org/downloads" -ForegroundColor Yellow
    exit 1
}

# Find MSBuild
$msbuildPath = Get-Command msbuild -ErrorAction SilentlyContinue
if (-not $msbuildPath) {
    # Try to find MSBuild in standard locations
    $vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vswhere) {
        $msbuildPath = & $vswhere -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1
        if ($msbuildPath) {
            Write-Host "Found MSBuild at: $msbuildPath" -ForegroundColor Green
        }
    }

    if (-not $msbuildPath) {
        Write-Host "ERROR: MSBuild not found" -ForegroundColor Red
        Write-Host "Please run this script from 'Developer PowerShell for VS'" -ForegroundColor Yellow
        Write-Host "Or install Visual Studio Build Tools" -ForegroundColor Yellow
        exit 1
    }
} else {
    $msbuildPath = "msbuild"
}

# Step 1: Restore NuGet packages
Write-Host "[1/4] Restoring NuGet packages..." -ForegroundColor Yellow
& nuget restore GhostDist.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: NuGet restore failed" -ForegroundColor Red
    exit 1
}

# Step 2: Build Release version
Write-Host ""
Write-Host "[2/4] Building Release version..." -ForegroundColor Yellow
& $msbuildPath GhostDist.sln /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=bin\Release\ /v:minimal
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Build failed" -ForegroundColor Red
    exit 1
}

# Step 3: Prepare release files
Write-Host ""
Write-Host "[3/4] Preparing release files..." -ForegroundColor Yellow
if (-not (Test-Path "release")) {
    New-Item -ItemType Directory -Path "release" | Out-Null
}
Copy-Item "GhostDist\bin\Release\GhostDist.exe" -Destination "release\" -Force
Copy-Item "readme_for_deploy.txt" -Destination "release\readme.txt" -Force

# Step 4: Create ZIP archive
Write-Host ""
Write-Host "[4/4] Creating ZIP archive..." -ForegroundColor Yellow
Compress-Archive -Force -Path "release\GhostDist.exe","release\readme.txt" -DestinationPath "release\ghostdist.zip"
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: ZIP creation failed" -ForegroundColor Red
    exit 1
}

# Success
Write-Host ""
Write-Host "================================" -ForegroundColor Green
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green
Write-Host "Output: release\ghostdist.zip" -ForegroundColor Cyan
Write-Host ""

# Show file info
$zipFile = Get-Item "release\ghostdist.zip"
Write-Host "File size: $([math]::Round($zipFile.Length / 1KB, 2)) KB" -ForegroundColor Cyan
