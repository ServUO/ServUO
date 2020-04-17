using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x456E, 0x456F)]
    public class SnowStatuePegasus : Item
    {
        [Constructable]
        public SnowStatuePegasus()
            : base(0x456E)
        {
        }

        public SnowStatuePegasus(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    [Flipable(0x4578, 0x4579)]
    public class SnowStatueSeahorse : Item
    {
        [Constructable]
        public SnowStatueSeahorse()
            : base(0x4578)
        {
        }

        public SnowStatueSeahorse(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    [Flipable(0x457A, 0x457B)]
    public class SnowStatueMermaid : Item
    {
        [Constructable]
        public SnowStatueMermaid()
            : base(0x457A)
        {
        }

        public SnowStatueMermaid(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    [Flipable(0x457C, 0x457D)]
    public class SnowStatueGriffon : Item
    {
        [Constructable]
        public SnowStatueGriffon()
            : base(0x457C)
        {
        }

        public SnowStatueGriffon(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class SnowStatueDeed : Item
    {
        [Constructable]
        public SnowStatueDeed()
            : base(0x14F0)
        {
            LootType = LootType.Blessed;
        }

        public SnowStatueDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1114296;// snow statue deed
        public override double DefaultWeight => 1.0;
        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
            {
                from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        private class InternalGump : Gump
        {
            private readonly SnowStatueDeed m_Deed;
            public InternalGump(SnowStatueDeed deed)
                : base(100, 200)
            {
                m_Deed = deed;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage(0);
                AddBackground(0, 0, 360, 225, 0xA28);

                AddPage(1);
                AddLabel(45, 15, 0, "Select One:");

                AddItem(35, 75, 0x456E);
                AddButton(65, 50, 0x845, 0x846, 1, GumpButtonType.Reply, 0);

                AddItem(120, 75, 0x4578);
                AddButton(135, 50, 0x845, 0x846, 2, GumpButtonType.Reply, 0);

                AddItem(190, 75, 0x457A);
                AddButton(205, 50, 0x845, 0x846, 3, GumpButtonType.Reply, 0);

                AddItem(250, 75, 0x457C);
                AddButton(275, 50, 0x845, 0x846, 4, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Deed == null || m_Deed.Deleted)
                    return;

                Mobile from = sender.Mobile;

                if (!m_Deed.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it
                    return;
                }

                Item statue = null;

                switch (info.ButtonID)
                {
                    case 1:
                        statue = new SnowStatuePegasus();
                        break;
                    case 2:
                        statue = new SnowStatueSeahorse();
                        break;
                    case 3:
                        statue = new SnowStatueMermaid();
                        break;
                    case 4:
                        statue = new SnowStatueGriffon();
                        break;
                }

                if (statue == null)
                    return;

                if (!from.PlaceInBackpack(statue))
                {
                    statue.Delete();
                    from.SendLocalizedMessage(1078837); // Your backpack is full! Please make room and try again.
                }
                else
                {
                    m_Deed.Delete();
                }
            }
        }
    }
}
