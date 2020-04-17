namespace Server.Items
{
    public class PartiallyDigestedTorso : PeerlessKey
    {
        [Constructable]
        public PartiallyDigestedTorso()
            : base(0x1D9F)
        {
            Weight = 1.0;
        }

        public PartiallyDigestedTorso(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074326;// partially digested torso
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