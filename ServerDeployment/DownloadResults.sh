#! /bin/bash
source ../.env
if [ "$1" == "-final" ]; then
scp -r $remote_server_user@$remote_server_address:$remote_results_path/${version}_$name/Walker.onnx ../Project/Assets/RemoteResults/${version}_$name.onnx
else
scp -r $remote_server_user@$remote_server_address:$remote_results_path/${version}_$name ../Project/Assets/RemoteResults
fi