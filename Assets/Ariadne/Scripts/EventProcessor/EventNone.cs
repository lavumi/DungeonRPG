using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Event process of None type which does nothing.
	/// </Summary>
	public class EventNone : AriadneEventStrategyBase, IAriadneEventStrategy {

		/// <Summary>
		/// Execute method of event process.
		/// </Summary>
		/// <param name="controller">Game controller object.</param>
		/// <param name="eventPos">The position of the event.</param>
		/// <param name="eventParts">The event parts data.</param>
		public void ExecuteEvent(GameObject controller, Vector2Int eventPos, AriadneEventParts parts){
			gameController = controller;
			PostEvent(parts);
		}
	}
}
