using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Items
{
    public enum HarpsichordColor
    {
        Purple = 1168,
        Yellow = 1177,
        NavyBlue = 1195,
        Black = 1910,
        DarkRed = 1922,
        Brown = 1933,
        White = 2498,
        TimberGreen = 2584,
        Green = 2541,
        DarkPurple = 2609
    }

    public class HarpsichordRoll : Item
    {
        public override int LabelNumber => 1098233;  // An Harpsichord Roll

        private MusicName m_Music;

        [CommandProperty(AccessLevel.GameMaster)]
        private MusicName Music
        {
            get { return m_Music; }
            set
            {
                m_Music = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public HarpsichordRoll()
            : this((MusicName)Utility.Random(88, 15))
        {
        }

        [Constructable]
        public HarpsichordRoll(MusicName music)
            : base(0x4BA1)
        {
            Music = music;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1153037); // Which Harpsichord do you wish to use this on?
                from.Target = new InternalTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(1054107); // This item must be in your backpack.
            }
        }

        public HarpsichordRoll(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1152910 + (int)Music);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write((int)m_Music);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Music = (MusicName)reader.ReadInt();
        }

        public class InternalTarget : Target
        {
            private readonly HarpsichordRoll Item;

            public InternalTarget(HarpsichordRoll item)
                : base(2, false, TargetFlags.None)
            {
                Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is AddonComponent && ((AddonComponent)targeted).Addon != null && ((AddonComponent)targeted).Addon is HarpsichordAddon)
                {
                    HarpsichordAddon addon = ((AddonComponent)targeted).Addon as HarpsichordAddon;

                    if (addon.List.Contains(Item.Music))
                    {
                        from.Send(new MessageLocalized(addon.Serial, addon.ItemID, MessageType.Regular, 0x3B2, 3, 1153059, "", "")); // The Harpsichord already has this song.                        
                    }
                    else
                    {
                        addon.List.Add(Item.Music);
                        Item.Delete();
                        from.Send(new MessageLocalized(addon.Serial, addon.ItemID, MessageType.Regular, 0x3B2, 3, 1153058, "", "")); // You carefully feed the roll into the Harpsichord.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1153038); // Use this on an Harpsichord. 
                }
            }
        }
    }

    public class HarpsichordAddon : BaseAddon
    {
        public override bool ForceShowProperties => false;
        public override bool RetainDeedHue => true;

        public List<MusicName> List;

        public override BaseAddonDeed Deed => new HarpsichordAddonDeed((HarpsichordColor)Hue, List);

        [Constructable]
        public HarpsichordAddon(HarpsichordColor hue, DirectionType type, List<MusicName> list)
        {
            if (list == null)
            {
                List = new List<MusicName>();
            }
            else
            {
                List = list;
            }

            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new AddonComponent(25539), 2, 0, 0);
                    AddComponent(new AddonComponent(25540), 2, 1, 0);
                    AddComponent(new AddonComponent(25545), -1, 0, 0);
                    AddComponent(new AddonComponent(25543), 0, 1, 0);
                    AddComponent(new AddonComponent(25541), 1, 1, 0);
                    AddComponent(new AddonComponent(25542), 1, 0, 0);
                    AddComponent(new AddonComponent(25544), 0, 0, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new AddonComponent(25532), 1, 2, 0);
                    AddComponent(new AddonComponent(25533), 0, 2, 0);
                    AddComponent(new AddonComponent(25538), 0, -1, 0);
                    AddComponent(new AddonComponent(25537), 1, 0, 0);
                    AddComponent(new AddonComponent(25536), 0, 0, 0);
                    AddComponent(new AddonComponent(25535), 1, 1, 0);
                    AddComponent(new AddonComponent(25534), 0, 1, 0);
                    break;
            }

            Hue = (int)hue;
        }

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 3))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && (house.IsFriend(from) || (house.LockDowns.ContainsKey(this) && house.LockDowns[this] == from)))
            {
                from.CloseGump(typeof(HarpsichordSongGump));
                from.SendGump(new HarpsichordSongGump(List));
            }
            else
            {
                from.SendLocalizedMessage(502092); // You must be in your house to do this.
            }
        }

        public HarpsichordAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(List.Count);

            List.ForEach(x =>
            {
                writer.Write((int)x);
            });
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int count = reader.ReadInt();

            List = new List<MusicName>();

            for (int i = count; i > 0; i--)
            {
                List.Add((MusicName)reader.ReadInt());
            }
        }

        private class HarpsichordSongGump : Gump
        {
            public List<MusicName> m_List;

            public HarpsichordSongGump(List<MusicName> list)
                : base(100, 100)
            {
                m_List = list;

                AddPage(0);

                AddBackground(0, 0, 270, 370, 0x2454);
                AddHtmlLocalized(10, 10, 250, 18, 1114513, "#1152961", 0x4000, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>

                int mindex;

                for (int i = 0; i < m_List.Count; ++i)
                {
                    mindex = (int)m_List[i];
                    AddButton(10, 37 + (i * 20), 0xFA5, 0xFA6, 100 + mindex, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(50, 37 + (i * 20), 150, 20, 1152910 + mindex, 0x90D, false, false);
                }

                AddButton(10, 340, 0xFAB, 0xFAC, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 340, 100, 20, 1075207, 0x10, false, false); // Stop Song

                AddButton(230, 340, 0xFB4, 0xFB5, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(120, 340, 100, 20, 1114514, "#1150300", 0x3E00, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                int index = info.ButtonID;

                switch (index)
                {
                    case 0: { break; }
                    case 1: // Stop Song
                        {
                            sender.Mobile.Send(StopMusic.Instance);

                            break;
                        }
                    default:
                        {
                            int music = index - 100;
                            sender.Mobile.Send(new PlayMusic((MusicName)music));

                            return;
                        }
                }
            }
        }
    }

    public class HarpsichordAddonDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1152937;  // Harpsichord Deed

        private DirectionType _Direction;
        private List<MusicName> _List;

        public override BaseAddon Addon => new HarpsichordAddon((HarpsichordColor)Hue, _Direction, _List);

        [Constructable]
        public HarpsichordAddonDeed()
            : this((HarpsichordColor)Utility.RandomList(1168, 1177, 1195, 1910, 1922, 1933, 2498, 2584, 2541, 2609))
        {
        }

        [Constructable]
        public HarpsichordAddonDeed(HarpsichordColor hue)
            : this(hue, null)
        {
        }

        [Constructable]
        public HarpsichordAddonDeed(HarpsichordColor hue, List<MusicName> list)
        {
            _List = list;
            Hue = (int)hue;
            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int count = 0;

            if (_List != null)
            {
                count = _List.Count;
            }

            list.Add(1153057, count.ToString());
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1153055); // Harpsichord South
            list.Add((int)DirectionType.East, 1153056); // Harpsichord East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, LabelNumber));
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public HarpsichordAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(_List != null ? _List.Count : 0);

            if (_List != null)
            {
                _List.ForEach(x =>
                {
                    writer.Write((int)x);
                });
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int count = reader.ReadInt();

            if (count > 0)
            {
                _List = new List<MusicName>();
            }

            for (int i = count; i > 0; i--)
            {
                _List.Add((MusicName)reader.ReadInt());
            }
        }
    }
}
