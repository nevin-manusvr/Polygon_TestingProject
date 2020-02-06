using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RootMotion.FinalIK;
using DG.Tweening;

namespace Manus.Polygon
{
	[System.Serializable]
	public class PolygonIK_TMP : IDisposable
	{
		#region fields

		private bool isInitialized = false;
		private bool ikGenerated = false;

		private GameObject ikContainer;

		private Animator animator;
		private SkeletonBoneReferences bones;

		// Settings
		[Header("Spine")]
		public float hipInfluence = 1f;
		public float headInfluence = 1f;

		[Header("Knee")]
		public Vector2 kneeMinMax = new Vector2(-40, 50);
		public Vector3 footInfluenceOnKneeRotation = new Vector3(.1f, 1f, 1f);
		public float totalFootInfluenceOnKneeRotation = 1f;

		[Header("Elbow")]
		public Vector2 elbowMinMax = new Vector2(-30, 80);
		public Vector3 handInfluenceOnElbowRotation = new Vector3(1f, .3f, -1f);
		public float totalHandInfluenceOnElbowRotation = 2f;

		// IK values
		[HideInInspector] public SingleBoneIK hipIK;
		[HideInInspector] public LimbIK[] spineIK;
		[HideInInspector] public LimbIK neckIK;
		[HideInInspector] public LimbIK leftArmIK;
		[HideInInspector] public LimbIK rightArmIK;
		[HideInInspector] public LimbIK leftLegIK;
		[HideInInspector] public LimbIK rightLegIK;

		private float[] spineIKWeight;
		private Tweener ikWeightTween;

		#endregion

		public void InitializeIK(Transform root, Animator animator, SkeletonBoneReferences bones)
		{
			if (isInitialized) return;
			isInitialized = true;

			ikContainer = new GameObject("IK");
			ikContainer.transform.SetParent(root);
			ikContainer.transform.localPosition = Vector3.zero;
			ikContainer.transform.localRotation = Quaternion.identity;

			this.animator = animator;
			this.bones = bones;
		}

		public void CreateCharacterIK()
		{
			if (ikGenerated) return;
			ikGenerated = true;

			IKExecutionOrder order = ikContainer.AddComponent<IKExecutionOrder>();
			order.animator = animator;

			List<IK> ikComponents = new List<IK>();

			ikComponents.Add(hipIK = ikContainer.AddComponent<SingleBoneIK>());
			hipIK.solver.SetChain(bones.main.bone, bones.main.bone);
			// hipIK.solver.target = targets.targetReal[IKTargetPosition.Hip];
		}

		#region Private Methods

		private LimbIK CreateIKChain(Transform bone1, Transform bone2, Transform bone3, Transform root, GameObject container, Transform target, Transform bendTarget)
		{
			LimbIK ik = container.AddComponent<LimbIK>();
			ik.solver.SetChain(bone1, bone2, bone3, root);
			ik.solver.bendModifier = IKSolverLimb.BendModifier.Goal;

			ik.solver.target = target;
			ik.solver.bendGoal = bendTarget;

			return ik;
		}

		#endregion

		public void Dispose()
		{
			MonoBehaviour.Destroy(ikContainer);

			if (ikGenerated)
			{
			}
		}
	}
}


