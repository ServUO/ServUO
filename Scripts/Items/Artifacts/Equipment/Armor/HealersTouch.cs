namespace Server.Items
{
    public class HealersTouch : LeatherGloves
    {
        public override bool IsArtifact => true;
        [Constructable]
        public HealersTouch()
        {
            LootType = LootType.Blessed;
            Attributes.BonusStam = 3;
            Attributes.ReflectPhysical = 5;
        }

        public HealersTouch(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1077684;// Healer's Touch
        public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;
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