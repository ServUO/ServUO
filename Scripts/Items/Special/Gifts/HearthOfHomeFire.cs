using System;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class HearthOfHomeFire : BaseAddon
    {
        [Constructable]
        public HearthOfHomeFire(bool east)
        {
            if (east)
            {
                this.AddLightComponent(new AddonComponent(0x2352), 0, 0, 0);
                this.AddLightComponent(new AddonComponent(0x2358), 0, -1, 0);
            }
            else
            {
                this.AddLightComponent(new AddonComponent(0x2360), 0, 0, 0);
                this.AddLightComponent(new AddonComponent(0x2366), -1, 0, 0);
            }
        }

        public HearthOfHomeFire(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new HearthOfHomeFireDeed();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        private void AddLightComponent(AddonComponent component, int x, int y, int z)
        {
            component.Light = LightType.Circle150;

            this.AddComponent(component, x, y, z);
        }
    }

    public class HearthOfHomeFireDeed : BaseAddonDeed
    {
        private bool m_East;
        [Constructable]
        public HearthOfHomeFireDeed()
        {
            this.LootType = LootType.Blessed;
        }

        public HearthOfHomeFireDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new HearthOfHomeFire(this.m_East);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1062919;
            }
        }// Hearth of the Home Fire
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
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
            private readonly HearthOfHomeFireDeed m_Deed;
            public InternalGump(HearthOfHomeFireDeed deed)
                : base(150, 50)
            {
                this.m_Deed = deed;

                this.AddBackground(0, 0, 350, 250, 0xA28);

                this.AddItem(90, 52, 0x2367);
                this.AddItem(112, 35, 0x2360);
                this.AddButton(70, 35, 0x868, 0x869, 1, GumpButtonType.Reply, 0); // South

                this.AddItem(220, 35, 0x2352);
                this.AddItem(242, 52, 0x2358);
                this.AddButton(185, 35, 0x868, 0x869, 2, GumpButtonType.Reply, 0); // East
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Deed.Deleted || info.ButtonID == 0)
                    return;

                this.m_Deed.m_East = (info.ButtonID != 1);
                this.m_Deed.SendTarget(sender.Mobile);
            }
        }
    }
}