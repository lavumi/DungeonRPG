using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ariadne {

	/// <Summary>
	/// Draw traversed position on the map.
	/// Traversed positions are drawn as meshes.
	/// </Summary>
	public class DrawMapTraversedUGUI : MaskableGraphic, IDirtyMarkerMap {

		int showLengthHorizontal = 4;
		int showLengthVertical = 4;
		int drawSmoothness = 2;

		DungeonMasterData dungeonData;
		FloorMapMasterData floorMapData;
		RectTransform parentRt;
		RectTransform mapRt;
		public bool isAutoMapping = true;
		Vector2 posLerp = Vector2.zero;
		GameObject gameController;
		DungeonSettings dungeonSettings;
		MapShowingSettings mapSettings;

		Vector2 leftBottomUv = new Vector2(0f, 0f);
		Vector2 leftTopUv = new Vector2(0f, 1f);
		Vector2 rightTopUv = new Vector2(1f, 1f);
		Vector2 rightBottomUv = new Vector2(1f, 0f);

		/// <Summary>
		/// Populate meshes.
		/// This method is called when the canvas is updated.
		/// </Summary>
		protected override void OnPopulateMesh(VertexHelper vh){
			vh.Clear();
			GetFloorData();
			GetMapSettings();
			DrawMap(vh);
		}

		/// <Summary>
		/// Get floor data from the DungeonSettings component.
		/// </Summary>
		void GetFloorData(){
			if (gameController == null){
				gameController = GameObject.FindGameObjectWithTag("GameController");
			}
			if (dungeonSettings == null){
				dungeonSettings = gameController.GetComponent<DungeonSettings>();
			}
			dungeonData = dungeonSettings.dungeonData;
			floorMapData = dungeonSettings.GetCurrentFloorData();
		}

		/// <Summary>
		/// Get settings about map from the MapShowingSettings component.
		/// </Summary>
		void GetMapSettings(){
			if (gameController == null){
				gameController = GameObject.FindGameObjectWithTag("GameController");
			}
			if (mapSettings == null){
				mapSettings = gameController.GetComponent<MapShowingSettings>();
			}
			this.showLengthHorizontal = mapSettings.showLengthHorizontal;
			this.showLengthVertical = mapSettings.showLengthVertical;
			this.drawSmoothness = mapSettings.smoothness;
		}

		/// <Summary>
		/// Generates meshes of traversed position.
		/// If "isAutoMapping" is false, all hallways in the map are shown on the map.
		/// </Summary>
		void DrawMap(VertexHelper vh){

			if (mapRt == null){
				mapRt = gameObject.GetComponent<RectTransform>();
			}
			Vector3 centerPos = mapRt.transform.localPosition;

			float cellWidth = mapRt.sizeDelta.x / (1 + showLengthHorizontal * 2);
			float cellHeight = mapRt.sizeDelta.y / (1 + showLengthVertical * 2);
			float widthOffset = cellWidth / 2;
			float heightOffset = cellHeight / 2;

			if (parentRt == null){
				parentRt = GameObject.FindGameObjectWithTag("Map").GetComponent<RectTransform>();
			}

			Vector2 parentPos = parentRt.transform.localPosition;
			float parentWidth = parentRt.sizeDelta.x;
			float parentHeight = parentRt.sizeDelta.y;

			Rect r = mapRt.rect;
			r.x += parentPos.x + centerPos.x;
			r.y += parentPos.y + centerPos.y;
			SetClipRect(r, true);

			int signX = centerPos.x < 0 ? -1 : 1;
			int signY = centerPos.y < 0 ? -1 : 1;

			Vector2 unitSize = new Vector2(cellWidth, cellHeight);
			float basePosX = parentWidth / 2 - signX * centerPos.x - widthOffset - posLerp.x * unitSize.x;
			float basePosY = parentHeight / 2 - signY * centerPos.y - heightOffset - posLerp.y * unitSize.y;
			Vector2 basePos = new Vector2(basePosX, basePosY);

			for (int yAxis = 0; yAxis < floorMapData.floorSizeVertical; yAxis++){
				for (int xAxis = 0; xAxis < floorMapData.floorSizeHorizontal; xAxis++){
					int index = xAxis + yAxis * floorMapData.floorSizeHorizontal;
					Vector3 basePosInLoop = new Vector3(basePos.x + unitSize.x * xAxis, basePos.y + unitSize.y * yAxis);
					
					int mapAttrId = MapAttributeDefine.WALL;
					Vector2Int pos = new Vector2Int(xAxis, yAxis);

					if (isAutoMapping){
						if (!TraverseManager.GetPositionTraverseData(dungeonData.dungeonId, floorMapData.floorId, pos)){
							continue;
						}
					}
					mapAttrId = floorMapData.mapInfo[index].mapAttr;
					PositionSet posSet = GetPositionSet(basePosInLoop, unitSize);
					DrawMTraversedPos(vh, mapAttrId, posSet);
				}
			}
		}

		/// <Summary>
		/// Generate meshes.
		/// </Summary>
		void DrawMTraversedPos(VertexHelper vh, int mapAttrId, PositionSet pos){
			if (mapAttrId != MapAttributeDefine.WALL){
					
				// Left bottom
				UIVertex leftBottom = UIVertex.simpleVert;
				leftBottom.position = pos.bottomLeft;
				leftBottom.uv0 = leftBottomUv;

				// Left Top
				UIVertex leftTop = UIVertex.simpleVert;
				leftTop.position = pos.topLeft;
				leftTop.uv0 = leftTopUv;

				// Right Top
				UIVertex rightTop = UIVertex.simpleVert;
				rightTop.position = pos.topRight;
				rightTop.uv0 = rightTopUv;

				// Right bottom
				UIVertex rightBottom = UIVertex.simpleVert;
				rightBottom.position = pos.bottomRight;
				rightBottom.uv0 = rightBottomUv;

				vh.AddUIVertexQuad(new UIVertex[]{
					leftBottom, rightBottom, rightTop, leftTop
				});
			}
		}

		/// <Summary>
		/// Returns Ariadne.PositionSet data to define the position of the drawing mesh.
		/// </Summary>
		PositionSet GetPositionSet(Vector3 basePosInLoop, Vector2 unitSize){
			PositionSet posSet = new PositionSet();
			posSet.bottomLeft = new Vector3(basePosInLoop.x, basePosInLoop.y, 0f);
			posSet.topLeft = new Vector3(basePosInLoop.x, basePosInLoop.y + unitSize.y, 0f);
			posSet.topRight = new Vector3(basePosInLoop.x + unitSize.x, basePosInLoop.y + unitSize.y, 0f);
			posSet.bottomRight = new Vector3(basePosInLoop.x + unitSize.x, basePosInLoop.y, 0f);
			return posSet;
		}

		/// <Summary>
		/// Receiver of OnSetDirty message.
		/// </Summary>
		public void OnSetDirty(){
			SetAllDirty();
		}

		/// <Summary>
		/// Receiver of OnSetDirtyLerp message.
		/// </Summary>
		/// <param name="time">The moveWait in MoveController.</param>
		public void OnSetDirtyLerp(float time){
			StartCoroutine(DrawIntermediateMap(time));
		}

		/// <Summary>
		/// Draw intermediate map according to the value of drawSmoothness.
		/// </Summary>
		/// <param name="time">The moveWait in MoveController.</param>
		IEnumerator DrawIntermediateMap(float time){
			yield return null;
			Vector2Int sourcePos = PlayerPosition.playerPosPre;
			Vector2Int destPos = PlayerPosition.playerPos;
			float finishTime = Time.time + time;

			while (true){
				float diff = finishTime - Time.time;
				if (diff <= 0){
					break;
				}
				float rate = 1 - Mathf.Clamp01(diff / time);
				posLerp = Vector2.Lerp(sourcePos, destPos, rate);
				// To update meshes, call Graphic.SetAllDirty() method.
				SetAllDirty();
				for (int i = 0; i < drawSmoothness; i++){
					yield return null;
				}
			}
			posLerp = destPos;
			SetAllDirty();
		}

		/// <Summary>
		/// Receiver of OnSetNewMap message.
		/// </Summary>
		public void OnSetNewMap(){
			// Get new floor data.
			GetFloorData();

			// Draw new floor.
			SetAllDirty();
		}
	}
}