using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public enum KoiPondSize
    {
        Small = 1,
        Medium,
        Large,
    }

    public class KoiPondAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new KoiPondDeed();

        [Constructable]
        public KoiPondAddon(KoiPondSize size)
        {
            switch (size)
            {
                case KoiPondSize.Large:
                    {
                        AddComponent(new AddonComponent(41037), 3, 0, 1);
                        AddComponent(new AddonComponent(41038), 3, 0, 1);
                        AddComponent(new AddonComponent(41035), 3, -2, 1);
                        AddComponent(new AddonComponent(41036), 3, -1, 1);
                        AddComponent(new AddonComponent(41038), 3, 1, 1);
                        AddComponent(new AddonComponent(41016), 3, 3, 1);
                        AddComponent(new AddonComponent(41039), 3, 2, 1);
                        AddComponent(new AddonComponent(41024), -3, 1, 0);
                        AddComponent(new AddonComponent(41022), -3, 3, 1);
                        AddComponent(new AddonComponent(41021), -2, 3, 1);
                        AddComponent(new AddonComponent(41020), -1, 3, 1);
                        AddComponent(new AddonComponent(41019), 0, 3, 1);
                        AddComponent(new AddonComponent(41025), -3, 0, 1);
                        AddComponent(new AddonComponent(41018), 1, 3, 1);
                        AddComponent(new AddonComponent(41032), 1, -3, 1);
                        AddComponent(new AddonComponent(41031), 0, -3, 1);
                        AddComponent(new AddonComponent(41026), -3, -1, 1);
                        AddComponent(new AddonComponent(41027), -3, -2, 1);
                        AddComponent(new AddonComponent(41029), -2, -3, 1);
                        AddComponent(new AddonComponent(41030), -1, -3, 1);
                        AddComponent(new AddonComponent(41017), 2, 3, 1);
                        AddComponent(new AddonComponent(41040), 2, 2, 1);

                        break;
                    }
                case KoiPondSize.Medium:
                    {
                        AddComponent(new AddonComponent(41004), 2, -1, 0);
                        AddComponent(new AddonComponent(40977), 2, 2, 0);
                        AddComponent(new AddonComponent(41006), 2, 1, 0);
                        AddComponent(new AddonComponent(41005), 2, 0, 0);
                        AddComponent(new AddonComponent(41001), 1, -2, 0);
                        AddComponent(new AddonComponent(41000), -1, -2, 0);
                        AddComponent(new AddonComponent(40999), 0, -2, 0);
                        AddComponent(new AddonComponent(40998), -2, -1, 0);
                        AddComponent(new AddonComponent(40997), -2, 0, 0);
                        AddComponent(new AddonComponent(40996), -2, 1, 0);
                        AddComponent(new AddonComponent(40995), -2, 2, 0);
                        AddComponent(new AddonComponent(40994), -1, 2, 0);
                        AddComponent(new AddonComponent(40993), 0, 2, 0);
                        AddComponent(new AddonComponent(40992), 1, 2, 0);
                        AddComponent(new AddonComponent(41007), 2, 2, 1);

                        break;
                    }
                case KoiPondSize.Small:
                    {
                        AddComponent(new AddonComponent(40980), -1, 0, 0);
                        AddComponent(new AddonComponent(40978), 0, 1, 0);
                        AddComponent(new AddonComponent(40984), 1, 0, 0);
                        AddComponent(new AddonComponent(40983), 1, -1, 0);
                        AddComponent(new AddonComponent(40982), 0, -1, 0);
                        AddComponent(new AddonComponent(40981), -1, -1, 0);
                        AddComponent(new AddonComponent(40980), 1, 0, 0);
                        AddComponent(new AddonComponent(40979), -1, 1, 0);
                        AddComponent(new AddonComponent(40978), 1, -1, 0);
                        AddComponent(new AddonComponent(40977), 1, 1, 0);
                        AddComponent(new AddonComponent(40985), 1, 1, 1);

                        break;
                    }
            }
        }

        public KoiPondAddon(Serial serial)
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

    public class KoiPondDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1157996;  // Koi Pond
        public override BaseAddon Addon => new KoiPondAddon(m_Size);
        public KoiPondSize m_Size;

        [Constructable]
        public KoiPondDeed()
        {
            LootType = LootType.Blessed;
        }

        public KoiPondDeed(Serial serial)
            : base(serial)
        {
        }

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

        private void SendTarget(Mobile m)
        {
            base.OnDoubleClick(m);
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
            private readonly KoiPondDeed m_Deed;

            public InternalGump(KoiPondDeed deed) : base(60, 36)
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
                AddHtmlLocalized(14, 12, 273, 20, 1071175, 0x7FFF, false, false); // Please select your item

                AddPage(1);

                AddButton(19, 49, 0x845, 0x846, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(44, 47, 213, 20, 1158000, 0x7FFF, false, false); // Small Koi Pond

                AddButton(19, 73, 0x845, 0x846, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(44, 71, 213, 20, 1158001, 0x7FFF, false, false); // Medium Koi Pond

                AddButton(19, 97, 0x845, 0x846, 3, GumpButtonType.Reply, 0);
                AddHtmlLocalized(44, 95, 213, 20, 1158002, 0x7FFF, false, false); // Large Koi Pond
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Deed == null || m_Deed.Deleted || info.ButtonID == 0)
                    return;

                m_Deed.m_Size = (KoiPondSize)info.ButtonID;
                m_Deed.SendTarget(sender.Mobile);
            }
        }
    }
}