using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EasyLayout {
	/// <summary>
	/// EasyLayout compact layout.
	/// </summary>
	public class EasyLayoutCompact {

		/// <summary>
		/// Group the specified uiElements.
		/// </summary>
		/// <param name="uiElements">User interface elements.</param>
		/// <param name="baseLength">Base length (width or height).</param>
		/// <param name="layout">Layout.</param>
		public static List<List<RectTransform>> Group(List<RectTransform> uiElements, float baseLength, EasyLayout layout)
		{
			var length = baseLength;
			
			var spacing = (layout.Stacking==Stackings.Horizontal) ? layout.Spacing.x : layout.Spacing.y;
			
			var group = new List<List<RectTransform>>();
			
			var row = new List<RectTransform>();
			
			foreach (var ui_element in uiElements)
			{
				var ui_length = layout.GetLength(ui_element);
				
				if (row.Count == 0)
				{
					length -= ui_length;
					row.Add(ui_element);
					continue;
				}
				
				if (length >= (ui_length + spacing))
				{
					length -= ui_length + spacing;
					row.Add(ui_element);
				}
				else
				{
					group.Add(row);
					length = baseLength;
					length -= ui_length;
					row = new List<RectTransform>();
					row.Add(ui_element);
				}
			}
			if (row.Count > 0)
			{
				group.Add(row);
			}
			
			return group;
		}

	}
}