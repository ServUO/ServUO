using System;

namespace Server.Items
{
	public class DecoSoulStone : Item
	{

		[Constructable]
		public DecoSoulStone() : base(10899)
		{
			Name = "Soulstone";
		}

		public DecoSoulStone(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}