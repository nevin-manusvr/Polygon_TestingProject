using HProt = Hermes.Protocol.Polygon;
using System.Collections.Generic;
using Hermes.Protocol.Polygon;

namespace Manus.ToBeHermes
{

	[System.Serializable]
	public class Grave
	{
		// TMP
		public int m_ID;
		
		public Possession possession;

		#region Properties

		public int ID { get { return (int)Skeleton.DeviceID; } }

		public Skeleton Skeleton { get; internal set; }


		#endregion

		public Grave(Skeleton _Skeleton)
		{
			m_ID = (int)_Skeleton.DeviceID;
			Skeleton = _Skeleton;

			possession = new Possession(Skeleton, PossessionUtilities.DefaultRetargetTypes);
		}
	}
}
