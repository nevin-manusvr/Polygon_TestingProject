using UnityEngine;

namespace Manus.Polygon
{
	public class PolygonMeshes : ScriptableObject
	{
		[System.Serializable]
		public struct MeshInfo
		{
			public Mesh m_Mesh;
			public string m_Hierarchy;
		}

		public string m_ID;
		public MeshInfo[] m_Meshes;
	}
}

