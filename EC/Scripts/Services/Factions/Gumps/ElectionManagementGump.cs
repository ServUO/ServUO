using System;
using Server.Gumps;
using Server.Network;

namespace Server.Factions
{
    public class ElectionManagementGump : Gump
    {
        public const int LabelColor = 0xFFFFFF;
        private readonly Election m_Election;
        private readonly Candidate m_Candidate;
        private readonly int m_Page;
        public ElectionManagementGump(Election election)
            : this(election, null, 0)
        {
        }

        public ElectionManagementGump(Election election, Candidate candidate, int page)
            : base(40, 40)
        {
            this.m_Election = election;
            this.m_Candidate = candidate;
            this.m_Page = page;

            this.AddPage(0);

            if (candidate != null)
            {
                this.AddBackground(0, 0, 448, 354, 9270);
                this.AddAlphaRegion(10, 10, 428, 334);

                this.AddHtml(10, 10, 428, 20, this.Color(this.Center("Candidate Management"), LabelColor), false, false);

                this.AddHtml(45, 35, 100, 20, this.Color("Player Name:", LabelColor), false, false);
                this.AddHtml(145, 35, 100, 20, this.Color(candidate.Mobile == null ? "null" : candidate.Mobile.Name, LabelColor), false, false);

                this.AddHtml(45, 55, 100, 20, this.Color("Vote Count:", LabelColor), false, false);
                this.AddHtml(145, 55, 100, 20, this.Color(candidate.Votes.ToString(), LabelColor), false, false);

                this.AddButton(12, 73, 4005, 4007, 1, GumpButtonType.Reply, 0);
                this.AddHtml(45, 75, 100, 20, this.Color("Drop Candidate", LabelColor), false, false);

                this.AddImageTiled(13, 99, 422, 242, 9264);
                this.AddImageTiled(14, 100, 420, 240, 9274);
                this.AddAlphaRegion(14, 100, 420, 240);

                this.AddHtml(14, 100, 420, 20, this.Color(this.Center("Voters"), LabelColor), false, false);

                if (page > 0)
                    this.AddButton(397, 104, 0x15E3, 0x15E7, 2, GumpButtonType.Reply, 0);
                else
                    this.AddImage(397, 104, 0x25EA);

                if ((page + 1) * 10 < candidate.Voters.Count)
                    this.AddButton(414, 104, 0x15E1, 0x15E5, 3, GumpButtonType.Reply, 0);
                else
                    this.AddImage(414, 104, 0x25E6);

                this.AddHtml(14, 120, 30, 20, this.Color(this.Center("DEL"), LabelColor), false, false);
                this.AddHtml(47, 120, 150, 20, this.Color("Name", LabelColor), false, false);
                this.AddHtml(195, 120, 100, 20, this.Color(this.Center("Address"), LabelColor), false, false);
                this.AddHtml(295, 120, 80, 20, this.Color(this.Center("Time"), LabelColor), false, false);
                this.AddHtml(355, 120, 60, 20, this.Color(this.Center("Legit"), LabelColor), false, false);

                int idx = 0;

                for (int i = page * 10; i >= 0 && i < candidate.Voters.Count && i < (page + 1) * 10; ++i, ++idx)
                {
                    Voter voter = (Voter)candidate.Voters[i];

                    this.AddButton(13, 138 + (idx * 20), 4002, 4004, 4 + i, GumpButtonType.Reply, 0);

                    object[] fields = voter.AcquireFields();

                    int x = 45;

                    for (int j = 0; j < fields.Length; ++j)
                    {
                        object obj = fields[j];

                        if (obj is Mobile)
                        {
                            this.AddHtml(x + 2, 140 + (idx * 20), 150, 20, this.Color(((Mobile)obj).Name, LabelColor), false, false);
                            x += 150;
                        }
                        else if (obj is System.Net.IPAddress)
                        {
                            this.AddHtml(x, 140 + (idx * 20), 100, 20, this.Color(this.Center(obj.ToString()), LabelColor), false, false);
                            x += 100;
                        }
                        else if (obj is DateTime)
                        {
                            this.AddHtml(x, 140 + (idx * 20), 80, 20, this.Color(this.Center(FormatTimeSpan(((DateTime)obj) - election.LastStateTime)), LabelColor), false, false);
                            x += 80;
                        }
                        else if (obj is int)
                        {
                            this.AddHtml(x, 140 + (idx * 20), 60, 20, this.Color(this.Center((int)obj + "%"), LabelColor), false, false);
                            x += 60;
                        }
                    }
                }
            }
            else
            {
                this.AddBackground(0, 0, 288, 334, 9270);
                this.AddAlphaRegion(10, 10, 268, 314);

                this.AddHtml(10, 10, 268, 20, this.Color(this.Center("Election Management"), LabelColor), false, false);

                this.AddHtml(45, 35, 100, 20, this.Color("Current State:", LabelColor), false, false);
                this.AddHtml(145, 35, 100, 20, this.Color(election.State.ToString(), LabelColor), false, false);

                this.AddButton(12, 53, 4005, 4007, 1, GumpButtonType.Reply, 0);
                this.AddHtml(45, 55, 100, 20, this.Color("Transition Time:", LabelColor), false, false);
                this.AddHtml(145, 55, 100, 20, this.Color(FormatTimeSpan(election.NextStateTime), LabelColor), false, false);

                this.AddImageTiled(13, 79, 262, 242, 9264);
                this.AddImageTiled(14, 80, 260, 240, 9274);
                this.AddAlphaRegion(14, 80, 260, 240);

                this.AddHtml(14, 80, 260, 20, this.Color(this.Center("Candidates"), LabelColor), false, false);
                this.AddHtml(14, 100, 30, 20, this.Color(this.Center("-->"), LabelColor), false, false);
                this.AddHtml(47, 100, 150, 20, this.Color("Name", LabelColor), false, false);
                this.AddHtml(195, 100, 80, 20, this.Color(this.Center("Votes"), LabelColor), false, false);

                for (int i = 0; i < election.Candidates.Count; ++i)
                {
                    Candidate cd = election.Candidates[i];
                    Mobile mob = cd.Mobile;

                    if (mob == null)
                        continue;

                    this.AddButton(13, 118 + (i * 20), 4005, 4007, 2 + i, GumpButtonType.Reply, 0);
                    this.AddHtml(47, 120 + (i * 20), 150, 20, this.Color(mob.Name, LabelColor), false, false);
                    this.AddHtml(195, 120 + (i * 20), 80, 20, this.Color(this.Center(cd.Votes.ToString()), LabelColor), false, false);
                }
            }
        }

