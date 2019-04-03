using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// Base class of event strategy classes.
	/// </Summary>
	public class AriadneEventStrategyBase : MonoBehaviour {

		protected GameObject gameController;
		protected List<string> sendShowingMsgList;
		protected AriadneEventParts sendParts;

		/// <Summary>
		/// Returns a GameObject which is searched by position and map attribute ID.
		/// </Summary>
		/// <param name="eventPos">The position of event on the dungeon.</param>
		/// <param name="mapAttrId">The map attribute id.</param>
		protected GameObject GetEventObj(Vector2Int eventPos, int mapAttrId){
			string objNamePostfix = "_" + eventPos.x.ToString() + "-" + eventPos.y.ToString();
			string attrName = MapAttributeDefine.GetAttrNameById(mapAttrId);
			string objName = attrName + objNamePostfix;
			GameObject obj = GameObject.Find(objName);
			return obj;
		}

		/// <Summary>
		/// Show warning message about animation.
		/// </Summary>
		/// <param name="obj">The object for animating.</param>
		protected void AnimatorWarning(GameObject obj){
			Debug.LogWarning("Animator script is not attached to object : " + obj.name);
		}

		/// <Summary>
		/// Show error message about missing the event object.
		/// </Summary>
		/// <param name="eventPos">The position of event on the dungeon.</param>
		protected void MissingEventObjectError(Vector2Int eventPos){
			Debug.LogError("Event object is missing. The event will be finished without executing. Position : " + eventPos);
		}

		/// <Summary>
		/// The post process of event.
		/// Check the event executed flag in this method.
		/// </Summary>
		/// <param name="parts">The executed event parts data.</param>
		protected void PostEvent(AriadneEventParts parts){
			SetEventExecutedFlag(parts);
			SendEventFinishedMessage(gameController);
		}

		/// <Summary>
		/// Set the event executed flag if the event parts had.
		/// </Summary>
		/// <param name="parts">The executed event parts data.</param>
		protected void SetEventExecutedFlag(AriadneEventParts parts){
			if (parts.hasExecutedFlag && parts.executedFlagName != ""){
				FlagManager.SetEventFlagDict(parts.executedFlagName, true);
			}
		}

		/// <Summary>
		/// Send a message which finished the event to the game controller.
		/// </Summary>
		/// <param name="obj">Specify the game controller object.</param>
		protected void SendEventFinishedMessage(GameObject obj){
			ExecuteEvents.Execute<IEventProcessor>(
				target: obj,
				eventData: null,
				functor: EventFinished
			);
		}

		/// <Summary>
		/// The functor of SendEventFinishedMessage method.
		/// </Summary>
		protected void EventFinished(IEventProcessor eventProcessor, BaseEventData eventData){
			eventProcessor.OnPostEvent();
		}

		/// <Summary>
		/// Send a message which is showing message UI to the game controller.
		/// </Summary>
		/// <param name="obj">Specify the game controller object.</param>
		protected void SendShowingMessage(GameObject obj){
			ExecuteEvents.Execute<IShowingMsg>(
				target: obj,
				eventData: null,
				functor: ShowMsg
			);
		}

		/// <Summary>
		/// The functor of SendShowingMessage method.
		/// </Summary>
		protected void ShowMsg(IShowingMsg showMsg, BaseEventData eventData){
			showMsg.OnShowingMsg(sendShowingMsgList, sendParts);
		}
	}
}
