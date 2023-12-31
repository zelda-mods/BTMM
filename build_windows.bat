@echo off
set "ProjectPath=%~dp0BTMM\BTMM.csproj"
set "ReleasePath=bin\Release"

echo clear %ReleasePath%...
rd /s /q "%~dp0%ReleasePath%"

set "TargetPlatforms=win-x86 win-x64 win-arm64"
for %%p in (%TargetPlatforms%) do (
    echo.
    echo build %%p...
    dotnet publish %ProjectPath% -c Release -o bin\Release\%%p\ --runtime %%p --self-contained true --framework net6.0 -p:PublishSingleFile=true -p:TrimMode=false
    echo build %%p success
)
echo.
echo all build success
pause
