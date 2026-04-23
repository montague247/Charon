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

echo "Update all python packages..."
$PIP list --local -o --format=freeze | cut -d = -f 1 | xargs $PIP install -U
$PIP list --local --format=freeze > "$listsdir/pip_list.txt"

echo "Update pipx apps..."
pipx upgrade-all
pipx list > "$listsdir/pipx_list.txt"

echo "Update homebrew..."
brew update
brew upgrade
brew list > "$listsdir/brew_list.txt"

echo "Update all brew casks..."
brew cask outdated | cut -d " " -f 1 | xargs -n 1 brew cask reinstall
brew cask list > "$listsdir/brew_cask_list.txt"

if command -v ollama >/dev/null 2>&1; then
    echo "Ollama is available"
else
    brew install ollama
fi

[ -d frontend ] && frontend/update_dependenies.sh
[ -d src ] && src/update_dependenies.sh

echo "Cleanup..."

brew cleanup --prune=0

echo "Finished updating Charon and dependencies."
