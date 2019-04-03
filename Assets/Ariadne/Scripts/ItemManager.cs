using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Manager class of holding items.
	/// </Summary>
	public static class ItemManager {

		public const string ITEM_NAME_NONE = "None";
		public static Dictionary<int, int> holdItemDict;
		public static int money;

		/// <Summary>
		/// Set amount of an item that is specified by item ID.
		/// </Summary>
		/// <param name="itemId">Specify item ID.</param>
		/// <param name="delta">Delta num of holding item.</param>
		public static void SetHoldItemList(int itemId, int delta){
			if (holdItemDict == null){
				InitializeHoldItemDict();
			}

			if (holdItemDict.ContainsKey(itemId)){
				int itemNum = holdItemDict[itemId];
				itemNum += delta;
				if (itemNum < 0){
					itemNum = 0;
				}
				holdItemDict[itemId] = itemNum;
			} else {
				int itemNum = delta > 0 ? delta : 0;
				holdItemDict.Add(itemId, itemNum);
			}
		}

		/// <Summary>
		/// Set a new item dictionary.
		/// Searches all item data under the Resource/ItemData directory
		/// and register it to the dictionary.
		/// </Summary>
		public static void InitializeHoldItemDict(){
			if (holdItemDict != null){
				return;
			}

			holdItemDict = new Dictionary<int, int>();
			ItemMasterData[] itemList = (ItemMasterData[])Resources.LoadAll<ItemMasterData>("ItemData");
			foreach (ItemMasterData itemData in itemList){
				if (itemData != null){
					holdItemDict.Add(itemData.itemId, 0);
				}
			}
		}

		/// <Summary>
		/// Returns the item name that is specified by item ID.
		/// </Summary>
		/// <param name="itemId">Specify item ID.</param>
		public static string GetItemNameById(int itemId){
			string itemName = ITEM_NAME_NONE;
			ItemMasterData item = (ItemMasterData)Resources.Load("ItemData/item_" + itemId.ToString("D5"));
			if (item != null){
				itemName = item.itemName;
			}
			return itemName;
		}

		/// <Summary>
		/// Returns the checking result of key type.
		/// </Summary>
		/// <param name="keyType">Key type of the key.</param>
		public static bool CheckCorrespondingKey(DoorKeyType keyType){
			bool hasDoorKey = false;
			foreach (KeyValuePair<int, int> pair in holdItemDict){
				if (pair.Value <= 0){
					continue;
				}

				ItemMasterData item = (ItemMasterData)Resources.Load("ItemData/item_" + pair.Key.ToString("D5"));
				if (item == null){
					continue;
				}

				if (item.itemType == AriadneItemType.Key && item.doorKeyType == keyType){
					hasDoorKey = true;
				}
			}
			return hasDoorKey;
		}

		/// <Summary>
		/// Returns the result of checking condition about holding items.
		/// </Summary>
		/// <param name="itemId">Specify item ID.</param>
		/// <param name="comparison">The comparison operator of the event parts.</param>
		/// <param name="compareNum">The number of conparing to.</param>
		public static bool CheckEventConditionItem(int itemId, AriadneComparison comparison, int compareNum){
			bool isMatched = false;
			if (holdItemDict == null){
				return isMatched;
			}

			int holdingItemNum = holdItemDict.ContainsKey(itemId) ? holdItemDict[itemId] : 0;
			isMatched = GetComparedResult(comparison, holdingItemNum, compareNum);

			return isMatched;
		}

		/// <Summary>
		/// Returns the result of checking condition about money.
		/// </Summary>
		/// <param name="comparison">The comparison operator of the event parts.</param>
		/// <param name="compareNum">The criterion number of conparing to.</param>
		public static bool CheckEventConditionMoney(AriadneComparison comparison, int compareNum){
			bool isMatched = false;
			
			isMatched = GetComparedResult(comparison, ItemManager.money, compareNum);

			return isMatched;
		}

		/// <Summary>
		/// Returns the checking result of the condition by using a comparison of the event parts data.
		/// </Summary>
		/// <param name="comparison">The comparison operator of the event parts.</param>
		/// <param name="value">The value of player's item or money.</param>
		/// <param name="compareNum">The criterion number of conparing to.</param>
		static bool GetComparedResult(AriadneComparison comparison, int value, int compareNum){
			bool isMatched = false;
			switch (comparison){
				case AriadneComparison.Equals:
					if (value == compareNum){
						isMatched = true;
					}
					break;
				case AriadneComparison.NotEqual:
					if (value != compareNum){
						isMatched = true;
					}
					break;
				case AriadneComparison.GreaterThan:
					if (value > compareNum){
						isMatched = true;
					}
					break;
				case AriadneComparison.GreaterOrEqual:
					if (value >= compareNum){
						isMatched = true;
					}
					break;
				case AriadneComparison.LessThan:
					if (value < compareNum){
						isMatched = true;
					}
					break;
				case AriadneComparison.LessOrEqual:
					if (value <= compareNum){
						isMatched = true;
					}
					break;
			}
			return isMatched;
		}
	}
}