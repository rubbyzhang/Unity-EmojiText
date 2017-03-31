using UnityEngine;

namespace UIWidgets {
	[AddComponentMenu("UI/CenteredSliderVertical", 300)]
	/// <summary>
	/// Centered vertical slider (zero at center, positive and negative parts have different scales).
	/// </summary>
	public class CenteredSliderVertical : CenteredSlider
	{
		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		protected override bool IsHorizontal ()
		{
			return false;
		}
		
		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/CenteredSliderVertical", false, 1020)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("CenteredSliderVertical");
		}
		#endif
	}
}