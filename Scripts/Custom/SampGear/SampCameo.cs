using System;

namespace Server.Items
{
	public class SampCameo : EnchantressCameo
	{
		[Constructable]
		public SampCameo(int whichSlayer)
		{
			Slayer = (TalismanSlayerName)whichSlayer;
		}

		public SampCameo(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}
}