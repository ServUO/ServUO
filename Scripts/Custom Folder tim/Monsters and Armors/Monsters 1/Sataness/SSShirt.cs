
using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class SSShirt : Shirt
    {
        [Constructable]
        public SSShirt()
        {
            
            Hue = 2255;
            Name = "Sataness Sin Shirt";
	    Attributes.BonusStam = Utility.Random( 10, 20 );
            Attributes.AttackChance = Utility.Random( 10, 20 );
            Attributes.BonusDex = 20;
            Attributes.BonusInt = 20;
            Attributes.BonusHits = Utility.Random( 10, 20 );
            Attributes.BonusMana = Utility.Random( 10, 20 );
            Attributes.DefendChance = Utility.Random( 10, 20 );
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 20;
            Attributes.BonusStr = 20;

            LootType = LootType.Regular;
        }

        public SSShirt( Serial serial ) : base( serial )
        {
        }
             
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 );
        }
              
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}