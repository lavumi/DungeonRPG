using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne{

	/// <Summary>
	/// Definition of traverse data.
	/// </Summary>
	public class TraverseData {

		public int dungeonId;
		public Dictionary<string, bool> traverseDict;

		/// <Summary>
		/// Constructor of the TraverseData.
		/// </Summary>
		public TraverseData(int dungeonId){
			this.dungeonId = dungeonId;
			traverseDict = new Dictionary<string, bool>();
		}
	}
}

