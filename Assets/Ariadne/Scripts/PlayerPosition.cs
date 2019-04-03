using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ariadne {

	/// <Summary>
	/// Holds player position and direction data.
	/// </Summary>
	public static class PlayerPosition {
		
		public static Vector2Int playerPos;
		public static Vector2Int playerPosPre;
		public static DungeonDir direction = DungeonDir.North;
		public static int currentDungeonId = 0;
		public static int currentFloorId = 0;

		/// <Summary>
		/// Returns the position of specified steps forward.
		/// </Summary>
		/// <param name="units">Forward steps to get position.</param>
		public static Vector2Int GetForwardPosition(int units){
			Vector2Int ps = playerPos;
			switch (direction){
				case DungeonDir.North:
					ps.y += units;
					break;
				case DungeonDir.East:
					ps.x += units;
					break;
				case DungeonDir.South:
					ps.y -= units;
					break;
				case DungeonDir.West:
					ps.x -= units;
					break;
			}
			return ps;
		}

		/// <Summary>
		/// Returns the map attribute of the position that is specified direction of target position.
		/// </Summary>
		/// <param name="mapData">Target position to check.</param>
		/// <param name="pos">Target position to check.</param>
		/// <param name="dir">Direction to check.</param>
		public static int GetMapInfoByDirection(FloorMapMasterData mapData, Vector2Int pos, DungeonDir dir){
			int wallInfo = 0;
			Vector2Int targetPos = pos;
			switch (dir){
				case DungeonDir.North:
					targetPos.y += 1;
					break;
				case DungeonDir.East:
					targetPos.x += 1;
					break;
				case DungeonDir.South:
					targetPos.y -= 1;
					break;
				case DungeonDir.West:
					targetPos.x -= 1;
					break;
			}
			if (CheckPositionIsValid(targetPos, mapData.floorSizeHorizontal, mapData.floorSizeVertical)){
				int index = targetPos.x + targetPos.y * mapData.floorSizeHorizontal;
				wallInfo = mapData.mapInfo[index].mapAttr;
			} else {
				wallInfo = 1;
			}
			return wallInfo;
		}

		/// <Summary>
		/// Set forward position to playerPos.
		/// </Summary>
		public static void MoveForward(){
			playerPosPre = playerPos;
			switch (direction){
				case DungeonDir.North:
					playerPos.y += 1;
					break;
				case DungeonDir.East:
					playerPos.x += 1;
					break;
				case DungeonDir.South:
					playerPos.y -= 1;
					break;
				case DungeonDir.West:
					playerPos.x -= 1;
					break;
			}
			
		}

		/// <Summary>
		/// Set direction to player's left hand side.
		/// </Summary>
		public static void TurnLeft(){
			Direction dir = new Direction();
			direction = dir.GetCounterclockwiseDir(direction);
		}

		/// <Summary>
		/// Set direction to player's right hand side.
		/// </Summary>
		public static void TurnRight(){
			Direction dir = new Direction();
			direction = dir.GetClockwiseDir(direction);
		}

		/// <Summary>
		/// Set direction to the player's back side.
		/// </Summary>
		public static void TurnBack(){
			Direction dir = new Direction();
			direction = dir.GetReverseDir(direction);
		}

		/// <Summary>
		/// Returns the result of checking index validation.
		/// </Summary>
		/// <param name="targetPosition">Target position to check.</param>
		/// <param name="horizontalMax">Horizontal size of the floor.</param>
		/// <param name="verticalMax">Vertical size of the floor.</param>
		public static bool CheckPositionIsValid(Vector2Int targetPosition, int horizontalMax, int verticalMax){
			bool isValid = true;

			// Check x axis position
			if (targetPosition.x < 0 || targetPosition.x >= horizontalMax){
				isValid = false;
			}

			// Check y axis position
			if (targetPosition.y < 0 || targetPosition.y >= verticalMax){
				isValid = false;
			}
			return isValid;
		}

		/// <Summary>
		/// Set traverse data.
		/// </Summary>
		public static void SetTraverseData(){
			TraverseManager.SetTraverseData(currentDungeonId, currentFloorId, playerPos, true);
		}
	}
}