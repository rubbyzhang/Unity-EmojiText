using UnityEngine;

namespace UIWidgets {
	/// <summary>
	/// TreeView.
	/// </summary>
	[AddComponentMenu("UI/TreeView", 252)]
	public class TreeView : TreeViewCustom<TreeViewComponent,TreeViewItem> {

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected override void SetData(TreeViewComponent component, ListNode<TreeViewItem> item)
		{
			component.SetData(item.Node, item.Depth);
		}
		
		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(TreeViewComponent component)
		{
			base.HighlightColoring(component);
			component.Text.color = HighlightedColor;
		}
		
		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void SelectColoring(TreeViewComponent component)
		{
			base.SelectColoring(component);
			component.Text.color = SelectedColor;
		}
		
		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void DefaultColoring(TreeViewComponent component)
		{
			if (component==null)
			{
				return ;
			}
			base.DefaultColoring(component);
			if (component.Text!=null)
			{
				component.Text.color = DefaultColor;
			}
		}
		
		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/TreeView", false, 1190)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("TreeView");
		}
		#endif
	}
}