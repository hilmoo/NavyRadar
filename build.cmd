rmdir /s /q "./publish"

@echo off
echo =================================
echo Building WPF Frontend...
echo =================================
dotnet publish NavyRadar.Frontend -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesInSingleFile=true /p:DebugType=none -o ./publish/Frontend

echo.
echo =================================
echo Building MVC Backend...
echo =================================
dotnet publish NavyRadar.Backend -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesInSingleFile=true /p:DebugType=none -o ./publish/Backend
COPY "NavyRadar.Backend\.env.example" "publish\Backend\.env.example"

echo.
echo =================================
echo Copy Migration Files...
echo =================================
MKDIR "publish\Migration"
XCOPY "NavyRadar.Shared\Migration\*" "publish\Migration\" /Y

echo.
echo =================================
echo Build complete!
echo =================================
echo Your executables are located in the './publish' directory.
echo.