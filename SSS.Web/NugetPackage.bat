call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsDevCmd.bat"
msbuild "C:\Users\sam.nesbitt\source\repos\SSSWeb\SSSWeb\SSSWeb.csproj" /t:pack /p:Configuration=Release

pause