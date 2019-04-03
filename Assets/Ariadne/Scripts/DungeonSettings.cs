using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Holds settings about dungeons.
	/// </Summary>
	public class DungeonSettings : MonoBehaviour, IDungeonSetter {
		public DungeonMasterData dungeonData;
		public bool isDrawOutsideWall = true;
		[Range (1, 10)]
		public int outsideWallSize = 1;

		/// <Summary>
		/// Returns a floor data which corresponds to the floor ID stored in PlayerPosition.
		/// </Summary>
		public FloorMapMasterData GetCurrentFloorData(){
			if (dungeonData.floorList == null){
				ShowAssignErrorMessage(dungeonData.name);
				return null;
			}

			FloorMapMasterData floorData = null;
			if (dungeonData.floorList.Count > 0){
				floorData = dungeonData.floorList[0];
				foreach (FloorMapMasterData floor in dungeonData.floorList){
					if (floor == null){
						continue;
					}
					if (floor.floorId == PlayerPosition.currentFloorId){
						floorData = floor;
						break;
					}
				}
			}

			if (floorData == null){
				ShowAssignErrorMessage(dungeonData.name);
			}
			return floorData;
		}

		/// <Summary>
		/// Show error message about assigning floor data.
		/// </Summary>
		/// <param name="dataName">The name of dungeon data.</param>
		void ShowAssignErrorMessage(string dataName){
			Debug.LogError("Floor data is not assigned to dungeon data " + dataName + ".");
		}

		/// <Summary>
		/// Set a new dungeon data to DungeonSetting component.
		/// </Summary>
		/// <param name="dungeonData">New dungeon data.</param>
		public void OnSetDungeon(DungeonMasterData dungeonData){
			this.dungeonData = dungeonData;
		}
	}
}