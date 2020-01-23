using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	public class SkeletonBoneScalersInterface : MonoBehaviour
	{
		private SkeletonBoneScalers boneScalers;

		[Header("Body")]
		[SerializeField, Range(.0001f, 10f)] private float thickness = 1f;
		[SerializeField, Range(.0001f, 10f)] private float spineLength = 1f;
		[SerializeField, Range(.0001f, 10f)] private float headSize = 1f;

		[Header("Arms")]
		[SerializeField, Range(.0001f, 10f)] private float upperArmLength = 1f;
		[SerializeField, Range(.0001f, 10f)] private float lowerArmLength = 1f;

		[Header("Hands")]
		[SerializeField, Range(.0001f, 10f)] private float handSize = 1f;

		[SerializeField, Range(.0001f, 10f)] private float fingerLength = 1f;
		[SerializeField, Range(.0001f, 10f)] private float fingerThickness = 1f;

		[Header("Legs")]
		[SerializeField, Range(.0001f, 10f)] private float upperLegLength = 1f;
		[SerializeField, Range(.0001f, 10f)] private float lowerLegLength = 1f;
		[SerializeField, Range(.0001f, 10f)] private float footSize = 1f;

		private void Start()
		{
			boneScalers = GetComponent<PolygonSkeleton>().boneScalers;
		}

		private void Update()
		{
			boneScalers.ChangeThickness(thickness);
			boneScalers.ChangeSpineLength(spineLength);
			boneScalers.ChangeHeadSize(headSize);


			boneScalers.ChangeArmLength(upperArmLength, lowerArmLength);
			boneScalers.ChangeLegLength(upperLegLength, lowerLegLength);

			boneScalers.ChangeHandSize(handSize);
			boneScalers.ChangeFootSize(footSize);

			boneScalers.ChangeFingerLength(fingerLength);
			boneScalers.ChangeFingerThickness(fingerThickness);
		}

		[ContextMenu("Randomize all values")]
		private void RandomizeValues()
		{
			float minValue = 0.0001f;
			float maxValue = 10f;

			thickness = Random.Range(minValue, maxValue);
			spineLength = Random.Range(minValue, maxValue);
			headSize = Random.Range(minValue, maxValue);
			upperArmLength = Random.Range(minValue, maxValue);
			lowerArmLength = Random.Range(minValue, maxValue);
			handSize = Random.Range(minValue, maxValue);
			fingerLength = Random.Range(minValue, maxValue);
			fingerThickness = Random.Range(minValue, maxValue);
			upperLegLength = Random.Range(minValue, maxValue);
			lowerLegLength = Random.Range(minValue, maxValue);
			footSize = Random.Range(minValue, maxValue);
		}
	}
}

