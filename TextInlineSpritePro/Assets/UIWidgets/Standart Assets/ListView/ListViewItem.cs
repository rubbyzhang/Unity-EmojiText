using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

namespace UIWidgets {
	[Serializable]
	public class ListViewItemSelect : UnityEvent<ListViewItem>
	{
		
	}

	[Serializable]
	public class ListViewItemMove : UnityEvent<AxisEventData, ListViewItem>
	{

	}

	/// <summary>
	/// ListViewItem.
	/// Item for ListViewBase.
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class ListViewItem : UIBehaviour,
		IPointerClickHandler,
		IPointerEnterHandler, IPointerExitHandler,
		ISubmitHandler, ICancelHandler,
		ISelectHandler, IDeselectHandler,
		IMoveHandler, IComparable<ListViewItem>
	{
		/// <summary>
		/// The index of item in ListView.
		/// </summary>
		[HideInInspector]
		public int Index = -1;

		/// <summary>
		/// What to do when the event system send a pointer click event.
		/// </summary>
		public UnityEvent onClick = new UnityEvent();

		/// <summary>
		/// What to do when the event system send a submit event.
		/// </summary>
		public ListViewItemSelect onSubmit = new ListViewItemSelect();

		/// <summary>
		/// What to do when the event system send a cancel event.
		/// </summary>
		public ListViewItemSelect onCancel = new ListViewItemSelect();

		/// <summary>
		/// What to do when the event system send a select event.
		/// </summary>
		public ListViewItemSelect onSelect = new ListViewItemSelect();

		/// <summary>
		/// What to do when the event system send a deselect event.
		/// </summary>
		public ListViewItemSelect onDeselect = new ListViewItemSelect();

		/// <summary>
		/// What to do when the event system send a move event.
		/// </summary>
		public ListViewItemMove onMove = new ListViewItemMove();

		/// <summary>
		/// What to do when the event system send a pointer click event.
		/// </summary>
		public PointerUnityEvent onPointerClick = new PointerUnityEvent();

		/// <summary>
		/// What to do when the event system send a pointer enter Event.
		/// </summary>
		public PointerUnityEvent onPointerEnter = new PointerUnityEvent();

		/// <summary>
		/// What to do when the event system send a pointer exit Event.
		/// </summary>
		public PointerUnityEvent onPointerExit = new PointerUnityEvent();

		/// <summary>
		/// The background.
		/// </summary>
		[System.NonSerialized]
		public ImageAdvanced Background;

		/// <summary>
		/// Awake this instance.
		/// </summary>
		protected override void Awake()
		{
			Background = GetComponent<ImageAdvanced>();
		}

		/// <summary>
		/// Raises the move event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnMove(AxisEventData eventData)
		{
			onMove.Invoke(eventData, this);
		}

		/// <summary>
		/// Raises the submit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSubmit(BaseEventData eventData)
		{
			onSubmit.Invoke(this);
		}

		/// <summary>
		/// Raises the cancel event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnCancel(BaseEventData eventData)
		{
			onCancel.Invoke(this);
		}

		/// <summary>
		/// Raises the select event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSelect(BaseEventData eventData)
		{
			Select();
			onSelect.Invoke(this);
		}

		/// <summary>
		/// Raises the deselect event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDeselect(BaseEventData eventData)
		{
			onDeselect.Invoke(this);
		}

		/// <summary>
		/// Raises the pointer click event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			onPointerClick.Invoke(eventData);
			onClick.Invoke();
			Select();
		}

		/// <summary>
		/// Raises the pointer enter event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			onPointerEnter.Invoke(eventData);
		}
		
		/// <summary>
		/// Raises the pointer exit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerExit(PointerEventData eventData)
		{
			onPointerExit.Invoke(eventData);
		}

		/// <summary>
		/// Select this instance.
		/// </summary>
		public virtual void Select()
		{
			if (EventSystem.current.alreadySelecting)
			{
				return;
			}

			var ev = new ListViewItemEventData(EventSystem.current) {
				NewSelectedObject = gameObject
			};
			EventSystem.current.SetSelectedGameObject(ev.NewSelectedObject, ev);
		}

		/// <summary>
		/// Compares the current object with another object of the same type by Index.
		/// </summary>
		/// <returns>Another object.</returns>
		/// <param name="compareItem">Compare item.</param>
		public int CompareTo(ListViewItem compareItem)
		{
			return (compareItem == null) ? 1 : Index.CompareTo(compareItem.Index);
		}

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public virtual void MovedToCache()
		{

		}
	}
}