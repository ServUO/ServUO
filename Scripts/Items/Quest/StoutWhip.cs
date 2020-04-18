namespace Server.Items
{
    public class StoutWhip : Item
    {
        [Constructable]
        public StoutWhip()
            : base(0x166F)
        {
            LootType = LootType.Blessed;
            Weight = 1;
        }

        public StoutWhip(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074812;// Stout Whip
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