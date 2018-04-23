
using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class SoulofArachnis : BoneArms
    {
        [Constructable]
        public SoulofArachnis() : base( 4246 )
        {
            
            Hue = 248;
            Name = "Soul of Arachnis";
	    Layer = Layer.Talisman;
            Attributes.AttackChance = 15;
            Attributes.BonusStam = 25;
            Attributes.BonusHits = 25;
            Attributes.CastRecovery = 2;
            Attributes.CastSpeed = 2;
            Attributes.DefendChance = 15;
            Attributes.LowerManaCost = 20;
            Attributes.LowerRegCost = 20;
            Attributes.BonusMana = 25;

            LootType = LootType.Regular;
        }

        public SoulofArachnis( Serial serial ) : base( serial )
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