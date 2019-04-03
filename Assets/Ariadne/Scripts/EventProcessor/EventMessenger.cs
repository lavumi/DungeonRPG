using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Event process of messenger.
	/// </Summary>
	public class EventMessenger : AriadneEventStrategyBase, IAriadneEventStrategy {

		/// <Summary>
		/// Execute method of event process.
		/// </Summary>
		/// <param name="controller">Game controller object.</param>
		/// <param name="eventPos">The position of the event.</param>
		/// <param name="eventParts">The event parts data.</param>
		public void ExecuteEvent(GameObject controller, Vector2Int eventPos, AriadneEventParts parts){
			gameController = controller;
			this.sendShowingMsgList = parts.msgList;
			this.sendParts = parts;
			SendShowingMessage(gameController);
		}
	}
}

