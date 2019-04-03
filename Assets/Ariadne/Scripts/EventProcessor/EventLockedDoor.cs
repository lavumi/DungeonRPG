using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Event process of opening the locked door.
	/// </Summary>
	public class EventLockedDoor : EventDoorBase, IAriadneEventStrategy, IDoorOpen {

		/// <Summary>
		/// Execute method of event process.
		/// </Summary>
		/// <param name="controller">Game controller object.</param>
		/// <param name="eventPos">The position of the event.</param>
		/// <param name="eventParts">The event parts data.</param>
		public void ExecuteEvent(GameObject controller, Vector2Int eventPos, AriadneEventParts parts){
			gameController = controller;
			GameObject doorObj = GetEventObj(eventPos, MapAttributeDefine.LOCKED_DOOR);
			if (doorObj != null){
				OpenLockedDoor(doorObj, parts);
			} else {
				MissingEventObjectError(eventPos);
			}
		}

		/// <Summary>
		/// Key type checking of the locked door.
		/// </Summary>
		/// <param name="doorObj">The door object to animate.</param>
		/// <param name="eventParts">The event parts data.</param>
		void OpenLockedDoor(GameObject doorObj, AriadneEventParts eventParts){

			// Check the key type of door
			DoorKeyType keyType = eventParts.doorKeyType;
			bool isKeyMatched = ItemManager.CheckCorrespondingKey(keyType);

			// If the key type was None, treat the door as unlocked.
			if (keyType == DoorKeyType.None){
				isKeyMatched = true;
			}

			if (isKeyMatched){
				StartCoroutine(OpenDoor(doorObj, eventParts));
			} else {
				string msg = "You don't have any corresponding key.";
				List<string> msgList = new List<string>();
				msgList.Add(msg);

				this.sendShowingMsgList = msgList;
				this.sendParts = eventParts;
				SendShowingMessage(gameController);
			}
		}

		/// <Summary>
		/// Receiver of the notification which is moving during the door opening event from MoveController.
		/// </Summary>
		public void OnMoveFinished(){
			isWaitingMove = false;
		}
	}
}