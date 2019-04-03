using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	public static class FloorMapConst {
		public const int INIT_HORIONTAL_SIZE = 6;
		public const int INIT_VERTICAL_SIZE = 6;
	}

	/// <Summary>
	/// Definition of each position data.
	/// </Summary>
	[System.Serializable]
	public class MapInfo {
		public int eventId;
		public int mapAttr;
		public DungeonDir objectFront;
		public int messengerType;
	}

	/// <Summary>
	/// Definition of each floor data.
	/// </Summary>
	public class FloorMapMasterData : ScriptableObject {

		public int floorId;
		public string floorName;
		public DungeonPartsMasterData dungeonParts;
		public int floorSizeHorizontal;
		public int floorSizeVertical;
		public Vector2Int entrancePos;
		public DungeonDir enteringDir;
		public List<MapInfo> mapInfo;
	}

}
