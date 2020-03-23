using System.Collections.Generic;

using Hermes.Protocol;
using Hermes.Protocol.Polygon;

using GlmSharp;
using Hermes.Tools;

namespace Manus.ToBeHermes
{
	public class GlmSkeleton
	{
		public uint id;
		public List<GlmBone> bones;
		public List<GlmControl> controls;
	
		public GlmSkeleton()
		{
			bones = new List<GlmBone>();
			controls = new List<GlmControl>();
		}

		#region Conversion

		public static implicit operator Skeleton(GlmSkeleton _Skeleton)
		{
			var t_Skeleton = new Skeleton { DeviceID = _Skeleton.id };

			foreach (var t_Bone in _Skeleton.bones)
			{
				t_Skeleton.Bones.Add(t_Bone);
			}

			foreach (var t_Control in _Skeleton.controls)
			{
				t_Skeleton.Controls.Add(t_Control);
			}

			return t_Skeleton;
		}

		public static implicit operator GlmSkeleton(Skeleton _Skeleton)
		{
			var t_Skeleton = new GlmSkeleton { id = _Skeleton.DeviceID };

			foreach (var t_Bone in _Skeleton.Bones)
			{
				t_Skeleton.bones.Add(t_Bone);
			}

			foreach (var t_Control in _Skeleton.Controls)
			{
				t_Skeleton.controls.Add(t_Control);
			}

			return t_Skeleton;
		}

		#endregion
	}

	public class GlmBone
	{
		public BoneType type;
		public vec3 position;
		public quat rotation;

		public GlmBone()
		{
			position = vec3.Zero;
			rotation = quat.Identity;
		}

		#region Conversion

		public static implicit operator Bone(GlmBone _Bone)
		{
			return new Bone()
			{
				Position = new Translation() { Full = _Bone.position.toProtoVec3() },
				Rotation = new Orientation() { Full = _Bone.rotation.toProtoQuat() },
				Type = _Bone.type
			};
		}

		public static implicit operator GlmBone(Bone _Bone)
		{
			return new GlmBone()
			{
				position = _Bone.Position.toGlmVec3(),
				rotation = _Bone.Rotation.toGlmQuat(),
				type = _Bone.Type
			};
		}

		#endregion
	}

	public class GlmControl
	{
		public ControlBoneType type;
		public vec3 position;
		public quat rotation;

		public List<GlmLocalBone> localBones;

		public GlmControl()
		{
			position = vec3.Zero;
			rotation = quat.Identity;
			localBones = new List<GlmLocalBone>();
		}

		#region Conversion

		public static implicit operator ControlBone(GlmControl _Control)
		{
			var t_Control = new ControlBone()
			{
				Type = _Control.type,
				Position = new Translation() { Full = _Control.position.toProtoVec3() },
				Rotation = new Orientation() { Full = _Control.rotation.toProtoQuat() }
			};

			foreach (var t_LocalBone in _Control.localBones)
			{
				t_Control.BoneLocalOffsets.Add(t_LocalBone);
			}

			return t_Control;
		}

		public static implicit operator GlmControl(ControlBone _Control)
		{
			var t_Control = new GlmControl()
			{
				type = _Control.Type,
				position = _Control.Position.toGlmVec3(),
				rotation = _Control.Rotation.toGlmQuat()
			};

			foreach (var t_LocalBone in _Control.BoneLocalOffsets)
			{
				t_Control.localBones.Add(t_LocalBone);
			}

			return t_Control;
		}

		#endregion
	}

	public class GlmLocalBone
	{
		public GlmBone bone;
		public vec3 localPosition;
		public quat localRotation;

		public GlmLocalBone()
		{
			localPosition = vec3.Zero;
			localRotation = quat.Identity;
		}

		#region Conversion

		public static implicit operator BoneLocalOffset(GlmLocalBone _LocalBone)
		{
			return new BoneLocalOffset()
			{
				Bone = _LocalBone.bone,
				LocalPosition = new Translation() { Full = _LocalBone.localPosition.toProtoVec3() },
				LocalRotation = new Orientation() { Full = _LocalBone.localRotation.toProtoQuat() }
			};
		}

		public static implicit operator GlmLocalBone(BoneLocalOffset _LocalBone)
		{
			return new GlmLocalBone()
			{
				bone = _LocalBone.Bone,
				localPosition = _LocalBone.LocalPosition.toGlmVec3(),
				localRotation = _LocalBone.LocalRotation.toGlmQuat()
			};
		}

		#endregion
	}
}
