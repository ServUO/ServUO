using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x49C8, 0x49C9 )]
	public class TallGiftBox : BaseContainer
	{
		public override int DefaultGumpID { get { return 0x121; } }

		[Constructable]
		public TallGiftBox()
			: base(0x49C8)
		{
			
		}

		public TallGiftBox(Serial serial)
			: base(serial)
		{
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