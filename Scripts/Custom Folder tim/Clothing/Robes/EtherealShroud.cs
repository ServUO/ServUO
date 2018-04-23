using System;
using Server;

namespace Server.Items
{
    public class EtherealShroud : BaseOuterTorso
	{
        public override int AosStrReq { get { return 10; } }
        public override int OldStrReq { get { return 5; } }

		[Constructable]
        public EtherealShroud() : base(0x2684)
		{
			Hue = 21845;
            Name = "an Ethereal Shroud [Heavenly Artifact]";
			Attributes.LowerRegCost = 10;
		}

        public EtherealShroud(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (Weight == 4.0)
                Weight = 5.0;
		}
	}
}