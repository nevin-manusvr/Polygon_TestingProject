using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Core.VR;

namespace Manus.Polygon
{
	public class Calibrator : MonoBehaviour
	{
		public CalibrationProfile profile;
		public CalibrationSequence sequence;

		private TrackerManager trackerManager;

		private void Start()
		{
			trackerManager = TrackerManager.instance;

			foreach (TrackedObject trackedObject in trackerManager.m_TrackedObjects)
			{
				Debug.Log(trackedObject.type);
			}
		}

		public void StartCalibration()
		{

		}
	}
}