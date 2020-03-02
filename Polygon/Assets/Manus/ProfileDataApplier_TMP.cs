using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;
using UnityEngine.UI;

namespace Manus.Polygon
{
	using Manus.Polygon.Skeleton;

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

		//public Slider legSlider;
		//public Slider spineSlider;
		//public Slider armSlider;

		private void Start()
		{
			polygon = GetComponent<PolygonSkeleton>();
			boneScalers = polygon.boneScalers;

			debug.SetActive(true);
			model.SetActive(false);
		}

		private void Update()
		{
			debug.SetActive(!sequence.isFinished);
			model.SetActive(sequence.isFinished);
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

			polygon.boneReferences.root.bone.localScale = new Vector3(characterSize, characterSize, characterSize);
			// polygon.newSkeleton.main.bone.localScale = new Vector3(characterSize, characterSize, characterSize);
			// boneScalers.ChangeSpineLength(spineMultiplier + spineSlider.value);
			// boneScalers.ChangeArmLength(bodyLength * 0.17f / characterSize * (armMultiplier + armSlider.value), bodyLength * 0.15f / characterSize * (armMultiplier + armSlider.value));
			// boneScalers.ChangeLegLength(bodyLength * 0.23f / characterSize * (legMultiplier + legSlider.value), bodyLength * 0.22f / characterSize * (legMultiplier + legSlider.value));
		}

		private void OnValidate()
		{
			if (Application.isPlaying && sequence.isFinished)
			{
				CalibrateBody();
			}
		}
	}
}

