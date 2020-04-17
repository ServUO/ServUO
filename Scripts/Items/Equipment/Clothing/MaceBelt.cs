namespace Server.Items
{
    public class MaceBelt : BaseWaist, IDyable
    {
        public override int LabelNumber => 1126020;  // mace belt

        [Constructable]
        public MaceBelt()
            : base(0xA40C)
        {
            Weight = 3.0;
            Layer = Layer.Waist;
        }

        public MaceBelt(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
