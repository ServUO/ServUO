//created by Darck... *this is supposed to be a custom currency for use with certian vendor stone setups. simple yet perfect id say*
using System;
using Server;

namespace Server.Items
{
	public class MasterCoin : Item
	{
		public override double DefaultWeight
		{
			get { return 0.001; }
		}

		[Constructable]
		public MasterCoin() : this( 1 )
		{
		}

		[Constructable]
		public MasterCoin( int amount ) : base( 0x3196 )
		{
            Name = "Master Coin";
            Hue = 1177; 
			Stackable = true;
			Amount = amount;
		}

		public MasterCoin( Serial serial ) : base( serial )
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