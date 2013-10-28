using System;

namespace Server.Items
{
    public class ResilientBracer : GoldBracelet
    {
        [Constructable]
        public ResilientBracer()
        {
            this.Hue = 0x488;

            this.SkillBonuses.SetValues(0, SkillName.MagicResist, 15.0);

            this.Attributes.BonusHits = 5;
            this.Attributes.RegenHits = 2;
            this.Attributes.DefendChance = 10;
        }

        public ResilientBracer(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072933;
            }
        }// Resillient Bracer
        public override int PhysicalResistance
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