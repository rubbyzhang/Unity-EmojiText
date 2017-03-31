using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace EasyLayout {
	/// <summary>
	/// Easy layout grid layout.
	/// </summary>
	public class EasyLayoutGrid {

		/// <summary>
		/// Gets the max columns count.
		/// </summary>
		/// <returns>The max columns count.</returns>
		/// <param name="uiElements">User interface elements.</param>
		/// <param name="baseLength">Base length.</param>
		/// <param name="layout">Layout.</param>
		/// <param name="maxColumns">Max columns.</param>
		public static int GetMaxColumnsCount(List<RectTransform> uiElements, float baseLength, EasyLayout layout, int maxColumns)
		{
			var length = baseLength;
			var spacing = (layout.Stacking==Stackings.Horizontal) ? layout.Spacing.x : layout.Spacing.y;
			
			bool min_columns_setted = false;
			int min_columns = maxColumns;
			int current_columns = 0;
			
			foreach (var ui_element in uiElements)
			{
				var ui_length = layout.GetLength(ui_element);
				
				if (current_columns==maxColumns)
				{
					min_columns_setted = true;
					min_columns = Mathf.Min(min_columns, current_columns);
					
					current_columns = 1;
					length = baseLength - ui_length;
					continue;
				}
				if (current_columns == 0)
				{
					current_columns = 1;
					length = baseLength - ui_length;
					continue;
				}
				
				if (length >= (ui_length + spacing))
				{
					length -= ui_length + spacing;
					current_columns++;
				}
				else
				{
					min_columns_setted = true;
					min_columns = Mathf.Min(min_columns, current_columns);
					
					current_columns = 1;
					length = baseLength - ui_length;
				}
			}
			if (!min_columns_setted)
			{
				min_columns = current_columns;
			}
			
			return min_columns;
		}

		/// <summary>
		/// Group the specified uiElements.
		/// </summary>
		/// <param name="uiElements">User interface elements.</param>
		/// <param name="baseLength">Base length (width or size).</param>
		/// <param name="layout">Layout.</param>
		/// <param name="maxColumns">Max columns.</param>
		public static List<List<RectTransform>> Group(List<RectTransform> uiElements, float baseLength, EasyLayout layout, int maxColumns=0)
		{
			int max_columns = 999999;
			while (true)
			{
				var new_max_columns = GetMaxColumnsCount(uiElements, baseLength, layout, max_columns);
				
				if ((max_columns==new_max_columns) || (new_max_columns==1))
				{
					max_columns = new_max_columns;
					break;
				}
				max_columns = new_max_columns;
			}
			
			var group = new List<List<RectTransform>>();
			var row = new List<RectTransform>();
			
			int i = 0;
			foreach (var ui_element in uiElements)
			{
				if ((i > 0) && ((i % max_columns)==0))
				{
					group.Add(row);
					row = new List<RectTransform>();
				}
				row.Add(ui_element);
				
				i++;
			}
			if (row.Count > 0)
			{
				group.Add(row);
			}
			
			return group;
		}
	}
}