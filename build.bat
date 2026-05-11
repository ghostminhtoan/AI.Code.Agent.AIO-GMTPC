rem AI Summary: 2026-04-30 - Standardized build script to Release Any CPU and copy AI.Code.Agent.AIO-GMTPC.exe to root before git push
@echo off
echo ========================================
echo Building AI.Code.Agent.AIO-GMTPC (Release Any CPU)
echo ========================================
echo.

cd /d "%~dp0"

set "MSBUILD="
if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
)

if "%MSBUILD%"=="" (
    echo [ERROR] MSBuild not found! Please install Visual Studio or Build Tools.
    pause
    exit /b 1
)

echo Using: %MSBUILD%
echo.

"%MSBUILD%" AI.Code.Agent.AIO-GMTPC.csproj /p:Configuration=Release /p:Platform="Any CPU" /nologo /v:minimal
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Build failed!
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ========================================
echo Build successful!
echo ========================================
echo.

echo Copying exe to root folder...
copy /Y "bin\Release\net48\AI.Code.Agent.AIO-GMTPC.exe" "AI.Code.Agent.AIO-GMTPC.exe" >nul
if %ERRORLEVEL% NEQ 0 (
    echo [WARNING] Failed to copy exe to root folder.
    goto end
) else (
    echo [OK] Exe copied to root: AI.Code.Agent.AIO-GMTPC.exe
)
echo.

echo Adding to git...
git add .
echo Enter commit message:
set /p COMMIT_MSG=
git commit -m "%COMMIT_MSG%"
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Commit failed!
    pause
    exit /b %ERRORLEVEL%
)
for /f %%i in ('git branch --show-current') do set CURRENT_BRANCH=%%i
echo Rebasing with remote...
git pull --rebase origin %CURRENT_BRANCH%
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Rebase failed! Please resolve conflicts manually.
    pause
    exit /b %ERRORLEVEL%
)
echo Pushing to remote...
git push
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Push failed!
    pause
    exit /b %ERRORLEVEL%
)
echo Done!
pause

