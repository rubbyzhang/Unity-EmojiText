using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;

namespace UIWidgetsSamples.Shops {
	public class HarborOrder : IOrder {
		List<HarborOrderLine> OrderLines;

		public HarborOrder(ObservableList<HarborOrderLine> orderLines)
		{
			OrderLines = orderLines.Where(x => x.Count != 0).ToList();
		}

		public List<IOrderLine> GetOrderLines()
		{
			return OrderLines.Convert(x => x as IOrderLine);
		}

		public int OrderLinesCount()
		{
			return OrderLines.Count;
		}

		public int Total()
		{
			return OrderLines.Sum(x => x.Count * ((x.Count > 0) ? x.BuyPrice : x.SellPrice));
		}
	}
}