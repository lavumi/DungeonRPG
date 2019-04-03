using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Factory class of event strategy classes.
	/// </Summary>
	public class AriadneEventStrategyFactory : MonoBehaviour {

		/// <Summary>
		/// Returns IAriadneEventStrategy component that corresponds to the event category.
		/// </Summary>
		/// <param name="eventCategory">The event category to execute.</param>
		public IAriadneEventStrategy GetEventExecuter(AriadneEventCategory eventCategory){
			Component component = (Component) gameObject.GetComponent<IAriadneEventStrategy>();
			if (component != null){
				Destroy(component);
			}

			IAriadneEventStrategy iStrategy = null;

			switch (eventCategory){
				case AriadneEventCategory.None:
					iStrategy = gameObject.AddComponent<EventNone>();
					break;
				case AriadneEventCategory.Door:
					iStrategy = gameObject.AddComponent<EventDoor>();
					break;
				case AriadneEventCategory.LockedDoor:
					iStrategy = gameObject.AddComponent<EventLockedDoor>();
					break;
				case AriadneEventCategory.MovePosition:
					iStrategy = gameObject.AddComponent<EventMovePosition>();
					break;
				case AriadneEventCategory.Treasure:
					iStrategy = gameObject.AddComponent<EventTreasure>();
					break;
				case AriadneEventCategory.Messenger:
					iStrategy = gameObject.AddComponent<EventMessenger>();
					break;
				case AriadneEventCategory.ExitPosition:
					iStrategy = gameObject.AddComponent<EventExitDungeon>();
					break;
			}

			return iStrategy;
		}
	}
}

