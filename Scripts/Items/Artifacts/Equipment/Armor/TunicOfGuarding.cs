namespace Server.Items
{
    public class TunicOfGuarding : LeatherChest
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1077693;// Tunic of Guarding
        public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        [Constructable]
        public TunicOfGuarding()
        {
            LootType = LootType.Blessed;
            Attributes.BonusHits = 2;
            Attributes.ReflectPhysical = 5;
        }

        public TunicOfGuarding(Serial serial)
            : base(serial)
        {
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
