using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ariadne {

	/// <Summary>
	/// Editor script for dungeon data.
	/// </Summary>

	[CustomEditor(typeof(DungeonMasterData))]
	public class DungeonMasterDataEditor : Editor {

		/// <Summary>
		/// Show floor selector on inspector.
		/// </Summary>
		public override void OnInspectorGUI(){
			var dungeonData = target as DungeonMasterData;

			DrawDefaultInspector();

			if (dungeonData.floorList == null){
				return;
			}
			string[] displayName = GetFloorNameArray(dungeonData.floorList);
			int[] optionValue = GetFloorIdArray(dungeonData.floorList);
			if (displayName != null && optionValue != null && displayName.Length != 0 && optionValue.Length != 0){
				Undo.RecordObject(dungeonData, "Change Start Floor");
				dungeonData.startFloorId = EditorGUILayout.IntPopup("Start Floor", dungeonData.startFloorId, displayName, optionValue);
			}
		}

		/// <Summary>
		/// Returns floor name array to show the selector.
		/// </Summary>
		/// <param name="floorList">List of floors assigned to the dungeon data.</param>
		string[] GetFloorNameArray(List<FloorMapMasterData> floorList){
			if (floorList.Count == 0){
				return null;
			}
			string[] floorNameArray = new string[floorList.Count];
			for (int i = 0; i < floorList.Count; i++){
				if (floorList[i] != null){
					floorNameArray[i] = floorList[i].floorName;
				}
			}
			return floorNameArray;
		}

		/// <Summary>
		/// Returns floor ID array to show the selector.
		/// </Summary>
		/// <param name="floorList">List of floors assigned to the dungeon data.</param>
		int[] GetFloorIdArray(List<FloorMapMasterData> floorList){
			if (floorList.Count == 0){
				return null;
			}
			int[] floorIdArray = new int[floorList.Count];
			for (int i = 0; i < floorList.Count; i++){
				if (floorList[i] != null){
					floorIdArray[i] = floorList[i].floorId;
				}
			}
			return floorIdArray;
		}
	}
}

