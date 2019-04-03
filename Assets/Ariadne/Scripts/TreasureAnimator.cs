using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Animate a treasure box object.
	/// Attach this script to treasure box prefabs for animation.
	/// </Summary>
	public class TreasureAnimator : MonoBehaviour {

		Animator animator;

		void Start () {
			CheckAnimator();
		}

		/// <Summary>
		/// Play treasure box opening animation.
		/// </Summary>
		public void OpenTreasureBox(){
			CheckAnimator();
			animator.SetBool("open", true);
		}

		/// <Summary>
		/// Play treasure box opening animation immediately.
		/// </Summary>
		public void OpenTreasureBoxImmediately(){
			CheckAnimator();
			animator.SetBool("open", false);
			animator.SetBool("hasOpened", true);
		}

		/// <Summary>
		/// Check whether the animator reference is null or not.
		/// </Summary>
		void CheckAnimator(){
			if (animator == null){
				animator = gameObject.GetComponent<Animator>();
			}
		}
	}
}

