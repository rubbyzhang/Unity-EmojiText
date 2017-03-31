using UnityEngine;

namespace UIWidgets
{
	[AddComponentMenu("UI/RangeSliderFloatVertical", 300)]
	/// <summary>
	/// Vertical range slider.
	/// </summary>
	public class RangeSliderFloatVertical : RangeSliderFloat
	{
		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		protected override bool IsHorizontal ()
		{
			return false;
		}
		
		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/RangeSliderFloatVertical", false, 1140)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("RangeSliderFloatVertical");
		}
		#endif
	}
}