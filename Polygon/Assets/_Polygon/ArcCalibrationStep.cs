using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.Utility;

namespace Manus.Polygon
{
	[CreateAssetMenu(fileName = "new Arc Calibration Step", menuName = "ManusVR/Polygon/Calibration/Arc Calibration Step", order = 10)]
	public class ArcCalibrationStep : CalibrationStep
	{
		public ArcSettings[] settings;
		private Arc[] arcArray;

		public override void Setup(CalibrationProfile profile, TrackerReference trackers)
		{
			base.Setup(profile, trackers);

			arcArray = new Arc[settings.Length];

			for (var i = 0; i < settings.Length; i++)
			{
				arcArray[i] = new Arc();//trackers.GetTracker(settings[i].parentTracker)); // TODO: fix this to work with local positions
			}
		}

		public override IEnumerator Start()
		{
			float timer = 0;

			while (timer < time)
			{
				timer += Time.deltaTime;
				Update();
				yield return new WaitForEndOfFrame();
			}

			End();
		}

		protected override void Update()
		{
			for (int i = 0; i < settings.Length; i++)
			{
				arcArray[i].AddMeasurement(trackers.GetTracker(settings[i].tracker).position);
			}
		}

		protected override void End()
		{
			for (int i = 0; i < settings.Length; i++)
			{
				arcArray[i].CalculateArc();
				GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				sphere.transform.position = arcArray[i].IntersectionPoint;
				sphere.transform.localScale = new Vector3(.05f, .05f, .05f);
			}
		}

		public override void Revert()
		{
			// TODO: implement this
		}
		
		[System.Serializable]
		public class ArcSettings
		{
			public VRTrackerType tracker = VRTrackerType.Waist;
			public bool useTrackerWithOffsets = false;

			public bool getPositionOffset = false;
			public VRTrackerType parentTracker;
			public OffsetsToTrackers offsetToTracker;

			public BodyMeasurements bodyMeasurement;
		}
	}
}

