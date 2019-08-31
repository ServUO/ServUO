using System;

namespace Server.Items
{
    public class MagesHoodOfScholarlyInsight : MagesHood
    {
        public override int LabelNumber { get { return 1159229; } } // mage's hood of scholarly insight

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public MagesHoodOfScholarlyInsight()
            : this(0)
        {
        }

        [Constructable]
        public MagesHoodOfScholarlyInsight(int hue)
            : base(hue)
        {
            Attributes.BonusMana = 15;
            Attributes.RegenMana = 2;
            Attributes.SpellDamage = 15;
            Attributes.CastSpeed = 1;
            Attributes.LowerManaCost = 10;
        }

        public MagesHoodOfScholarlyInsight(Serial serial)
            : base(serial)
        {
        }

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
