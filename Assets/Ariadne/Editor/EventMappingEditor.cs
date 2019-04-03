using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Ariadne {

	/// <Summary>
	/// Show event mappings to check event data references.
	/// </Summary>
	public class EventMappingEditor : EditorWindow {

		List<EventMappingData> eventMappingList;
		List<EventMasterData> eventDataList;
		List<FloorMapMasterData> floorMapDataList;
		const int LABEL_WIDTH = 100;
		const int NAME_LABEL_WIDTH = 150;
		const int ID_LABEL_WIDTH = 50;
		Vector2 scrollPos = Vector2.zero;
		string saveFileName;
		Texture2D highlightTex;
		Color highlightColor = new Color(1.0f, 1.0f, 0.5f, 0.3f);
		Color[] highlightColors;


		/// <Summary>
		/// Ready the event mapping list.
		/// </Summary>
		/// <param name="floorName">Name of the floor.</param>
		public void GetEventRef(string floorName){
			// Set highlight color to the texture.
			highlightColors = new Color[1]{highlightColor};
			highlightTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			highlightTex.SetPixels(0, 0, 1, 1, highlightColors);
			highlightTex.Apply();

			// Initialize event list.
			eventMappingList = new List<EventMappingData>();
			SetEventMasterDataList();
			SetFloorMapDataList();
			this.saveFileName = floorName;

			// Check FloorMapData in project files.
			foreach (FloorMapMasterData floorMapData in floorMapDataList){
				string mapFileName = floorMapData.name;
				string mapName = floorMapData.floorName;
				for (int y = 0; y < floorMapData.floorSizeVertical; y++){
					for (int x = 0; x < floorMapData.floorSizeHorizontal; x++){
						int index = x + y * floorMapData.floorSizeHorizontal;
						int eventId = floorMapData.mapInfo[index].eventId;
						if (eventId <= 0){
							continue;
						}
						string fileName = MapEditorUtil.eventFilePrefix + eventId.ToString("D5");
						var eventData = eventDataList.Find((e) => e.name == fileName);
						if (eventData == null){
							continue;
						}
						Vector2Int pos = new Vector2Int(x, y);
						if (eventData.eventParts == null){
							continue;
						}
						for (int i = 0; i < eventData.eventParts.Count; i++){
							EventMappingData mapping = new EventMappingData(eventData.eventId, eventData.eventName, i, eventData.eventParts[i].startCondition, eventData.eventParts[i].eventCategory, mapFileName, mapName, pos);
							eventMappingList.Add(mapping);
						}
					}
				}
			}

			foreach (EventMasterData eventData in eventDataList){
				EventMappingData mapping = eventMappingList.Find((e) => e.eventId == eventData.eventId);
				if (mapping != null){
					continue;
				}
				if (eventData.eventParts == null){
					continue;
				}
				for (int i = 0; i < eventData.eventParts.Count; i++){
					EventMappingData mappingData = new EventMappingData(eventData.eventId, eventData.eventName, i, eventData.eventParts[i].startCondition, eventData.eventParts[i].eventCategory, MapEditorUtil.notAssignedText, MapEditorUtil.notAssignedText, Vector2Int.zero);
					eventMappingList.Add(mappingData);
				}
			}
			
		}

		/// <Summary>
		/// Search event data in the project and set it to the list.
		/// </Summary>
		void SetEventMasterDataList(){
			eventDataList = new List<EventMasterData>();

			string dataType = "EventMasterData";
			string[] guidArray = MapEditorUtil.GetGuidArray(dataType);

			foreach (string guid in guidArray){
				string path = AssetDatabase.GUIDToAssetPath(guid);
				EventMasterData eventData = AssetDatabase.LoadAssetAtPath<EventMasterData>(path);
				if (eventData == null){
					continue;
				}
				if (eventData.eventId > 0){
					eventDataList.Add(eventData);
				}
			}
		}

		/// <Summary>
		/// Search floor map data in the project and set it to the list.
		/// </Summary>
		void SetFloorMapDataList(){
			floorMapDataList = new List<FloorMapMasterData>();

			string dataType = "FloorMapMasterData";
			string[] guidArray = MapEditorUtil.GetGuidArray(dataType);

			foreach (string guid in guidArray){
				string path = AssetDatabase.GUIDToAssetPath(guid);
				FloorMapMasterData floorData = AssetDatabase.LoadAssetAtPath<FloorMapMasterData>(path);
				if (floorData == null){
					continue;
				}
				if (floorData.name != MapEditorUtil.tempFileName){
					floorMapDataList.Add(floorData);
				}
			}
		}

		/// <Summary>
		/// Show event mapping information.
		/// </Summary>
		void OnGUI(){
			EditorGUILayout.LabelField("Ariadne Event Mapping Viewer");
			EditorGUILayout.Space();

			// Show labels.
			ShowLabelParts();

			// Show event data mappings.
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUI.skin.box);
			ShowEventMapping();
			EditorGUILayout.EndScrollView();
		}

		/// <Summary>
		/// When lost focus, close the window and destroy this window object.
		/// </Summary>
		void OnLostFocus(){
			Close();
		}

		/// <Summary>
		/// Label parts of event mapping.
		/// </Summary>
		void ShowLabelParts(){
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField("Event ID", GUILayout.Width(ID_LABEL_WIDTH));
					EditorGUILayout.LabelField("Event Name", GUILayout.Width(NAME_LABEL_WIDTH));
					EditorGUILayout.LabelField("Event Index", GUILayout.Width(LABEL_WIDTH));
					EditorGUILayout.LabelField("Start Condition", GUILayout.Width(LABEL_WIDTH));
					EditorGUILayout.LabelField("Event Category", GUILayout.Width(LABEL_WIDTH));
					EditorGUILayout.LabelField("Map File Name", GUILayout.Width(NAME_LABEL_WIDTH));
					EditorGUILayout.LabelField("Map Name", GUILayout.Width(NAME_LABEL_WIDTH));
					EditorGUILayout.LabelField("Position", GUILayout.Width(LABEL_WIDTH));
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show event mapping data based on the list that is created in GetEventRef method.
		/// </Summary>
		void ShowEventMapping(){
			if (eventMappingList == null){
				return;
			}
			var query = eventMappingList.OrderBy(mapping => mapping.eventId)
										.ThenBy(mapping => mapping.mapName)
										.ThenBy(mapping => mapping.pos.x)
										.ThenBy(mapping => mapping.pos.y)
										.ThenBy(mapping => mapping.eventId);
			var sortedList = query.ToList();

			foreach (EventMappingData mapping in sortedList){
				if (mapping == null){
					continue;
				}
				GUIStyle editingMap = new GUIStyle();
				Color backColor = GUI.backgroundColor;

				if (mapping.mapFileName == saveFileName){
					editingMap.normal.textColor = GUI.skin.label.normal.textColor;
					editingMap.normal.background = highlightTex;
				} else if (mapping.mapFileName == MapEditorUtil.notAssignedText){
					editingMap.normal.textColor = Color.yellow;
				} else {
					editingMap.normal.textColor = GUI.skin.label.normal.textColor;
					editingMap.normal.background = GUI.skin.label.normal.background;
				}
				ShowEventMappingParts(mapping, editingMap);
			}
		}

		/// <Summary>
		/// Mapping data parts of event mapping.
		/// </Summary>
		/// <param name="mapping">Event mapping data.</param>
		/// <param name="editingMap">Style to highlight.</param>
		void ShowEventMappingParts(EventMappingData mapping, GUIStyle editingMap){
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField(mapping.eventId.ToString(), GUILayout.Width(ID_LABEL_WIDTH));
				EditorGUILayout.LabelField(mapping.eventName, GUILayout.Width(NAME_LABEL_WIDTH));
				EditorGUILayout.LabelField(mapping.eventIndex.ToString(), GUILayout.Width(LABEL_WIDTH));
				EditorGUILayout.LabelField(mapping.startCondition, GUILayout.Width(LABEL_WIDTH));
				EditorGUILayout.LabelField(mapping.eventCategory, GUILayout.Width(LABEL_WIDTH));
				EditorGUILayout.LabelField(mapping.mapFileName, editingMap, GUILayout.Width(NAME_LABEL_WIDTH));
				EditorGUILayout.LabelField(mapping.mapName, editingMap, GUILayout.Width(NAME_LABEL_WIDTH));
				EditorGUILayout.LabelField(mapping.pos.ToString(), GUILayout.Width(LABEL_WIDTH));
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
