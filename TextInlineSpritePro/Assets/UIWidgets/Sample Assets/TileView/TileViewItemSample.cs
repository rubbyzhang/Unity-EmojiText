using UnityEngine;
using System.Collections;

namespace UIWidgetsSamples {
	[System.Serializable]
	/// <summary>
	/// ListViewIcons item description.
	/// </summary>
	public class TileViewItemSample {
		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Sprite Icon;
		
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public string Name;

		[SerializeField]
		public string Capital;

		[SerializeField]
		public int Area;

		[SerializeField]
		public int Population;
	}
}