using UnityEngine;
using System.Collections;
using UIWidgets;

namespace UIWidgetsSamples.Shops {
	public class TraderListView : ListViewCustom<TraderListViewComponent,JRPGOrderLine> {
		protected override void SetData(TraderListViewComponent component, JRPGOrderLine item)
		{
			component.SetData(item);
		}
		
		protected override void HighlightColoring(TraderListViewComponent component)
		{
			base.HighlightColoring(component);
			component.Name.color = HighlightedColor;
			component.Price.color = HighlightedColor;
			component.AvailableCount.color = HighlightedColor;
		}
		
		protected override void SelectColoring(TraderListViewComponent component)
		{
			base.SelectColoring(component);
			component.Name.color = SelectedColor;
			component.Price.color = SelectedColor;
			component.AvailableCount.color = SelectedColor;
		}
		
		protected override void DefaultColoring(TraderListViewComponent component)
		{
			base.DefaultColoring(component);
			component.Name.color = DefaultColor;
			component.Price.color = DefaultColor;
			component.AvailableCount.color = DefaultColor;
		}
	}
}