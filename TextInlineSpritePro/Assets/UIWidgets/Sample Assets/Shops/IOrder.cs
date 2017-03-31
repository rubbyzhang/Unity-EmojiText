using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIWidgetsSamples.Shops {
	public interface IOrder {
		List<IOrderLine> GetOrderLines();
		int OrderLinesCount();
		int Total();
	}
}