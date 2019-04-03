using UnityEngine.EventSystems;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// The interface of drawing maps from MoveController.
	/// </Summary>
	public interface IDirtyMarkerMap : IEventSystemHandler {

		/// <Summary>
		/// Send OnSetDirty message to DrawMap scripts.
		/// </Summary>
		void OnSetDirty();

		/// <Summary>
		/// Send OnSetDirtyLerp message to DrawMap scripts.
		/// </Summary>
		/// <param name="time">The moveWait in MoveController.</param>
		void OnSetDirtyLerp(float time);

		/// <Summary>
		/// Send OnSetNewMap message to DrawMap scripts.
		/// </Summary>
		void OnSetNewMap();
	}
}