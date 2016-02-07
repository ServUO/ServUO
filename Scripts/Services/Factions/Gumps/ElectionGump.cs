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
            this.m_From = from;
            this.m_Election = election;

            this.AddPage(0);

            this.AddBackground(0, 0, 420, 180, 5054);
            this.AddBackground(10, 10, 400, 160, 3000);

            this.AddHtmlText(20, 20, 380, 20, election.Faction.Definition.Header, false, false);

            // NOTE: Gump not entirely OSI-accurate, intentionally so

            switch ( election.State )
            {
                case ElectionState.Pending:
                    {
                        TimeSpan toGo = (election.LastStateTime + Election.PendingPeriod) - DateTime.UtcNow;
                        int days = (int)(toGo.TotalDays + 0.5);

                        this.AddHtmlLocalized(20, 40, 380, 20, 1038034, false, false); // A new election campaign is pending

                        if (days > 0)
                        {
                            this.AddHtmlLocalized(20, 60, 280, 20, 1018062, false, false); // Days until next election :
                            this.AddLabel(300, 60, 0, days.ToString());
                        }
                        else
                        {
                            this.AddHtmlLocalized(20, 60, 280, 20, 1018059, false, false); // Election campaigning begins tonight.
                        }

                        break;
                    }
                case ElectionState.Campaign:
                    {
                        TimeSpan toGo = (election.LastStateTime + Election.CampaignPeriod) - DateTime.UtcNow;
                        int days = (int)(toGo.TotalDays + 0.5);

                        this.AddHtmlLocalized(20, 40, 380, 20, 1018058, false, false); // There is an election campaign in progress.

                        if (days > 0)
                        {
                            this.AddHtmlLocalized(20, 60, 280, 20, 1038033, false, false); // Days to go:
                            this.AddLabel(300, 60, 0, days.ToString());
                        }
                        else
                        {
                            this.AddHtmlLocalized(20, 60, 280, 20, 1018061, false, false); // Campaign in progress. Voting begins tonight.
                        }

                        if (this.m_Election.CanBeCandidate(this.m_From))
                        {
                            this.AddButton(20, 110, 4005, 4007, 2, GumpButtonType.Reply, 0);
                            this.AddHtmlLocalized(55, 110, 350, 20, 1011427, false, false); // CAMPAIGN FOR LEADERSHIP
                        }
                        else
                        {
                            PlayerState pl = PlayerState.Find(this.m_From);

                            if (pl == null || pl.Rank.Rank < Election.CandidateRank)
                                this.AddHtmlLocalized(20, 100, 380, 20, 1010118, false, false); // You must have a higher rank to run for office
                        }

                        break;
                    }
                case ElectionState.Election:
                    {
                        TimeSpan toGo = (election.LastStateTime + Election.VotingPeriod) - DateTime.UtcNow;
                        int days = (int)Math.Ceiling(toGo.TotalDays);

                        this.AddHtmlLocalized(20, 40, 380, 20, 1018060, false, false); // There is an election vote in progress.

                        this.AddHtmlLocalized(20, 60, 280, 20, 1038033, false, false);
                        this.AddLabel(300, 60, 0, days.ToString());

                        this.AddHtmlLocalized(55, 100, 380, 20, 1011428, false, false); // VOTE FOR LEADERSHIP
                        this.AddButton(20, 100, 4005, 4007, 1, GumpButtonType.Reply, 0);

                        break;
                    }
            }

            this.AddButton(20, 140, 4005, 4007, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 140, 350, 20, 1011012, false, false); // CANCEL
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch ( info.ButtonID )
            {
                case 0: // back
                    {
                        this.m_From.SendGump(new FactionStoneGump(this.m_From, this.m_Election.Faction));
                        break;
                    }
                case 1: // vote
                    {
                        if (this.m_Election.State == ElectionState.Election)
                            this.m_From.SendGump(new VoteGump(this.m_From, this.m_Election));

                        break;
                    }
                case 2: // campaign
                    {
                        if (this.m_Election.CanBeCandidate(this.m_From))
                            this.m_Election.AddCandidate(this.m_From);

                        break;
                    }
            }
        }
    }
}