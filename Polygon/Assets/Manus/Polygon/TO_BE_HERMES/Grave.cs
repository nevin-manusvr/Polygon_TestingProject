using HProt = Hermes.Protocol.Polygon;
using System.Collections.Generic;
using Hermes.Protocol.Polygon;
using UnityEngine;

namespace Manus.ToBeHermes
{
	[System.Serializable]
	public class Grave
	{
		// TMP
		public uint m_ID;
		
		public Possession possession;

		#region Properties

		public uint ID { get { return Skeleton.DeviceID; } }

		public Skeleton Skeleton { get; internal set; }


		#endregion

		public Grave(Skeleton _Skeleton)
		{
			m_ID = _Skeleton.DeviceID;
			Skeleton = _Skeleton;

			possession = new Possession(Skeleton, PossessionUtilities.DefaultRetargetTypes);
		}
	}
}
