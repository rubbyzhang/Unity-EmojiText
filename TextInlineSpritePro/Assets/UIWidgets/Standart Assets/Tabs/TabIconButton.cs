using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace UIWidgets {
	/// <summary>
	/// TabIconButton.
	/// </summary>
	public class TabIconButton : TabButton {
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public Text Name;

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// The size of the set native.
		/// </summary>
		[SerializeField]
		public bool SetNativeSize;

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public virtual void SetData(TabIcons tab)
		{

		}
	}
}