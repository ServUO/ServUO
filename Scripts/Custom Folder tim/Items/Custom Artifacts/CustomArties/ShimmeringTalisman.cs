using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class ShimmeringTalisman : BaseTalisman, ITokunoDyable
	{

		public override bool ForceShowName{ get{ return true; } }
	
		[Constructable]
		public ShimmeringTalisman() : base( 0x2F5B )
		{
			Name = "Shimmering Talisman";
			Hue = 1266;
			MaxChargeTime = 1800;
		
			Blessed = GetRandomBlessed();
			Protection = GetRandomProtection();
			
			Attributes.RegenMana = 2;
			Attributes.LowerRegCost = 10;
		}
		
		public ShimmeringTalisman( Serial serial ) :  base( serial )
		{
		}
		
		public override Type GetSummoner()
		{
			return Utility.RandomBool() ? typeof( SummonedSkeletalKnight ) : typeof( SummonedSheep );
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