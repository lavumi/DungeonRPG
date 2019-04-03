using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// Base class of door opening events.
	/// </Summary>
	public class EventDoorBase : AriadneEventStrategyBase {

		protected float doorOpenTime = 1.0f;
		protected bool isWaitingMove;

		/// <Summary>
		/// Door opening process.
		/// </Summary>
		/// <param name="doorObj">The door object to animate.</param>
		/// <param name="eventParts">The event parts data.</param>
		protected IEnumerator OpenDoor(GameObject doorObj, AriadneEventParts eventParts){
			// Get door animatior component.
			DoorAnimator doorAnim = doorObj.GetComponent<DoorAnimator>();

			if (doorAnim != null){
				// Send door open message
				doorAnim.OpenDoor();

				// Wait until door opend
				yield return new WaitForSeconds(doorOpenTime);

				// Move 2 step
				SendOpenDoorAndMoveMessage(gameController);

				float wait = 0.1f;
				isWaitingMove = true;
				while (isWaitingMove){
					yield return new WaitForSeconds(wait);
				}

				// Close door
				doorAnim.CloseDoor();
			} else {
				AnimatorWarning(doorObj);
			}
			PostEvent(eventParts);

			// Check if that event exists on the new position after moving.
			SendPostOpenDoorMessage(gameController);
		}

		/// <Summary>
		/// Send a message for moving the player to MoveController.
		/// </Summary>
		/// <param name="obj">Specify the game controller object.</param>
		protected void SendOpenDoorAndMoveMessage(GameObject obj){
			ExecuteEvents.Execute<IEventProcessor>(
				target: obj,
				eventData: null,
				functor: EventOpenDoor
			);
		}

		/// <Summary>
		/// The functor of SendOpenDoorAndMoveMessage method.
		/// </Summary>
		protected void EventOpenDoor(IEventProcessor eventProcessor, BaseEventData eventData){
			eventProcessor.OnDoorOpen();
		}

		/// <Summary>
		/// Send a message to EventProcessor to notify the finishing of the event.
		/// </Summary>
		/// <param name="obj">Specify the game controller object.</param>
		protected void SendPostOpenDoorMessage(GameObject obj){
			ExecuteEvents.Execute<IEventProcessor>(
				target: obj,
				eventData: null,
				functor: PostOpenDoor
			);
		}

		/// <Summary>
		/// The functor of SendPostOpenDoorMessage method.
		/// </Summary>
		protected void PostOpenDoor(IEventProcessor eventProcessor, BaseEventData eventData){
			eventProcessor.OnPostMove();
		}
	}
}