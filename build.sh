#!/bin/sh

dotnet build -c Release
if [ $? -ne 0 ]; then
    echo "✗ Build failed"
    exit 1
fi

# Copy DLL and assets to Subnautica BepInEx plugins folder
# Set SUBNAUTICA_PATH environment variable or edit here for your system
SUBNAUTICA_PATH="${SUBNAUTICA_PATH:-D:/SteamLibrary/steamapps/common/Subnautica}"
PLUGIN_DIR="$SUBNAUTICA_PATH/BepInEx/plugins/SeamothFlareSystem"
DLL_PATH="SeamothFlareSystem/bin/Release/net472/SeamothFlareSystem.dll"
LOCALIZATION_PATH="SeamothFlareSystem/Localizations.xml"
RESOURCES_PATH="SeamothFlareSystem/Resources"
MOD_JSON_PATH="SeamothFlareSystem/mod.json"
if [ -f "$DLL_PATH" ]; then
    mkdir -p "$PLUGIN_DIR"
    cp "$DLL_PATH" "$PLUGIN_DIR/"
    echo "✓ DLL copied to $PLUGIN_DIR"

    if [ -f "$LOCALIZATION_PATH" ]; then
        cp "$LOCALIZATION_PATH" "$PLUGIN_DIR/"
        echo "✓ Localizations.xml copied to $PLUGIN_DIR"
    else
        echo "✗ Localizations.xml not found at $LOCALIZATION_PATH"
    fi

    if [ -d "$RESOURCES_PATH" ]; then
        cp -r "$RESOURCES_PATH" "$PLUGIN_DIR/"
        echo "✓ Resources copied to $PLUGIN_DIR"
    else
        echo "✗ Resources directory not found at $RESOURCES_PATH"
    fi
    
    if [ -f "$MOD_JSON_PATH" ]; then
        cp "$MOD_JSON_PATH" "$PLUGIN_DIR/"
        echo "✓ mod.json copied to $PLUGIN_DIR"
    else
        echo "✗ mod.json not found at $MOD_JSON_PATH"
    fi
else
    echo "✗ DLL not found at $DLL_PATH"
    echo "Make sure you have built the project in Release mode"
    exit 1
fi

echo "✓ Build and deployment complete!"