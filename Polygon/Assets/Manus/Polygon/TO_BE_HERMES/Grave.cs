using HProt = Hermes.Protocol;

namespace Manus.ToBeHermes
{
	[System.Serializable]
	public class Grave
	{
		public int m_ID;

		public int ID
		{
			get { return (int)Skeleton.DeviceID; }
		}

		public HProt.Polygon.Skeleton Skeleton { get; internal set; }

		public Grave(HProt.Polygon.Skeleton _Skeleton)
		{
			m_ID = (int)_Skeleton.DeviceID;
			Skeleton = _Skeleton;
		}
	}
}
