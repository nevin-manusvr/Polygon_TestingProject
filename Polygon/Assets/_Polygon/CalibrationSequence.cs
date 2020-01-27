using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	using Manus.Core.VR;

	[CreateAssetMenu(fileName = "new Calibration Sequence", menuName = "ManusVR/Polygon/Calibration/Calibration Sequence", order = 0)]
	public class CalibrationSequence : ScriptableObject
	{
		public ProfileRequirements profileRequirements;
		public List<CalibrationStep> calibrationSteps;

		private CalibrationProfile profile;

		private void StartCalibrationSequence(CalibrationProfile profile, MonoBehaviour mono)
		{
			this.profile = profile;
			
		}

		private IEnumerator Calibration()
		{
			while (calibrationSteps[0])
			{
				
			}

			yield return new WaitForSeconds(0.5f);
		}
	}
}
