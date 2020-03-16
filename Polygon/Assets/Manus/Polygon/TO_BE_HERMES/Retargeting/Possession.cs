using System;
using System.Collections;
using System.Collections.Generic;
using Hermes.Protocol.Polygon;
using System.Linq;

using UnityEngine;
using GlmSharp;
using Hermes.Tools;

namespace Manus.ToBeHermes 
{
	public class Possession
	{
		private readonly Skeleton m_Skeleton;

		private List<Constraint> m_AllConstraints;
		private Dictionary<int, List<Constraint>> m_Constraints;

		public Possession(Skeleton _Skeleton, BoneType[] _Filter)
		{
			m_Skeleton = new Skeleton() { DeviceID = (uint)_Skeleton.DeviceID };

			foreach (var t_Bone in _Skeleton.Bones)
			{
				if (!_Filter.Contains(t_Bone.Type))
					continue;

				m_Skeleton.Bones.Add(t_Bone.Clone());
			}
		}

		public void SetTargetSkeleton(Skeleton _TargetSkeleton)
		{
			GeneratePosessedSkeleton(_TargetSkeleton);
		}

		private void GeneratePosessedSkeleton(Skeleton _TargetSkeleton)
		{
			m_AllConstraints = new List<Constraint>();
			m_Constraints = new Dictionary<int, List<Constraint>>();

			foreach (var t_Bone in m_Skeleton.Bones)
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

		public void Move()
		{
			
		}

		#region Modules

		public class Constraint
		{
			public int priority;

			public Bone bone;
			private Bone m_TargetBone;

			private Chain[] m_Chains;

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

				foreach (var chain in chains)
				{
					Debug.Log(bone.Type + " - " + chain.endConstraint.bone.Type);
				}

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
							// Add chain;
							_ChainCollection.Add(new Chain(t_Constraint));
							Debug.Log("Found chain end");
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
		}

		public class Chain
		{
			public Constraint endConstraint;
			public float maxDistance;
			public bool directConnection;

			public Chain(Constraint _EndConstraint)
			{
				endConstraint = _EndConstraint;
			}
		}

		#endregion
	}
}

