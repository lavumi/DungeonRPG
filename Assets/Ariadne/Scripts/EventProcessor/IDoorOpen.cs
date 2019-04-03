using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// The interface of moving on the opening door event.
	/// </Summary>
	public interface IDoorOpen : IEventSystemHandler {

		/// <Summary>
		/// Send a message which notifies finishing of moving.
		/// </Summary>
		void OnMoveFinished();
	}
}