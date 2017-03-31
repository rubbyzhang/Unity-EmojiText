using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System;

namespace EasyLayout {
	[Serializable]
	/// <summary>
	/// Padding.
	/// </summary>
	public struct Padding
	{
		/// <summary>
		/// The left padding.
		/// </summary>
		[SerializeField]
		public float Left;

		/// <summary>
		/// The right padding.
		/// </summary>
		[SerializeField]
		public float Right;

		/// <summary>
		/// The top padding.
		/// </summary>
		[SerializeField]
		public float Top;

		/// <summary>
		/// The bottom padding.
		/// </summary>
		[SerializeField]
		public float Bottom;

		/// <summary>
		/// Initializes a new instance of the struct.
		/// </summary>
		/// <param name="left">Left.</param>
		/// <param name="right">Right.</param>
		/// <param name="top">Top.</param>
		/// <param name="bottom">Bottom.</param>
		public Padding(float left, float right, float top, float bottom)
		{
			Left = left;
			Right = right;
			Top = top;
			Bottom = bottom;
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format("Padding(left: {0}, right: {1}, top: {2}, bottom: {3})",
				Left,
				Right,
				Top,
				Bottom
			);
		}
	}

	[Flags]
	/// <summary>
	/// Children size.
	/// DoNothing - Don't change size of children.
	/// SetPreferred - Set size of children to preferred.
	/// SetMaxFromPreferred - Set size of children to maximum size of children preferred.
	/// FitContainer - Stretch size of children to fit container.
	/// </summary>
	public enum ChildrenSize {
		DoNothing = 0,
		SetPreferred = 1,
		SetMaxFromPreferred = 2,
		FitContainer = 3,
	}

	[Flags]
	/// <summary>
	/// Anchors.
	/// UpperLeft - UpperLeft.
	/// UpperCenter - UpperCenter.
	/// UpperRight - UpperRight.
	/// MiddleLeft - MiddleLeft.
	/// MiddleCenter - MiddleCenter.
	/// MiddleRight - MiddleRight.
	/// LowerLeft - LowerLeft.
	/// LowerCenter - LowerCenter.
	/// LowerRight - LowerRight.
	/// </summary>
	public enum Anchors {
		UpperLeft = 0,
		UpperCenter = 1,
		UpperRight = 2,
		
		MiddleLeft = 3,
		MiddleCenter = 4,
		MiddleRight = 5,
		
		LowerLeft = 6,
		LowerCenter = 7,
		LowerRight = 8,
	}
	
	[Flags]
	/// <summary>
	/// Stackings.
	/// Horizontal - Horizontal.
	/// Vertical - Vertical.
	/// </summary>
	public enum Stackings {
		Horizontal = 0,
		Vertical = 1,
	}

	[Flags]
	/// <summary>
	/// Horizontal aligns.
	/// Left - Left.
	/// Center - Center.
	/// Right - Right.
	/// </summary>
	public enum HorizontalAligns {
		Left = 0,
		Center = 1,
		Right = 2,
	}

	[Flags]
	/// <summary>
	/// Inner aligns.
	/// Top - Top.
	/// Middle - Middle.
	/// Bottom - Bottom.
	/// </summary>
	public enum InnerAligns {
		Top = 0,
		Middle = 1,
		Bottom = 2,
	}

	[Flags]
	/// <summary>
	/// Layout type to use.
	/// Compact - Compact.
	/// Grid - Grid.
	/// </summary>
	public enum LayoutTypes {
		Compact = 0,
		Grid = 1,
	}

	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	/// <summary>
	/// EasyLayout.
	/// </summary>
	public class EasyLayout : UnityEngine.UI.LayoutGroup {
		/// <summary>
		/// The group position.
		/// </summary>
		[SerializeField]
		public Anchors GroupPosition = Anchors.UpperLeft;

		/// <summary>
		/// The stacking type.
		/// </summary>
		[SerializeField]
		public Stackings Stacking = Stackings.Horizontal;
		
		/// <summary>
		/// The type of the layout.
		/// </summary>
		[SerializeField]
		public LayoutTypes LayoutType = LayoutTypes.Compact;

		/// <summary>
		/// The row align.
		/// </summary>
		[SerializeField]
		public HorizontalAligns RowAlign = HorizontalAligns.Left;

		/// <summary>
		/// The inner align.
		/// </summary>
		[SerializeField]
		public InnerAligns InnerAlign = InnerAligns.Top;

		/// <summary>
		/// The cell align.
		/// </summary>
		[SerializeField]
		public Anchors CellAlign = Anchors.UpperLeft;

