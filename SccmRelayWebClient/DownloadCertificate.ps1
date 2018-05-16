
$hostName = "[[your_hostname_goes_here]]"
Invoke-WebRequest -UseBasicParsing $hostName/api/Certificate -ContentType "application/json" -Method POST -Body "{ 'hostName': '$env:computername' }" -OutFile client.pfx