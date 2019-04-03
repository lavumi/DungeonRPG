using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// The interface of drawing the dungeon.
	/// This interface is used between MoveController and DrawManager.
	/// </Summary>
	public interface IDungeonObjects : IEventSystemHandler {

		/// <Summary>
		/// Send OnDrawObj message to DrawDungeon scripts.
		/// This message will be sent when the player enters the dungeon.
		/// </Summary>
		void OnDrawObj();

		/// <Summary>
		/// Send OnRedrawObj message to DrawDungeon scripts.
		/// This message will be sent when move position event is executed.
		/// </Summary>
		void OnRedrawObj();

		/// <Summary>
		/// Send OnRemoveObjects message to DrawDungeon scripts.
		/// This message will be sent when removing dungeon objects.
		/// </Summary>
		void OnRemoveObj();
	}
}