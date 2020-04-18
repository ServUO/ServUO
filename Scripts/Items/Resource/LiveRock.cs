namespace Server.Items
{
    public class LiveRock : Item, ICommodity
    {
        public override int LabelNumber => 1125985;  // live rock

        [Constructable]
        public LiveRock()
            : this(1)
        {
        }

        [Constructable]
        public LiveRock(int amount)
            : base(0xA3E9)
        {
            Stackable = true;
            Amount = amount;
        }

        public LiveRock(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

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
