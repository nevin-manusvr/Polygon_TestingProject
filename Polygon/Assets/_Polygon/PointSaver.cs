using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class PointSaver : MonoBehaviour
{
	public struct Ray
	{
		public Vector3 start;
		public Vector3 direction;

		public Ray(Vector3 start, Vector3 direction)
		{
			this.start = start;
			this.direction = direction;
		}
	}

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

	private List<TransformValues> savedPoints;

	public float minPointDistance = 0.01f;
	public Vector3 normal;
	private Vector3 intersectionPoint;

	private void Start()
	{
		savedPoints = new List<TransformValues>();
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			if (savedPoints.Count == 0 || Vector3.Distance(savedPoints[savedPoints.Count - 1].position, transform.position) > minPointDistance)
			{
				savedPoints.Add(new TransformValues(transform));
			}
		}

		if (Input.GetKey(KeyCode.Backspace))
		{
			savedPoints.Clear();
		}

		if (Input.GetKeyDown(KeyCode.D))
		{
			CalculatePlane();
			CalculateIntersectionPoint();
			GetOffsetToTracker();
		}
	}

	private void CalculatePlane()
	{
		Vector3[] savedPositions = GetSavedPositions();

		var normals = new List<Vector3>();
		for (int i = 0; i < savedPoints.Count - 2; i++)
		{
			int[] indexes = { i, i + 1, i + 2 };
			Vector3 normal = Vector3.Cross(savedPoints[indexes[1]].position - savedPoints[indexes[0]].position, savedPoints[indexes[2]].position - savedPoints[indexes[0]].position).normalized;
			
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

			Debug.DrawRay(AverageVector(savedPositions), normal * 0.1f, Color.blue, 1f);
		}

		Debug.DrawRay(AverageVector(savedPositions), AverageVector(normals.ToArray()), Color.white, 1f);
		normal = AverageVector(normals.ToArray());
	}

	private void CalculateIntersectionPoint()
	{
		var directions = new List<Ray>();

		for (int i = 0; i < savedPoints.Count - 1; i++)
		{
			int[] indexes = { i, i + 1};
			Vector3 averagePoint = (Vector3.ProjectOnPlane(savedPoints[indexes[0]].position, normal) + Vector3.ProjectOnPlane(savedPoints[indexes[1]].position, normal)) / 2f;
			Vector3 toCenterDirection = Vector3.Cross(savedPoints[indexes[0]].position - savedPoints[indexes[1]].position, normal).normalized;

			if (directions.Count > 0)
			{
				if (Vector3.Dot(directions[directions.Count - 1].direction, toCenterDirection) < 0)
				{
					toCenterDirection *= -1;
				}
			}
			directions.Add(new Ray(averagePoint, toCenterDirection));
			Debug.DrawRay(averagePoint, toCenterDirection * 0.3f, Color.yellow, 1f);
		}

		var intersections = new List<Vector3>();
		for (int i = 0; i < directions.Count; i++)
		{
			Ray currentRay = directions[i];

			foreach (Ray ray in directions)
			{
				Vector3 intersection;
				if (LineLineIntersection(out intersection, currentRay.start, currentRay.direction, ray.start, ray.direction))
				{
					intersections.Add(intersection);
					Debug.DrawRay(intersection, normal * 0.01f, Color.cyan, 1f);
				}
			}
		}

		intersectionPoint = AverageVector(intersections.ToArray());
	}

	private void GetOffsetToTracker()
	{
		Transform trackerTransform = new GameObject("trackerPosition").transform;
		var offsetsToTracker = new List<Vector3>();

		foreach (TransformValues value in savedPoints)
		{
			trackerTransform.position = Vector3.ProjectOnPlane(value.position, normal);
			trackerTransform.rotation = value.rotation;

			Vector3 pointOffset = trackerTransform.InverseTransformPoint(intersectionPoint);
			offsetsToTracker.Add(pointOffset);
		}

		Vector3 offset = AverageVector(offsetsToTracker.ToArray());

		transform.GetChild(0).localPosition = offset;

		Destroy(trackerTransform.gameObject);
	}

	private Vector3 AverageVector(Vector3[] vectors)
	{
		Vector3 average = Vector3.zero;
		foreach (var vec in vectors)
		{
			average += vec;
		}
		average /= vectors.Length;

		return average;
	}

	private Vector3[] GetSavedPositions()
	{
		Vector3[] savedPositions = new Vector3[savedPoints.Count];
		for (var i = 0; i < savedPoints.Count; i++)
		{
			savedPositions[i] = savedPoints[i].position;
		}

		return savedPositions;
	}

	private void OnDrawGizmos()
	{
		if (savedPoints == null) return;
		foreach (TransformValues point in savedPoints)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(point.position, .001f);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(Vector3.ProjectOnPlane(point.position, normal), .001f);
		}

		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(intersectionPoint, .001f);
	}

	//Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
	//Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
	//same plane, use ClosestPointsOnTwoLines() instead.
	public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{

		Vector3 lineVec3 = linePoint2 - linePoint1;
		Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
		Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

		float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

		//is coplanar, and not parrallel
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
}
