#! /bin/bash
source ../.env
if [ "$1" == "-final" ]; then
scp -r $remote_server_user@$remote_server_address:$remote_results_path/$name$version/Walker.onnx ../Project/Assets/RemoteResults/$name$version.onnx
else
scp -r $remote_server_user@$remote_server_address:$remote_results_path/$name$version ../Project/Assets/RemoteResults
fi