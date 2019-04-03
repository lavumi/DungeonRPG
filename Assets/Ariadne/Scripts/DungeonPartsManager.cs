using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// When dungeon parts data is missing, set default dungeon parts data.
	/// Dungeon parts data is used for drawing dungeon.
	/// </Summary>
	public static class DungeonPartsManager {

		const string path = "DungeonPartsData/";
		const string fileName = "DefaultParts";

		/// <Summary>
		/// Set the default dungeon parts.
		/// </Summary>
		public static DungeonPartsMasterData GetDefaultDungeonParts(){
			DungeonPartsMasterData dungeonParts = Resources.Load<DungeonPartsMasterData>(path + fileName);

			if (dungeonParts == null){
				Debug.LogWarning("Dungeon parts prefab 'DefaultParts' is missing...");
			}

			return dungeonParts;
		}
	}
}

