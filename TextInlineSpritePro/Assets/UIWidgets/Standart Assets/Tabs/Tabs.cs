using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UIWidgets
{
	[Serializable]
	public class TabSelectEvent : UnityEvent<int>
	{
	}

	/// <summary>
	/// Tabs.
	/// http://ilih.ru/images/unity-assets/UIWidgets/Tabs.png
	/// </summary>
	[AddComponentMenu("UI/Tabs", 290)]
	public class Tabs : MonoBehaviour
	{
		/// <summary>
		/// The container for tab toggle buttons.
		/// </summary>
		[SerializeField]
		public Transform Container;
		
		/// <summary>
		/// The default tab button.
		/// </summary>
		[SerializeField]
		public Button DefaultTabButton;
		
		/// <summary>
		/// The active tab button.
		/// </summary>
		[SerializeField]
		public Button ActiveTabButton;
		
		[SerializeField]
		Tab[] tabObjects = new Tab[]{};

		/// <summary>
		/// Gets or sets the tab objects.
		/// </summary>
		/// <value>The tab objects.</value>
		public Tab[] TabObjects {
			get {
				return tabObjects;
			}
			set {
				tabObjects = value;
				UpdateButtons();
			}
		}

		/// <summary>
		/// If true does not deactivate hidden tabs.
		/// </summary>
		[SerializeField]
		[Tooltip("If true does not deactivate hidden tabs.")]
		public bool KeepTabsActive = false;

		/// <summary>
		/// OnTabSelect event.
		/// </summary>
		[SerializeField]
		public TabSelectEvent OnTabSelect = new TabSelectEvent();

		/// <summary>
		/// Gets or sets the selected tab.
		/// </summary>
		/// <value>The selected tab.</value>
		public Tab SelectedTab {
			get;
			protected set;
		}
		
		List<Button> defaultButtons = new List<Button>();
		List<Button> activeButtons = new List<Button>();
		List<UnityAction> callbacks = new List<UnityAction>();
		
		void Start()
		{
			if (Container==null)
			{
				throw new NullReferenceException("Container is null. Set object of type GameObject to Container.");
			}
			if (DefaultTabButton==null)
			{
				throw new NullReferenceException("DefaultTabButton is null. Set object of type GameObject to DefaultTabButton.");
			}
			if (ActiveTabButton==null)
			{
				throw new NullReferenceException("ActiveTabButton is null. Set object of type GameObject to ActiveTabButton.");
			}
			DefaultTabButton.gameObject.SetActive(false);
			ActiveTabButton.gameObject.SetActive(false);
			
			UpdateButtons();
		}

		/// <summary>
		/// Updates the buttons.
		/// </summary>
		void UpdateButtons()
		{
			if (tabObjects.Length==0)
			{
				throw new ArgumentException("TabObjects array is empty. Fill it.");
			}
			
			RemoveCallbacks();

			CreateButtons();

			AddCallbacks();

			SelectTab(tabObjects[0].Name);
		}

		void AddCallback(Tab tab, int index)
		{
			var tabName = tab.Name;
			UnityAction callback = () => SelectTab(tabName);
			callbacks.Add(callback);
			
			defaultButtons[index].onClick.AddListener(callbacks[index]);
		}

		void AddCallbacks()
		{
			tabObjects.ForEach(AddCallback);
		}

		void RemoveCallback(Tab tab, int index)
		{
			if ((tab!=null) && (index < callbacks.Count))
			{
				defaultButtons[index].onClick.RemoveListener(callbacks[index]);
			}
		}

		void RemoveCallbacks()
		{
			if (callbacks.Count > 0)
			{
				tabObjects.ForEach(RemoveCallback);
				callbacks.Clear();
			}
		}

		void OnDestroy()
		{
			RemoveCallbacks();
		}

		/// <summary>
		/// Selects the tab.
		/// </summary>
		/// <param name="tabName">Tab name.</param>
		public void SelectTab(string tabName)
		{
			var index = Array.FindIndex(tabObjects, x => x.Name==tabName);
			if (index==-1)
			{
				throw new ArgumentException(string.Format("Tab with name \"{0}\" not found.", tabName));
			}

			if (KeepTabsActive)
			{
				tabObjects[index].TabObject.transform.SetAsLastSibling();
			}
			else
			{
				tabObjects.ForEach(DeactivateTab);
				tabObjects[index].TabObject.SetActive(true);
			}
			
			defaultButtons.ForEach(ActivateButton);
			defaultButtons[index].gameObject.SetActive(false);
			
			activeButtons.ForEach(DeactivateButton);
			activeButtons[index].gameObject.SetActive(true);

			SelectedTab = tabObjects[index];
			OnTabSelect.Invoke(index);
		}

		void DeactivateTab(Tab tab)
		{
			tab.TabObject.SetActive(false);
		}

		void ActivateButton(Button button)
		{
			button.gameObject.SetActive(true);
		}

		void DeactivateButton(Button button)
		{
			button.gameObject.SetActive(false);
		}

		/// <summary>
		/// Creates the buttons.
		/// </summary>
		void CreateButtons()
		{
			if (tabObjects.Length > defaultButtons.Count)
			{
				for (var i = defaultButtons.Count; i < tabObjects.Length; i++)
				{
					var defaultButton = Instantiate(DefaultTabButton) as Button;
					defaultButton.transform.SetParent(Container, false);
					
					//Utilites.FixInstantiated(DefaultTabButton, defaultButton);
					
					defaultButtons.Add(defaultButton);
					
					var activeButton = Instantiate(ActiveTabButton) as Button;
					activeButton.transform.SetParent(Container, false);
					
					//Utilites.FixInstantiated(ActiveTabButton, activeButton);
					
					activeButtons.Add(activeButton);
				}
			}
			//del existing ui elements if necessary
			if (tabObjects.Length < defaultButtons.Count)
			{
				for (var i = defaultButtons.Count; i > tabObjects.Length; i--)
				{
					Destroy(defaultButtons[i]);
					Destroy(activeButtons[i]);
					
					defaultButtons.RemoveAt(i);
					activeButtons.RemoveAt(i);
				}
			}
			
			defaultButtons.ForEach(SetButtonName);
			activeButtons.ForEach(SetButtonName);
		}

		/// <summary>
		/// Sets the name of the button.
		/// </summary>
		/// <param name="button">Button.</param>
		/// <param name="index">Index.</param>
		void SetButtonName(Button button, int index)
		{
			button.gameObject.SetActive(true);
			var text = button.GetComponentInChildren<Text>();
			if (text)
			{
				text.text = tabObjects[index].Name;
			}
		}
		
		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/Tabs", false, 1180)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("Tabs");
		}
		#endif
	}
}