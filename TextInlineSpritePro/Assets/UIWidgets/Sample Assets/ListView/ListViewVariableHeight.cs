using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;

namespace UIWidgetsSamples {
	
	[System.Serializable]
	public class ListViewVariableHeightItemDescription : IItemHeight {
		[SerializeField]
		public string Name;

		[SerializeField]
		public string Text;

		[SerializeField]
		float height;

		public float Height {
			get {
				return height;
			}
			set {
				height = value;
			}
		}
	}
	
	public class ListViewVariableHeight : ListViewCustomHeight<ListViewVariableHeightComponent,ListViewVariableHeightItemDescription> {
		protected override void SetData(ListViewVariableHeightComponent component, ListViewVariableHeightItemDescription item)
		{
			component.SetData(item);
		}
		
		protected override void HighlightColoring(ListViewVariableHeightComponent component)
		{
			base.HighlightColoring(component);
			component.Name.color = HighlightedColor;
			component.Text.color = HighlightedColor;
		}
		
		protected override void SelectColoring(ListViewVariableHeightComponent component)
		{
			base.SelectColoring(component);
			component.Name.color = SelectedColor;
			component.Text.color = SelectedColor;
		}
		
		protected override void DefaultColoring(ListViewVariableHeightComponent component)
		{
			base.DefaultColoring(component);
			component.Name.color = DefaultColor;
			component.Text.color = DefaultColor;
		}
	}
}