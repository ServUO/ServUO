namespace Server.Items
{
    public class GiantToadCostume : BaseCostume
    {
        public override string CreatureName => "giant toad";

        [Constructable]
        public GiantToadCostume() : base()
        {
            CostumeBody = 80;
        }

        public override int LabelNumber => 1114226;// giant toad costume

        public GiantToadCostume(Serial serial) : base(serial)
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