        public static string FormatTimeSpan(TimeSpan ts)
        {
            return String.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", ts.Days, ts.Hours % 24, ts.Minutes % 60, ts.Seconds % 60);
        }

        public string Right(string text)
        {
            return String.Format("<DIV ALIGN=RIGHT>{0}</DIV>", text);
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            int bid = info.ButtonID;

            if (this.m_Candidate == null)
            {
                if (bid == 0)
                {
                }
                else if (bid == 1)
                {
                }
                else
                {
                    bid -= 2;

                    if (bid >= 0 && bid < this.m_Election.Candidates.Count)
                        from.SendGump(new ElectionManagementGump(this.m_Election, this.m_Election.Candidates[bid], 0));
                }
            }
            else
            {
                if (bid == 0)
                {
                    from.SendGump(new ElectionManagementGump(this.m_Election));
                }
                else if (bid == 1)
                {
                    this.m_Election.RemoveCandidate(this.m_Candidate.Mobile);
                    from.SendGump(new ElectionManagementGump(this.m_Election));
                }
                else if (bid == 2 && this.m_Page > 0)
                {
                    from.SendGump(new ElectionManagementGump(this.m_Election, this.m_Candidate, this.m_Page - 1));
                }
                else if (bid == 3 && (this.m_Page + 1) * 10 < this.m_Candidate.Voters.Count)
                {
                    from.SendGump(new ElectionManagementGump(this.m_Election, this.m_Candidate, this.m_Page + 1));
                }
                else
                {
                    bid -= 4;

                    if (bid >= 0 && bid < this.m_Candidate.Voters.Count)
                    {
                        this.m_Candidate.Voters.RemoveAt(bid);
                        from.SendGump(new ElectionManagementGump(this.m_Election, this.m_Candidate, this.m_Page));
                    }
                }
            }
        }
    }
}