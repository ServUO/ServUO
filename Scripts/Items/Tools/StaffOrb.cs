using Server.ContextMenus;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    /// <summary>
    /// David O'Hara
    /// 08-13-2004
    /// Version 3.0
    /// This orb allows staff to switch between a Player access level and their current staff level. 
    /// It also sets the mortality as appropriate for staff.
    /// A home location can be set/used thru the context menu.
    /// Will auto resurrect it's owner on death.
    /// </summary>
    public class StaffOrb : Item
    {
        public Point3D m_HomeLocation;
        public Map m_HomeMap;
        private Mobile m_Owner;
        private AccessLevel m_StaffLevel;
        private bool m_AutoRes = true;
        [Constructable]
        public StaffOrb()
            : base(0x0E2F)
        {
            LootType = LootType.Blessed;
            Weight = 0;
            Name = "Unassigned Staff Orb";
        }

        [Constructable]
        public StaffOrb(Mobile player)
            : base(0x0E2F)
        {
            LootType = LootType.Blessed;
            Weight = 0;
            m_Owner = player;
            Name = player.Name + "'s Staff Orb";
        }

        public StaffOrb(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Point3D HomeLocation
        {
            get
            {
                return m_HomeLocation;
            }
            set
            {
                m_HomeLocation = value;
            }
        }
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Map HomeMap
        {
            get
            {
                return m_HomeMap;
            }
            set
            {
                m_HomeMap = value;
            }
        }
        [CommandProperty(AccessLevel.Counselor)]
        public bool AutoRes
        {
            get
            {
                return m_AutoRes;
            }
            set
            {
                m_AutoRes = value;
            }
        }
        public static void GetContextMenuEntries(Mobile from, Item item, List<ContextMenuEntry> list)
        {
            list.Add(new GoHomeEntry(from, item));
            list.Add(new SetHomeEntry(from, item));
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (m_Owner == null)
            {
                return;
            }
            else
            {
                if (m_Owner != from)
                {
                    from.SendMessage("This is not yours to use.");
                    return;
                }
                else
                {
                    base.GetContextMenuEntries(from, list);
                    GetContextMenuEntries(from, this, list);
                }
            }
        }

        public override DeathMoveResult OnInventoryDeath(Mobile parent)
        {
            if (m_AutoRes && parent == m_Owner)
            {
                SwitchAccessLevels(parent);
                new AutoResTimer(parent).Start();
            }
            return base.OnInventoryDeath(parent);
        }


        public override void OnDoubleClickDead(Mobile from)
        {
            if (m_Owner == from)
            {
                OnDoubleClick(from);
            }
            else
            {
                base.OnDoubleClickDead(from);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            // set owner if not already set -- this is only done the first time.
            if (m_Owner == null)
            {
                m_Owner = from;
                Name = m_Owner.Name.ToString() + "'s Staff Orb";
                HomeLocation = from.Location;
                HomeMap = from.Map;
                from.SendMessage("This orb has been assigned to you.");
            }
            else
            {
                if (m_Owner != from)
                {
                    from.SendMessage("This is not your's to use.");
                    return;
                }
                else
                {
                    SwitchAccessLevels(from);
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(3); // version

            // version 3
            writer.Write(m_AutoRes);

            // version 2
            writer.Write(m_HomeLocation);
            writer.Write(m_HomeMap);

            writer.Write(m_Owner);
            writer.WriteEncodedInt((int)m_StaffLevel);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 3:
                    {
                        m_AutoRes = reader.ReadBool();
                        goto case 2;
                    }
                case 2:
                    {
                        m_HomeLocation = reader.ReadPoint3D();
                        m_HomeMap = reader.ReadMap();
                        goto case 1;
                    }
                case 1:
                    {
                        m_Owner = reader.ReadMobile();
                        m_StaffLevel = (AccessLevel)reader.ReadEncodedInt();
                        break;
                    }
            }
        }

        private void SwitchAccessLevels(Mobile from)
        {
            // check current access level
            if (from.IsPlayer())
            {
                // return to staff status
                from.AccessLevel = m_StaffLevel;
                from.Blessed = true;
            }
            else
            {
                m_StaffLevel = from.AccessLevel;
                from.AccessLevel = AccessLevel.Player;
                from.Blessed = false;
            }
        }

        private class GoHomeEntry : ContextMenuEntry
        {
            private readonly StaffOrb m_Item;
            private readonly Mobile m_Mobile;
            public GoHomeEntry(Mobile from, Item item)
                : base(5134)// uses "Goto Loc" entry
            {
                m_Item = (StaffOrb)item;
                m_Mobile = from;
            }

            public override void OnClick()
            {
                // go to home location
                m_Mobile.Location = m_Item.HomeLocation;
                if (m_Item.HomeMap != null)
                    m_Mobile.Map = m_Item.HomeMap;
            }
        }

        private class SetHomeEntry : ContextMenuEntry
        {
            private readonly StaffOrb m_Item;
            private readonly Mobile m_Mobile;
            public SetHomeEntry(Mobile from, Item item)
                : base(2055)// uses "Mark" entry
            {
                m_Item = (StaffOrb)item;
                m_Mobile = from;
            }

            public override void OnClick()
            {
                // set home location
                m_Item.HomeLocation = m_Mobile.Location;
                m_Item.HomeMap = m_Mobile.Map;
                m_Mobile.SendMessage("The home location on your orb has been set to your current position.");
            }
        }

        private class AutoResTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public AutoResTimer(Mobile mob)
                : base(TimeSpan.FromSeconds(5.0))
            {
                m_Mobile = mob;
            }

            protected override void OnTick()
            {
                m_Mobile.Resurrect();
                m_Mobile.SendMessage("...How in the hell did you manage to die? You're a staff member!");
                Stop();
            }
        }
    }
}
