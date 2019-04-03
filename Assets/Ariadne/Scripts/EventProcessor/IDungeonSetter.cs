using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// The interface for setting a new dungeon.
	/// </Summary>
	public interface IDungeonSetter : IEventSystemHandler {

		/// <Summary>
		/// Set a new dungeon data to DungeonSetting component.
		/// </Summary>
		/// <param name="dungeonData">New dungeon data.</param>
		void OnSetDungeon(DungeonMasterData dungeonData);
	}
}