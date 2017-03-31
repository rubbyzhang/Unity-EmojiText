using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace UIWidgets
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ListViewBase), true)]
	public class ListViewCustomBaseEditor : Editor
	{
		protected bool IsListViewCustom = false;
		protected bool IsListViewCustomHeight = false;
		protected bool IsTileView = false;
		protected bool IsTreeViewCustom = false;

		protected Dictionary<string,SerializedProperty> SerializedProperties = new Dictionary<string,SerializedProperty>();
		protected Dictionary<string,SerializedProperty> SerializedEvents = new Dictionary<string,SerializedProperty>();

		protected List<string> Properties = new List<string>{
			"customItems",
			"Multiple",
			"selectedIndex",
			
			"direction",

			"DefaultItem",
			"Container",
			"scrollRect",

			"defaultColor",
			"defaultBackgroundColor",

			"HighlightedColor",
			"HighlightedBackgroundColor",

			"selectedColor",
			"selectedBackgroundColor",

			//"OnSelectObject",

			"EndScrollDelay",
		};

		protected List<string> Events = new List<string>{
			"OnSelect",
			"OnDeselect",
			//"OnSubmit",
			//"OnCancel",
			//"OnItemSelect",
			//"OnItemCancel",
			//"OnFocusIn",
			//"OnFocusOut",
			"OnSelectObject",
			"OnDeselectObject",
			//"OnPointerEnterObject",
			//"OnPointerExitObject",
			"OnStartScrolling",
			"OnEndScrolling",

		};

		static bool DetectGenericType(object instance, string name)
		{
			Type type = instance.GetType();
			while (type != null)
			{
				if (type.FullName.StartsWith(name, StringComparison.InvariantCulture))
				{
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}

		static bool DetectTileView(object instance)
		{
			Type type = instance.GetType();
			while (type != null)
			{
				if (type.FullName.StartsWith("UIWidgets.TileView`2", StringComparison.InvariantCulture))
				{
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}

		protected virtual void OnEnable()
		{
			if (!IsListViewCustom)
			{
				IsListViewCustom = DetectGenericType(serializedObject.targetObject, "UIWidgets.ListViewCustom`2");
			}
			if (!IsListViewCustomHeight)
			{
				IsListViewCustomHeight = DetectGenericType(serializedObject.targetObject, "UIWidgets.ListViewCustomHeight`2");
			}
			if (!IsTileView)
			{
				IsTileView = DetectGenericType(serializedObject.targetObject, "UIWidgets.TileView`2");
			}
			if (!IsTreeViewCustom)
			{
				IsTreeViewCustom = DetectGenericType(serializedObject.targetObject, "UIWidgets.TreeViewCustom`2");
			}

			if (IsListViewCustomHeight)
			{
				if (!Properties.Contains("ForceAutoHeightCalculation"))
				{
					Properties.Add("ForceAutoHeightCalculation");
				}
				if (!Properties.Contains("itemHeight"))
				{
					Properties.Add("itemHeight");
				}
			}

			if (IsTreeViewCustom)
			{
				Properties.Remove("customItems");
				Properties.Remove("selectedIndex");
				Properties.Remove("direction");
			}

			if (IsListViewCustom)
			{
				Properties.ForEach(x => {
					var property = serializedObject.FindProperty(x);
					if (property!=null)
					{
						SerializedProperties.Add(x, property);
					}
				});
				Events.ForEach(x => {
					var property = serializedObject.FindProperty(x);
					if (property!=null)
					{
						SerializedEvents.Add(x, property);
					}
				});
			}
		}

		public bool ShowEvents;

		public override void OnInspectorGUI()
		{
			if (IsListViewCustom)
			{
				serializedObject.Update();

				SerializedProperties.ForEach(x => EditorGUILayout.PropertyField(x.Value, true));

				EditorGUILayout.BeginVertical();
				ShowEvents = GUILayout.Toggle(ShowEvents, "Events", "Foldout", GUILayout.ExpandWidth(true));
				if (ShowEvents)
				{
					SerializedEvents.ForEach(x => EditorGUILayout.PropertyField(x.Value, true));
				}
				EditorGUILayout.EndVertical();

				serializedObject.ApplyModifiedProperties();

				var showWarning = false;
				Array.ForEach(targets, x => {
					var ourType = x.GetType(); 
					
					var mi = ourType.GetMethod("CanOptimize", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
					
					if (mi!= null){
						var canOptimize = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), x, mi);
						showWarning |= !canOptimize.Invoke();
					}
				});
				if (showWarning)
				{
					if (IsTileView || IsTreeViewCustom)
					{
						EditorGUILayout.HelpBox("Virtualization requires specified ScrollRect and Container should have EasyLayout component.", MessageType.Warning);
					}
					else
					{
						EditorGUILayout.HelpBox("Virtualization requires specified ScrollRect and Container should have EasyLayout or Horizontal or Vertical Layout Group component.", MessageType.Warning);
					}
				}
			}
			else
			{
				DrawDefaultInspector();
			}
		}
	}
}