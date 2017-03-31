using UnityEngine;
using UIWidgets;

namespace UIWidgetsSamples {
	public class TreeViewSample : TreeViewCustom<TreeViewSampleComponent,ITreeViewSampleItem> {
		
		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected override void SetData(TreeViewSampleComponent component, ListNode<ITreeViewSampleItem> item)
		{
			component.SetData(item.Node, item.Depth);
		}
		
		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(TreeViewSampleComponent component)
		{
			base.HighlightColoring(component);
			component.Text.color = HighlightedColor;
		}
		
		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void SelectColoring(TreeViewSampleComponent component)
		{
			base.SelectColoring(component);
			component.Text.color = SelectedColor;
		}
		
		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void DefaultColoring(TreeViewSampleComponent component)
		{
			base.DefaultColoring(component);
			component.Text.color = DefaultColor;
		}
	}
}