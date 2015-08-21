using System;
using Server;

namespace Server.Items
{
	public class SingleUseSwitch : SimpleSwitch
	{

		[Constructable]
		public SingleUseSwitch()
		{
		}

		public SingleUseSwitch(Serial serial)
			: base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from == null || Disabled) return;

			if (!from.InRange(GetWorldLocation(), 2) || !from.InLOS(this))
			{
				from.SendLocalizedMessage(500446); // That is too far away.
				return;
			}

			base.OnDoubleClick(from);

			// delete after use
			Delete();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}
