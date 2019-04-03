using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// Event processing class.
	/// Events are defined in MapEditor and EventEditor.
	/// </Summary>
	public class EventProcessor : MonoBehaviour, IShowingMsg {
		bool isWaitingMsg;
		GameObject gameController;
		FadeManager fadeManager;

		void Start(){
			SetObjRef();
		}

		/// <Summary>
		/// Set object references to cache them.
		/// </Summary>
		void SetObjRef(){
			fadeManager = gameObject.GetComponent<FadeManager>();
			gameController = GameObject.FindGameObjectWithTag("GameController");
		}

		void Update(){
			if (isWaitingMsg){
				MsgKeyWait();
			}
		}

		/// <Summary>
		/// Get the key input of waiting in message event.
		/// </Summary>
		void MsgKeyWait(){
			if (Input.GetKeyUp(KeyCode.Space)){
				isWaitingMsg = false;
			}
		}

		/// <Summary>
		/// Action of the enter button for waiting message.
		/// This method is used in mobile devices.
		/// </Summary>
		public void OnPressedOkButton(){
			isWaitingMsg = false;
		}

		/// <Summary>
		/// Returns the event parts data that matched the condition.
		/// </Summary>
		/// <param name="eventData">The event data of target position.</param>
		public AriadneEventParts GetExecutionEventParts(EventMasterData eventData){
			if (eventData.eventParts == null) {
				return null;
			}

			AriadneEventParts parts = null;
			// Check the condition of event parts from last index.
			for (int i = eventData.eventParts.Count - 1; i >= 0; i--){
				parts = eventData.eventParts[i];
				if (CheckEventPartsCondition(parts)){
					break;
				}
			}
			return parts;
		}

		/// <Summary>
		/// Returns that event parts condition is matched.
		/// </Summary>
		/// <param name="parts">The event parts to check the condition.</param>
		bool CheckEventPartsCondition(AriadneEventParts parts){
			bool isMatched = false;
			switch (parts.startCondition){
				case AriadneEventCondition.Flag:
					isMatched = FlagManager.CheckEventFlag(parts.startFlagName);
					break;
				case AriadneEventCondition.Item:
					isMatched = ItemManager.CheckEventConditionItem(parts.startItemId, parts.comparisonOperator, parts.startNum);
					break;
				case AriadneEventCondition.Money:
					isMatched = ItemManager.CheckEventConditionMoney(parts.comparisonOperator, parts.startNum);
					break;
				case AriadneEventCondition.NoCondition:
					isMatched = true;
					break;
			}
			return isMatched;
		}

		/// <Summary>
		/// Execute events according to event type.
		/// </Summary>
		/// <param name="eventParts">The event parts to execute.</param>
		/// <param name="eventPos">The position of event.</param>
		public void EventExecuter(AriadneEventParts eventParts, Vector2Int eventPos){
			AriadneEventStrategyFactory factory = gameController.GetComponent<AriadneEventStrategyFactory>();
			IAriadneEventStrategy iEventExecuter = factory.GetEventExecuter(eventParts.eventCategory);
			iEventExecuter.ExecuteEvent(gameController, eventPos, eventParts);
		}

		/// <Summary>
		/// Start point of showing message UI.
		/// </Summary>
		/// <param name="msg">Message list to show.</param>
		/// <param name="eventParts">The event parts to execute.</param>
		public void OnShowingMsg(List<string> msg, AriadneEventParts eventParts){
			if (msg == null){
				PostEvent(eventParts);
				return;
			}
			StartCoroutine(ShowMessages(msg, eventParts));
		}

		/// <Summary>
		/// Showing message process.
		/// </Summary>
		/// <param name="msg">Message list to show.</param>
		/// <param name="eventParts">The event parts to execute.</param>
		IEnumerator ShowMessages(List<string> msg, AriadneEventParts eventParts){
			// Fade in message window.
			fadeManager.ClearText();
			fadeManager.FadeInMsgWindow();

			var wait = new WaitForSeconds(0.2f);

			for (int page = 0; page < msg.Count; page++){
				fadeManager.SetText(msg[page]);
				yield return new WaitForSeconds(fadeManager.keyWaitFadeTime);
				isWaitingMsg = true;

				while (isWaitingMsg){
					yield return wait;
				}
			}

			fadeManager.FadeOutMsgWindow();
			yield return new WaitForSeconds(fadeManager.keyWaitFadeTime);

			PostEvent(eventParts);
		}

		/// <Summary>
		/// The post process of event.
		/// Check the event executed flag in this method.
		/// </Summary>
		/// <param name="parts">The executed event parts data.</param>
		void PostEvent(AriadneEventParts parts){
			SetEventExecutedFlag(parts);
			SendEventFinishedMessage(gameController);
		}

		/// <Summary>
		/// Set the event executed flag if the event parts had.
		/// </Summary>
		/// <param name="parts">The executed event parts data.</param>
		void SetEventExecutedFlag(AriadneEventParts parts){
			if (parts.hasExecutedFlag && parts.executedFlagName != ""){
				FlagManager.SetEventFlagDict(parts.executedFlagName, true);
			}
		}

		/// <Summary>
		/// Send a message which finished the event to the game controller.
		/// </Summary>
		/// <param name="obj">Specify the game controller object.</param>
		void SendEventFinishedMessage(GameObject obj){
			ExecuteEvents.Execute<IEventProcessor>(
				target: obj,
				eventData: null,
				functor: EventFinished
			);
		}

		/// <Summary>
		/// The functor of SendEventFinishedMessage method.
		/// </Summary>
		void EventFinished(IEventProcessor eventProcessor, BaseEventData eventData){
			eventProcessor.OnPostEvent();
		}
	}
}
