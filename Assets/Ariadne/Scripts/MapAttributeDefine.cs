using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Definitions of map attribute.
	/// </Summary>
	public static class MapAttributeDefine{

		public const int HALL_WAY = 0;
		public const int WALL = 1;
		public const int DOOR = 2;
		public const int LOCKED_DOOR = 3;
		public const int DOWNSTAIRS = 4;
		public const int UPSTAIRS = 5;
		public const int TREASURE = 6;
		public const int MESSENGER = 7;
		public const int PILLAR = 8;
		public const int WALL_WITH_TORCH = 9;

		/// <Summary>
		/// Definitions of map attribute.
		/// </Summary>
		/// <param name="attrId">Specify map attribute ID.</param>
		public static string GetAttrNameById(int attrId){
			string attrName = "";
			switch (attrId){
				case WALL:
					attrName = "Wall";
					break;
				case DOOR:
					attrName = "Door";
					break;
				case LOCKED_DOOR:
					attrName = "LockedDoor";
					break;
				case DOWNSTAIRS:
					attrName = "Downstairs";
					break;
				case UPSTAIRS:
					attrName = "Upstairs";
					break;
				case TREASURE:
					attrName = "Treasure";
					break;
				case MESSENGER:
					attrName = "Messenger";
					break;
				case PILLAR:
					attrName = "Pillar";
					break;
				case WALL_WITH_TORCH:
					attrName = "WallWithTorch";
					break;
				default:
					attrName = "Hallway";
					break;
			}
			return attrName;
		}
	}
}

