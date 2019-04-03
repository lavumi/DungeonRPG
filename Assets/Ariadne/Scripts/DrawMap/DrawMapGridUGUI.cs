using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ariadne {

	/// <Summary>
	/// Draw grid lines on the map.
	/// Grid lines are drawn as meshes.
	/// </Summary>
	public class DrawMapGridUGUI : MaskableGraphic, IDirtyMarkerMap {

		float gridLineWidth = 1f;
		int showLengthHorizontal = 4;
		int showLengthVertical = 4;
		int drawSmoothness = 2;
		FloorMapMasterData floorMapData;

		RectTransform parentRt;
		RectTransform mapRt;
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
			DrawGrid(vh);
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
			this.gridLineWidth = mapSettings.gridLineWidth;
			this.drawSmoothness = mapSettings.smoothness;
		}

		/// <Summary>
		/// Generates meshes of grid line according to map size.
		/// </Summary>
		void DrawGrid(VertexHelper vh){

			if (parentRt == null){
				parentRt = GameObject.FindGameObjectWithTag("Map").GetComponent<RectTransform>();
			}
			Vector2 parentPos = parentRt.transform.localPosition;

			if (mapRt == null){
				mapRt = gameObject.GetComponent<RectTransform>();
			}
			Vector3 centerPos = mapRt.transform.localPosition;

			float cellWidth = mapRt.sizeDelta.x / (1 + showLengthHorizontal * 2);
			float cellHeight = mapRt.sizeDelta.y / (1 + showLengthVertical * 2);
			float widthOffset = cellWidth / 2;
			float heightOffset = cellHeight / 2;
			float gridWidthOffset = gridLineWidth / 2;

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
			
			int longerLength = showLengthHorizontal >= showLengthVertical ? showLengthHorizontal : showLengthVertical;
			int horizontalLength = floorMapData.floorSizeHorizontal + longerLength + 1;
			int verticalLength = floorMapData.floorSizeVertical + longerLength + 1;

			for (int xAxis = -(longerLength + 1); xAxis <= horizontalLength; xAxis++){
				// Draw vertical lines
				Vector3 basePosInLoop = new Vector3(basePos.x + unitSize.x * xAxis, basePos.y - unitSize.y * (longerLength + 1));

				// Left bottom
				UIVertex leftBottom = UIVertex.simpleVert;
				leftBottom.position = new Vector3(basePosInLoop.x - gridWidthOffset, basePosInLoop.y, 0f);
				leftBottom.uv0 = leftBottomUv;

				// Left Top
				UIVertex leftTop = UIVertex.simpleVert;
				leftTop.position = new Vector3(basePosInLoop.x - gridWidthOffset, basePos.y + unitSize.y * verticalLength, 0f);
				leftTop.uv0 = leftTopUv;

				// Right Top
				UIVertex rightTop = UIVertex.simpleVert;
				rightTop.position = new Vector3(basePosInLoop.x + gridWidthOffset, basePos.y + unitSize.y * verticalLength, 0f);
				rightTop.uv0 = rightTopUv;

				// Right bottom
				UIVertex rightBottom = UIVertex.simpleVert;
				rightBottom.position = new Vector3(basePosInLoop.x + gridWidthOffset, basePosInLoop.y, 0f);
				rightBottom.uv0 = rightBottomUv;

				vh.AddUIVertexQuad(new UIVertex[]{
					leftBottom, rightBottom, rightTop, leftTop
				});
			}

			for (int yAxis = -(longerLength + 1); yAxis <= verticalLength; yAxis++){
				// Draw vertical lines
				Vector3 basePosInLoop = new Vector3(basePos.x - unitSize.x * (longerLength + 1), basePos.y + unitSize.y * yAxis);
				
				// Left bottom
				UIVertex leftBottom = UIVertex.simpleVert;
				leftBottom.position = new Vector3(basePosInLoop.x, basePosInLoop.y - gridWidthOffset, 0f);
				leftBottom.uv0 = leftBottomUv;

				// Left Top
				UIVertex leftTop = UIVertex.simpleVert;
				leftTop.position = new Vector3(basePosInLoop.x, basePosInLoop.y + gridWidthOffset, 0f);
				leftTop.uv0 = leftTopUv;

				// Right Top
				UIVertex rightTop = UIVertex.simpleVert;
				rightTop.position = new Vector3(basePos.x + unitSize.x * horizontalLength, basePosInLoop.y + gridWidthOffset, 0f);
				rightTop.uv0 = rightTopUv;

				// Right bottom
				UIVertex rightBottom = UIVertex.simpleVert;
				rightBottom.position = new Vector3(basePos.x + unitSize.x * horizontalLength, basePosInLoop.y - gridWidthOffset, 0f);
				rightBottom.uv0 = rightBottomUv;

				vh.AddUIVertexQuad(new UIVertex[]{
					leftBottom, rightBottom, rightTop, leftTop
				});
			}
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