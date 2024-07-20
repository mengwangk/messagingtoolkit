@ECHO OFF

SET OUTDIR=build
mkdir %OUTDIR%

nuget pack messagingtoolkit.nuspec -outputdirectory %OUTDIR% -NoDefaultExcludes