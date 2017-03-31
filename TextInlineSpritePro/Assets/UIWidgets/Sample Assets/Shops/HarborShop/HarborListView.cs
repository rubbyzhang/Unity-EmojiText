using UnityEngine;
using System.Collections;
using UIWidgets;

namespace UIWidgetsSamples.Shops {
	public class HarborListView : ListViewCustom<HarborListViewComponent,HarborOrderLine> {
		protected override void SetData(HarborListViewComponent component, HarborOrderLine item)
		{
			component.SetData(item);
		}
		
		protected override void HighlightColoring(HarborListViewComponent component)
		{
			base.HighlightColoring(component);
			component.Name.color = HighlightedColor;
			component.BuyPrice.color = HighlightedColor;
			component.SellPrice.color = HighlightedColor;
			component.AvailableBuyCount.color = HighlightedColor;
			component.AvailableSellCount.color = HighlightedColor;
		}
		
		protected override void SelectColoring(HarborListViewComponent component)
		{
			base.SelectColoring(component);
			component.Name.color = SelectedColor;
			component.BuyPrice.color = SelectedColor;
			component.SellPrice.color = SelectedColor;
			component.AvailableBuyCount.color = SelectedColor;
			component.AvailableSellCount.color = SelectedColor;
		}
		
		protected override void DefaultColoring(HarborListViewComponent component)
		{
			base.DefaultColoring(component);
			component.Name.color = DefaultColor;
			component.BuyPrice.color = DefaultColor;
			component.SellPrice.color = DefaultColor;
			component.AvailableBuyCount.color = DefaultColor;
			component.AvailableSellCount.color = DefaultColor;
		}
	}
}