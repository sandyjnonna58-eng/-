#!/usr/bin/env bash
# Пересборка Assembly-CSharp.dll с модом из оригинальной игры.
# Нужны: .NET SDK 8, ilspycmd 8.2.0.7535 (dotnet tool install -g ilspycmd --version 8.2.0.7535), patch, python3.
# Использование: tools/build.sh "/путь/к/DummyWar Alpha 0.0.3"
set -euo pipefail
GAME="$(cd "$1" && pwd)"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
WORK="$ROOT/build"
rm -rf "$WORK" && mkdir -p "$WORK"
export DOTNET_ROLL_FORWARD=Major
ilspycmd -p -o "$WORK" "$GAME/DummyWar_Data/Managed/Assembly-CSharp.dll"
cp "$ROOT/tools/Assembly-CSharp.csproj" "$WORK/Assembly-CSharp.csproj"
python3 "$ROOT/tools/gen_locru.py" "$ROOT/mod/loc_ru.txt" "$ROOT/mod/src/LocRu.cs"
(cd "$WORK" && patch -p1 --no-backup-if-mismatch < "$ROOT/mod/changes.patch")
cp "$ROOT"/mod/src/*.cs "$WORK/CombatSandbox.Demo/"
dotnet build "$WORK/Assembly-CSharp.csproj" -c Release -p:GameDir="$GAME"
echo "Готово: $WORK/bin/Release/netstandard2.1/Assembly-CSharp.dll"
