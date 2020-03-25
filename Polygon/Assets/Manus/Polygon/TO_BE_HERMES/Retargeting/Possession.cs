using System;
using System.Collections;
using System.Collections.Generic;
using Hermes.Protocol.Polygon;
using System.Linq;

using UnityEngine;
using GlmSharp;
using GlmMathAddons;
using Manus.Core.Hermes;

namespace Manus.ToBeHermes 
{
	public class Possession
	{
		private GlmSkeleton m_Skeleton;

		private List<Constraint> m_AllConstraints;
		private Dictionary<int, List<Constraint>> m_Constraints;

		private List<TargetControl> m_TargetControls;

		public Skeleton Skeleton { get { return m_Skeleton; } }

		public Possession(Skeleton _Skeleton, BoneType[] _Filter)
		{
			m_Skeleton = new GlmSkeleton() { id = _Skeleton.DeviceID };

			foreach (var t_Bone in _Skeleton.Bones)
			{
				m_Skeleton.bones.Add(t_Bone);
			}

			foreach (var t_Control in _Skeleton.Controls)
			{
				m_Skeleton.controls.Add(t_Control);
			}
		}

		#region Setup

		public void SetTargetSkeleton(GlmSkeleton _TargetSkeleton)
		{
			GeneratePosessedSkeleton(_TargetSkeleton);
		}

		private void GeneratePosessedSkeleton(GlmSkeleton _TargetSkeleton)
		{
			// Reset current skeleton
			m_AllConstraints = new List<Constraint>();
			m_Constraints = new Dictionary<int, List<Constraint>>();
			m_TargetControls = new List<TargetControl>();

			// Copy target skeleton
			var t_TargetSkeleton = new GlmSkeleton() { id = _TargetSkeleton.id };
			foreach (var t_Bone in _TargetSkeleton.bones)
			{
				t_TargetSkeleton.bones.Add(t_Bone);
			}
			foreach (var t_Control in _TargetSkeleton.controls)
			{
				t_TargetSkeleton.controls.Add(t_Control);
			}


			// Regenerating the bones that are controlled by control bones and also exist in the main skeleton controls
			foreach (var t_TargetControl in t_TargetSkeleton.controls)
			{
				TargetControl t_Target = new TargetControl { control = t_TargetControl };
				m_TargetControls.Add(t_Target);

				// main control
				GlmControl t_MainControl = m_Skeleton.controls.Where(control => control.type == t_TargetControl.type).FirstOrDefault();
				if (t_MainControl == null)
					continue;

				foreach (var t_LocalBone in t_TargetControl.localBones)
				{
					var t_MainBone = t_MainControl.localBones.Where(localBone => localBone.bone.type == t_LocalBone.bone.type).FirstOrDefault();
					if (t_MainBone == null)
						continue;

					for (int i = 0; i < t_TargetSkeleton.bones.Count; i++)
					{
						if (t_LocalBone.bone.type == t_TargetSkeleton.bones[i].type)
						{
							var t_BoneSettings = t_TargetSkeleton.bones[i];
							
							var t_NewTargetBone = new GlmBone { type = t_BoneSettings.type, position = t_BoneSettings.position, rotation = t_BoneSettings.rotation };
							t_TargetSkeleton.bones[i] = t_NewTargetBone;
							t_Target.localOffsets.Add(new TargetLocalOffsets { bone = t_NewTargetBone, localPosition = t_MainBone.localPosition, localRotation = t_MainBone.localRotation });
						}
					}
				}
			}


			// Generate Constraints
			foreach (var t_Bone in m_Skeleton.bones)
			{
				foreach (var t_TargetBone in t_TargetSkeleton.bones)
				{
					if (t_Bone.type == t_TargetBone.type)
					{
						var t_Priority = PossessionUtilities.GetBonePriority(t_Bone.type);
						var t_Constraint = new Constraint(t_Priority, t_Bone, t_TargetBone);

						if (!m_Constraints.ContainsKey(t_Priority))
							m_Constraints.Add(t_Priority, new List<Constraint>());

						m_AllConstraints.Add(t_Constraint);
						m_Constraints[t_Priority].Add(t_Constraint);
					}
				}
			}

			// Order and generate max lengths
			m_Constraints = m_Constraints.OrderByDescending(t_Result => t_Result.Key).ToDictionary(t_Result => t_Result.Key, t_Result => t_Result.Value);

			var m_AllConstraintsArray = m_AllConstraints.ToArray();
			foreach (var t_Constraint in m_AllConstraints)
			{
				t_Constraint.AddChains(m_AllConstraintsArray);
			}
		}

