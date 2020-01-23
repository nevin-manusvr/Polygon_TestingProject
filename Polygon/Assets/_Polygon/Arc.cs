using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manus.Polygon
{
	public class Arc
	{
		#region Fields

		// Data
		private readonly List<ArcPoint> arcPoints;
		private readonly Transform parent;

		// Results
		private Vector3 normal;
		private Vector3 intersectionPoint;

		// Settings
		private float minPointDistance = 0.01f;

		#endregion

		public Arc(Transform parent = null)
		{
			this.arcPoints = new List<ArcPoint>();
			this.parent = parent;
		}

		#region Public Methods

		public void AddMeasurement(Vector3 point)
		{
			if (arcPoints.Count == 0 || Vector3.Distance(arcPoints[arcPoints.Count - 1].point, point) > minPointDistance)
			{
				arcPoints.Add(new ArcPoint(point, parent));
			}
		}

		public void CalculateArc()
		{
			Vector3[] arcPositions = arcPoints.Select(value => value.point).ToArray();

			normal = CalculatePlaneNormal(arcPositions);
			intersectionPoint = CalculateIntersectionPoint(arcPositions);
		}

		public Vector3 GetOffsetToTracker()
		{
			if (parent == null) return Vector3.zero;

			Transform trackerTransform = new GameObject("trackerPosition").transform;
			var offsetsToTracker = new List<Vector3>();

			foreach (ArcPoint value in arcPoints)
			{
				if (value.parent == null) continue;
				TransformValues parentValues = (TransformValues)value.parent;

				trackerTransform.position = Vector3.ProjectOnPlane(parentValues.position, normal);
				trackerTransform.rotation = parentValues.rotation;

				Vector3 pointOffset = trackerTransform.InverseTransformPoint(intersectionPoint);
				offsetsToTracker.Add(pointOffset);
			}

			MonoBehaviour.Destroy(trackerTransform.gameObject);

			return AverageVector(offsetsToTracker.ToArray());
		}

		public float GetArcRadius()
		{
			var distanceToCenter = new List<float>();
			foreach (ArcPoint value in arcPoints)
			{
				distanceToCenter.Add(Vector3.Distance(intersectionPoint, value.point));
			}

			// Get the average off all the distances
			float averageDistance = 0;
			foreach (float distance in distanceToCenter)
			{
				averageDistance += distance;
			}

			return averageDistance / distanceToCenter.Count;
		}

		#endregion

		#region Private Methods

		private Vector3 AverageVector(Vector3[] vectors)
		{
			if (vectors.Length == 0) return Vector3.zero;

			Vector3 average = Vector3.zero;
			
			foreach (var vec in vectors)
			{
				average += vec;
			}

			average /= vectors.Length;

			return average;
		}

		private Vector3 CalculatePlaneNormal(Vector3[] arcPositions)
		{
			var normals = new List<Vector3>();

			for (int i = 0; i < arcPositions.Length - 2; i++)
			{
				int[] indexes = { i, i + 1, i + 2 };
				Vector3 normal = Vector3.Cross(arcPositions[indexes[1]] - arcPositions[indexes[0]], arcPositions[indexes[2]] - arcPositions[indexes[0]]).normalized;

				if (normal != Vector3.zero)
				{
					if (normals.Count > 0)
					{
						if (Vector3.Dot(AverageVector(normals.ToArray()), normal) < 0)
						{
							normal *= -1f;
						}
					}

					normals.Add(normal);
				}

				Debug.DrawRay(AverageVector(arcPositions), normal * 0.1f, Color.blue, 1f);
			}

			Debug.DrawRay(AverageVector(arcPositions), AverageVector(normals.ToArray()), Color.white, 1f);
			return AverageVector(normals.ToArray());
		}

		private Vector3 CalculateIntersectionPoint(Vector3[] arcPositions)
		{
			var lines = new List<Ray>();
			for (int i = 0; i < arcPositions.Length - 1; i++)
			{
				int[] indexes = { i, i + 1 };
				Vector3 averagePoint = (Vector3.ProjectOnPlane(arcPositions[indexes[0]], normal) + Vector3.ProjectOnPlane(arcPositions[indexes[1]], normal)) / 2f;
				Vector3 toCenterDirection = Vector3.Cross(arcPositions[indexes[0]] - arcPositions[indexes[1]], normal).normalized;

				if (lines.Count > 0)
				{
					if (Vector3.Dot(lines[lines.Count - 1].direction, toCenterDirection) < 0)
					{
						toCenterDirection *= -1;
					}
				}
				lines.Add(new Ray(averagePoint, toCenterDirection));
				Debug.DrawRay(averagePoint, toCenterDirection * 0.3f, Color.yellow, 1f);
			}

			var intersections = new List<Vector3>();
			foreach (var currentRay in lines)
			{
				foreach (Ray otherRay in lines)
				{
					if (currentRay.origin == otherRay.origin) continue;

					if (LineLineIntersection(out Vector3 intersection, currentRay.origin, currentRay.direction, otherRay.origin, otherRay.direction))
					{
						intersections.Add(intersection);
						Debug.DrawRay(intersection, normal * 0.01f, Color.cyan, 1f);
					}
				}
			}

			return AverageVector(intersections.ToArray());
		}

		private bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
		{

			Vector3 lineVec3 = linePoint2 - linePoint1;
			Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
			Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

			float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

			// is coplanar, and not parallel
			if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
			{
				float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
				intersection = linePoint1 + (lineVec1 * s);
				return true;
			}
			else
			{
				intersection = Vector3.zero;
				return false;
			}
		}

		#endregion

		#region Structs

		public struct TransformValues
		{
			public Vector3 position;
			public Quaternion rotation;

			public TransformValues(Transform transform)
			{
				position = transform.position;
				rotation = transform.rotation;
			}
		}

		public struct ArcPoint
		{
			public Vector3 point;
			public TransformValues? parent;

			public ArcPoint(Vector3 point, Transform parent)
			{
				this.point = point;
				this.parent = parent == null ? null : (TransformValues?)new TransformValues(parent);
			}
		}

		public struct Line
		{
			public Vector3 start;
			public Vector3 direction;

			public Line(Vector3 start, Vector3 direction)
			{
				this.start = start;
				this.direction = direction;
			}
		}

		#endregion
	}
}
