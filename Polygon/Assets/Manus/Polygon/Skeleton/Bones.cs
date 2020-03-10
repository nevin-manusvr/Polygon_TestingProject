using UnityEngine;
using Hermes.Protocol.Polygon;
using Hermes.Protocol;
using Manus.Core.Hermes;

namespace Manus.Polygon.Skeleton
{
	using Manus.Polygon.Skeleton.Utilities;

	[System.Serializable]
	public class Bone
	{
		public bool optional = false;
		public bool retarget = false;

		public BoneType type;
		public Transform bone;

		public Quaternion desiredRotation;

		public Bone(bool _Optional, BoneType _Type)
		{
			optional = _Optional;
			type = _Type;
		}

		public void AssignTransform(Transform _Bone)
		{
			bone = _Bone;
		}

		public static implicit operator Hermes.Protocol.Polygon.Bone(Bone _Bone)
		{
			return new Hermes.Protocol.Polygon.Bone()  
			{ 
				Position = new Translation() { Full = _Bone.bone.position.ToProto() }, 
				Rotation = new Orientation() { Full = _Bone.bone.rotation.ToProto() },
				Type = _Bone.type
			};
		}
	}

	[System.Serializable]
	public class ControlBone
	{
		public ControlBoneType type;

		public Vector3 position;
		public Quaternion rotation;

		public BoneLocalOffset[] bonesToControl;

		public ControlBone(ControlBoneType _Type, Bone[] _BonesToControl)
		{
			type = _Type;

			if (bonesToControl == null)
			{
				bonesToControl = new BoneLocalOffset[0];
			}
			else
			{
				var t_Offsets = new BoneLocalOffset[_BonesToControl.Length];

				for (int i = 0; i < _BonesToControl.Length; i++)
				{
					t_Offsets[i] = new BoneLocalOffset(_BonesToControl[i], this);
				}

				bonesToControl = t_Offsets;
			}
		}

		#region Update

		public void UpdateTransformation(Vector3 _Position)
		{
			UpdateTransformation(_Position, rotation);
		}

		public void UpdateTransformation(Quaternion _Rotation)
		{
			UpdateTransformation(position, _Rotation);
		}

		public void UpdateTransformation(Vector3 _Position, Quaternion _Rotation)
		{
			foreach (var t_BoneLocalOffset in bonesToControl)
			{
				t_BoneLocalOffset.UpdateTransformation(this);
			}
		}

		#endregion

		public static implicit operator Hermes.Protocol.Polygon.ControlBone(ControlBone _Control)
		{
			var offsets = new Hermes.Protocol.Polygon.BoneLocalOffset[_Control.bonesToControl.Length];
			for (int i = 0; i < _Control.bonesToControl.Length; i++)
			{
				offsets[i] = _Control.bonesToControl[i];
			}

			return new Hermes.Protocol.Polygon.ControlBone
				       {
					       Type = _Control.type,
					       Position = new Translation() { Full = _Control.position.ToProto() },
					       Rotation = new Orientation() { Full = _Control.rotation.ToProto() },
						   BoneLocalOffsets = { offsets }
				       };
		}
	}

	[System.Serializable]
	public class BoneLocalOffset
	{
		public Bone bone;

		public Vector3 localPosition;
		public Quaternion localRotation;

		public BoneLocalOffset(Bone _Bone, ControlBone _Parent)
		{
			bone = _Bone;
			UpdateTransformation(_Parent);
		}

		public void UpdateTransformation(ControlBone _Parent)
		{
			if (bone.bone == null) 
				return;

			Matrix4x4 t_ParentMatrix = Matrix4x4.TRS(_Parent.position, _Parent.rotation.IsValid() ? _Parent.rotation : Quaternion.identity, Vector3.one).inverse;
			localPosition = t_ParentMatrix.MultiplyPoint3x4(bone.bone.position);
			Vector3 t_Forward = t_ParentMatrix.MultiplyVector(bone.bone.rotation * Vector3.forward);
			Vector3 t_Up = t_ParentMatrix.MultiplyVector(bone.bone.rotation * Vector3.up);
			localRotation = Quaternion.LookRotation(t_Forward, t_Up);
		}

		public static implicit operator Hermes.Protocol.Polygon.BoneLocalOffset(BoneLocalOffset _Offset)
		{
			return new Hermes.Protocol.Polygon.BoneLocalOffset()
				       {
					       Bone = _Offset.bone,
					       LocalPosition = new Translation() { Full = _Offset.localPosition.ToProto() },
					       LocalRotation = new Orientation() { Full = _Offset.localRotation.ToProto() }
				       };
		}
	}
}