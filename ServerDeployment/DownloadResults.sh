#! /bin/bash
source ../.env
scp -r $remote_server_user@$remote_server_address:$remote_results_path$version ../Project/Assets/RemoteResults