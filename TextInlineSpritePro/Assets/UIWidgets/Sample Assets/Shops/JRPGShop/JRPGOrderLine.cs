using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;

namespace UIWidgetsSamples.Shops {

	[System.Serializable]
	public class JRPGOrderLine : IOrderLine {
		[SerializeField]
		Item item;

		public Item Item {
			get {
				return item;
			}
			set {
				item = value;
			}
		}

		[SerializeField]
		public int Price;

		[SerializeField]
		int count;

		public int Count {
			get {
				return count;
			}
			set {
				count = value;
			}
		}

		public JRPGOrderLine(Item newItem, int newPrice)
		{
			item = newItem;
			Price = newPrice;
		}
	}
}