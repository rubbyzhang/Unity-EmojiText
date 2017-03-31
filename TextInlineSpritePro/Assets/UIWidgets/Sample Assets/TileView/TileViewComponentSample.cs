using UnityEngine;
using UnityEngine.UI;
using System;
using UIWidgets;

namespace UIWidgetsSamples {
	/// <summary>
	/// ListViewIcons item component.
	/// </summary>
	public class TileViewComponentSample : ListViewItem {
		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;
		
		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		public Text Name;

		[SerializeField]
		public Text Capital;

		[SerializeField]
		public Text Area;

		[SerializeField]
		public Text Population;

		[SerializeField]
		public Text Density;

		/// <summary>
		/// Set icon native size.
		/// </summary>
		public bool SetNativeSize = true;

		public TileViewSample Tiles;

		public void Remove()
		{
			Tiles.DataSource.RemoveAt(Index);
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(TileViewItemSample item)
		{
			if (item==null)
			{
				Icon.sprite = null;
				Name.text = string.Empty;
				Capital.text = string.Empty;
				Area.text = string.Empty;
				Population.text = string.Empty;
				Density.text = string.Empty;
			}
			else
			{
				Icon.sprite = item.Icon;
				Name.text = item.Name;
				Capital.text = "Capital: " + item.Capital;
				Area.text = "Area: " + item.Area.ToString("N0") + " sq. km";
				Population.text = "Population: " + item.Population.ToString("N0");
				var density = item.Area==0 ? "n/a" : Mathf.CeilToInt(item.Population / item.Area).ToString("N") + " / sq. km";
				Density.text = "Density: " + density;
			}
			
			if (SetNativeSize)
			{
				Icon.SetNativeSize();
			}
			
			//set transparent color if no icon
			Icon.color = (Icon.sprite==null) ? Color.clear : Color.white;
		}
	}
}