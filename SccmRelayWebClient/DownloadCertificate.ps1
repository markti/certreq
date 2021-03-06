
# SCENARIO 1: use ACTUAL hostname
$hostName = "[[your_hostname_goes_here]]"
Invoke-WebRequest -UseBasicParsing $hostName/api/Certificate -ContentType "application/json" -Method POST -Body "{ 'hostName': '$env:computername', 'secretKey': 'foo' }" -OutFile client.pfx