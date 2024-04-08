#! /bin/bash
source ../.env
scp -r ../$current_build_path $remote_server_user@$remote_server_address:$remote_path