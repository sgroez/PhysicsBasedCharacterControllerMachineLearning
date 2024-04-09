#! /bin/bash
source ../.env
scp -r ../.env ../Build $config_path $remote_server_user@$remote_server_address:$remote_path
