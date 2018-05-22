
$hostName = "[[your_hostname_goes_here]]"

# SCENARIO 2: use Random hostname
Invoke-WebRequest -UseBasicParsing $hostName/api/Certificate -ContentType "application/json" -Method POST -Body "{ 'generaterandom': 'true' }" -OutFile client.pfx
