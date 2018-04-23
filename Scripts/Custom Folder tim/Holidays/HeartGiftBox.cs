using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x49CC, 0x4900 )]
	public class HeartGiftBox : BaseContainer
	{
		public override int DefaultGumpID { get { return 0x120; } }

		[Constructable]
		public HeartGiftBox()
			: base(0x49CC)
		{
			
		}

		public HeartGiftBox(Serial serial)
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