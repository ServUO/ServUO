using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class UnmadeBedAddon : BaseAddon
    {
        [Constructable]
        public UnmadeBedAddon(bool east)
            : base()
        {
            if (east) // east
            {
                AddComponent(new LocalizedAddonComponent(0xA8C, 1076279), 0, 0, 0);
                AddComponent(new LocalizedAddonComponent(0xA8D, 1076279), 0, -1, 0);
                AddComponent(new LocalizedAddonComponent(0xA90, 1076279), -1, 0, 0);
                AddComponent(new LocalizedAddonComponent(0xA91, 1076279), -1, -1, 0);
            }
            else // south
            {
                AddComponent(new LocalizedAddonComponent(0xDB0, 1076279), 0, 0, 0);
                AddComponent(new LocalizedAddonComponent(0xDB1, 1076279), -1, 0, 0);
                AddComponent(new LocalizedAddonComponent(0xDB4, 1076279), 0, -1, 0);
                AddComponent(new LocalizedAddonComponent(0xDB5, 1076279), -1, -1, 0);
            }
        }

        public UnmadeBedAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new UnmadeBedDeed();
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

    public class UnmadeBedDeed : BaseAddonDeed
    {
        private bool m_East;
        [Constructable]
        public UnmadeBedDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public UnmadeBedDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new UnmadeBedAddon(m_East);
        public override int LabelNumber => 1076279;// Unmade Bed
        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
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

        private void SendTarget(Mobile m)
        {
            base.OnDoubleClick(m);
        }

        private class InternalGump : Gump
        {
            private readonly UnmadeBedDeed m_Deed;
            public InternalGump(UnmadeBedDeed deed)
                : base(60, 36)
            {
                m_Deed = deed;

                AddPage(0);

                AddBackground(0, 0, 273, 324, 0x13BE);
                AddImageTiled(10, 10, 253, 20, 0xA40);
                AddImageTiled(10, 40, 253, 244, 0xA40);
                AddImageTiled(10, 294, 253, 20, 0xA40);
                AddAlphaRegion(10, 10, 253, 304);
                AddButton(10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 296, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
                AddHtmlLocalized(14, 12, 273, 20, 1076580, 0x7FFF, false, false); // Pleae select your unmade bed position

                AddPage(1);

                AddButton(19, 49, 0x845, 0x846, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(44, 47, 213, 20, 1075386, 0x7FFF, false, false); // South
                AddButton(19, 73, 0x845, 0x846, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(44, 71, 213, 20, 1075387, 0x7FFF, false, false); // East
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Deed == null || m_Deed.Deleted || info.ButtonID == 0)
                    return;

                m_Deed.m_East = (info.ButtonID != 1);
                m_Deed.SendTarget(sender.Mobile);
            }
        }
    }
}