using UnityEngine;
using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// The interface of AriadneEventStrategy.
	/// </Summary>
	public interface IAriadneEventStrategy : IEventSystemHandler {

		/// <Summary>
		/// Execute method of event process.
		/// </Summary>
		/// <param name="controller">Game controller object.</param>
		/// <param name="eventPos">The position of the event.</param>
		/// <param name="eventParts">The event parts data.</param>
		void ExecuteEvent(GameObject contorller, Vector2Int eventPos, AriadneEventParts parts);
	}
}
