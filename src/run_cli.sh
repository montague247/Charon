#!/usr/bin/env bash
#  _____ _                           
# /  __ \ |                          
# | /  \/ |__   __ _ _ __ ___  _ __  
# | |   | '_ \ / _` | '__/ _ \| '_ \ 
# | \__/\ | | | (_| | | | (_) | | | |
#  \____/_| |_|\__,_|_|  \___/|_| |_|
#                                 
# Copyright (c) Dirk Helbig. All rights reserved.
#

# Stop script on NZEC
set -e
# Stop script if unbound variable found (use ${var:-} if intentional)
set -u
# By default cmd1 | cmd2 returns exit code of cmd2 regardless of cmd1 success
# This is causing it to fail
set -o pipefail

dotnet run --project Charon.Cli/Charon.Cli.csproj
