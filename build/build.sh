#!/usr/bin/env bash

set -xe

cd ../

MOD_NAME="MLPatcherPlugin"
VERSION="$(git describe --abbrev=0 | tr -d  "v")"

ML_NAME="$MOD_NAME-$VERSION"
ML_DIR="build/$ML_NAME"


dotnet build -c Release

mkdir -p "$ML_DIR"/{Plugins,UserLibs}

cp bin/release/net472/*.dll \
    "$ML_DIR/Plugins/"
cp bin/release/net472/libs/{Mono.Cecil,MonoMod.Utils}.dll \
    "$ML_DIR/UserLibs/"
chmod -x "$ML_DIR"/UserLibs/*.dll
cp build/README.txt "$ML_DIR"

# Zip everything
pushd "$ML_DIR"
zip -r ../"$ML_NAME.zip" .
popd

# Remove directories
rm -rf "$ML_DIR"
