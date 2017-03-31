using UnityEngine;
using UnityEngine.UI;

namespace UIWidgets {
	/// <summary>
	/// Combobox with icons.
	/// </summary>
	[AddComponentMenu("UI/ComboboxIcons", 230)]
	public class ComboboxIcons : ComboboxCustom<ListViewIcons,ListViewIconsItemComponent,ListViewIconsItemDescription>
	{
		/// <summary>
		/// Updates the current component with selected item.
		/// </summary>
		protected override void SetData(ListViewIconsItemComponent component, ListViewIconsItemDescription item)
		{
			component.SetData(item);
		}

		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/ComboboxIcons", false, 1040)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("ComboboxIcons");
		}

		[UnityEditor.MenuItem("GameObject/UI/ComboboxIconsMultiselect", false, 1041)]
		static void CreateObjectMultiselect()
		{
			Utilites.CreateWidgetFromAsset("ComboboxIconsMultiselect");
		}
		#endif
	}
}