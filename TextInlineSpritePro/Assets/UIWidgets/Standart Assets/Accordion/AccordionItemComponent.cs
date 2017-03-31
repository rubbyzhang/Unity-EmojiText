using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

namespace UIWidgets {
	/// <summary>
	/// OnClick listener.
	/// </summary>
	public class AccordionItemComponent : MonoBehaviour, IPointerClickHandler, ISubmitHandler {

		/// <summary>
		/// What to do when the event system send a pointer click event.
		/// </summary>
		public UnityEvent OnClick = new UnityEvent();

		/// <summary>
		/// Raises the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			OnClick.Invoke();
		}

		/// <summary>
		/// Raises the submit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSubmit(BaseEventData eventData)
		{
			OnClick.Invoke();
		}
	}
}