		#endregion

		public void Move()
		{
			if (m_AllConstraints == null)
				return;
			//foreach (var control in Skeleton.Controls)
			//{
			//	if (control.Type == ControlBoneType.LeftHeelControl)
			//	{
			//		m4x4 t_HeelMatrix = m4x4.TRS(control.Position.toGlmVec3(), control.Rotation.toGlmQuat(), vec3.Ones);
			//		foreach (var t_OffsetBone in control.BoneLocalOffsets)
			//		{
			//			vec3 t_Pos = t_HeelMatrix.MultiplyPoint3x4(t_OffsetBone.LocalPosition.toGlmVec3());
			//			foreach (var t_Bone in Skeleton.Bones)
			//			{
			//				if (t_Bone.Type == t_OffsetBone.Bone.Type)
			//				{
			//					t_Bone.Position.Full = t_Pos.toProtoVec3();
			//				}
			//			}
			//		}
			//	}
			//}

			UpdateTargetControls();
			PositionBones();

			SolveBoneLengths();
			ApplyResults();
		}

		#region Solver

		private void UpdateTargetControls()
		{
			foreach (var t_Control in m_TargetControls)
			{
				t_Control.Update();
			}
		}

		private void PositionBones()
		{
			foreach (var t_Contraint in m_AllConstraints)
			{
				t_Contraint.PositionConstraint();
			}
		}

		private void SolveBoneLengths()
		{
			int t_Iterations = 25;
			float t_ClampedQuality = Mathf.Lerp(1, 0.001f, .5f);

			foreach (int priority in m_Constraints.Keys)
			{
				for (int i = 0; i < t_Iterations; i++)
				{
					foreach (var t_Contraint in m_Constraints[priority])
					{
						t_Contraint.SolveLengths(t_ClampedQuality);
					}
				}
			}
		}

		private void ApplyResults()
		{
			Constraint[] t_Constraints = m_AllConstraints.ToArray();

			for (int i = 0; i < t_Constraints.Length; i++)
			{
				t_Constraints[i].ApplyRetargetingPosition();
				t_Constraints[i].ApplyRetargetingRotation(t_Constraints);
			}
		}

		#endregion

		public void DrawGizmos()
		{
			Gizmos.color = Color.magenta;

			foreach (var t_Constraint in m_AllConstraints)
			{
				Gizmos.DrawWireCube(t_Constraint.targetBone.position.ToUnityVector3(), new Vector3(.05f, .05f, .05f));
			}
		}

		#region Modules

		public class Constraint
		{
			public int priority;

			public GlmBone bone;
			public GlmBone targetBone;

			private Chain[] m_Chains;
			// private ControlConstraint[] m_Controls;

			public vec3 targetPosition;
			public quat targetRotation;

			public Constraint(int _Priority, GlmBone _Bone, GlmBone _TargetBone)
			{
				priority = _Priority;
				bone = _Bone;
				targetBone = _TargetBone;
			}

			#region Setup

			public void AddChains(Constraint[] _AllConstraints)
			{
				var chains = new List<Chain>();
				TraverseConstraints(bone, new List<GlmBone>(), _AllConstraints, chains);

				m_Chains = chains.ToArray();
			}

			private void TraverseConstraints(GlmBone _CurrentNode, List<GlmBone> _Path, Constraint[] _AllConstraints, List<Chain> _ChainCollection)
			{
				_Path = new List<GlmBone>(_Path);
				_Path.Add(_CurrentNode);

				foreach (Constraint t_Constraint in _AllConstraints)
				{
					if (t_Constraint.bone == _CurrentNode && _CurrentNode != bone)
					{
						if (t_Constraint.priority >= priority)
						{
							_ChainCollection.Add(new Chain(t_Constraint, _Path, _AllConstraints));
							return;
						}
					}
				}

				// Continue path
				Constraint t_Parent = PossessionUtilities.GetParentForType(_AllConstraints, _CurrentNode.type);
				if (t_Parent != null && !_Path.Contains(t_Parent.bone))
				{
					// Debug.Log($"Parent: { _CurrentNode.Type} - {t_Parent.bone.Type}");
					TraverseConstraints(t_Parent.bone, _Path, _AllConstraints, _ChainCollection);
				}

				Constraint[] t_Children = PossessionUtilities.GetChildrenForType(_AllConstraints, _CurrentNode.type);
				if (t_Children != null)
				{
					foreach (var t_Child in t_Children)
					{
						if (!_Path.Contains(t_Child.bone))
						{
							// Debug.Log($"Child: { _CurrentNode.Type} - {t_Child.bone.Type}");
							TraverseConstraints(t_Child.bone, _Path, _AllConstraints, _ChainCollection);
						}
					}
				}
			}

