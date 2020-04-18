namespace Server.Items
{
    public class RedKey1 : AbyssKey
    {
        [Constructable]
        public RedKey1()
            : base(0x1012)
        {
            Weight = 1.0;
            Hue = 0x8F; // TODO check
            LootType = LootType.Blessed;
            Movable = false;
        }

        public RedKey1(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1111647;
        public override int Lifespan => 21600;
        public override void OnDoubleClick(Mobile m)
        {
            Item a = m.Backpack.FindItemByType(typeof(YellowKey1));
            if (a != null)
            {
                Item b = m.Backpack.FindItemByType(typeof(BlueKey1));
                if (b != null)
                {
                    m.AddToBackpack(new TripartiteKey());
                    a.Delete();
                    b.Delete();
                    Delete();
                    m.SendLocalizedMessage(1111649);
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