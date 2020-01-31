using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using Manus.Core.Utility;

namespace Manus.Polygon
{
	[CustomPropertyDrawer(typeof(ArcCalibrationStep.ArcSettings))]
	public class ArcSettingsPropertyDrawer : PropertyDrawer
	{
		private int lineHeight = 4;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// __prefab__ override logic works on the entire property.

			EditorGUI.BeginProperty(position, label, property);

			// Get variables
			var localOffset = property.FindPropertyRelative("useTrackerLocal");
			var parent = property.FindPropertyRelative("useParentTracker");

			// Draw label
			string arrayIndex = Regex.Replace(property.displayName, "[^0-9]", string.Empty);
			VRTrackerType trackerName = (VRTrackerType)property.FindPropertyRelative("tracker").intValue;
			OffsetsToTrackers offsetName = (OffsetsToTrackers)property.FindPropertyRelative("localOffset").intValue;
			VRTrackerType parentName = (VRTrackerType)property.FindPropertyRelative("parentTracker").intValue;

			label.text = (arrayIndex.Length > 0 ? $"{arrayIndex} - " : string.Empty) +
			             $"{trackerName} Tracker" +
			             (localOffset.boolValue ? $" - (Local: {offsetName})" : string.Empty) +
						 (parent.boolValue ? $" - (Parent: {parentName})" : string.Empty);

			// Foldout group
			var expendRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
			property.isExpanded = EditorGUI.Foldout(expendRect, property.isExpanded, label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel += 2;

			// Calculate rects
			var trackerRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + lineHeight, position.width, EditorGUIUtility.singleLineHeight);
			var useLocalRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 2, position.width * .45f, EditorGUIUtility.singleLineHeight);
			var useLocalEnumRect = new Rect(position.x + position.width * .45f, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 2, position.width * .55f, EditorGUIUtility.singleLineHeight);

			var useParentRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 3, position.width * .45f, EditorGUIUtility.singleLineHeight);
			var useParentEnumRect = new Rect(position.x + position.width * .45f, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 3, position.width * .55f, EditorGUIUtility.singleLineHeight);

			// Draw fields
			if (property.isExpanded)
			{
				EditorGUI.PropertyField(trackerRect, property.FindPropertyRelative("tracker"), new GUIContent("Tracker"));
				EditorGUI.PropertyField(useLocalRect, property.FindPropertyRelative("useTrackerLocal"), new GUIContent("Use tracker local"));

				if (localOffset.boolValue)
					EditorGUI.PropertyField(useLocalEnumRect, property.FindPropertyRelative("localOffset"), GUIContent.none);

				EditorGUI.PropertyField(useParentRect, property.FindPropertyRelative("useParentTracker"), new GUIContent("Use parent"));

				if (parent.boolValue)
					EditorGUI.PropertyField(useParentEnumRect, property.FindPropertyRelative("parentTracker"), GUIContent.none);
			}

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			// The 6 comes from extra spacing between the fields (2px each)
			return property.isExpanded
				       ? (EditorGUIUtility.singleLineHeight + lineHeight) * 4 - lineHeight
				       : (EditorGUIUtility.singleLineHeight + lineHeight) * 1 - lineHeight;
		}
	}
}
