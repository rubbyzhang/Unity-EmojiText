using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;

namespace UIWidgetsSamples.Shops {
	public class JRPGOrder : IOrder {
		List<JRPGOrderLine> OrderLines;

		public JRPGOrder(ObservableList<JRPGOrderLine> orderLines)
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
			return OrderLines.Sum(x => x.Count * x.Price);
		}
	}
}