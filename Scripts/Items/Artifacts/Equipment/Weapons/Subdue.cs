using System;

namespace Server.Items
{
    public class Subdue : Scythe
    {
        [Constructable]
        public Subdue()
        {
            this.Hue = 0x2cb;

            this.Attributes.SpellChanneling = 1;
            this.Attributes.WeaponSpeed = 20;
            this.Attributes.WeaponDamage = 50;
            this.Attributes.AttackChance = 10;

            this.WeaponAttributes.HitLeechMana = 100;
            this.WeaponAttributes.UseBestSkill = 1;
        }

        public Subdue(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094930;
            }
        }// Subdue [Replica]
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override bool CanFortify
        {
            get
            {
                return false;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}