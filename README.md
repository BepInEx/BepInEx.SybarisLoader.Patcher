# BepInEx.SybarisLoader.Patcher


This is a patcher for [BepInEx 4.0](https://github.com/BepInEx/BepInEx) that allows to apply Sybaris-style patches to the game.  

## Requirements

* BepInEx 4.0 or newer

## Installation

1. Put `BepInEx.SybarisLoader.Patcher.dll` into `BepInEx\patchers` folder.  

2. Add and **modify** the following options in the `BepInEx\config.ini`:
  
  ```ini
  [org.bepinex.patchers.sybarisloader]
  ; Location of Sybaris patchers
  ; For Sybaris 2 this should be Sybaris
  ; For Sybaris 1 this should be Sybaris\Loader
  sybaris-patches-location=Sybaris
  ```


Put Sybaris patches into `Sybaris` folder as you have done before.

> ⚠️ **NOTE** ⚠️
> 
> You **must** remove the following files from `Sybaris` folder in order for this to work:
> * `Sybaris.Loader.dll`
> * `Mono.Cecil.dll`


## Building

To build, you will need to reference `BepInEx.dll`. All other dependencies are downloaded via NuGet automatically.