			#endregion

			#region Solver
			
			public void PositionConstraint()
			{
				targetPosition = targetBone.position;
			}

			public void SolveLengths(float _Quality)
			{
				if (m_Chains == null)
					return;

				foreach (var t_Chain in m_Chains)
				{
					float distance = vec3.Distance(targetPosition, t_Chain.endConstraint.targetPosition);
					float difference = t_Chain.maxDistance - distance;

					if (difference < 0 || t_Chain.directConnection)
					{
						difference = t_Chain.maxDistance - distance;

						vec3 direction = (t_Chain.endConstraint.targetPosition - targetPosition).Normalized;
						targetPosition -= direction * difference * (t_Chain.endConstraint.priority > priority ? 1 : 0.5f) * _Quality;
					}
				}
			}

			public void ApplyRetargetingPosition()
			{
				bone.position = targetPosition;
			}

			public void ApplyRetargetingRotation(Constraint[] _Constraints)
			{
				bone.rotation = bone.CalculateBoneRotation(_Constraints);
			}

			#endregion
		}

		public class Chain
		{
			public Constraint endConstraint;
			public float maxDistance;
			public bool directConnection;

			public Chain(Constraint _EndConstraint, List<GlmBone> _Chain, Constraint[] _AllConstraints)
			{
				endConstraint = _EndConstraint;

				CalculateShortcuts(ref _Chain, _AllConstraints);
				maxDistance = CalculateChainLength(_Chain);
				directConnection = _Chain.Count < 3;
			}

			private void CalculateShortcuts(ref List<GlmBone> _Chain, Constraint[] _AllConstraints)
			{
				var t_BonesToRemove = new List<GlmBone>();
				bool t_LastWasParent = false;

				for (var i = 0; i < _Chain.Count; i++)
				{
					if (i == _Chain.Count - 1) continue;

					var t_Parent = PossessionUtilities.GetParentForType(_AllConstraints, _Chain[i].type);
					if (t_Parent != null && t_Parent.bone == _Chain[i + 1]) 
					{
						t_LastWasParent = true;
					}
					else
					{
						if (t_LastWasParent)
						{
							t_BonesToRemove.Add(_Chain[i]);
							// Debug.Log("Remove :  " + _Chain[i].Type + " - From : " + _Chain[0].Type + " - " + _Chain[_Chain.Count -1].Type);
						}

						t_LastWasParent = false;
					}
				}

				foreach (GlmBone t_BoneToRemove in t_BonesToRemove)
				{
					_Chain.RemoveAt(_Chain.IndexOf(t_BoneToRemove));
				}
			}

			private float CalculateChainLength(List<GlmBone> _Chain)
			{
				// Calculate max length
				float t_Length = 0;
				for (int i = 0; i < _Chain.Count - 1; i++)
				{
					t_Length += vec3.Distance(_Chain[i].position, _Chain[i + 1].position);
				}
				return t_Length;
			}
		}

		#endregion

		#region Control constraints

		public class ControlConstraint
		{


			public ControlConstraint()
			{

			}
		}

		#endregion

		#region Target bontrol bones

		public class TargetControl
		{
			public GlmControl control;
			public List<TargetLocalOffsets> localOffsets;

			public TargetControl()
			{
				localOffsets = new List<TargetLocalOffsets>();
			}

			public void Update()
			{
				var t_Parent = m4x4.TRS(control.position, control.rotation, vec3.Ones);

				foreach (var t_Offset in localOffsets)
				{
					t_Offset.bone.position = t_Parent.MultiplyPoint3x4(t_Offset.localPosition);
					t_Offset.bone.rotation = GlmMathExtensions.LookRotation(t_Parent.MultiplyVector(t_Offset.localRotation * vec3.UnitZ), t_Parent.MultiplyVector(t_Offset.localRotation * vec3.UnitY));
				}
			}
		}

		public class TargetLocalOffsets
		{
			public GlmBone bone;
			public vec3 localPosition;
			public quat localRotation;
		}

		#endregion
	}
}

