using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples {
	public class ListViewUnderlineSampleComponent : ListViewItem {
		// specify components for displaying item data
		[SerializeField]
		public Image Icon;
		
		[SerializeField]
		public Text Text;
		
		[SerializeField]
		public Image Underline;
		
		// Displaying item data
		public void SetData(ListViewUnderlineSampleItemDescription item)
		{
			if (item==null)
			{
				Icon.sprite = null;
				Text.text = string.Empty;
			}
			else
			{
				Icon.sprite = item.Icon;
				Text.text = item.Name;
			}
			
			Icon.SetNativeSize();
			//set transparent color if no icon
			Icon.color = (Icon.sprite==null) ? Color.clear : Color.white;
		}
	}
}