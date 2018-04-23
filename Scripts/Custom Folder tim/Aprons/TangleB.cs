using System;

namespace Server.Items
{
    public class TangleB : HalfApron, ITokunoDyable
	{	
		[Constructable]
		public TangleB() : base()
			
		{
			Name = "Tangle";
			Hue = 0x487;
			
			Attributes.BonusInt = 10;
			Attributes.RegenMana = 2;
			Attributes.DefendChance = 5;			
		}

		public TangleB( Serial serial ) : base( serial )
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

