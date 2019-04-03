using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// The interface for sending a message on entering the dungeon.
	/// </Summary>
	public interface IEnterDungeon : IEventSystemHandler {

		/// <Summary>
		/// Send a message which notifies entering the dungeon.
		/// </Summary>
		void OnEnterDungeon();
	}
}