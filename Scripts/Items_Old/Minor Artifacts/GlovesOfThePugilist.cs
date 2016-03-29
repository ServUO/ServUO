using System;

namespace Server.Items
{
    public class GlovesOfThePugilist : LeatherGloves
    {
        [Constructable]
        public GlovesOfThePugilist()
        {
            this.Hue = 0x6D1;
            this.SkillBonuses.SetValues(0, SkillName.Wrestling, 10.0);
            this.Attributes.BonusDex = 8;
            this.Attributes.WeaponDamage = 15;
        }

        public GlovesOfThePugilist(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070690;
            }
        }
        public override int BasePhysicalResistance
        {
            get
            {
                return 18;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
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