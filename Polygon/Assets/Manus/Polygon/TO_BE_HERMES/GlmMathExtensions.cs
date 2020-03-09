using GlmSharp;

namespace Manus.ToBeHermes
{
	public static class GlmMathExtensions
	{
		#region vec3

		public static vec3 Project(vec3 vector, vec3 onNormal)
		{
			float num1 = vec3.Dot(onNormal, onNormal);
			if ((double)num1 < double.Epsilon)
				return vec3.Zero;
			float num2 = vec3.Dot(vector, onNormal);
			return new vec3(onNormal.x * num2 / num1, onNormal.y * num2 / num1, onNormal.z * num2 / num1);
		}

		public static vec3 ProjectOnPlane(vec3 vector, vec3 planeNormal)
		{
			float num1 = vec3.Dot(planeNormal, planeNormal);
			if ((double)num1 < double.Epsilon)
				return vector;
			float num2 = vec3.Dot(vector, planeNormal);
			return new vec3(vector.x - planeNormal.x * num2 / num1, vector.y - planeNormal.y * num2 / num1, vector.z - planeNormal.z * num2 / num1);
		}

		#endregion

		#region quat

		public static quat LookRotation(vec3 direction, vec3 up)
		{
			vec3 t_Normal = vec3.Cross(up, direction).Normalized;
			mat3 mat = new mat3(t_Normal, vec3.Cross(direction, t_Normal), direction);
			return mat.ToQuaternion;
		}

		#endregion
	}
}
