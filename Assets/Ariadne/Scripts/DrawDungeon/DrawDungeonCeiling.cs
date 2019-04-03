using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Drawing dungeon ceiling.
	/// </Summary>
	public class DrawDungeonCeiling : DrawDungeon, IDrawer {

		[SerializeField]
		bool isDrawCeiling = true;

		/// <Summary>
		/// Instantiate dungeon ceiling.
		/// </Summary>
		void DrawCeiling(){
			Vector3 basePos = Vector3.zero;
			GameObject ceilingPrefab = dungeonParts.ceilingObj;

			// Get height of wall
			GameObject wallPrefab = dungeonParts.wallObj;
			Vector3 unitSize = new Vector3(wallPrefab.transform.localScale.x, wallPrefab.transform.localScale.y, wallPrefab.transform.localScale.z);
			
			outsideWallSize = isDrawOutsideWall ? outsideWallSize : 0;
			float sizeX = (currentFloorMapData.floorSizeHorizontal + outsideWallSize * 2) * unitSize.x;
			float sizeZ = (currentFloorMapData.floorSizeVertical + outsideWallSize * 2) * unitSize.z;
			Vector3 ceilingSize = new Vector3(sizeX, ceilingPrefab.transform.localScale.y, sizeZ);

			float outSideWallOffsetX = outsideWallSize * unitSize.x;
			float outSideWallOffsetZ = outsideWallSize * unitSize.z;
			float posX = (basePos.x + ceilingSize.x - unitSize.x) / 2 - outSideWallOffsetX;
			float posZ = (basePos.z + ceilingSize.z - unitSize.z) / 2 - outSideWallOffsetZ;
			float height = unitSize.y + ceilingPrefab.transform.localScale.y;
			Vector3 ceilingPos = new Vector3(posX, height, posZ);

			GameObject ceiling = (GameObject) Instantiate(ceilingPrefab, ceilingPos, Quaternion.identity);
			ceiling.transform.localScale = ceilingSize;
			ceiling.transform.SetParent(gameObject.transform);
			ceiling.name = "Ceiling";

			int tile = 2;
			int texScaleX = (currentFloorMapData.floorSizeHorizontal + outsideWallSize * 2) * tile;
			int texScaleZ = (currentFloorMapData.floorSizeVertical + outsideWallSize * 2) * tile;
			ceiling.GetComponent<Renderer>().material.mainTextureScale = new Vector2(texScaleX, texScaleZ);
		}

		/// <Summary>
		/// Receiver of OnDraw message.
		/// </Summary>
		public void OnDraw(){
			if (isDrawCeiling){
				GetSettings();
				DrawCeiling();
			}
		}

		/// <Summary>
		/// Receiver of OnRedraw message.
		/// Before instantiate new ceiling object, this method calls RemoveChildObjects() method.
		/// </Summary>
		public void OnRedraw(){
			if (isDrawCeiling){
				GetSettings();
				RemoveChildObjects();
				DrawCeiling();
			}
		}

		/// <Summary>
		/// Remove dungeon ceiling.
		/// </Summary>
		public void OnRemoveObjects(){
			RemoveChildObjects();
		}
	}
}