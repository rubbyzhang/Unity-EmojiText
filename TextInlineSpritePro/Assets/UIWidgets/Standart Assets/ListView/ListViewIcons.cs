using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UIWidgets {

	[System.Serializable]
	/// <summary>
	/// ListViewIcons item description.
	/// </summary>
	public class ListViewIconsItemDescription {
		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Sprite Icon;

		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public string Name;

		/// <summary>
		/// The localized name.
		/// </summary>
		[System.NonSerialized]
		public string LocalizedName;

		/// <summary>
		/// The value.
		/// </summary>
		[SerializeField]
		public int Value;
	}

	/// <summary>
	/// ListViewIcons.
	/// </summary>
	[AddComponentMenu("UI/ListViewIcons", 252)]
	public class ListViewIcons : ListViewCustom<ListViewIconsItemComponent,ListViewIconsItemDescription> {
		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected override void Awake()
		{
			Start();
		}

		[System.NonSerialized]
		bool isStartedListViewIcons = false;

		protected Comparison<ListViewIconsItemDescription> ItemsComparison =
			(x, y) => (x.LocalizedName ?? x.Name).CompareTo(y.LocalizedName ?? y.Name);

		/// <summary>
		/// Start this instance.
		/// </summary>
		public override void Start()
		{
			if (isStartedListViewIcons)
			{
				return ;
			}
			isStartedListViewIcons = true;

			base.Start();
			SortFunc = list => list.OrderBy(item => item.LocalizedName ?? item.Name);
			//DataSource.Comparison = ItemsComparison;
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected override void SetData(ListViewIconsItemComponent component, ListViewIconsItemDescription item)
		{
			component.SetData(item);
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(ListViewIconsItemComponent component)
		{
			base.HighlightColoring(component);
			component.Text.color = HighlightedColor;
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void SelectColoring(ListViewIconsItemComponent component)
		{
			base.SelectColoring(component);
			component.Text.color = SelectedColor;
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void DefaultColoring(ListViewIconsItemComponent component)
		{
			base.DefaultColoring(component);
			component.Text.color = DefaultColor;
		}

		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/ListViewIcons", false, 1080)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("ListViewIcons");
		}
		#endif
	}
}