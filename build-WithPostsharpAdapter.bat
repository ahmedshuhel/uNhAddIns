@echo off
SET sn="%programfiles%\Microsoft SDKs\Windows\V6.0A\Bin\sn.exe"
set gacutil="%programfiles%\Microsoft SDKs\Windows\V6.0A\Bin\gacutil.exe"
set msbuild="%windir%\Microsoft.NET\Framework\v3.5\MSBuild.exe"
REM set lib="\lib"
REM set configuration=Debug
REM if exist commonfiles\uNhAddIns.snk goto build
REM echo Generating strong key...
REM %sn% -k commonfiles\uNhAddIns.snk
:build
%msbuild% default.build /v:n /t:Build /property:IncludePostSharpAdapter=True
:end
echo -------------------------------