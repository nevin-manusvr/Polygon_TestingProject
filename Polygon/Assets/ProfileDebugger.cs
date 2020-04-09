using System.Collections;
using System.Collections.Generic;
using Manus.Polygon;
using UnityEngine;
using Manus.Core.Utility;
using System.Linq;

namespace Manus.Polygon
{
	public class ProfileDebugger : MonoBehaviour
	{
		public bool alsoHead = false;
		public CalibrationProfile profile;
		private TrackerReference trackers;

		private List<GameObject> trackerVisuals;
		private List<GameObject> trackerOffsetVisuals;
		private List<GameObject[]> trackerDirectionVisuals;

		private GameObject hip;
		private GameObject leftHand, rightHand;
		private GameObject leftFoot, rightFoot;

		[Header("Models")]
		public GameObject trackerModel;
		public GameObject leftHandModel, rightHandModel;
		public GameObject leftFootModel, rightFootModel;
		public GameObject hipModel;

		private void Start()
		{
			// Find references
			trackers = FindObjectOfType<TrackerReference>();

			// Initializing
			trackerVisuals = new List<GameObject>();
			trackerOffsetVisuals = new List<GameObject>();
			trackerDirectionVisuals = new List<GameObject[]>();
		}

		private void Update()
		{
			VisualizeTrackers();
			VisualizeTrackerOffsets();
			VisualizeDirection();

			VisualizeBodyPart(ref leftHand, leftHandModel, VRTrackerType.LeftHand, OffsetsToTrackers.LeftHandTrackerToWrist);
			VisualizeBodyPart(ref rightHand, rightHandModel, VRTrackerType.RightHand, OffsetsToTrackers.RightHandTrackerToWrist);

			VisualizeBodyPart(ref leftFoot, leftFootModel, VRTrackerType.LeftFoot, OffsetsToTrackers.LeftFootTrackerToAnkle);
			VisualizeBodyPart(ref rightFoot, rightFootModel, VRTrackerType.RightFoot, OffsetsToTrackers.RightFootTrackerToAnkle);
			
			VisualizeBodyPart(ref hip, hipModel, VRTrackerType.Waist, OffsetsToTrackers.HipTrackerToHip);

		}

		private void VisualizeTrackers()
		{
			for (int i = 0; i < trackers.RequiredTrackers.Length; i++)
			{
				// Spawn new visual if there are not enough
				if (trackerVisuals.Count < i + 1)
				{
					trackerVisuals.Add(Instantiate(trackerModel, transform));
				}

				VRTrackerType trackerType = trackers.RequiredTrackers[i];

				// Hide tracker when there is no tracker data
				if (!trackers.GetTracker(trackerType, out TransformValues trackerTransform))
				{
					if (trackerVisuals[i].activeSelf) trackerVisuals[i].SetActive(false);
					continue;
				}

				// Position tracker model to the right position
				if (!trackerVisuals[i].activeSelf) trackerVisuals[i].SetActive(true);

				trackerVisuals[i]?.transform.SetPositionAndRotation(
					trackerTransform.position,
					trackerTransform.rotation);

				//gets child object with collider and gives them a corresponding tag
				GameObject Child = trackerVisuals[i].transform.GetChild(0).gameObject;
				int l = 9 + i;
				Child.layer = l;
			}
		}

		private void VisualizeTrackerOffsets()
		{
			foreach (GameObject offsetVisual in trackerOffsetVisuals)
			{
				offsetVisual.SetActive(false);
			}

			for (int i = 0; i < profile.trackerOffsets.Keys.Count; i++)
			{
				if (trackerOffsetVisuals.Count < i + 1)
				{
					trackerOffsetVisuals.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere));
					trackerOffsetVisuals[i].GetComponent<MeshRenderer>().material.color = Color.black;
					Destroy(trackerOffsetVisuals[i].GetComponent<SphereCollider>());
					trackerOffsetVisuals[i].transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
					trackerOffsetVisuals[i].transform.SetParent(transform);
				}

				OffsetsToTrackers offsetType = profile.trackerOffsets.Keys.ToArray()[i];
				VRTrackerType type = CalibrationProfile.GetMatchingTrackerFromOffset(offsetType) ?? VRTrackerType.Other;

				TrackerOffset offset = profile.trackerOffsets[offsetType];

				if (!trackers.GetTrackerWithOffset(type, offset.Position, Quaternion.identity, out TransformValues trackerWithOffset))
				{
					if (trackerOffsetVisuals[i].activeSelf) trackerOffsetVisuals[i].SetActive(false);
					continue;
				}

