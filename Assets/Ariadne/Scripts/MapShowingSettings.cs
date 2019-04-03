using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Holds settings about showing maps.
	/// </Summary>
	public class MapShowingSettings : MonoBehaviour {

		[Range(1, 10)]
		public int showLengthHorizontal = 4;

		[Range(1, 10)]
		public int showLengthVertical = 4;
		[Range(1, 10)]
		public int smoothness = 2;

		[Range(0f, 10f)]
		public float gridLineWidth = 1f;
		
	}
}
