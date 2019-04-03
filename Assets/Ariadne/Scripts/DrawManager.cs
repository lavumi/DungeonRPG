using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ariadne {

	/// <Summary>
	/// Manager class of drawing dungeon.
	/// Passes drawing message to dungeon object's parents.
	/// </Summary>
	public class DrawManager : MonoBehaviour, IDungeonObjects {

		GameObject groundObj;
		GameObject wallObj;
		GameObject ceilingObj;
		GameObject mapObj;
		List<GameObject> objList;

		string groundTag = "GroundParent";
		string wallTag = "WallParent";
		string ceilingTag = "CeilingParent";
		string mapTag = "Map";

		[SerializeField]
		bool isDrawMap = false;

		void Start () {
			SetObjRef();

			SetParentList();

			OnDrawObj();
		}

		/// <Summary>
		/// Set object references to cache them.
		/// </Summary>
		void SetObjRef(){
			groundObj = GameObject.FindGameObjectWithTag(groundTag);
			wallObj = GameObject.FindGameObjectWithTag(wallTag);
			ceilingObj = GameObject.FindGameObjectWithTag(ceilingTag);
			mapObj = GameObject.FindGameObjectWithTag(mapTag);
		}

		/// <Summary>
		/// Set object list of dungeon object's parents.
		/// </Summary>
		void SetParentList(){
			objList = new List<GameObject>();
			objList.Add(groundObj);
			objList.Add(wallObj);
			objList.Add(ceilingObj);
			objList.Add(mapObj);
		}

		/// <Summary>
		/// Send draw message to each parent object.
		/// </Summary>
		public void OnDrawObj(){
			foreach (GameObject obj in objList){
				SendDrawMsg(obj);
			}
		}

		/// <Summary>
		/// Send redraw message to each parent object.
		/// </Summary>
		public void OnRedrawObj(){
			foreach (GameObject obj in objList){
				SendRedrawMsg(obj);
			}
		}

		/// <Summary>
		/// Send remove message to each parent object.
		/// </Summary>
		public void OnRemoveObj(){
			foreach (GameObject obj in objList){
				SendRemoveMsg(obj);
			}
		}
		
		void Update () {
            return;
			if (isDrawMap){
				mapObj.SetActive(true);
			} else {
				mapObj.SetActive(false);
			}
		}

		/// <Summary>
		/// Send messages for drawing dungeon objects.
		/// </Summary>
		/// <param name="obj">The parent object of dungeon parts.</param>
		void SendDrawMsg(GameObject obj){
			ExecuteEvents.Execute<IDrawer>(
				target: obj,
				eventData: null,
				functor: CallDraw
			);
		}

		/// <Summary>
		/// The functor of SendDrawMsg method.
		/// </Summary>
		void CallDraw(IDrawer drawer, BaseEventData eventData){
			drawer.OnDraw();
		}

		/// <Summary>
		/// Send messages for re-drawing dungeon objects.
		/// </Summary>
		/// <param name="obj">The parent object of dungeon parts.</param>
		void SendRedrawMsg(GameObject obj){
			ExecuteEvents.Execute<IDrawer>(
				target: obj,
				eventData: null,
				functor: CallRedraw
			);
		}

		/// <Summary>
		/// The functor of SendRedrawMsg method.
		/// </Summary>
		void CallRedraw(IDrawer drawer, BaseEventData eventData){
			drawer.OnRedraw();
		}

		/// <Summary>
		/// Send messages for removing dungeon objects.
		/// </Summary>
		/// <param name="obj">The parent object of dungeon parts.</param>
		void SendRemoveMsg(GameObject obj){
			ExecuteEvents.Execute<IDrawer>(
				target: obj,
				eventData: null,
				functor: CallRemoveObjects
			);
		}

		/// <Summary>
		/// The functor of SendRemoveMsg method.
		/// </Summary>
		void CallRemoveObjects(IDrawer drawer, BaseEventData eventData){
			drawer.OnRemoveObjects();
		}
	}
}