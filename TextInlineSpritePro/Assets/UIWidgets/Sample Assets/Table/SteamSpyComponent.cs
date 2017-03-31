using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UIWidgets;

namespace UIWidgetsSamples {
	public class SteamSpyComponent : ListViewItem, IResizableItem {
		[SerializeField]
		public Text Name;

		[SerializeField]
		public Text ScoreRank;
		
		[SerializeField]
		public Text Owners;

		[SerializeField]
		public Text Players;

		[SerializeField]
		public Text PlayersIn2Week;

		[SerializeField]
		public Text TimeIn2Week;

		[SerializeField]
		public Text TooltipText;

		//SteamSpyItem Item;

		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		public GameObject[] ObjectsToResize {
			get {
				return new GameObject[] {
					Name.transform.parent.gameObject,
					ScoreRank.transform.parent.gameObject,
					Owners.transform.parent.gameObject,
					Players.transform.parent.gameObject,
					PlayersIn2Week.transform.parent.gameObject,
					TimeIn2Week.transform.parent.gameObject,
				};
			}
		}

		public void SetData(SteamSpyItem item)
		{
			//Item = item;
			
			Name.text = item.Name;

			TooltipText.text = item.Name;

			ScoreRank.text = (item.ScoreRank==-1) ? string.Empty : item.ScoreRank.ToString();

			Owners.text = item.Owners.ToString("N0") + "\n±" + item.OwnersVariance.ToString("N0");

			Players.text = item.Players.ToString("N0") + "\n±" + item.PlayersVariance.ToString("N0");

			PlayersIn2Week.text = item.PlayersIn2Week.ToString("N0") + "\n±" + item.PlayersIn2WeekVariance.ToString("N0");

			TimeIn2Week.text = Minutes2String(item.AverageTimeIn2Weeks) + "\n(" + Minutes2String(item.MedianTimeIn2Weeks) + ")";
		}

		string Minutes2String(int minutes)
		{
			return string.Format("{0:00}:{1:00}", minutes / 60, minutes % 60);
		}
	}
}