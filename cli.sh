#!/bin/sh

export BASIC_RSA_STORE=/home/basicec/RiderProjects/BasicEC.Secret/rsa_store

dotnet run --project BasicEC.Secret.Console -- "$@"
