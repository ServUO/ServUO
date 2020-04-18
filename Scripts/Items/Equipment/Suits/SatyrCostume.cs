namespace Server.Items
{
    public class SatyrCostume : BaseCostume
    {
        public override string CreatureName => "satyr";

        [Constructable]
        public SatyrCostume() : base()
        {
            CostumeBody = 271;
        }

        public override int LabelNumber => 1114287;// satyr costume

        public SatyrCostume(Serial serial) : base(serial)
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
