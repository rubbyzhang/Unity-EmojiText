using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UIWidgets;

namespace UIWidgetsSamples.Shops {
	public class TraderListViewComponent : ListViewItem {
		[SerializeField]
		public Text Name;

		[SerializeField]
		public Text Price;

		[SerializeField]
		public Text AvailableCount;

		[SerializeField]
		Spinner Count;

		JRPGOrderLine OrderLine;

		protected override void Start()
		{
			Count.onValueChangeInt.AddListener(ChangeCount);
			base.Start();
		}

		public override void OnMove (AxisEventData eventData)
		{
			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
					Count.Value -= 1;
					break;
				case MoveDirection.Right:
					Count.Value += 1;
					break;
				default:
					base.OnMove(eventData);
					break;
			}

		}

		public void SetData(JRPGOrderLine orderLine)
		{
			OrderLine = orderLine;

			Name.text = OrderLine.Item.Name;
			Price.text = OrderLine.Price.ToString();
			AvailableCount.text = (OrderLine.Item.Count==-1) ? "∞" : OrderLine.Item.Count.ToString();

			Count.Value = 0;
			Count.Min = 0;
			Count.Max = (OrderLine.Item.Count==-1) ? 9999 : OrderLine.Item.Count;
		}

		void ChangeCount(int count)
		{
			OrderLine.Count = count;
		}

		protected override void OnDestroy()
		{
			if (Count!=null)
			{
				Count.onValueChangeInt.RemoveListener(ChangeCount);
			}
		}
	}
}