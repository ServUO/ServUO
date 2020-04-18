namespace Server.Items
{
    public class WispCostume : BaseCostume
    {
        public override string CreatureName => "wisp";

        [Constructable]
        public WispCostume() : base()
        {
            CostumeBody = 58;
        }

        public override int LabelNumber => 1114225;// wisp costume


        public WispCostume(Serial serial) : base(serial)
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
