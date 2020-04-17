namespace Server.Items
{
    public class BlueKey1 : AbyssKey
    {
        [Constructable]
        public BlueKey1()
            : base(0x1012)
        {
            Weight = 1.0;
            Hue = 0x5D; // TODO check
            LootType = LootType.Blessed;
            Movable = false;
        }

        public BlueKey1(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1111646;// Blue Key Fragment
        public override int Lifespan => 21600;
        public override void OnDoubleClick(Mobile m)
        {
            Item a = m.Backpack.FindItemByType(typeof(RedKey1));
            if (a != null)
            {
                Item b = m.Backpack.FindItemByType(typeof(YellowKey1));
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