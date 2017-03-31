using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Reflection;
using System.Linq;

namespace UIWidgets
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ListViewItem), true)]
	public class ListViewItemEditor : Editor
	{
		protected Dictionary<string,SerializedProperty> SerializedProperties = new Dictionary<string,SerializedProperty>();
		protected Dictionary<string,SerializedProperty> SerializedEvents = new Dictionary<string,SerializedProperty>();

		protected List<string> Properties = new List<string>();
		protected List<string> Events = new List<string>();

		protected virtual void OnEnable()
		{
			Properties.Clear();
			Events.Clear();
			SerializedProperties.Clear();
			SerializedEvents.Clear();

			var property = serializedObject.GetIterator();
			property.NextVisible(true);
			while (property.NextVisible(false))
			{
				AddProperty(property);
			}

			Events.Sort();

			Properties.ForEach(x => {
				SerializedProperties.Add(x, serializedObject.FindProperty(x));
			});
			Events.ForEach(x => {
				SerializedEvents.Add(x, serializedObject.FindProperty(x));
			});
		}

		void AddProperty(SerializedProperty property)
		{
			if (IsEvent(property))
			{
				Events.Add(property.name);
			}
			else
			{
				Properties.Add(property.name);
			}
		}

		bool IsEvent(SerializedProperty property)
		{
			var object_type = property.serializedObject.targetObject.GetType();
			var property_type = object_type.GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

			return typeof(UnityEventBase).IsAssignableFrom(property_type.FieldType);
		}

		protected bool ShowEvents;

		public override void OnInspectorGUI()
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
		}
	}
}