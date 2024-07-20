Installation of the Barcode Scanning Service
============================================

1. Go to Visual Studio Command Prompt as Windows Administrator. (Alternatively you can find InstallUtil under \Windows\Microsoft.NET\Framework\v4.0.30319, 
depending on the version of your .NET framework).

2. Change directory to <Library Installation Folder>\Demo Clients\Windows Service

3. Run "installutil messagingtoolkit.barcode.service.demo.exe"

4. The Windows Service "ScanService" should be installed

5. Open "messagingtoolkit.barcode.service.demo.exe.config" and modify the watch path. Default is "c:\temp\";

6. Start the service and it should scan and try to decode files added to the watch path.

7. The decoded result is generated in a text file with ".txt" extension and original file name.