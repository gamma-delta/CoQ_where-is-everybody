#!/usr/bin/env janet

(use cbt)

(build-metadata
  :qud-dlls "/home/petrak/.local/share/Steam/steamapps/common/Caves of Qud/CoQ_Data/Managed/"
  :qud-mods-folder "/home/petrak/.config/unity3d/Freehold Games/CavesOfQud/Mods/")

(declare-mod
  "where-is-everybody"
  "Where Is Everybody?"
  "petrak@"
  "0.1.0"
  :description "Makes village leaders more likely to tell you the location of nearby villages in water rituals.")

# (set-debug-output true)

