using UnityEngine;
using System.Collections.Generic;
using System;
using UIWidgets;

namespace UIWidgetsSamples {
	/// <summary>
	/// TileViewSample.
	/// </summary>
	public class TileViewSample : TileView<TileViewComponentSample,TileViewItemSample> {
		bool isStartedTileViewSample = false;

		Comparison<TileViewItemSample> itemsComparison = (x, y) => {
			return x.Name.CompareTo(y.Name);
		};

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected override void Awake()
		{
			Start();
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		public override void Start()
		{
			if (isStartedTileViewSample)
			{
				return ;
			}
			isStartedTileViewSample = true;
			
			base.Start();
			DataSource.Comparison = itemsComparison;
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="item">Item.</param>
		protected override void SetData(TileViewComponentSample component, TileViewItemSample item)
		{
			component.SetData(item);
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(TileViewComponentSample component)
		{
			base.HighlightColoring(component);
			component.Name.color = HighlightedColor;
			component.Capital.color = HighlightedColor;
			component.Area.color = HighlightedColor;
			component.Population.color = HighlightedColor;
			component.Density.color = HighlightedColor;
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void SelectColoring(TileViewComponentSample component)
		{
			base.SelectColoring(component);
			component.Name.color = SelectedColor;
			component.Capital.color = SelectedColor;
			component.Area.color = SelectedColor;
			component.Population.color = SelectedColor;
			component.Density.color = SelectedColor;
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void DefaultColoring(TileViewComponentSample component)
		{
			base.DefaultColoring(component);
			component.Name.color = DefaultColor;
			component.Capital.color = DefaultColor;
			component.Area.color = DefaultColor;
			component.Population.color = DefaultColor;
			component.Density.color = DefaultColor;
		}
	}
}