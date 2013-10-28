using System;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class VanityAddon : BaseAddonContainer
    {
        [Constructable]
        public VanityAddon(bool east)
            : base(east ? 0xA44 : 0xA3C)
        {
            if (east) // east
            {
                this.AddComponent(new AddonContainerComponent(0xA45), 0, -1, 0);
            }
            else // south
            {
                this.AddComponent(new AddonContainerComponent(0xA3D), -1, 0, 0);
            }
        }

        public VanityAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed
        {
            get
            {
                return new VanityDeed();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074027;
            }
        }// Vanity
        public override int DefaultGumpID
        {
            get
            {
                return 0x51;
            }
        }
        public override int DefaultDropSound
        {
            get
            {
                return 0x42;
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
    }

    public class VanityDeed : BaseAddonContainerDeed
    {
        private bool m_East;
        [Constructable]
        public VanityDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public VanityDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainer Addon
        {
            get
            {
                return new VanityAddon(this.m_East);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074027;
            }
        }// Vanity
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
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
            private readonly VanityDeed m_Deed;
            public InternalGump(VanityDeed deed)
                : base(60, 36)
            {
                this.m_Deed = deed;

                this.AddPage(0);

                this.AddBackground(0, 0, 273, 324, 0x13BE);
                this.AddImageTiled(10, 10, 253, 20, 0xA40);
                this.AddImageTiled(10, 40, 253, 244, 0xA40);
                this.AddImageTiled(10, 294, 253, 20, 0xA40);
                this.AddAlphaRegion(10, 10, 253, 304);
                this.AddButton(10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 296, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
                this.AddHtmlLocalized(14, 12, 273, 20, 1076744, 0x7FFF, false, false); // Please select your vanity position.

                this.AddPage(1);

                this.AddButton(19, 49, 0x845, 0x846, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 47, 213, 20, 1075386, 0x7FFF, false, false); // South
                this.AddButton(19, 73, 0x845, 0x846, 2, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(44, 71, 213, 20, 1075387, 0x7FFF, false, false); // East
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Deed == null || this.m_Deed.Deleted || info.ButtonID == 0)
                    return;

                this.m_Deed.m_East = (info.ButtonID != 1);
                this.m_Deed.SendTarget(sender.Mobile);
            }
        }
    }
}