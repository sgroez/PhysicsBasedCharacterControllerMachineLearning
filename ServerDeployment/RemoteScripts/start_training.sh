#!/bin/bash
source .env #load env variables
source ~/anaconda3/etc/profile.d/conda.sh
conda activate mlagentsR21
flags="--env=./Build/Build --run-id=$name$version --no-graphics"
if [ "$1" == "-continue" ]; then
flags+=" --resume"
else
flags+=" --force"
fi
mlagents-learn ./Walker.yaml $flags