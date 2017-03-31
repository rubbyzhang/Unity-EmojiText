using UnityEngine;
using UnityEngine.UI;
using UIWidgets;
using System;

namespace UIWidgetsSamples.ToDoList {
	public class ToDoListViewComponent : ListViewItem, IListViewItemHeight {
		[SerializeField]
		public Toggle Toggle;
		
		[SerializeField]
		public Text Task;
		
		[NonSerialized]
		public ToDoListItem Item;

		public float Height {
			get {
				return CalculateHeight();
			}
		}

		protected override void Start()
		{
			base.Start();
			Toggle.onValueChanged.AddListener(OnToggle);
		}

		void OnToggle(bool toggle)
		{
			Item.Done = toggle;
		}

		public void SetData(ToDoListItem item)
		{
			Item = item;

			if (Item==null)
			{
				Toggle.isOn = false;
				Task.text = string.Empty;
			}
			else
			{
				Toggle.isOn = Item.Done;
				Task.text = Item.Task.Replace("\\n", "\n");
			}
		}

		LayoutGroup layoutGroup;

		public LayoutGroup LayoutGroup {
			get {
				if (layoutGroup==null)
				{
					layoutGroup = GetComponent<LayoutGroup>();
				}
				return layoutGroup;
			}
		}

		float CalculateHeight()
		{
			gameObject.SetActive(true);

			LayoutGroup.CalculateLayoutInputHorizontal();
			LayoutGroup.SetLayoutHorizontal();
			LayoutGroup.CalculateLayoutInputVertical();
			LayoutGroup.SetLayoutVertical();

			var h = LayoutUtility.GetPreferredHeight(transform as RectTransform);

			gameObject.SetActive(false);

			return h;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (Toggle!=null)
			{
				Toggle.onValueChanged.RemoveListener(OnToggle);
			}
		}
	}
}