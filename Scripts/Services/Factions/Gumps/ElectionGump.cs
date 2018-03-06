using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Factions
{
    public class ElectionGump : FactionGump
    {
        private readonly PlayerMobile m_From;
        private readonly Election m_Election;

        public ElectionGump(PlayerMobile from, Election election)
            : base(50, 50)
        {
            m_From = from;
            m_Election = election;

            AddPage(0);

            AddBackground(0, 0, 420, 180, 5054);
            AddBackground(10, 10, 400, 160, 3000);

            AddHtmlText(20, 20, 380, 20, election.Faction.Definition.Header, false, false);

            // NOTE: Gump not entirely OSI-accurate, intentionally so

            switch ( election.State )
            {
                case ElectionState.Pending:
                    {
                        TimeSpan toGo = (election.LastStateTime + Election.PendingPeriod) - DateTime.UtcNow;
                        int days = (int)(toGo.TotalDays + 0.5);

                        AddHtmlLocalized(20, 40, 380, 20, 1038034, false, false); // A new election campaign is pending

                        if (days > 0)
                        {
                            AddHtmlLocalized(20, 60, 280, 20, 1018062, false, false); // Days until next election :
                            AddLabel(300, 60, 0, days.ToString());
                        }
                        else
                        {
                            AddHtmlLocalized(20, 60, 280, 20, 1018059, false, false); // Election campaigning begins tonight.
                        }

                        break;
                    }
                case ElectionState.Campaign:
                    {
                        TimeSpan toGo = (election.LastStateTime + Election.CampaignPeriod) - DateTime.UtcNow;
                        int days = (int)(toGo.TotalDays + 0.5);

                        AddHtmlLocalized(20, 40, 380, 20, 1018058, false, false); // There is an election campaign in progress.

                        if (days > 0)
                        {
                            AddHtmlLocalized(20, 60, 280, 20, 1038033, false, false); // Days to go:
                            AddLabel(300, 60, 0, days.ToString());
                        }
                        else
                        {
                            AddHtmlLocalized(20, 60, 280, 20, 1018061, false, false); // Campaign in progress. Voting begins tonight.
                        }

                        if (m_Election.CanBeCandidate(m_From))
                        {
                            AddButton(20, 110, 4005, 4007, 2, GumpButtonType.Reply, 0);
                            AddHtmlLocalized(55, 110, 350, 20, 1011427, false, false); // CAMPAIGN FOR LEADERSHIP
                        }
                        else
                        {
                            PlayerState pl = PlayerState.Find(m_From);

                            if (pl == null || pl.Rank.Rank < Election.CandidateRank)
                                AddHtmlLocalized(20, 100, 380, 20, 1010118, false, false); // You must have a higher rank to run for office
                        }

                        break;
                    }
                case ElectionState.Election:
                    {
                        TimeSpan toGo = (election.LastStateTime + Election.VotingPeriod) - DateTime.UtcNow;
                        int days = (int)Math.Ceiling(toGo.TotalDays);

                        AddHtmlLocalized(20, 40, 380, 20, 1018060, false, false); // There is an election vote in progress.

                        AddHtmlLocalized(20, 60, 280, 20, 1038033, false, false);
                        AddLabel(300, 60, 0, days.ToString());

                        AddHtmlLocalized(55, 100, 380, 20, 1011428, false, false); // VOTE FOR LEADERSHIP
                        AddButton(20, 100, 4005, 4007, 1, GumpButtonType.Reply, 0);

                        break;
                    }
            }

            AddButton(20, 140, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 140, 350, 20, 1011012, false, false); // CANCEL
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch ( info.ButtonID )
            {
                case 0: // back
                    {
                        m_From.SendGump(new FactionStoneGump(m_From, m_Election.Faction));
                        break;
                    }
                case 1: // vote
                    {
                        if (m_Election.State == ElectionState.Election)
                            m_From.SendGump(new VoteGump(m_From, m_Election));

                        break;
                    }
                case 2: // campaign
                    {
                        if (m_Election.CanBeCandidate(m_From))
                            m_Election.AddCandidate(m_From);

                        break;
                    }
            }
        }
    }
}