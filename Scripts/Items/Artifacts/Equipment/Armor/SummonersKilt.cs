
using Server.Engines.Craft;

namespace Server.Items
{
    public class SummonersKilt : GargishClothKilt, IRepairable
    {
        public CraftSystem RepairSystem => DefTailoring.CraftSystem;

        public override bool IsArtifact => true;
        public override int LabelNumber => 1113540;  // Summoner's Kilt
        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 7;
        public override int BaseColdResistance => 21;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 21;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public SummonersKilt()
        {
            Hue = 1266;
            Attributes.BonusMana = 5;
            Attributes.RegenMana = 2;
            Attributes.SpellDamage = 5;
            SAAbsorptionAttributes.CastingFocus = 2;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 10;
        }

        public SummonersKilt(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
