powershell -ExecutionPolicy Bypass -File ./DownloadCertificate.ps1
certutil -addstore "Root" .\RootCA.cer
certutil -importpfx -f -p "" client.pfx

