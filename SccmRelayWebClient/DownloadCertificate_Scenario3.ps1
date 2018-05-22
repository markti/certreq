
$hostName = "[[your_hostname_goes_here]]"

# SCENARIO 3: use a common (non-unique) hostname (i.e. use the same certificate for EVERY machine)
Invoke-WebRequest -UseBasicParsing $hostName/api/Certificate -ContentType "application/json" -Method POST -Body "{ }" -OutFile client.pfx
