using UnityEngine;
using System.Collections;
using UIWidgets;

namespace UIWidgetsSamples.ToDoList {
	public class ToDoListView : ListViewCustomHeight<ToDoListViewComponent,ToDoListItem> {
		bool isStarted = false;

		protected override void Awake()
		{
			Start();
		}

		public override void Start()
		{
			if (isStarted)
			{
				return ;
			}
			isStarted = true;

			base.Start();
		}
			
		protected override void SetData(ToDoListViewComponent component, ToDoListItem item)
		{
			component.SetData(item);
		}
			
		protected override void HighlightColoring(ToDoListViewComponent component)
		{
			base.HighlightColoring(component);
			component.Task.color = HighlightedColor;
		}
			
		protected override void SelectColoring(ToDoListViewComponent component)
		{
			base.SelectColoring(component);
			component.Task.color = SelectedColor;
		}
			
		protected override void DefaultColoring(ToDoListViewComponent component)
		{
			base.DefaultColoring(component);
			component.Task.color = DefaultColor;
		}
	}
}