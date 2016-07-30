using Server;
using System;
using Server.Mobiles;
using Server.Engines.HuntsmasterChallenge;
using System.Collections.Generic;
using Server.Gumps;

namespace Server.Items
{
	public class BestKillBoard : Item
	{
        public override string DefaultName { get { return "Top 10 Kill Board"; } }

        [Constructable]
		public BestKillBoard() : base(7775)
		{
            Movable = false;
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.Location, 3) && HuntingSystem.Instance != null && HuntingSystem.Instance.Active)
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
			writer.Write((int)0);
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

                if (m_Filter < -1) m_Filter = 8;
                if (m_Filter > 8) m_Filter = -1;

                List<HuntingKillEntry> useList = new List<HuntingKillEntry>();

                if (m_Filter == -1)
                {
                    foreach (KeyValuePair<HuntType, List<HuntingKillEntry>> kvp in HuntingSystem.Instance.Top10)
                    {
                        if(kvp.Value.Count > 0)
                            useList.AddRange(kvp.Value);
                    }
                }
                else if(HuntingSystem.Instance.Top10.ContainsKey((HuntType)m_Filter))
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

                AddHtml(0, 365, 500, 16, String.Format("<Center>{0}</Center>", GetHuntTypeString()), false, false);

                AddButton(150, 365, 4014, 4016, 1, GumpButtonType.Reply, 0);
                AddButton(328, 365, 4005, 4007, 2, GumpButtonType.Reply, 0);
            }

            private string FormatFont(string str, int index)
            {
                int hue = 080000 + (100000 * index);

                return String.Format("<BaseFont Color=#{0}>{1}</basefont>", hue.ToString(), str);
            }

            public override void OnResponse(Server.Network.NetState state, RelayInfo info)
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
                    case HuntType.GrayWolf: return "Gray Wolf";
                    case HuntType.Cougar: return "Cougar";
                    case HuntType.Turkey: return "Turkey";
                    case HuntType.Bull: return "Bull";
                    case HuntType.Boar: return "Boar";
                    case HuntType.Walrus: return "Walrus";
                    case HuntType.Alligator: return "Alligator";
                    case HuntType.Eagle: return "Eagle";
                }
            }

            private string GetHuntTypeString()
            {
                switch (m_Filter)
                {
                    default: return "No Filter";
                    case (int)HuntType.GrizzlyBear: return "Grizzly Bear";
                    case (int)HuntType.GrayWolf: return "Gray Wolf";
                    case (int)HuntType.Cougar: return "Cougar";
                    case (int)HuntType.Turkey: return "Turkey";
                    case (int)HuntType.Bull: return "Bull";
                    case (int)HuntType.Boar: return "Boar";
                    case (int)HuntType.Walrus: return "Walrus";
                    case (int)HuntType.Alligator: return "Alligator";
                    case (int)HuntType.Eagle: return "Eagle";
                }
            }
        }
	}
}