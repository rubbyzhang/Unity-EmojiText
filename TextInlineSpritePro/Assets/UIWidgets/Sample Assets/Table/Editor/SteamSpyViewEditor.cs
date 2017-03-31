using UnityEditor;
using UIWidgets;

namespace UIWidgetsSamples.Shops
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SteamSpyView), true)]
	public class SteamSpyEditor : ListViewCustomEditor
	{
		public SteamSpyEditor()
		{
			Properties.Remove("customItems");
		}
	}
}