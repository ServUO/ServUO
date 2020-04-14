using Server.Gumps;

namespace Server.Items
{
    public class DetectiveBook : Item
    {
        [Constructable]
        public DetectiveBook() : base(4082)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1158598); // Book title
            list.Add(1154760, "K. Jasper, Chief Inspector"); // By: ~1_NAME~
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                Gump g = new Gump(150, 150);
                g.AddImage(0, 0, 30236);
                g.AddHtmlLocalized(110, 30, 350, 630, 1158600, false, false); // Book content

                m.SendGump(g);
            }
        }

        public DetectiveBook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
