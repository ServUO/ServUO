#region References
using Server.Network;
#endregion

namespace Server.Targeting
{
	public abstract class MultiTarget : Target
	{
		public int MultiID { get; set; }
		public int Hue { get; set; }
		public Point3D Offset { get; set; }

		protected MultiTarget(int multiID, Point3D offset)
			: this(multiID, 0, offset)
		{ }

		protected MultiTarget(int multiID, Point3D offset, int range, bool allowGround, TargetFlags flags)
			: this(multiID, 0, offset, range, allowGround, flags)
		{ }

		protected MultiTarget(int multiID, int hue, Point3D offset)
			: this(multiID, hue, offset, 10, true, TargetFlags.None)
		{ }

		protected MultiTarget(int multiID, int hue, Point3D offset, int range, bool allowGround, TargetFlags flags)
			: base(range, allowGround, flags)
		{
			MultiID = multiID;
			Hue = hue;
			Offset = offset;
		}

		public override Packet GetPacketFor(NetState ns)
		{
			return MultiTargetReq.Instantiate(ns, this);
		}
	}
}
