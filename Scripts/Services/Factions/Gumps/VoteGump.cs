using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Factions
{
    public class VoteGump : FactionGump
    {
        private readonly PlayerMobile m_From;
        private readonly Election m_Election;
        public VoteGump(PlayerMobile from, Election election)
            : base(50, 50)
        {
            this.m_From = from;
            this.m_Election = election;

            bool canVote = election.CanVote(from);

            this.AddPage(0);

            this.AddBackground(0, 0, 420, 350, 5054);
            this.AddBackground(10, 10, 400, 330, 3000);

            this.AddHtmlText(20, 20, 380, 20, election.Faction.Definition.Header, false, false);

            if (canVote)
                this.AddHtmlLocalized(20, 60, 380, 20, 1011428, false, false); // VOTE FOR LEADERSHIP
            else
                this.AddHtmlLocalized(20, 60, 380, 20, 1038032, false, false); // You have already voted in this election.

            for (int i = 0; i < election.Candidates.Count; ++i)
            {
                Candidate cd = election.Candidates[i];

                if (canVote)
                    this.AddButton(20, 100 + (i * 20), 4005, 4007, i + 1, GumpButtonType.Reply, 0);

                this.AddLabel(55, 100 + (i * 20), 0, cd.Mobile.Name);
                this.AddLabel(300, 100 + (i * 20), 0, cd.Votes.ToString());
            }

            this.AddButton(20, 310, 4005, 4007, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 310, 100, 20, 1011012, false, false); // CANCEL
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                this.m_From.SendGump(new FactionStoneGump(this.m_From, this.m_Election.Faction));
            }
            else
            {
                if (!this.m_Election.CanVote(this.m_From))
                    return;

                int index = info.ButtonID - 1;

                if (index >= 0 && index < this.m_Election.Candidates.Count)
                    this.m_Election.Candidates[index].Voters.Add(new Voter(this.m_From, this.m_Election.Candidates[index].Mobile));

                this.m_From.SendGump(new VoteGump(this.m_From, this.m_Election));
            }
        }
    }
}