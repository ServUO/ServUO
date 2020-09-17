using Server.ContextMenus;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class AnkhOfSacrificeComponent : AddonComponent
    {
        public AnkhOfSacrificeComponent(int itemID)
            : base(itemID)
        {
        }

        public AnkhOfSacrificeComponent(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

        public override int LabelNumber => 1027772;// Ankh of Sacrifice
        public static void Resurrect(PlayerMobile m, AnkhOfSacrificeAddon ankh)
        {
            if (m == null)
            {
            }
            else if (!m.InRange(ankh.GetWorldLocation(), 2))
            {
                m.SendLocalizedMessage(500446); // That is too far away.
            }
            else if (m.Alive)
            {
                m.SendLocalizedMessage(1060197); // You are not dead, and thus cannot be resurrected!
            }
            else if (m.AnkhNextUse > DateTime.UtcNow)
            {
                TimeSpan delay = m.AnkhNextUse - DateTime.UtcNow;

                if (delay.TotalMinutes > 0)
                    m.SendLocalizedMessage(1079265, Math.Round(delay.TotalMinutes).ToString()); // You must wait ~1_minutes~ minutes before you can use this item.
                else
                    m.SendLocalizedMessage(1079263, Math.Round(delay.TotalSeconds).ToString()); // You must wait ~1_seconds~ seconds before you can use this item.		
            }
            else
            {
                m.CloseGump(typeof(AnkhResurrectGump));
                m.SendGump(new AnkhResurrectGump(m, ResurrectMessage.VirtueShrine));
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from is PlayerMobile)
                list.Add(new LockKarmaEntry((PlayerMobile)from, Addon as AnkhOfSacrificeAddon));

            list.Add(new ResurrectEntry(from, Addon as AnkhOfSacrificeAddon));
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

        private class ResurrectEntry : ContextMenuEntry
        {
            private readonly Mobile m_Mobile;
            private readonly AnkhOfSacrificeAddon m_Ankh;
            public ResurrectEntry(Mobile mobile, AnkhOfSacrificeAddon ankh)
                : base(6195, 2)
            {
                m_Mobile = mobile;
                m_Ankh = ankh;
            }

            public override void OnClick()
            {
                if (m_Ankh == null || m_Ankh.Deleted)
                    return;

                Resurrect(m_Mobile as PlayerMobile, m_Ankh);
            }
        }

        private class LockKarmaEntry : ContextMenuEntry
        {
            private readonly PlayerMobile m_Mobile;
            private readonly AnkhOfSacrificeAddon m_Ankh;
            public LockKarmaEntry(PlayerMobile mobile, AnkhOfSacrificeAddon ankh)
                : base(mobile.KarmaLocked ? 6197 : 6196, 2)
            {
                m_Mobile = mobile;
                m_Ankh = ankh;
            }

            public override void OnClick()
            {
                if (!m_Mobile.InRange(m_Ankh.GetWorldLocation(), 2))
                    m_Mobile.SendLocalizedMessage(500446); // That is too far away.
                else
                {
                    m_Mobile.KarmaLocked = !m_Mobile.KarmaLocked;

                    if (m_Mobile.KarmaLocked)
                        m_Mobile.SendLocalizedMessage(1060192); // Your karma has been locked. Your karma can no longer be raised.
                    else
                        m_Mobile.SendLocalizedMessage(1060191); // Your karma has been unlocked. Your karma can be raised again.
                }
            }
        }

        private class AnkhResurrectGump : ResurrectGump
        {
            public AnkhResurrectGump(Mobile owner, ResurrectMessage msg)
                : base(owner, owner, msg, false)
            {
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                Mobile from = state.Mobile;

                if (info.ButtonID == 1 || info.ButtonID == 2)
                {
                    if (from.Map == null || !from.Map.CanFit(from.Location, 16, false, false))
                    {
                        from.SendLocalizedMessage(502391); // Thou can not be resurrected there!
                        return;
                    }

                    if (from is PlayerMobile)
                    {
                        ((PlayerMobile)from).AnkhNextUse = DateTime.UtcNow + TimeSpan.FromHours(1);
                    }

                    base.OnResponse(state, info);
                }
            }
        }
    }

    public class AnkhOfSacrificeAddon : BaseAddon, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public AnkhOfSacrificeAddon(bool east)
            : base()
        {
            if (east)
            {
                AddComponent(new AnkhOfSacrificeComponent(0x1D98), 0, 0, 0);
                AddComponent(new AnkhOfSacrificeComponent(0x1D97), 0, 1, 0);
                AddComponent(new AnkhOfSacrificeComponent(0x1CD6), 1, 0, 0);
                AddComponent(new AnkhOfSacrificeComponent(0x1CD4), 1, 1, 0);
                AddComponent(new AnkhOfSacrificeComponent(0x1CD0), 2, 0, 0);
                AddComponent(new AnkhOfSacrificeComponent(0x1CCE), 2, 1, 0);
            }
            else
            {
                AddComponent(new AnkhOfSacrificeComponent(0x1E5D), 0, 0, 0);
                AddComponent(new AnkhOfSacrificeComponent(0x1E5C), 1, 0, 0);
                AddComponent(new AnkhOfSacrificeComponent(0x1CD2), 0, 1, 0);
                AddComponent(new AnkhOfSacrificeComponent(0x1CD8), 1, 1, 0);
                AddComponent(new AnkhOfSacrificeComponent(0x1CCD), 0, 2, 0);
                AddComponent(new AnkhOfSacrificeComponent(0x1CCE), 1, 2, 0);
            }
        }

        public AnkhOfSacrificeAddon(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnMovement => true;
        public override BaseAddonDeed Deed
        {
            get
            {
                AnkhOfSacrificeDeed deed = new AnkhOfSacrificeDeed
                {
                    IsRewardItem = m_IsRewardItem
                };

                return deed;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (!m.Alive && Utility.InRange(Location, m.Location, 1) && !Utility.InRange(Location, oldLocation, 1))
                AnkhOfSacrificeComponent.Resurrect(m as PlayerMobile, this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_IsRewardItem = reader.ReadBool();
        }
    }

    public class AnkhOfSacrificeDeed : BaseAddonDeed, IRewardItem, IRewardOption
    {
        private bool m_East;
        private bool m_IsRewardItem;
        [Constructable]
        public AnkhOfSacrificeDeed()
            : this(false)
        {
        }

        [Constructable]
        public AnkhOfSacrificeDeed(bool isRewardItem)
            : base()
        {
            LootType = LootType.Blessed;

            m_IsRewardItem = isRewardItem;
        }

        public AnkhOfSacrificeDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1080397;// Deed For An Ankh Of Sacrifice
        public override BaseAddon Addon
        {
            get
            {
                AnkhOfSacrificeAddon addon = new AnkhOfSacrificeAddon(m_East)
                {
                    IsRewardItem = m_IsRewardItem
                };

                return addon;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.   
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1080457); // 10th Year Veteran Reward
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_IsRewardItem = reader.ReadBool();
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(1, 1080398); // Ankh of Sacrifice South
            list.Add(2, 1080399); // Ankh of Sacrifice East
        }

        public void OnOptionSelected(Mobile from, int option)
        {
            switch (option)
            {
                case 1:
                    m_East = false;
                    break;
                case 2:
                    m_East = true;
                    break;
            }

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}
