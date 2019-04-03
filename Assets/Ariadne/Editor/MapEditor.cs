using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ariadne {

	/// <Summary>
	/// Editor class for floor map data.
	/// </Summary>
	public class MapEditor : EditorWindow {

		// Const
		const int FLOOR_SIZE_MAX = 64;
		const string DEFAULT_FLOOR_NAME = "New Floor";
		const string DEFAULT_SAVE_FILE_NAME = "NewMap";
		const int DEFAULT_FLOOR_ID = 0;
		const string undoName = "Change Map Setting";

		const int LOAD_FILE_OK = 0;
		const int LOAD_FILE_IS_NULL = 1;
		const int LOAD_FILE_IS_TEMP = 2;

		// Const of tool ID.
		const int TOOL_SELECT = 0;
		const int TOOL_DRAW = 1;
		const int TOOL_DRAW_RECT = 2;
		const int TOOL_DRAW_RECT_FILL = 3;

		int settingPaneWidth = 400;
		
		// Floor map data settings
		[SerializeField]
		int floorSizeHorizontal = FloorMapConst.INIT_HORIONTAL_SIZE;
		[SerializeField]
		int floorSizeVertical = FloorMapConst.INIT_VERTICAL_SIZE;
		[SerializeField]
		int floorId = DEFAULT_FLOOR_ID;
		[SerializeField]
		string floorName = DEFAULT_FLOOR_NAME;
		[SerializeField]
		DungeonPartsMasterData dungeonParts;
		[SerializeField]
		Vector2Int entrancePos;
		[SerializeField]
		DungeonDir enteringDir;

		[SerializeField]
		List<MapInfo> editMapInfo;
		int selectedPosEventId = 0;
		int selectedMapIcon = 0;
		[SerializeField]
		DungeonDir selectedPosObjectFront;
		[SerializeField]
		int selectedMessengerTypeId;

		[SerializeField]
		int sliderHorizontal = FloorMapConst.INIT_HORIONTAL_SIZE;
		[SerializeField]
		int sliderVertical = FloorMapConst.INIT_VERTICAL_SIZE;

		bool isShowingLoadPane;
		bool isShowingSavePane;

		// Draw map GUI
		int cellSize = 32;
		Vector2Int selectedGridPos = Vector2Int.zero;
		Vector2Int hoverGridPos = Vector2Int.zero;
		Vector2Int dragStartGridPos = Vector2Int.zero;
		List<Vector2Int> dragPosList;
		Color baseColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		Color gridColor = new Color(0.7f, 0.9f, 0.7f, 1.0f);
		Color hoverColor = new Color(0.0f, 0.5f, 1.0f, 0.2f);
		Color selectedColor = new Color(0.0f, 0.5f, 1.0f, 0.5f);
		Color transparentColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		Color[] hoverColors;
		Color[] selectedColors;
		Color[] transparentColors;
		// Base layer texture (base color, grid)
		Texture2D mapBg;
		// Icon layer
		bool isPosSelected = false;

		// Map icon settings
		string[] mapAttrNameList = new string[]{"Hallway", "Wall", "Door", "LockedDoor", "Downstairs", "Upstairs", "Treasure", "Messenger", "Pillar", "WallWithTorch"};
		Texture2D[] mapIconList;
		Texture2D hoverTex;
		Texture2D selectedTex;
		bool isDragging = false;
		int selectedDrawTool = 0;
		string[] drawToolNameList = new string[]{"Select", "Draw", "Draw rect", "Draw filled rect"};
		Texture2D[] drawToolIconList;
		Texture2D eventIcon;
		Texture2D entranceIcon;

		// Setting about save file
		string tempFileName;
		string tempFilePathPrefix;
		string tempFilePath;
		string infoFileName;
		string saveFolderPrefix;
		[SerializeField]
		string saveFolderPath;
		string saveFilePostFix;
		[SerializeField]
		string saveFileName = DEFAULT_SAVE_FILE_NAME;

		// Setting about load file
		[SerializeField]
		FloorMapMasterData loadFloor;
		int loadFileState = LOAD_FILE_OK;

		GUIStyle boldLabelStyle;

		Vector2 leftPaneScrollPos = Vector2.zero;
		Vector2 rightPaneScrollPos = Vector2.zero;
		
		EventEditor eventEditor;
		EventMappingEditor eventMappingEditor;

		/// <Summary>
		/// Open MapEditor window.
		/// </Summary>
		[MenuItem("Window/MapEditor")]
		static void Open(){
			GetWindow<MapEditor>();
		}

		/// <Summary>
		/// Processes of opening MapEditor window.
		/// </Summary>
		public void Awake(){
			InitializeMapEditor();
			LoadTempFile();
		}

		/// <Summary>
		/// Initialize state of MapEditor.
		/// </Summary>
		void InitializeMapEditor(){
			SetInitialFilePath();
			SetColorArrays();

			SetHoverTexture();
			SetSelectedTexture();

			SetToolIcons();
			SetEventIcons();

			InitializePosSelection();
			InitializeDraggingEvent();

			SetCallbackForUndo();

			SetMapBackgroundTexture();
		}

		/// <Summary>
		/// Processes to open a new file.
		/// </Summary>
		void SetNewFile(){
			InitializePosSelection();
			SetInitialFilePath();
			InitializeDraggingEvent();
			InitializeProperties();
			SetDefaultDungeonParts();

			SetNewMapData();

			SetMapBackgroundTexture();
		}

		/// <Summary>
		/// When get focus, initialize map editor state.
		/// </Summary>
		void OnFocus(){
			InitializeMapEditor();
			LoadTempFileInfo();
		}

		/// <Summary>
		/// When lost focus, save the state of editing data to temp file.
		/// </Summary>
		void OnLostFocus(){
			SaveTempFile();
		}

		/// <Summary>
		/// When this window is destroyed, save the state of editing data to temp file.
		/// </Summary>
		void OnDestroy(){
			SaveTempFile();
		}

		/// <Summary>
		/// Show GUI for setting of the floor map data.
		/// </Summary>
		void OnGUI(){
			InitializeStyles();
			EditorGUILayout.LabelField("Ariadne Map Data Editor", boldLabelStyle);
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			{
				leftPaneScrollPos = EditorGUILayout.BeginScrollView(leftPaneScrollPos, GUI.skin.box, GUILayout.Width(settingPaneWidth + 40f));
				{
					ShowFileOperationButtons();

					ShowLoadMapFileParts();
					ShowSaveMapFileParts();
					EditorGUILayout.Space();

					ShowFloorSettingParts();
					EditorGUILayout.Space();

					ShowSelectedPositionInformationParts();
				}
				EditorGUILayout.EndScrollView();

				rightPaneScrollPos = EditorGUILayout.BeginScrollView(rightPaneScrollPos, GUI.skin.box);
				{
					ShowToolBarParts();
					EditorGUILayout.BeginHorizontal();
					{
						ShowVerticalScaleParts();
						ShowMapParts();
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndScrollView();
			}
			EditorGUILayout.EndHorizontal();
		}

		/// <Summary>
		/// Show setting GUI for file operation.
		/// </Summary>
		void ShowFileOperationButtons(){
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("New")){
						bool dialogChoice = EditorUtility.DisplayDialog("Create new file",
																			"Editing data will be discarded. Create new file?",
																			"Create New",
																			"Cancel");
						if (dialogChoice){
							SetNewFile();
							ClearUndoStack();
						}
					}
					if (GUILayout.Button("Load")){
						isShowingLoadPane = !isShowingLoadPane;
						isShowingSavePane = false;
					}
					if (GUILayout.Button("Save")){
						isShowingSavePane = !isShowingSavePane;
						isShowingLoadPane = false;
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show setting GUI for loading file.
		/// </Summary>
		void ShowLoadMapFileParts(){
			if (!isShowingLoadPane){
				return;
			}

			EditorGUILayout.BeginVertical("Box");
			{
				Undo.RecordObject(this, undoName);
				loadFloor = (FloorMapMasterData)EditorGUILayout.ObjectField("Load floor map data", loadFloor, typeof(FloorMapMasterData), false);
				if (GUILayout.Button("Load Map File")){
					CheckLoadFileProcess();
				}
				CheckLoadingWarn();
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Check the specified file and load it.
		/// </Summary>
		void CheckLoadFileProcess(){
			if (loadFloor == null){
				loadFileState = LOAD_FILE_IS_NULL;
				return;
			}
			if (loadFloor.name == MapEditorUtil.tempFileName){
				loadFileState = LOAD_FILE_IS_TEMP;
				return;
			}
			loadFileState = LOAD_FILE_OK;
			bool dialogChoice = EditorUtility.DisplayDialog("Load floor map data",
															"Editing data will be discarded. Open this file?",
															"OK",
															"Cancel");
			if (dialogChoice){
				InitializePosSelection();
				LoadMapFile();
				SetMapBackgroundTexture();
				ClearUndoStack();
			}
		}

		/// <Summary>
		/// Check warnings about loading the file.
		/// </Summary>
		void CheckLoadingWarn(){
			switch (loadFileState){
				case LOAD_FILE_IS_NULL:
					EditorGUILayout.HelpBox("Load file is not assigned.", MessageType.Error);
					break;
				case LOAD_FILE_IS_TEMP:
					EditorGUILayout.HelpBox("You can not load the temp file directly.", MessageType.Error);
					break;
			}
		}

		/// <Summary>
		/// Show setting GUI for saving the file.
		/// </Summary>
		void ShowSaveMapFileParts(){
			if (!isShowingSavePane){
				return;
			}
			EditorGUILayout.BeginVertical("Box");
			{
				Undo.RecordObject(this, undoName);
				saveFolderPath = EditorGUILayout.TextField("Path : Assets/", saveFolderPath);
				Undo.RecordObject(this, undoName);
				saveFileName = EditorGUILayout.TextField("File Name", saveFileName);
				if (saveFileName == ""){
					EditorGUILayout.HelpBox("Input map file name.", MessageType.Error);
				}
				EditorGUILayout.Space();

				if (GUILayout.Button("Save Map File")){
					SaveMapFile();
				}
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show setting GUI about the floor.
		/// </Summary>
		void ShowFloorSettingParts(){
			EditorGUILayout.BeginVertical("Box");
			{
				// Input size of dungeon
				EditorGUILayout.LabelField("Floor Settings", boldLabelStyle);
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Floor Size");

				// Floor size - Horizontal
				int preSizeHorizontal = floorSizeHorizontal;
				Undo.RecordObject(this, undoName);
				sliderHorizontal = EditorGUILayout.IntSlider("Horizontal Size", sliderHorizontal, 1, FLOOR_SIZE_MAX);
				if (sliderHorizontal <= 0){
					EditorGUILayout.HelpBox("Floor size should be larger than 0.", MessageType.Error);
				}

				// Floor size - Vertical
				int preSizeVertical = floorSizeVertical;
				Undo.RecordObject(this, undoName);
				sliderVertical = EditorGUILayout.IntSlider("Vertical Size", sliderVertical, 1, FLOOR_SIZE_MAX);
				if (sliderVertical <= 0){
					EditorGUILayout.HelpBox("Floor size should be larger than 0.", MessageType.Error);
				}

				if (GUILayout.Button("Apply")){
					if (preSizeHorizontal > sliderHorizontal || preSizeVertical > sliderVertical){
						bool dialogChoice = EditorUtility.DisplayDialog("Apply new floor size",
																		"The new floor size is smaller than old one. A part of map data will be cut off.",
																		"Continue",
																		"Cancel");
						if (dialogChoice){
							ChangeMapSizeProcess(preSizeHorizontal, preSizeVertical);
						}
					} else {
						ChangeMapSizeProcess(preSizeHorizontal, preSizeVertical);
					}
				}
				EditorGUILayout.Space();

				Undo.RecordObject(this, undoName);
				floorName = EditorGUILayout.TextField("Floor Name", floorName);
				Undo.RecordObject(this, undoName);
				floorId = EditorGUILayout.IntField("Floor ID", floorId);
				EditorGUILayout.Space();

				// Dungeon parts data
				Undo.RecordObject(this, undoName);
				dungeonParts = (DungeonPartsMasterData)EditorGUILayout.ObjectField("Set dungeon parts data", dungeonParts, typeof(DungeonPartsMasterData), false);
				EditorGUILayout.Space();

				// Entrance position setting
				Undo.RecordObject(this, undoName);
				entrancePos = EditorGUILayout.Vector2IntField("Entrance position", entrancePos);
				ValidateEntrancePos();

				Undo.RecordObject(this, undoName);
				enteringDir = (DungeonDir)EditorGUILayout.EnumPopup("Direction of entering", enteringDir);

				EditorGUILayout.Space();
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Process about changing the map size.
		/// </Summary>
		void ChangeMapSizeProcess(int preSizeHorizontal, int preSizeVertical){
			if (preSizeHorizontal != sliderHorizontal){
				InitializePosSelection();
				Undo.RecordObject(this, undoName);
				floorSizeHorizontal = sliderHorizontal;
				SetMapBackgroundTexture();
				ConvertMapHorizontalChanged(preSizeHorizontal);
			}

			if (preSizeVertical != sliderVertical){
				InitializePosSelection();
				Undo.RecordObject(this, undoName);
				floorSizeVertical = sliderVertical;
				SetMapBackgroundTexture();
				ConvertMapVerticalChanged(preSizeVertical);
			}
		}

		/// <Summary>
		/// Show setting GUI for each position on the map.
		/// </Summary>
		void ShowSelectedPositionInformationParts(){
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.LabelField("Position Settings", boldLabelStyle);
				EditorGUILayout.Space();

				Vector2Int showPos = isPosSelected ? selectedGridPos : hoverGridPos;
				EditorGUILayout.Vector2IntField("Selected Position", showPos);

				int index = showPos.x + showPos.y * floorSizeHorizontal;

				// Detail information
				ShowPositionDetailParts(index);
				EditorGUILayout.Space();

				// Event editor
				ShowEventEditorParts(index);

				// Event list in this map
				ShowEventMappingButton();
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show setting GUI for the position that is selected on the map.
		/// </Summary>
		/// <param name="index">Index in the MapInfo array.</param>
		void ShowPositionDetailParts(int index){
			EditorGUILayout.BeginVertical();
			{
				// Map attribute
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField("Map Attribute");
					string toolName = MapEditorUtil.CheckEditMapIndexIsValid(index, editMapInfo.Count) ? mapAttrNameList[editMapInfo[index].mapAttr] : "";
					EditorGUILayout.LabelField(toolName);
				}
				EditorGUILayout.EndHorizontal();

				// Object front
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField("Front direction of object");
					selectedPosObjectFront = MapEditorUtil.CheckEditMapIndexIsValid(index, editMapInfo.Count) ? editMapInfo[index].objectFront : 0;
					EditorGUILayout.LabelField(selectedPosObjectFront.ToString());
				}
				EditorGUILayout.EndHorizontal();

				// Messenger type ID
				if (MapEditorUtil.CheckEditMapIndexIsValid(index, editMapInfo.Count)){
					if (editMapInfo[index].mapAttr == MapAttributeDefine.MESSENGER){
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField("Messenger Type ID");
							selectedMessengerTypeId = MapEditorUtil.CheckEditMapIndexIsValid(index, editMapInfo.Count) ? editMapInfo[index].messengerType : 0;
							EditorGUILayout.LabelField(selectedMessengerTypeId.ToString());
						}
						EditorGUILayout.EndHorizontal();
					}
				}

				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField("Event");
					selectedPosEventId = MapEditorUtil.CheckEditMapIndexIsValid(index, editMapInfo.Count) ? editMapInfo[index].eventId : 0;
					EditorGUILayout.LabelField(selectedPosEventId.ToString());
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show setting GUI about event settings.
		/// </Summary>
		/// <param name="index">Index in the MapInfo array.</param>
		void ShowEventEditorParts(int index){
			if (selectedDrawTool != TOOL_SELECT || !isPosSelected){
				return;
			}

			EditorGUILayout.BeginVertical();
			{
				// Object front define.
				Undo.RecordObject(this, undoName);
				selectedPosObjectFront = (DungeonDir)EditorGUILayout.EnumPopup("Object front", selectedPosObjectFront);
				if (MapEditorUtil.CheckEditMapIndexIsValid(index, editMapInfo.Count)){
					MapInfo infoFront = editMapInfo[index];
					infoFront.objectFront = selectedPosObjectFront;
					editMapInfo[index] = infoFront;
				}

				// Messenger type define.
				if (MapEditorUtil.CheckEditMapIndexIsValid(index, editMapInfo.Count)){
					if (editMapInfo[index].mapAttr == MapAttributeDefine.MESSENGER){
						string[] messengerNameArray = GetMessengerNameArray(dungeonParts.messengerObjects);
						int[] messengerIdArray = GetMessengerIdArray(dungeonParts.messengerObjects);
						Undo.RecordObject(this, undoName);
						selectedMessengerTypeId = EditorGUILayout.IntPopup("Messenger Type", selectedMessengerTypeId, messengerNameArray, messengerIdArray);
						
						MapInfo infoMessenger = editMapInfo[index];
						infoMessenger.messengerType = selectedMessengerTypeId;
						editMapInfo[index] = infoMessenger;
						EditorGUILayout.Space();
					}
				}

				selectedPosEventId = EditorGUILayout.IntField("Selected pos event id", selectedPosEventId);
				if (selectedPosEventId >= 0){
					int selectedPosIndex = selectedGridPos.x + selectedGridPos.y * floorSizeHorizontal;
					if (MapEditorUtil.CheckEditMapIndexIsValid(selectedPosIndex, editMapInfo.Count)){
						MapInfo info = new MapInfo();
						info = editMapInfo[selectedPosIndex];
						info.eventId = selectedPosEventId;
						Undo.RecordObject(this, undoName);
						editMapInfo[selectedPosIndex] = info;
					}
					if (selectedPosEventId > 0){
						if (GUILayout.Button("Open Event Editor")){
							if (eventEditor == null){
								eventEditor = CreateInstance<EventEditor>();
								eventEditor.InitializeEvent(selectedPosEventId);
							}
							eventEditor.ShowUtility();
						}
						string eventFileName = selectedPosEventId.ToString("D5") + ".asset";
						string path = MapEditorUtil.eventDataPathPrefix + MapEditorUtil.eventDataPath + MapEditorUtil.eventFilePrefix + eventFileName;
						var asset = (EventMasterData)AssetDatabase.LoadAssetAtPath(path, typeof(EventMasterData));
						string eventName = MapEditorUtil.eventDefaultName;
						if (asset != null){
							eventName = asset.eventName;
						}
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField("Event Name");
							EditorGUILayout.LabelField(eventName);
						}
						EditorGUILayout.EndHorizontal();

					} else {
						EditorGUILayout.HelpBox("To edit event, set non-zero value in Event ID.", MessageType.Info);
					}
				}
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show the button that shows EventMappingData.
		/// </Summary>
		void ShowEventMappingButton(){
			EditorGUILayout.BeginVertical();
			{
				if (GUILayout.Button("Check Event Mapping")){
					if (eventMappingEditor == null){
						eventMappingEditor = CreateInstance<EventMappingEditor>();
						eventMappingEditor.GetEventRef(saveFileName);
					}
					eventMappingEditor.Show();
				}
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show toolbar to set map attribute.
		/// </Summary>
		void ShowToolBarParts(){
			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginVertical();
				{
					EditorGUILayout.LabelField("Drawing tool : " + drawToolNameList[selectedDrawTool]);
					selectedDrawTool = GUILayout.Toolbar(selectedDrawTool, drawToolIconList, GUI.skin.button, GUI.ToolbarButtonSize.FitToContents);
					if (selectedDrawTool != 0){
						isPosSelected = false;
					}
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();
				EditorGUILayout.BeginVertical();
				{
					EditorGUILayout.LabelField("Map attribute : " + mapAttrNameList[selectedMapIcon]);
					selectedMapIcon = GUILayout.Toolbar(selectedMapIcon, mapIconList, GUI.skin.button, GUI.ToolbarButtonSize.FitToContents);
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show vertical scale of the map.
		/// </Summary>
		void ShowVerticalScaleParts(){
			EditorGUILayout.BeginVertical(GUILayout.Width(cellSize - GUI.skin.label.margin.right));
			{
				EditorGUILayout.LabelField("", GUILayout.Width(GUI.skin.label.margin.vertical));
				for (int i = floorSizeVertical; i > 0; i--){
					EditorGUILayout.LabelField((i - 1).ToString(), GUILayout.Width(cellSize - GUI.skin.label.margin.right), GUILayout.Height(cellSize - GUI.skin.label.margin.top));
				}
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show the map GUI to edit the map on the floor.
		/// </Summary>
		void ShowMapParts(){
			EditorGUILayout.BeginVertical();
			{
				GUIContent content = new GUIContent(mapBg);
				GUILayout.Box(content);
				Rect mapRect = GUILayoutUtility.GetLastRect();

				// Draw map icons.
				Rect[,] iconRect = MapEditorUtil.GetMapRectArray(mapRect, floorSizeHorizontal, floorSizeVertical, cellSize);
				SetMapAttrIcon(iconRect);

				// Draw entrance position icon.
				Rect[,] entranceRect = MapEditorUtil.GetMapRectArray(mapRect, floorSizeHorizontal, floorSizeVertical, cellSize);
				SetEntranceIconToMap(entranceRect);

				// Draw event icons.
				Rect[,] eventRect = MapEditorUtil.GetMapRectArray(mapRect, floorSizeHorizontal, floorSizeVertical, cellSize);
				SetEventIconToMap(eventRect);

				Rect[,] highlightRect = MapEditorUtil.GetMapRectArray(mapRect, floorSizeHorizontal, floorSizeVertical, cellSize);
				if (isPosSelected){
					SetHoverColor(highlightRect[selectedGridPos.x, selectedGridPos.y], selectedTex);
				}

				wantsMouseMove = true;
				if (mapRect.Contains(Event.current.mousePosition)){
					Vector2 pos = new Vector2(Event.current.mousePosition.x - mapRect.position.x, Event.current.mousePosition.y - mapRect.position.y);
					hoverGridPos = MapEditorUtil.GetPosInGrid(pos, floorSizeHorizontal, floorSizeVertical, cellSize);
					if (isDragging){
						foreach (Vector2Int listPos in dragPosList){
							SetHoverColor(highlightRect[listPos.x, listPos.y], hoverTex);
						}
					} else {
						SetHoverColor(highlightRect[hoverGridPos.x, hoverGridPos.y], hoverTex);
					}
					MouseEventProcess(mapRect, highlightRect, pos);
					Repaint();
				}
				ShowHorizontalScaleParts();
			}
			EditorGUILayout.EndVertical();
		}

		/// <Summary>
		/// Show horizontal scale of the map.
		/// </Summary>
		void ShowHorizontalScaleParts(){
			EditorGUILayout.BeginHorizontal(GUILayout.Height(cellSize));
			{
				EditorGUILayout.LabelField("", GUILayout.Width(GUI.skin.label.margin.horizontal));
				for (int i = 0; i < floorSizeHorizontal; i++){
					EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(cellSize - GUI.skin.label.margin.right));
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		/// <Summary>
		/// Initialize the selection of position on the map.
		/// </Summary>
		void InitializePosSelection(){
			isPosSelected = false;
			selectedGridPos = Vector2Int.zero;
			hoverGridPos = Vector2Int.zero;
		}

		/// <Summary>
		/// Initialize properties of MapEditor.
		/// </Summary>
		void InitializeProperties(){
			floorSizeHorizontal = FloorMapConst.INIT_HORIONTAL_SIZE;
			floorSizeVertical = FloorMapConst.INIT_VERTICAL_SIZE;
			sliderHorizontal = FloorMapConst.INIT_HORIONTAL_SIZE;
			sliderVertical = FloorMapConst.INIT_VERTICAL_SIZE;

			floorName = DEFAULT_FLOOR_NAME;
			floorId = DEFAULT_FLOOR_ID;
			saveFileName = DEFAULT_SAVE_FILE_NAME;

			loadFloor = null;

			selectedPosEventId = 0;

			entrancePos = Vector2Int.zero;
			enteringDir = DungeonDir.North;

			leftPaneScrollPos = Vector2.zero;
			rightPaneScrollPos = Vector2.zero;
		}

		/// <Summary>
		/// Set map background texture by using floor sizes.
		/// </Summary>
		void SetMapBackgroundTexture(){
			mapBg = MapEditorUtil.RepaintMapTexture(floorSizeHorizontal, floorSizeVertical, cellSize, gridColor, baseColor);
		}

		/// <Summary>
		/// Set color arrays to show the map for editing.
		/// </Summary>
		void SetColorArrays(){
			hoverColors = new Color[cellSize * cellSize];
			transparentColors = new Color[cellSize * cellSize];
			selectedColors = new Color[cellSize * cellSize];
			for (int i = 0; i < hoverColors.Length; i++){
				hoverColors[i] = hoverColor;
				transparentColors[i] = transparentColor;
				selectedColors[i] = selectedColor;
			}
		}

		/// <Summary>
		/// Set tool icons for toolbars.
		/// </Summary>
		void SetToolIcons(){
			mapIconList = MapEditorUtil.GetMapIconArray();
			drawToolIconList = MapEditorUtil.GetToolIconArray();
		}

		/// <Summary>
		/// Set event icon which indicates that some event exists on the position.
		/// </Summary>
		void SetEventIcons(){
			eventIcon = MapEditorUtil.GetEventIcon();
			entranceIcon = MapEditorUtil.GetEntranceIcon();
		}

		/// <Summary>
		/// Set hover color to the texture.
		/// </Summary>
		/// <param name="r">Rect for the texture.</param>
		/// <param name="tex">Texture to draw.</param>
		void SetHoverColor(Rect r, Texture2D tex){
			if (tex != null){
				GUI.DrawTexture(r, tex);
			}
		}

		/// <Summary>
		/// Set map icons on the map.
		/// </Summary>
		/// <param name="r">Rect array for the texture.</param>
		void SetMapAttrIcon(Rect[,] r){
			for (int y = 0; y < floorSizeVertical; y++){
				for (int x = 0; x < floorSizeHorizontal; x++){
					int index = x + y * floorSizeHorizontal;
					MapInfo info = editMapInfo[index];
					if (info.mapAttr != MapAttributeDefine.HALL_WAY){
						GUI.DrawTexture(r[x, y], mapIconList[info.mapAttr]);
					}
				}
			}
		}

		/// <Summary>
		/// Set an entrance icon on the map.
		/// </Summary>
		/// <param name="r">Rect array for the texture.</param>
		void SetEntranceIconToMap(Rect[,] r){
			for (int y = 0; y < floorSizeVertical; y++){
				for (int x = 0; x < floorSizeHorizontal; x++){
					if (x == entrancePos.x && y == entrancePos.y){
						GUI.DrawTexture(r[x, y], entranceIcon);
					}
				}
			}
		}

		/// <Summary>
		/// Set event icons on the map.
		/// </Summary>
		/// <param name="r">Rect array for the texture.</param>
		void SetEventIconToMap(Rect[,] r){
			for (int y = 0; y < floorSizeVertical; y++){
				for (int x = 0; x < floorSizeHorizontal; x++){
					int index = x + y * floorSizeHorizontal;
					MapInfo info = editMapInfo[index];
					if (info.eventId > 0){
						GUI.DrawTexture(r[x, y], eventIcon);
					}
				}
			}
		}

		/// <Summary>
		/// Set the color array to the texture of a hover color.
		/// </Summary>
		void SetHoverTexture(){
			hoverTex = new Texture2D(cellSize, cellSize, TextureFormat.ARGB32, false);
			hoverTex.SetPixels(0, 0, cellSize, cellSize, hoverColors);
			hoverTex.Apply();
		}

		/// <Summary>
		/// Set the color array to the texture of a selected color.
		/// </Summary>
		void SetSelectedTexture(){
			selectedTex = new Texture2D(cellSize, cellSize, TextureFormat.ARGB32, false);
			selectedTex.SetPixels(0, 0, cellSize, cellSize, selectedColors);
			selectedTex.Apply();
		}

		/// <Summary>
		/// Set default dungeon parts.
		/// </Summary>
		void SetDefaultDungeonParts(){
			dungeonParts = DungeonPartsManager.GetDefaultDungeonParts();
		}

		/// <Summary>
		/// Process of mouse events.
		/// </Summary>
		/// <param name="mapRect">Rect for mouse event listener.</param>
		/// <param name="highlightRect">Highlighted rects.</param>
		/// <param name="pos">Position on the map.</param>
		void MouseEventProcess(Rect mapRect, Rect[,] highlightRect, Vector2 pos){
			switch (selectedDrawTool){
				case TOOL_SELECT:
					if (Event.current.type == EventType.MouseUp){
						selectedGridPos = MapEditorUtil.GetPosInGrid(pos, floorSizeHorizontal, floorSizeVertical, cellSize);
						isPosSelected = true;
					}
					break;
				case TOOL_DRAW:
					if (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDrag){
						hoverGridPos = MapEditorUtil.GetPosInGrid(pos, floorSizeHorizontal, floorSizeVertical, cellSize);
						SetMapAttribute(hoverGridPos);
					}
					break;
				case TOOL_DRAW_RECT:
					DrawRect(highlightRect, pos, TOOL_DRAW_RECT);
					break;
				case TOOL_DRAW_RECT_FILL:
					DrawRect(highlightRect, pos, TOOL_DRAW_RECT_FILL);
					break;
			}
		}

		/// <Summary>
		/// Set a map attribute ID to the selected position.
		/// </Summary>
		/// <param name="gridPos">Position on the map grids.</param>
		void SetMapAttribute(Vector2Int gridPos){
			int index = gridPos.x + gridPos.y * floorSizeHorizontal;
			MapInfo info = editMapInfo[index];
			info.mapAttr = selectedMapIcon;
			Undo.RecordObject(this, undoName);
			editMapInfo[index] = info;
		}

		/// <Summary>
		/// Draw rects by mouse event.
		/// </Summary>
		/// <param name="highlightRect">Highlighted rects.</param>
		/// <param name="pos">Position on the map.</param>
		/// <param name="toolId">Selected tool ID.</param>
		void DrawRect(Rect[,] highlightRect, Vector2 pos, int toolId){
			if (Event.current.type == EventType.MouseDown){
				dragStartGridPos = MapEditorUtil.GetPosInGrid(pos, floorSizeHorizontal, floorSizeVertical, cellSize);
				
			} else if (Event.current.type == EventType.MouseDrag){
				isDragging = true;
				dragPosList = new List<Vector2Int>();
				Vector2Int mouseGridPos = MapEditorUtil.GetPosInGrid(pos, floorSizeHorizontal, floorSizeVertical, cellSize);
				dragPosList = GetHighlightList(mouseGridPos, toolId);

			} else if (Event.current.type == EventType.MouseUp) {
				dragPosList = new List<Vector2Int>();
				Vector2Int mouseGridPos = MapEditorUtil.GetPosInGrid(pos, floorSizeHorizontal, floorSizeVertical, cellSize);
				dragPosList = GetHighlightList(mouseGridPos, toolId);
				foreach (Vector2Int listPos in dragPosList)
				{
					SetMapAttribute(listPos);
				}
				isDragging = false;
			}
		}

		/// <Summary>
		/// Initialize dragging events of mouse.
		/// </Summary>
		void InitializeDraggingEvent(){
			isDragging = false;
			dragPosList = new List<Vector2Int>();
			dragStartGridPos = Vector2Int.zero;
		}

		/// <Summary>
		/// Draw rects by mouse event.
		/// </Summary>
		/// <param name="mouseGridPos">Grid position of the mouse.</param>
		/// <param name="toolId">Selected tool ID.</param>
		List<Vector2Int> GetHighlightList(Vector2Int mouseGridPos, int toolId){
			List<Vector2Int> list = new List<Vector2Int>();
			Vector2Int min = Vector2Int.zero;
			Vector2Int max = Vector2Int.zero;
			if (dragStartGridPos.x <= mouseGridPos.x){
				min.x = dragStartGridPos.x;
				max.x = mouseGridPos.x;
			} else {
				min.x = mouseGridPos.x;
				max.x = dragStartGridPos.x;
			}

			if (dragStartGridPos.y <= mouseGridPos.y){
				min.y = dragStartGridPos.y;
				max.y = mouseGridPos.y;
			} else {
				min.y = mouseGridPos.y;
				max.y = dragStartGridPos.y;
			}

			for (int y = min.y; y <= max.y; y++){
				for (int x = min.x; x <= max.x; x++){
					if (toolId == TOOL_DRAW_RECT){
						if (x == min.x || x == max.x || y == min.y || y == max.y){
							list.Add(new Vector2Int(x, y));
						}
					} else if (toolId == TOOL_DRAW_RECT_FILL){
						list.Add(new Vector2Int(x, y));
					}
					
				}
			}

			return list;
		}

		/// <Summary>
		/// Set new map data for editing.
		/// </Summary>
		void SetNewMapData(){
			editMapInfo = new List<MapInfo>();
			// Set edit data
			int listSize = floorSizeHorizontal * floorSizeVertical;
			for (int i = 0; i < listSize; i++){
				MapInfo info = new MapInfo();
				info.eventId = 0;
				info.mapAttr = 0;
				editMapInfo.Add(info);
			}
		}

		/// <Summary>
		/// Set map settings to new map to avoid lack of information by changing index.
		/// </Summary>
		/// <param name="oldHorizontalSize">Pre size of horizontal floor size.</param>
		void ConvertMapHorizontalChanged(int oldHorizontalSize){
			List<MapInfo> oldMapInfo = editMapInfo;
			MapInfo[,] oldArray = GetMapDataArrayFromList(oldMapInfo, oldHorizontalSize, floorSizeVertical);
			
			SetNewMapData();

			MapInfo[,] newArray = GetMapDataArrayFromList(editMapInfo, floorSizeHorizontal, floorSizeVertical);

			// Get lesser horizontal size.
			int max = oldHorizontalSize < floorSizeHorizontal ? oldHorizontalSize : floorSizeHorizontal;

			for (int y = 0; y < floorSizeVertical; y++){
				for (int x = 0; x < max; x++){
					newArray[x, y] = oldArray[x, y];
				}
			}
			Undo.RecordObject(this, undoName);
			editMapInfo = GetMapDataListFromArray(newArray, floorSizeHorizontal, floorSizeVertical);
		}

		/// <Summary>
		/// Set map settings to new map to avoid lack of information by changing index.
		/// </Summary>
		/// <param name="oldVerticalSize">Pre size of vertical floor size.</param>
		void ConvertMapVerticalChanged(int oldVerticalSize){
			List<MapInfo> oldMapInfo = editMapInfo;
			MapInfo[,] oldArray = GetMapDataArrayFromList(oldMapInfo, floorSizeHorizontal, oldVerticalSize);
			
			SetNewMapData();

			MapInfo[,] newArray = GetMapDataArrayFromList(editMapInfo, floorSizeHorizontal, floorSizeVertical);

			// Get lesser vertical size.
			int max = oldVerticalSize < floorSizeVertical ? oldVerticalSize : floorSizeVertical;

			for (int y = 0; y < max; y++){
				for (int x = 0; x < floorSizeHorizontal; x++){
					newArray[x, y] = oldArray[x, y];
				}
			}
			Undo.RecordObject(this, undoName);
			editMapInfo = GetMapDataListFromArray(newArray, floorSizeHorizontal, floorSizeVertical);
		}

		/// <Summary>
		/// Convert the list to array for manipulating map data by index. 
		/// </Summary>
		/// <param name="mapInfoList">List of MapInfo data.</param>
		/// <param name="horizontalSize">Horizontal size of the floor.</param>
		/// <param name="verticalSize">Vertical size of the floor.</param>
		MapInfo[,] GetMapDataArrayFromList(List<MapInfo> mapInfoList, int horizontalSize, int verticalSize){
			MapInfo[,] mapArray = new MapInfo[horizontalSize, verticalSize];
			for (int y = 0; y < verticalSize; y++){
				for (int x = 0; x < horizontalSize; x++){
					mapArray[x, y] = mapInfoList[x + y * horizontalSize];
				}
			}
			return mapArray;
		}

		/// <Summary>
		/// Convert the array to list for holding map data. 
		/// </Summary>
		/// <param name="mapInfoArray">Array of MapInfo data.</param>
		/// <param name="horizontalSize">Horizontal size of the floor.</param>
		/// <param name="verticalSize">Vertical size of the floor.</param>
		List<MapInfo> GetMapDataListFromArray(MapInfo[,] mapInfoArray, int horizontalSize, int verticalSize){
			List<MapInfo> mapList = new List<MapInfo>();
			for (int y = 0; y < verticalSize; y++){
				for (int x = 0; x < horizontalSize; x++){
					mapList.Add(mapInfoArray[x, y]);
				}
			}
			return mapList;
		}

		/// <Summary>
		/// Validate the entrance position.
		/// </Summary>
		void ValidateEntrancePos(){
			// Horizontal axis validation.
			if (entrancePos.x < 0){
				entrancePos.x = 0;
			} else if (entrancePos.x >= floorSizeHorizontal){
				entrancePos.x = floorSizeHorizontal - 1;
			}

			// Vertical axis validation.
			if (entrancePos.y < 0){
				entrancePos.y = 0;
			} else if (entrancePos.y >= floorSizeVertical){
				entrancePos.y = floorSizeVertical - 1;
			}
		}

		/// <Summary>
		/// Register action for the callback of Undo/Redo.
		/// </Summary>
		void SetCallbackForUndo(){
			// To avoid additional registeration.
			Undo.undoRedoPerformed -= UndoRedoCallbackAction;
			Undo.undoRedoPerformed += UndoRedoCallbackAction;
		}

		/// <Summary>
		/// Define callback action of Undo/Redo.
		/// </Summary>
		void UndoRedoCallbackAction(){
			InitializePosSelection();
			SetMapBackgroundTexture();
			Repaint();
		}

		/// <Summary>
		/// Clear Undo/Redo stack to prevent out of index error of MapInfo array in another file.
		/// </Summary>
		void ClearUndoStack(){
			Undo.FlushUndoRecordObjects();
			Undo.ClearAll();
		}

		/// <Summary>
		/// Initialize the font style.
		/// </Summary>
		void InitializeStyles(){
			boldLabelStyle = new GUIStyle(GUI.skin.label);
			boldLabelStyle.fontStyle = FontStyle.Bold;
		}

		/// <Summary>
		/// Set initial strings for file path.
		/// </Summary>
		void SetInitialFilePath(){
			tempFileName = MapEditorUtil.tempFileName;
			tempFilePathPrefix = MapEditorUtil.tempFilePathPrefix;
			tempFilePath = MapEditorUtil.tempFilePath;
			infoFileName = MapEditorUtil.infoFileName;
			saveFolderPrefix = MapEditorUtil.saveFolderPrefix;
			saveFolderPath = MapEditorUtil.saveFolderPath;
			saveFilePostFix = MapEditorUtil.saveFilePostFix;
		}

		/// <Summary>
		/// Load process of the temp file.
		/// </Summary>
		void LoadTempFile(){
			string path = tempFilePathPrefix + tempFilePath + tempFileName + ".asset";
			var tempData = CreateInstance<FloorMapMasterData>();
			
			var asset = (FloorMapMasterData)AssetDatabase.LoadAssetAtPath(path, typeof(FloorMapMasterData));
			if (asset == null){
				SetNewMapData();
				tempData.mapInfo = editMapInfo;
				if (this.dungeonParts == null){
					SetDefaultDungeonParts();
				}
			} else {
				tempData = AssetDatabase.LoadAssetAtPath<FloorMapMasterData>(path);
				if (tempData != null){
					this.editMapInfo = tempData.mapInfo;
					this.floorSizeHorizontal = tempData.floorSizeHorizontal;
					this.floorSizeVertical = tempData.floorSizeVertical;
					sliderHorizontal = this.floorSizeHorizontal;
					sliderVertical = this.floorSizeVertical;
					this.floorName = tempData.floorName;
					this.floorId = tempData.floorId;
					this.entrancePos = tempData.entrancePos;
					this.enteringDir = tempData.enteringDir;
					this.dungeonParts = tempData.dungeonParts;
					if (this.dungeonParts == null){
						SetDefaultDungeonParts();
					}
				}
			}

			LoadTempFileInfo();
		}

		/// <Summary>
		/// Set file name and path of editing file.
		/// </Summary>
		void LoadTempFileInfo(){
			string infoPath = tempFilePathPrefix + tempFilePath + infoFileName + ".asset";
			var infoAsset = (EditFileInfo)AssetDatabase.LoadAssetAtPath(infoPath, typeof(EditFileInfo));
			if (infoAsset != null){
				this.saveFileName = infoAsset.editFileName;
				this.saveFolderPath = infoAsset.editFilePath;
			}
		}

		/// <Summary>
		/// Save process of the temp file.
		/// </Summary>
		void SaveTempFile(){
			var tempData = CreateInstance<FloorMapMasterData>();
			tempData.mapInfo = this.editMapInfo;
			tempData.floorSizeHorizontal = this.floorSizeHorizontal;
			tempData.floorSizeVertical = this.floorSizeVertical;
			tempData.floorName = this.floorName;
			tempData.floorId = this.floorId;
			tempData.entrancePos = this.entrancePos;
			tempData.enteringDir = this.enteringDir;
			tempData.dungeonParts = this.dungeonParts;
			
			string path = tempFilePathPrefix + tempFilePath + tempFileName + ".asset";
			AssetDatabase.CreateAsset(tempData, path);
			AssetDatabase.Refresh();

			// save editing file name.
			var tempFileInfo = CreateInstance<EditFileInfo>();
			tempFileInfo.editFileName = saveFileName;
			tempFileInfo.editFilePath = saveFolderPath;
			string infoPath = tempFilePathPrefix + tempFilePath + infoFileName + ".asset";
			AssetDatabase.CreateAsset(tempFileInfo, infoPath);
			AssetDatabase.Refresh();

		}

		/// <Summary>
		/// Load process of the FloorMapMasterData file.
		/// </Summary>
		void LoadMapFile(){
			var loadData = loadFloor;

			this.floorSizeHorizontal = loadData.floorSizeHorizontal;
			this.floorSizeVertical = loadData.floorSizeVertical;
			sliderHorizontal = this.floorSizeHorizontal;
			sliderVertical = this.floorSizeVertical;
			this.floorName = loadData.floorName;
			this.floorId = loadData.floorId;
			this.entrancePos = loadData.entrancePos;
			this.enteringDir = loadData.enteringDir;
			this.dungeonParts = loadData.dungeonParts;
			if (this.dungeonParts == null){
				
				SetDefaultDungeonParts();
			}

			this.saveFileName = loadData.name;
			string loadFilePath = AssetDatabase.GetAssetPath(loadData);

			int prefixIndex = loadFilePath.IndexOf(saveFolderPrefix);
			string cutPrefix = loadFilePath.Substring(prefixIndex + saveFolderPrefix.Length);

			int fileNameIndex = cutPrefix.IndexOf(saveFileName);
			string path = cutPrefix.Substring(0, fileNameIndex);
			saveFolderPath = path;
			SetEditMapData(loadData);

		}

		/// <Summary>
		/// Set MapInfo data from loaded FloorMapMasterData file.
		/// </Summary>
		void SetEditMapData(FloorMapMasterData mapDataFile){
			editMapInfo = new List<MapInfo>();
			// Set edit data
			int listSize = floorSizeHorizontal * floorSizeVertical;
			for (int i = 0; i < listSize; i++){
				MapInfo info = new MapInfo();
				info.eventId = mapDataFile.mapInfo[i].eventId;
				info.mapAttr = mapDataFile.mapInfo[i].mapAttr;
				info.objectFront = mapDataFile.mapInfo[i].objectFront;
				info.messengerType = mapDataFile.mapInfo[i].messengerType;
				editMapInfo.Add(info);
			}
		}

		/// <Summary>
		/// Save process of the FloorMapMasterData file.
		/// </Summary>
		void SaveMapFile(){
			var mapData = CreateInstance<FloorMapMasterData>();
			mapData.mapInfo = this.editMapInfo;
			mapData.floorSizeHorizontal = this.floorSizeHorizontal;
			mapData.floorSizeVertical = this.floorSizeVertical;
			mapData.floorName = this.floorName;
			mapData.floorId = this.floorId;
			mapData.entrancePos = this.entrancePos;
			mapData.enteringDir = this.enteringDir;
			mapData.dungeonParts = this.dungeonParts;
			SetEditMapData(mapData);
			
			string path = saveFolderPrefix + saveFolderPath + saveFileName + saveFilePostFix;
			var asset = (FloorMapMasterData)AssetDatabase.LoadAssetAtPath(path, typeof(FloorMapMasterData));
			if (asset == null){
				AssetDatabase.CreateAsset(mapData, path);
			} else {
				EditorUtility.CopySerialized(mapData, asset);
				AssetDatabase.SaveAssets();
			}
			AssetDatabase.Refresh();
		}

		/// <Summary>
		/// Returns messenger prefab name array to show the selector.
		/// </Summary>
		/// <param name="messengerList">List of messengers assigned to the dungeon parts data.</param>
		string[] GetMessengerNameArray(List<GameObject> messengerList){
			if (messengerList.Count == 0){
				return null;
			}
			string[] messengerNameArray = new string[messengerList.Count];
			for (int i = 0; i < messengerList.Count; i++){
				if (messengerList[i] != null){
					messengerNameArray[i] = messengerList[i].name;
				}
			}
			return messengerNameArray;
		}

		/// <Summary>
		/// Returns messenger prefab index array to show the selector.
		/// </Summary>
		/// <param name="messengerList">List of messengers assigned to the dungeon parts data.</param>
		int[] GetMessengerIdArray(List<GameObject> messengerList){
			if (messengerList.Count == 0){
				return null;
			}
			int[] messengerIdArray = new int[messengerList.Count];
			for (int i = 0; i < messengerList.Count; i++){
				messengerIdArray[i] = i;
			}
			return messengerIdArray;
		}
	}

}
