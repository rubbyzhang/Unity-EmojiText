using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace EasyLayout {
	[CustomEditor(typeof(EasyLayout), true)]
	[CanEditMultipleObjects]
	public class EasyLayoutEditor : Editor
	{
		Dictionary<string,SerializedProperty> sProperties = new Dictionary<string,SerializedProperty>();
		string[] properties = new string[]{
			"GroupPosition",
			"Stacking",
			"LayoutType",
			"RowAlign",
			"InnerAlign",
			"CellAlign",
			"Spacing",
			"Symmetric",
			"Margin",
			"MarginTop",
			"MarginBottom",
			"MarginLeft",
			"MarginRight",
			"TopToBottom",
			"RightToLeft",
			"SkipInactive",
			/*
			"ControlWidth",
			"MaxWidth",
			"ControlHeight",
			"MaxHeight",
			*/
			"ChildrenWidth",
			"ChildrenHeight",
		};

		bool AutoUpdate = true;

		protected virtual void OnEnable()
		{
			Array.ForEach(targets, x => ((EasyLayout)x).Upgrade());
			sProperties.Clear();

			Array.ForEach(properties, x => sProperties.Add(x, serializedObject.FindProperty(x)));
		}
		
		public override void OnInspectorGUI()
		{


			serializedObject.Update();

			EditorGUILayout.PropertyField(sProperties["GroupPosition"], true);
			EditorGUILayout.PropertyField(sProperties["Stacking"], true);
			EditorGUILayout.PropertyField(sProperties["LayoutType"], true);

			EditorGUI.indentLevel++;
			if (sProperties["LayoutType"].enumValueIndex==0)
			{
				EditorGUILayout.PropertyField(sProperties["RowAlign"], true);
				EditorGUILayout.PropertyField(sProperties["InnerAlign"], true);
			}
			if (sProperties["LayoutType"].enumValueIndex==1)
			{
				EditorGUILayout.PropertyField(sProperties["CellAlign"], true);
			}
			EditorGUI.indentLevel--;

			EditorGUILayout.PropertyField(sProperties["Spacing"], true);
			EditorGUILayout.PropertyField(sProperties["Symmetric"], true);
			if (sProperties["Symmetric"].boolValue)
			{
				EditorGUILayout.PropertyField(sProperties["Margin"], true);
			}
			else
			{
				EditorGUILayout.PropertyField(sProperties["MarginTop"], true);
				EditorGUILayout.PropertyField(sProperties["MarginBottom"], true);
				EditorGUILayout.PropertyField(sProperties["MarginLeft"], true);
				EditorGUILayout.PropertyField(sProperties["MarginRight"], true);
			}

			EditorGUILayout.PropertyField(sProperties["SkipInactive"], true);
			EditorGUILayout.PropertyField(sProperties["RightToLeft"], true);
			EditorGUILayout.PropertyField(sProperties["TopToBottom"], true);

			EditorGUILayout.PropertyField(sProperties["ChildrenWidth"], true);
			EditorGUILayout.PropertyField(sProperties["ChildrenHeight"], true);
			/*
			EditorGUILayout.PropertyField(sProperties["ControlWidth"], true);
			if (sProperties["ControlWidth"].boolValue)
			{
				EditorGUILayout.PropertyField(sProperties["MaxWidth"], true);
			}
			EditorGUILayout.PropertyField(sProperties["ControlHeight"], true);
			if (sProperties["ControlHeight"].boolValue)
			{
				EditorGUILayout.PropertyField(sProperties["MaxHeight"], true);
			}
			*/

			if (targets.Length==1)
			{
				var script = (EasyLayout)target;

				EditorGUILayout.LabelField("Block size", string.Format("{0}x{1}", script.BlockSize.x, script.BlockSize.y));
				EditorGUILayout.LabelField("UI size", string.Format("{0}x{1}", script.UISize.x, script.UISize.y));
			}

			serializedObject.ApplyModifiedProperties();
			if (AutoUpdate)
			{
				UpdateLayout();
			}
			else
			{
				if (GUILayout.Button("Apply"))
				{
					UpdateLayout();
				}
			}
		}

		void UpdateLayout()
		{
			Array.ForEach(targets, x => ((EasyLayout)x).UpdateLayout());
		}


	}
}