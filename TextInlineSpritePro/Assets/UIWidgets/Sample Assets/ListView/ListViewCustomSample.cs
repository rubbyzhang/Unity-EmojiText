using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UIWidgets;

namespace UIWidgetsSamples {

	[System.Serializable]
	public class ListViewCustomSampleItemDescription {
		//specify data fields
		[SerializeField]
		public Sprite Icon;
		[SerializeField]
		public string Name;
		[SerializeField]
		public int Progress;

		// Serves as a hash function for a particular type.
		public override int GetHashCode()
		{
			return Icon.GetHashCode() ^ Name.GetHashCode() ^ Progress;
		}

		// Determines whether the specified object is equal to the current object.
		public override bool Equals(System.Object obj)
		{
			ListViewCustomSampleItemDescription descObj = obj as ListViewCustomSampleItemDescription; 
			if (descObj == null)
			{
				return false;
			}
			if (((descObj.Icon==null) && (Icon!=null)) || ((descObj.Icon!=null) && (Icon==null)))
			{
				return false;
			}
			return Name==descObj.Name && Progress==descObj.Progress && Icon.Equals(descObj.Icon);
		}
	}

	public class ListViewCustomSample : ListViewCustom<ListViewCustomSampleComponent,ListViewCustomSampleItemDescription> {
		bool isStartedListViewCustomSample = false;

		Comparison<ListViewCustomSampleItemDescription> itemsComparison = (x, y) => {
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

		protected override void SetData(ListViewCustomSampleComponent component, ListViewCustomSampleItemDescription item)
		{
			component.SetData(item);
		}

		protected override void HighlightColoring(ListViewCustomSampleComponent component)
		{
			base.HighlightColoring(component);
			component.Text.color = HighlightedColor;
		}

		protected override void SelectColoring(ListViewCustomSampleComponent component)
		{
			base.SelectColoring(component);
			component.Text.color = SelectedColor;
		}

		protected override void DefaultColoring(ListViewCustomSampleComponent component)
		{
			base.DefaultColoring(component);
			component.Text.color = DefaultColor;
		}
	}
}