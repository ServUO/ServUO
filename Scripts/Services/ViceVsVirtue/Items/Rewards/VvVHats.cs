using Server.Items;

namespace Server.Engines.VvV
{
    public class VvVWizardsHat : WizardsHat
    {
        public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 6;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 25;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public VvVWizardsHat()
        {
            Attributes.BonusHits = 5;
            Attributes.RegenMana = 3;
            Attributes.DefendChance = 4;
            Attributes.SpellDamage = 10;
            Attributes.LowerRegCost = 20;
        }

        public VvVWizardsHat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
                Timer.DelayCall(() => ViceVsVirtueSystem.Instance.AddVvVItem(this));
        }
    }

    public class VvVGargishEarrings : GargishEarrings
    {
        public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 6;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 25;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public VvVGargishEarrings()
        {
            Attributes.BonusHits = 5;
            Attributes.RegenMana = 3;
            Attributes.DefendChance = 4;
            Attributes.SpellDamage = 10;
            Attributes.LowerRegCost = 20;
        }

        public VvVGargishEarrings(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
                Timer.DelayCall(() => ViceVsVirtueSystem.Instance.AddVvVItem(this));
        }
    }
}