using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Manager class of dungeon traverse data.
	/// </Summary>
	public static class TraverseManager {
		static List<TraverseData> traverseList;

		/// <Summary>
		/// Initialize traverse data list.
		/// </Summary>
		public static void InitializeTraverseData(){
			if (traverseList == null){
				traverseList = new List<TraverseData>();
			}
		}

		/// <Summary>
		/// Add traverse data set of the new floor.
		/// </Summary>
		/// <param name="dungeonId">Sepcify the dungeon ID.</param>
		/// <param name="floorId">Floor ID in the dungeon.</param>
		/// <param name="floorData">FloorMapData for adding traverse data.</param>
		public static void AddDungeonTraverseData(int dungeonId, int floorId, FloorMapMasterData floorData){
			TraverseData td = GetDungeonTraverseData(dungeonId);
			if (td == null){
				td = new TraverseData(dungeonId);
				traverseList.Add(td);
			}
			for (int y = 0; y < floorData.floorSizeVertical; y++){
				for (int x = 0; x < floorData.floorSizeHorizontal; x++){
					Vector2Int pos = new Vector2Int(x, y);
					string key = GetTraverseKeyString(floorId, pos);
					if (!td.traverseDict.ContainsKey(key)){
						td.traverseDict.Add(key, false);
					}
				}
			}
		}

		/// <Summary>
		/// Generates the key string of traverse dictionary.
		/// </Summary>
		/// <param name="floorId">Floor ID in the dungeon.</param>
		/// <param name="pos">Target position of traverse data.</param>
		static string GetTraverseKeyString(int floorId, Vector2Int pos){
			string key = floorId.ToString() + "-" + pos.x.ToString() + "-" + pos.y.ToString();
			return key;
		}

		/// <Summary>
		/// Returns traverse data that correspond to the dungeon ID.
		/// </Summary>
		/// <param name="dungeonId">Sepcify the dungeon ID.</param>
		public static TraverseData GetDungeonTraverseData(int dungeonId){
			TraverseData dungeonTraverseData = null;
			if (traverseList != null){
				dungeonTraverseData = traverseList.Find((td) => td.dungeonId == dungeonId);
			}
			return dungeonTraverseData;
		}

		/// <Summary>
		/// Returns traverse data of the specified position.
		/// </Summary>
		/// <param name="dungeonId">Sepcify the dungeon ID.</param>
		/// <param name="floorId">Floor ID in the dungeon.</param>
		/// <param name="pos">Target position of traverse data.</param>
		public static bool GetPositionTraverseData(int dungeonId, int floorId, Vector2Int pos){
			bool isTraversed = false;
			if (traverseList == null){
				return isTraversed;
			}
			TraverseData dungeonTraverseData = GetDungeonTraverseData(dungeonId);
			if (dungeonTraverseData == null){
				return isTraversed;
			}
			string key = GetTraverseKeyString(floorId, pos);
			if (dungeonTraverseData.traverseDict.ContainsKey(key)){
				isTraversed = dungeonTraverseData.traverseDict[key];
			}
			
			return isTraversed;
		}

		/// <Summary>
		/// Set traverse data of specified position.
		/// </Summary>
		/// <param name="dungeonId">Sepcify the dungeon ID.</param>
		/// <param name="floorId">Floor ID in the dungeon.</param>
		/// <param name="pos">Target position of traverse data.</param>
		/// <param name="flag">Traversed flag.</param>
		public static void SetTraverseData(int dungeonId, int floorId, Vector2Int pos, bool flag){
			// Set a traversed flag to traverse data in list.
			if (traverseList == null){
				return;
			}
			TraverseData dungeonTraverseData = GetDungeonTraverseData(dungeonId);
			if (dungeonTraverseData == null){
				return;
			}
			string key = GetTraverseKeyString(floorId, pos);
			if (dungeonTraverseData.traverseDict.ContainsKey(key)){
				dungeonTraverseData.traverseDict[key] = flag;
			}
		}
	}
}
