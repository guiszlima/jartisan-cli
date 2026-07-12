#!/bin/bash
set -e

OS="$(uname -s | tr '[:upper:]' '[:lower:]')"
ARCH="x64"

TARGET_DIR="$HOME/.local/bin"
BINARY_PATH="$TARGET_DIR/jartisan"

REPO="guiszlima/jartisan-cli"
URL="https://github.com/$REPO/releases/latest/download/jartisan-${OS}-${ARCH}.tar.gz"

echo "📥 Downloading Jartisan for $OS ($ARCH)..."
mkdir -p "$TARGET_DIR"


TMP_DIR=$(mktemp -d)


curl -sSL "$URL" | tar -xzf - -C "$TMP_DIR"

FOUND_BIN=$(find "$TMP_DIR" -type f -name "jartisan*" -print -quit)

if [ -n "$FOUND_BIN" ]; then
    mv "$FOUND_BIN" "$BINARY_PATH"
else
    echo "❌ Erro: Nenhum binário jartisan encontrado no pacote baixado."
    rm -rf "$TMP_DIR"
    exit 1
fi


rm -rf "$TMP_DIR"


chmod +x "$BINARY_PATH"

echo "🔧 Configuring system aliases (jartisan & jart)..."
HOME_DIR="$HOME"
ALIAS_JARTISAN="alias jartisan='$BINARY_PATH'"
ALIAS_JART="alias jart='$BINARY_PATH'"
SHELL_CONFIGS=("$HOME_DIR/.bashrc" "$HOME_DIR/.zshrc")

for config in "${SHELL_CONFIGS[@]}"; do
    if [ -f "$config" ]; then
        if ! grep -q "alias jartisan=" "$config"; then
            echo -e "\n# Jartisan CLI Aliases\n$ALIAS_JARTISAN" >> "$config"
        fi
        if ! grep -q "alias jart=" "$config"; then
            echo "$ALIAS_JART" >> "$config"
        fi
    fi
done

echo -e "\n✨ Jartisan successfully installed to $BINARY_PATH!"
echo "👉 Please restart your terminal or run: source ~/.bashrc (or ~/.zshrc)"
echo "🚀 You can now use either 'jartisan' or 'jart' commands anywhere!"
