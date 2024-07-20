@ECHO OFF

SET OUTDIR=build
mkdir %OUTDIR%

nuget pack messagingtoolkit.barcode.nuspec -outputdirectory %OUTDIR% -NoDefaultExcludes