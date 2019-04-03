using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Definition of dungeon data.
	/// </Summary>
	[CreateAssetMenu(fileName = "dungeon_", menuName = "Ariadne/DungeonData")]
	public class DungeonMasterData : ScriptableObject {

		public int dungeonId;
		public string dungeonName;
		public List<FloorMapMasterData> floorList;
		public int startFloorId;
	}
}

