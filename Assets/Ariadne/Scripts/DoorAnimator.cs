using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Animate a door object.
	/// Attach this script to door prefabs for animation.
	/// </Summary>
	public class DoorAnimator : MonoBehaviour {

		Animator animator;

		void Start(){
			CheckAnimator();
		}

		/// <Summary>
		/// Play door opening animation.
		/// </Summary>
		public void OpenDoor(){
			CheckAnimator();
			animator.SetBool("close", false);
			animator.SetBool("open", true);
		}

		/// <Summary>
		/// Play door closing animation.
		/// </Summary>
		public void CloseDoor(){
			CheckAnimator();
			animator.SetBool("open", false);
			animator.SetBool("close", true);
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

