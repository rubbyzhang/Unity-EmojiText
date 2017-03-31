using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UIWidgets
{
	/// <summary>
	/// ListView sources.
	/// List - Use strings as source for list.
	/// File - Get strings from file, one line per string.
	/// </summary>
	public enum ListViewSources {
		List = 0,
		File = 1,
	}

	/// <summary>
	/// List view event.
	/// </summary>
	[Serializable]
	public class ListViewEvent : UnityEvent<int,string> {

	}

	/// <summary>
	/// List view.
	/// http://ilih.ru/images/unity-assets/UIWidgets/ListView.png
	/// </summary>
	[AddComponentMenu("UI/ListView", 250)]
	public class ListView : ListViewBase {
		[SerializeField]
		[Obsolete("Use DataSource instead.")]
		List<string> strings = new List<string>();

		//[SerializeField]
		//[HideInInspector]
		ObservableList<string> dataSource;

		/// <summary>
		/// Gets or sets the data source.
		/// </summary>
		/// <value>The data source.</value>
		public ObservableList<string> DataSource {
			get {
				if (dataSource==null)
				{
					#pragma warning disable 0618
					dataSource = new ObservableList<string>(strings);
					dataSource.OnChange += UpdateItems;
					strings = null;
					#pragma warning restore 0618
				}
				return dataSource;
			}
			set {
				SetNewItems(value);
				SetScrollValue(0f);
			}
		}

		/// <summary>
		/// Gets the strings.
		/// </summary>
		/// <value>The strings.</value>
		[Obsolete("Use DataSource instead.")]
		public List<string> Strings {
			get {
				return new List<string>(DataSource);
			}
			set {
				SetNewItems(new ObservableList<string>(value));
				SetScrollValue(0f);
			}
		}

		/// <summary>
		/// Gets the strings.
		/// </summary>
		/// <value>The strings.</value>
		[Obsolete("Use DataSource instead.")]
		public new List<string> Items {
			get {
				return new List<string>(DataSource);
			}
			set {
				SetNewItems(new ObservableList<string>(value));
				SetScrollValue(0f);
			}
		}

		[SerializeField]
		TextAsset file;

		/// <summary>
		/// Gets or sets the file with strings for ListView. One string per line.
		/// </summary>
		/// <value>The file.</value>
		public TextAsset File {
			get {
				return file;
			}
			set {
				file = value;
				if (file!=null)
				{
					GetItemsFromFile(file);
					SetScrollValue(0f);
				}
			}
		}

		/// <summary>
		/// The comments in file start with specified strings.
		/// </summary>
		[SerializeField]
		public List<string> CommentsStartWith = new List<string>(){"#", "//"};

		/// <summary>
		/// The source.
		/// </summary>
		[SerializeField]
		public ListViewSources Source = ListViewSources.List;

		/// <summary>
		/// Allow only unique strings.
		/// </summary>
		[SerializeField]
		public bool Unique = true;

		/// <summary>
		/// Allow empty strings.
		/// </summary>
		[SerializeField]
		public bool AllowEmptyItems;

		[SerializeField]
		Color backgroundColor = Color.white;

		[SerializeField]
		Color textColor = Color.black;

		/// <summary>
		/// Default background color.
		/// </summary>
		public Color BackgroundColor {
			get {
				return backgroundColor;
			}
			set {
				backgroundColor = value;
				UpdateColors();
			}
		}

		/// <summary>
		/// Default text color.
		/// </summary>
		public Color TextColor {
			get {
				return textColor;
			}
			set {
				textColor = value;
				UpdateColors();
			}
		}

		/// <summary>
		/// Color of background on pointer over.
		/// </summary>
		[SerializeField]
		public Color HighlightedBackgroundColor = new Color(203, 230, 244, 255);

		/// <summary>
		/// Color of text on pointer text.
		/// </summary>
		[SerializeField]
		public Color HighlightedTextColor = Color.black;

		[SerializeField]
		Color selectedBackgroundColor = new Color(53, 83, 227, 255);

		[SerializeField]
		Color selectedTextColor = Color.black;

		/// <summary>
		/// Background color of selected item.
		/// </summary>
		public Color SelectedBackgroundColor {
			get {
				return selectedBackgroundColor;
			}
			set {
				selectedBackgroundColor = value;
				UpdateColors();
			}
		}

		/// <summary>
		/// Text color of selected item.
		/// </summary>
		public Color SelectedTextColor {
			get {
				return selectedTextColor;
			}
			set {
				selectedTextColor = value;
				UpdateColors();
			}
		}

		/// <summary>
		/// The default item.
		/// </summary>
		[SerializeField]
		public ImageAdvanced DefaultItem;

		//List<ImageAdvanced> itemsImages = new List<ImageAdvanced>();
		//List<Text> itemsText = new List<Text>();
		List<ListViewStringComponent> components = new List<ListViewStringComponent>();

		List<UnityAction<PointerEventData>> callbacksEnter = new List<UnityAction<PointerEventData>>();
		List<UnityAction<PointerEventData>> callbacksExit = new List<UnityAction<PointerEventData>>();

		[SerializeField]
		[FormerlySerializedAs("Sort")]
		bool sort = true;

		/// <summary>
		/// Sort items.
		/// Advice to use DataSource.Comparison instead Sort and SortFunc.
		/// </summary>
		public bool Sort {
			get {
				return sort;
			}
			set {
				sort = value;
				if (Sort && isStartedListView && sortFunc!=null)
				{
					UpdateItems();
				}
			}
		}

		Func<IEnumerable<string>,IEnumerable<string>> sortFunc = items => items.OrderBy(x => x);

		/// <summary>
		/// Sort function.
		/// Advice to use DataSource.Comparison instead Sort and SortFunc.
		/// </summary>
		public Func<IEnumerable<string>, IEnumerable<string>> SortFunc {
			get {
				return sortFunc;
			}
			set {
				sortFunc = value;
				if (Sort && isStartedListView && sortFunc!=null)
				{
					UpdateItems();
				}
			}
		}

		/// <summary>
		/// OnSelect event.
		/// </summary>
		public ListViewEvent OnSelectString = new ListViewEvent();

		/// <summary>
		/// OnDeselect event.
		/// </summary>
		public ListViewEvent OnDeselectString = new ListViewEvent();

		[SerializeField]
		ScrollRect scrollRect;
		
		/// <summary>
		/// Gets or sets the ScrollRect.
		/// </summary>
		/// <value>The ScrollRect.</value>
		public ScrollRect ScrollRect {
			get {
				return scrollRect;
			}
			set {
				if (scrollRect!=null)
				{
					var r = scrollRect.GetComponent<ResizeListener>();
					if (r!=null)
					{
						r.OnResize.RemoveListener(SetNeedResize);
					}

					scrollRect.onValueChanged.RemoveListener(OnScrollUpdate);
				}
				scrollRect = value;
				if (scrollRect!=null)
				{
					var r = scrollRect.GetComponent<ResizeListener>() ?? scrollRect.gameObject.AddComponent<ResizeListener>();
					r.OnResize.AddListener(SetNeedResize);

					scrollRect.onValueChanged.AddListener(OnScrollUpdate);
				}
			}
		}
				
		/// <summary>
		/// The height of the DefaultItem.
		/// </summary>
		float itemHeight;

		/// <summary>
		/// The width of the DefaultItem.
		/// </summary>
		float itemWidth;

		/// <summary>
		/// The height of the ScrollRect.
		/// </summary>
		float scrollHeight;

		/// <summary>
		/// The width of the ScrollRect.
		/// </summary>
		float scrollWidth;

		/// <summary>
		/// Count of visible items.
		/// </summary>
		int maxVisibleItems;

		/// <summary>
		/// Count of visible items.
		/// </summary>
		int visibleItems;
		
		/// <summary>
		/// Count of hidden items by top filler.
		/// </summary>
		int topHiddenItems;
		
		/// <summary>
		/// Count of hidden items by bottom filler.
		/// </summary>
		int bottomHiddenItems;

		[SerializeField]
		ListViewDirection direction = ListViewDirection.Vertical;

		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		/// <value>The direction.</value>
		public ListViewDirection Direction {
			get {
				return direction;
			}
			set {
				direction = value;

				if (scrollRect)
				{
					scrollRect.horizontal = IsHorizontal();
					scrollRect.vertical = !IsHorizontal();
				}
				if (CanOptimize() && (layout is EasyLayout.EasyLayout))
				{
					LayoutBridge.IsHorizontal = IsHorizontal();

					CalculateMaxVisibleItems();
				}
				UpdateView();
			}
		}

		void Awake()
		{
			Start();
		}

		[System.NonSerialized]
		bool isStartedListView = false;

		LayoutGroup layout;

		/// <summary>
		/// Gets the layout.
		/// </summary>
		/// <value>The layout.</value>
		public EasyLayout.EasyLayout Layout {
			get {
				return layout as EasyLayout.EasyLayout;
			}
		}

		/// <summary>
		/// LayoutBridge.
		/// </summary>
		protected ILayoutBridge LayoutBridge;

		List<string> SelectedItemsCache;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public override void Start()
		{
			if (isStartedListView)
			{
				return ;
			}
			isStartedListView = true;

			base.Start();
			base.Items = new List<ListViewItem>();

			SelectedItemsCache = SelectedIndicies.Convert<int,string>(GetDataItem);

			DestroyGameObjects = false;

			if (DefaultItem==null)
			{
				throw new NullReferenceException("DefaultItem is null. Set component of type ImageAdvanced to DefaultItem.");
			}
			DefaultItem.gameObject.SetActive(true);
			if (DefaultItem.GetComponentInChildren<Text>()==null)
			{
				throw new MissingComponentException("DefaultItem don't have child with 'Text' component. Add child with 'Text' component to DefaultItem.");
			}

			if (CanOptimize())
			{
				ScrollRect = scrollRect;

				var scrollRectTransform = scrollRect.transform as RectTransform;
				scrollHeight = scrollRectTransform.rect.height;
				scrollWidth = scrollRectTransform.rect.width;

				layout = Container.GetComponent<LayoutGroup>();
				if (layout is EasyLayout.EasyLayout)
				{
					LayoutBridge = new EasyLayoutBridge(layout as EasyLayout.EasyLayout, DefaultItem.transform as RectTransform);
					LayoutBridge.IsHorizontal = IsHorizontal();
				}
				else if (layout is HorizontalOrVerticalLayoutGroup)
				{
					LayoutBridge = new StandardLayoutBridge(layout as HorizontalOrVerticalLayoutGroup, DefaultItem.transform as RectTransform);
				}

				CalculateItemSize();
				CalculateMaxVisibleItems();
			}
			
			DefaultItem.gameObject.SetActive(false);

			UpdateItems();

			OnSelect.AddListener(OnSelectCallback);
			OnDeselect.AddListener(OnDeselectCallback);
		}

		/// <summary>
		/// Calculates the size of the item.
		/// </summary>
		protected virtual void CalculateItemSize()
		{
			if (LayoutBridge==null)
			{
				return ;
			}
			var size = LayoutBridge.GetItemSize();
			itemHeight = size.y;
			itemWidth = size.x;
		}

		/// <summary>
		/// Gets the item.
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="index">Index.</param>
		protected string GetDataItem(int index)
		{
			return DataSource[index];
		}

		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		protected bool IsHorizontal()
		{
			return direction==ListViewDirection.Horizontal;
		}

		/// <summary>
		/// Calculates the max count of visible items.
		/// </summary>
		void CalculateMaxVisibleItems()
		{
			if (IsHorizontal())
			{
				maxVisibleItems = Mathf.CeilToInt(scrollWidth / itemWidth) + 1;
			}
			else
			{
				maxVisibleItems = Mathf.CeilToInt(scrollHeight / itemHeight) + 1;
			}
		}

		/// <summary>
		/// Handle instance resize.
		/// </summary>
		void Resize()
		{
			needResize = false;

			var scrollRectTransform = scrollRect.transform as RectTransform;
			scrollHeight = scrollRectTransform.rect.height;
			scrollWidth = scrollRectTransform.rect.width;

			CalculateMaxVisibleItems();
			UpdateView();
		}

		/// <summary>
		/// Determines whether this instance can be optimized.
		/// </summary>
		/// <returns><c>true</c> if this instance can be optimized; otherwise, <c>false</c>.</returns>
		bool CanOptimize()
		{
			return scrollRect!=null && (layout!=null || Container.GetComponent<EasyLayout.EasyLayout>()!=null);
		}

		/// <summary>
		/// Raises the select callback event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		void OnSelectCallback(int index, ListViewItem item)
		{
			if (SelectedItemsCache!=null)
			{
				SelectedItemsCache.Add(DataSource[index]);
			}

			OnSelectString.Invoke(index, DataSource[index]);
			
			if (item!=null)
			{
				SelectColoring(item as ListViewStringComponent);
			}
		}

		/// <summary>
		/// Raises the deselect callback event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		void OnDeselectCallback(int index, ListViewItem item)
		{
			if (SelectedItemsCache!=null)
			{
				SelectedItemsCache.Remove(DataSource[index]);
			}

			OnDeselectString.Invoke(index, DataSource[index]);

			if (item!=null)
			{
				DefaultColoring(item as ListViewStringComponent);
			}
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		public override void UpdateItems()
		{
			if (Source==ListViewSources.List)
			{
				SetNewItems(DataSource);
			}
			else
			{
				Source = ListViewSources.List;

				GetItemsFromFile(File);
			}
		}

		/// <summary>
		/// Clear strings list.
		/// </summary>
		public override void Clear()
		{
			DataSource.Clear();
		}

		/// <summary>
		/// Gets the items from file.
		/// </summary>
		public void GetItemsFromFile()
		{
			GetItemsFromFile(File);
		}

		string StringTrimEnd(string str)
		{
			return str.TrimEnd();
		}

		bool IsStringNotEmpty(string str)
		{
			return str!=string.Empty;
		}

		bool NotComment(string str)
		{
			return !CommentsStartWith.Any(comment => str.StartsWith(comment));
		}

		/// <summary>
		/// Gets the items from file.
		/// </summary>
		/// <param name="sourceFile">Source file.</param>
		public void GetItemsFromFile(TextAsset sourceFile)
		{
			if (file==null)
			{
				return ;
			}

			var new_items = sourceFile.text.Split(new string[] {"\r\n", "\r", "\n"}, StringSplitOptions.None).Select<string,string>(StringTrimEnd);
			if (Unique)
			{
				new_items = new_items.Distinct();
			}
			
			if (!AllowEmptyItems)
			{
				new_items = new_items.Where(IsStringNotEmpty);
			}
			
			if (CommentsStartWith.Count > 0)
			{
				new_items = new_items.Where(NotComment);
			}
			SetNewItems(new_items.ToObservableList());
		}

		/// <summary>
		/// Finds the indicies of specified item.
		/// </summary>
		/// <returns>The indicies.</returns>
		/// <param name="item">Item.</param>
		public virtual List<int> FindIndicies(string item)
		{
			return Enumerable.Range(0, DataSource.Count)
				.Where(i => DataSource[i]==item)
				.ToList();
		}

		/// <summary>
		/// Finds the index of specified item.
		/// </summary>
		/// <returns>The index.</returns>
		/// <param name="item">Item.</param>
		public virtual int FindIndex(string item)
		{
			return DataSource.IndexOf(item);
		}

		/// <summary>
		/// Add the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of added item.</returns>
		public virtual int Add(string item)
		{
			var old_indicies = (Sort && SortFunc!=null) ? FindIndicies(item) : null;

			DataSource.Add(item);

			if (Sort && SortFunc!=null)
			{
				var new_indicies = FindIndicies(item);
				
				var diff = new_indicies.Except(old_indicies).ToArray();
				if (diff.Length > 0)
				{
					return diff[0];
				}
				if (new_indicies.Count > 0)
				{
					return new_indicies[0];
				}
				return -1;
			}
			else
			{
				return DataSource.Count - 1;
			}
		}

		/// <summary>
		/// Remove the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Index of removed item.</returns>
		public virtual int Remove(string item)
		{
			var index = FindIndex(item);
			if (index==-1)
			{
				return index;
			}

			DataSource.Remove(item);

			return index;
		}

		void RemoveCallback(ListViewStringComponent component, int index)
		{
			if (component==null)
			{
				return ;
			}
			if (index < callbacksEnter.Count)
			{
				component.onPointerEnter.RemoveListener(callbacksEnter[index]);
			}
			if (index < callbacksExit.Count)
			{
				component.onPointerExit.RemoveListener(callbacksExit[index]);
			}
		}

		/// <summary>
		/// Removes the callbacks.
		/// </summary>
		void RemoveCallbacks()
		{
			components.ForEach(RemoveCallback);
			callbacksEnter.Clear();
			callbacksExit.Clear();
		}

		/// <summary>
		/// Adds the callbacks.
		/// </summary>
		void AddCallbacks()
		{
			components.ForEach(AddCallback);
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="index">Index.</param>
		void AddCallback(ListViewStringComponent component, int index)
		{
			callbacksEnter.Add(ev => OnPointerEnterCallback(component));
			callbacksExit.Add(ev => OnPointerExitCallback(component));
			
			component.onPointerEnter.AddListener(callbacksEnter[index]);
			component.onPointerExit.AddListener(callbacksExit[index]);
		}

		/// <summary>
		/// Determines if item exists with the specified index.
		/// </summary>
		/// <returns><c>true</c> if item exists with the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public override bool IsValid(int index)
		{
			return (index >= 0) && (index < DataSource.Count);
		}

		/// <summary>
		/// Raises the pointer enter callback event.
		/// </summary>
		/// <param name="component">Component.</param>
		void OnPointerEnterCallback(ListViewStringComponent component)
		{
			if (!IsValid(component.Index))
			{
				var message = string.Format("Index must be between 0 and Items.Count ({0})", DataSource.Count);
				throw new IndexOutOfRangeException(message);
			}

			if (IsSelected(component.Index))
			{
				return ;
			}

			HighlightColoring(component);
		}

		/// <summary>
		/// Raises the pointer exit callback event.
		/// </summary>
		/// <param name="component">Component.</param>
		void OnPointerExitCallback(ListViewStringComponent component)
		{
			if (!IsValid(component.Index))
			{
				var message = string.Format("Index must be between 0 and Items.Count ({0})", DataSource.Count);
				throw new IndexOutOfRangeException(message);
			}

			if (IsSelected(component.Index))
			{
				return ;
			}

			DefaultColoring(component);
		}

		/// <summary>
		/// Sets the scroll value.
		/// </summary>
		/// <param name="value">Value.</param>
		protected void SetScrollValue(float value)
		{
			var current_position = scrollRect.content.anchoredPosition;
			var new_position = new Vector2(current_position.x, value);
			if (new_position != current_position)
			{
				scrollRect.content.anchoredPosition = new_position;
				ScrollUpdate();
			}
		}

		/// <summary>
		/// Gets the scroll value.
		/// </summary>
		/// <returns>The scroll value.</returns>
		protected float GetScrollValue()
		{
			var pos = scrollRect.content.anchoredPosition;
			return Mathf.Max(0f, (IsHorizontal()) ? -pos.x : pos.y);
		}

		/// <summary>
		/// Gets the size of the item.
		/// </summary>
		/// <returns>The item size.</returns>
		protected float GetItemSize()
		{
			return (IsHorizontal())
				? itemWidth + LayoutBridge.GetSpacing()
				: itemHeight + LayoutBridge.GetSpacing();
		}

		/// <summary>
		/// Gets the size of the scroll.
		/// </summary>
		/// <returns>The scroll size.</returns>
		protected float GetScrollSize()
		{
			return (IsHorizontal()) ? scrollWidth : scrollHeight;
		}

		/// <summary>
		/// Scrolls to item with specifid index.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void ScrollTo(int index)
		{
			if (!CanOptimize())
			{
				return ;
			}
			
			var first_visible = GetFirstVisibleIndex(true);
			var last_visible = GetLastVisibleIndex(true);
			
			if (first_visible > index)
			{
				var item_starts = index * GetItemSize();
				SetScrollValue(item_starts);
			}
			else if (last_visible < index)
			{
				var item_ends = (index + 1) * GetItemSize() - LayoutBridge.GetSpacing() + LayoutBridge.GetMargin() - GetScrollSize();

				SetScrollValue(item_ends);
			}
		}

		/// <summary>
		/// Gets the last index of the visible.
		/// </summary>
		/// <returns>The last visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		int GetLastVisibleIndex(bool strict=false)
		{
			var window = GetScrollValue() + GetScrollSize();
			var last_visible_index = (strict)
				? Mathf.FloorToInt(window / GetItemSize())
				: Mathf.CeilToInt(window / GetItemSize());
			
			return last_visible_index - 1;
		}

		/// <summary>
		/// Gets the first index of the visible.
		/// </summary>
		/// <returns>The first visible index.</returns>
		/// <param name="strict">If set to <c>true</c> strict.</param>
		int GetFirstVisibleIndex(bool strict=false)
		{
			var first_visible_index = (strict)
				? Mathf.CeilToInt(GetScrollValue() / GetItemSize())
				: Mathf.FloorToInt(GetScrollValue() / GetItemSize());
			if (strict)
			{
				return first_visible_index;
			}
			
			return Mathf.Min(first_visible_index, Mathf.Max(0, DataSource.Count - visibleItems));
		}

		/// <summary>
		/// Move first component from top to bottom.
		/// </summary>
		/// <returns>Component.</returns>
		ListViewStringComponent ComponentTopToBottom()
		{
			var bottom = components.Count - 1;

			var bottomComponent = components[bottom];
			components.RemoveAt(bottom);
			components.Insert(0, bottomComponent);
			bottomComponent.transform.SetAsFirstSibling();

			return bottomComponent;
		}

		/// <summary>
		/// Move last component from bottom to top.
		/// </summary>
		/// <returns>Component.</returns>
		ListViewStringComponent ComponentBottomToTop()
		{
			var topComponent = components[0];
			components.RemoveAt(0);
			components.Add(topComponent);
			topComponent.transform.SetAsLastSibling();

			return topComponent;
		}

		/// <summary>
		/// Raises the scroll update event.
		/// </summary>
		/// <param name="position">Position.</param>
		void OnScrollUpdate(Vector2 position)
		{
			ScrollUpdate();
		}

		/// <summary>
		/// Raises the scroll event.
		/// </summary>
		/// <param name="value">Value.</param>
		void OnScroll(float value)
		{
			ScrollUpdate();
		}

		void ScrollUpdate()
		{
			var oldTopHiddenItems = topHiddenItems;
			
			topHiddenItems = GetFirstVisibleIndex();
			bottomHiddenItems = Mathf.Max(0, DataSource.Count - visibleItems - topHiddenItems);
			
			if (oldTopHiddenItems==topHiddenItems)
			{
				//do nothing
			}
			// optimization on +-1 item scroll
			else if (oldTopHiddenItems==(topHiddenItems + 1))
			{
				var bottomComponent = ComponentTopToBottom();
				
				bottomComponent.Index = topHiddenItems;
				bottomComponent.Text.text = DataSource[topHiddenItems];
				Coloring(bottomComponent);
			}
			else if (oldTopHiddenItems==(topHiddenItems - 1))
			{
				var topComponent = ComponentBottomToTop();
				
				var new_index = topHiddenItems + visibleItems - 1;
				topComponent.Index = new_index;
				topComponent.Text.text = DataSource[new_index];
				Coloring(topComponent);
			}
			// all other cases
			else
			{
				//!
				var new_indicies = Enumerable.Range(topHiddenItems, visibleItems).ToArray();
				components.ForEach((x, i) => {
					x.Index = new_indicies[i];
					x.Text.text = DataSource[new_indicies[i]];
					Coloring(x);
				});
			}
			
			if (LayoutBridge!=null)
			{
				LayoutBridge.SetFiller(CalculateTopFillerSize(), CalculateBottomFillerSize());
				LayoutBridge.UpdateLayout();
			}
		}

		/// <summary>
		/// The components cache.
		/// </summary>
		List<ListViewStringComponent> componentsCache = new List<ListViewStringComponent>();

		bool IsNullComponent(ListViewStringComponent component)
		{
			return component==null;
		}

		/// <summary>
		/// Gets the new components according max count of visible items.
		/// </summary>
		/// <returns>The new components.</returns>
		List<ListViewStringComponent> GetNewComponents()
		{
			componentsCache.RemoveAll(IsNullComponent);
			var new_components = new List<ListViewStringComponent>();
			DataSource.ForEach ((x, i) =>  {
				if (i >= visibleItems)
				{
					return;
				}
				
				if (components.Count > 0)
				{
					new_components.Add(components[0]);
					components.RemoveAt(0);
				}
				else if (componentsCache.Count > 0)
				{
					componentsCache[0].gameObject.SetActive(true);
					new_components.Add(componentsCache[0]);
					componentsCache.RemoveAt(0);
				}
				else
				{
					#if UNITY_4_6 || UNITY_4_7
					var background = Instantiate(DefaultItem) as ImageAdvanced;
					#else
					var background = Instantiate(DefaultItem);
					#endif

					background.gameObject.SetActive(true);
					
					var component = background.GetComponent<ListViewStringComponent>();
					if (component==null)
					{
						component = background.gameObject.AddComponent<ListViewStringComponent>();
						component.Background = background;
						component.Text = background.GetComponentInChildren<Text>();
					}

					Utilites.FixInstantiated(DefaultItem, background);
					component.gameObject.SetActive(true);

					new_components.Add(component);
				}
			});
			
			components.ForEach(x => {
				x.MovedToCache();
				x.Index = -1;
				x.gameObject.SetActive(false);
			});
			componentsCache.AddRange(components);
			components.Clear();
			
			return new_components;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		void UpdateView()
		{
			RemoveCallbacks();
			
			if ((CanOptimize()) && (DataSource.Count > 0))
			{
				visibleItems = (maxVisibleItems < DataSource.Count) ? maxVisibleItems : DataSource.Count;
			}
			else
			{
				visibleItems = DataSource.Count;
			}

			components = GetNewComponents();

			base.Items = components.Convert(x => x as ListViewItem);

			components.ForEach(SetComponentData);
			
			AddCallbacks();
			
			topHiddenItems = 0;
			bottomHiddenItems = DataSource.Count() - visibleItems;

			if (LayoutBridge!=null)
			{
				LayoutBridge.SetFiller(CalculateTopFillerSize(), CalculateBottomFillerSize());
				LayoutBridge.UpdateLayout();
			}

			if (scrollRect!=null)
			{
				var r = scrollRect.transform as RectTransform;
				r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, r.rect.width);
			}
		}

		void SetComponentData(ListViewStringComponent component, int index)
		{
			component.Index = index;
			component.Text.text = DataSource[index];
			Coloring(component);
		}

		bool IndexNotFound(int index)
		{
			return index==-1;
		}

		/// <summary>
		/// Updates the items.
		/// </summary>
		/// <param name="newItems">New items.</param>
		void SetNewItems(ObservableList<string> newItems)
		{
			DataSource.OnChange -= UpdateItems;

			if (Sort && SortFunc!=null)
			{
				newItems.BeginUpdate();

				var sorted = SortFunc(newItems).ToArray();

				newItems.Clear();
				newItems.AddRange(sorted);

				newItems.EndUpdate();
			}

			SilentDeselect(SelectedIndicies);
			var new_selected_indicies = SelectedItemsCache.Convert<string,int>(newItems.IndexOf);
			new_selected_indicies.RemoveAll(IndexNotFound);

			dataSource = newItems;

			SilentSelect(new_selected_indicies);
			SelectedItemsCache = SelectedIndicies.Convert<int,string>(GetDataItem);

			UpdateView();

			DataSource.OnChange += UpdateItems;
		}

		/// <summary>
		/// Calculates the size of the bottom filler.
		/// </summary>
		/// <returns>The bottom filler size.</returns>
		float CalculateBottomFillerSize()
		{
			return (bottomHiddenItems==0) ? 0f : bottomHiddenItems * GetItemSize() - LayoutBridge.GetSpacing();
		}

		/// <summary>
		/// Calculates the size of the top filler.
		/// </summary>
		/// <returns>The top filler size.</returns>
		float CalculateTopFillerSize()
		{
			return (topHiddenItems==0) ? 0f : topHiddenItems * GetItemSize() - LayoutBridge.GetSpacing();
		}

		/// <summary>
		/// Calculated selected indicies of new items .
		/// </summary>
		/// <returns>The selected indicies.</returns>
		/// <param name="newItems">New items.</param>
		List<int> NewSelectedIndicies(ObservableList<string> newItems)
		{
			var selected_indicies = new List<int>();
			if (newItems.Count==0)
			{
				return selected_indicies;
			}

			//duplicated items should not be selected more than at start
			var new_items_copy = new List<string>(newItems);

			var selected_items = SelectedIndicies.Convert<int,string>(GetDataItem);

			selected_items = selected_items.Where(x => {
				var is_valid_item = newItems.Contains(x);
				if (is_valid_item)
				{
					new_items_copy.Remove(x);
				}
				return is_valid_item;
			}).ToList();

			newItems.ForEach((item, index) => {
				if (selected_items.Contains(item))
				{
					selected_items.Remove(item);
					selected_indicies.Add(index);
				}
			});

			return selected_indicies;
		}

		/// <summary>
		/// Coloring the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void Coloring(ListViewItem component)
		{
			if (component==null)
			{
				return ;
			}
			if (SelectedIndicies.Contains(component.Index))
			{
				SelectColoring(component);
			}
			else
			{
				DefaultColoring(component);
			}
		}

		/// <summary>
		/// Updates the colors of components.
		/// </summary>
		void UpdateColors()
		{
			components.ForEach(Coloring);
		}

		/// <summary>
		/// Gets the component.
		/// </summary>
		/// <returns>The component.</returns>
		/// <param name="index">Index.</param>
		ListViewStringComponent GetComponent(int index)
		{
			return components.Find(x => x.Index==index);
		}

		/// <summary>
		/// Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		public int Set(string item, bool allowDuplicate=true)
		{
			int index;
			if (!allowDuplicate)
			{
				index = DataSource.IndexOf(item);
				if (index==-1)
				{
					index = Add(item);
				}
			}
			else
			{
				index = Add(item);
			}
			Select(index);
			
			return index;
		}

		/// <summary>
		/// Called when item selected.
		/// Use it for change visible style of selected item.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void SelectItem(int index)
		{
			SelectColoring(GetComponent(index));
		}

		/// <summary>
		/// Called when item deselected.
		/// Use it for change visible style of deselected item.
		/// </summary>
		/// <param name="index">Index.</param>
		protected override void DeselectItem(int index)
		{
			DefaultColoring(GetComponent(index));
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected override void HighlightColoring(ListViewItem component)
		{
			if (IsSelected(component.Index))
			{
				return ;
			}
			HighlightColoring(component as ListViewStringComponent);
		}

		/// <summary>
		/// Set highlights colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void HighlightColoring(ListViewStringComponent component)
		{
			if (component==null)
			{
				return ;
			}

			component.Background.color = HighlightedBackgroundColor;
			component.Text.color = HighlightedTextColor;
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(ListViewItem component)
		{
			if (component==null)
			{
				return ;
			}
			
			SelectColoring(component as ListViewStringComponent);
		}

		/// <summary>
		/// Set select colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void SelectColoring(ListViewStringComponent component)
		{
			if (component==null)
			{
				return ;
			}

			component.Background.color = selectedBackgroundColor;
			component.Text.color = selectedTextColor;
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DefaultColoring(ListViewItem component)
		{
			if (component==null)
			{
				return ;
			}
			
			DefaultColoring(component as ListViewStringComponent);
		}

		/// <summary>
		/// Set default colors of specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void DefaultColoring(ListViewStringComponent component)
		{
			if (component==null)
			{
				return ;
			}

			component.Background.color = backgroundColor;
			component.Text.color = textColor;
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			OnSelect.RemoveListener(OnSelectCallback);
			OnDeselect.RemoveListener(OnDeselectCallback);

			ScrollRect = null;

			RemoveCallbacks();

			base.OnDestroy();
		}

		bool needResize;

		void Update()
		{
			if (needResize)
			{
				Resize();
			}
		}

		void SetNeedResize()
		{
			if (!CanOptimize())
			{
				return ;
			}
			needResize = true;
		}

#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/ListView", false, 1060)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("ListView");
		}
#endif
	}
}