# BepInEx.SybarisLoader.Patcher


This is a patcher for [BepInEx 5.0 RC1](https://github.com/BepInEx/BepInEx) that allows to apply Sybaris-style patches to the game.  

## Requirements

* BepInEx 5.0 RC1 or newer

## Installation

1. Extract the contents of the archive into `BepInEx` folder.  

2. Modify the configuration options in `BepInEx\config\SybarisLoader.cfg`
  
Put Sybaris patches into `Sybaris` folder as you have done before.

> ⚠️ **NOTE** ⚠️
> 
> You **must** remove the following files from `Sybaris` folder in order for this to work:
> * `Sybaris.Loader.dll`
> * `Mono.Cecil.dll`


## Building

To build, you will need to reference `BepInEx.dll`. All other dependencies are downloaded via NuGet automatically.