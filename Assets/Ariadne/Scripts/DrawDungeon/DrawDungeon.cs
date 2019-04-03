using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Base class of drawing dungeon classes.
	/// </Summary>
	public class DrawDungeon : MonoBehaviour {

		protected FloorMapMasterData currentFloorMapData;
		protected GameObject gameController;
		protected DungeonMasterData dungeonData;
		protected DungeonPartsMasterData dungeonParts;
		protected bool isDrawOutsideWall;
		protected int outsideWallSize;

		/// <Summary>
		/// Get settings such as dungeon data from DungeonSettings class.
		/// </Summary>
		protected void GetSettings(){
			gameController = GameObject.FindGameObjectWithTag("GameController");
			DungeonSettings ds = gameController.GetComponent<DungeonSettings>();
			dungeonData = ds.dungeonData;
			currentFloorMapData = ds.GetCurrentFloorData();
			dungeonParts = currentFloorMapData.dungeonParts;
			if (dungeonParts == null){
				DungeonPartsManager.GetDefaultDungeonParts();
			}
			isDrawOutsideWall = ds.isDrawOutsideWall;
			outsideWallSize = ds.outsideWallSize;
		}

		/// <Summary>
		/// Remove child objects of the parent of dungeon parts.
		/// </Summary>
		protected void RemoveChildObjects(){
			foreach (Transform child in gameObject.transform){
				GameObject childObj = child.gameObject;
				Destroy(childObj);
			}
		}
	}
}
