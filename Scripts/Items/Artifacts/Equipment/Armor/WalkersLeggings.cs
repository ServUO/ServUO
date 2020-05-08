namespace Server.Items
{
    public class WalkersLeggings : LeatherNinjaPants
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1078222;// Walker's Leggings
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 6;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 3;

        [Constructable]
        public WalkersLeggings()
        {
            LootType = LootType.Blessed;
        }

        public WalkersLeggings(Serial serial)
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
