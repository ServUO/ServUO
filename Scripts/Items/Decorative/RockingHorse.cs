namespace Server.Items
{
    [Flipable(0x4214, 0x4215)]
    public class RockingHorse : Item
    {
        [Constructable]
        public RockingHorse()
            : base(0x4214)
        {
            LootType = LootType.Blessed;
        }

        public RockingHorse(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight => 30.0;
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