using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ariadne {

	/// <Summary>
	/// Manager class of fading in and out.
	/// </Summary>
	public class FadeManager : MonoBehaviour {

		[SerializeField]
		GameObject fadeMaskPanel;
		Image panel;

		[SerializeField]
		GameObject mapMaskPanel;
		Image mapPanel;
		

		public float fadeTime = 1.0f;

		[SerializeField]
		GameObject keyWaitWindow;
		public float keyWaitFadeTime = 0.1f;
		Image keyWaitBg;
		Text keyWaitText;

		[SerializeField]
		GameObject msgWindow;
		Image msgWindowBg;
		Text msgWindowText;

		bool isFadingInKeyWait = false;
		bool isFadingOutKeyWait = false;
		bool isFadingInMsgWindow = false;
		bool isFadingOutMsgWindow = false;

		void Start () {
			CheckUIRef();
		}

		/// <Summary>
		/// Check UI references before manipulating UIs.
		/// </Summary>
		void CheckUIRef(){
			// Fade mask panel
			if (panel == null){
				panel = fadeMaskPanel.GetComponent<Image>();
			}

			// Map mask panel
			if (mapPanel == null){
				mapPanel = mapMaskPanel.GetComponent<Image>();
			}

			// Key wait text
			if (keyWaitBg == null){
				keyWaitBg = keyWaitWindow.GetComponent<Image>();
			}
			if (keyWaitText == null){
				keyWaitText = keyWaitWindow.GetComponentInChildren<Text>();
			}

			// Message window
			if (msgWindowBg == null){
				msgWindowBg = msgWindow.GetComponent<Image>();
			}
			if (msgWindowText == null){
				msgWindowText = msgWindow.GetComponentInChildren<Text>();
			}
		}

		/// <Summary>
		/// Fade in the screen.
		/// </Summary>
		public void FadeIn(){
			CheckUIRef();
			StartCoroutine(FadeInMask(panel));
		}

		/// <Summary>
		/// Fade out the screen.
		/// </Summary>
		public void FadeOut(){
			CheckUIRef();
			StartCoroutine(FadeOutMask(panel));
		}

		/// <Summary>
		/// Fade in the map.
		/// </Summary>
		public void MapFadeIn(){
			CheckUIRef();
			StartCoroutine(FadeInMask(mapPanel));
		}

		/// <Summary>
		/// Fade out the map.
		/// </Summary>
		public void MapFadeOut(){
			CheckUIRef();
			StartCoroutine(FadeOutMask(mapPanel));
		}
		
		/// <Summary>
		/// Fade in process of the screen.
		/// </Summary>
		/// <param name="mask">Specify image of the fade mask object.</param>
		IEnumerator FadeInMask(Image mask){
			Color maskColor = mask.color;
			Color maskFadeInColor = new Color(maskColor.r, maskColor.g, maskColor.b, 0.0f);
			float fadeFinishTime = GetFadeFinishTime(fadeTime);

			while (true){
				float diff = fadeFinishTime - Time.time;
				if (diff <= 0){
					break;
				}
				float rate = 1 - Mathf.Clamp01(diff / fadeTime);
				mask.color = Color.Lerp(maskColor, maskFadeInColor, rate);
				yield return null;
			}
			mask.color = maskFadeInColor;
		}

		/// <Summary>
		/// Fade out process of the screen.
		/// </Summary>
		/// <param name="mask">Specify image of the fade mask object.</param>
		IEnumerator FadeOutMask(Image mask){
			Color maskColor = mask.color;
			Color maskFadeOutColor = new Color(maskColor.r, maskColor.g, maskColor.b, 1.0f);
			float fadeFinishTime = GetFadeFinishTime(fadeTime);

			while (true){
				float diff = fadeFinishTime - Time.time;
				if (diff <= 0){
					break;
				}
				float rate = 1 - Mathf.Clamp01(diff / fadeTime);
				mask.color = Color.Lerp(maskColor, maskFadeOutColor, rate);
				yield return null;
			}
			mask.color = maskFadeOutColor;
		}

		/// <Summary>
		/// Fade in the key wait UI.
		/// </Summary>
		public void FadeInKeyWait(){
			CheckUIRef();
			if (!isFadingInKeyWait){
				StartCoroutine(FadeInUI(keyWaitBg, keyWaitText));
			}
		}

		/// <Summary>
		/// Fade in the message window UI.
		/// </Summary>
		public void FadeInMsgWindow(){
			CheckUIRef();
			if (!isFadingInMsgWindow){
				StartCoroutine(FadeInUI(msgWindowBg, msgWindowText));
			}
		}

		/// <Summary>
		/// Fade in process of UIs.
		/// </Summary>
		/// <param name="bg">Background image.</param>
		/// <param name="text">Showing message.</param>
		IEnumerator FadeInUI(Image bg, Text text){
			isFadingInKeyWait = true;
			Color bgColor = bg.color;
			Color textColor = text.color;
			Color bgFadeInColor = new Color(bgColor.r, bgColor.g, bgColor.b, 1.0f);
			Color textFadeInColor = new Color(textColor.r, textColor.g, textColor.b, 1.0f);

			float fadeFinishTime = GetFadeFinishTime(keyWaitFadeTime);

			while (true){
				float diff = fadeFinishTime - Time.time;
				if (diff <= 0){
					break;
				}
				float bgRate = 1 - Mathf.Clamp01(diff / keyWaitFadeTime);
				float textRate = 1 - Mathf.Clamp01(diff / keyWaitFadeTime);
				bg.color = Color.Lerp(bgColor, bgFadeInColor, bgRate);
				text.color = Color.Lerp(textColor, textFadeInColor, textRate);
				yield return null;
			}
			bg.color = bgFadeInColor;
			text.color = textFadeInColor;
		}

		/// <Summary>
		/// Fade out process of UIs.
		/// </Summary>
		/// <param name="bg">Background image.</param>
		/// <param name="text">Showing message.</param>
		IEnumerator FadeOutUI(Image bg, Text text){
			isFadingOutKeyWait = true;
			Color bgColor = bg.color;
			Color textColor = text.color;
			Color bgFadeOutColor = new Color(bgColor.r, bgColor.g, bgColor.b, 0f);
			Color textFadeOutColor = new Color(textColor.r, textColor.g, textColor.b, 0f);

			float fadeFinishTime = GetFadeFinishTime(keyWaitFadeTime);

			while (true){
				float diff = fadeFinishTime - Time.time;
				if (diff <= 0){
					break;
				}
				float bgRate = 1 - Mathf.Clamp01(diff / keyWaitFadeTime);
				float textRate = 1 - Mathf.Clamp01(diff / keyWaitFadeTime);
				bg.color = Color.Lerp(bgColor, bgFadeOutColor, bgRate);
				text.color = Color.Lerp(textColor, textFadeOutColor, textRate);
				yield return null;
			}
			bg.color = bgFadeOutColor;
			text.color = textFadeOutColor;
		}

		/// <Summary>
		/// Fade out the key wait UI.
		/// </Summary>
		public void FadeOutKeyWait(){
			CheckUIRef();
			if (!isFadingOutKeyWait){
				StartCoroutine(FadeOutUI(keyWaitBg, keyWaitText));
			}
		}

		/// <Summary>
		/// Fade out the message window UI.
		/// </Summary>
		public void FadeOutMsgWindow(){
			CheckUIRef();
			if (!isFadingOutMsgWindow){
				StartCoroutine(FadeOutUI(msgWindowBg, msgWindowText));
			}
		}

		/// <Summary>
		/// Initialize wait flags of fade in and out.
		/// </Summary>
		public void InitializeWaitFlags(){
			isFadingInKeyWait = false;
			isFadingOutKeyWait = false;
		}

		/// <Summary>
		/// Clear text messages in the message window.
		/// </Summary>
		public void ClearText(){
			CheckUIRef();
			msgWindowText.text = "";
		}

		/// <Summary>
		/// Set text messages in the message window.
		/// </Summary>
		public void SetText(string msg){
			CheckUIRef();
			msgWindowText.text = msg;
		}

		/// <Summary>
		/// Returns the time fade will be finished.
		/// </Summary>
		float GetFadeFinishTime(float time){
			return Time.time + time;
		}
	}
}
