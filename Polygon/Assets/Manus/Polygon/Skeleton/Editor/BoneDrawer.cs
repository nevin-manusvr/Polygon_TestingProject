﻿using UnityEditor;
using UnityEngine;

namespace Manus.Polygon.Skeleton
{
	[CustomPropertyDrawer(typeof(Bone))]
	public class BoneDrawer : PropertyDrawer
	{
		// Draw the property inside the given rect
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// __prefab__ override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			// position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
			
			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel += 0;
			
			// Calculate rects
			float margin = 3f;
			float iconSize = EditorGUIUtility.singleLineHeight - margin * 2f;

			var amountRect = new Rect(position.x, position.y, position.width - iconSize - margin, position.height);
			var optionalRect = new Rect(position.x, position.y + margin, iconSize, iconSize);

			// Draw fields - passs GUIContent.none to each so they are drawn without labels
			EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("bone"), label);

			EditorGUI.DrawRect(optionalRect, Color.red);

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}

}