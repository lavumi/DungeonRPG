namespace Ariadne {

	/// <Summary>
	/// Definition of direction enums.
	/// These enums are used to express the direction of player and dungeon objects.
	/// </Summary>
	public enum DungeonDir {
		North,
		East,
		South,
		West
	}

	/// <Summary>
	/// Definition of event category enums.
	/// These enums are used for processing events.
	/// </Summary>
	public enum AriadneEventCategory {
		None,
		Door,
		LockedDoor,
		MovePosition,
		Treasure,
		Messenger,
		ExitPosition
	}

	/// <Summary>
	/// Definition of event trigger enums.
	/// These enums are used for event trigger.
	/// </Summary>
	public enum AriadneEventTrigger {
		KeyPress,
		Auto
	}

	/// <Summary>
	/// Definition of event position enums.
	/// These enums are used for position of starting events.
	/// </Summary>
	public enum AriadneEventPosition {
		OneStepShortOfThis,
		ThisPosition
	}

	/// <Summary>
	/// Definition of treasure type enums.
	/// </Summary>
	public enum TreasureType {
		Item,
		Money
	}

	/// <Summary>
	/// Definition of conditions of starting events enums.
	/// </Summary>
	public enum AriadneEventCondition{
		NoCondition,
		Flag,
		Item,
		Money
	}

	/// <Summary>
	/// Definition of comparisons that are used for comparing event conditions.
	/// </Summary>
	public enum AriadneComparison {
		Equals,
		NotEqual,
		GreaterThan,
		LessThan,
		GreaterOrEqual,
		LessOrEqual
	}

	/// <Summary>
	/// Definition of item type enums.
	/// </Summary>
	public enum AriadneItemType{
		Normal,
		Key
	}

	/// <Summary>
	/// Definition of key type of the door enums.
	/// </Summary>
	public enum DoorKeyType{
		None,
		Bronze,
		Silver,
		Gold
	}
}
