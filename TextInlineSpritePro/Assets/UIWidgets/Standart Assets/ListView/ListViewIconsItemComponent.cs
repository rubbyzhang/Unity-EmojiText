using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace UIWidgets {
	/// <summary>
	/// ListViewIcons item component.
	/// </summary>
	public class ListViewIconsItemComponent : ListViewItem, IResizableItem {
		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		public GameObject[] ObjectsToResize {
			get {
				return new GameObject[] {Icon.transform.parent.gameObject, Text.gameObject};
			}
		}

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		public Text Text;

		/// <summary>
		/// Set icon native size.
		/// </summary>
		public bool SetNativeSize = true;

		ListViewIconsItemDescription item;

		/// <summary>
		/// Gets the current item.
		/// </summary>
		/// <value>Current item.</value>
		public ListViewIconsItemDescription Item {
			get {
				return item;
			}
		}
		
		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="newItem">Item.</param>
		public virtual void SetData(ListViewIconsItemDescription newItem)
		{
			item = newItem;
			if (item==null)
			{
				if (Icon!=null)
				{
					Icon.sprite = null;
				}
				Text.text = string.Empty;
			}
			else
			{
				if (Icon!=null)
				{
					Icon.sprite = item.Icon;
				}
				Text.text = item.LocalizedName ?? item.Name;
			}

			if ((SetNativeSize) && (Icon!=null))
			{
				Icon.SetNativeSize();
			}

			//set transparent color if no icon
			if (Icon!=null)
			{
				Icon.color = (Icon.sprite==null) ? Color.clear : Color.white;
			}
		}

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public override void MovedToCache()
		{
			if (Icon!=null)
			{
				Icon.sprite = null;
			}
		}
	}
}