@echo off
set msbuild="%windir%\Microsoft.NET\Framework\v3.5\MSBuild.exe"
%msbuild% default.build /v:n /t:GenerateAssemblyInfo
echo -------------------------------