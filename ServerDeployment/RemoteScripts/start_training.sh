#!/bin/bash
source .env #load env variables
source ~/anaconda3/etc/profile.d/conda.sh
conda activate mlagentsR21
flags="--env=./Build/Build --run-id=${version}_$name --no-graphics"
mlagents-learn ./$config_name $flags "$@"