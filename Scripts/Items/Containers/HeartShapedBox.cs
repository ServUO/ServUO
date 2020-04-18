namespace Server.Items
{
    [Flipable(0x49CC, 0x49D0)]
    public class HeartShapedBox : Container
    {
        public override int LabelNumber => 1097762;  // heart shaped box

        [Constructable]
        public HeartShapedBox()
            : base(0x49CC)
        {
            Weight = 1.0;
            GumpID = 0x120;

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    PlaceItemIn(new ValentineChocolate(), 60 + (10 * i), 35 + (j * 13));
                }
            }
        }

        private void PlaceItemIn(Item item, int x, int y)
        {
            AddItem(item);
            item.Location = new Point3D(x, y, 0);
        }

        public HeartShapedBox(Serial serial)
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
