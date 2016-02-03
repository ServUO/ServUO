using System;

namespace Server.Items
{
    [FlipableAttribute(0xF62, 0xF63)]
    public class LereisHuntingSpear : Spear
    {
        [Constructable]
        public LereisHuntingSpear()
        {
            WeaponAttributes.HitCurse = 10;
            WeaponAttributes.HitLeechMana = 50;
            Attributes.WeaponSpeed = 30;
            Attributes.AttackChance = 20;
            Attributes.WeaponDamage = 60;
            AosElementDamages.Poison = 100;
            Slayer = SlayerName.ReptilianDeath;
        }

        public LereisHuntingSpear(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1156128;
            }
        }// Lerei's Hunting Spear

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}