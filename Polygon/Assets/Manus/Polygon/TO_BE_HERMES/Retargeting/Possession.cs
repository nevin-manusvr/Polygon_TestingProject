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
			private int m_Priority;

			public Bone bone;
			private Bone m_TargetBone;

			private Chain[] m_Chains;

			public Constraint(int _Priority, Bone _Bone, Bone _TargetBone)
			{
				m_Priority = _Priority;
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

				foreach (var t_Constraint in _AllConstraints)
				{
					if (t_Constraint.bone == _CurrentNode && _CurrentNode != bone)
					{
						// Add chain;
						Debug.Log("Found chain end");
						return;
					}
				}

				// Continue path

			}

			#endregion
		}

		public class Chain
		{
			public Constraint endConstraint;
			public float maxDistance;
			public bool directConnection;
		}

		#endregion
	}
}

