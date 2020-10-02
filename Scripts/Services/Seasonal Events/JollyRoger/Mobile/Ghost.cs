using System;
using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Fellowship
{
    public enum GhostType
    {
        One = 1159350,
        Two = 1159351,
        Three = 1159352,
        Four = 1159348,
        Five = 1159349
    }

    public class Ghost : BaseQuester
    {
        public int GumpCliloc{ get; set; }

        [Constructable]
        public Ghost(GhostType type)
            : base("the Ghost")
        {
            GumpCliloc = (int)type;
        }

        public override bool DisallowAllMoves => Hidden;

        public override void InitBody()
        {
            base.InitBody();

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            Utility.AssignRandomHair(this);

            Hue = 2500;
        }

        public override void InitOutfit()
        {
            if (Female)
            {
                SetWearable(new PlainDress(), 2500);
                SetWearable(new ThighBoots(), 2500);
            }
            else
            {
                SetWearable(new Shirt(), 2500);
                SetWearable(new ShortPants(), 2500);
                SetWearable(new ThighBoots(), 2500);
            }
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 3))
            {
                if (FellowshipMedallion.IsDressed(from))
                {
                    Gump g = new Gump(100, 100);
                    g.AddBackground(0, 0, 570, 295, 0x2454);
                    g.AddImage(0, 0, 0x6D2);
                    g.AddHtmlLocalized(335, 24, 223, 261, GumpCliloc, 0xC63, false, true);

                    from.SendGump(g);
                    from.PlaySound(1664);
                }
                else
                {
                    PrivateOverheadMessage(MessageType.Regular, 0x47E, 1159380,
                        from.NetState); // * You attempt to understand the spirit but your connection to them is weak... *
                }
            }
        }

        private DateTime _NextTimeCheck;

        public override void OnThink()
        {
            base.OnThink();

            if (_NextTimeCheck > DateTime.UtcNow)
            {
                return;
            }

            Clock.GetTime(Map, Location.X, Location.Y, out int hours, out int minutes);

            if (hours >= 0 && hours < 4)
            {
                if (Hidden)
                {
                    Hidden = false;
                }
            }
            else
            {
                if (!Hidden)
                {
                    Hidden = true;
                }
            }

            _NextTimeCheck = DateTime.UtcNow + TimeSpan.FromSeconds(5);
        }

        public Ghost(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(GumpCliloc);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            GumpCliloc = reader.ReadInt();
        }
    }
}
