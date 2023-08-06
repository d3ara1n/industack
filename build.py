import os
import shutil
import subprocess
import time
import xml.etree.ElementTree as ET
from pathlib import Path
import json

# ----- CONFIGURE THESE -----
SYNC_FOLDERS = [
    "Blueprints",
    "Boosterpacks",
    "Cards",
    "Icons",
    "Sounds",
]  # folders to be synced, such as Cards, Blueprints, Icons, etc.
COPY_FILES = [
    "manifest.json",
    "localization.tsv",
]  # individual files to copy, such as manifest.json, localization.tsv, etc. (the mod dll is copied automatically)
MODS_ROOT = Path(os.environ["userprofile"]) / Path(
    "AppData/LocalLow/sokpop/Stacklands/Mods"
)  # windows only, can be hardcoded with the below line instead
# MODS_ROOT = Path("C:/Users/cyber/AppData/LocalLow/sokpop/Stacklands/Mods").resolve()

MOD_BIN = Path("./src/bin/Debug/netstandard2.1").resolve()


def sync_folder(src: Path, dst: Path):
    sync_files = []
    for file in dst.glob("**/*"):
        file_in_src = src / file.relative_to(dst)
        if file.is_dir() and not file_in_src.exists():
            shutil.rmtree(file)
        elif file.is_file() and (
            not file_in_src.exists()
            or file.stat().st_mtime < file_in_src.stat().st_mtime
        ):
            os.remove(file)
    for file in src.glob("**/*"):
        if file.name.endswith(".cs") or file.name.endswith(".xcf"):
            continue
        file_in_dst = dst / file.relative_to(src)
        if not file_in_dst.exists():
            file_in_dst.parent.mkdir(parents=True, exist_ok=True)
            if file.is_file():
                shutil.copy(file, file_in_dst)
                sync_files.append(file_in_dst)
            elif file.is_dir():
                shutil.copytree(file, file_in_dst)
    return sync_files


# build
start_time = time.time()
p = subprocess.Popen(
    "dotnet build Industack.sln",
    stdout=subprocess.PIPE,
    stderr=subprocess.PIPE,
    shell=True,
)
stdout, stderr = p.communicate()
if p.returncode != 0:
    print(stdout.decode())
    exit(p.returncode)
print(f"built in {time.time() - start_time:.2f}s")

# grab metadata
found_csprojs = list(Path("./src").glob("*.csproj"))
if len(found_csprojs) != 1:
    raise RuntimeError("Can't find .csproj file")
with open(found_csprojs[0], encoding="utf-8") as f:
    root = ET.parse(f).getroot()
    MOD_ID = json.load(open("./src/manifest.json"))["id"]
    DLL_NAME = f"{root.find('./PropertyGroup/AssemblyName').text}.dll"
    MOD_DLL = MOD_BIN / DLL_NAME
    MOD_PATH = MODS_ROOT / MOD_ID

# copy dll
MOD_PATH.mkdir(exist_ok=True)
shutil.copyfile(MOD_DLL, MOD_PATH / f"{DLL_NAME}")

# copy files
for blueprint_file in COPY_FILES:
    try:
        shutil.copyfile(f"./src/{blueprint_file}", MOD_PATH / blueprint_file)
    except FileNotFoundError:
        print(f"No such file: '{blueprint_file}'")

# copy folders
IDS_FIELDS = ["RequiredCards", "CardsToRemove", "ExtraResultCards"]
print("syncing folders..")
for folder in SYNC_FOLDERS:
    sync_files = sync_folder(Path(f"./src/{folder}"), MOD_PATH / folder)
    if folder == "Blueprints":
        # do post-process for Blueprint.Subprints[i].RequiredCards|CardsToRemove|ExtraResultCards
        blueprint_files = sync_files
        for blueprint_file in blueprint_files:
            file_handle = open(blueprint_file, "r", encoding="utf-8")
            file_content = file_handle.read()
            file_handle.close()
            blueprint = json.loads(file_content)
            count: int = 0
            for subprint in blueprint["subprints"]:
                for field in IDS_FIELDS:
                    if field in subprint:
                        subprint[field] = ", ".join(subprint[field])
                        count += 1
            file_handle = open(blueprint_file, "w", encoding="utf-8")
            file_handle.write(json.dumps(blueprint))
            file_handle.flush()
            file_handle.close()
            print(f"processed {blueprint_file.name} with {count} field(s)")
