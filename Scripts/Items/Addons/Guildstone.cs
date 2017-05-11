using System;
using Server.Factions;
using Server.Guilds;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class Guildstone : Item, IAddon, IChopable
    {
        private Guild m_Guild;
        private string m_GuildName;
        private string m_GuildAbbrev;

        [CommandProperty(AccessLevel.GameMaster)]
        public string GuildName
        {
            get
            {
                return this.m_GuildName;
            }
            set
            {
                this.m_GuildName = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string GuildAbbrev
        {
            get
            {
                return this.m_GuildAbbrev;
            }
            set
            {
                this.m_GuildAbbrev = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Guild Guild
        {
            get
            {
                return this.m_Guild;
            }
        }

        public override int LabelNumber
        {
            get
            {
                return 1041429;
            }
        }// a guildstone

        public Guildstone(Guild g)
            : this(g, g.Name, g.Abbreviation)
        {
        }

        public Guildstone(Guild g, string guildName, string abbrev)
            : base(Guild.NewGuildSystem ? 0xED6 : 0xED4)
        {
            this.m_Guild = g;
            this.m_GuildName = guildName;
            this.m_GuildAbbrev = abbrev;

            this.Movable = false;
        }

        public Guildstone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            if (this.m_Guild != null && !this.m_Guild.Disbanded)
            {
                this.m_GuildName = this.m_Guild.Name;
                this.m_GuildAbbrev = this.m_Guild.Abbreviation;
            }

            writer.Write((int)3); // version

            writer.Write(this.m_BeforeChangeover);

            writer.Write(this.m_GuildName);
            writer.Write(this.m_GuildAbbrev);

            writer.Write(this.m_Guild);
        }

        private bool m_BeforeChangeover;
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch( version )
            {
                case 3:
                    {
                        this.m_BeforeChangeover = reader.ReadBool();
                        goto case 2;
                    }
                case 2:
                    {
                        this.m_GuildName = reader.ReadString();
                        this.m_GuildAbbrev = reader.ReadString();

                        goto case 1;
                    }
                case 1:
                    {
                        this.m_Guild = reader.ReadGuild() as Guild;

                        goto case 0;
                    }
                case 0:
                    {
                        break;
                    }
            }

            if (Guild.NewGuildSystem && this.ItemID == 0xED4)
                this.ItemID = 0xED6;

            if (version <= 2)
                this.m_BeforeChangeover = true;

            if (Guild.NewGuildSystem && this.m_BeforeChangeover)
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(AddToHouse));

            if (!Guild.NewGuildSystem && this.m_Guild == null)
                this.Delete();
        }

        private void AddToHouse()
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (Guild.NewGuildSystem && this.m_BeforeChangeover && house != null && !house.Addons.ContainsKey(this))
            {
                house.Addons[this] = house.Owner;
                this.m_BeforeChangeover = false;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Guild != null && !this.m_Guild.Disbanded)
            {
                string name;
                string abbr;

                if ((name = this.m_Guild.Name) == null || (name = name.Trim()).Length <= 0)
                    name = "(unnamed)";

                if ((abbr = this.m_Guild.Abbreviation) == null || (abbr = abbr.Trim()).Length <= 0)
                    abbr = "";

                //list.Add( 1060802, Utility.FixHtml( name ) ); // Guild name: ~1_val~
                list.Add(1060802, String.Format("{0} [{1}]", Utility.FixHtml(name), Utility.FixHtml(abbr)));
            }
            else if (this.m_GuildName != null && this.m_GuildAbbrev != null)
            {
                list.Add(1060802, String.Format("{0} [{1}]", Utility.FixHtml(this.m_GuildName), Utility.FixHtml(this.m_GuildAbbrev)));
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (this.m_Guild != null && !this.m_Guild.Disbanded)
            {
                string name;

                if ((name = this.m_Guild.Name) == null || (name = name.Trim()).Length <= 0)
                    name = "(unnamed)";

                this.LabelTo(from, name);
            }
            else if (this.m_GuildName != null)
            {
                this.LabelTo(from, this.m_GuildName);
            }
        }

        public override void OnAfterDelete()
        {
            if (!Guild.NewGuildSystem && this.m_Guild != null && !this.m_Guild.Disbanded)
                this.m_Guild.Disband();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Guild.NewGuildSystem)
                return;

            if (this.m_Guild == null || this.m_Guild.Disbanded)
            {
                this.Delete();
            }
            else if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
            else if (this.m_Guild.Accepted.Contains(from))
            {
                #region Factions
                PlayerState guildState = PlayerState.Find(this.m_Guild.Leader);
                PlayerState targetState = PlayerState.Find(from);

                Faction guildFaction = (guildState == null ? null : guildState.Faction);
                Faction targetFaction = (targetState == null ? null : targetState.Faction);

                if (guildFaction != targetFaction || (targetState != null && targetState.IsLeaving))
                    return;

                if (guildState != null && targetState != null)
                    targetState.Leaving = guildState.Leaving;
                #endregion

                this.m_Guild.Accepted.Remove(from);
                this.m_Guild.AddMember(from);

                GuildGump.EnsureClosed(from);
                from.SendGump(new GuildGump(from, this.m_Guild));
            }
            else if (from.AccessLevel < AccessLevel.GameMaster && !this.m_Guild.IsMember(from))
            {
                from.Send(new MessageLocalized(this.Serial, this.ItemID, MessageType.Regular, 0x3B2, 3, 501158, "", "")); // You are not a member ...
            }
            else
            {
                GuildGump.EnsureClosed(from);
                from.SendGump(new GuildGump(from, this.m_Guild));
            }
        }

        #region IAddon Members
        public Item Deed
        {
            get
            {
                return new GuildstoneDeed(this.m_Guild, this.m_GuildName, this.m_GuildAbbrev);
            }
        }

        public bool CouldFit(IPoint3D p, Map map)
        {
            return map.CanFit(p.X, p.Y, p.Z, this.ItemData.Height);
        }

        #endregion

        #region IChopable Members

        public void OnChop(Mobile from)
        {
            if (!Guild.NewGuildSystem)
                return;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if ((house == null && this.m_BeforeChangeover) || (house != null && house.IsOwner(from) && house.Addons.ContainsKey(this)))
            {
                Effects.PlaySound(this.GetWorldLocation(), this.Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.

                this.Delete();

                if (house != null && house.Addons.ContainsKey(this))
                    house.Addons.Remove(this);

                Item deed = this.Deed;

                if (deed != null)
                {
                    from.AddToBackpack(deed);
                }
            }
        }
        #endregion
    }

    [Flipable(0x14F0, 0x14EF)]
    public class GuildstoneDeed : Item
    {
        public override int LabelNumber
        {
            get
            {
                return 1041233;
            }
        }// deed to a guildstone

        private Guild m_Guild;
        private string m_GuildName;
        private string m_GuildAbbrev;

        [CommandProperty(AccessLevel.GameMaster)]
        public string GuildName
        {
            get
            {
                return this.m_GuildName;
            }
            set
            {
                this.m_GuildName = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string GuildAbbrev
        {
            get
            {
                return this.m_GuildAbbrev;
            }
            set
            {
                this.m_GuildAbbrev = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Guild Guild
        {
            get
            {
                return this.m_Guild;
            }
        }

        [Constructable]
        public GuildstoneDeed()
            : this(null, null)
        {
        }

        [Constructable]
        public GuildstoneDeed(string guildName, string abbrev)
            : this(null, guildName, abbrev)
        {
        }

        public GuildstoneDeed(Guild g, string guildName, string abbrev)
            : base(0x14F0)
        {
            this.m_Guild = g;
            this.m_GuildName = guildName;
            this.m_GuildAbbrev = abbrev;

            this.Weight = 1.0;
        }

        public GuildstoneDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            if (this.m_Guild != null && !this.m_Guild.Disbanded)
            {
                this.m_GuildName = this.m_Guild.Name;
                this.m_GuildAbbrev = this.m_Guild.Abbreviation;
            }

            writer.Write((int)1); // version

            writer.Write(this.m_GuildName);
            writer.Write(this.m_GuildAbbrev);

            writer.Write(this.m_Guild);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_GuildName = reader.ReadString();
                        this.m_GuildAbbrev = reader.ReadString();

                        this.m_Guild = reader.ReadGuild() as Guild;

                        break;
                    }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Guild != null && !this.m_Guild.Disbanded)
            {
                string name;
                string abbr;

                if ((name = this.m_Guild.Name) == null || (name = name.Trim()).Length <= 0)
                    name = "(unnamed)";

                if ((abbr = this.m_Guild.Abbreviation) == null || (abbr = abbr.Trim()).Length <= 0)
                    abbr = "";

                //list.Add( 1060802, Utility.FixHtml( name ) ); // Guild name: ~1_val~
                list.Add(1060802, String.Format("{0} [{1}]", Utility.FixHtml(name), Utility.FixHtml(abbr)));
            }
            else if (this.m_GuildName != null && this.m_GuildAbbrev != null)
            {
                list.Add(1060802, String.Format("{0} [{1}]", Utility.FixHtml(this.m_GuildName), Utility.FixHtml(this.m_GuildAbbrev)));
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && house.IsOwner(from))
                {
                    from.SendLocalizedMessage(1062838); // Where would you like to place this decoration?
                    from.BeginTarget(-1, true, Targeting.TargetFlags.None, new TargetStateCallback(Placement_OnTarget), null);
                }
                else
                {
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public void Placement_OnTarget(Mobile from, object targeted, object state)
        {
            IPoint3D p = targeted as IPoint3D;

            if (p == null || this.Deleted)
                return;

            Point3D loc = new Point3D(p);

            BaseHouse house = BaseHouse.FindHouseAt(loc, from.Map, 16);

            if (this.IsChildOf(from.Backpack))
            {
                if (house != null && house.IsOwner(from))
                {
                    Item addon = new Guildstone(this.m_Guild, this.m_GuildName, this.m_GuildAbbrev);

                    addon.MoveToWorld(loc, from.Map);

                    house.Addons[addon] = from;
                    this.Delete();
                }
                else
                {
                    from.SendLocalizedMessage(1042036); // That location is not in your house.
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }
    }
}