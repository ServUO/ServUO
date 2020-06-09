using Server.Engines.Craft;

namespace Server.Items
{
    public class MantleOfTheFallen : GargishClothChest, IRepairable
    {
        public CraftSystem RepairSystem => DefTailoring.CraftSystem;

        public override bool IsArtifact => true;
        public override int LabelNumber => 1113819; // Mantle of the Fallen
        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 8;
        public override int BaseColdResistance => 11;
        public override int BasePoisonResistance => 12;
        public override int BaseEnergyResistance => 8;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public MantleOfTheFallen()
        {
            Hue = 1512;
            Attributes.LowerRegCost = 25;
            Attributes.BonusInt = 8;
            Attributes.BonusMana = 8;
            Attributes.RegenMana = 1;
            SAAbsorptionAttributes.CastingFocus = 3;
            Attributes.SpellDamage = 5;
        }

        public MantleOfTheFallen(Serial serial)
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
