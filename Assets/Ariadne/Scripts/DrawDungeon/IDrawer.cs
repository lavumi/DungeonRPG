using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// The interface of drawing dungeon parts.
	/// This interface is used between DrawManager and each parent of dungeon objects.
	/// </Summary>
	public interface IDrawer : IEventSystemHandler {

		/// <Summary>
		/// Send OnDraw message to each parent of dungeon objects.
		/// This message is sent when the player enters the dungeon.
		/// </Summary>
		void OnDraw();

		/// <Summary>
		/// Send OnRedraw message to each parent of dungeon objects.
		/// This message is sent when move position event is executed.
		/// </Summary>
		void OnRedraw();

		/// <Summary>
		/// Send OnRemoveObjects message to each parent of dungeon objects.
		/// This message is sent when the player is exiting the dungeon.
		/// </Summary>
		void OnRemoveObjects();
	}
}