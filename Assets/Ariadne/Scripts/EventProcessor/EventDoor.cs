using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Event process of opening the door.
	/// </Summary>
	public class EventDoor : EventDoorBase, IAriadneEventStrategy, IDoorOpen {

		/// <Summary>
		/// Execute method of event process.
		/// </Summary>
		/// <param name="controller">Game controller object.</param>
		/// <param name="eventPos">The position of the event.</param>
		/// <param name="eventParts">The event parts data.</param>
		public void ExecuteEvent(GameObject controller, Vector2Int eventPos, AriadneEventParts parts){
			gameController = controller;
			GameObject doorObj = GetEventObj(eventPos, MapAttributeDefine.DOOR);
			if (doorObj != null){
				StartCoroutine(OpenDoor(doorObj, parts));
			} else {
				MissingEventObjectError(eventPos);
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