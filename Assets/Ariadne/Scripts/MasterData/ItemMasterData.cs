using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne{
	
	/// <Summary>
	/// Definition of item data.
	/// </Summary>
	[CreateAssetMenu(fileName = "item_", menuName = "Ariadne/ItemData")]
	public class ItemMasterData : ScriptableObject {
		public AriadneItemType itemType;
		public int itemId;
		public string itemName;
		public DoorKeyType doorKeyType;
	}
}