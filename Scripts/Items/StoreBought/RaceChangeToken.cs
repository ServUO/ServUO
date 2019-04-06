using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Engines.Quests;

namespace Server.Items
{
    public class RaceChangeToken : Item, IPromotionalToken
    {
        public override int LabelNumber { get { return 1070997; } } // a promotional token
        public TextDefinition ItemName { get { return 1113656; } } // race change

        public Type GumpType { get { return typeof(RaceChangeConfirmGump); } }
        [Constructable]
        public RaceChangeToken()
            : base(0x2AAA)
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
            else if (from is PlayerMobile)
            {
                from.CloseGump(typeof(RaceChangeConfirmGump));
                BaseGump.SendGump(new RaceChangeConfirmGump((PlayerMobile)from, this));
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1070998, ItemName.ToString()); // Use this to redeem<br>Your ~1_PROMO~ : race change
        }

        public RaceChangeToken(Serial serial)
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
            int version = reader.ReadInt();
        }

        public static Dictionary<Mobile, Tuple<RaceChangeToken, Race>> Pending { get; private set; }

        public static bool AddPending(Mobile m, Race race, RaceChangeToken token)
        {
            if (token.IsChildOf(m.Backpack))
            {
                if (Pending == null)
                    Pending = new Dictionary<Mobile, Tuple<RaceChangeToken, Race>>();

                Pending[m] = new Tuple<RaceChangeToken, Race>(token, race);

                return true;
            }
            else
            {
                m.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }

            return false;
        }

        public static void RemovePending(Mobile m)
        {
            if (Pending != null && Pending.ContainsKey(m))
            {
                Pending.Remove(m);

                if (Pending.Count == 0)
                    Pending = null;
            }
        }

        public static bool IsPending(Mobile m)
        {
            return Pending != null && Pending.ContainsKey(m);
        }

        public static Race GetPendingRace(Mobile m)
        {
            if (Pending != null && Pending.ContainsKey(m))
            {
                var tuple = Pending[m];

                if (!tuple.Item1.IsChildOf(m.Backpack))
                {
                    m.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
                }
                else
                {
                    return Pending[m].Item2;
                }
            }

            return null;
        }

        public static void OnRaceChange(Mobile m)
        {
            if (Pending != null && Pending.ContainsKey(m))
            {
                var tuple = Pending[m];

                if (tuple.Item1 != null && !tuple.Item1.Deleted)
                {
                    tuple.Item1.Delete();
                }
            }
        }
    }

    public class RaceChangeConfirmGump : BaseGump
    {
        public enum GumpMode
        {
            Confirm,
            Select
        }

        public RaceChangeToken Token { get; private set; }
        public GumpMode Mode { get; private set; }

        public RaceChangeConfirmGump(PlayerMobile pm, RaceChangeToken token, GumpMode mode = GumpMode.Confirm)
            : base(pm, 100, 100)
        {
            Token = token;
            Mode = mode;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 291, 159, 9200);
            AddImageTiled(5, 5, 281, 20, 2702);
            AddImageTiled(5, 30, 281, 100, 2702);

            AddHtmlLocalized(8, 5, 279, 20, 1113641, 0x7FFF, false, false); // Change your character's race. 

            AddButton(5, 132, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 132, 100, 20, 1011012, 0x7FFF, false, false); // CANCEL

            if (Mode == GumpMode.Confirm)
            {
                AddHtmlLocalized(8, 30, 279, 124, 1113642, 0x7FFF, false, false); // Click OK to change your character's race. This change is permanent.<BR><BR>Are you sure you wish to change your character's race?

                AddButton(126, 132, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(160, 132, 120, 20, 1113643, 0x7FFF, false, false); // CHANGE RACE
            }
            else
            {
                AddHtmlLocalized(8, 30, 279, 40, 1113657, 0x7FFF, false, false);

                int y = 80;

                if (User.Race != Race.Human)
                {
                    AddButton(8, y, 0xFB7, 0xFB9, 2, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(100, y, 150, 20, 1111906, 0x7FFF, false, false); // Make me a human!
                    y += 25;
                }

                if (User.Race != Race.Elf)
                {
                    AddButton(8, y, 0xFB7, 0xFB9, 3, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(100, y, 150, 20, 1111910, 0x7FFF, false, false); // Make me an elf!
                    y += 25;
                }

                if (User.Race != Race.Gargoyle)
                {
                    AddButton(8, y, 0xFB7, 0xFB9, 4, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(100, y, 150, 20, 1111900, 0x7FFF, false, false); // Make me a gargoyle!
                }
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            int id = info.ButtonID;

            switch (id)
            {
                case 0: 
                    break;
                case 1:
                    Mode = GumpMode.Select;
                    Refresh();
                    break;
                default:
                    if (User.NetState != null && HeritageQuester.Check(User) && RaceChangeToken.AddPending(User, Race.Races[id - 2], Token))
                    {
                        User.NetState.Send(new HeritagePacket(User.Female, (short)(id - 1)));
                    }
                    break;
            }
        }
    }
}
