using UnityEngine;
using UnityEngine.UI;
using UIWidgets;

namespace UIWidgetsSamples {
	public class ListViewVariableHeightComponent : ListViewItem, IListViewItemHeight {
		[SerializeField]
		public Text Name;

		[SerializeField]
		public Text Text;

		public float Height {
			get {
				return CalculateHeight();
			}
		}

		// Displaying item data
		public void SetData(ListViewVariableHeightItemDescription item)
		{
			Name.text = item.Name;
			Text.text = item.Text.Replace("\\n", "\n");
		}

		float CalculateHeight()
		{
			float default_total_height = 63;
			float default_name_height = 21;
			float default_text_height = 17;

			var base_height = default_total_height - default_name_height - default_text_height;

			var h = base_height + Name.preferredHeight + Text.preferredHeight;

			return h;
		}
	}
}