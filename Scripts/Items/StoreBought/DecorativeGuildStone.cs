using System.Collections.Generic;
using Server.ContextMenus;
using Server.Guilds;
using Server.Gumps;
using Server.Misc;
using Server.Multis;

namespace Server.Items
{
    public class DecorativeGuildstone : Item, ISecurable
    {
        public override int LabelNumber => 1159490;  // Decorative Guildstone

        public Guild Guild { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        public DecorativeGuildstone(int itemid)
            : base(itemid)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public DecorativeGuildstone(Serial serial)
            : base(serial)
        {
        }

        public bool HasAccces(Mobile m)
        {
            if (m.AccessLevel >= AccessLevel.GameMaster)
                return true;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            return (house != null && house.HasAccess(m));
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (!IsLockedDown && !IsSecure)
            {
                from.SendLocalizedMessage(1112573); // This must be locked down or secured in order to use it.
            }
            else if (house != null && !house.IsCoOwner(from))
            {
                from.SendLocalizedMessage(500447); // That is not accessible.
            }
            else
            {
                list.Add(new UpdateGuildInfoEntry(from, this));
            }
        }

        private class UpdateGuildInfoEntry : ContextMenuEntry
        {
            private readonly DecorativeGuildstone _GuildStone;
            private readonly Mobile _Mobile;

            public UpdateGuildInfoEntry(Mobile from, DecorativeGuildstone gs)
                : base(1159486, 12) // Update Guild Information
            {
                _Mobile = from;
                _GuildStone = gs;
            }

            public override void OnClick()
            {
                if (_Mobile.Guild != null)
                {
                    _GuildStone.Guild = _Mobile.Guild as Guild;
                    _Mobile.SendLocalizedMessage(1159488); // The stone has been updated.
                    _GuildStone.InvalidateProperties();
                }
                else
                {
                    _Mobile.SendLocalizedMessage(1159487); // You must be in a guild to do this.
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Guild == null)
            {
                from.SendLocalizedMessage(1159489); // The guild this stone is linked to is no longer valid.
            }
            else if (Guild.Leader == null)
            {
                from.SendLocalizedMessage(1159489); // The guild this stone is linked to is no longer valid.
                Guild = null;
                InvalidateProperties();
            }
            else
            {
                from.CloseGump(typeof(GuildInformationGump));
                from.SendGump(new GuildInformationGump(this, Guild));
            }
        }

        public override void OnLockDownChange()
        {
            if (!IsLockedDown)
            {
                Guild = null;
                InvalidateProperties();
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Guild != null)
            {
                list.Add(string.Format("{0}", Guild.Name));
                list.Add(string.Format("{0}", Guild.Abbreviation));
            }

            list.Add(1155536); // Shard Bound
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)Level);
            writer.Write(Guild);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            Level = (SecureLevel)reader.ReadInt();
            Guild = reader.ReadGuild() as Guild;
        }

        public class GuildInformationGump : Gump
        {
            public GuildInformationGump(Item i, Guild g)
                : base(100, 100)
            {
                AddPage(0);

                AddBackground(0, 0, 520, 245, 0x6DB);
                AddHtmlLocalized(10, 10, 500, 18, 1114513, g.Name, 0x7FFF, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddHtmlLocalized(10, 30, 500, 18, 1114513, g.Abbreviation, 0x7FFF, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddHtmlLocalized(10, 50, 500, 18, 1114513, string.Format("A Guild Created On {0}", ServerList.ServerName), 0x7FFF, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
                AddHtmlLocalized(10, 150, 500, 18, 1114513, g.Leader != null ? g.Leader.Name : "Unknown", 0x1745, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>

                int[] itemIDs = null;

                object[] objs = i.GetType().GetCustomAttributes(typeof(FlipableAttribute), true);

                if (objs != null && objs.Length > 0)
                {
                    var fp = objs[0] as FlipableAttribute;

                    if (fp != null)
                    {
                        itemIDs = fp.ItemIDs;
                    }
                }

                AddItem(30, 150, itemIDs != null ? itemIDs[1] : i.ItemID);
                AddItem(450, 150, i.ItemID);
                AddImage(5, 0, 0x15A0);
                AddImage(425, 0, 0x15A0);
            }
        }
    }

    [Flipable(0xED4, 0xED5)]
    public class LegacyGuildstone : DecorativeGuildstone
    {
        [Constructable]
        public LegacyGuildstone()
            : base(0xED4)
        {
        }

        public LegacyGuildstone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    [Flipable(0xA581, 0xA582)]
    public class KnightChessPieceGuildstone : DecorativeGuildstone
    {
        [Constructable]
        public KnightChessPieceGuildstone()
            : base(0xA581)
        {
        }

        public KnightChessPieceGuildstone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class RookChessPieceGuildstone : DecorativeGuildstone
    {
        [Constructable]
        public RookChessPieceGuildstone()
            : base(0xA583)
        {
        }

        public RookChessPieceGuildstone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
