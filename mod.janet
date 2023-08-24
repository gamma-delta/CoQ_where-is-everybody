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
  :description "Makes creatures able to tell you additional contextual secrets during the water ritual, like the locations of villages they are loved or hated by.")

# (set-debug-output true)

