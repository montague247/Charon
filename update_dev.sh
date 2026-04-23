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

echo "Updating Charon and dependencies..."

PIP=pip3

scriptdir=$(dirname "$0")
listsdir="$scriptdir/lists"
mkdir -p "$listsdir"

#echo "Update all python packages..."

#for line in $(pip3 list --local --format=freeze); do
#package=${line%%=*}
#$PIP install -U $package
#done

#echo "Update pipx apps..."

#pipx upgrade-all
#pipx list > "$listsdir/pipx_list.txt"

echo "Update homebrew..."

brew update
brew upgrade
brew list > "$listsdir/brew_list.txt"

echo "Update all brew casks..."

brew list --cask | xargs brew upgrade --cask --greedy
brew list --cask > "$listsdir/brew_cask_list.txt"

if command -v ollama >/dev/null 2>&1; then
    echo "Ollama is available"
else
    brew install ollama
fi

if [ -d frontend ]; then
    cd frontend
    ./update_dependencies.sh
    cd ..
fi

if [ -d src ]; then
    cd src
    ./update_dependencies.sh
    cd ..
fi

echo "Cleanup..."

brew cleanup --prune=0

echo "Finished updating Charon and dependencies."
