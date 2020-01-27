﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Manus.Polygon
{
	[CustomPropertyDrawer(typeof(ArcCalibrationStep.ArcSettings))]
	public class ArcSettingsPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// __prefab__ override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Calculate rects
			var amountRect = new Rect(position.x, position.y, 30, position.height);
			var unitRect = new Rect(position.x + 35, position.y + 15, 50, position.height);
			//var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

			// Draw fields - passs GUIContent.none to each so they are drawn without labels
			EditorGUI.LabelField(amountRect, new GUIContent("tracker"));
			EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("tracker"), GUIContent.none);
			// EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("useTrackerWithOffsets"), GUIContent.none);

			//EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("tracker"), GUIContent.none);


			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}
}