		/// <summary>
		/// The spacing.
		/// </summary>
		[SerializeField]
		public Vector2 Spacing = new Vector2(5, 5);

		/// <summary>
		/// Symmetric margin.
		/// </summary>
		[SerializeField]
		public bool Symmetric = true;

		/// <summary>
		/// The margin.
		/// </summary>
		[SerializeField]
		public Vector2 Margin = new Vector2(5, 5);

		/// <summary>
		/// The padding.
		/// </summary>
		[SerializeField]
		public Padding PaddingInner = new Padding();

		/// <summary>
		/// The margin top.
		/// </summary>
		[SerializeField]
		public float MarginTop = 5f;

		/// <summary>
		/// The margin bottom.
		/// </summary>
		[SerializeField]
		public float MarginBottom = 5f;

		/// <summary>
		/// The margin left.
		/// </summary>
		[SerializeField]
		public float MarginLeft = 5f;

		/// <summary>
		/// The margin right.
		/// </summary>
		[SerializeField]
		public float MarginRight = 5f;

		/// <summary>
		/// The right to left stacking.
		/// </summary>
		[SerializeField]
		public bool RightToLeft = false;

		/// <summary>
		/// The top to bottom stacking.
		/// </summary>
		[SerializeField]
		public bool TopToBottom = true;

		/// <summary>
		/// The skip inactive.
		/// </summary>
		[SerializeField]
		public bool SkipInactive = true;

		/// <summary>
		/// The filter.
		/// </summary>
		public Func<IEnumerable<GameObject>,IEnumerable<GameObject>> Filter = null;

		/// <summary>
		/// How to control width of the children.
		/// </summary>
		public ChildrenSize ChildrenWidth;

		/// <summary>
		/// How to control height of the children.
		/// </summary>
		public ChildrenSize ChildrenHeight;

		/// <summary>
		/// Control width of children.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ChildrenWidth with ChildrenSize.SetPreferred instead.")]
		public bool ControlWidth;

		/// <summary>
		/// Control height of children.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ChildrenHeight with ChildrenSize.SetPreferred instead.")]
		public bool ControlHeight;

		/// <summary>
		/// Sets width of the chidren to maximum width from them.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ChildrenWidth with ChildrenSize.SetMaxFromPreferred instead.")]
		public bool MaxWidth;
		
		/// <summary>
		/// Sets height of the chidren to maximum height from them.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ChildrenHeight with ChildrenSize.SetMaxFromPreferred instead.")]
		public bool MaxHeight;

		Vector2 _blockSize;

		/// <summary>
		/// Gets or sets the size of the inner block.
		/// </summary>
		/// <value>The size of the inner block.</value>
		public Vector2 BlockSize {
			get {
				return _blockSize;
			}
			protected set {
				_blockSize = value;
			}
		}

		Vector2 _uiSize;
		/// <summary>
		/// Gets or sets the UI size.
		/// </summary>
		/// <value>The UI size.</value>
		public Vector2 UISize {
			get {
				return _uiSize;
			}
			protected set {
				_uiSize = value;
			}
		}

		/// <summary>
		/// Gets the minimum height.
		/// </summary>
		/// <value>The minimum height.</value>
		public override float minHeight
		{
			get
			{
				//CalculateLayoutSize();
				return BlockSize[1];
			}
		}

		/// <summary>
		/// Gets the minimum width.
		/// </summary>
		/// <value>The minimum width.</value>
		public override float minWidth
		{
			get
			{
				//CalculateLayoutSize();
				return BlockSize[0];
			}
		}

		/// <summary>
		/// Gets the preferred height.
		/// </summary>
		/// <value>The preferred height.</value>
		public override float preferredHeight
		{
			get
			{
				//CalculateLayoutSize();
				return BlockSize[1];
			}
		}

		/// <summary>
		/// Gets the preferred width.
		/// </summary>
		/// <value>The preferred width.</value>
		public override float preferredWidth
		{
			get
			{
				//CalculateLayoutSize();
				return BlockSize[0];
			}
		}

		static readonly Dictionary<Anchors,Vector2> groupPositions = new Dictionary<Anchors,Vector2>{
			{Anchors.LowerLeft,    new Vector2(0.0f, 0.0f)},
			{Anchors.LowerCenter,  new Vector2(0.5f, 0.0f)},
			{Anchors.LowerRight,   new Vector2(1.0f, 0.0f)},
			
			{Anchors.MiddleLeft,   new Vector2(0.0f, 0.5f)},
			{Anchors.MiddleCenter, new Vector2(0.5f, 0.5f)},
			{Anchors.MiddleRight,  new Vector2(1.0f, 0.5f)},
			
			{Anchors.UpperLeft,    new Vector2(0.0f, 1.0f)},
			{Anchors.UpperCenter,  new Vector2(0.5f, 1.0f)},
			{Anchors.UpperRight,   new Vector2(1.0f, 1.0f)},
		};

