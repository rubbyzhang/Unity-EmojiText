using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples {
	public class ListViewCustomSampleComponent : ListViewItem {
		// specify components for displaying item data
		[SerializeField]
		public Image Icon;

		[SerializeField]
		public Text Text;

		[SerializeField]
		public Progressbar Progressbar;

		// Displaying item data
		public void SetData(ListViewCustomSampleItemDescription item)
		{
			if (item==null)
			{
				Icon.sprite = null;
				Text.text = string.Empty;
				Progressbar.Value = 0;
			}
			else
			{
				Icon.sprite = item.Icon;
				Text.text = item.Name;
				Progressbar.Value = item.Progress;
			}

			Icon.SetNativeSize();
			//set transparent color if no icon
			Icon.color = (Icon.sprite==null) ? Color.clear : Color.white;
		}
	}
}