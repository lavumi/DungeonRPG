///**************************************************
/// Ariadne 3D Dungeon Maker
/// © 2018 Explorers Lab
/// Version 1.0.1
///**************************************************

Ariadne 3D Dungeon Maker is a powerful asset 
to create 3D dungeons.

In the extension, you can make grid-based map data in MapEditor.
And set it to the Game Controller prefab, 
Ariadne 3D Dungeon Maker produces dungeons 
according to your map data at runtime.

This asset includes the MoveController that enables 
movement in the dungeon.
The controller enables processing events, too.


# Table of Contents
1. Features
2. Demo
3. Workflow
4. Support
5. Version history


## 1. Features
- Powerful map data editor on Unity Editor.
The dungeon prefabs are instantiated at runtime 
according to your map.  

- 6 type of events can define on each point on the map.
    - Opening a door
    - Opening a locked door
    - Move to other position (such as upstairs)
    - Get treasure (Item and money)
    - Show messages to the screen
    - Exiting from the dungeon

- Showing map data on the screen. 
That indicates player position and attributes of the map.

- Including a controller for movement in the dungeon.

- Including a demo dungeon to play.


## 2. Demo
As a tour of Ariadne 3D Dungeon Maker, there is a demo scene.
The scene file is placed on [Ariadne/Demo] folder.

In the dungeon, you can control the player 
by using arrow keys to move and using a space key to deciding. 

You can also control by using buttons on the screen. 
Those buttons correspond to controlling on mobile devices.


## 3. Workflow
First, create a map data by using MapEditor.
You can open the MapEditor from [Window/MapEditor] 
in the menu bar.

On the MapEditor, you can set attributes of the map 
by using draw tools.
To add an event, select a position using 
select tool and press [Open Event Editor] button.

On the EventEditor, you can define contents of events 
and starting conditions.

Second, after saving the map data, 
create a dungeon data from 
[Asset/Create/Ariadne/DungeonData] in the menu bar.
The dungeon data is a holder of map data. 
Set map data that you created to dungeon data.

Next, set the dungeon data to GameController object in the Scene. 
The GameController object has a component named DungeonSettings, 
so set the dungeon data to this component.

Required objects are placed in [Ariadne/Resources/Prefabs/SceneObjects] folder 
and [Ariadne/Resources/Prefabs/SceneObjects/CanvasParts] folder as prefabs.
When you intend to create a new scene, 
it is useful to duplicate demo scene and customize it.

Finally, ready to explore your dungeon. Execute the game!


For more information, see also Ariadne_manual.pdf.


## 4. Support
Please feel free to contact me if you have any questions or comments.

Web
https://www.facebook.com/ExplorersLabSupport/

E-mail
explorers-lab@hotmail.com


## 5. Version history
### Ver 1.0.1
- About prefabs
  - Added "pillar" parts and "wall with torch" parts as prefabs.
    New files are placed below [Ariadne/Resources/Prefabs].
  - Added "pillar" and "wall with torch" attributes to MapEditor.
  - Added "pillar" and "wall with torch" reference to DungeonPartsMasterData.

- About manuals
  - Added a manual for Japanese.


### Ver 1.0.0
- First release.