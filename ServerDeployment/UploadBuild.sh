#! /bin/bash
source ../.env
scp -r ../Builds/$current_build_name $remote_server_user@$remote_server_address:$remote_path