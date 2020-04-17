namespace Server.Items
{
    public class DrakeCostume : BaseCostume
    {
        public override string CreatureName => "drake";

        [Constructable]
        public DrakeCostume() : base()
        {
            CostumeBody = 60;
        }

        public override int LabelNumber => 1114245;// drake costume

        public DrakeCostume(Serial serial) : base(serial)
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
