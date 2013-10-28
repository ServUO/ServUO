using System;

namespace Server.Items
{
    public class StitchersMittens : LeafGloves
    {
        [Constructable]
        public StitchersMittens()
        {
            this.Hue = 0x481;

            this.SkillBonuses.SetValues(0, SkillName.Healing, 10.0);

            this.Attributes.BonusDex = 5;
            this.Attributes.LowerRegCost = 30;
        }

        public StitchersMittens(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072932;
            }
        }// Stitcher's Mittens
        public override int BasePhysicalResistance
        {
            get
            {
                return 20;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 20;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}