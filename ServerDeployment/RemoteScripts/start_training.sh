#!/bin/bash
source .env #load env variables
source ~/anaconda3/etc/profile.d/conda.sh
conda activate mlagents
mlagents-learn ./Walker.yaml --env=./Build/Build --run-id=$name$version --no-graphics --num-envs=$env_count --force #start training