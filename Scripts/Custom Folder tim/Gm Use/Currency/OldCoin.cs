
using System;
using Server;

namespace Server.Items
{
	public class OldCoin : Item
	{
		public override double DefaultWeight
		{
			get { return 0.001; }
		}

		[Constructable]
		public OldCoin() : this( 1 )
		{
		}

		[Constructable]
		public OldCoin( int amount ) : base( 0x3196 )
		{
            Name = "Old Coin";
            Hue = 1151; 
			Stackable = true;
			Amount = amount;
		}

		public OldCoin( Serial serial ) : base( serial )
		{
		}

        public override int GetDropSound()
        {
            if (Amount <= 1)
                return 0x2E4;
            else if (Amount <= 5)
                return 0x2E5;
            else
                return 0x2E6;
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