using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace UIWidgets {
	/// <summary>
	/// ListViewIcons drop support.
	/// Receive drops from TreeView and ListViewIcons.
	/// </summary>
	[RequireComponent(typeof(ListViewIcons))]
	public class ListViewIconsDropSupport : MonoBehaviour, IDropSupport<TreeNode<TreeViewItem>>, IDropSupport<ListViewIconsItemDescription> {

		#region IDropSupport<TreeNode<TreeViewItem>>
		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
			return data.Nodes==null || data.Nodes.Count==0;
		}

		/// <summary>
		/// Process dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
			var listView = GetComponent<ListViewIcons>();

			listView.DataSource.Add(new ListViewIconsItemDescription() {
				Name = data.Item.Name,
				LocalizedName = data.Item.LocalizedName,
				Icon = data.Item.Icon,
				Value = data.Item.Value
			});

			// remove node from tree
			data.Parent = null;
		}

		/// <summary>
		/// Process canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
		}
		#endregion

		#region IDropSupport<ListViewIconsItemDescription>
		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(ListViewIconsItemDescription data, PointerEventData eventData)
		{
			// sorted = listView.DataSource.Comparison //support for ListView.Sort?
			// if sorted then use BinarySearch to find index
			// else convert eventData to index
			// if index > 0 then index--
			// index to position -> GetItemPositionBottom(index)
			// show line in position according ListView.Direction

			return true;
		}

		/// <summary>
		/// Process dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(ListViewIconsItemDescription data, PointerEventData eventData)
		{
			var listView = GetComponent<ListViewIcons>();

			listView.DataSource.Add(data);
		}

		/// <summary>
		/// Process canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(ListViewIconsItemDescription data, PointerEventData eventData)
		{
			// hide line from CanReceiveDrop()
		}
		#endregion
	}
}