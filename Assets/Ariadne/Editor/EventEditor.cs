using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Ariadne {

	/// <Summary>
	/// Editor class for event data.
	/// </Summary>
	public class EventEditor : EditorWindow {

		const string undoName = "Change Event Setting";
		int leftPaneWidth = 400;

		// Event info
		int eventId = 0;
		[SerializeField]
		string eventName = "New Event";
		string eventDataPathPrefix = "Assets/Ariadne/Resources/";
		string eventDataPath = "EventData/";
		string eventFilePrefix = "event_";
		[SerializeField]
		List<AriadneEventParts> eventPartsList;
		[SerializeField]
		AriadneEventParts editingEventParts;
		int selectedParts = -1;
		int selectedFlagIndex;
		
		string msg = "";
		bool isShowMap = false;
		Vector2 leftScrollPos = Vector2.zero;
		Vector2 rightScrollPos = Vector2.zero;
		
		GUIStyle removeStyle;
		GUIStyle editStyle;
		GUIStyle boldLabelStyle;

		// For Move Position event.
		int cellSize = 16;
		Color removeColor = new Color(1.0f, 0.2f, 0.0f, 1.0f);
		Color baseColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		Color gridColor = new Color(0.7f, 0.9f, 0.7f, 1.0f);
		Color selectedColor = new Color(0.0f, 0.5f, 1.0f, 0.5f);
		Color hoverColor = new Color(0.0f, 0.5f, 1.0f, 0.2f);
		Color[] selectedColors;
		Color[] hoverColors;

		Texture2D mapBg;
		Texture2D selectedTex;
		Texture2D hoverTex;
		Texture2D[] mapIconList;
		bool isPosSelected = false;
		[SerializeField]
		Vector2Int selectedGridPos = Vector2Int.zero;
		Vector2Int hoverGridPos = Vector2Int.zero;
		DungeonMasterData oldDungeon;
		int[] floorIds;
		string[] floorNames;
		[SerializeField]
		int selectedFloor;
		FloorMapMasterData oldMap;

		// For treasure event.
		int[] itemIds;
		string[] itemNames;

		// For event start condition.
		string[] flagNames;

		/// <Summary>
		/// Show GUI for setting of the event data.
		/// </Summary>
		void OnGUI(){
			EditorGUILayout.LabelField("Ariadne Event Editor", boldLabelStyle);
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			{
				leftScrollPos = EditorGUILayout.BeginScrollView(leftScrollPos, GUI.skin.box, GUILayout.Width(leftPaneWidth + 40f));
				{
					EditorGUILayout.BeginVertical();
					{
						ShowEventSettingParts();
						ShowEachEventPartsInformation();

						EditorGUILayout.Space();
						ShowSaveAndCancelButtonParts();
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndScrollView();

				rightScrollPos = EditorGUILayout.BeginScrollView(rightScrollPos, GUI.skin.box);
				{
					EditorGUILayout.BeginVertical();
					{
						ShowEditingEventIndexParts();
						ShowEditingPartsDetail();
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndScrollView();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}

		/// <Summary>
		/// Show setting parts about event data such as event name.
		/// </Summary>
		void ShowEventSettingParts(){
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.LabelField("Event ID", eventId.ToString());
				Undo.RecordObject(this, undoName);
				eventName = EditorGUILayout.TextField("Event Name", eventName);
				EditorGUILayout.Space();
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show information parts about event parts data.
		/// </Summary>
		void ShowEachEventPartsInformation(){
			EditorGUILayout.BeginVertical();
			{
				for (int i = 0; i < eventPartsList.Count; i++){
					EditorGUILayout.BeginVertical("Box");
					{
						ShowEventPartsCategoryAndConditions(i);
						ShowEditAndRemoveButtonParts(i);
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();
				}
				
				if (GUILayout.Button("Add event parts")){
					var newParts = new AriadneEventParts();
					Undo.RecordObject(this, undoName);
					eventPartsList.Add(newParts);
				}
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show information parts about event parts data in detail.
		/// </Summary>
		/// <param name="i">Index of the event parts.</param>
		void ShowEventPartsCategoryAndConditions(int i){
			if (selectedParts == i){
				EditorGUILayout.LabelField("Event List Index : " + i.ToString() + " (In Editing)", boldLabelStyle);
			} else {
				EditorGUILayout.LabelField("Event List Index : " + i.ToString());
			}

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Start Condition");
				EditorGUILayout.LabelField(eventPartsList[i].startCondition.ToString());
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Event Trigger");
				EditorGUILayout.LabelField(eventPartsList[i].eventTrigger.ToString());
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Event Start Pos");
				EditorGUILayout.LabelField(eventPartsList[i].eventPos.ToString());
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("Event Category");
				EditorGUILayout.LabelField(eventPartsList[i].eventCategory.ToString());
			}
			EditorGUILayout.EndHorizontal();
		}

		/// <Summary>
		/// Show edit button and remove button.
		/// When remove button is pressed, confirming dialog will be shown.
		/// </Summary>
		/// <param name="i">Index of the event parts.</param>
		void ShowEditAndRemoveButtonParts(int i){
			EditorGUILayout.BeginHorizontal();
			{
				if (eventPartsList.Count > 1){
					if (GUILayout.Button("Remove", removeStyle)){
						string confirmMsg = "Remove this event parts?" + "\n"
											+ "Index : " + i.ToString() + " \n"
											+ "Event Category : " + eventPartsList[i].eventCategory.ToString();
						bool dialogChoice = EditorUtility.DisplayDialog("Confirm",
																	confirmMsg,
																	"Remove",
																	"Cancel");
						if (dialogChoice){
							selectedParts = -1;
							Undo.RecordObject(this, undoName);
							eventPartsList.RemoveAt(i);
						}
					}
				}

				if (GUILayout.Button("Edit", editStyle)){
					// Set event data of this index.
					selectedParts = i;
					editingEventParts = new AriadneEventParts(eventPartsList[selectedParts]);
					GetExecutedFlagList();
					ClearUndoStack();
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		/// <Summary>
		/// Show save button and cancel button.
		/// </Summary>
		void ShowSaveAndCancelButtonParts(){
			EditorGUILayout.BeginVertical("Box");
			{
				if (GUILayout.Button("Save", editStyle)){
					SaveEventFile();
				}
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show index of editing event parts.
		/// </Summary>
		void ShowEditingEventIndexParts(){
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.LabelField("Event Edit", boldLabelStyle);
				if (selectedParts >= 0){
					EditorGUILayout.LabelField("Editing event index : " + selectedParts.ToString());
				}
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show setting GUI for event parts.
		/// </Summary>
		void ShowEditingPartsDetail(){
			if (selectedParts < 0){
				return;
			}

			EditorGUILayout.BeginVertical();
			{
				ShowEventStartConditionParts();
				ShowEventExecutedFlagParts();
				ShowEventTriggerAndStartPositionParts();
				ShowEventCategoryParts();
			}
			EditorGUILayout.EndVertical();

			ShowSelectDestinationMapParts();
			ShowApplyAndCancelButtonParts();
		}

		/// <Summary>
		/// Show setting GUI for start condition.
		/// </Summary>
		void ShowEventStartConditionParts(){
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.LabelField("Condition", boldLabelStyle);
				Undo.RecordObject(this, undoName);
				editingEventParts.startCondition = (AriadneEventCondition)EditorGUILayout.EnumPopup("Event start condition", editingEventParts.startCondition);
				if (editingEventParts.startCondition == AriadneEventCondition.Flag){
					// Show flag name popup.
					selectedFlagIndex = Array.IndexOf(flagNames, editingEventParts.startFlagName);
					selectedFlagIndex = EditorGUILayout.Popup("Event start flag", selectedFlagIndex, flagNames);
					if (selectedFlagIndex >= 0 && selectedFlagIndex < flagNames.Length){
						Undo.RecordObject(this, undoName);
						editingEventParts.startFlagName = flagNames[selectedFlagIndex];
					}
				} else if (editingEventParts.startCondition == AriadneEventCondition.Item){
					// Show item condition.
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("When the player has ");
						Undo.RecordObject(this, undoName);
						editingEventParts.startItemId = EditorGUILayout.IntPopup(editingEventParts.startItemId, itemNames, itemIds);
						Undo.RecordObject(this, undoName);
						editingEventParts.comparisonOperator = (AriadneComparison)EditorGUILayout.EnumPopup(editingEventParts.comparisonOperator);
						Undo.RecordObject(this, undoName);
						editingEventParts.startNum = EditorGUILayout.IntField(editingEventParts.startNum);
					}
					EditorGUILayout.EndHorizontal();
				} else if (editingEventParts.startCondition == AriadneEventCondition.Money){
					// Show money condition.
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("When the player has Money ");
						Undo.RecordObject(this, undoName);
						editingEventParts.comparisonOperator = (AriadneComparison)EditorGUILayout.EnumPopup(editingEventParts.comparisonOperator);
						Undo.RecordObject(this, undoName);
						editingEventParts.startNum = EditorGUILayout.IntField(editingEventParts.startNum);
					}
					EditorGUILayout.EndHorizontal();
				} 
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show setting GUI for executed flag and name of the flag.
		/// </Summary>
		void ShowEventExecutedFlagParts(){
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.LabelField("Executed Flag", boldLabelStyle);
				Undo.RecordObject(this, undoName);
				editingEventParts.hasExecutedFlag = EditorGUILayout.Toggle("Event has executed flag", editingEventParts.hasExecutedFlag);
				Undo.RecordObject(this, undoName);
				editingEventParts.executedFlagName = EditorGUILayout.TextField("Flag name", editingEventParts.executedFlagName);
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show setting GUI for start trigger and start position of the event.
		/// </Summary>
		void ShowEventTriggerAndStartPositionParts(){
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.LabelField("Trigger and Position", boldLabelStyle);
				Undo.RecordObject(this, undoName);
				editingEventParts.eventTrigger = (AriadneEventTrigger)EditorGUILayout.EnumPopup("Select event trigger", editingEventParts.eventTrigger);
				Undo.RecordObject(this, undoName);
				editingEventParts.eventPos = (AriadneEventPosition)EditorGUILayout.EnumPopup("Select event position", editingEventParts.eventPos);
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show setting GUI for the category of the event.
		/// </Summary>
		void ShowEventCategoryParts(){
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.LabelField("Category", boldLabelStyle);
				Undo.RecordObject(this, undoName);
				editingEventParts.eventCategory = (AriadneEventCategory)EditorGUILayout.EnumPopup("Select event category", editingEventParts.eventCategory);
				if (editingEventParts.eventCategory != AriadneEventCategory.MovePosition){
					isShowMap = false;
				}
				
				switch (editingEventParts.eventCategory){
					case AriadneEventCategory.LockedDoor:
						ShowLockedDoorEventParts();
						break;

					case AriadneEventCategory.MovePosition:
						ShowMovePositionEventParts();
						break;

					case AriadneEventCategory.Treasure:
						 ShowTreasureEventParts();
						break;

					case AriadneEventCategory.Messenger:
						ShowMessengerEventParts();
						break;
				}
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show setting GUI about locked door event.
		/// </Summary>
		void ShowLockedDoorEventParts(){
			Undo.RecordObject(this, undoName);
			editingEventParts.doorKeyType = (DoorKeyType)EditorGUILayout.EnumPopup("Key type of locked door", editingEventParts.doorKeyType);
		}

		/// <Summary>
		/// Show setting GUI about move position event.
		/// </Summary>
		void ShowMovePositionEventParts(){
			// Dest dungeon
			oldDungeon = editingEventParts.destDungeon;
			Undo.RecordObject(this, undoName);
			editingEventParts.destDungeon = (DungeonMasterData)EditorGUILayout.ObjectField("Dest dungeon", editingEventParts.destDungeon, typeof(DungeonMasterData), false);
			if (editingEventParts.destDungeon == null){
				return;
			}
			DungeonFloorStruct dfs = new DungeonFloorStruct(editingEventParts.destDungeon);
			floorIds = dfs.floorIds;
			floorNames = dfs.floorNames;

			if (oldDungeon != editingEventParts.destDungeon){
				selectedFloor = 0;
				editingEventParts.destMap = null;
				isPosSelected = false;
				selectedGridPos = Vector2Int.zero;
				editingEventParts.destPos = Vector2Int.zero;
			}

			// Dest map
			oldMap = editingEventParts.destMap;
			Undo.RecordObject(this, undoName);
			selectedFloor = EditorGUILayout.IntPopup("Dest map", selectedFloor, floorNames, floorIds);
			editingEventParts.destMap = editingEventParts.destDungeon.floorList.Find((floor) => floor.floorId == selectedFloor);
			if (editingEventParts.destMap != null){
				isShowMap = true;
			} else {
				isShowMap = false;
			}
			if (oldMap != editingEventParts.destMap){
				isPosSelected = false;
				selectedGridPos = Vector2Int.zero;
				editingEventParts.destPos = Vector2Int.zero;
			}

			// Dest position
			Undo.RecordObject(this, undoName);
			editingEventParts.destPos = EditorGUILayout.Vector2IntField("Dest position", editingEventParts.destPos);
			if (isPosSelected){
				editingEventParts.destPos = selectedGridPos;
			}
			Undo.RecordObject(this, undoName);
			editingEventParts.direction = (DungeonDir)EditorGUILayout.EnumPopup("Direction of after move", editingEventParts.direction);
		}

		/// <Summary>
		/// Show setting GUI about treasure event.
		/// </Summary>
		void ShowTreasureEventParts(){
			Undo.RecordObject(this, undoName);
			editingEventParts.treasureType = (TreasureType)EditorGUILayout.EnumPopup("Treasure type", editingEventParts.treasureType);

			if (editingEventParts.treasureType == TreasureType.Item){
				Undo.RecordObject(this, undoName);
				editingEventParts.itemId = EditorGUILayout.IntPopup("Item Id", editingEventParts.itemId, itemNames, itemIds);
				Undo.RecordObject(this, undoName);
				editingEventParts.itemNum = EditorGUILayout.IntField("Num", editingEventParts.itemNum);
			} else {
				Undo.RecordObject(this, undoName);
				editingEventParts.itemNum = EditorGUILayout.IntField("Money", editingEventParts.itemNum);
			}
		}

		/// <Summary>
		/// Show setting GUI about messenger event.
		/// </Summary>
		void ShowMessengerEventParts(){
			if (editingEventParts.msgList.Count == 0){
				editingEventParts.msgList.Add(msg);
			} else {
				for (int i = 0; i < editingEventParts.msgList.Count; i++){
					Undo.RecordObject(this, undoName);
					editingEventParts.msgList[i] = EditorGUILayout.TextArea(editingEventParts.msgList[i]);
				}
				if (GUILayout.Button("Add page")){
					Undo.RecordObject(this, undoName);
					editingEventParts.msgList.Add(msg);
				}
				if (editingEventParts.msgList.Count > 1){
					if (GUILayout.Button("Remove page")){
						Undo.RecordObject(this, undoName);
						editingEventParts.msgList.RemoveAt(editingEventParts.msgList.Count - 1);
					}
				}
			}
		}

		/// <Summary>
		/// Show setting GUI about destination map that relates move position event.
		/// </Summary>
		void ShowSelectDestinationMapParts(){
			if (!isShowMap || editingEventParts.destMap == null || editingEventParts.destDungeon == null){
				return;
			}

			int horizontal = editingEventParts.destMap.floorSizeHorizontal;
			int vertical = editingEventParts.destMap.floorSizeVertical;

			mapBg = MapEditorUtil.RepaintMapTexture(horizontal, vertical, cellSize, gridColor, baseColor);
			EditorGUILayout.BeginVertical("Box");
			{
				GUIContent content = new GUIContent(mapBg);
				GUILayout.Box(content);

				Rect mapRect = GUILayoutUtility.GetLastRect();
				Rect[,] iconRect = MapEditorUtil.GetMapRectArray(mapRect, horizontal, vertical, cellSize);
				SetMapAttrIconToMap(iconRect, editingEventParts.destMap.mapInfo, horizontal, vertical);
				Rect[,] highlightRect = MapEditorUtil.GetMapRectArray(mapRect, horizontal, vertical, cellSize);
				if (isPosSelected){
					SetHoverColor(highlightRect[selectedGridPos.x, selectedGridPos.y], selectedTex);
				}

				wantsMouseMove = true;
				if (mapRect.Contains(Event.current.mousePosition)){
					Vector2 pos = new Vector2(Event.current.mousePosition.x - mapRect.position.x, Event.current.mousePosition.y - mapRect.position.y);
					hoverGridPos = MapEditorUtil.GetPosInGrid(pos, horizontal, vertical, cellSize);
					SetHoverColor(highlightRect[hoverGridPos.x, hoverGridPos.y], hoverTex);

					if (Event.current.type == EventType.MouseUp){
						Undo.RecordObject(this, undoName);
						selectedGridPos = MapEditorUtil.GetPosInGrid(pos, horizontal, vertical, cellSize);
						isPosSelected = true;
					}
					Repaint();
				}
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show apply button and cancel button.
		/// </Summary>
		void ShowApplyAndCancelButtonParts(){
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("Cancel")){
						selectedParts = -1;
					}
					GUIStyle applyStyle = new GUIStyle(GUI.skin.button);
					applyStyle.fontStyle = FontStyle.Bold;
					if (GUILayout.Button("Apply", applyStyle)){
						eventPartsList[selectedParts] = editingEventParts;
						editingEventParts = new AriadneEventParts(eventPartsList[selectedParts]);
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Clear Undo objects.
		/// </Summary>
		void CloseWindowProcess(){
			Undo.FlushUndoRecordObjects();
			Close();
		}

		/// <Summary>
		/// Initialize settings of event editor.
		/// </Summary>
		public void InitializeEvent(int selectedEventId){
			SetEventId(selectedEventId);
			SetFilePathStrings();
			InitializeProperties();
			LoadEventFile();

			SetColorArrays();
			SetHoverTexture();
			SetSelectedTexture();

			SetMapIconArray();
			InitializeStyles();
			SetCallbackForUndo();
			ClearUndoStack();
		}

		/// <Summary>
		/// Set event id to EventEditor.
		/// </Summary>
		/// <param name="eventId">Event ID provided in MapEditor.</param>
		void SetEventId(int eventId){
			this.eventId = eventId;
		}

		/// <Summary>
		/// Get file path from MapEditorUtil and set them to EventEditor.
		/// </Summary>
		void SetFilePathStrings(){
			eventName = MapEditorUtil.eventDefaultName;
			eventDataPathPrefix = MapEditorUtil.eventDataPathPrefix;
			eventDataPath = MapEditorUtil.eventDataPath;
			eventFilePrefix = MapEditorUtil.eventFilePrefix;
		}

		/// <Summary>
		/// Set color arrays to show destination map.
		/// </Summary>
		void SetColorArrays(){
			hoverColors = new Color[cellSize * cellSize];
			selectedColors = new Color[cellSize * cellSize];
			for (int i = 0; i < selectedColors.Length; i++){
				selectedColors[i] = selectedColor;
				hoverColors[i] = hoverColor;
			}
		}

		/// <Summary>
		/// Set a texture of the selected color in the destination selection map.
		/// </Summary>
		void SetSelectedTexture(){
			selectedTex = new Texture2D(cellSize, cellSize, TextureFormat.ARGB32, false);
			selectedTex.SetPixels(0, 0, cellSize, cellSize, selectedColors);
			selectedTex.Apply();
		}

		/// <Summary>
		/// Set a texture of the hover color in the destination selection map.
		/// </Summary>
		void SetHoverTexture(){
			hoverTex = new Texture2D(cellSize, cellSize, TextureFormat.ARGB32, false);
			hoverTex.SetPixels(0, 0, cellSize, cellSize, hoverColors);
			hoverTex.Apply();
		}

		/// <Summary>
		/// Set up map icons.
		/// </Summary>
		void SetMapIconArray(){
			mapIconList = MapEditorUtil.GetMapIconArray();
		}

		/// <Summary>
		/// Set the texture of hover color to the rect.
		/// </Summary>
		void SetHoverColor(Rect r, Texture2D tex){
			if (tex != null){
				GUI.DrawTexture(r, tex);
			}
		}

		/// <Summary>
		/// Draw map icons to the destination selection map.
		/// </Summary>
		void SetMapAttrIconToMap(Rect[,] r, List<MapInfo> mapInfoList, int horizontal, int vertical){
			for (int y = 0; y < vertical; y++){
				for (int x = 0; x < horizontal; x++){
					int index = x + y * horizontal;
					MapInfo info = mapInfoList[index];
					if (info.mapAttr != MapAttributeDefine.HALL_WAY){
						GUI.DrawTexture(r[x, y], mapIconList[info.mapAttr]);
					}
				}
			}
		}

		/// <Summary>
		/// Register action for the callback of Undo/Redo.
		/// </Summary>
		void SetCallbackForUndo(){
			Undo.undoRedoPerformed += () => {
				Repaint();
			};
		}

		/// <Summary>
		/// Clear Undo/Redo stack to prevent setting data of another file.
		/// </Summary>
		void ClearUndoStack(){
			Undo.FlushUndoRecordObjects();
			Undo.ClearAll();
		}

		/// <Summary>
		/// Load event file data from saved path.
		/// </Summary>
		void LoadEventFile(){
			string eventFileName = eventId.ToString("D5") + ".asset";
			string path = eventDataPathPrefix + eventDataPath + eventFilePrefix + eventFileName;
			
			var asset = (EventMasterData)AssetDatabase.LoadAssetAtPath(path, typeof(EventMasterData));
			if (asset == null){
				return;
			}
			this.eventName = asset.eventName;
			this.eventPartsList = GetEventPartsListForEdit(asset.eventParts);
		}

		/// <Summary>
		/// Ready event parts for edit to avoid changing ScriptableObject directly.
		/// </Summary>
		List<AriadneEventParts> GetEventPartsListForEdit(List<AriadneEventParts> partsList){
			List<AriadneEventParts> partsListForEdit = new List<AriadneEventParts>();
			if (partsList == null){
				return partsListForEdit;
			}
			foreach (AriadneEventParts parts in partsList){
				if (parts == null){
					continue;
				}
				AriadneEventParts partsForEdit = new AriadneEventParts(parts);
				partsListForEdit.Add(partsForEdit);
			}
			return partsListForEdit;
		}

		/// <Summary>
		/// Save EventMasterData as ScriptableObject.
		/// </Summary>
		void SaveEventFile(){
			var eventData = ScriptableObject.CreateInstance<EventMasterData>();
			eventData.eventId = this.eventId;
			eventData.eventName = this.eventName;
			eventData.eventParts = this.eventPartsList;

			string eventFileName = eventId.ToString("D5") + ".asset";
			string path = eventDataPathPrefix + eventDataPath + eventFilePrefix + eventFileName;
			var asset = (EventMasterData)AssetDatabase.LoadAssetAtPath(path, typeof(EventMasterData));
			if (asset == null){
				AssetDatabase.CreateAsset(eventData, path);
			} else {
				EditorUtility.CopySerialized(eventData, asset);
				AssetDatabase.SaveAssets();
			}
			AssetDatabase.Refresh();
		}

		/// <Summary>
		/// Initialize properties of EventEditor.
		/// </Summary>
		void InitializeProperties(){
			this.eventName = MapEditorUtil.eventDefaultName;
			this.eventPartsList = new List<AriadneEventParts>();
			this.editingEventParts = null;

			GetTreasureItemList();
			GetExecutedFlagList();
		}

		/// <Summary>
		/// Set GUIStyles of labels and buttons.
		/// </Summary>
		void InitializeStyles(){
			removeStyle = new GUIStyle(GUI.skin.button);
			removeStyle.normal.textColor = removeColor;
			removeStyle.fontStyle = FontStyle.Bold;

			editStyle = new GUIStyle(GUI.skin.button);
			editStyle.fontStyle = FontStyle.Bold;

			boldLabelStyle = new GUIStyle(GUI.skin.label);
			boldLabelStyle.fontStyle = FontStyle.Bold;
		}

		/// <Summary>
		/// Get an item list from item data.
		/// </Summary>
		void GetTreasureItemList(){
			List<int> itemIdList = new List<int>();
			List<string> itemNameList = new List<string>();

			string dataType = "ItemMasterData";
			string[] guidArray = MapEditorUtil.GetGuidArray(dataType);

			foreach (string guid in guidArray){
				string path = AssetDatabase.GUIDToAssetPath(guid);
				ItemMasterData itemData = AssetDatabase.LoadAssetAtPath<ItemMasterData>(path);
				if (itemData == null){
					continue;
				}
				itemIdList.Add(itemData.itemId);
				itemNameList.Add(itemData.itemName);
				
			}
			itemIds = itemIdList.ToArray();
			itemNames = itemNameList.ToArray();
		}


		/// <Summary>
		/// Get executed flag list from event data.
		/// </Summary>
		void GetExecutedFlagList(){
			List<string> flagNameList = new List<string>();

			string dataType = "EventMasterData";
			string[] guidArray = MapEditorUtil.GetGuidArray(dataType);

			foreach (string guid in guidArray){
				string path = AssetDatabase.GUIDToAssetPath(guid);
				EventMasterData eventData = AssetDatabase.LoadAssetAtPath<EventMasterData>(path);
				if (eventData == null){
					continue;
				}

				if (eventData.eventParts == null){
					continue;
				}
				foreach (AriadneEventParts parts in eventData.eventParts){
					if (parts == null){
						continue;
					}
					if (parts.hasExecutedFlag && parts.executedFlagName != ""){
						flagNameList.Add(parts.executedFlagName);
					}
				}
			}
			flagNames = flagNameList.ToArray();
		}
	}

	/// <Summary>
	/// Struct to hold floor ID and name.
	/// </Summary>
	public struct DungeonFloorStruct {
		public int[] floorIds;
		public string[] floorNames;

		/// <Summary>
		/// Constructor of DungeonFloorStruct.
		/// </Summary>
		/// <param name="dungeonData">Dungeon data to get floor list.</param>
		public DungeonFloorStruct(DungeonMasterData dungeonData){
			List<int> floorIdList = new List<int>();
			List<string> floorNameList = new List<string>();
			if (dungeonData != null){
				foreach (FloorMapMasterData floor in dungeonData.floorList){
					floorIdList.Add(floor.floorId);
					floorNameList.Add(floor.name);
				}
			}
			floorIds = floorIdList.ToArray();
			floorNames = floorNameList.ToArray();
		}
	}
}

