namespace Server.Items
{
    public class TaintedSeeds : Item
    {
        [Constructable]
        public TaintedSeeds()
            : base(0xDFA)
        {
            LootType = LootType.Blessed;
            Hue = 0x48; // TODO check
        }

        public TaintedSeeds(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074233;// Tainted Seeds
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