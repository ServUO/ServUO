namespace Server.Items
{
    public class CusteauPerronHouseSign : Item
    {
        [Constructable]
        public CusteauPerronHouseSign()
            : base(3026)
        {
            Movable = false;
            Name = "Cousteau Perron's";
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1044097); // Tinkering
        }

        public CusteauPerronHouseSign(Serial serial)
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
            reader.ReadInt();
        }
    }
}
