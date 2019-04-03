using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// The interface for sending a message on exiting the dungeon.
	/// </Summary>
	public interface IExitDungeon : IEventSystemHandler {

		/// <Summary>
		/// Send a message which notifies exiting the dungeon.
		/// </Summary>
		void OnExitDungeon();
	}
}