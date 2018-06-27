using System;
using Server;
using Server.Spells;

namespace Server.Items
{
    public class GargishDupresSword : StoneWarSword
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishDupresSword()
        {
            Hue = 0xA91;

            Attributes.BonusStr = 10;
            Attributes.AttackChance = 25;
            Attributes.WeaponSpeed = 35;
            Attributes.WeaponDamage = 100;
            WeaponAttributes.HitManaDrain = 50;
        }
        
        public GargishDupresSword(Serial serial) : base(serial)
        {
        }

        public override bool CanFortify { get { return false; } }

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