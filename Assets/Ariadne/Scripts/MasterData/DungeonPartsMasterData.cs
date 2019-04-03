using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Definition of dungeon parts data.
	/// </Summary>
	[CreateAssetMenu(fileName = "parts_", menuName = "Ariadne/DungeonPartsData")]
	public class DungeonPartsMasterData : ScriptableObject {

		public GameObject wallObj;
		public GameObject groundObj;
		public GameObject ceilingObj;
		public GameObject doorObj;
		public GameObject lockedDoorObj;
		public GameObject upstairsObj;
		public GameObject downstairsObj;
		public GameObject treasureObj;
		public List<GameObject> messengerObjects;
		public GameObject pillarObj;
		public GameObject wallWithTorchObj;
	}

}

