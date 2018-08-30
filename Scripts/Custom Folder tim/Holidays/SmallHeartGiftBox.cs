using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x49CA, 0x49CB )]
	public class SmallHeartGiftbox : BaseContainer
	{
		public override int DefaultGumpID { get { return 0x120; } }

		[Constructable]
		public SmallHeartGiftbox()
			: base(0x49CA)
		{
			
		}

		public SmallHeartGiftbox(Serial serial)
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