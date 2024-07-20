@ECHO OFF

rem xcopy /y "MessagingToolkit.chm" "lib\"
rem xcopy /y "Binaries\.net 4.6.2\any cpu\*.dll" "lib/net462/*.dll" 
rem xcopy /y "Binaries\.net 4.6.2\x64\*.dll" "lib/net462/x64/*.dll" 
rem xcopy /y "Binaries\.net 4.6\any cpu\*.dll" "lib/net46/*.dll" 
rem xcopy /y "Binaries\.net 4.6\x64\*.dll" "lib/net46/x64/*.dll" 
rem xcopy /y "Binaries\.net 4.5\any cpu\*.dll" "lib/net45/*.dll" 
rem xcopy /y "Binaries\.net 4.5\x64\*.dll" "lib/net45/x64/*.dll" 
rem xcopy /y "Binaries\.net 4.0\any cpu\*.dll" "lib/net40/*.dll" 
rem xcopy /y "Binaries\.net 4.0\x64\*.dll" "lib/net40/x64/*.dll" 
rem xcopy /y "MessagingToolkit.Core.xml" "lib/net40/MessagingToolkit.Core.xml" 
rem xcopy /y "MessagingToolkit.Core.xml" "lib/net45/MessagingToolkit.Core.xml" 
rem xcopy /y "MessagingToolkit.Core.xml" "lib/net46/MessagingToolkit.Core.xml" 
rem xcopy /y "MessagingToolkit.Core.xml" "lib/net462/MessagingToolkit.Core.xml" 

SET OUTDIR=build
mkdir %OUTDIR%

nuget pack messagingtoolkit.nuspec -outputdirectory %OUTDIR% 