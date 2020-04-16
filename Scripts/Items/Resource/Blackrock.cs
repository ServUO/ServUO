namespace Server.Items
{
    public class Blackrock : Item
    {
        [Constructable]
        public Blackrock()
            : base(Utility.RandomList(0x136C, 0x1EA7))
        {
            Hue = 1954;
        }

        public Blackrock(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                switch (ItemID)
                {
                    case 0x136C: return 1153837; // a large piece of blackrock
                    case 0x1EA7: return 1150016; // a small piece of blackrock
                    default: return 1153836; // a piece of blackrock
                }
            }
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