using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class RainbowTile : Item
    {
        [Constructable]
        public RainbowTile()
            : base(1310)
        {
            Movable = false;
        }

        public override bool OnMoveOver(Mobile from)
        {
            if (Utility.RandomBool())
                Hue = Utility.RandomDyedHue();
            else
                Hue = Utility.RandomNondyedHue();
            return true;
        }
           public RainbowTile( Serial serial ) : base( serial )
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