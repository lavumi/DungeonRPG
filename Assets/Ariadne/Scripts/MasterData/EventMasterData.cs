using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Definition of event parts data.
	/// </Summary>
	[System.Serializable]
	public class AriadneEventParts {
		public AriadneEventCategory eventCategory;
		public AriadneEventTrigger eventTrigger;
		public AriadneEventPosition eventPos;

		// event flag
		public AriadneEventCondition startCondition;
		public string startFlagName;
		public int startItemId;
		public int startNum;
		public AriadneComparison comparisonOperator;

		public bool hasExecutedFlag;
		public string executedFlagName;

		// for locked door
		public DoorKeyType doorKeyType;

		// for move position
		public DungeonMasterData destDungeon;
		public FloorMapMasterData destMap;
		public Vector2Int destPos;
		public DungeonDir direction;

		// for treasure
		public TreasureType treasureType;
		public int itemId;
		public int itemNum;

		// for messenger
		public List<string> msgList;

		/// <Summary>
		/// Constructor of event parts data.
		/// </Summary>
		public AriadneEventParts(){
			msgList = new List<string>();
		}

		/// <Summary>
		/// Copy constructor of base parts.
		/// </Summary>
		/// <param name="baseParts">Base parts data for copying.</param>
		public AriadneEventParts(AriadneEventParts baseParts){
			eventCategory = baseParts.eventCategory;
			eventTrigger = baseParts.eventTrigger;
			eventPos = baseParts.eventPos;

			startCondition = baseParts.startCondition;
			startFlagName = baseParts.startFlagName;
			startItemId = baseParts.startItemId;
			startNum = baseParts.startNum;
			comparisonOperator = baseParts.comparisonOperator;

			hasExecutedFlag = baseParts.hasExecutedFlag;
			executedFlagName = baseParts.executedFlagName;

			doorKeyType = baseParts.doorKeyType;

			destDungeon = baseParts.destDungeon;
			destMap = baseParts.destMap;
			destPos = baseParts.destPos;
			direction = baseParts.direction;

			treasureType = baseParts.treasureType;
			itemId = baseParts.itemId;
			itemNum = baseParts.itemNum;

			msgList = new List<string>();
			foreach (string msg in baseParts.msgList){
				msgList.Add(msg);
			}
		}
	}

	/// <Summary>
	/// Definition of event data.
	/// </Summary>
	public class EventMasterData : ScriptableObject {

		public int eventId;
		public string eventName;
		public List<AriadneEventParts> eventParts;

	}
}

