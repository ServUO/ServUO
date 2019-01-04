using System;
using System.Collections.Generic;
using System.Linq;
using Server;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public enum HarpsichordColor
    {
        DarkBlackRed = 1933,
        BlackRed = 1922,
        White = 2498,
        Pink = 2609,
        Frostwood = 1195,
        Green = 2541
    }

    public enum SoundType
    {
        AnvilStrikeInMinoc,
        ASkaranLullaby,
        BlackthornsMarch,
        DupresNightInTrinsic,
        FayaxionAndTheSix,
        FlightOfTheNexus,
        GalehavenJaunt,
        JhelomToArms,
        MidnightInYew,
        MoonglowSonata,
        NewMaginciaMarch,
        NujelmWaltz,
        SherrysSong,
        StarlightInBritain,
        TheVesperMist
    }

    public class HarpsichordDefinition
    {
        public SoundType Type { get; set; }
        public int Cliloc { get; set; }
        public int SoundID { get; set; }

        public HarpsichordDefinition(SoundType type, int cliloc, int sound)
        {
            Type = type;
            Cliloc = cliloc;
            SoundID = sound;
        }
    }

    public class HarpsichordRoll : Item
    {
        public override int LabelNumber { get { return 1098233; } } // An Harpsichord Roll

        [CommandProperty(AccessLevel.GameMaster)]
        private SoundType Type { get; set; }

        [Constructable]
        public HarpsichordRoll()
            : this((SoundType)Utility.Random(Enum.GetNames(typeof(SoundType)).Length))
        {
        }

        [Constructable]
        public HarpsichordRoll(SoundType type)
            : base(0x4BA1)
        {
            Type = type;            
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

            list.Add(HarpsichordAddon.Definitions.ToList().Find(x => x.Type == Type).Cliloc);
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

                    if (addon.List.Any(x => x.Type == Item.Type))                       
                    {
                        from.SendLocalizedMessage(1153059); // The Harpsichord already has this song.
                    }
                    else
                    {
                        addon.List.Add(HarpsichordAddon.Definitions.ToList().Find(x => x.Type == Item.Type));

                        from.SendLocalizedMessage(1153058); // You carefully feed the roll into the Harpsichord.
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
        public override bool ForceShowProperties { get { return true; } }

        public List<HarpsichordDefinition> List;

        public override BaseAddonDeed Deed { get { return new HarpsichordAddonDeed(List); } }

        [Constructable]
        public HarpsichordAddon(DirectionType type, List<HarpsichordDefinition> list)
        {
            if (list == null)
            {
                List = new List<HarpsichordDefinition>();
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
        }

        public static HarpsichordDefinition[] Definitions = new HarpsichordDefinition[]
        {
            new HarpsichordDefinition(SoundType.AnvilStrikeInMinoc, 1152998, 0),
            new HarpsichordDefinition(SoundType.ASkaranLullaby, 1152999, 0),
            new HarpsichordDefinition(SoundType.BlackthornsMarch, 1153000, 0),
            new HarpsichordDefinition(SoundType.DupresNightInTrinsic, 1153001, 0),
            new HarpsichordDefinition(SoundType.FayaxionAndTheSix, 1153002, 0),
            new HarpsichordDefinition(SoundType.FlightOfTheNexus, 1153003, 0),
            new HarpsichordDefinition(SoundType.GalehavenJaunt, 1153004, 0),
            new HarpsichordDefinition(SoundType.JhelomToArms, 1153005, 0),
            new HarpsichordDefinition(SoundType.MidnightInYew, 1153006, 0),
            new HarpsichordDefinition(SoundType.MoonglowSonata, 1153007, 0),
            new HarpsichordDefinition(SoundType.NewMaginciaMarch, 1153008, 0),
            new HarpsichordDefinition(SoundType.NujelmWaltz, 1153009, 0),
            new HarpsichordDefinition(SoundType.SherrysSong, 1153010, 0),
            new HarpsichordDefinition(SoundType.StarlightInBritain, 1153011, 0),
            new HarpsichordDefinition(SoundType.TheVesperMist, 1153012, 0),
            
        };

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && (house.IsOwner(from) || (house.LockDowns.ContainsKey(this) && house.LockDowns[this] == from)))
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

            List.ForEach(s =>
            {
                writer.Write((int)s.Type);
                writer.Write((int)s.Cliloc);
                writer.Write((int)s.SoundID);
            });
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int count = reader.ReadInt();

            List = new List<HarpsichordDefinition>();

            for (int i = count; i > 0; i--)
            {
                SoundType type = (SoundType)reader.ReadInt();
                int cliloc = reader.ReadInt();
                int sound = reader.ReadInt();

                List.Add(new HarpsichordDefinition(type, cliloc, sound));
            }
        }

        private class HarpsichordSongGump : Gump
        {
            public List<HarpsichordDefinition> m_List;

            public HarpsichordSongGump(List<HarpsichordDefinition> list)
                : base(100, 100)
            {
                m_List = list;

                AddPage(0);

                AddBackground(0, 0, 270, 370, 0x2454);
                AddHtmlLocalized(10, 10, 250, 18, 1114513, "#1152961", 0x4000, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>

                for (int i = 0; i < m_List.Count; ++i)
                {
                    AddButton(10, 37 + (i * 20), 0xFA5, 0xFA6, 100, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(50, 37 + (i * 20), 150, 20, m_List[i].Cliloc, 0x90D, false, false);
                }

                AddButton(10, 340, 0xFAB, 0xFAC, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 340, 100, 20, 1075207, 0x10, false, false); // Stop Song

                AddButton(230, 340, 0xFB4, 0xFB5, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(120, 340, 100, 20, 1114514, "#1150300 ", 0x3E00, false, false); // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
            }
        }
    }
    
    public class HarpsichordAddonDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber { get { return 1152937; } } // Harpsichord Deed

        private DirectionType _Direction;
        public List<HarpsichordDefinition> _List;

        public override BaseAddon Addon { get { return new HarpsichordAddon(_Direction, _List); } }

        [Constructable]
        public HarpsichordAddonDeed()
            : this(null)
        {
        }

        [Constructable]
        public HarpsichordAddonDeed(List<HarpsichordDefinition> list)
        {
            _List = list;
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

            writer.Write(_List.Count);

            _List.ForEach(s =>
            {
                writer.Write((int)s.Type);
                writer.Write((int)s.Cliloc);
                writer.Write((int)s.SoundID);
            });
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int count = reader.ReadInt();

            _List = new List<HarpsichordDefinition>();

            for (int i = count; i > 0; i--)
            {
                SoundType type = (SoundType)reader.ReadInt();
                int cliloc = reader.ReadInt();
                int sound = reader.ReadInt();

                _List.Add(new HarpsichordDefinition(type, cliloc, sound));
            }
        }
    }
}
