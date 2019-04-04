using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ariadne {

	/// <Summary>
	/// Controller class of player movement.
	/// </Summary>
	public class MoveController : MonoBehaviour, IEnterDungeon, IEventProcessor {

		FloorMapMasterData floorMapData;
		GameObject gameController;
		DungeonMasterData dungeonData;
		DungeonPartsMasterData dungeonParts;
		GameObject player;
		bool isMoveOk = false;
		public float moveWait = 0.5f;

		// Button ID
		const int TURN_LEFT = 0;
		const int TURN_RIGHT = 1;
		const int TURN_BACK = 2;
		const int MOVE_FRONT = 3;

		bool isPressedTurnLeft;
		bool isPressedTurnRight;
		bool isPressedTurnBack;
		bool isPressedMoveFront;

		[SerializeField]
		bool isUGUIButtonUsed = true;
		[SerializeField]
		GameObject okButtonParent;
		[SerializeField]
		GameObject arrowButtonParent;

		Vector3 unitSize;

		GameObject[] mapParts;
		bool isExecutingEvent = false;
		bool isEventReady = false;
		FadeManager fadeManager;
		EventProcessor eventProcessor;
		bool isInDungeon = false;
		DungeonMasterData moveDestDungeon;


		void Start () {
			InitializeDungeonState();
		}

		/// <Summary>
		/// Initialize state of dungeon.
		/// </Summary>
		void InitializeDungeonState(){

			// Get settings from dungeon manager.
			GetSettings();
			
			// Get references of UI components.
			GetRef();

			// Set unit size of dungeon.
			SetUnitSize();

			// Set player init position.
			SetInitPos();

			// Set traverse data of init position.
			SetTraverse();

			// Set reference of map objects list.
			SetMapObjList();

			// Set state of UGUI buttons.
			SetUGUIButtons();

			// Fade in map.
			FadeInMap();

			// Refrect traversed data of init position to map.
			SendSetDirtyMsg();

			// Set event dict.
			FlagManager.InitializeEventFlagList();

			// Set holding item dict.
			ItemManager.InitializeHoldItemDict();

			// Fade in.
			StartCoroutine(DelayFadeIn());
		}

        void GetSettings(){
			gameController = GameObject.FindGameObjectWithTag("GameController");
			DungeonSettings ds = gameController.GetComponent<DungeonSettings>();
			dungeonData = ds.dungeonData;
			floorMapData = ds.GetCurrentFloorData();
			dungeonParts = floorMapData.dungeonParts;
			if (dungeonParts == null){
				DungeonPartsManager.GetDefaultDungeonParts();
			}

			player = GameObject.FindGameObjectWithTag("Player");
		}

		void GetRef(){
			fadeManager = gameController.GetComponent<FadeManager>();
			eventProcessor = gameController.GetComponent<EventProcessor>();
		}

		void SetUnitSize(){
			GameObject groundPrefab = dungeonParts.groundObj;
			unitSize = new Vector3(groundPrefab.transform.localScale.x, groundPrefab.transform.localScale.y, groundPrefab.transform.localScale.z);
		}

		/// <Summary>
		/// Set the initial position of the player.
		/// </Summary>
		void SetInitPos(){
			Vector2Int initPos = Vector2Int.zero;
			DungeonDir initDir = DungeonDir.North;

			if (floorMapData != null){
				initPos = floorMapData.entrancePos;
				initDir = floorMapData.enteringDir;
			}
			PlayerPosition.playerPos = initPos;
			PlayerPosition.direction = initDir;
			PlayerPosition.currentFloorId = floorMapData.floorId;
			PlayerPosition.currentDungeonId = dungeonData.dungeonId;

			float targetAngle = CurrentDirAngle();
			player.transform.eulerAngles = new Vector3(0, targetAngle, 0);
			SetCameraPos();
			
		}

		/// <Summary>
		/// Set traverse data of the floor.
		/// </Summary>
		void SetTraverse(){
			TraverseManager.InitializeTraverseData();
			TraverseManager.AddDungeonTraverseData(dungeonData.dungeonId, floorMapData.floorId, floorMapData);
			PlayerPosition.SetTraverseData();
		}

		/// <Summary>
		/// Set the position of the player camera.
		/// </Summary>
		void SetCameraPos(){
			Vector3 currentPos = Vector3.zero;
			currentPos.x += PlayerPosition.playerPos.x * unitSize.x;
			currentPos.y = player.transform.position.y;
			currentPos.z += PlayerPosition.playerPos.y * unitSize.z;
			Vector3 targetPos = currentPos;
			player.transform.position = targetPos;
		}

		/// <Summary>
		/// Set the list of map objects to send SetDirty message.
		/// </Summary>
		void SetMapObjList(){
			mapParts = GameObject.FindGameObjectsWithTag("MapParts");
		}

		/// <Summary>
		/// Set UGUI buttons.
		/// </Summary>
		void SetUGUIButtons(){
			if (isUGUIButtonUsed){
				okButtonParent.SetActive(true);
				arrowButtonParent.SetActive(true);
			} else {
				okButtonParent.SetActive(false);
				arrowButtonParent.SetActive(false);
			}
		}

		/// <Summary>
		/// Set the state of the parent of UGUI arrow buttons.
		/// </Summary>
		void SetArrowButtonState(bool isActive){
			if (isUGUIButtonUsed){
				arrowButtonParent.SetActive(isActive);
			}
		}

		/// <Summary>
		/// Call fade in process of FadeManager.
		/// </Summary>
		void FadeInMap(){
			isInDungeon = true;
			fadeManager.MapFadeIn();
		}
		
		void Update (){
			if (!isInDungeon){
				return;
			}
			if (isMoveOk){
				PlayerMove();
			}
		}

		/// <Summary>
		/// Check key inputs about movement.
		/// </Summary>
		void PlayerMove(){
			if (Input.GetKey(KeyCode.UpArrow) || isPressedMoveFront){
				MoveFrontProcess();
            }

			if (Input.GetKey(KeyCode.LeftArrow) || isPressedTurnLeft){
				TurnProcess(TURN_LEFT);
			}

			if (Input.GetKey(KeyCode.RightArrow) || isPressedTurnRight){
				TurnProcess(TURN_RIGHT);
			}

			if (Input.GetKey(KeyCode.DownArrow) || isPressedTurnBack){
				TurnProcess(TURN_BACK);
			}

			if (isEventReady){
				if (Input.GetKeyUp(KeyCode.Space)){
					isMoveOk = false;
					OnEventKeyPressed();
				}
			}
		}

		/// <Summary>
		/// Action of ok button.
		/// This method is used in mobile devices.
		/// </Summary>
		public void OnPressedOkButton(){
			if (!isInDungeon){
				return;
			}
			if (!isMoveOk){
				return;
			}
			if (isEventReady){
				isMoveOk = false;
				OnEventKeyPressed();
			}
		}

		/// <Summary>
		/// Action on button pressed.
		/// This method is used for UGUI button.
		/// </Summary>
		public void OnPressedButton(int buttonId){
			switch (buttonId){
				case TURN_LEFT:
					isPressedTurnLeft = true;
					break;
				case TURN_RIGHT:
					isPressedTurnRight = true;
					break;
				case TURN_BACK:
					isPressedTurnBack = true;
					break;
				case MOVE_FRONT:
					isPressedMoveFront = true;
					break;
			}
		}

		/// <Summary>
		/// Action on button released.
		/// This method is used for UGUI button.
		/// </Summary>
		public void OnReleasedButton(int buttonId){
			switch (buttonId){
				case TURN_LEFT:
					isPressedTurnLeft = false;
					break;
				case TURN_RIGHT:
					isPressedTurnRight = false;
					break;
				case TURN_BACK:
					isPressedTurnBack = false;
					break;
				case MOVE_FRONT:
					isPressedMoveFront = false;
					break;
			}
		}

		/// <Summary>
		/// Initialize states of uGUI buttuns.
		/// </Summary>
		void InitializeButtonPressedState(){
			isPressedTurnLeft = false;
			isPressedTurnRight = false;
			isPressedTurnBack = false;
			isPressedMoveFront = false;
		}

		/// <Summary>
		/// Move forward process.
		/// </Summary>
		public void MoveFrontProcess(){
			PreMove();

			// Check forward wall in current position
			Vector2Int currentPos = PlayerPosition.playerPos;
			int mapAttrId = PlayerPosition.GetMapInfoByDirection(floorMapData, currentPos, PlayerPosition.direction);
			if (!CheckCanMove(mapAttrId)){
				isMoveOk = true;
				return;
			}

			Vector2Int ps = PlayerPosition.GetForwardPosition(1);
			if (CheckPositionIsValid(ps)){
				StartCoroutine(MoveForward(1));
				PlayerPosition.SetTraverseData();
				SendSetDirtyMsg();
			}

        }

		/// <Summary>
		/// Turn process of the player.
		/// </Summary>
		/// <param name="turnDirection">Direction of turning.</param>
		public void TurnProcess(int turnDirection){
			PreMove();
			StartCoroutine(TurnCamera(turnDirection));
		}

		/// <Summary>
		/// Returns the event data of target position.
		/// </Summary>
		/// <param name="targetPos">Target position.</param>
		EventMasterData GetEventData(Vector2Int targetPos){
			EventMasterData eventData = null;
			if (!CheckPositionIsValid(targetPos)){
				return eventData;
			}
			int index = targetPos.x + targetPos.y * floorMapData.floorSizeHorizontal;
			int targetPosEventId = floorMapData.mapInfo[index].eventId;
			if (targetPosEventId > 0){
				string path = "EventData/event_" + targetPosEventId.ToString("D5");
				eventData = (EventMasterData)Resources.Load(path);
			}
			return eventData;
		}

		/// <Summary>
		/// Returns the flag that the event is executed.
		/// This method is called after the player has moved.
		/// </Summary>
		/// <param name="eventData">Event data of the position.</param>
		/// <param name="targetPos">Target position.</param>
		/// <param name="eventStartPos">Starting position of the event defined in the event parts.</param>
		bool ExecuteEventOnPostMove(EventMasterData eventData, Vector2Int targetPos, AriadneEventPosition eventStartPos){
			bool isEventExecute = false;
			if (eventData == null){
				return isEventExecute;
			}

			AriadneEventParts parts = eventProcessor.GetExecutionEventParts(eventData);
			if (parts == null){
				return isEventExecute;
			}

			if (parts.eventPos != eventStartPos){
				return isEventExecute;
			}

			if (parts.eventTrigger == AriadneEventTrigger.Auto){
				// Execute event immediately.
				isEventExecute = true;
				SetEventTraverse(targetPos);
				this.isExecutingEvent = true;
				InitializeButtonPressedState();
				SetArrowButtonState(false);
				eventProcessor.EventExecuter(parts, targetPos);
			} else {
				// Set key wait flag.
				this.isEventReady = true;
				fadeManager.FadeInKeyWait();
			}
			return isEventExecute;
		}

		/// <Summary>
		/// Returns the flag that the event is executed.
		/// This method is called when the key is pressed.
		/// </Summary>
		/// <param name="eventData">Event data of the position.</param>
		/// <param name="targetPos">Target position.</param>
		/// <param name="eventStartPos">Starting position of the event defined in the event parts.</param>
		bool ExecuteEventOnKeyPressed(EventMasterData eventData, Vector2Int targetPos, AriadneEventPosition eventStartPos){
			bool isEventExecute = false;
			fadeManager.FadeOutKeyWait();
			if (eventData == null){
				return isEventExecute;
			}

			AriadneEventParts parts = eventProcessor.GetExecutionEventParts(eventData);
			if (parts == null){
				return isEventExecute;
			}

			if (parts.eventPos != eventStartPos){
				return isEventExecute;
			}

			if (parts.eventTrigger == AriadneEventTrigger.KeyPress){
				// Execute event.
				isEventExecute = true;
				SetEventTraverse(targetPos);
				this.isExecutingEvent = true;
				SetArrowButtonState(false);
				eventProcessor.EventExecuter(parts, targetPos);
			}
			return isEventExecute;
		}

		/// <Summary>
		/// Execute event when a key is pressed.
		/// </Summary>
		void OnEventKeyPressed(){
			// Check an event ID of target position.
			Vector2Int ps = PlayerPosition.playerPos;
			EventMasterData eventData = GetEventData(ps);
			bool isExecuted = ExecuteEventOnKeyPressed(eventData, ps, AriadneEventPosition.ThisPosition);

			// Check an event ID of forward position
			bool isForwardEventExecuted = false;
			if (!isExecuted){
				ps = PlayerPosition.GetForwardPosition(1);
				EventMasterData eventDataForward = GetEventData(ps);
				isForwardEventExecuted = ExecuteEventOnKeyPressed(eventDataForward, ps, AriadneEventPosition.OneStepShortOfThis);
			}

			// When any event has not been executed, set true to the move flag.
			if (!isExecuted && !isForwardEventExecuted){
				isMoveOk = true;
			}
		}

		/// <Summary>
		/// Set traverse data of the event position.
		/// </Summary>
		/// <param name="eventPos">Position of the event.</param>
		void SetEventTraverse(Vector2Int eventPos){
			// This position is validated in GetEventData method.
			TraverseManager.SetTraverseData(PlayerPosition.currentDungeonId, PlayerPosition.currentFloorId, eventPos, true);
			SendSetDirtyMsgImmediately();
		}

		/// <Summary>
		/// Pre process of the player moving.
		/// </Summary>
		void PreMove(){
			// Set move flag to false.
			isMoveOk = false;

			// Fade out key wait window.
			fadeManager.FadeOutKeyWait();
		}

		/// <Summary>
		/// Post process of the player moving.
		/// </Summary>
		void PostMove(){
			isExecutingEvent = false;
			isEventReady = false;

			// Check an event ID of target position.
			Vector2Int ps = PlayerPosition.playerPos;
			EventMasterData eventData = GetEventData(ps);
			bool isExecuted = ExecuteEventOnPostMove(eventData, ps, AriadneEventPosition.ThisPosition);

			// Check an event ID of forward position
			bool isForwardEventExecuted = false;
			if (!isExecuted){
				ps = PlayerPosition.GetForwardPosition(1);
				EventMasterData eventDataForward = GetEventData(ps);
				isForwardEventExecuted = ExecuteEventOnPostMove(eventDataForward, ps, AriadneEventPosition.OneStepShortOfThis);
			}
			
			// When no event has been executed, set true to the move flag.
			if (!isExecuted && !isForwardEventExecuted){
				isMoveOk = true;
				fadeManager.InitializeWaitFlags();
			}
		}

		/// <Summary>
		/// Initialize flags of events.
		/// </Summary>
		void InitializeEventFlags(){
			isExecutingEvent = false;
			isEventReady = false;
			isMoveOk = true;
			fadeManager.InitializeWaitFlags();
			SetArrowButtonState(true);
		}

		/// <Summary>
		/// Receiver of OnDoorOpen message from IEventProcessor.
		/// </Summary>
		public void OnDoorOpen(){
			StartCoroutine(MoveAtDoorOpen());
		}

		/// <Summary>
		/// Move process in the door opening event.
		/// </Summary>
		IEnumerator MoveAtDoorOpen(){
			// Move 2 step
			StartCoroutine(MoveForward(1));
			PlayerPosition.SetTraverseData();
			SendSetDirtyMsg();
			yield return new WaitForSeconds(moveWait);

			StartCoroutine(MoveForward(1));
			PlayerPosition.SetTraverseData();
			SendSetDirtyMsg();
			yield return new WaitForSeconds(moveWait);

			SendMoveFinishedMsg(gameController);
		}

		/// <Summary>
		/// Receiver of OnPostMove message from IEventProcessor.
		/// </Summary>
		public void OnPostMove(){
			PostMove();
		}

		/// <Summary>
		/// Receiver of OnMovePosition message from IEventProcessor.
		/// </Summary>
		/// <param name="eventParts">Event parts data.</param>
		public void OnMovePosition(AriadneEventParts eventParts){
			StartCoroutine(EventMovePosition(eventParts));
		}

		/// <Summary>
		/// Move position event process.
		/// </Summary>
		/// <param name="eventParts">Event parts data.</param>
		IEnumerator EventMovePosition(AriadneEventParts eventParts){
			// Fade out
			fadeManager.FadeOut();
			var waitTime = new WaitForSeconds(fadeManager.fadeTime);
			yield return waitTime;

			// Set new position
			if (eventParts.destDungeon == null){
				Debug.LogError("Dest dungeon is not assigned to this EventData!");
				yield break;
			}
			if (eventParts.destMap == null){
				Debug.LogError("Dest map is not assigned to this EventData!");
				yield break;
			}

			// Send dungeon data to DungeonSetting.
			moveDestDungeon = eventParts.destDungeon;
			SendDungeonData(gameController);

			PlayerPosition.currentDungeonId = eventParts.destDungeon.dungeonId;
			PlayerPosition.currentFloorId = eventParts.destMap.floorId;
			PlayerPosition.playerPos = eventParts.destPos;
			PlayerPosition.direction = eventParts.direction;

			// Get new floor data
			DungeonSettings ds = gameController.GetComponent<DungeonSettings>();
			dungeonData = ds.dungeonData;
			floorMapData = ds.GetCurrentFloorData();

			// Add traverse data
			TraverseManager.AddDungeonTraverseData(PlayerPosition.currentDungeonId, PlayerPosition.currentFloorId, floorMapData);
			PlayerPosition.SetTraverseData();
			yield return waitTime;

			// Remove dungeon walls and redraw dungeon
			SendRedrawMessage(gameController);

			// Move camera
			SetCameraPos();
			float targetAngle = CurrentDirAngle();
			player.transform.eulerAngles = new Vector3(0, targetAngle, 0);
			SendSetNewMap();
			SendSetDirtyMsg();

			yield return waitTime;

			// Fade in
			fadeManager.FadeIn();
			yield return waitTime;

			OnPostEvent();
		}

		/// <Summary>
		/// Receiver of OnExitDungeon message from IEventProcessor.
		/// </Summary>
		public void OnExitDungeon(){
			StartCoroutine(EventExit());
		}

		/// <Summary>
		/// Exiting the dungeon process.
		/// </Summary>
		IEnumerator EventExit(){
			// Fade out
			fadeManager.FadeOut();
			yield return new WaitForSeconds(fadeManager.fadeTime);

			// Send hiding map messages.
			isInDungeon = false;
			fadeManager.MapFadeOut();

			// Remove dungeon objects
			SendRemoveMessage(gameController);
			yield return new WaitForSeconds(fadeManager.fadeTime);
			
			// Exit dungeon
			SendExitDungeonMessage(gameController);

			OnPostEvent();
			isMoveOk = false;

			if (isUGUIButtonUsed){
				okButtonParent.SetActive(false);
				arrowButtonParent.SetActive(false);
			}
		}

		/// <Summary>
		/// Receiver of OnEnterDungeon message from IEnterDungeon.
		/// </Summary>
		public void OnEnterDungeon(){
			SendDrawMessage(gameController);
			SendSetNewMap();
			InitializeDungeonState();
		}

		/// <Summary>
		/// Receiver of OnPostEvent message from IEventProcessor.
		/// </Summary>
		public void OnPostEvent(){
			InitializeEventFlags();
		}

		/// <Summary>
		/// Moving forward process.
		/// </Summary>
		/// <param name="steps">Number of steps.</param>
		IEnumerator MoveForward(int steps){

			for (int i = 0; i < steps; i++){
				Vector3 currentPos = Vector3.zero;
				currentPos.x += PlayerPosition.playerPos.x * unitSize.x;
				currentPos.y = player.transform.position.y;
				currentPos.z += PlayerPosition.playerPos.y * unitSize.z;
				PlayerPosition.MoveForward();
				Vector3 targetPos = currentPos;
				switch (PlayerPosition.direction){
					case DungeonDir.North:
						targetPos = new Vector3(currentPos.x, currentPos.y, currentPos.z + unitSize.z);
						break;
					case DungeonDir.East:
						targetPos = new Vector3(currentPos.x + unitSize.x, currentPos.y, currentPos.z);
						break;
					case DungeonDir.South:
						targetPos = new Vector3(currentPos.x, currentPos.y, currentPos.z - unitSize.z);
						break;
					case DungeonDir.West:
						targetPos = new Vector3(currentPos.x - unitSize.x, currentPos.y, currentPos.z);
						break;
				}

				float finishTime = Time.time + moveWait;
				while (true){
					float diff = finishTime - Time.time;
					if (diff <= 0){
						break;
					}
					float rate = 1 - Mathf.Clamp01(diff / moveWait);
					player.transform.position = Vector3.Lerp(currentPos, targetPos, rate);
					yield return null;
				}
				player.transform.position = targetPos;
                GetComponent<EncounterManager>().IncEncounterChance();
            }
			
			if (!isExecutingEvent){
				PostMove();
			}
		}

		/// <Summary>
		/// Turn the player camera process.
		/// </Summary>
		/// <param name="turnDir">Direction to turn.</param>
		IEnumerator TurnCamera(int turnDir){
			float currentAngle = CurrentDirAngle();
			float targetAngle = 0f;
			switch (turnDir){
				case TURN_LEFT:
					PlayerPosition.TurnLeft();
					targetAngle = CurrentDirAngle();
					break;
				case TURN_RIGHT:
					PlayerPosition.TurnRight();
					targetAngle = CurrentDirAngle();
					break;
				case TURN_BACK:
					PlayerPosition.TurnBack();
					targetAngle = CurrentDirAngle();
					break;
			}

			float finishTime = Time.time + moveWait;
			while (true){
				float diff = finishTime - Time.time;
				if (diff <= 0){
					break;
				}
				float rate = 1 - Mathf.Clamp01(diff / moveWait);
				// Debug.Log("rate : " + rate);
				float angle = Mathf.LerpAngle(currentAngle, targetAngle, rate);
				player.transform.eulerAngles = new Vector3(0, angle, 0);
				yield return null;
			}
			player.transform.eulerAngles = new Vector3(0, targetAngle, 0);
			PostMove();
		}

		/// <Summary>
		/// Returns the angle that corresponds to player direction.
		/// </Summary>
		float CurrentDirAngle(){
			float angle = 0f;
			switch (PlayerPosition.direction){
				case DungeonDir.North:
					angle = 0f;
					break;
				case DungeonDir.East:
					angle = 90f;
					break;
				case DungeonDir.South:
					angle = 180f;
					break;
				case DungeonDir.West:
					angle = 270f;
					break;
			}
			return angle;
		}

		/// <Summary>
		/// Check if the position is valid.
		/// </Summary>
		/// <param name="position">Position to check.</param>
		bool CheckPositionIsValid(Vector2Int position){
			bool isValid = true;

			// Check x axis position
			if (position.x < 0 || position.x >= floorMapData.floorSizeHorizontal){
				isValid = false;
			}

			// Check y axis position
			if (position.y < 0 || position.y >= floorMapData.floorSizeVertical){
				isValid = false;
			}

			return isValid;
		}

		/// <Summary>
		/// Check if that player can move.
		/// </Summary>
		bool CheckCanMove(int mapAttrId){
			bool isPass = true;
			if (mapAttrId != MapAttributeDefine.HALL_WAY){
				isPass = false;
			}
			return isPass;
		}

		/// <Summary>
		/// Call fade in with delay time.
		/// </Summary>
		IEnumerator DelayFadeIn(){
			yield return new WaitForSeconds(moveWait);
			fadeManager.FadeIn();
			isMoveOk = true;
		}

		/// <Summary>
		/// Send SetDirty message to DrawMap components.
		/// </Summary>
		void SendSetDirtyMsg(){
			foreach (GameObject obj in mapParts){
				ExecuteEvents.Execute<IDirtyMarkerMap>(
					target: obj,
					eventData: null,
					functor: CallSetDirty
				);
			}
		}

		/// <Summary>
		/// The functor of SendSetDirtyMsg method.
		/// </Summary>
		void CallSetDirty(IDirtyMarkerMap marker, BaseEventData eventData){
			marker.OnSetDirtyLerp(moveWait);
		}

		/// <Summary>
		/// Send SetDirty message to DrawMap components.
		/// This message will update map immediately.
		/// </Summary>
		void SendSetDirtyMsgImmediately(){
			foreach (GameObject obj in mapParts){
				ExecuteEvents.Execute<IDirtyMarkerMap>(
					target: obj,
					eventData: null,
					functor: CallSetDirtyImmediately
				);
			}
		}

		/// <Summary>
		/// The functor of SendSetDirtyMsgImmediately method.
		/// </Summary>
		void CallSetDirtyImmediately(IDirtyMarkerMap marker, BaseEventData eventData){
			marker.OnSetDirty();
		}

		/// <Summary>
		/// Send SetNewMap message to DrawMap components.
		/// </Summary>
		void SendSetNewMap(){
			foreach (GameObject obj in mapParts){
				ExecuteEvents.Execute<IDirtyMarkerMap>(
					target: obj,
					eventData: null,
					functor: CallSetNewMap
				);
			}
		}

		/// <Summary>
		/// The functor of SendSetNewMap method.
		/// </Summary>
		void CallSetNewMap(IDirtyMarkerMap marker, BaseEventData eventData){
			marker.OnSetNewMap();
		}

		/// <Summary>
		/// Send ExitDungeon message to EnterDungeonManager.
		/// </Summary>
		/// <param name="obj">Specify the game controller object.</param>
		void SendExitDungeonMessage(GameObject obj){
				ExecuteEvents.Execute<IExitDungeon>(
					target: obj,
					eventData: null,
					functor: ExitDungeonMsg
				);
		}

		/// <Summary>
		/// The functor of SendExitDungeonMessage method.
		/// </Summary>
		void ExitDungeonMsg(IExitDungeon exitDungeon, BaseEventData eventData){
			exitDungeon.OnExitDungeon();
		}

		/// <Summary>
		/// Send Draw message to DrawManager.
		/// </Summary>
		/// <param name="obj">Specify the game controller object.</param>
		void SendDrawMessage(GameObject obj){
				ExecuteEvents.Execute<IDungeonObjects>(
					target: obj,
					eventData: null,
					functor: DrawDungeonMsg
				);
		}

		/// <Summary>
		/// The functor of SendDrawMessage method.
		/// </Summary>
		void DrawDungeonMsg(IDungeonObjects dungeon, BaseEventData eventData){
			dungeon.OnDrawObj();
		}

		/// <Summary>
		/// Send Redraw message to DrawManager.
		/// </Summary>
		/// <param name="obj">Specify the game controller object.</param>
		void SendRedrawMessage(GameObject obj){
				ExecuteEvents.Execute<IDungeonObjects>(
					target: obj,
					eventData: null,
					functor: RedrawDungeonMsg
				);
		}

		/// <Summary>
		/// The functor of SendRedrawMessage method.
		/// </Summary>
		void RedrawDungeonMsg(IDungeonObjects dungeon, BaseEventData eventData){
			dungeon.OnRedrawObj();
		}

		/// <Summary>
		/// Send Remove message to DrawManager for removing dungeon objects.
		/// </Summary>
		/// <param name="obj">Specify the game controller object.</param>
		void SendRemoveMessage(GameObject obj){
				ExecuteEvents.Execute<IDungeonObjects>(
					target: obj,
					eventData: null,
					functor: RemoveDungeonMsg
				);
		}

		/// <Summary>
		/// The functor of SendRemoveMessage method.
		/// </Summary>
		void RemoveDungeonMsg(IDungeonObjects dungeon, BaseEventData eventData){
			dungeon.OnRemoveObj();
		}

		/// <Summary>
		/// Send dungeon data to DungeonSettings.
		/// </Summary>
		/// <param name="obj">Specify the game controller object.</param>
		void SendDungeonData(GameObject obj){
				ExecuteEvents.Execute<IDungeonSetter>(
					target: obj,
					eventData: null,
					functor: SendDungeonMsg
				);
		}

		/// <Summary>
		/// The functor of SendDungeonData method.
		/// </Summary>
		void SendDungeonMsg(IDungeonSetter dungeon, BaseEventData eventData){
			dungeon.OnSetDungeon(moveDestDungeon);
		}

		/// <Summary>
		/// Send move finishing message to door event components.
		/// </Summary>
		/// <param name="obj">Specify the game controller object.</param>
		void SendMoveFinishedMsg(GameObject obj){
				ExecuteEvents.Execute<IDoorOpen>(
					target: obj,
					eventData: null,
					functor: SendFinishedMsg
				);
		}

		/// <Summary>
		/// The functor of SendMoveFinishedMsg method.
		/// </Summary>
		void SendFinishedMsg(IDoorOpen doorOpen, BaseEventData eventData){
			doorOpen.OnMoveFinished();
		}


       public void setMovable(bool movable)
        {
            isInDungeon = movable;
        }
	}
}
