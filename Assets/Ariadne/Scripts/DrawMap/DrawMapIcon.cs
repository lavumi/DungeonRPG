using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ariadne {

	/// <Summary>
	/// Draw icons on the map.
	/// Icons are instantiated as GameObject.
	/// </Summary>
	public class DrawMapIcon : MonoBehaviour, IDirtyMarkerMap {

		int showLengthHorizontal = 4;
		int showLengthVertical = 4;
		int drawSmoothness = 2;
		DungeonMasterData dungeonData;
		FloorMapMasterData floorMapData;

		public Sprite doorIcon;
		public Sprite lockedDoorIcon;
		public Sprite downstairsIcon;
		public Sprite upstairsIcon;
		public Sprite treasureIcon;
		public Sprite messengerIcon;

		Dictionary<Vector2Int, GameObject> posIconDict;
		RectTransform parentRt;
		RectTransform mapRt;
		Vector2 posLerp = Vector2.zero;
		Vector3 iconScale = new Vector3(1.0f, 1.0f, 1.0f);
		GameObject gameController;
		DungeonSettings dungeonSettings;
		MapShowingSettings mapSettings;

		void Start () {
			GetFloorData();
			GetMapSettings();
			DrawIcon();
		}

		/// <Summary>
		/// Get floor data from the DungeonSettings component.
		/// </Summary>
		void GetFloorData(){
			if (gameController == null){
				gameController = GameObject.FindGameObjectWithTag("GameController");
			}
			if (dungeonSettings == null){
				dungeonSettings = gameController.GetComponent<DungeonSettings>();
			}
			dungeonData = dungeonSettings.dungeonData;
			floorMapData = dungeonSettings.GetCurrentFloorData();
		}

		/// <Summary>
		/// Get settings about map from the MapShowingSettings component.
		/// </Summary>
		void GetMapSettings(){
			if (gameController == null){
				gameController = GameObject.FindGameObjectWithTag("GameController");
			}
			if (mapSettings == null){
				mapSettings = gameController.GetComponent<MapShowingSettings>();
			}
			this.showLengthHorizontal = mapSettings.showLengthHorizontal;
			this.showLengthVertical = mapSettings.showLengthVertical;
			this.drawSmoothness = mapSettings.smoothness;
		}

		/// <Summary>
		/// Instantiate icon GameObjects according to map attribute.
		/// When they have been instantiated already, this method updates their position and visible state.
		/// </Summary>
		void DrawIcon(){
			if (posIconDict == null){
				posIconDict = new Dictionary<Vector2Int, GameObject>();
			}
			if (mapRt == null){
				mapRt = gameObject.GetComponent<RectTransform>();
			}
			Vector3 centerPos = mapRt.transform.localPosition;

			float cellWidth = mapRt.sizeDelta.x / (1 + showLengthHorizontal * 2);
			float cellHeight = mapRt.sizeDelta.y / (1 + showLengthVertical * 2);
			Vector2 unitSize = new Vector2(cellWidth, cellHeight);
			float widthOffset = cellWidth / 2;
			float heightOffset = cellHeight / 2;
			
			if (parentRt == null){
				parentRt = GameObject.FindGameObjectWithTag("Map").GetComponent<RectTransform>();
			}
			Vector2 parentPos = parentRt.transform.localPosition;
			float parentWidth = parentRt.sizeDelta.x;
			float parentHeight = parentRt.sizeDelta.y;

			int signX = centerPos.x < 0 ? -1 : 1;
			int signY = centerPos.y < 0 ? -1 : 1;

			float basePosX = parentWidth / 2 - signX * centerPos.x - widthOffset - posLerp.x * unitSize.x;
			float basePosY = parentHeight / 2 - signY * centerPos.y - heightOffset - posLerp.y * unitSize.y;

			for (int yAxis = 0; yAxis < floorMapData.floorSizeVertical; yAxis++){
				for (int xAxis = 0; xAxis < floorMapData.floorSizeHorizontal; xAxis++){

					int index = xAxis + yAxis * floorMapData.floorSizeHorizontal;
					int mapAttrId = floorMapData.mapInfo[index].mapAttr;
					if (mapAttrId == MapAttributeDefine.HALL_WAY || mapAttrId == MapAttributeDefine.WALL){
						continue;
					}

					Vector3 basePosInLoop = new Vector3(basePosX + unitSize.x * xAxis, basePosY + unitSize.y * yAxis);
					Vector2Int axisPos = new Vector2Int(xAxis, yAxis);
					Vector2 iconPos = new Vector2(basePosInLoop.x + unitSize.x / 2, basePosInLoop.y + unitSize.y / 2);
					SetIconObj(iconPos, mapAttrId, axisPos, unitSize);
				}
			}
		}

		/// <Summary>
		/// Set icon GameObjects.
		/// </Summary>
		/// <param name="iconPos">The icon position on the map.</param>
		/// <param name="mapAttrId">The map attribute id.</param>
		/// <param name="axisPos">The position on the dungeon.</param>
		/// <param name="sizeSet">The Icon size.</param>
		void SetIconObj(Vector2 iconPos, int mapAttrId, Vector2Int axisPos, Vector2 sizeSet){
			if (mapAttrId == MapAttributeDefine.HALL_WAY || 
				mapAttrId == MapAttributeDefine.WALL || 
				mapAttrId == MapAttributeDefine.PILLAR || 
				mapAttrId == MapAttributeDefine.WALL_WITH_TORCH){
				return;
			}
			string objName = "Icon_" + axisPos.x.ToString() + "-" + axisPos.y.ToString();
			Sprite sp = doorIcon;
			if (mapAttrId == MapAttributeDefine.DOOR){
				objName = objName + "_Door";
				sp = doorIcon;
				
			} else if (mapAttrId == MapAttributeDefine.LOCKED_DOOR){
				objName = objName + "_LockedDoor";
				sp = lockedDoorIcon;
				
			} else if (mapAttrId == MapAttributeDefine.DOWNSTAIRS){
				objName = objName + "_Downstairs";
				sp = downstairsIcon;
				
			} else if (mapAttrId == MapAttributeDefine.UPSTAIRS){
				objName = objName + "_Upstairs";
				sp = upstairsIcon;
				
			} else if (mapAttrId == MapAttributeDefine.TREASURE){
				objName = objName + "_Treasure";
				sp = treasureIcon;
				
			} else if (mapAttrId == MapAttributeDefine.MESSENGER){
				objName = objName + "_Messenger";
				sp = messengerIcon;
				
			}

			Transform trans = transform.Find(objName);
			if (trans == null){
				ShowIconObj(iconPos, axisPos, sizeSet, sp, objName);
				return;
			}

			GameObject iconObj = trans.gameObject;
			if (posIconDict.ContainsKey(axisPos)){
				iconObj.transform.localPosition = iconPos;
				iconObj.transform.localScale = iconScale;
				iconObj.GetComponent<RectTransform>().sizeDelta = sizeSet;
				SetIconVisibleState(axisPos, trans.gameObject);
			} else {
				ShowIconObj(iconPos, axisPos, sizeSet, sp, objName);
			}
		}

		/// <Summary>
		/// Instantiate icon GameObjects.
		/// </Summary>
		/// <param name="iconPos">The icon position on the map.</param>
		/// <param name="axisPos">The position on the dungeon.</param>
		/// <param name="sizeSet">The Icon size.</param>
		/// <param name="sprite">The sprite image of the icon.</param>
		/// <param name="objName">The name of icon object.</param>
		void ShowIconObj(Vector2 iconPos, Vector2Int axisPos, Vector2 sizeSet, Sprite sprite, string objName){
			GameObject iconPrefab = (GameObject)Resources.Load("Prefabs/IconObj");
			GameObject icon = (GameObject)Instantiate(iconPrefab, iconPos, Quaternion.identity);

			icon.GetComponent<Image>().sprite = sprite;
			SetIconVisibleState(axisPos, icon);

			icon.transform.SetParent(gameObject.transform);
			icon.transform.localPosition = iconPos;
			icon.transform.localScale = iconScale;
			icon.GetComponent<RectTransform>().sizeDelta = sizeSet;
			icon.name = objName;

			if (posIconDict.ContainsKey(axisPos)){
				posIconDict[axisPos] = icon;
			} else {
				posIconDict.Add(axisPos, icon);
			}
		}

		/// <Summary>
		/// Move icon objects.
		/// </Summary>
		/// <param name="prePos">The pre-position of the icon.</param>
		/// <param name="destPos">The dest position of the icon.</param>
		void MoveIcon(Vector2 prePos, Vector2 destPos){
			float cellWidth = mapRt.sizeDelta.x / (1 + showLengthHorizontal * 2);
			float cellHeight = mapRt.sizeDelta.y / (1 + showLengthVertical * 2);
			Vector2 unitSize = new Vector2(cellWidth, cellHeight);

			float deltaX = (destPos.x - prePos.x) * unitSize.x;
			float deltaY = (destPos.y - prePos.y) * unitSize.y;
			foreach (KeyValuePair<Vector2Int, GameObject> pair in posIconDict){
				// Check if the position is traversed.
				SetIconVisibleState(pair.Key, pair.Value);

				Vector2 pos = pair.Value.transform.localPosition;
				pos.x -= deltaX;
				pos.y -= deltaY;
				pair.Value.transform.localPosition = pos;
				pair.Value.GetComponent<RectTransform>().sizeDelta = unitSize;
			}
		}

		/// <Summary>
		/// Set the visible state of the icon according to traverse data.
		/// </Summary>
		/// <param name="axisPos">The position on the dungeon.</param>
		/// <param name="obj">The icon object.</param>
		void SetIconVisibleState(Vector2Int axisPos, GameObject obj){
			bool isTraversed = TraverseManager.GetPositionTraverseData(dungeonData.dungeonId, floorMapData.floorId, axisPos);
			obj.GetComponent<Image>().enabled = isTraversed;
		}

		/// <Summary>
		/// Receiver of OnSetDirty message.
		/// </Summary>
		public void OnSetDirty(){
			GetFloorData();
			GetMapSettings();
			DrawIcon();
		}

		/// <Summary>
		/// Receiver of OnSetDirtyLerp message.
		/// </Summary>
		/// <param name="time">The moveWait in MoveController.</param>
		public void OnSetDirtyLerp(float time){
			GetFloorData();
			GetMapSettings();
			DrawIcon();
			StartCoroutine(DrawIntermediateMap(time));
		}

		/// <Summary>
		/// Draw intermediate map according to the value of drawSmoothness.
		/// </Summary>
		/// <param name="time">The moveWait in MoveController.</param>
		IEnumerator DrawIntermediateMap(float time){
			yield return null;
			Vector2Int sourcePos = PlayerPosition.playerPosPre;
			Vector2Int destPos = PlayerPosition.playerPos;
			float finishTime = Time.time + time;

			Vector2 prePosLerp = Vector2.zero;

			while (true){
				float diff = finishTime - Time.time;
				if (diff <= 0){
					break;
				}
				float rate = 1 - Mathf.Clamp01(diff / time);
				prePosLerp = posLerp;
				posLerp = Vector2.Lerp(sourcePos, destPos, rate);
				MoveIcon(prePosLerp, posLerp);
				for (int i = 0; i < drawSmoothness; i++){
					yield return null;
				}
			}
			prePosLerp = posLerp;
			posLerp = destPos;
			MoveIcon(prePosLerp, posLerp);
		}

		/// <Summary>
		/// Receiver of OnSetNewMap message.
		/// </Summary>
		public void OnSetNewMap(){
			// Remove map icons.
			RemoveMapIcons();

			// Get new floor data.
			GetFloorData();

			// Draw icons.
			DrawIcon();
		}

		/// <Summary>
		/// Removes icon objects.
		/// </Summary>
		void RemoveMapIcons(){
			posIconDict = new Dictionary<Vector2Int, GameObject>();
			foreach (Transform child in gameObject.transform){
				GameObject childObj = child.gameObject;
				Destroy(childObj);
			}
		}
	}

}
