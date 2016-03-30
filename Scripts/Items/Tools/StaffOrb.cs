using System;
using System.Collections.Generic;
using CustomsFramework;
using Server.ContextMenus;

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
            this.LootType = LootType.Blessed;
            this.Weight = 0;
            this.Name = "Unassigned Staff Orb";
        }

        [Constructable]
        public StaffOrb(Mobile player)
            : base(0x0E2F)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 0;
            this.m_Owner = player;
            this.Name = player.Name + "'s Staff Orb";
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
                return this.m_HomeLocation;
            }
            set
            {
                this.m_HomeLocation = value;
            }
        }
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Map HomeMap
        {
            get
            {
                return this.m_HomeMap;
            }
            set
            {
                this.m_HomeMap = value;
            }
        }
        [CommandProperty(AccessLevel.Counselor)]
        public bool AutoRes
        {
            get
            {
                return this.m_AutoRes;
            }
            set
            {
                this.m_AutoRes = value;
            }
        }
        public static void GetContextMenuEntries(Mobile from, Item item, List<ContextMenuEntry> list)
        {
            list.Add(new GoHomeEntry(from, item));
            list.Add(new SetHomeEntry(from, item));
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (this.m_Owner == null)
            {
                return;
            }
            else
            {
                if (this.m_Owner != from)
                {
                    from.SendMessage("This is not yours to use.");
                    return;
                }
                else
                {
                    base.GetContextMenuEntries(from, list);
                    StaffOrb.GetContextMenuEntries(from, this, list);
                }
            }
        }

        public override DeathMoveResult OnInventoryDeath(Mobile parent)
        {
            if (this.m_AutoRes && parent == this.m_Owner)
            {
                this.SwitchAccessLevels(parent);
                new AutoResTimer(parent).Start();
            }
            return base.OnInventoryDeath(parent);
        }

        public override void OnDoubleClick(Mobile from)
        {
            // set owner if not already set -- this is only done the first time.
            if (this.m_Owner == null)
            {
                this.m_Owner = from;
                this.Name = this.m_Owner.Name.ToString() + "'s Staff Orb";
                this.HomeLocation = from.Location;
                this.HomeMap = from.Map;
                from.SendMessage("This orb has been assigned to you.");
            }
            else
            {
                if (this.m_Owner != from)
                {
                    from.SendMessage("This is not your's to use.");
                    return;
                }
                else
                {
                    this.SwitchAccessLevels(from);
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3); // version

            // version 3
            writer.Write(this.m_AutoRes);

            // version 2
            writer.Write(this.m_HomeLocation);
            writer.Write(this.m_HomeMap);

            writer.Write(this.m_Owner);
            writer.WriteEncodedInt((int)this.m_StaffLevel);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch ( version )
            {
                case 3:
                    {
                        this.m_AutoRes = reader.ReadBool();
                        goto case 2;
                    }
                case 2:
                    {
                        this.m_HomeLocation = reader.ReadPoint3D();
                        this.m_HomeMap = reader.ReadMap();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_Owner = reader.ReadMobile();
                        this.m_StaffLevel = (AccessLevel)reader.ReadEncodedInt();
                        break;
                    }
            }
        }

        private void SwitchAccessLevels(Mobile from)
        {
            // check current access level
            if (Utilities.IsPlayer(from))
            {
                // return to staff status
                from.AccessLevel = this.m_StaffLevel;
                from.Blessed = true;
            }
            else
            {
                this.m_StaffLevel = from.AccessLevel;
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
                this.m_Item = (StaffOrb)item;
                this.m_Mobile = from;
            }

            public override void OnClick()
            {
                // go to home location
                this.m_Mobile.Location = this.m_Item.HomeLocation;
                if (this.m_Item.HomeMap != null)
                    this.m_Mobile.Map = this.m_Item.HomeMap;
            }
        }

        private class SetHomeEntry : ContextMenuEntry
        {
            private readonly StaffOrb m_Item;
            private readonly Mobile m_Mobile;
            public SetHomeEntry(Mobile from, Item item)
                : base(2055)// uses "Mark" entry
            {
                this.m_Item = (StaffOrb)item;
                this.m_Mobile = from;
            }

            public override void OnClick()
            {
                // set home location
                this.m_Item.HomeLocation = this.m_Mobile.Location;
                this.m_Item.HomeMap = this.m_Mobile.Map;
                this.m_Mobile.SendMessage("The home location on your orb has been set to your current position.");
            }
        }

        private class AutoResTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public AutoResTimer(Mobile mob)
                : base(TimeSpan.FromSeconds(5.0))
            {
                this.m_Mobile = mob;
            }

            protected override void OnTick()
            {
                this.m_Mobile.Resurrect();
                this.m_Mobile.SendMessage("...How in the hell did you manage to die? You're a staff member!");
                this.Stop();
            }
        }
    }
}