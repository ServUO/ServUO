namespace Server.Items
{
    public class BagOfNecroReagents : Bag
    {
        [Constructable]
        public BagOfNecroReagents()
            : this(50)
        {
        }

        [Constructable]
        public BagOfNecroReagents(int amount)
        {
            DropItem(new BatWing(amount));
            DropItem(new GraveDust(amount));
            DropItem(new DaemonBlood(amount));
            DropItem(new NoxCrystal(amount));
            DropItem(new PigIron(amount));
        }

        public BagOfNecroReagents(Serial serial)
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