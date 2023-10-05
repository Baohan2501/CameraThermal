c:
cd C:\Windows\system32
SET CurrentDir="%~dp0WindowsCameraService.exe"
sc.exe create WindowThermalCameraService binPath=%CurrentDir%
sc.exe description WindowThermalCameraService "Connect to thermal camera"
sc.exe config WindowThermalCameraService start= "auto"
sc start WindowThermalCameraService
