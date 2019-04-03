using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Definition of event mapping data.
	/// </Summary>
	public class EventMappingData {
		
		public int eventId;
		public string eventName;
		public int eventIndex;
		public string startCondition;
		public string eventCategory;
		public string mapFileName;
		public string mapName;
		public Vector2Int pos;

		/// <Summary>
		/// Constructor of the EventMappingData.
		/// </Summary>
		public EventMappingData(int id, string eventName, int eventIndex, AriadneEventCondition cond, AriadneEventCategory eventCat, string mapFile, string mapName, Vector2Int pos){
			this.eventId = id;
			this.eventName = eventName;
			this.eventIndex = eventIndex;
			this.startCondition = cond.ToString();
			this.eventCategory = eventCat.ToString();
			this.mapFileName = mapFile;
			this.mapName = mapName;
			this.pos = pos;
		}
		
	}
}

