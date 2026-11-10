@echo off
rem Builds the extension and registers it from its build folder (Developer Mode),
rem the way Visual Studio's Deploy does, then restarts Command Palette so it picks the new build up.
cd /d %~dp0
rem The running extension holds its exe open; Command Palette starts it again on next use
taskkill /im TerminalTabs.exe /f >nul 2>&1
set "MSB=C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"
"%MSB%" TerminalTabs.sln /restore /p:Configuration=Release /p:Platform=x64 /m /nologo /v:m /clp:ErrorsOnly;Summary
if errorlevel 1 (echo BUILD-FAILED & exit /b 1)
echo BUILD-OK
powershell -NoProfile -Command "Add-AppxPackage -Register '%~dp0TerminalTabs\bin\x64\Release\net10.0-windows10.0.26100.0\win-x64\AppxManifest.xml' -ForceUpdateFromAnyVersion" || (echo DEPLOY-FAILED & exit /b 1)
rem Command Palette stays attached to the extension the kill above stopped, so # finds nothing
rem until it restarts: restart it here
powershell -NoProfile -Command "Stop-Process -Name Microsoft.CmdPal.UI -Force -ErrorAction SilentlyContinue; Start-Sleep 1; Start-Process 'shell:AppsFolder\Microsoft.CommandPalette_8wekyb3d8bbwe!App'"
echo DEPLOY-OK
