@echo off
REM GhostDist Manual Build Script
REM This script builds GhostDist in Release mode and creates a distribution ZIP

echo ================================
echo GhostDist Manual Build Script
echo ================================
echo.

REM Check if NuGet is available
where nuget >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ERROR: nuget.exe not found in PATH
    echo Please download from: https://www.nuget.org/downloads
    pause
    exit /b 1
)

REM Check if MSBuild is available
where msbuild >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo ERROR: MSBuild not found in PATH
    echo Please run this script from "Developer Command Prompt for VS"
    echo Or add MSBuild to PATH manually
    pause
    exit /b 1
)

echo [1/4] Restoring NuGet packages...
nuget restore GhostDist.sln
if %ERRORLEVEL% neq 0 (
    echo ERROR: NuGet restore failed
    pause
    exit /b 1
)

echo.
echo [2/4] Building Release version...
msbuild GhostDist.sln /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=bin\Release\ /v:minimal
if %ERRORLEVEL% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo [3/4] Preparing release files...
if not exist release mkdir release
copy /Y GhostDist\bin\Release\GhostDist.exe release\ >nul
copy /Y readme_for_deploy.txt release\readme.txt >nul

echo.
echo [4/4] Creating ZIP archive...
powershell -Command "Compress-Archive -Force -Path release\GhostDist.exe,release\readme.txt -DestinationPath release\ghostdist.zip"
if %ERRORLEVEL% neq 0 (
    echo ERROR: ZIP creation failed
    pause
    exit /b 1
)

echo.
echo ================================
echo Build completed successfully!
echo ================================
echo Output: release\ghostdist.zip
echo.
pause
