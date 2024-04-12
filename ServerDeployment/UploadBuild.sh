#! /bin/bash
source ../.env
#increase version number
ex ../.env  << EOF
/^version=/
c
version=$((version + 1))
.
wq
EOF
localFiles="../.env $config_path ./RemoteScripts/*"
if [ "$1" != "-onlyconfig" ]; then
localFiles+=" ../Build"
fi
scp -r $localFiles $remote_server_user@$remote_server_address:$remote_path
