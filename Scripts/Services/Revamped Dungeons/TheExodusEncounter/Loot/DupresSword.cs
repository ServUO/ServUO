using System;
using Server;
using Server.Spells;

namespace Server.Items
{
    public class DupresSword : VikingSword
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public DupresSword()
        {
            Hue = 0xA91;
            Attributes.BonusStr = 10;
            Attributes.AttackChance = 25;
            Attributes.WeaponSpeed = 35;
            Attributes.WeaponDamage = 100;
            WeaponAttributes.HitManaDrain = 50;
        }
        
        public DupresSword(Serial serial) : base(serial)
        {
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override int LabelNumber { get { return 1153551; } }       

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
