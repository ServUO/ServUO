
using System;
using Server;
using Server.Items;

namespace Server.Items
{
    public class SurcoatofArachnis : Surcoat
    {
        [Constructable]
        public SurcoatofArachnis()
        {
            
            this.Hue = 248;
            this.Name = "Golden Silk Surcoat of Arachnis";

            this.Attributes.AttackChance = 15;
            this.Attributes.BonusStam = 25;
            this.Attributes.BonusHits = 25;
            this.Attributes.CastRecovery = 2;
            this.Attributes.CastSpeed = 2;
            this.Attributes.DefendChance = 15;
            this.Attributes.LowerManaCost = 20;
            this.Attributes.LowerRegCost = 20;
            this.Attributes.BonusMana = 25;

            this.LootType = LootType.Regular;
        }

        public SurcoatofArachnis( Serial serial ) : base( serial )
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