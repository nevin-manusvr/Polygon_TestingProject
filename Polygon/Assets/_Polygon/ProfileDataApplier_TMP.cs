using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	using Manus.Core.Utility;

	public class ProfileDataApplier_TMP : MonoBehaviour
	{
		private PolygonSkeleton polygon;
		private SkeletonBoneScalers boneScalers;

		public CalibrationSequence sequence;
		public CalibrationProfile profile;

		public GameObject model;
		public GameObject debug;

		public float spineMultiplier = 1f;
		public float armMultiplier = 1f;
		public float legMultiplier = 1f;

		private void Start()
		{
			polygon = GetComponent<PolygonSkeleton>();
			boneScalers = polygon.boneScalers;

			model.SetActive(false);
		}

		private void Update()
		{
			//CalibrateBody();
		}

		private void OnEnable()
		{
			sequence.calibrationFinished += CalibrateBody;
			if (sequence.isFinished)
			{
				CalibrateBody();
			}
		}

		private void OnDisable()
		{
			sequence.calibrationFinished -= CalibrateBody;
		}

		private void CalibrateBody()
		{
			model.SetActive(true);
			debug.SetActive(false);

			float armSpan = profile.bodyMeasurements[BodyMeasurements.ArmSpan];
			float bodyLength = armSpan / 0.8f;

			float characterSize = bodyLength / 1.83f;

			polygon.newSkeleton.main.bone.localScale = new Vector3(characterSize, characterSize, characterSize);
			boneScalers.ChangeSpineLength(spineMultiplier);
			boneScalers.ChangeArmLength(bodyLength * 0.17f / characterSize * armMultiplier, bodyLength * 0.15f / characterSize * armMultiplier);
			boneScalers.ChangeLegLength(bodyLength * 0.23f / characterSize * legMultiplier, bodyLength * 0.22f / characterSize * legMultiplier);
		}

		private void OnValidate()
		{
			if (sequence.isFinished && Application.isPlaying)
			{
				CalibrateBody();
			}
		}
	}
}

