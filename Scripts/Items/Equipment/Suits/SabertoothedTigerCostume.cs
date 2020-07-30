namespace Server.Items
{
    [TypeAlias("Server.Items.SabertoothedTigerCostume")]
    public class SabreToothedTigerCostume : BaseCostume
    {
        public override string CreatureName => "sabre-toothed tiger";

        [Constructable]
        public SabreToothedTigerCostume()
            : base()
        {
            CostumeBody = 0x588;
        }

        public override string DefaultName => "a sabre-toothed tiger costume";

        public SabreToothedTigerCostume(Serial serial)
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
            reader.ReadInt();
        }
    }
}
