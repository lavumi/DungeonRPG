using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ariadne {

	/// <Summary>
	/// Utility class for MapEditor and EventEditor.
	/// </Summary>
	public static class MapEditorUtil {

		// Temp file of floor map data.
		public const string tempFileName = "MapDataTemp";
		public const string tempFilePathPrefix = "Assets/Ariadne/Resources/";
		public const string tempFilePath = "MapData/MapEditor/";
		public const string saveFolderPrefix = "Assets/";
		public const string saveFolderPath = "Ariadne/Resources/MapData/";
		public const string saveFilePostFix = ".asset";
		public const string infoFileName = "tempInfo";

		public const string iconPath = "MapData/Icon/";
		public const string eventDefaultName = "New Event";
		public const string eventDataPathPrefix = "Assets/Ariadne/Resources/";
		public const string eventDataPath = "EventData/";
		public const string eventFilePrefix = "event_";

		public const string notAssignedText = "*** Not Assigned ***";

		/// <Summary>
		/// Validation of the index.
		/// </Summary>
		/// <param name="index">Index to check.</param>
		/// <param name="listSize">Size of map array.</param>
		public static bool CheckEditMapIndexIsValid(int index, int listSize){
			bool isValid = false;
			if (index >= 0 && index < listSize){
				isValid = true;
			}
			return isValid;
		}

		/// <Summary>
		/// Validation of the floor size.
		/// </Summary>
		/// <param name="horizontal">Horizontal size of the floor.</param>
		/// <param name="vertical">Vertical size of the floor.</param>
		public static bool CheckFloorSizeIsValid(int horizontal, int vertical){
			bool isValid = false;
			if (horizontal > 0 && vertical > 0){
				isValid = true;
			}
			return isValid;
		}

		/// <Summary>
		/// Returns a texture of map base.
		/// </Summary>
		/// <param name="tex">Texture to set pixel data.</param>
		/// <param name="cellSize">Cell size of the map.</param>
		/// <param name="gridColor">Color of the grid line on map.</param>
		/// <param name="baseColor">Color of the map base.</param>
		public static Texture2D SetMapColor(Texture2D tex, int cellSize, Color gridColor, Color baseColor){
			for (int y = 0; y < tex.height; y++){
				for (int x = 0; x < tex.width; x++){
					if (CheckGridPoint(x, y, cellSize)){
						tex.SetPixel(x, y, gridColor);
					} else {
						tex.SetPixel(x, y, baseColor);
					}
				}
			}
			tex.Apply();
			return tex;
		}

		/// <Summary>
		/// Check whether draw grid line color of the pixel.
		/// </Summary>
		/// <param name="x">Horizontal axis of the pixel.</param>
		/// <param name="y">Vertical axis of the pixel.</param>
		/// <param name="cellSize">Cell size of the map.</param>
		static bool CheckGridPoint(int x, int y, int cellSize){
			bool isValid = false;
			if (x % cellSize == 0 || x % cellSize == cellSize - 1){
				isValid = true;
			} else if (y % cellSize == 0 || y % cellSize == cellSize - 1){
				isValid = true;
			}
			
			return isValid;
		}

		/// <Summary>
		/// Returns new texture of the map.
		/// </Summary>
		/// <param name="horizontalSize">Horizontal size of the map.</param>
		/// <param name="verticalSize">Vertical size of the map.</param>
		/// <param name="cellSize">Cell size of the map.</param>
		public static Texture2D RepaintMapTexture(int horizontalSize, int verticalSize, int cellSize, Color gridColor, Color baseColor){
			Texture2D tex = new Texture2D(16, 16, TextureFormat.ARGB32, false);
			if (CheckFloorSizeIsValid(horizontalSize, verticalSize)){
				int texWidth = cellSize * horizontalSize;
				int texHeight = cellSize * verticalSize;

				tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);
				tex = SetMapColor(tex, cellSize, gridColor, baseColor);
			} else {
				Debug.LogError("Floor size might be invalid value. Input a value which is greater than 0.");
			}
			return tex;
		}

		/// <Summary>
		/// Returns rect for map to select the axis on the dungeon.
		/// </Summary>
		/// <param name="baseLayerRect">Texture of base layer of the map.</param>
		/// <param name="horizontalSize">Horizontal size of the map.</param>
		/// <param name="verticalSize">Vertical size of the map.</param>
		/// <param name="cellSize">Cell size of the map.</param>
		public static Rect[,] GetMapRectArray(Rect baseLayerRect, int horizontalSize, int verticalSize, int cellSize){
			Rect[,] mapRect = new Rect[horizontalSize, verticalSize];
			for (int y = 0; y < verticalSize; y++){
				for (int x = 0; x < horizontalSize; x++){
					Vector2 pos = baseLayerRect.position;
					pos.x += cellSize * x + GUI.skin.box.padding.left;
					pos.y -= cellSize * (y + 1 - verticalSize) - GUI.skin.box.padding.top;
					Rect r = new Rect(pos.x, pos.y, cellSize, cellSize);
					mapRect[x, y] = r;
				}
			}
			return mapRect;
		}

		/// <Summary>
		/// Returns map icon textures.
		/// </Summary>
		public static Texture2D[] GetMapIconArray(){
			// Map icon
			Texture2D hallwayIcon = (Texture2D)Resources.Load(iconPath + "hallway");
			Texture2D wallIcon = (Texture2D)Resources.Load(iconPath + "wall");
			Texture2D doorIcon = (Texture2D)Resources.Load(iconPath + "door");
			Texture2D lockedDoorIcon = (Texture2D)Resources.Load(iconPath + "lockedDoor");
			Texture2D downstairsIcon = (Texture2D)Resources.Load(iconPath + "downstairs");
			Texture2D upstairsIcon = (Texture2D)Resources.Load(iconPath + "upstairs");
			Texture2D treasureIcon = (Texture2D)Resources.Load(iconPath + "treasure");
			Texture2D messengerIcon = (Texture2D)Resources.Load(iconPath + "messenger");
			Texture2D pillarIcon = (Texture2D)Resources.Load(iconPath + "pillar");
			Texture2D wallWithTorchIcon = (Texture2D)Resources.Load(iconPath + "wallWithTorch");
			Texture2D[] mapIconList = new Texture2D[]{hallwayIcon, wallIcon, doorIcon, lockedDoorIcon, downstairsIcon, upstairsIcon, treasureIcon, messengerIcon, pillarIcon, wallWithTorchIcon};
			return mapIconList;
		}

		/// <Summary>
		/// Returns tool icon textures.
		/// </Summary>
		public static Texture2D[] GetToolIconArray(){
			// Tool icon
			Texture2D toolSelectIcon = (Texture2D)Resources.Load(iconPath + "tool_select");
			Texture2D toolDrawIcon = (Texture2D)Resources.Load(iconPath + "tool_pen");
			Texture2D toolRectIcon = (Texture2D)Resources.Load(iconPath + "tool_rect");
			Texture2D toolRectFillIcon = (Texture2D)Resources.Load(iconPath + "tool_rect_fill");
			Texture2D[] drawToolIconList = new Texture2D[]{toolSelectIcon, toolDrawIcon, toolRectIcon, toolRectFillIcon};
			return drawToolIconList;
		}

		/// <Summary>
		/// Returns event icon textures.
		/// </Summary>
		public static Texture2D GetEventIcon(){
			Texture2D eventIcon = (Texture2D)Resources.Load(iconPath + "event");
			return eventIcon;
		}

		/// <Summary>
		/// Returns entrance icon textures.
		/// </Summary>
		public static Texture2D GetEntranceIcon(){
			Texture2D entranceIcon = (Texture2D)Resources.Load(iconPath + "entrance");
			return entranceIcon;
		}

		/// <Summary>
		/// Returns the axis of selected point on the grid.
		/// </Summary>
		/// <param name="pos">Position on the map texture GUI.</param>
		/// <param name="horizontalSize">Horizontal size of the map.</param>
		/// <param name="verticalSize">Vertical size of the map.</param>
		/// <param name="cellSize">Cell size of the map.</param>
		public static Vector2Int GetPosInGrid(Vector2 pos, int horizontalSize, int verticalSize, int cellSize){
			// The origin is left bottom
			Vector2Int gridPos = Vector2Int.zero;

			gridPos.x = (int)pos.x / cellSize;
			if (gridPos.x > horizontalSize - 1){
				gridPos.x = horizontalSize - 1;
			} else if (gridPos.x < 0){
				gridPos.x = 0;
			}

			gridPos.y = (int)pos.y / cellSize;
			gridPos.y = verticalSize - 1 - gridPos.y;
			if (gridPos.y > verticalSize - 1){
				gridPos.y = verticalSize - 1;
			} else if (gridPos.y < 0){
				gridPos.y = 0;
			}
			return gridPos;
		}

		/// <Summary>
		/// Returns the array of GUIDs specified type.
		/// </Summary>
		/// <param name="dataType">Type name to search asset.</param>
		public static string[] GetGuidArray(string dataType){
			string filter = "t:" + dataType;
			string[] guidArray = AssetDatabase.FindAssets(filter);
			return guidArray;
		}
	}
}

