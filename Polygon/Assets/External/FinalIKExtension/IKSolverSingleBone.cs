using UnityEngine;
using System.Collections;
using System;

namespace RootMotion.FinalIK
{

	/// <summary>
	/// Analytic %IK solver based on the Law of Cosines.
	/// </summary>
	[System.Serializable]
	public class IKSolverSingleBone : IKSolver
	{

		#region Main Interface

		/// <summary>
		/// The targetRaw Transform.
		/// </summary>
		public Transform target;
		/// <summary>
		/// The %IK rotation weight (rotation of the last bone).
		/// </summary>
		[Range(0f, 1f)]
		public float IKRotationWeight = 1f;
		/// <summary>
		/// The %IK rotation targetRaw.
		/// </summary>
		public Quaternion IKRotation = Quaternion.identity;

		/// <summary>
		/// The first bone (upper arm or thigh).
		/// </summary>
		public IKSolver.Point bone = new IKSolver.Point();

		/// <summary>
		/// Sets the %IK rotation.
		/// </summary>
		public void SetIKRotation(Quaternion rotation)
		{
			IKRotation = rotation;
		}

		/// <summary>
		/// Sets the %IK rotation weight.
		/// </summary>
		public void SetIKRotationWeight(float weight)
		{
			IKRotationWeight = Mathf.Clamp(weight, 0f, 1f);
		}

		/// <summary>
		/// Gets the %IK rotation.
		/// </summary>
		public Quaternion GetIKRotation()
		{
			return IKRotation;
		}

		/// <summary>
		/// Gets the %IK rotation weight.
		/// </summary>
		public float GetIKRotationWeight()
		{
			return IKRotationWeight;
		}

		public override IKSolver.Point[] GetPoints()
		{
			return new IKSolver.Point[1] { (IKSolver.Point)bone };
		}

		public override IKSolver.Point GetPoint(Transform transform)
		{
			if (bone.transform == transform) return (IKSolver.Point)bone;
			return null;
		}

		public override void StoreDefaultLocalState()
		{
			bone.StoreDefaultLocalState();
		}

		public override void FixTransforms()
		{
			if (!initiated) return;

			bone.FixTransform();
		}

		public override bool IsValid(ref string message)
		{
			if (bone.transform == null)
			{
				message = "Please assign all Bones to the IK solver.";
				return false;
			}

			return true;
		}


		/// <summary>
		/// Reinitiate the solver with new bone Transforms.
		/// </summary>
		/// <returns>
		/// Returns true if the new chain is valid.
		/// </returns>
		public bool SetChain(Transform bone, Transform root)
		{
			this.bone.transform = bone;

			Initiate(root);
			return initiated;
		}

		#endregion Main Interface

		#region Class Methods

		/// <summary>
		/// Solve the bone chain.
		/// </summary>
		public static void Solve(Transform bone1, Transform bone2, Transform bone3, Vector3 targetPosition, Vector3 bendNormal, float weight)
		{
			if (weight <= 0f) return;

			// Direction of the limb in solver
			targetPosition = Vector3.Lerp(bone3.position, targetPosition, weight);

			Vector3 dir = targetPosition - bone1.position;

			// Distance between the first and the last node solver positions
			float length = dir.magnitude;
			if (length == 0f) return;

			float sqrMag1 = (bone2.position - bone1.position).sqrMagnitude;
			float sqrMag2 = (bone3.position - bone2.position).sqrMagnitude;

			// Get the general world space bending direction
			Vector3 bendDir = Vector3.Cross(dir, bendNormal);

			// Get the direction to the trigonometrically solved position of the second node
			Vector3 toBendPoint = GetDirectionToBendPoint(dir, length, bendDir, sqrMag1, sqrMag2);

			// Position the second node
			Quaternion q1 = Quaternion.FromToRotation(bone2.position - bone1.position, toBendPoint);
			if (weight < 1f) q1 = Quaternion.Lerp(Quaternion.identity, q1, weight);

			bone1.rotation = q1 * bone1.rotation;

			Quaternion q2 = Quaternion.FromToRotation(bone3.position - bone2.position, targetPosition - bone2.position);
			if (weight < 1f) q2 = Quaternion.Lerp(Quaternion.identity, q2, weight);

			bone2.rotation = q2 * bone2.rotation;
		}

		//Calculates the bend direction based on the law of cosines. NB! Magnitude of the returned vector does not equal to the length of the first bone!
		private static Vector3 GetDirectionToBendPoint(Vector3 direction, float directionMag, Vector3 bendDirection, float sqrMag1, float sqrMag2)
		{
			float x = ((directionMag * directionMag) + (sqrMag1 - sqrMag2)) / 2f / directionMag;
			float y = (float)Math.Sqrt(Mathf.Clamp(sqrMag1 - x * x, 0, Mathf.Infinity));

			if (direction == Vector3.zero) return Vector3.zero;
			return Quaternion.LookRotation(direction, bendDirection) * new Vector3(0f, y, x);
		}

		#endregion Class Methods

		protected override void OnInitiate()
		{
			OnInitiateVirtual();

			IKPosition = bone.transform.position;
			IKRotation = bone.transform.rotation;
		}

		protected override void OnUpdate()
		{
			IKPositionWeight = Mathf.Clamp(IKPositionWeight, 0f, 1f);
			IKRotationWeight = Mathf.Clamp(IKRotationWeight, 0f, 1f);

			if (target != null)
			{
				IKPosition = target.position;
				IKRotation = target.rotation;
			}

			OnUpdateVirtual();

			if (IKPositionWeight > 0)
			{
				bone.transform.position = Vector3.Lerp(bone.transform.position, IKPosition, IKPositionWeight);
			}

			// Rotating bone3
			if (IKRotationWeight > 0)
			{
				bone.transform.rotation = Quaternion.Slerp(bone.transform.rotation, IKRotation, IKRotationWeight);
			}

			OnPostSolveVirtual();
		}

		protected Vector3 weightIKPosition;
		protected virtual void OnInitiateVirtual() { }
		protected virtual void OnUpdateVirtual() { }
		protected virtual void OnPostSolveVirtual() { }
	}
}

