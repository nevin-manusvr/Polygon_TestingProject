using GlmSharp;

namespace GlmMathAddons
{
	public struct m4x4
	{
		public float m00;
		public float m10;
		public float m20;
		public float m30;

		public float m01;
		public float m11;
		public float m21;
		public float m31;

		public float m02;
		public float m12;
		public float m22;
		public float m32;

		public float m03;
		public float m13;
		public float m23;
		public float m33;

		public m4x4(vec4 column0, vec4 column1, vec4 column2, vec4 column3)
		{
			this.m00 = column0.x; this.m01 = column1.x; this.m02 = column2.x; this.m03 = column3.x;
			this.m10 = column0.y; this.m11 = column1.y; this.m12 = column2.y; this.m13 = column3.y;
			this.m20 = column0.z; this.m21 = column1.z; this.m22 = column2.z; this.m23 = column3.z;
			this.m30 = column0.w; this.m31 = column1.w; this.m32 = column2.w; this.m33 = column3.w;
		}

		public static m4x4 TRS(vec3 pos, quat q, vec3 s)
		{
			m4x4 result = new m4x4();
			// Rotation and Scale
			float sqw = q.w * q.w;
			float sqx = q.x * q.x;
			float sqy = q.y * q.y;
			float sqz = q.z * q.z;
			result.m00 = (1f - 2f * sqy - 2f * sqz) * s.x;
			result.m01 = (2f * q.x * q.y - 2f * q.z * q.w);
			result.m02 = (2f * q.x * q.z + 2f * q.y * q.w);
			result.m10 = (2f * q.x * q.y + 2f * q.z * q.w);
			result.m11 = (1f - 2 * sqx - 2 * sqz) * s.y;
			result.m12 = (2f * q.y * q.z - 2f * q.x * q.w);
			result.m20 = (2f * q.x * q.z - 2f * q.y * q.w);
			result.m21 = (2f * q.y * q.z + 2f * q.x * q.w);
			result.m22 = (1f - 2f * sqx - 2f * sqy) * s.z;
			// Translation
			result.m03 = pos.x;
			result.m13 = pos.y;
			result.m23 = pos.z;
			result.m33 = 1.0f;
			// Return result
			return result;
		}

		// Transforms a position by this matrix, without a perspective divide. (fast)
		public vec3 MultiplyPoint3x4(vec3 point)
		{
			vec3 res;
			res.x = this.m00 * point.x + this.m01 * point.y + this.m02 * point.z + this.m03;
			res.y = this.m10 * point.x + this.m11 * point.y + this.m12 * point.z + this.m13;
			res.z = this.m20 * point.x + this.m21 * point.y + this.m22 * point.z + this.m23;
			return res;
		}

		#region Values

		static readonly m4x4 zeroMatrix = new m4x4(new vec4(0, 0, 0, 0),
		new vec4(0, 0, 0, 0),
		new vec4(0, 0, 0, 0),
		new vec4(0, 0, 0, 0));

		// Returns a matrix with all elements set to zero (RO).
		public static m4x4 zero { get { return zeroMatrix; } }

		static readonly m4x4 identityMatrix = new m4x4(new vec4(1, 0, 0, 0),
			new vec4(0, 1, 0, 0),
			new vec4(0, 0, 1, 0),
			new vec4(0, 0, 0, 1));

		// Returns the identity matrix (RO).
		public static m4x4 identity { get { return identityMatrix; } }

		public static m4x4 Invert(m4x4 _Matrix)
		{
			m4x4 t_Result;
   
			float a = _Matrix.m00, b = _Matrix.m01, c = _Matrix.m02, d = _Matrix.m03;
			float e = _Matrix.m10, f = _Matrix.m11, g = _Matrix.m12, h = _Matrix.m13;
			float i = _Matrix.m20, j = _Matrix.m21, k = _Matrix.m22, l = _Matrix.m23;
			float m = _Matrix.m30, n = _Matrix.m31, o = _Matrix.m32, p = _Matrix.m33;

			float kp_lo = k * p - l * o;
			float jp_ln = j * p - l * n;
			float jo_kn = j * o - k * n;
			float ip_lm = i * p - l * m;
			float io_km = i * o - k * m;
			float in_jm = i * n - j * m;

			float a11 = +(f * kp_lo - g * jp_ln + h * jo_kn);
			float a12 = -(e * kp_lo - g * ip_lm + h * io_km);
			float a13 = +(e * jp_ln - f * ip_lm + h * in_jm);
			float a14 = -(e * jo_kn - f * io_km + g * in_jm);

			float det = a * a11 + b * a12 + c * a13 + d * a14;

			if (glm.Abs(det) < float.Epsilon)
			{
				return m4x4.identity;
			}

			float invDet = 1.0f / det;

			t_Result.m00 = a11 * invDet;
			t_Result.m10 = a12 * invDet;
			t_Result.m20 = a13 * invDet;
			t_Result.m30 = a14 * invDet;

			t_Result.m01 = -(b * kp_lo - c * jp_ln + d * jo_kn) * invDet;
			t_Result.m11 = +(a * kp_lo - c * ip_lm + d * io_km) * invDet;
			t_Result.m21 = -(a * jp_ln - b * ip_lm + d * in_jm) * invDet;
			t_Result.m31 = +(a * jo_kn - b * io_km + c * in_jm) * invDet;

			float gp_ho = g * p - h * o;
			float fp_hn = f * p - h * n;
			float fo_gn = f * o - g * n;
			float ep_hm = e * p - h * m;
			float eo_gm = e * o - g * m;
			float en_fm = e * n - f * m;

			t_Result.m02 = +(b * gp_ho - c * fp_hn + d * fo_gn) * invDet;
			t_Result.m12 = -(a * gp_ho - c * ep_hm + d * eo_gm) * invDet;
			t_Result.m22 = +(a * fp_hn - b * ep_hm + d * en_fm) * invDet;
			t_Result.m32 = -(a * fo_gn - b * eo_gm + c * en_fm) * invDet;

			float gl_hk = g * l - h * k;
			float fl_hj = f * l - h * j;
			float fk_gj = f * k - g * j;
			float el_hi = e * l - h * i;
			float ek_gi = e * k - g * i;
			float ej_fi = e * j - f * i;

			t_Result.m03 = -(b * gl_hk - c * fl_hj + d * fk_gj) * invDet;
			t_Result.m13 = +(a * gl_hk - c * el_hi + d * ek_gi) * invDet;
			t_Result.m23 = -(a * fl_hj - b * el_hi + d * ej_fi) * invDet;
			t_Result.m33 = +(a * fk_gj - b * ek_gi + c * ej_fi) * invDet;

			return t_Result;
		}
	}
		#endregion
}
