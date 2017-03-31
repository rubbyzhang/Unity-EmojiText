using UnityEngine;
using System.Collections.Generic;
using System;
using UIWidgets;

namespace UIWidgetsSamples {
	
	[System.Serializable]
	public class ListViewUnderlineSampleItemDescription {
		[SerializeField]
		public Sprite Icon;
		[SerializeField]
		public string Name;
	}
	
	public class ListViewUnderlineSample : ListViewCustom<ListViewUnderlineSampleComponent,ListViewUnderlineSampleItemDescription> {
		bool isStartedListViewCustomSample = false;

		Comparison<ListViewUnderlineSampleItemDescription> itemsComparison = (x, y) => {
			return x.Name.CompareTo(y.Name);
		};

		protected override void Awake()
		{
			Start();
		}
		
		public override void Start()
		{
			if (isStartedListViewCustomSample)
			{
				return ;
			}
			isStartedListViewCustomSample = true;
			
			base.Start();
			DataSource.Comparison = itemsComparison;
		}
		
		protected override void SetData(ListViewUnderlineSampleComponent component, ListViewUnderlineSampleItemDescription item)
		{
			component.SetData(item);
		}
		
		protected override void HighlightColoring(ListViewUnderlineSampleComponent component)
		{
			component.Underline.color = HighlightedColor;
			component.Text.color = HighlightedColor;
		}
		
		protected override void SelectColoring(ListViewUnderlineSampleComponent component)
		{
			component.Underline.color = SelectedColor;
			component.Text.color = SelectedColor;
		}
		
		protected override void DefaultColoring(ListViewUnderlineSampleComponent component)
		{
			component.Underline.color = DefaultColor;
			component.Text.color = DefaultColor;
		}
	}
}