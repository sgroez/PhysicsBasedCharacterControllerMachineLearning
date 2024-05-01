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
localFiles="../.env $local_config_path/$config_name ./RemoteScripts/*"
if [ "$1" != "-onlyconfig" ]; then
localFiles+=" ../Build"
fi
scp -r $localFiles $remote_server_user@$remote_server_address:$remote_path
