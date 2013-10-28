#region Header
// **********
// ServUO - MultiTarget.cs
// **********
#endregion

#region References
using Server.Network;
#endregion

namespace Server.Targeting
{
	public abstract class MultiTarget : Target
	{
		public int MultiID { get; set; }

		public Point3D Offset { get; set; }

		protected MultiTarget(int multiID, Point3D offset)
			: this(multiID, offset, 10, true, TargetFlags.None)
		{ }

		protected MultiTarget(int multiID, Point3D offset, int range, bool allowGround, TargetFlags flags)
			: base(range, allowGround, flags)
		{
			MultiID = multiID;
			Offset = offset;
		}

		public override Packet GetPacketFor(NetState ns)
		{
			if (ns.HighSeas)
			{
				return new MultiTargetReqHS(this);
			}
			else
			{
				return new MultiTargetReq(this);
			}
		}
	}
}