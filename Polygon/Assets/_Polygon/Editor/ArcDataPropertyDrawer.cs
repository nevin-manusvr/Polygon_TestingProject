using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Manus.Core.Utility;

namespace Manus.Polygon
{
	[CustomPropertyDrawer(typeof(ArcCalibrationStep.ArcDataToSave))]
	public class ArcDataPropertyDrawer : PropertyDrawer
	{
		private int lineHeight = 4;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Using BeginProperty / EndProperty on the parent property means that
			// __prefab__ override logic works on the entire property.

			EditorGUI.BeginProperty(position, label, property);

			// Get variables
			var currentType = (ArcCalibrationStep.ArcDataType)property.FindPropertyRelative("dataType").intValue;
			//var localOffset = property.FindPropertyRelative("useTrackerLocal");
			//var parent = property.FindPropertyRelative("useParentTracker");

			// Draw label
			//string arrayIndex = Regex.Replace(property.displayName, "[^0-9]", string.Empty);
			ArcCalibrationStep.ArcDataType dataType = (ArcCalibrationStep.ArcDataType)property.FindPropertyRelative("dataType").intValue;

			string dataName = string.Empty;
			switch (dataType)
			{
				case ArcCalibrationStep.ArcDataType.OffsetToTracker:
					OffsetsToTrackers trackerOffsetType = (OffsetsToTrackers)property.FindPropertyRelative("trackerOffset").intValue;
					dataName = $"{trackerOffsetType} ({property.FindPropertyRelative("arcPositionIndex").intValue})";
					break;
				case ArcCalibrationStep.ArcDataType.Length:
					BodyMeasurements lengthBodyMeasurementType = (BodyMeasurements)property.FindPropertyRelative("measurement").intValue;
					SerializedProperty indices = property.FindPropertyRelative("arcMeasurementIndices");
					string index = string.Empty;

					for (int i = 0; i < indices.arraySize; i++)
					{
						if (i != 0)
							index += ", ";
						index += indices.GetArrayElementAtIndex(i).intValue.ToString();
					}

					dataName = $"{lengthBodyMeasurementType} ({index})";

					break;
				case ArcCalibrationStep.ArcDataType.Distance:
					BodyMeasurements distanceBodyMeasurementType = (BodyMeasurements)property.FindPropertyRelative("distanceMeasurement").intValue;
					string distanceIndex = string.Empty;

					ArcCalibrationStep.ArcPointType point1Type = (ArcCalibrationStep.ArcPointType)property.FindPropertyRelative("point1").FindPropertyRelative("pointType").intValue;
					switch (point1Type)
					{
						case ArcCalibrationStep.ArcPointType.ArcPoint:
							distanceIndex += property.FindPropertyRelative("point1").FindPropertyRelative("arcIndex").intValue;
							break;
						case ArcCalibrationStep.ArcPointType.Tracker:
							VRTrackerType trackerTypePoint1 = (VRTrackerType)property.FindPropertyRelative("point1").FindPropertyRelative("tracker").intValue;
							distanceIndex += trackerTypePoint1;

							if (property.FindPropertyRelative("point2").FindPropertyRelative("useLocalOffset").boolValue)
								distanceIndex += " (Local)";
							break;
					}

					distanceIndex += ", ";

					ArcCalibrationStep.ArcPointType point2Type = (ArcCalibrationStep.ArcPointType)property.FindPropertyRelative("point2").FindPropertyRelative("pointType").intValue;
					switch (point2Type)
					{
						case ArcCalibrationStep.ArcPointType.ArcPoint:
							distanceIndex += property.FindPropertyRelative("point2").FindPropertyRelative("arcIndex").intValue;
							break;
						case ArcCalibrationStep.ArcPointType.Tracker:
							VRTrackerType trackerTypePoint2 = (VRTrackerType)property.FindPropertyRelative("point2").FindPropertyRelative("tracker").intValue;
							distanceIndex += trackerTypePoint2;

							if (property.FindPropertyRelative("point2").FindPropertyRelative("useLocalOffset").boolValue)
								distanceIndex += " (Local)";
							break;
					}

					dataName = $"{distanceBodyMeasurementType} ({distanceIndex})";
					break;
				case ArcCalibrationStep.ArcDataType.Direction:
					Axis directionAxis = (Axis)property.FindPropertyRelative("directionAxis").intValue;
					dataName = $"{directionAxis} ({property.FindPropertyRelative("arcDirectionIndex").intValue})";
					break;
			}

			label.text = $"{dataType}: {dataName}";

			// Foldout group
			var expendRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
			property.isExpanded = EditorGUI.Foldout(expendRect, property.isExpanded, label);