				if (!trackerOffsetVisuals[i].activeSelf) trackerOffsetVisuals[i].SetActive(true);
				trackerOffsetVisuals[i]?.transform.SetPositionAndRotation(trackerWithOffset.position, trackerWithOffset.rotation);
			}
		}

		private void VisualizeDirection()
		{
			foreach (GameObject[] directionVisual in trackerDirectionVisuals)
			{
				foreach (GameObject dir in directionVisual)
				{
					dir.SetActive(false);
				}
			}

			for (int i = 0; i < profile.trackerDirections.Keys.Count; i++)
			{
				if (trackerDirectionVisuals.Count < i + 1)
				{
					trackerDirectionVisuals.Add(
						new[]
							{
								GameObject.CreatePrimitive(PrimitiveType.Cube),
								GameObject.CreatePrimitive(PrimitiveType.Cube),
								GameObject.CreatePrimitive(PrimitiveType.Cube)
							});

					trackerDirectionVisuals[i][0].GetComponent<MeshRenderer>().material.color = Color.red;
					trackerDirectionVisuals[i][1].GetComponent<MeshRenderer>().material.color = Color.green;
					trackerDirectionVisuals[i][2].GetComponent<MeshRenderer>().material.color = Color.blue;
					Destroy(trackerDirectionVisuals[i][0].GetComponent<BoxCollider>());
					Destroy(trackerDirectionVisuals[i][1].GetComponent<BoxCollider>());
					Destroy(trackerDirectionVisuals[i][2].GetComponent<BoxCollider>());
					
					foreach (GameObject obj in trackerDirectionVisuals[i])
					{
						obj.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
						obj.transform.SetParent(transform);
					}
				}

				VRTrackerType trackerType = profile.trackerDirections.Keys.ToArray()[i];
				if (alsoHead == false && trackerType == VRTrackerType.Head) continue; 
				OffsetsToTrackers? offsetType = CalibrationProfile.GetMatchingTrackerOffsetForTracker(trackerType);

				if (offsetType == null) continue;

				Matrix4x4 trackerMatrix;

				if (profile.trackerOffsets.ContainsKey((OffsetsToTrackers)offsetType))
				{
					TrackerOffset trackerOffset = profile.trackerOffsets[(OffsetsToTrackers)offsetType];
					trackers.GetTrackerWithOffset(trackerType, trackerOffset.Position, Quaternion.identity, out TransformValues trackerTransform);
					trackerMatrix = Matrix4x4.TRS(trackerTransform.position, trackerTransform.rotation, Vector3.one);
				}
				else
				{
					trackers.GetTracker(trackerType, out TransformValues trackerTransform);
					trackerMatrix = Matrix4x4.TRS(trackerTransform.position, trackerTransform.rotation, Vector3.one);
				}

				TrackerDirection trackerDirection = profile.trackerDirections[trackerType];

				if (trackerDirection.X != Vector3.zero)
				{
					trackerDirectionVisuals[i][0].SetActive(true);
					trackerDirectionVisuals[i][0].transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
					trackerDirectionVisuals[i][0].transform.position = trackerMatrix.GetPosition() + trackerMatrix.MultiplyVector(trackerDirection.X) * 0.1f;
				} 
				else trackerDirectionVisuals[i][0].transform.localScale = Vector3.zero;

				if (trackerDirection.Y != Vector3.zero)
				{
					trackerDirectionVisuals[i][1].SetActive(true);
					trackerDirectionVisuals[i][1].transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
					trackerDirectionVisuals[i][1].transform.position = trackerMatrix.GetPosition() + trackerMatrix.MultiplyVector(trackerDirection.Y) * 0.1f;
				} 
				else trackerDirectionVisuals[i][1].transform.localScale = Vector3.zero;

				if (trackerDirection.Z != Vector3.zero)
				{
					trackerDirectionVisuals[i][2].SetActive(true);
					trackerDirectionVisuals[i][2].transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
					trackerDirectionVisuals[i][2].transform.position = trackerMatrix.GetPosition() + trackerMatrix.MultiplyVector(trackerDirection.Z) * 0.1f;
				} 
				else trackerDirectionVisuals[i][2].transform.localScale = Vector3.zero;
			}
		}

		private void VisualizeBodyPart(ref GameObject obj, GameObject objModel, VRTrackerType trackerType, OffsetsToTrackers offset)
		{
			if (obj == null)
			{
				obj = Instantiate(objModel, transform);
				obj.SetActive(false);
			}

			trackers.GetTracker(trackerType, out TransformValues handTransform);
			if (profile.trackerOffsets.ContainsKey(offset) && profile.trackerOffsets[offset].position != null)
			{
				trackers.GetTrackerWithOffset(trackerType, profile.trackerOffsets[offset].Position, Quaternion.identity, out handTransform);
			}

			if (!profile.trackerDirections.ContainsKey(trackerType)
                || profile.trackerDirections[trackerType].GetAxis(Axis.Z) == null
                || profile.trackerDirections[trackerType].GetAxis(Axis.Y) == null)
			{
				if (obj.activeSelf) obj.SetActive(false);
				return;
			}

			if (!obj.activeSelf) obj.SetActive(true);
			Matrix4x4 trackerMatrix = Matrix4x4.TRS(handTransform.position, handTransform.rotation, Vector3.one);

			Quaternion rotation = Quaternion.LookRotation(
				trackerMatrix.MultiplyVector(profile.trackerDirections[trackerType].Z),
				trackerMatrix.MultiplyVector(profile.trackerDirections[trackerType].Y));

			obj.transform.SetPositionAndRotation(handTransform.position, rotation);
		}
	}
}