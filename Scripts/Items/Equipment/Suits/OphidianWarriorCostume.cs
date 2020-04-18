namespace Server.Items
{
    public class OphidianWarriorCostume : BaseCostume
    {
        public override string CreatureName => "ophidian warrior";

        [Constructable]
        public OphidianWarriorCostume() : base()
        {
            CostumeBody = 86;
        }

        public OphidianWarriorCostume(Serial serial) : base(serial)
        {
        }

        public override int LabelNumber => 1114229;// ophidian warrior costume

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