			// Don't make child fields be indented
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel += 2;

			// Calculate rects
			var dataTypeRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + lineHeight, position.width, EditorGUIUtility.singleLineHeight);
			// Tracker offset
			var trackerOffsetEnumRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 2, position.width, EditorGUIUtility.singleLineHeight);
			var trackerOffetArcIndexRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 3, position.width, EditorGUIUtility.singleLineHeight);
			// Length
			SerializedProperty lengthIndicesProperty = property.FindPropertyRelative("arcMeasurementIndices");
			var lengthMeasurementTypeRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 2, position.width, EditorGUIUtility.singleLineHeight);
			var lengthIndicesRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 3, position.width, EditorGUI.GetPropertyHeight(lengthIndicesProperty));
			// Distance
			SerializedProperty distancePoint1 = property.FindPropertyRelative("point1");
			SerializedProperty distancePoint2 = property.FindPropertyRelative("point2");
			var distanceEnumRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 2, position.width, EditorGUIUtility.singleLineHeight);
			var distancePoint1Rect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 3, position.width, EditorGUI.GetPropertyHeight(distancePoint1));
			var distancePoint2Rect = new Rect(position.x, position.y + EditorGUI.GetPropertyHeight(distancePoint1) + lineHeight + (EditorGUIUtility.singleLineHeight + lineHeight) * 3, position.width, EditorGUI.GetPropertyHeight(distancePoint2));
			// Direction
			var directionAxisRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 2, position.width, EditorGUIUtility.singleLineHeight);
			var arcDirectionIndexRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + lineHeight) * 3, position.width, EditorGUIUtility.singleLineHeight);


			// Draw fields
			if (property.isExpanded)
			{
				EditorGUI.PropertyField(dataTypeRect, property.FindPropertyRelative("dataType"), new GUIContent("Data type"));

				switch (currentType)
				{
					case ArcCalibrationStep.ArcDataType.OffsetToTracker:
						EditorGUI.PropertyField(trackerOffsetEnumRect, property.FindPropertyRelative("trackerOffset"), new GUIContent("Tracker offset"));
						EditorGUI.PropertyField(trackerOffetArcIndexRect, property.FindPropertyRelative("arcPositionIndex"), new GUIContent("Arc index"));
						break;
					case ArcCalibrationStep.ArcDataType.Length:
						EditorGUI.PropertyField(lengthMeasurementTypeRect, property.FindPropertyRelative("measurement"), new GUIContent("Measurement"));
						EditorGUI.PropertyField(lengthIndicesRect, lengthIndicesProperty, new GUIContent("Arc indices"), true);
						break;
					case ArcCalibrationStep.ArcDataType.Distance:
						EditorGUI.PropertyField(distanceEnumRect, property.FindPropertyRelative("distanceMeasurement"), new GUIContent("Measurement"));
						EditorGUI.PropertyField(distancePoint1Rect, distancePoint1, new GUIContent("Point 1"), true);
						EditorGUI.PropertyField(distancePoint2Rect, distancePoint2, new GUIContent("Point 2"), true);
						break;
					case ArcCalibrationStep.ArcDataType.Direction:
						EditorGUI.PropertyField(directionAxisRect, property.FindPropertyRelative("directionAxis"), new GUIContent("Axis"));
						EditorGUI.PropertyField(arcDirectionIndexRect, property.FindPropertyRelative("arcDirectionIndex"), new GUIContent("Arc index"));
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
			SerializedProperty lengthIndicesProperty = property.FindPropertyRelative("arcMeasurementIndices");
			SerializedProperty distancePoint1Property = property.FindPropertyRelative("point1");
			SerializedProperty distancePoint2Property = property.FindPropertyRelative("point2");

			var currentType = (ArcCalibrationStep.ArcDataType)property.FindPropertyRelative("dataType").intValue;
			int lines = 0;
			float extraLength = 0;

			switch (currentType)
			{
				case ArcCalibrationStep.ArcDataType.OffsetToTracker:
					lines = 4;
					break;
				case ArcCalibrationStep.ArcDataType.Length:
					lines = 3;
					if (property.isExpanded)
						extraLength += EditorGUI.GetPropertyHeight(lengthIndicesProperty) + lineHeight;
					break;
				case ArcCalibrationStep.ArcDataType.Distance:
					lines = 3;
					if (property.isExpanded)
					{
						extraLength += EditorGUI.GetPropertyHeight(distancePoint1Property) + lineHeight;
						extraLength += EditorGUI.GetPropertyHeight(distancePoint2Property) + lineHeight;
					}
					break;
				case ArcCalibrationStep.ArcDataType.Direction:
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

