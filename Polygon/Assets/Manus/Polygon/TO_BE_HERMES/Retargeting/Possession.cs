using System;
using System.Collections;
using System.Collections.Generic;
using Hermes.Protocol.Polygon;
using System.Linq;

using UnityEngine;
using GlmSharp;
using GlmMathAddons;
using Hermes.Tools;

namespace Manus.ToBeHermes 
{
	public class Possession
	{
		private List<Constraint> m_AllConstraints;
		private Dictionary<int, List<Constraint>> m_Constraints;

		public Skeleton Skeleton { get; }

		public Possession(Skeleton _Skeleton, BoneType[] _Filter)
		{
			Skeleton = new Skeleton() { DeviceID = (uint)_Skeleton.DeviceID };

			foreach (var t_Bone in _Skeleton.Bones)
			{
				//if (!_Filter.Contains(t_Bone.Type))
				//	continue;

				Skeleton.Bones.Add(t_Bone.Clone());
			}

			foreach (var t_Control in _Skeleton.Controls)
			{
				Skeleton.Controls.Add(t_Control.Clone());
			}
		}

		#region Setup

		public void SetTargetSkeleton(Skeleton _TargetSkeleton)
		{
			GeneratePosessedSkeleton(_TargetSkeleton);
		}

		private void GeneratePosessedSkeleton(Skeleton _TargetSkeleton)
		{
			m_AllConstraints = new List<Constraint>();
			m_Constraints = new Dictionary<int, List<Constraint>>();

			foreach (var t_Bone in Skeleton.Bones)
			{
				foreach (var t_TargetBone in _TargetSkeleton.Bones)
				{
					if (t_Bone.Type == t_TargetBone.Type)
					{
						var t_Priority = PossessionUtilities.GetBonePriority(t_Bone.Type);
						var t_Constraint = new Constraint(t_Priority, t_Bone, t_TargetBone);

						if (!m_Constraints.ContainsKey(t_Priority))
							m_Constraints.Add(t_Priority, new List<Constraint>());

						m_AllConstraints.Add(t_Constraint);
						m_Constraints[t_Priority].Add(t_Constraint);
					}
				}
			}

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

			PositionBones();
			SolveBoneLengths();
			ApplyResults();
		}

		#region Solver

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
			foreach (var t_Contraint in m_AllConstraints)
			{
				t_Contraint.ApplyRetargetingPosition();
				t_Contraint.ApplyRetargetingRotation();
			}
		}

		#endregion

		#region Modules

		public class Constraint
		{
			public int priority;

			public Bone bone;
			private Bone m_TargetBone;

			private Chain[] m_Chains;
			private ControlConstraint[] m_Controls;

			public vec3 targetPosition;
			public quat targetRotation;

			public Constraint(int _Priority, Bone _Bone, Bone _TargetBone)
			{
				priority = _Priority;
				bone = _Bone;
				m_TargetBone = _TargetBone;
			}

			#region Setup

			public void AddChains(Constraint[] _AllConstraints)
			{
				var chains = new List<Chain>();
				TraverseConstraints(bone, new List<Bone>(), _AllConstraints, chains);

				m_Chains = chains.ToArray();
			}

			private void TraverseConstraints(Bone _CurrentNode, List<Bone> _Path, Constraint[] _AllConstraints, List<Chain> _ChainCollection)
			{
				_Path = new List<Bone>(_Path);
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
				Constraint t_Parent = PossessionUtilities.GetParentForType(_AllConstraints, _CurrentNode.Type);
				if (t_Parent != null && !_Path.Contains(t_Parent.bone))
				{
					// Debug.Log($"Parent: { _CurrentNode.Type} - {t_Parent.bone.Type}");
					TraverseConstraints(t_Parent.bone, _Path, _AllConstraints, _ChainCollection);
				}

				Constraint[] t_Children = PossessionUtilities.GetChildrenForType(_AllConstraints, _CurrentNode.Type);
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
				targetPosition = m_TargetBone.Position.toGlmVec3();
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
				bone.Position.Full = targetPosition.toProtoVec3();
			}

			public void ApplyRetargetingRotation()
			{
				bone.Rotation = m_TargetBone.Rotation;
			}

			#endregion
		}

		public class ControlConstraint
		{

		}

		public class Chain
		{
			public Constraint endConstraint;
			public float maxDistance;
			public bool directConnection;

			public Chain(Constraint _EndConstraint, List<Bone> _Chain, Constraint[] _AllConstraints)
			{
				endConstraint = _EndConstraint;

				CalculateShortcuts(ref _Chain, _AllConstraints);
				maxDistance = CalculateChainLength(_Chain);
				directConnection = _Chain.Count < 3;
			}

			private void CalculateShortcuts(ref List<Bone> _Chain, Constraint[] _AllConstraints)
			{
				var t_BonesToRemove = new List<Bone>();
				bool t_LastWasParent = false;

				for (var i = 0; i < _Chain.Count; i++)
				{
					if (i == _Chain.Count - 1) continue;

					var t_Parent = PossessionUtilities.GetParentForType(_AllConstraints, _Chain[i].Type);
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

				foreach (Bone t_BoneToRemove in t_BonesToRemove)
				{
					_Chain.RemoveAt(_Chain.IndexOf(t_BoneToRemove));
				}
			}

			private float CalculateChainLength(List<Bone> _Chain)
			{
				// Calculate max length
				float t_Length = 0;
				for (int i = 0; i < _Chain.Count - 1; i++)
				{
					t_Length += vec3.Distance(_Chain[i].Position.toGlmVec3(), _Chain[i + 1].Position.toGlmVec3());
				}
				return t_Length;
			}
		}

		#endregion
	}
}

