using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ariadne {

	/// <Summary>
	/// Calculate directions which are used in the dungeon.
	/// The enumeration of directions is defined in AriadneEnumerations.
	/// </Summary>
	public class Direction {

		/// <Summary>
		/// Get the counterclockwise direction of the argument.
		/// </Summary>
		/// <param name="direction">Base direction.</param>
		public DungeonDir GetCounterclockwiseDir(DungeonDir direction){
			DungeonDir dir = direction;
			switch(direction){
				case DungeonDir.North:
					dir = DungeonDir.West;
					break;
				case DungeonDir.East:
					dir = DungeonDir.North;
					break;
				case DungeonDir.South:
					dir = DungeonDir.East;
					break;
				case DungeonDir.West:
					dir = DungeonDir.South;
					break;
			}
			return dir;
		}

		/// <Summary>
		/// Get the clockwise direction of the argument.
		/// </Summary>
		/// <param name="direction">Base direction.</param>
		public DungeonDir GetClockwiseDir(DungeonDir direction){
			DungeonDir dir = direction;
			switch(direction){
				case DungeonDir.North:
					dir = DungeonDir.East;
					break;
				case DungeonDir.East:
					dir = DungeonDir.South;
					break;
				case DungeonDir.South:
					dir = DungeonDir.West;
					break;
				case DungeonDir.West:
					dir = DungeonDir.North;
					break;
			}
			return dir;
		}

		/// <Summary>
		/// Get the reverse direction of the argument.
		/// </Summary>
		/// <param name="direction">Base direction.</param>
		public DungeonDir GetReverseDir(DungeonDir direction){
			DungeonDir dir = direction;
			switch(direction){
				case DungeonDir.North:
					dir = DungeonDir.South;
					break;
				case DungeonDir.East:
					dir = DungeonDir.West;
					break;
				case DungeonDir.South:
					dir = DungeonDir.North;
					break;
				case DungeonDir.West:
					dir = DungeonDir.East;
					break;
			}
			return dir;
		}

		/// <Summary>
		/// Get the direction name of the argument.
		/// </Summary>
		/// <param name="direction">Base direction.</param>
		public string GetDirectionName(DungeonDir direction){
			string dirName = "";
			switch(direction){
				case DungeonDir.North:
					dirName = "North";
					break;
				case DungeonDir.East:
					dirName = "East";
					break;
				case DungeonDir.South:
					dirName = "South";
					break;
				case DungeonDir.West:
					dirName = "West";
					break;
			}
			return dirName;
		}

		/// <Summary>
		/// Returns the rotation of the object that corresponds to DungeonDir.
		/// </Summary>
		/// <param name="dir">The direction of the object.</param>
		public Vector3 GetRotationOfDirection(DungeonDir dir){
			Vector3 rotation = Vector3.zero;
			switch (dir){
				case DungeonDir.North:
					rotation = Vector3.zero;
					break;
				case DungeonDir.East:
					rotation = new Vector3(0f, 90f, 0f);
					break;
				case DungeonDir.South:
					rotation = new Vector3(0f, 180f, 0f);
					break;
				case DungeonDir.West:
					rotation = new Vector3(0f, 270f, 0f);
					break;
			}
			return rotation;
		}
	}
}