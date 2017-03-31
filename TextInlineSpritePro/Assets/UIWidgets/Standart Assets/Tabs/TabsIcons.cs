using UnityEngine;

namespace UIWidgets
{
	/// <summary>
	/// TabsIcons.
	/// </summary>
	[AddComponentMenu("UI/TabsIcons", 295)]
	public class TabsIcons : TabsCustom<TabIcons,TabIconButton>
	{
		/// <summary>
		/// Sets the name of the button.
		/// </summary>
		/// <param name="button">Button.</param>
		/// <param name="index">Index.</param>
		protected override void SetButtonData(TabIconButton button, int index)
		{
			button.SetData(TabObjects[index]);
		}
		
		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/TabsIcons", false, 1185)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("TabsIcons");
		}
		#endif
	}
}