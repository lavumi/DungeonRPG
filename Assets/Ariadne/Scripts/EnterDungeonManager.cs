using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// Processes about entering the dungeon.
	/// </Summary>
	public class EnterDungeonManager : MonoBehaviour, IExitDungeon {

		[SerializeField]
		GameObject enterButtonObj;
		Button enterButton;
		FadeManager fadeManager;
		GameObject gameController;

		void Start() {
			SetObjRef();
		}

		/// <Summary>
		/// Set object references to cache them.
		/// </Summary>
		void SetObjRef(){
			gameController = GameObject.FindGameObjectWithTag("GameController");
			fadeManager = gameObject.GetComponent<FadeManager>();
			enterButton = enterButtonObj.GetComponent<Button>();
		}

		/// <Summary>
		/// Fade in screen and dungeon entering button.
		/// </Summary>
		IEnumerator FadeIn(){
			enterButtonObj.SetActive(true);
			fadeManager.FadeIn();
			yield return new WaitForSeconds(fadeManager.fadeTime);
			enterButton.interactable = true;
		}

		/// <Summary>
		/// Fade out screen and dungeon entering button.
		/// </Summary>
		IEnumerator FadeOut(){
			fadeManager.FadeOut();
			yield return new WaitForSeconds(fadeManager.fadeTime);
			enterButtonObj.SetActive(false);
			yield return new WaitForSeconds(fadeManager.fadeTime);
			SendEnterDungeonMessage(gameController);
		}

		/// <Summary>
		/// Receiver of exit dungeon message.
		/// </Summary>
		public void OnExitDungeon(){
			StartCoroutine(FadeIn());
		}
		
		/// <Summary>
		/// Entering the dungeon button listener.
		/// </Summary>
		public void OnPressedEnterButton(){
			enterButton.interactable = false;
			StartCoroutine(FadeOut());
		}

		/// <Summary>
		/// Send a message which notifies entering the dungeon.
		/// </Summary>
		/// <param name="obj">The GameObject that holds the MoveConteroller component.</param>
		void SendEnterDungeonMessage(GameObject obj){
			ExecuteEvents.Execute<IEnterDungeon>(
				target: obj,
				eventData: null,
				functor: EnterDungeonMsg
			);
		}

		/// <Summary>
		/// The functor of SendEnterDungeonMessage method.
		/// </Summary>
		void EnterDungeonMsg(IEnterDungeon enterDungeon, BaseEventData eventData){
			enterDungeon.OnEnterDungeon();
		}
	}
}
