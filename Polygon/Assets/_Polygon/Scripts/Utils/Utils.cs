using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{

	public static class Utils
	{
		#region stringUtils

		public static Transform FindDeepChildTransform(Transform trans, string[] childCriteria)
		{
			foreach (Transform child in trans)
			{
				if (StringContainsCriteria(child.name, childCriteria))
				{
					return child;
				}

				if (child.childCount > 0)
				{
					Transform foundTransform = FindDeepChildTransform(child, childCriteria);
					if (foundTransform != null)
					{
						return foundTransform;
					}
				}
			}
			return null;
		}

		public static bool StringContainsCriteria(string stringToCheck, string[] criteria)
		{
			int count = 0;
			foreach (var crit in criteria)
			{
				if (stringToCheck.ToUpper().Contains(crit.ToUpper()))
				{
					count++;
				}
			}
			return count == criteria.Length;
		}

		#endregion

		#region DirectionUtils

		public static Direction GetClosestDirection(Quaternion rotation, Vector3 direction)
		{
			Direction closestDirection = Direction.forward;
			float closestDistance = float.MaxValue;

			foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
			{
				Vector3 directionValue = Vector3.zero;
				switch (dir)
				{
					case Direction.forward:
						directionValue = rotation * Vector3.forward;
						break;
					case Direction.back:
						directionValue = rotation * Vector3.back;
						break;
					case Direction.left:
						directionValue = rotation * Vector3.left;
						break;
					case Direction.right:
						directionValue = rotation * Vector3.right;
						break;
					case Direction.up:
						directionValue = rotation * Vector3.up;
						break;
					case Direction.down:
						directionValue = rotation * Vector3.down;
						break;
				}

				float distance = Vector3.Distance(directionValue, direction);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestDirection = dir;
				}
			}

			return closestDirection;
		}

		public static Vector3 GetConvertedDirection(Quaternion rotation, Direction direction)
		{
			switch (direction)
			{
				case Direction.forward:
					return rotation * Vector3.forward;
				case Direction.back:
					return rotation * -Vector3.forward;
				case Direction.left:
					return rotation * -Vector3.right;
				case Direction.right:
					return rotation * Vector3.right;
				case Direction.up:
					return rotation * Vector3.up;
				case Direction.down:
					return rotation * -Vector3.up;
				default:
					return Vector3.zero;
			}
		}

		public static Vector3 ConvertDirectionToVector(Direction direction)
		{
			switch (direction)
			{
				case Direction.forward:
					return Vector3.forward;
				case Direction.back:
					return Vector3.back;
				case Direction.right:
					return Vector3.right;
				case Direction.left:
					return Vector3.left;
				case Direction.up:
					return Vector3.up;
				case Direction.down:
					return Vector3.down;
				default:
					return Vector3.zero;
			}
		}

		public static Quaternion GetRotationDifference(Direction bone1Forward, Direction bone1Up, Direction bone2Forward, Direction bone2Up)
		{
			Quaternion bone1Rotation = Quaternion.LookRotation(ConvertDirectionToVector(bone1Forward), ConvertDirectionToVector(bone1Up));
			Quaternion bone2Rotation = Quaternion.LookRotation(ConvertDirectionToVector(bone2Forward), ConvertDirectionToVector(bone2Up));

			Quaternion diff = Quaternion.Inverse(bone1Rotation) * bone2Rotation;

			Vector3 startPosition = new Vector3(2, 2, 2);
			Debug.DrawRay(startPosition, bone1Rotation * Vector3.forward, Color.blue, 1f);
			Debug.DrawRay(startPosition, bone1Rotation * Vector3.right, Color.red, 1f);
			Debug.DrawRay(startPosition, bone1Rotation * Vector3.up, Color.green, 1f);

			startPosition += Vector3.up * 2f;
			Debug.DrawRay(startPosition, bone2Rotation * Vector3.forward, Color.blue, 1f);
			Debug.DrawRay(startPosition, bone2Rotation * Vector3.right, Color.red, 1f);
			Debug.DrawRay(startPosition, bone2Rotation * Vector3.up, Color.green, 1f);

			startPosition -= Vector3.up * 1f + Vector3.forward * 2;
			Debug.DrawRay(startPosition, bone1Rotation * diff * Vector3.forward, Color.blue, 1f);
			Debug.DrawRay(startPosition, bone1Rotation * diff * Vector3.right, Color.red, 1f);
			Debug.DrawRay(startPosition, bone1Rotation * diff * Vector3.up, Color.green, 1f);

			return diff;
		}

		#endregion
	}
}


public enum Direction
{
	forward,
	back,
	left,
	right,
	up,
	down
}