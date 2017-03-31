using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace UIWidgets {
	/// <summary>
	/// Layout bridge to Horizontal or Vertical Layout Group.
	/// </summary>
	public class StandardLayoutBridge : ILayoutBridge {
		
		bool isHorizontal;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is horizontal.
		/// StandardLayoutBridge not support changes.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		public bool IsHorizontal {
			get {
				return isHorizontal;
			}
			set {
				throw new NotSupportedException("HorizontalLayoutGroup Or VerticalLayoutGroup direction cannot be change in runtime.");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UIWidgets.StandardLayoutBridge"/> update content size fitter.
		/// </summary>
		/// <value><c>true</c> if update content size fitter; otherwise, <c>false</c>.</value>
		public bool UpdateContentSizeFitter {
			get; set;
		}
		
		HorizontalOrVerticalLayoutGroup Layout;
		
		RectTransform DefaultItem;
		
		LayoutElement FirstFiller;
		
		LayoutElement LastFiller;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.StandardLayoutBridge"/> class.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="defaultItem">Default item.</param>
		public StandardLayoutBridge(HorizontalOrVerticalLayoutGroup layout, RectTransform defaultItem)
		{
			Utilites.UpdateLayout(layout);
			
			Layout = layout;
			DefaultItem = defaultItem;
			
			isHorizontal = layout is HorizontalLayoutGroup;
			
			var firstFillerGO = new GameObject("FirstFiller");
			var firstFillerTransform = (firstFillerGO.transform as RectTransform) ?? firstFillerGO.AddComponent<RectTransform>();
			firstFillerTransform.SetParent(Layout.transform);
			firstFillerTransform.localScale = Vector3.one;
			FirstFiller = firstFillerGO.AddComponent<LayoutElement>();
			
			var lastFillerGO = new GameObject("LastFiller");
			var lastFillerTransform = (lastFillerGO.transform as RectTransform) ?? lastFillerGO.AddComponent<RectTransform>();
			lastFillerTransform.SetParent(Layout.transform);
			lastFillerTransform.localScale = Vector3.one;
			LastFiller = lastFillerGO.AddComponent<LayoutElement>();
			
			var size = GetItemSize();
			if (IsHorizontal)
			{
				firstFillerTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
				lastFillerTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
			}
			else
			{
				firstFillerTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				lastFillerTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
			}
		}

		/// <summary>
		/// Updates the layout.
		/// </summary>
		public void UpdateLayout()
		{
			Utilites.UpdateLayout(Layout);
		}

		/// <summary>
		/// Sets the filler.
		/// </summary>
		/// <param name="first">First.</param>
		/// <param name="last">Last.</param>
		public void SetFiller(float first, float last)
		{
			if (FirstFiller!=null)
			{
				if (first==0f)
				{
					FirstFiller.gameObject.SetActive(false);
				}
				else
				{
					FirstFiller.gameObject.SetActive(true);
					FirstFiller.transform.SetAsFirstSibling();
					if (IsHorizontal)
					{
						FirstFiller.preferredWidth = first;
					}
					else
					{
						FirstFiller.preferredHeight = first;
					}
				}
			}
			
			if (LastFiller!=null)
			{
				if (last==0f)
				{
					LastFiller.gameObject.SetActive(false);
				}
				else
				{
					LastFiller.gameObject.SetActive(true);
					
					if (IsHorizontal)
					{
						LastFiller.preferredWidth = last;
					}
					else
					{
						LastFiller.preferredHeight = last;
					}
					LastFiller.transform.SetAsLastSibling();
				}
			}
		}

		/// <summary>
		/// Gets the size of the item.
		/// </summary>
		/// <returns>The item size.</returns>
		public Vector2 GetItemSize()
		{
			return new Vector2(LayoutUtility.GetPreferredWidth(DefaultItem), LayoutUtility.GetPreferredHeight(DefaultItem));
		}

		/// <summary>
		/// Gets the top or left margin.
		/// </summary>
		/// <returns>The margin.</returns>
		public float GetMargin()
		{
			return IsHorizontal ? Layout.padding.left : Layout.padding.top;
		}

		/// <summary>
		/// Gets the full margin.
		/// </summary>
		/// <returns>The full margin.</returns>
		public float GetFullMargin()
		{
			return IsHorizontal ? Layout.padding.horizontal : Layout.padding.vertical;
		}

		/// <summary>
		/// Gets the spacing.
		/// </summary>
		/// <returns>The spacing.</returns>
		public float GetSpacing()
		{
			return Layout.spacing;
		}
	}
}