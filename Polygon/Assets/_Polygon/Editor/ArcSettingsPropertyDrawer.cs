using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Manus.Polygon
{
	//[CustomPropertyDrawer(typeof(ArcCalibrationStep.ArcSettings))]
	//public class ArcSettingsPropertyDrawer : PropertyDrawer
	//{
		//public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		//{
		//	// Using BeginProperty / EndProperty on the parent property means that
		//	// __prefab__ override logic works on the entire property.

		//	EditorGUI.BeginProperty(position, label, property);

		//	// Draw label
		//	label.text = "property"; // TODO: change name according to the data
		//	EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		//	// Don't make child fields be indented
		//	var indent = EditorGUI.indentLevel;
		//	// EditorGUI.indentLevel = 0;
		//	EditorGUI.indentLevel++;

		//	// Calculate rects
		//	var trackerLabel = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2f, position.width * .2f, EditorGUIUtility.singleLineHeight);
		//	var trackerEnum = new Rect(position.x + position.width * 0.2f, position.y + EditorGUIUtility.singleLineHeight + 2f, position.width * 0.4f, EditorGUIUtility.singleLineHeight);
		//	var localLabel = new Rect(position.x + position.width * 0.65f, position.y + EditorGUIUtility.singleLineHeight + 2f, position.width * 0.2f, EditorGUIUtility.singleLineHeight);
		//	var localBool = new Rect(position.x + position.width * 0.85f, position.y + EditorGUIUtility.singleLineHeight + 2f, position.width * 0.15f, EditorGUIUtility.singleLineHeight);

		//	var positionOffsetLabel = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2f) * 2, position.width * .2f, EditorGUIUtility.singleLineHeight);
		//	var positionOffsetBool = new Rect(position.x + position.width * .2f, position.y + (EditorGUIUtility.singleLineHeight + 2f) * 2, position.width * .15f, EditorGUIUtility.singleLineHeight);

		//	var parentTrackerLabel = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2f) * 3, position.width * .2f, EditorGUIUtility.singleLineHeight);
		//	var parentTrackerEnum = new Rect(position.x + position.width * .2f, position.y + (EditorGUIUtility.singleLineHeight + 2f) * 3, position.width * .4f, EditorGUIUtility.singleLineHeight);
		//	var offsetEnum = new Rect(position.x + position.width * .6f, position.y + (EditorGUIUtility.singleLineHeight + 2f) * 3, position.width * .4f, EditorGUIUtility.singleLineHeight);

		//	var measurementLabel = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2f) * 3, position.width * .2f, EditorGUIUtility.singleLineHeight);
		//	var measurementEnum = new Rect(position.x + position.width * .2f, position.y + (EditorGUIUtility.singleLineHeight + 2f) * 3, position.width * .66f, EditorGUIUtility.singleLineHeight);

		//	// Draw fields
		//	EditorGUI.LabelField(trackerLabel, new GUIContent("tracker : "));
		//	EditorGUI.PropertyField(trackerEnum, property.FindPropertyRelative("tracker"), GUIContent.none);
		//	EditorGUI.LabelField(localLabel, new GUIContent("use local : "));
		//	EditorGUI.PropertyField(localBool, property.FindPropertyRelative("useTrackerWithOffsets"), GUIContent.none);

		//	EditorGUI.LabelField(positionOffsetLabel, new GUIContent("Add offset : "));
		//	EditorGUI.PropertyField(positionOffsetBool, property.FindPropertyRelative("getPositionOffset"), GUIContent.none);

		//	var positionOffset = property.FindPropertyRelative("getPositionOffset");
		//	if (positionOffset.boolValue)
		//	{
		//		EditorGUI.LabelField(parentTrackerLabel, new GUIContent("Parent tracker : "));
		//		EditorGUI.PropertyField(parentTrackerEnum, property.FindPropertyRelative("parentTracker"), GUIContent.none);
		//		EditorGUI.PropertyField(offsetEnum, property.FindPropertyRelative("offsetToTracker"), GUIContent.none);
		//	}
		//	else
		//	{
		//		EditorGUI.LabelField(measurementLabel, new GUIContent("Measurement : "));
		//		EditorGUI.PropertyField(measurementEnum, property.FindPropertyRelative("bodyMeasurement"), GUIContent.none);
		//	}

		//	// Set indent back to what it was
		//	EditorGUI.indentLevel = indent;

		//	EditorGUI.EndProperty();
		//}

		//public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		//{
		//	// The 6 comes from extra spacing between the fields (2px each)
		//	return EditorGUIUtility.singleLineHeight * 4 + 6;
		//}
	//}
}
