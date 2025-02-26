#!/usr/bin/env bash

dnf update -y
dnf install -y findutils gzip libicu tar

cd /tmp

# Install .NET SDK

curl -L -o dotnet.tar.gz https://builds.dotnet.microsoft.com/dotnet/Sdk/9.0.200/dotnet-sdk-9.0.200-linux-x64.tar.gz
mkdir /usr/share/dotnet
tar -xzf dotnet.tar.gz -C /usr/share/dotnet
rm dotnet.tar.gz

cat << 'EOF' > /etc/profile.d/dotnet.sh
export DOTNET_ROOT="/usr/share/dotnet"
export PATH="$DOTNET_ROOT:$PATH"
EOF

source /etc/profile.d/dotnet.sh

# Install Node

curl -L -o node.tar.gz https://nodejs.org/dist/v22.14.0/node-v22.14.0-linux-x64.tar.gz
mkdir /usr/share/node
tar -xzf node.tar.gz --strip-components=1 -C /usr/share/node
rm node.tar.gz

cat << 'EOF' > /etc/profile.d/node.sh
export NODE_ROOT="/usr/share/node/bin"
export PATH="$NODE_ROOT:$PATH"
EOF

source /etc/profile.d/node.sh

# Persist command paths

cat << 'EOF' > ~/.bashrc
if [ -d /etc/profile.d ]; then
  for file in /etc/profile.d/*.sh; do
    source "$file"
  done
fi
EOF

# Install PNPM

corepack prepare pnpm@10.4.1 --activate
corepack enable pnpm
