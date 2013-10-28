using System;

namespace Server.Items
{
    public class OblivionsNeedle : Dagger
    {
        [Constructable]
        public OblivionsNeedle()
        {
            this.Attributes.BonusStam = 20;
            this.Attributes.AttackChance = 20;
            this.Attributes.DefendChance = -20;
            this.Attributes.WeaponDamage = 40;

            this.WeaponAttributes.HitLeechStam = 50;
        }

        public OblivionsNeedle(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094916;
            }
        }// Oblivion's Needle [Replica]
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