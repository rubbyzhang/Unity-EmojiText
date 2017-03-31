using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace UIWidgets {
	/// <summary>
	/// ListViewIcons drag support.
	/// </summary>
	[RequireComponent(typeof(ListViewIconsItemComponent))]
	public class ListViewIconsDragSupport : DragSupport<ListViewIconsItemDescription> {
		/// <summary>
		/// ListViewIcons.
		/// </summary>
		[SerializeField]
		public ListViewIcons ListView;

		/// <summary>
		/// The drag info.
		/// </summary>
		[SerializeField]
		public ListViewIconsItemComponent DragInfo;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			if (DragInfo!=null)
			{
				DragInfo.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Set Data, which will be passed to Drop component.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected override void InitDrag(PointerEventData eventData)
		{
			Data = GetComponent<ListViewIconsItemComponent>().Item;

			ShowDragInfo();
		}

		/// <summary>
		/// Shows the drag info.
		/// </summary>
		protected virtual void ShowDragInfo()
		{
			if (DragInfo==null)
			{
				return ;
			}
			DragInfo.transform.SetParent(DragPoint, false);
			DragInfo.transform.localPosition = new Vector3(-5, 5, 0);

			DragInfo.SetData(Data);

			DragInfo.gameObject.SetActive(true);
		}

		/// <summary>
		/// Hides the drag info.
		/// </summary>
		protected virtual void HideDragInfo()
		{
			if (DragInfo==null)
			{
				return ;
			}
			DragInfo.gameObject.SetActive(false);
		}

		/// <summary>
		/// Called when drop completed.
		/// </summary>
		/// <param name="success">true</param>
		/// <c>false</c>
		public override void Dropped(bool success)
		{
			HideDragInfo();
			
			// remove used from current ListViewIcons.
			if (success && (ListView!=null))
			{
				ListView.DataSource.Remove(Data);
			}

			base.Dropped(success);
		}
	}
}