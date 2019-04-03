using UnityEngine.EventSystems;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// The interface of event processor.
	/// </Summary>
	public interface IEventProcessor : IEventSystemHandler {

		/// <Summary>
		/// Send an event finished message.
		/// </Summary>
		void OnPostEvent();

		/// <Summary>
		/// Send a move message in the event of opening door.
		/// </Summary>
		void OnDoorOpen();

		/// <Summary>
		/// Send a move finished message in the event of the opening door.
		/// </Summary>
		void OnPostMove();

		/// <Summary>
		/// Send a message to start the event of move position.
		/// </Summary>
		/// <param name="eventParts">The event parts data.</param>
		void OnMovePosition(AriadneEventParts eventParts);

		/// <Summary>
		/// Send a exiting the dungeon message.
		/// </Summary>
		void OnExitDungeon();
	}
}