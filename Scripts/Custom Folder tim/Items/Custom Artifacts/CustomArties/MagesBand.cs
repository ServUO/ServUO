using System;
using Server;
namespace Server.Items
{
    public class MagesBand : GoldRing, ITokunoDyable
    {
public override int ArtifactRarity{ get{ return 15; } }
        [Constructable]
        public MagesBand()
        {
            Name = "Mages Band";
            Attributes.LowerRegCost = 15;
            Attributes.LowerManaCost = 5;
            Hue = 1170;
            Attributes.CastRecovery = 3;
            Attributes.BonusMana = 15;
            Attributes.RegenMana = 5;
        }
        public MagesBand( Serial serial )
            : base( serial )
        {
        }
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }
        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}
