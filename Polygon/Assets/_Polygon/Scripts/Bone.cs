using UnityEngine;

namespace ManusVR.Polygon
{
	[System.Serializable]
	public class Bone
	{
		public Transform bone;

		public Vector3 initialPosition;
		public Quaternion initialRotation;

		public Bone(Transform bone)
		{
			if (bone == null) return;
			this.bone = bone;
		}

		public void SetInitialValues()
		{
			if (bone == null)
			{
				ResetValues();
			}

			initialPosition = bone.position;
			initialRotation = bone.rotation;
		}

		private void ResetValues()
		{
			initialPosition = Vector3.zero;
			initialRotation = Quaternion.identity;
		}
	}
}