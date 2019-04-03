using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ariadne {

	/// <Summary>
	/// Showing player position on the map with an icon.
	/// The position of this pointer is based on MapBase object.
	/// </Summary>
	public class PlayerPointerOnMap : MonoBehaviour {

		int showLengthHorizontal = 4;
		int showLengthVertical = 4;
		GameObject mapObj;
		RectTransform mapRt;
		RectTransform pointerRt;

		GameObject gameController;
		DungeonSettings dungeonSettings;
		MapShowingSettings mapSettings;

		void Awake() {
			SetObjRef();
		}
		
		void Update() {
			GetMapSettings();
			SetPointerPos();
			SetPointerSize();
			SetPointerRotation();
		}

		/// <Summary>
		/// Set object references to cache them.
		/// </Summary>
		void SetObjRef(){
			gameController = GameObject.FindGameObjectWithTag("GameController");
			mapSettings = gameController.GetComponent<MapShowingSettings>();

			mapObj = GameObject.Find("MapBase");
			mapRt = mapObj.GetComponent<RectTransform>();
			pointerRt = gameObject.GetComponent<RectTransform>();
		}

		/// <Summary>
		/// Get settings of the map.
		/// </Summary>
		void GetMapSettings(){
			this.showLengthHorizontal = mapSettings.showLengthHorizontal;
			this.showLengthVertical = mapSettings.showLengthVertical;
		}

		/// <Summary>
		/// Set the position of the player pointer icon.
		/// </Summary>
		void SetPointerPos(){
			Vector3 centerPos = mapRt.transform.localPosition;
			pointerRt.transform.localPosition = centerPos;
		}

		/// <Summary>
		/// Set the size of the player pointer icon.
		/// </Summary>
		void SetPointerSize(){
			float cellWidth = mapRt.sizeDelta.x / (1 + showLengthHorizontal * 2);
			float cellHeight = mapRt.sizeDelta.y / (1 + showLengthVertical * 2);
			Vector2 sizeSet = new Vector2(cellWidth, cellHeight);
			pointerRt.sizeDelta = sizeSet;
		}

		/// <Summary>
		/// Set the rotation of the player pointer icon.
		/// </Summary>
		void SetPointerRotation(){
			float minAngle = 0f;
			float maxAngle = minAngle + 90f;
			switch(PlayerPosition.direction){
				case DungeonDir.North:
					maxAngle = 0f;
					break;
				case DungeonDir.East:
					maxAngle = -90f;
					break;
				case DungeonDir.South:
					maxAngle = 180f;
					break;
				case DungeonDir.West:
					maxAngle = 90f;
					break;
			}
			float angle = Mathf.LerpAngle(minAngle, maxAngle, Time.time);
			pointerRt.localEulerAngles = new Vector3(0, 0, angle);
		}
	}
}