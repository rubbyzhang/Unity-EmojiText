using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIWidgets;

namespace UIWidgetsSamples.Shops {
	
	[System.Serializable]
	public class HarborOrderLine : IOrderLine {
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
		public int BuyPrice;

		[SerializeField]
		public int SellPrice;

		[SerializeField]
		public int BuyCount;
		
		[SerializeField]
		public int SellCount;

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
		
		public HarborOrderLine(Item newItem, int buyPrice, int sellPrice, int buyCount, int sellCount)
		{
			item = newItem;
			BuyPrice = buyPrice;
			SellPrice = sellPrice;
			BuyCount = buyCount;
			SellCount = sellCount;
		}
	}
}