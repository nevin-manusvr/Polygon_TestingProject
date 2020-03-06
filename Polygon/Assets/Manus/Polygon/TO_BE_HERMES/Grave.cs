using HProt = Hermes.Protocol;

namespace Manus.ToBeHermes
{

	[System.Serializable]
	public class Grave
	{
		// TMP
		public int m_ID;

		#region Properties

		public int ID { get { return (int)Skeleton.DeviceID; } }

		public HProt.Polygon.Skeleton Skeleton { get; internal set; }

		#endregion

		public Grave(HProt.Polygon.Skeleton _Skeleton)
		{
			m_ID = (int)_Skeleton.DeviceID;
			Skeleton = _Skeleton;
		}

		public void SetTarget(HProt.Polygon.Skeleton _Skeleton)
		{

		}
	}

	public class RetargetBone
	{
		public HProt.Polygon.Bone bone;
	}
}