		static readonly Dictionary<HorizontalAligns,float> rowAligns = new Dictionary<HorizontalAligns,float>{
			{HorizontalAligns.Left,   0.0f},
			{HorizontalAligns.Center, 0.5f},
			{HorizontalAligns.Right,  1.0f},
		};
		static readonly Dictionary<InnerAligns,float> innerAligns = new Dictionary<InnerAligns,float>{
			{InnerAligns.Top,   0.0f},
			{InnerAligns.Middle, 0.5f},
			{InnerAligns.Bottom,  1.0f},
		};

		/// <summary>
		/// Raises the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			propertiesTracker.Clear();
			base.OnDisable();
		}

		/// <summary>
		/// Raises the rect transform removed event.
		/// </summary>
		void OnRectTransformRemoved()
		{
			SetDirty();
		}

		/// <summary>
		/// Sets the layout horizontal.
		/// </summary>
		public override void SetLayoutHorizontal()
		{
			RepositionUIElements();
		}

		/// <summary>
		/// Sets the layout vertical.
		/// </summary>
		public override void SetLayoutVertical()
		{
			//RepositionUIElements();
		}

		/// <summary>
		/// Calculates the layout input horizontal.
		/// </summary>
		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			CalculateLayoutSize();
		}

		/// <summary>
		/// Calculates the layout input vertical.
		/// </summary>
		public override void CalculateLayoutInputVertical()
		{
			//CalculateLayoutSize();
		}

		/// <summary>
		/// Gets the length.
		/// </summary>
		/// <returns>The length.</returns>
		/// <param name="ui">User interface.</param>
		/// <param name="scaled">If set to <c>true</c> scaled.</param>
		public float GetLength(RectTransform ui, bool scaled=true)
		{
			if (scaled)
			{
				return Stacking==Stackings.Horizontal ? ScaledWidth(ui) : ScaledHeight(ui);
			}
			return Stacking==Stackings.Horizontal ? ui.rect.width : ui.rect.height;
		}

		float GetRowWidth(List<RectTransform> row)
		{
			return row.Select<RectTransform,float>(ScaledWidth).Sum() + (Spacing.x * (row.Count - 1)); 
		}

		float GetRowHeight(List<RectTransform> row)
		{
			return row.Select<RectTransform,float>(ScaledHeight).Max() + Spacing.y; 
		}

		Vector2 CalculateGroupSize(List<List<RectTransform>> group)
		{
			float width;
			if (LayoutType==LayoutTypes.Compact)
			{
				width = group.Select<List<RectTransform>,float>(GetRowWidth).Max();
			}
			else
			{
				var widths = GetMaxColumnsWidths(group);
				width = widths.Sum() + widths.Length * Spacing.x - Spacing.x;
			}
			float height = group.Select<List<RectTransform>,float>(GetRowHeight).Sum() - Spacing.y;

			width += PaddingInner.Left + PaddingInner.Right;
			height += PaddingInner.Top + PaddingInner.Bottom;

			return new Vector2(width, height);
		}

		/// <summary>
		/// Marks layout to update.
		/// </summary>
		public void NeedUpdateLayout()
		{
			UpdateLayout();
		}

		void UpdateBlockSize()
		{
			if (Symmetric)
			{
				BlockSize = new Vector2(UISize.x + Margin.x * 2, UISize.y + Margin.y * 2);
			}
			else
			{
				BlockSize = new Vector2(UISize.x + MarginLeft + MarginRight, UISize.y + MarginLeft + MarginRight);
			}
		}

		/// <summary>
		/// Calculates the size of the layout.
		/// </summary>
		public void CalculateLayoutSize()
		{
			var group = GroupUIElements();
			if (group.Count==0)
			{
				UISize = new Vector2(0, 0);
				UpdateBlockSize();
				
				return ;
			}
			
			UISize = CalculateGroupSize(group);
			UpdateBlockSize();
		}

		void RepositionUIElements()
		{
			var group = GroupUIElements();
			if (group.Count==0)
			{
				UISize = new Vector2(0, 0);
				UpdateBlockSize();
				
				//LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
				return ;
			}
			
			UISize = CalculateGroupSize(group);
			UpdateBlockSize();
			
			var anchor_position = groupPositions[GroupPosition];
			var start_position = new Vector2(
				rectTransform.rect.width * (anchor_position.x - rectTransform.pivot.x),
				rectTransform.rect.height * (anchor_position.y - rectTransform.pivot.y)
			);
			
			start_position.x -= anchor_position.x * UISize.x;
			start_position.y += (1 - anchor_position.y) * UISize.y;

			start_position.x += GetMarginLeft() * ((1 - anchor_position.x) * 2 - 1);
			start_position.y += GetMarginTop() * ((1 - anchor_position.y) * 2 - 1);
			
			start_position.x += PaddingInner.Left;
			start_position.y -= PaddingInner.Top;

			SetPositions(group, start_position, UISize);
			
			//LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
		}

		/// <summary>
		/// Updates the layout.
		/// </summary>
		public void UpdateLayout()
		{
			CalculateLayoutInputHorizontal();

			RepositionUIElements();
		}
		
		Vector2 GetUIPosition(RectTransform ui, Vector2 position, Vector2 align)
		{
			var pivot_fix_x = ScaledWidth(ui) * ui.pivot.x;
			var pivox_fix_y = ScaledHeight(ui) * ui.pivot.y;
			var new_x = position.x + pivot_fix_x + align.x;
			var new_y = position.y - ScaledHeight(ui) + pivox_fix_y - align.y;
			
			return new Vector2(new_x, new_y);
		}
		
		List<float> GetRowsWidths(List<List<RectTransform>> group)
		{
			var widths = new List<float>();

			foreach (var row_index in Enumerable.Range(0, group.Count))
			{
				var width = group[row_index].Convert<RectTransform,float>(ScaledWidth).Sum();
				widths.Add(width + (group[row_index].Count - 1) * Spacing.x);
			}
			return widths;
		}

		float GetMaxColumnWidth(List<RectTransform> column)
		{
			return column.Convert<RectTransform,float>(ScaledWidth).Max();
		}

		float[] GetMaxColumnsWidths(List<List<RectTransform>> group)
		{
			return Transpose(group).Select<List<RectTransform>,float>(GetMaxColumnWidth).ToArray();
		}

		List<float> GetColumnsHeights(List<List<RectTransform>> group)
		{
			var heights = new List<float>();
			
			foreach (var column_index in Enumerable.Range(0, group.Count))
			{
				var height = group[column_index].Convert<RectTransform,float>(ScaledHeight).Sum();
				heights.Add(height + (group[column_index].Count - 1) * Spacing.y);
			}
			return heights;
		}

		float GetMaxRowHeight(List<RectTransform> row)
		{
			return row.Select<RectTransform,float>(ScaledHeight).Max();
		}

		float[] GetMaxRowsHeights(List<List<RectTransform>> group)
		{
			return Transpose(group).Convert<List<RectTransform>,float>(GetMaxRowHeight).ToArray();
		}

		Vector2 GetMaxCellSize(List<List<RectTransform>> group)
		{
			var rows_max = group.Convert<List<RectTransform>,Vector2>(GetMaxCellSize);

			return new Vector2(
				rows_max.Max<Vector2,float>(GetWidth),
				rows_max.Max<Vector2,float>(GetHeight)
			);
		}

		float GetWidth(Vector2 size)
		{
			return size.x;
		}

		float GetHeight(Vector2 size)
		{
			return size.y;
		}

		Vector2 GetMaxCellSize(List<RectTransform> row)
		{
			return new Vector2(
				row.Max<RectTransform,float>(ScaledWidth),
				row.Max<RectTransform,float>(ScaledHeight)
			);
		}

		Vector2 GetAlignByWidth(RectTransform ui, float maxWidth, Vector2 cellMaxSize, float emptyWidth)
		{
			if (LayoutType==LayoutTypes.Compact)
			{
				return new Vector2(
					emptyWidth * rowAligns[RowAlign],
					(cellMaxSize.y - ScaledHeight(ui)) * innerAligns[InnerAlign]
				);
			}
			else
			{
				var cell_align = groupPositions[CellAlign];

				return new Vector2(
					(maxWidth - ScaledWidth(ui)) * cell_align.x,
					(cellMaxSize.y - ScaledHeight(ui)) * (1 - cell_align.y)
				);
			}
		}

		Vector2 GetAlignByHeight(RectTransform ui, float maxHeight, Vector2 cellMaxSize, float emptyHeight)
		{
			if (LayoutType==LayoutTypes.Compact)
			{
				return new Vector2(
					(cellMaxSize.x - ScaledWidth(ui)) * innerAligns[InnerAlign],
					emptyHeight * rowAligns[RowAlign]
				);
			}
			else
			{
				var cell_align = groupPositions[CellAlign];
				
				return new Vector2(
					(cellMaxSize.x - ScaledWidth(ui)) * (1 - cell_align.x),
					(maxHeight - ScaledHeight(ui)) * cell_align.y
				);
			}
		}

		void SetPositions(List<List<RectTransform>> group, Vector2 startPosition, Vector2 groupSize)
		{
			var position = startPosition;

			if (Stacking==Stackings.Horizontal)
			{
				var rows_widths = GetRowsWidths(group);
				var max_widths = GetMaxColumnsWidths(group);

				var align = new Vector2(0, 0);

				int coord_x = 0;
				foreach (var row in group)
				{
					var row_cell_max_size = GetMaxCellSize(row);

					int coord_y = 0;
					foreach (var ui_element in row)
					{
						align = GetAlignByWidth(ui_element, max_widths[coord_y], row_cell_max_size, groupSize.x - rows_widths[coord_x]);

						var new_position = GetUIPosition(ui_element, position, align);
						if (ui_element.localPosition.x != new_position.x || ui_element.localPosition.y != new_position.y)
						{
							ui_element.localPosition = new_position;
						}

						position.x += ((LayoutType==LayoutTypes.Compact)
						    ? ScaledWidth(ui_element)
							: max_widths[coord_y]) + Spacing.x;

						coord_y += 1;
					}
					position.x = startPosition.x;
					position.y -= row_cell_max_size.y + Spacing.y;

					coord_x += 1;
				}
			}
			else
			{
				group = Transpose(group);
				var rows_height = GetColumnsHeights(group);
				var max_heights = GetMaxRowsHeights(group);
				
				var align = new Vector2(0, 0);
				
				int coord_y = 0;
				foreach (var row in group)
				{
					var column_cell_max_size = GetMaxCellSize(row);
					
					int coord_x = 0;
					foreach (var ui_element in row)
					{
						align = GetAlignByHeight(ui_element, max_heights[coord_x], column_cell_max_size, groupSize.y - rows_height[coord_y]);
						
						var new_position = GetUIPosition(ui_element, position, align);
						if (ui_element.localPosition.x != new_position.x || ui_element.localPosition.y != new_position.y)
						{
							ui_element.localPosition = new_position;
						}
						
						position.y -= ((LayoutType==LayoutTypes.Compact)
						               ? ScaledHeight(ui_element)
						               : max_heights[coord_x]) + Spacing.y;
						
						coord_x += 1;
					}
					position.y = startPosition.y;
					position.x += column_cell_max_size.x + Spacing.x;
					
					coord_y += 1;
				}
			}
		}

		float max_width = -1;
		float max_height = -1;
		void ResizeElements(List<RectTransform> elements)
		{
			propertiesTracker.Clear();

			if (ChildrenWidth==ChildrenSize.DoNothing && ChildrenHeight==ChildrenSize.DoNothing)
			{
				return ;
			}
			if (elements==null)
			{
				return ;
			}
			if (elements.Count==0)
			{
				return ;
			}
			max_width = (ChildrenWidth==ChildrenSize.SetMaxFromPreferred) ? elements.Select<RectTransform,float>(GetPreferredWidth).Max() : -1f;
			max_height = (ChildrenHeight==ChildrenSize.SetMaxFromPreferred) ? elements.Select<RectTransform,float>(GetPreferredHeight).Max() : -1f;

			elements.ForEach(ResizeChild);
		}

		/// <summary>
		/// Gets the preferred width of the RectTransform.
		/// </summary>
		/// <returns>The preferred width.</returns>
		/// <param name="rect">Rect.</param>
		public static float GetPreferredWidth(RectTransform rect)
		{
			#if UNITY_4_6 || UNITY_4_7
			return Mathf.Max(1f, LayoutUtility.GetPreferredWidth(rect));
			#else
			if (rect==null)
			{
				return 1f;
			}
			if (rect.gameObject.activeInHierarchy)
			{
				return Mathf.Max(1, LayoutUtility.GetPreferredWidth(rect));
			}
			else
			{
				float result = 1f;
				var elements = rect.GetComponents<ILayoutElement>();
				var max_priority = elements.Max(x => x.layoutPriority);
				foreach (var elem in elements)
				{
					if (elem.layoutPriority==max_priority)
					{
						result = Mathf.Max(result, Mathf.Max(elem.preferredWidth, elem.minWidth));
					}
				}
				return result;
			}
			#endif
		}

		/// <summary>
		/// Gets the preferred height of the RectTransform.
		/// </summary>
		/// <returns>The preferred height.</returns>
		/// <param name="rect">Rect.</param>
		public static float GetPreferredHeight(RectTransform rect)
		{
			#if UNITY_4_6 || UNITY_4_7
			return Mathf.Max(1f, LayoutUtility.GetPreferredHeight(rect));
			#else
			if (rect==null)
			{
				return 1f;
			}
			if (rect.gameObject.activeInHierarchy)
			{
				return Mathf.Max(1, LayoutUtility.GetPreferredHeight(rect));
			}
			else
			{
				float result = 1f;
				var elements = rect.GetComponents<ILayoutElement>();
				var max_priority = elements.Max(x => x.layoutPriority);
				foreach (var elem in elements)
				{
					if (elem.layoutPriority==max_priority)
					{
						result = Mathf.Max(result, Mathf.Max(elem.preferredHeight, elem.minHeight));
					}
				}
				return result;
			}
			#endif
		}

		/// <summary>
		/// Gets the flexible width of the RectTransform.
		/// </summary>
		/// <returns>The flexible width.</returns>
		/// <param name="rect">Rect.</param>
		public static float GetFlexibleWidth(RectTransform rect)
		{
			#if UNITY_4_6 || UNITY_4_7
			return Mathf.Max(1, LayoutUtility.GetFlexibleWidth(rect));
			#else
			if (rect==null)
			{
				return 1f;
			}
			if (rect.gameObject.activeInHierarchy)
			{
				return Mathf.Max(1, LayoutUtility.GetFlexibleWidth(rect));
			}
			else
			{
				float result = 1f;
				var elements = rect.GetComponents<ILayoutElement>();
				var max_priority = elements.Max(x => x.layoutPriority);
				foreach (var elem in elements)
				{
					if (elem.layoutPriority==max_priority)
					{
						result = Mathf.Max(result, elem.flexibleWidth);
					}
				}
				return result;
			}
			#endif
		}

		/// <summary>
		/// Gets the flexible height of the RectTransform.
		/// </summary>
		/// <returns>The flexible height.</returns>
		/// <param name="rect">Rect.</param>
		public static float GetFlexibleHeight(RectTransform rect)
		{
			#if UNITY_4_6 || UNITY_4_7
			return Mathf.Max(1, LayoutUtility.GetFlexibleHeight(rect));
			#else
			if (rect==null)
			{
				return 1f;
			}
			if (rect.gameObject.activeInHierarchy)
			{
				return Mathf.Max(1, LayoutUtility.GetFlexibleHeight(rect));
			}
			else
			{
				float result = 1f;
				var elements = rect.GetComponents<ILayoutElement>();
				var max_priority = elements.Max(x => x.layoutPriority);
				foreach (var elem in elements)
				{
					if (elem.layoutPriority==max_priority)
					{
						result = Mathf.Max(result, elem.flexibleHeight);
					}
				}
				return result;
			}
			#endif
		}

		DrivenRectTransformTracker propertiesTracker;
		void ResizeChild(RectTransform child)
		{
			DrivenTransformProperties driven_properties = DrivenTransformProperties.None;

			if (ChildrenWidth!=ChildrenSize.DoNothing)
			{
				driven_properties |= DrivenTransformProperties.SizeDeltaX;
				var width = (max_width!=-1) ? max_width : GetPreferredWidth(child);
				child.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			}
			if (ChildrenHeight!=ChildrenSize.DoNothing)
			{
				driven_properties |= DrivenTransformProperties.SizeDeltaY;
				var height = (max_height!=-1) ? max_height : GetPreferredHeight(child);
				child.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
			}

			propertiesTracker.Add(this, child, driven_properties);
		}

		bool IsIgnoreLayout(Transform rect)
		{
			#if UNITY_4_6 || UNITY_4_7
			var ignorer = rect.GetComponent(typeof(ILayoutIgnorer)) as ILayoutIgnorer;
			#else
			var ignorer = rect.GetComponent<ILayoutIgnorer>();
			#endif
			return (ignorer!=null) && ignorer.ignoreLayout;
		}

		List<RectTransform> GetUIElements()
		{
			var elements = rectChildren;

			if (!SkipInactive)
			{
				elements = new List<RectTransform>();
				foreach (Transform child in transform)
				{
					if (!IsIgnoreLayout(child))
					{
						elements.Add(child as RectTransform);
					}
				}
			}

			if (Filter!=null)
			{
				var temp = Filter(elements.Convert<RectTransform,GameObject>(GetGameObject));
				var result = temp.Select<GameObject,RectTransform>(GetRectTransform).ToList();

				ResizeElements(result);

				return result;
			}

			ResizeElements(elements);

			return elements;
		}

		GameObject GetGameObject(RectTransform element)
		{
			return element.gameObject;
		}

		RectTransform GetRectTransform(GameObject go)
		{
			return go.transform as RectTransform;
		}

		/// <summary>
		/// Gets the margin top.
		/// </summary>
		public float GetMarginTop()
		{
			return Symmetric ? Margin.y : MarginTop;
		}
		
		/// <summary>
		/// Gets the margin bottom.
		/// </summary>
		public float GetMarginBottom()
		{
			return Symmetric ? Margin.y : MarginBottom;
		}

		/// <summary>
		/// Gets the margin left.
		/// </summary>
		public float GetMarginLeft()
		{
			return Symmetric ? Margin.x : MarginLeft;
		}

		/// <summary>
		/// Gets the margin right.
		/// </summary>
		public float GetMarginRight()
		{
			return Symmetric ? Margin.y : MarginRight;
		}

		void ReverseList(List<RectTransform> list)
		{
			list.Reverse();
		}

		List<List<RectTransform>> GroupUIElements()
		{
			var base_length = GetLength(rectTransform, false);
			base_length -= (Stacking==Stackings.Horizontal) ? (GetMarginLeft() + GetMarginRight()) : (GetMarginTop() + GetMarginBottom());

			var ui_elements = GetUIElements();

			var group = (LayoutType==LayoutTypes.Compact)
				? EasyLayoutCompact.Group(ui_elements, base_length, this)
				: EasyLayoutGrid.Group(ui_elements, base_length, this);

			if (Stacking==Stackings.Vertical)
			{
				group = Transpose(group);
			}

			if (!TopToBottom)
			{
				group.Reverse();
			}
			
			if (RightToLeft)
			{
				group.ForEach(ReverseList);
			}

			var width = rectTransform.rect.width - (GetMarginLeft() + GetMarginRight());
			var height = rectTransform.rect.height - (GetMarginTop() + GetMarginBottom());
			if (LayoutType==LayoutTypes.Grid)
			{
				if (ChildrenWidth==ChildrenSize.FitContainer)
				{
					ResizeColumnWidthToFit(width, group);
				}
				if (ChildrenHeight==ChildrenSize.FitContainer)
				{
					ResizeRowHeightToFit(height, group);
				}
			}
			else
			{
				if (Stacking==Stackings.Horizontal)
				{
					if (ChildrenWidth==ChildrenSize.FitContainer)
					{
						ResizeWidthToFit(width, group);
					}
					if (ChildrenHeight==ChildrenSize.FitContainer)
					{
						ResizeRowHeightToFit(height, group);
					}
				}
				else
				{
					if (ChildrenHeight==ChildrenSize.FitContainer)
					{
						ResizeHeightToFit(height, group);
					}
					if (ChildrenWidth==ChildrenSize.FitContainer)
					{
						ResizeColumnWidthToFit(width, group);
					}
				}
			}

			return group;
		}

		float GetRectWidth(RectTransform rect)
		{
			return rect.rect.width;
		}
		
		float GetRectHeight(RectTransform rect)
		{
			return rect.rect.height;
		}

		void ResizeWidthToFit(float width, List<List<RectTransform>> group)
		{
			foreach (var row in group)
			{
				float free_space = width - row.SumFloat(GetRectWidth) - ((row.Count - 1) * Spacing.x);
				if (free_space<=0f)
				{
					continue ;
				}
				
				var per_flexible = free_space / row.SumFloat(GetFlexibleWidth);
				foreach (var element in row)
				{
					var flexible = GetFlexibleWidth(element);
					element.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, element.rect.width + (per_flexible * flexible));
				}
			}
		}

		float GetMaxPreferredWidth(List<RectTransform> row)
		{
			return row.Max<RectTransform,float>(GetPreferredWidth);
		}

		float GetMaxFlexibleWidth(List<RectTransform> row)
		{
			return row.Max<RectTransform,float>(GetFlexibleWidth);
		}

		void ResizeColumnWidthToFit(float width, List<List<RectTransform>> group)
		{
			var transpore_group = Transpose(group);

			var row_preferred_widths = transpore_group.Convert<List<RectTransform>,float>(GetMaxPreferredWidth);
			var row_flexible_widths = transpore_group.Convert<List<RectTransform>,float>(GetMaxFlexibleWidth);
			float free_space = width - row_preferred_widths.Sum() - ((transpore_group.Count - 1) * Spacing.x)  - PaddingInner.Left - PaddingInner.Right;

			if (free_space<=0f)
			{
				return ;
			}
			var per_flexible = free_space / row_flexible_widths.Sum();

			int column_index = 0;
			foreach (var column in transpore_group)
			{
				foreach (var element in column)
				{
					element.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, row_preferred_widths[column_index] + (per_flexible * row_flexible_widths[column_index]));
				}
				column_index += 1;
			}
		}

		void ResizeHeightToFit(float height, List<List<RectTransform>> group)
		{
			foreach (var column in Transpose(group))
			{
				float free_space = height - column.SumFloat(GetRectHeight) - ((column.Count - 1) * Spacing.y);
				if (free_space<=0f)
				{
					continue ;
				}
				
				var per_flexible = free_space / column.SumFloat(GetFlexibleHeight);
				foreach (var element in column)
				{
					var flexible = Mathf.Max(1, GetFlexibleHeight(element));
					element.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, element.rect.height + (per_flexible * flexible));
				}
			}
		}
		
		float GetMaxPreferredHeight(List<RectTransform> column)
		{
			return column.Max<RectTransform,float>(GetPreferredWidth);
		}
		
		float GetMaxFlexibleHeight(List<RectTransform> column)
		{
			return column.Max<RectTransform,float>(GetFlexibleHeight);
		}

		void ResizeRowHeightToFit(float height, List<List<RectTransform>> group)
		{
			var row_preferred_heights = group.Convert<List<RectTransform>,float>(GetMaxPreferredHeight);
			var row_flexible_heights = group.Convert<List<RectTransform>,float>(GetMaxFlexibleHeight);
			float free_space = height - row_preferred_heights.Sum() - ((group.Count - 1) * Spacing.y) - PaddingInner.Top - PaddingInner.Bottom;

			if (free_space<=0f)
			{
				return ;
			}
			var per_flexible = free_space / row_flexible_heights.Sum();

			int row_index = 0;
			foreach (var row in group)
			{
				foreach (var element in row)
				{
					element.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, row_preferred_heights[row_index] + (per_flexible * row_flexible_heights[row_index]));
				}
				row_index += 1;
			}
		}

		/// <summary>
		/// Transpose the specified group.
		/// </summary>
		/// <param name="group">Group.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<List<T>> Transpose<T>(List<List<T>> group)
		{
			var result = new List<List<T>>();

			int i = 0;
			foreach (var row in group)
			{
				int j = 0;
				foreach (var element in row)
				{
					if (result.Count<=j)
					{
						result.Add(new List<T>());
					}
					result[j].Add(element);

					j += 1;
				}

				i += 1;
			}

			return result;
		}

		static void Log(IEnumerable<float> values)
		{
			Debug.Log("[" + string.Join("; ", values.Select(x => x.ToString()).ToArray()) + "]");
		}

		float ScaledWidth(RectTransform ui)
		{
			return ui.rect.width * ui.localScale.x;
		}

		float ScaledHeight(RectTransform ui)
		{
			return ui.rect.height * ui.localScale.y;
		}

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			Upgrade();
		}

		[SerializeField]
		int version = 0;

		#pragma warning disable 0618
		/// <summary>
		/// Upgrade to keep compatibility between versions.
		/// </summary>
		public virtual void Upgrade()
		{
			//upgrade to 1.6
			if (version==0)
			{
				if (ControlWidth)
				{
					ChildrenWidth = (MaxWidth) ? ChildrenSize.SetMaxFromPreferred : ChildrenSize.SetPreferred;
				}
				if (ControlHeight)
				{
					ChildrenHeight = (MaxHeight) ? ChildrenSize.SetMaxFromPreferred : ChildrenSize.SetPreferred;
				}
			}
			version = 1;
		}
		#pragma warning restore 0618
	}

	/// <summary>
	/// For each extensions.
	/// </summary>
	public static class EasyLayoutExtensions
	{
		public static float SumFloat<T>(this List<T> list, Func<T,float> calculate)
		{
			var result = 0f;
			for (int i = 0; i < list.Count; i++)
			{
				result += calculate(list[i]);
			}
			return result;
		}
		
		static public List<TOutput> Convert<TInput,TOutput>(this List<TInput> input, Converter<TInput,TOutput> converter)
		{
			#if NETFX_CORE
			var output = new List<TOutput>(input.Count);
			for (int i = 0; i < input.Count; i++)
			{
				output.Add(converter(input[i]));
			}
			
			return output;
			#else
			return input.ConvertAll<TOutput>(converter);
			#endif
		}
		
		#if NETFX_CORE
		static public void ForEach<T>(this List<T> list, Action<T> action)
		{
			for (int i = 0; i < list.Count; i++)
			{
				action(list[i]);
			}
		}
		#endif
	}
}