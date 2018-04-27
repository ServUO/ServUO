using System;
using Server.Gumps;
using Server.Network;
using Server.ContextMenus;
using System.Collections.Generic;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public class GardenShedComponent : AddonContainerComponent
    {
        public override int LabelNumber { get { return 1153492; } } // garden shed

        public GardenShedComponent(int itemID)
            : base(itemID)
        {
            Weight = 0;
        }

        public override void OnDoubleClick(Mobile from)
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
        }

        public GardenShedComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GardenShedAddon : BaseAddonContainer
    {
        public override BaseAddonContainerDeed Deed { get { return new GardenShedDeed(); } }
        public override int LabelNumber { get { return 1153492; } } // garden shed
        public override int DefaultGumpID { get { return 0x10B; } }
        public override int DefaultDropSound { get { return 0x42; } }
        public BaseAddonContainer m_SecondContainer;
        private Point3D m_Offset;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseAddonContainer SecondContainer
        {
            get { return m_SecondContainer; }
            set { m_SecondContainer = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Offset
        {
            get { return m_Offset; }
            set { m_Offset = value; }
        }

        [Constructable]
        public GardenShedAddon(bool east) : base(east ? 0x4BEB : 0x4BE7)
        {
            SecondContainer = new GardenShedBarrel(this, east);

            if (east) // East
            {
                AddComponent(new GardenShedComponent(0x4BEA), 0, 1, 0);                
                AddComponent(new GardenShedComponent(0x4BEC), 0, -1, 0);
                AddComponent(new GardenShedComponent(0x4BF1), -1, 1, 0);
                AddComponent(new GardenShedComponent(0x4BF0), -1, 0, 0);
                AddComponent(new GardenShedComponent(0x4BEF), -2, -1, 0);
                AddComponent(new GardenShedComponent(0x4BEE), -1, -2, 0);
                AddComponent(new GardenShedComponent(0x4BF5), -2, -2, 0);
                AddComponent(new GardenShedComponent(0x4BF3), -2, 0, 0);
                AddComponent(new GardenShedComponent(0x4BEA), -2, 1, 0);
                Offset = new Point3D(0, -2, 0);
            }
            else    // South
            {
                AddComponent(new GardenShedComponent(0x4BE2), 2, -1, 0);
                AddComponent(new GardenShedComponent(0x4BE5), -1, -1, 0);
                AddComponent(new GardenShedComponent(0x4BDE), -1, -2, 0);
                AddComponent(new GardenShedComponent(0x4BE1), 2, -2, 0);
                AddComponent(new GardenShedComponent(0x4BE8), 1, 0, 0);
                AddComponent(new GardenShedComponent(0x4BE3), 1, -1, 0);
                AddComponent(new GardenShedComponent(0x4BE6), -1, 0, 0);                
                AddComponent(new GardenShedComponent(0x4BE0), 1, -2, 0);
                AddComponent(new GardenShedComponent(0x4BE4), 0, -1, 0);
                Offset = new Point3D(2, 0, 0);
            }
        }

        public GardenShedAddon(Serial serial) : base(serial)
        {
        }

        public override void OnLocationChange(Point3D old)
        {
            base.OnLocationChange(old);

            if (SecondContainer != null)
            {
                SecondContainer.MoveToWorld(new Point3D(X + m_Offset.X, Y + m_Offset.Y, Z + m_Offset.Z), Map);
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Deleted)
                return;

            if (SecondContainer != null)
            {
                SecondContainer.Map = Map;
            }
        }

        public override void OnDelete()
        {
            if (SecondContainer != null)
                SecondContainer.Delete();

            base.OnDelete();
        }

        public override void OnChop(Mobile from)
        {
            if (!SecondContainer.IsSecure)
            {
                SecondContainer.DropItemsToGround();
                base.OnChop(from);
            }
            else
            {
                from.SendLocalizedMessage(1074870); // This item must be unlocked/unsecured before re-deeding it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_SecondContainer);
            writer.Write(m_Offset);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_SecondContainer = reader.ReadItem() as BaseAddonContainer;
            m_Offset = reader.ReadPoint3D();
        }
    }

    [TypeAlias("Server.Items.GardenShedAddonSecond")]
    public class GardenShedBarrel : BaseAddonContainer
    {
        public GardenShedAddon m_MainContainer;

        [Constructable]
        public GardenShedBarrel(GardenShedAddon container, bool east)
            : base(east ? 0x4BED : 0x4BE9)
        {
            m_MainContainer = container;
        }

        public GardenShedBarrel(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed { get { return m_MainContainer.Deed; } }
        public override int LabelNumber { get { return 1153492; } } // garden shed
        public override int DefaultGumpID { get { return 0x3E; } }
        public override int DefaultDropSound { get { return 0x42; } }

        public override void OnLocationChange(Point3D old)
        {
            m_MainContainer.Location = new Point3D(X - m_MainContainer.Offset.X, Y - m_MainContainer.Offset.Y, Z - m_MainContainer.Offset.Z);
        }

        public override void OnMapChange()
        {
            if (m_MainContainer != null)
                m_MainContainer.Map = Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_MainContainer != null)
                m_MainContainer.Delete();
        }

        public override void OnChop(Mobile from)
        {
            if (m_MainContainer == null)
                return;

            if (!m_MainContainer.IsSecure)
            {
                m_MainContainer.DropItemsToGround();
                base.OnChop(from);
            }
            else
            {
                from.SendLocalizedMessage(1074870); // This item must be unlocked/unsecured before re-deeding it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_MainContainer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_MainContainer = reader.ReadItem() as GardenShedAddon;
        }
    }

    public class GardenShedDeed : BaseAddonContainerDeed, IRewardItem
    {
        public override BaseAddonContainer Addon { get { return new GardenShedAddon(m_East); } }
        public override int LabelNumber { get { return 1153491; } } // Garden Shed Deed

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem { get; set; }

        private bool m_East;

        [Constructable]
        public GardenShedDeed() : base()
        {
            LootType = LootType.Blessed;
        }

        public GardenShedDeed(Serial serial) : base(serial)
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

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if(IsRewardItem)
                list.Add(1113805); // 15th Year Veteran Reward
        }

        private void SendTarget(Mobile m)
        {
            base.OnDoubleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1); // version

            writer.Write(IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            if (version > 0)
                IsRewardItem = reader.ReadBool();
        }

        private class InternalGump : Gump
        {
            private GardenShedDeed m_Deed;

            public InternalGump(GardenShedDeed deed) : base(60, 36)
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
                AddHtmlLocalized(14, 12, 273, 20, 1071175, 0x7FFF, false, false); // Please select your vanity position.

                AddPage(1);

                AddButton(19, 49, 0x845, 0x846, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(44, 47, 213, 20, 1153490, 0x7FFF, false, false); // Garden Shed (South)
                AddButton(19, 73, 0x845, 0x846, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(44, 71, 213, 20, 1153489, 0x7FFF, false, false); // Garden Shed (East)
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