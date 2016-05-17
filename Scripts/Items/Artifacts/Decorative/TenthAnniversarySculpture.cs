using System;

namespace Server.Items
{
	[Flipable( 0x3BB3, 0x3BB4 )]
	public class TenthAnniversarySculpture : Item
	{
        public override bool IsArtifact { get { return true; } }

        [Constructable]
		public TenthAnniversarySculpture() : base( 15283 )
		{
			Name = "10th Anniversary Sculpture";
			Weight = 1.0;
		}

		public TenthAnniversarySculpture(Serial serial) : base(serial)
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