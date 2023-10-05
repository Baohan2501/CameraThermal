c:
cd C:\Windows\system32
SET CurrentDir="%~dp0WindowsCameraService.exe"
sc.exe create WindowCameraService binPath=%CurrentDir%
sc.exe description WindowCameraService "Connect to thermal camera"
sc.exe config WindowCameraService start= "auto"
sc start WindowCameraService
