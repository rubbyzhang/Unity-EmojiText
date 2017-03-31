using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIWidgetsSamples.Shops {
	public interface IOrderLine {
		Item Item {get; set; }
		int Count {get; set; }
	}
}