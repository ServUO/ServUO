using Server.Engines.HuntsmasterChallenge;
using Server.Gumps;
using System.Collections.Generic;

namespace Server.Items
{
    public class BestKillBoard : Item
    {
        public override string DefaultName => "Top 10 Kill Board";

        [Constructable]
        public BestKillBoard() : base(7775)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 3) && HuntingSystem.Instance != null && HuntingSystem.Instance.Active)
            {
                from.CloseGump(typeof(BestKillGump));
                from.SendGump(new BestKillGump());
            }
        }

        public BestKillBoard(Serial serial) : base(serial)
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
            int v = reader.ReadInt();
        }

        private class BestKillGump : Gump
        {
            private int m_Filter;

            public BestKillGump() : this(-1) { }

            public BestKillGump(int filter) : base(20, 20)
            {
                if (HuntingSystem.Instance == null)
                    return;

                m_Filter = filter;

                if (m_Filter < -1) m_Filter = 23;
                if (m_Filter > 23) m_Filter = -1;

                List<HuntingKillEntry> useList = new List<HuntingKillEntry>();

                if (m_Filter == -1)
                {
                    foreach (KeyValuePair<HuntType, List<HuntingKillEntry>> kvp in HuntingSystem.Instance.Top10)
                    {
                        if (kvp.Value.Count > 0)
                            useList.AddRange(kvp.Value);
                    }
                }
                else if (HuntingSystem.Instance.Top10.ContainsKey((HuntType)m_Filter))
                {
                    useList = HuntingSystem.Instance.Top10[(HuntType)m_Filter];
                }

                AddBackground(0, 0, 500, 400, 9250);

                AddHtml(0, 15, 500, 16, "<Center>Top 10 Kills</Center>", false, false);
                AddHtml(20, 40, 150, 16, "<Basefont Color=#A52A2A>Hunter</Basefont>", false, false);
                AddHtml(170, 40, 120, 16, "<Basefont Color=#A52A2A>Species</Basefont>", false, false);
                AddHtml(290, 40, 150, 16, "<Basefont Color=#A52A2A>Measurement</Basefont>", false, false);
                AddHtml(390, 40, 110, 16, "<Basefont Color=#A52A2A>Date Killed</Basefont>", false, false);

                useList.Sort();
                int y = 80;

                for (int i = 0; i < useList.Count && i < 10; i++)
                {
                    HuntingKillEntry entry = useList[i];
                    HuntingTrophyInfo info = HuntingTrophyInfo.Infos[entry.KillIndex];

                    AddHtml(20, y, 150, 16, entry.Owner != null ? FormatFont(entry.Owner.Name, i) : FormatFont("Unknown", i), false, false);
                    AddHtml(170, y, 120, 16, FormatFont(GetHuntTypeString(info.HuntType), i), false, false);
                    AddHtml(290, y, 100, 16, info.MeasuredBy == MeasuredBy.Weight ? FormatFont(entry.Measurement.ToString() + " stones", i) : FormatFont(entry.Measurement.ToString() + " feet", i), false, false);
                    AddHtml(390, y, 150, 16, FormatFont(entry.DateKilled.ToShortDateString(), i), false, false);

                    y += 20;
                }

                AddHtml(0, 365, 500, 16, string.Format("<Center>{0}</Center>", GetHuntTypeString()), false, false);

                AddButton(150, 365, 4014, 4016, 1, GumpButtonType.Reply, 0);
                AddButton(328, 365, 4005, 4007, 2, GumpButtonType.Reply, 0);
            }

            private string FormatFont(string str, int index)
            {
                int hue = 080000 + (100000 * index);

                return string.Format("<BaseFont Color=#{0}>{1}</basefont>", hue.ToString(), str);
            }

            public override void OnResponse(Network.NetState state, RelayInfo info)
            {
                Mobile from = state.Mobile;

                if (info.ButtonID == 1)
                {
                    m_Filter--;
                    from.SendGump(new BestKillGump(m_Filter));
                }
                else if (info.ButtonID == 2)
                {
                    m_Filter++;
                    from.SendGump(new BestKillGump(m_Filter));
                }
            }

            private string GetHuntTypeString(HuntType type)
            {
                switch (type)
                {
                    default:
                    case HuntType.GrizzlyBear: return "Grizzly Bear";
                    case HuntType.GrayWolf: return "Grey Wolf";
                    case HuntType.Cougar: return "Cougar";
                    case HuntType.Turkey: return "Turkey";
                    case HuntType.Bull: return "Bull";
                    case HuntType.Boar: return "Boar";
                    case HuntType.Walrus: return "Walrus";
                    case HuntType.Alligator: return "Alligator";
                    case HuntType.Eagle: return "Eagle";
                    //Publish 91 added:
                    case HuntType.MyrmidexLarvae: return "Myrmidex Larvae";
                    case HuntType.Najasaurus: return "Najasaurus";
                    case HuntType.Anchisaur: return "Anchisaur";
                    case HuntType.Allosaurus: return "Allosaurus";
                    case HuntType.Dimetrosaur: return "Dimetrosaur";
                    case HuntType.Saurosaurus: return "Saurosaurus";
                    //Publish 95 added:
                    case HuntType.Tiger: return "Tiger";
                    case HuntType.MyrmidexDrone: return "Myrmidex Drone";
                    case HuntType.Triceratops: return "Triceratops";
                    case HuntType.Lion: return "Lion";
                    case HuntType.WhiteTiger: return "White Tiger";
                    case HuntType.BlackTiger: return "Black Tiger";
                    //Publish 102 added:
                    case HuntType.Raptor: return "Raptor";
                    case HuntType.SeaSerpent: return "Sea Serpent";
                    case HuntType.Scorpion: return "Scorpion";
                }
            }

            private string GetHuntTypeString()
            {
                switch (m_Filter)
                {
                    default: return "No Filter";
                    case (int)HuntType.GrizzlyBear: return "Grizzly Bear";
                    case (int)HuntType.GrayWolf: return "Grey Wolf";
                    case (int)HuntType.Cougar: return "Cougar";
                    case (int)HuntType.Turkey: return "Turkey";
                    case (int)HuntType.Bull: return "Bull";
                    case (int)HuntType.Boar: return "Boar";
                    case (int)HuntType.Walrus: return "Walrus";
                    case (int)HuntType.Alligator: return "Alligator";
                    case (int)HuntType.Eagle: return "Eagle";
                    //Publish 91 added:
                    case (int)HuntType.MyrmidexLarvae: return "Myrmidex Larvae";
                    case (int)HuntType.Najasaurus: return "Najasaurus";
                    case (int)HuntType.Anchisaur: return "Anchisaur";
                    case (int)HuntType.Allosaurus: return "Allosaurus";
                    case (int)HuntType.Dimetrosaur: return "Dimetrosaur";
                    case (int)HuntType.Saurosaurus: return "Saurosaurus";
                    //Publish 95 added:
                    case (int)HuntType.Tiger: return "Tiger";
                    case (int)HuntType.MyrmidexDrone: return "Myrmidex Drone";
                    case (int)HuntType.Triceratops: return "Triceratops";
                    case (int)HuntType.Lion: return "Lion";
                    case (int)HuntType.WhiteTiger: return "White Tiger";
                    case (int)HuntType.BlackTiger: return "Black Tiger";
                    //Publish 102 added:
                    case (int)HuntType.Raptor: return "Raptor";
                    case (int)HuntType.SeaSerpent: return "Sea Serpent";
                    case (int)HuntType.Scorpion: return "Scorpion";
                }
            }
        }
    }
}
