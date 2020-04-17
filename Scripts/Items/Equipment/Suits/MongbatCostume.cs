namespace Server.Items
{
    public class MongbatCostume : BaseCostume
    {
        public override string CreatureName => "mongbat";

        [Constructable]
        public MongbatCostume() : base()
        {
            CostumeBody = 39;
        }

        public override int LabelNumber => 1114223;// mongbat costume

        public MongbatCostume(Serial serial) : base(serial)
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
