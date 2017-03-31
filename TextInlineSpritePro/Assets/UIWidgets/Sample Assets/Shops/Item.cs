using UnityEngine;
using System.Collections;
using System.ComponentModel;
using UIWidgets;

namespace UIWidgetsSamples.Shops {
	public class Item : IObservable, INotifyPropertyChanged {
		public string Name;
		// -1 for infinite count
		int count;

		public event OnChange OnChange;
		public event PropertyChangedEventHandler PropertyChanged;

		public int Count {
			get {
				return count;
			}
			set {
				if (count==-1)
				{
					Changed();
					return ;
				}
				count = value;
				Changed();
			}
		}

		public Item(string name, int count)
		{
			Name = name;
			Count = count;
		}

		void Changed()
		{
			if (PropertyChanged!=null)
			{
				PropertyChanged(this, null);
			}

			if (OnChange!=null)
			{
				OnChange();
			}
		}
	}
}