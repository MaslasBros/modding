# Modding

## Description 

A C# tool that will provide in your game the ability of having modding and to your users the possibility to easily change all kinds of assets.

## Folder Structure

The var folder will act as a fallback. Ιn case there are no mods or they don't meet the conditions, the assets from the var folder will be displayed.

```mermaid 
flowchart TB
    Var --> DB
    Var --> GIS
    Var --> Missions
    Var --> Samples
    Var --> Scenarios 
    Var --> Scripts
    Var --> package.json
```

The Mods folder is where the mods should be to work properly. Each mod folder should have the same structure as the var folder. If the mod passes the evaluation and is active, the asset from the mod folder will be displayed , on the contrary if there is no mod or none is active, then the asset from the var folder will be displayed.

```mermaid 
flowchart TB
    GlobalData --> Mods --> Mod0 --> DB
    Mod0 --> GIS
    Mod0 --> Missions
    Mod0 --> Samples
    Mod0 --> Scenarios 
    Mod0 --> Scripts
    Mod0 --> package.json

    Mods --> Mod1 --> DB
    Mod1 --> GIS
    Mod1 --> Missions
    Mod1 --> Samples
    Mod1 --> Scenarios 
    Mod1 --> Scripts
    Mod1 --> package.json

    Mods --> Mod2
    Mods --> etc. 
```


In order for a mod package to be valid, only one package.json file should be included in the root of its folder with the following JSON format...

{

  "name": "my_package",
  
  "description": "",
  
  "author": "",
  
  "version": "1.0.0",
  
  "supported": "^1.0.2"
  
}

We check if the version from the mod is the same as the version of the game and if the supported version is not exceed the acceptable version range. In case it doesnt apply the previous, the mod will not be valid.
