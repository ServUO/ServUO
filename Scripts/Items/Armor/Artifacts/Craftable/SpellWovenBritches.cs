using System;

namespace Server.Items
{
    public class SpellWovenBritches : LeafLegs
    {
        [Constructable]
        public SpellWovenBritches()
        {
            this.Hue = 0x487;

            this.SkillBonuses.SetValues(0, SkillName.Meditation, 10.0);

            this.Attributes.BonusInt = 8;
            this.Attributes.SpellDamage = 10;
            this.Attributes.LowerManaCost = 10;
        }

        public SpellWovenBritches(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072929;
            }
        }// Spell Woven Britches
        public override int BaseFireResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 16;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}