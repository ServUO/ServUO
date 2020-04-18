namespace Server.Items
{
    public class OphidianMatriarchCostume : BaseCostume
    {
        public override string CreatureName => "ophidian matriarch";

        [Constructable]
        public OphidianMatriarchCostume() : base()
        {
            CostumeBody = 87;
        }

        public override int LabelNumber => 1114230;// ophidian matriarch costume

        public OphidianMatriarchCostume(Serial serial) : base(serial)
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
