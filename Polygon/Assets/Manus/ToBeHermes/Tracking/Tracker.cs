using UnityEngine;
using GlmSharp;
using Manus.Core.Utility;

namespace Manus.ToBeHermes.Tracking
{
	public class Tracker
	{
		public VRTrackerType type;
		public vec3 position;
		public quat rotation;

		#region Constructor

		public Tracker(VRTrackerType type, vec3 position, quat rotation)
		{
			this.position = position;
			this.rotation = rotation;
		}

		public Tracker(vec3 position)
		{
			this.position = position;
		}

		public Tracker(quat rotation)
		{
			this.rotation = rotation;
		}

		public Tracker()
		{
			this.position = vec3.Zero;
			this.rotation = quat.Identity;
		}

		#endregion

		#region Update Tracker

		public void UpdateTracker(vec3 position, quat rotation)
		{
			UpdateTracker(position);
			UpdateTracker(rotation);
		}

		public void UpdateTracker(vec3 position)
		{
			this.position = position;
		}

		public void UpdateTracker(quat rotation)
		{
			this.rotation = rotation;
		}

		#endregion

	}
}