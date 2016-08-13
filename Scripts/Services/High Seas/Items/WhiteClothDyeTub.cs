using Server;
using System;

namespace Server.Items
{
    public class WhiteClothDyeTub : DyeTub
	{
		[Constructable]
		public WhiteClothDyeTub()
		{
			Hue = DyedHue = 2498;
			Redyable = false;
		}

        public WhiteClothDyeTub(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
