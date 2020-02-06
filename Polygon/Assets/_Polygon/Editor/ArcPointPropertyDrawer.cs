using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Manus.Polygon
{
	[CustomPropertyDrawer(typeof(ArcCalibrationStep.ArcPoint))]
	public class ArcPointPropertyDrawer : PropertyDrawer
	{
		private int lineHeight = 4;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// __prefab__ override logic works on the entire property.

			EditorGUI.BeginProperty(position, label, property);

			// Get variables
			var currentType = (ArcCalibrationStep.PointType)property.FindPropertyRelative("pointType").intValue;
			var localOffset = property.FindPropertyRelative("useLocalOffset");

			// Foldout group
			var expendRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
			property.isExpanded = EditorGUI.Foldout(expendRect, property.isExpanded, label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel += 2;

			// Calculate rects
			var pointTypeRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + lineHeight, position.width, EditorGUIUtility.singleLineHeight);
			
			var arcIndexRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 2, position.width, EditorGUIUtility.singleLineHeight);
			
			var trackerRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 2, position.width, EditorGUIUtility.singleLineHeight);
			var useLocalRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 3, position.width * .45f, EditorGUIUtility.singleLineHeight);
			var useLocalEnumRect = new Rect(position.x + position.width * .45f, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 3, position.width * .55f, EditorGUIUtility.singleLineHeight);

			// Draw fields
			if (property.isExpanded)
			{
				EditorGUI.PropertyField(pointTypeRect, property.FindPropertyRelative("pointType"), new GUIContent("point type"));

				switch (currentType)
				{
					case ArcCalibrationStep.PointType.ArcPoint:
						EditorGUI.PropertyField(arcIndexRect, property.FindPropertyRelative("arcIndex"), new GUIContent("Arc index"));
						break;
					case ArcCalibrationStep.PointType.Tracker:
						EditorGUI.PropertyField(trackerRect, property.FindPropertyRelative("tracker"), new GUIContent("Tracker"));
						EditorGUI.PropertyField(useLocalRect, property.FindPropertyRelative("useLocalOffset"), new GUIContent("Use local offset"));
						
						if (localOffset.boolValue)
							EditorGUI.PropertyField(useLocalEnumRect, property.FindPropertyRelative("offset"), GUIContent.none);

						break;
				}
			}

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			// Values
			var currentType = (ArcCalibrationStep.PointType)property.FindPropertyRelative("pointType").intValue;
			int lines = 0;
			float extraLength = 0;

			switch (currentType)
			{
				case ArcCalibrationStep.PointType.ArcPoint:
					lines = 3;
					break;
				case ArcCalibrationStep.PointType.Tracker:
					lines = 4;
					break;
			}

			float totalHeight = property.isExpanded
				                    ? (EditorGUIUtility.singleLineHeight + lineHeight) * lines - lineHeight
				                    : (EditorGUIUtility.singleLineHeight + lineHeight) * 1 - lineHeight;

			return totalHeight + extraLength;
		}
	}
}

