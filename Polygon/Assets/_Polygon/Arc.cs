using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Manus.Core.VR;
using Manus.Core.Utility;

namespace Manus.Polygon
{

	public class Arc
	{
		#region Fields

		// Data
		private readonly List<ArcPoint> arcPoints;
		private readonly TrackerReference trackers;
		private readonly VRTrackerType parentTracker;

		// Results
		private Vector3 normal;
		private Vector3 intersectionPoint;
		private Vector3 averageArcPosition;

		// Settings
		private float minPointDistance = 0.01f;

		#endregion

		#region Properties

		public Vector3 IntersectionPoint
		{
			get { return intersectionPoint; }
		}

		public Vector3 planeNormal
		{
			get { return normal; }
		}

		#endregion

		public Arc()
		{
			this.arcPoints = new List<ArcPoint>();
			this.trackers = null;
		}

		public Arc(TrackerReference trackers, VRTrackerType parentTrackerType) // TODO: add local offset
		{
			this.arcPoints = new List<ArcPoint>();
			this.trackers = trackers;
			this.parentTracker = parentTrackerType;
		}

		#region Public Methods

		public void AddMeasurement(Vector3 point)
		{
			if (arcPoints.Count == 0 || Vector3.Distance(arcPoints[arcPoints.Count - 1].point, point) > minPointDistance)
			{
				TransformValues? trackerTransform = trackers?.GetTracker(parentTracker);
				arcPoints.Add(new ArcPoint(point, trackerTransform));
			}
		}

		public void CalculateArc()
		{
			Vector3[] arcPositions = arcPoints.Select(value => value.point).ToArray();
			averageArcPosition = AverageVector(arcPositions);

			normal = CalculatePlaneNormal(arcPositions);
			intersectionPoint = CalculateIntersectionPoint(arcPositions);
		}

		public Vector3 GetOffsetToTracker()
		{
			if (trackers == null) return Vector3.zero;

			// Transform trackerTransform = new GameObject("trackerPosition").transform;
			var offsetsToTracker = new List<Vector3>();

			foreach (ArcPoint value in arcPoints)
			{
				if (value.parent == null) continue;
				TransformValues parentValues = (TransformValues)value.parent;

				Matrix4x4 trackerMatrix = Matrix4x4.TRS(parentValues.position, parentValues.rotation, Vector3.one);
				trackerMatrix = trackerMatrix.inverse;

				//trackerTransform.position = parentValues.position;
				//trackerTransform.rotation = parentValues.rotation;

				Vector3 pointOffset = trackerMatrix.MultiplyPoint3x4(intersectionPoint);// trackerTransform.InverseTransformPoint(intersectionPoint);
				offsetsToTracker.Add(pointOffset);
			}

			// MonoBehaviour.Destroy(trackerTransform.gameObject); // TODO: remove all transform stuff

			return AverageVector(offsetsToTracker.ToArray());
		}

		public Vector3 GetArcNormalFromTracker()
		{
			if (trackers == null) return Vector3.zero;

			var normalsFromTracker = new List<Vector3>();

			foreach (ArcPoint value in arcPoints)
			{
				if (value.parent == null) continue;
				TransformValues parentValues = (TransformValues)value.parent;

				Matrix4x4 trackerMatrix = Matrix4x4.TRS(parentValues.position, parentValues.rotation, Vector3.one);
				trackerMatrix = trackerMatrix.inverse;


				Vector3 trackerNormal = trackerMatrix.MultiplyVector(normal);
				normalsFromTracker.Add(trackerNormal);
			}

			return AverageVector(normalsFromTracker.ToArray());
		}

		public float GetArcRadius()
		{
			var distanceToCenter = new List<float>();
			foreach (ArcPoint value in arcPoints)
			{
				Vector3 arcPoint = PlacePointOnPlane(value.point, averageArcPosition, normal);
				distanceToCenter.Add(Vector3.Distance(intersectionPoint, arcPoint));
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

		public void DebugPoints()
		{
			foreach (ArcPoint point in arcPoints)
			{
				Debug.DrawRay(point.point, normal * 0.01f, Color.yellow, 100f);
				Debug.DrawRay(PlacePointOnPlane(point.point, averageArcPosition, normal), normal * 0.01f, Color.red, 100f);
			}

			Debug.DrawRay(intersectionPoint, normal * 0.01f, Color.cyan, 100f);
			Debug.DrawRay(averageArcPosition, normal, Color.blue, 100f);
		}

		private Vector3 PlacePointOnPlane(Vector3 point, Vector3 center, Vector3 planeNormal)
		{
			Vector3 dir = point - center;
			dir = Vector3.ProjectOnPlane(dir, planeNormal);

			return center + dir;
		}

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

				// Debug.DrawRay(AverageVector(arcPositions), normal * 0.1f, Color.blue, 1f);
			}

			// Debug.DrawRay(AverageVector(arcPositions), AverageVector(normals.ToArray()), Color.white, 1f);
			return AverageVector(normals.ToArray());
		}

		private Vector3 CalculateIntersectionPoint(Vector3[] arcPositions)
		{
			var lines = new List<Ray>();
			for (int i = 0; i < arcPositions.Length - 1; i++)
			{
				int[] indexes = { i, i + 1 };

				Vector3 point1 = PlacePointOnPlane(arcPositions[indexes[0]], averageArcPosition, normal);
				Vector3 point2 = PlacePointOnPlane(arcPositions[indexes[1]], averageArcPosition, normal);

				Vector3 averagePoint = (point1 + point2) / 2f;
				Vector3 toCenterDirection = Vector3.Cross(point1 - point2, normal).normalized;

				if (lines.Count > 0)
				{
					if (Vector3.Dot(lines[lines.Count - 1].direction, toCenterDirection) < 0)
					{
						toCenterDirection *= -1;
					}
				}

				lines.Add(new Ray(averagePoint, toCenterDirection));
				// Debug.DrawRay(averagePoint, toCenterDirection * 0.3f, Color.yellow, 100f);
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
						// Debug.DrawRay(intersection, normal * 0.01f, Color.cyan, 1f);
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

		public struct ArcPoint
		{
			public Vector3 point;
			public TransformValues? parent;

			public ArcPoint(Vector3 point, TransformValues? parentTransformValues = null)
			{
				this.point = point;
				this.parent = parentTransformValues;
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
