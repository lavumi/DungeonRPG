using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Drawing dungeon walls.
	/// </Summary>
	public class DrawDungeonWall : DrawDungeon, IDrawer {

		/// <Summary>
		/// Instantiate dungeon walls and objects according to map attribute.
		/// </Summary>
		void DrawWalls(){
			Vector3 basePos = Vector3.zero;

			GameObject groundPrefab = dungeonParts.groundObj;
			Vector3 groundSize = new Vector3(groundPrefab.transform.localScale.x, groundPrefab.transform.localScale.y, groundPrefab.transform.localScale.z);

			GameObject wallPrefab = dungeonParts.wallObj;
			Vector3 unitSize = new Vector3(wallPrefab.transform.localScale.x, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
			float height = (groundSize.y + unitSize.y) / 2;

			Direction direction = new Direction();
			// Instanciate parts on X-Z plane
			for (int zAxis = 0; zAxis < currentFloorMapData.floorSizeVertical; zAxis++){
				for (int xAxis = 0; xAxis < currentFloorMapData.floorSizeHorizontal; xAxis++){

					int index = xAxis + zAxis * currentFloorMapData.floorSizeHorizontal;
					int mapAttrData = currentFloorMapData.mapInfo[index].mapAttr;
					int messengerTypeId = currentFloorMapData.mapInfo[index].messengerType;
					Vector3 posInLoop = new Vector3(basePos.x + xAxis * unitSize.x, basePos.y + height, basePos.z + zAxis * unitSize.z);
					string objName = MapAttributeDefine.GetAttrNameById(mapAttrData) + "_" + xAxis.ToString() + "-" + zAxis.ToString();
					Vector3 rotation = direction.GetRotationOfDirection(currentFloorMapData.mapInfo[index].objectFront);
					InstantiateWall(mapAttrData, posInLoop, objName, rotation, messengerTypeId, index);
				}
			}

			// Instanciate outside walls
			if (isDrawOutsideWall){
				// Horizontal walls - South
				string outsideObjName = "OutsideWall_South_Horizontal";
				Vector3 scale = new Vector3(unitSize.x * (currentFloorMapData.floorSizeHorizontal + outsideWallSize * 2), unitSize.y, unitSize.z * outsideWallSize);
				float outSideWallOffsetX = (1 + outsideWallSize) * 0.5f * unitSize.x;
				float outSideWallOffsetZ = (1 + outsideWallSize) * 0.5f * unitSize.z;
				Vector3 baseOffset = new Vector3(unitSize.x / 2, 0f, unitSize.z / 2);
				float posX = baseOffset.x + scale.x / 2 - outSideWallOffsetX * 2;
				float posZ = -1 * outSideWallOffsetZ;
				Vector3 pos = new Vector3(posX, height, posZ);
				InstantiateOutsideWalls(pos, scale, outsideObjName);

				// Horizontal walls - North
				outsideObjName = "OutsideWall_North_Horizontal";
				posZ = (currentFloorMapData.floorSizeVertical - 1) * unitSize.z + outSideWallOffsetZ;
				pos = new Vector3(posX, height, posZ);
				InstantiateOutsideWalls(pos, scale, outsideObjName);

				// Vertical walls - West
				outsideObjName = "OutsideWall_West_Vertial";
				scale = new Vector3(unitSize.x * outsideWallSize, unitSize.y, unitSize.z * currentFloorMapData.floorSizeVertical);
				posX = -1 * outSideWallOffsetX;
				posZ = baseOffset.z + (scale.z + outsideWallSize * 2 * unitSize.z) / 2 - outSideWallOffsetZ * 2;
				pos = new Vector3(posX, height, posZ);
				InstantiateOutsideWalls(pos, scale, outsideObjName);

				// Vertical walls - East
				outsideObjName = "OutsideWall_East_Vertial";
				posX = (currentFloorMapData.floorSizeHorizontal - 1) * unitSize.x + outSideWallOffsetX;
				pos = new Vector3(posX, height, posZ);
				InstantiateOutsideWalls(pos, scale, outsideObjName);
			}
		}

		/// <Summary>
		/// Receiver of OnDraw message.
		/// </Summary>
		public void OnDraw(){
			GetSettings();
			DrawWalls();
		}

		/// <Summary>
		/// Receiver of OnRedraw message.
		/// Before instantiate new ceiling object, this method calls RemoveChildObjects() method.
		/// </Summary>
		public void OnRedraw(){
			GetSettings();
			RemoveChildObjects();
			DrawWalls();
		}

		/// <Summary>
		/// Remove dungeon walls.
		/// </Summary>
		public void OnRemoveObjects(){
			RemoveChildObjects();
		}

		/// <Summary>
		/// Instantiate dungeon walls and objects.
		/// </Summary>
		/// <param name="mapAttrId">The map attribute id of the position.</param>
		/// <param name="pos">The position of the object.</param>
		/// <param name="objName">The name of the object.</param>
		/// <param name="rotation">The rotation of the object.</param>
		/// <param name="messengerTypeId">Messenger type ID in DungeonPartsMasterData.</param>
		/// <param name="index">Index of the position in MapInfo list.</param>
		void InstantiateWall(int mapAttrId, Vector3 pos, string objName, Vector3 rotation, int messengerTypeId, int index){
			if (mapAttrId == MapAttributeDefine.HALL_WAY){
				return;
			}
			string partsName = "";
			GameObject prefab = dungeonParts.wallObj;

			if (mapAttrId == MapAttributeDefine.WALL){
				partsName = "WallParts";
				
			} else if (mapAttrId == MapAttributeDefine.DOOR){
				partsName = "DoorParts";
				prefab = dungeonParts.doorObj;

			} else if (mapAttrId == MapAttributeDefine.LOCKED_DOOR){
				partsName = "LockedDoorParts";
				prefab = dungeonParts.lockedDoorObj;

			} else if (mapAttrId == MapAttributeDefine.DOWNSTAIRS){
				partsName = "DownstairsParts";
				prefab = dungeonParts.downstairsObj;

			} else if (mapAttrId == MapAttributeDefine.UPSTAIRS){
				partsName = "UpstairsParts";
				prefab = dungeonParts.upstairsObj;

			} else if (mapAttrId == MapAttributeDefine.TREASURE){
				partsName = "TreasureParts";
				prefab = dungeonParts.treasureObj;
				pos.y = 0f;

			} else if (mapAttrId == MapAttributeDefine.MESSENGER){
				partsName = "MessengerParts";
				prefab = GetMessengerByTypeId(dungeonParts.messengerObjects, messengerTypeId, objName);
				pos.y = 0f;

			} else if (mapAttrId == MapAttributeDefine.PILLAR){
				partsName = "PillarParts";
				prefab = dungeonParts.pillarObj;

			} else if (mapAttrId == MapAttributeDefine.WALL_WITH_TORCH){
				partsName = "WallWithTorchParts";
				prefab = dungeonParts.wallWithTorchObj;
			}

			if (partsName == "" || prefab == null){
				return;
			}
			GameObject wall = (GameObject) Instantiate(prefab, pos, Quaternion.identity);
			wall.transform.SetParent(gameObject.transform);
			wall.transform.Rotate(rotation);
			wall.name = objName;

			if (mapAttrId == MapAttributeDefine.TREASURE){
				SetTreasureBoxState(wall, index);
			}
		}

		/// <Summary>
		/// Set treasure box open state by checking event flags.
		/// When all flags in the event are true, open the treasure box.
		/// </Summary>
		/// <param name="treasureBoxObj">The treasure box object to instantiate.</param>
		/// <param name="index">Index of the position in MapInfo list.</param>
		void SetTreasureBoxState(GameObject treasureBoxObj, int index){
			TreasureAnimator treasureAnim = treasureBoxObj.GetComponent<TreasureAnimator>();
			if (treasureAnim == null){
				return;
			}

			EventMasterData eventData = null;
			int eventId = currentFloorMapData.mapInfo[index].eventId;
			if (eventId > 0){
				string path = "EventData/event_" + eventId.ToString("D5");
				eventData = (EventMasterData)Resources.Load(path);
			}
			if (eventData == null){
				return;
			}

			bool isOpend = false;
			AriadneEventParts parts = null;
			for (int i = eventData.eventParts.Count - 1; i >= 0; i--){
				parts = eventData.eventParts[i];
				if (parts == null){
					continue;
				}
				if (parts.hasExecutedFlag && !string.IsNullOrEmpty(parts.executedFlagName)){
					isOpend = FlagManager.CheckEventFlag(parts.executedFlagName);
				}
			}
			if (!isOpend){
				return;
			}

			treasureAnim.OpenTreasureBoxImmediately();
		}

		/// <Summary>
		/// Instantiate dungeon outside walls.
		/// </Summary>
		/// <param name="pos">The position of the wall.</param>
		/// <param name="scale">The scale of the wall.</param>
		/// <param name="objName">The name of the wall.</param>
		void InstantiateOutsideWalls(Vector3 pos, Vector3 scale, string objName){
			GameObject prefab = dungeonParts.wallObj;
			GameObject wall = (GameObject) Instantiate(prefab, pos, Quaternion.identity);
			wall.transform.SetParent(gameObject.transform);
			wall.transform.localScale = scale;
			wall.name = objName;

			int tile = 2;
			int texScaleX = tile * outsideWallSize;
			int texScaleZ = tile;
			if (scale.x >= scale.z){
				texScaleX = (currentFloorMapData.floorSizeHorizontal + outsideWallSize * 2) * tile;
			} else {
				texScaleX = currentFloorMapData.floorSizeVertical * tile;
			}
			wall.GetComponent<Renderer>().material.mainTextureScale = new Vector2(texScaleX, texScaleZ);
		}

		/// <Summary>
		/// Returns a GameObject which corresponds to the messenger type ID.
		/// </Summary>
		/// <param name="messengerList">Messenger list in dungeon parts data.</param>
		/// <param name="typeId">Messenger type ID.</param>
		/// <param name="objName">Name of the object to instantiate.</param>
		GameObject GetMessengerByTypeId(List<GameObject> messengerList, int typeId, string objName){
			GameObject messenger = null;
			if (messengerList == null){
				Debug.LogError("The list of messenger object in this DungeonPartsData is null. Assign messenger objects.");
				return messenger;
			}
			if (typeId >= messengerList.Count){
				Debug.LogError("Specified messenger type ID is invalid in this DungeonPartsData. Messenger type ID : " + typeId + ", Name : " + objName);
				return messenger;
			}
			messenger = messengerList[typeId];
			if (messenger == null){
				Debug.LogError("Messenger object of specified type ID is not assigned in this DungeonPartsData. Messenger type ID : " + typeId + ", Name : " + objName);
			}
			return messenger;
		}
	}
}
