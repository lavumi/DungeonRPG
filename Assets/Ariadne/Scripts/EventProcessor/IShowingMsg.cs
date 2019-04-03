using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// The interface of showing message on messenger event.
	/// </Summary>
	public interface IShowingMsg : IEventSystemHandler {

		/// <Summary>
		/// Send a message to showing message UI.
		/// </Summary>
		/// <param name="msg">Message list to show.</param>
		/// <param name="eventParts">The event parts to execute.</param>
		void OnShowingMsg(List<string> msg, AriadneEventParts eventParts);
	}
}