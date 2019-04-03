using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// Event process of exiting the dungeon.
	/// </Summary>
	public class EventExitDungeon : AriadneEventStrategyBase, IAriadneEventStrategy {

		/// <Summary>
		/// Execute method of event process.
		/// </Summary>
		/// <param name="controller">Game controller object.</param>
		/// <param name="eventPos">The position of the event.</param>
		/// <param name="eventParts">The event parts data.</param>
		public void ExecuteEvent(GameObject controller, Vector2Int eventPos, AriadneEventParts parts){
			gameController = controller;
			SendExitDungeonMessage(gameController);
		}

		/// <Summary>
		/// Send a message which notifies exiting the dungeon to MoveController.
		/// </Summary>
		/// <param name="obj">Specify the game controller object.</param>
		void SendExitDungeonMessage(GameObject obj){
			ExecuteEvents.Execute<IEventProcessor>(
				target: obj,
				eventData: null,
				functor: EventExit
			);
		}

		/// <Summary>
		/// The functor of SendExitDungeonMessage method.
		/// </Summary>
		void EventExit(IEventProcessor eventProcessor, BaseEventData eventData){
			eventProcessor.OnExitDungeon();
		}
	}
}