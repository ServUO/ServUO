namespace Server.Items
{
    public class SpeckledPoisonSac : PeerlessKey
    {
        [Constructable]
        public SpeckledPoisonSac()
            : base(0x23A)
        {
            LootType = LootType.Blessed;
            Weight = 2.0;
        }

        public SpeckledPoisonSac(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073133;// Speckled Poison Sac
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