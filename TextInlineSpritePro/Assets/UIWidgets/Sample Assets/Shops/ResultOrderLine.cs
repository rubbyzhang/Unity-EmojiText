using UnityEngine;
using System.Collections;

namespace UIWidgetsSamples.Shops {
	public class ResultOrderLine : IOrderLine {
		public Item Item { get; set; }
		public int Count { get; set; }
	}
}