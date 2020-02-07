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

		public float tmp = 1.6f;

		private void Start()
		{
			polygon = GetComponent<PolygonSkeleton>();
			boneScalers = polygon.boneScalers;
		}

		private void Update()
		{
			CalibrateBody();
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
			float armSpan = tmp; // profile.bodyMeasurements[BodyMeasurements.ArmSpan];

			float bodyLength = armSpan / 0.8f;

			float characterSize = bodyLength / 1.83f;

			polygon.newSkeleton.main.bone.localScale = new Vector3(characterSize, characterSize, characterSize);
			boneScalers.ChangeArmLength(bodyLength * 0.17f / characterSize, bodyLength * 0.15f / characterSize);
			boneScalers.ChangeLegLength(bodyLength * 0.23f / characterSize, bodyLength * 0.22f / characterSize);
		}
	}
}

