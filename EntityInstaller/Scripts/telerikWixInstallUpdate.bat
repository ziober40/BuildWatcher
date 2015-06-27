@ECHO OFF
CLS
"C:\Program Files (x86)\WiX Toolset v3.8\bin\heat.exe" dir ../../EntitySandbox -out BuildWatcher.wxs -cg EntitySandboxComponent -dr ENTITYSANDBOXFOLDER -gg -pog:content -srd -sfrag -suid -svb6

@echo off
setlocal

MOVE BuildWatcher.wxs ..
