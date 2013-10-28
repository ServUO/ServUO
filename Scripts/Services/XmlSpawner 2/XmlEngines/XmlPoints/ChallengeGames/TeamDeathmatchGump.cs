using System;
using System.Collections;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Network;
using Server.Targeting;

/*
** TeamDeathmatchGump
** ArteGordon
** updated 12/06/04
**
*/
namespace Server.Gumps
{
    public class TeamDeathmatchGump : Gump
    {
        private const int textentrybackground = 0x24A8;
        private readonly int y_inc = 25;
        private readonly int PlayersPerPage = 10;
        private readonly int MaxTeamSize = 0;// zero means unlimited number of players
        private readonly TeamDeathmatchGauntlet m_gauntlet;
        private readonly ArrayList m_WorkingList;
        private readonly int viewpage;
        public TeamDeathmatchGump(TeamDeathmatchGauntlet gauntlet, Mobile from)
            : this(gauntlet, from, 0)
        {
        }

        public TeamDeathmatchGump(TeamDeathmatchGauntlet gauntlet, Mobile from, int page)
            : base(20, 30)
        {
            if (from == null || gauntlet == null || gauntlet.Deleted || gauntlet.Challenger == null)
                return;

            from.CloseGump(typeof(TeamDeathmatchGump));

            this.m_gauntlet = gauntlet;

            this.viewpage = page;

            int height = 555;

            this.AddBackground(0, 0, 350, height, 0xDAC);
            //AddAlphaRegion( 0, 0, 340, height );

            this.AddLabel(100, 10, 0, XmlPoints.GetText(from, 200590));  // "Team Deathmatch Challenge"
            this.AddLabel(20, 30, 0, String.Format(XmlPoints.GetText(from, 200501), gauntlet.Challenger.Name));  // "Organized by: {0}"
            this.AddLabel(20, 50, 0, String.Format(XmlPoints.GetText(from, 200502), this.m_gauntlet.EntryFee));      // "Entry Fee: {0}"
            this.AddLabel(20, 70, 0, String.Format(XmlPoints.GetText(from, 200503), this.m_gauntlet.ArenaSize)); // "Arena Size: {0}"

            this.AddImageTiled(15, 130, 320, 20, 0xdb3);

            // display all of the current team members
            if (gauntlet.Participants != null)
            {
                // copy the master list to a temporary working list
                this.m_WorkingList = (ArrayList)gauntlet.Participants.Clone();

                this.AddLabel(150, 50, 0, String.Format(XmlPoints.GetText(from, 200504), this.m_WorkingList.Count * this.m_gauntlet.EntryFee)); // "Total Purse: {0}"

                this.AddLabel(150, 70, 0, String.Format(XmlPoints.GetText(from, 200505), this.m_gauntlet.Location, this.m_gauntlet.Map));  // "Loc: {0} {1}"

                this.AddLabel(20, 90, 0, String.Format(XmlPoints.GetText(from, 200506), gauntlet.Participants.Count));   // "Players: {0}"
                
                this.AddLabel(150, 90, 0, String.Format(XmlPoints.GetText(from, 200507), gauntlet.ActivePlayers()));  // "Active: {0}"

                if (gauntlet.TargetScore > 0)
                    this.AddLabel(20, 110, 0, String.Format(XmlPoints.GetText(from, 200561), gauntlet.TargetScore));  // "Target Score: {0}"
                else
                    this.AddLabel(20, 110, 0, XmlPoints.GetText(from, 200562)); // "Target Score: None"

                if (gauntlet.MatchLength > TimeSpan.Zero)
                    this.AddLabel(150, 110, 0, String.Format(XmlPoints.GetText(from, 200563), gauntlet.MatchLength));  // "Match Length: {0}"
                else
                    this.AddLabel(150, 110, 0, XmlPoints.GetText(from, 200564));   // "Match Length: Unlimited"

                int yoffset = 155;

                // page up and down buttons
                this.AddButton(300, 130, 0x15E0, 0x15E4, 13, GumpButtonType.Reply, 0);
                this.AddButton(320, 130, 0x15E2, 0x15E6, 12, GumpButtonType.Reply, 0);

                // find the players entry to determine the viewing page
                for (int i = 0; i < this.m_WorkingList.Count; i++)
                {
                    TeamDeathmatchGauntlet.ChallengeEntry entry = (TeamDeathmatchGauntlet.ChallengeEntry)this.m_WorkingList[i];

                    if (entry == null)
                        continue;

                    if (entry.Participant == from)
                    {
                        this.viewpage = entry.PageBeingViewed;
                        break;
                    }
                }

                this.AddLabel(220, 130, 0, String.Format(XmlPoints.GetText(from, 200508), this.viewpage + 1, (int)(this.m_WorkingList.Count / this.PlayersPerPage) + 1)); // "Page: {0}/{1}"

                if (gauntlet.GameInProgress && gauntlet.MatchLength > TimeSpan.Zero)
                {
                    this.AddLabelCropped(20, 130, 180, 21, 0, String.Format(XmlPoints.GetText(from, 200565), // "Time left {0}"
                        TimeSpan.FromSeconds((double)((int)((gauntlet.MatchStart + gauntlet.MatchLength - DateTime.UtcNow).TotalSeconds)))));
                }

                this.AddLabel(160, 130, 0, XmlPoints.GetText(from, 200591));  // "Team"

                for (int i = 0; i < this.m_WorkingList.Count; i++)
                {
                    // determine which page is being viewed
                    if ((int)(i / this.PlayersPerPage) != this.viewpage)
                        continue;

                    TeamDeathmatchGauntlet.ChallengeEntry entry = (TeamDeathmatchGauntlet.ChallengeEntry)this.m_WorkingList[i];

                    if (entry == null)
                        continue;

                    // display the entry with a color indicating whether they have accepted or not
                    Mobile m = entry.Participant;

                    string statusmsg = XmlPoints.GetText(from, 200509);   // "Waiting"
                    int texthue = 0;

                    // this section is available during game setup
                    if (!this.m_gauntlet.GameLocked)
                    {
                        statusmsg = XmlPoints.GetText(from, 200509);  // "Waiting"
                        if (entry.Accepted)
                        {
                            texthue = 68;
                            statusmsg = XmlPoints.GetText(from, 200510);    // "Accepted"
                        }

                        // check to see if they have the Entry fee
                        if (!this.m_gauntlet.HasEntryFee(m))
                        {
                            texthue = 33;
                            statusmsg = XmlPoints.GetText(from, 200511);   // "Insufficient funds"
                        }

                        // if the game is still open then enable these buttons

                        // if this is the participant then add the accept button to the entry
                        if (m == from)
                        {
                            this.AddButton(15, yoffset, entry.Accepted ? 0xd1 : 0xd0, entry.Accepted ? 0xd0 : 0xd1, 1000 + i, GumpButtonType.Reply, 0);
                        }
                        // if this is the organizer then add the kick button and the team assignment to each entry
                        if (from == this.m_gauntlet.Challenger)
                        {
                            this.AddImageTiled(223, yoffset, 20, 19, textentrybackground);
                            this.AddTextEntry(225, yoffset, 20, 19, 0, 500 + i, entry.Team.ToString());
                        }
                        if (from == this.m_gauntlet.Challenger || from == entry.Participant)
                        {
                            this.AddButton(190, yoffset, 0xFB1, 0xFB3, 2000 + i, GumpButtonType.Reply, 0);
                        }
                    }
                    else
                    {
                        // this section is active after the game has started
                        // enable the forfeit button
                        if (m == from && entry.Status == ChallengeStatus.Active && !this.m_gauntlet.GameCompleted)
                        {
                            this.AddButton(190, yoffset, 0xFB1, 0xFB3, 4000 + i, GumpButtonType.Reply, 0);
                        }

                        if (entry.Status == ChallengeStatus.Forfeit)
                        {
                            texthue = 33;
                            statusmsg = XmlPoints.GetText(from, 200520);  // "Forfeit"
                        }
                        else if (entry.Caution == ChallengeStatus.Hidden && entry.Status == ChallengeStatus.Active)
                        {
                            texthue = 53;
                            statusmsg = XmlPoints.GetText(from, 200521);   // "Hidden"
                        }
                        else if (entry.Caution == ChallengeStatus.OutOfBounds && entry.Status == ChallengeStatus.Active)
                        {
                            texthue = 53;
                            statusmsg = XmlPoints.GetText(from, 200522);  // "Out of Bounds"
                        }
                        else if (entry.Caution == ChallengeStatus.Offline && entry.Status == ChallengeStatus.Active)
                        {
                            texthue = 53;
                            statusmsg = XmlPoints.GetText(from, 200523);    // "Offline"
                        }
                        else if (entry.Status == ChallengeStatus.Active)
                        {
                            texthue = 68;
                            if (entry.Winner)
                                statusmsg = XmlPoints.GetText(from, 200524);      // "Winner"
                            else
                                statusmsg = XmlPoints.GetText(from, 200525);      // "Active"
                        }
                        else if (entry.Status == ChallengeStatus.Dead)
                        {
                            texthue = 33;
                            statusmsg = XmlPoints.GetText(from, 200526);            // "Dead"
                        }
                        else if (entry.Status == ChallengeStatus.Disqualified)
                        {
                            texthue = 33;
                            statusmsg = XmlPoints.GetText(from, 200527);     // "Disqualified"
                        }
                    }

                    if (m != null)
                    {
                        int teamhue = 0;
                        if (entry.Team > 0)
                        {
                            teamhue = BaseChallengeGame.TeamColor(entry.Team);
                        }
                        this.AddLabel(40, yoffset, teamhue, m.Name);
                        this.AddLabel(165, yoffset, teamhue, entry.Team.ToString());
                        this.AddLabel(255, yoffset, texthue, statusmsg);
                        
                        if (this.m_gauntlet.GameInProgress || this.m_gauntlet.GameCompleted)
                        {
                            this.AddLabel(13, yoffset, 0, entry.Score.ToString());
                        }
                    }

                    yoffset += this.y_inc;
                }
            }

            // the challenger gets additional options
            if (from == gauntlet.Challenger && !this.m_gauntlet.GameLocked)
            {
                this.AddImageTiled(15, height - 135, 320, 20, 0xdb3);

                this.AddButton(130, height - 35, 0xFA8, 0xFAA, 100, GumpButtonType.Reply, 0);
                this.AddLabel(170, height - 35, 0, XmlPoints.GetText(from, 200528));      // "Add"

                this.AddButton(230, height - 35, 0xFB7, 0xFB9, 300, GumpButtonType.Reply, 0);
                this.AddLabel(270, height - 35, 0, XmlPoints.GetText(from, 200529));     // "Start"

                // set entry fee
                this.AddButton(20, height - 110, 0xFAE, 0xFAF, 10, GumpButtonType.Reply, 0);
                this.AddImageTiled(120, height - 110, 60, 19, textentrybackground);
                this.AddTextEntry(120, height - 110, 60, 25, 0, 10, this.m_gauntlet.EntryFee.ToString());
                this.AddLabel(55, height - 110, 0, XmlPoints.GetText(from, 200572));  // "Entry Fee: "

                // set arena size
                this.AddButton(20, height - 85, 0xFAE, 0xFAF, 20, GumpButtonType.Reply, 0);
                this.AddImageTiled(130, height - 85, 30, 19, textentrybackground);
                this.AddTextEntry(130, height - 85, 30, 25, 0, 20, this.m_gauntlet.ArenaSize.ToString());
                this.AddLabel(55, height - 85, 0, XmlPoints.GetText(from, 200573));    //  "Arena Size: "

                // set target score
                this.AddButton(200, height - 110, 0xFAE, 0xFAF, 30, GumpButtonType.Reply, 0);
                this.AddImageTiled(275, height - 110, 30, 19, textentrybackground);
                this.AddTextEntry(275, height - 110, 30, 25, 0, 30, this.m_gauntlet.TargetScore.ToString());
                this.AddLabel(235, height - 110, 0, XmlPoints.GetText(from, 200566));    //  "Score: "

                // set match length
                this.AddButton(200, height - 85, 0xFAE, 0xFAF, 40, GumpButtonType.Reply, 0);
                this.AddImageTiled(310, height - 85, 25, 19, textentrybackground);
                this.AddTextEntry(310, height - 85, 25, 25, 0, 40, this.m_gauntlet.MatchLength.TotalMinutes.ToString());
                this.AddLabel(235, height - 85, 0, XmlPoints.GetText(from, 200567));  // "Length mins: "

                // set teams
                this.AddButton(200, height - 60, 0xFAE, 0xFAF, 11, GumpButtonType.Reply, 0);
                this.AddLabel(240, height - 60, 0, XmlPoints.GetText(from, 200592));   // "Set Teams"
            }
            else
            {
                this.AddImageTiled(15, height - 60, 320, 20, 0xdb3);
            }

            this.AddButton(20, height - 35, 0xFAB, 0xFAD, 1, GumpButtonType.Reply, 0);
            this.AddLabel(60, height - 35, 0, XmlPoints.GetText(from, 200532));  // "Refresh"

            if (gauntlet.GameInProgress)
            {
                this.AddLabel(150, height - 35, 68, XmlPoints.GetText(from, 200533));  // "Game is in progress!"
            }
            else if (gauntlet.Winner != 0)
            {
                this.AddLabel(130, height - 35, 68, String.Format(XmlPoints.GetText(from, 200593), gauntlet.Winner));   // "Team {0} is the winner!"
            }

            //AddButton( 30, height - 35, 0xFB7, 0xFB9, 0, GumpButtonType.Reply, 0 );
            //AddLabel( 70, height - 35, 0, "Close" );
			
            // display the teams gump
            from.CloseGump(typeof(TeamsGump));
            from.SendGump(new TeamsGump(this.m_gauntlet, from));
        }

        public static void RefreshAllGumps(TeamDeathmatchGauntlet gauntlet, bool force)
        {
            if (gauntlet.Participants != null)
            {
                foreach (TeamDeathmatchGauntlet.ChallengeEntry entry in gauntlet.Participants)
                {
                    if (entry.Participant != null)
                    {
                        if (force || entry.Participant.HasGump(typeof(TeamDeathmatchGump)))
                        {
                            entry.Participant.SendGump(new TeamDeathmatchGump(gauntlet, entry.Participant));
                        }
                    }
                }
            }

            // update for the organizer
            if (gauntlet.Challenger != null)
            {
                if (force || gauntlet.Challenger.HasGump(typeof(TeamDeathmatchGump)))
                {
                    gauntlet.Challenger.SendGump(new TeamDeathmatchGump(gauntlet, gauntlet.Challenger));
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (state == null || info == null || state.Mobile == null || this.m_gauntlet == null || this.m_gauntlet.Challenger == null)
                return;

            XmlPoints afrom = (XmlPoints)XmlAttach.FindAttachment(state.Mobile, typeof(XmlPoints));

            switch ( info.ButtonID )
            {
                case 1:
                    // refresh

                    //m_gauntlet.CheckForDisqualification();

                    state.Mobile.SendGump(new TeamDeathmatchGump(this.m_gauntlet, state.Mobile, this.viewpage));
                    break;
                case 10:
                    // Entry fee
                    int val = 0;
                    TextRelay tr = info.GetTextEntry(10);
                    if (tr != null)
                    {
                        try
                        {
                            val = int.Parse(tr.Text);
                        }
                        catch
                        {
                        }
                    }
                    this.m_gauntlet.EntryFee = val;

                    this.m_gauntlet.ResetAcceptance();

                    // update all the gumps
                    RefreshAllGumps(this.m_gauntlet, true);

                    state.Mobile.SendGump(new TeamDeathmatchGump(this.m_gauntlet, state.Mobile, this.viewpage));
                    break;
                case 11:
                    // update teams
                    if (this.m_WorkingList != null)
                        for (int i = 0; i < this.m_WorkingList.Count; i++)
                        {
                            // is this on the visible page?
                            if ((int)(i / this.PlayersPerPage) != this.viewpage)
                                continue;

                            TeamDeathmatchGauntlet.ChallengeEntry entry = (TeamDeathmatchGauntlet.ChallengeEntry)this.m_WorkingList[i];
                            if (entry != null)
                            {
                                int tval = 0;
                                tr = info.GetTextEntry(500 + i);
                                if (tr != null)
                                {
                                    try
                                    {
                                        tval = int.Parse(tr.Text);
                                    }
                                    catch
                                    {
                                    }
                                }
                                entry.Team = tval;
                            }
                        }

                    this.m_gauntlet.ResetAcceptance();
                    // update all the gumps
                    RefreshAllGumps(this.m_gauntlet, true);

                    state.Mobile.SendGump(new TeamDeathmatchGump(this.m_gauntlet, state.Mobile, this.viewpage));
                    break;
                case 12:
                    // page up
                    // try doing the default for non-participants
                    int nplayers = 0;
                    if (this.m_gauntlet.Participants != null)
                        nplayers = this.m_gauntlet.Participants.Count;

                    int page = this.viewpage + 1;
                    if (page > (int)(nplayers / this.PlayersPerPage))
                    {
                        page = (int)(nplayers / this.PlayersPerPage);
                    }

                    if (this.m_WorkingList != null)
                        for (int i = 0; i < this.m_WorkingList.Count; i++)
                        {
                            TeamDeathmatchGauntlet.ChallengeEntry entry = (TeamDeathmatchGauntlet.ChallengeEntry)this.m_WorkingList[i];
                            if (entry != null)
                            {
                                if (entry.Participant == state.Mobile)
                                {
                                    entry.PageBeingViewed++;

                                    if (entry.PageBeingViewed > (int)(nplayers / this.PlayersPerPage))
                                        entry.PageBeingViewed = (int)(nplayers / this.PlayersPerPage);

                                    page = entry.PageBeingViewed;
                                    //break;
                                }
                            }
                        }

                    state.Mobile.SendGump(new TeamDeathmatchGump(this.m_gauntlet, state.Mobile, page));
                    break;
                case 13:
                    // page down
                    // try doing the default for non-participants
                    page = this.viewpage - 1;
                    if (page < 0)
                    {
                        page = 0;
                    }
                    if (this.m_WorkingList != null)
                        for (int i = 0; i < this.m_WorkingList.Count; i++)
                        {
                            TeamDeathmatchGauntlet.ChallengeEntry entry = (TeamDeathmatchGauntlet.ChallengeEntry)this.m_WorkingList[i];
                            if (entry != null)
                            {
                                if (entry.Participant == state.Mobile)
                                {
                                    entry.PageBeingViewed--;

                                    if (entry.PageBeingViewed < 0)
                                        entry.PageBeingViewed = 0;

                                    page = entry.PageBeingViewed;
                                    //break;
                                }
                            }
                        }

                    state.Mobile.SendGump(new TeamDeathmatchGump(this.m_gauntlet, state.Mobile, page));
                    break;
                case 20:
                    // arena size
                    val = 0;
                    tr = info.GetTextEntry(20);
                    if (tr != null)
                    {
                        try
                        {
                            val = int.Parse(tr.Text);
                        }
                        catch
                        {
                        }
                    }
                    this.m_gauntlet.ArenaSize = val;

                    this.m_gauntlet.ResetAcceptance();

                    // update all the gumps
                    RefreshAllGumps(this.m_gauntlet, true);

                    state.Mobile.SendGump(new TeamDeathmatchGump(this.m_gauntlet, state.Mobile, this.viewpage));
                    break;
                case 30:
                    // target score
                    val = 0;
                    tr = info.GetTextEntry(30);
                    if (tr != null)
                    {
                        try
                        {
                            val = int.Parse(tr.Text);
                        }
                        catch
                        {
                        }
                    }
                    this.m_gauntlet.TargetScore = val;

                    this.m_gauntlet.ResetAcceptance();

                    // update all the gumps
                    RefreshAllGumps(this.m_gauntlet, true);

                    state.Mobile.SendGump(new TeamDeathmatchGump(this.m_gauntlet, state.Mobile, this.viewpage));
                    break;
                case 40:
                    // match length
                    double dval = 0;
                    tr = info.GetTextEntry(40);
                    if (tr != null)
                    {
                        try
                        {
                            dval = double.Parse(tr.Text);
                        }
                        catch
                        {
                        }
                    }
                    this.m_gauntlet.MatchLength = TimeSpan.FromMinutes(dval);

                    this.m_gauntlet.ResetAcceptance();

                    // update all the gumps
                    RefreshAllGumps(this.m_gauntlet, true);

                    state.Mobile.SendGump(new TeamDeathmatchGump(this.m_gauntlet, state.Mobile, this.viewpage));
                    break;
                case 100:

                    // add to Participants
                    if (this.m_gauntlet.Participants == null)
                        this.m_gauntlet.Participants = new ArrayList();

                    if (this.MaxTeamSize > 0 && this.m_gauntlet.Participants.Count >= this.MaxTeamSize)
                    {
                        XmlPoints.SendText(state.Mobile, 100535);     // "Challenge is full!"
                    }
                    else
                    {
                        state.Mobile.Target = new MemberTarget(this.m_gauntlet, this.m_gauntlet.Participants);
                    }

                    state.Mobile.SendGump(new TeamDeathmatchGump(this.m_gauntlet, state.Mobile, this.viewpage));
                    break;
                case 300:
                    // Start game
                    if (this.m_WorkingList == null)
                        return;

                    bool complete = true;
                    foreach (TeamDeathmatchGauntlet.ChallengeEntry entry in this.m_WorkingList)
                    {
                        if (entry != null)
                        {
                            Mobile m = entry.Participant;

                            if (m == null)
                                continue;
                            
                            if (!this.m_gauntlet.CheckQualify(m))
                            {
                                complete = false;
                                break;
                            }

                            if (!entry.Accepted)
                            {
                                XmlPoints.SendText(this.m_gauntlet.Challenger, 100539, m.Name);  // "{0} has not accepted yet."
                                complete = false;
                                break;
                            }
                            
                            // and they have a team
                            if (entry.Team <= 0)
                            {
                                XmlPoints.SendText(this.m_gauntlet.Challenger, 100594, m.Name); // "{0} has not been assigned a team."
                                complete = false;
                            }
                        }
                    }

                    if (this.m_WorkingList.Count < 2)
                    {
                        XmlPoints.SendText(this.m_gauntlet.Challenger, 100540);  // "Insufficient number of players."
                        complete = false;
                    }
                    // copy all of the accepted entries to the final participants list

                    if (complete)
                    {
                        this.m_gauntlet.Participants = new ArrayList();
    
                        foreach (TeamDeathmatchGauntlet.ChallengeEntry entry in this.m_WorkingList)
                        {
                            if (entry != null)
                            {
                                Mobile m = entry.Participant;

                                if (m == null)
                                    continue;

                                // try to collect any entry fee
                                if (!this.m_gauntlet.CollectEntryFee(m, this.m_gauntlet.EntryFee))
                                    continue;

                                // set up the challenge on each player
                                XmlPoints a = (XmlPoints)XmlAttach.FindAttachment(m, typeof(XmlPoints));
                                if (a != null)
                                {
                                    a.ChallengeGame = this.m_gauntlet;
                                }

                                entry.Status = ChallengeStatus.Active;
    
                                this.m_gauntlet.Participants.Add(entry);
                            }
                        }

                        // and lock the game
                        this.m_gauntlet.StartGame();

                        // refresh all gumps
                        RefreshAllGumps(this.m_gauntlet, true);
                    }
                    else
                    {
                        state.Mobile.SendGump(new TeamDeathmatchGump(this.m_gauntlet, state.Mobile, this.viewpage));
                    }

                    break;
                default:
                    {
                        // forfeit buttons
                        if (info.ButtonID >= 4000)
                        {
                            int selection = info.ButtonID - 4000;

                            if (selection < this.m_WorkingList.Count)
                            {
                                TeamDeathmatchGauntlet.ChallengeEntry entry = (TeamDeathmatchGauntlet.ChallengeEntry)this.m_WorkingList[selection];

                                // find the master participants list entry with the same participant
                                if (this.m_gauntlet.Participants != null)
                                {
                                    TeamDeathmatchGauntlet.ChallengeEntry forfeitentry = null;

                                    foreach (TeamDeathmatchGauntlet.ChallengeEntry masterentry in this.m_gauntlet.Participants)
                                    {
                                        if (entry == masterentry)
                                        {
                                            forfeitentry = masterentry;
                                            break;
                                        }
                                    }

                                    // and remove it
                                    if (forfeitentry != null)
                                    {
                                        forfeitentry.Status = ChallengeStatus.Forfeit;

                                        // inform him that he has been kicked
                                        this.m_gauntlet.Forfeit(forfeitentry.Participant);
                                    }
                                }
                            }
                        }
                        // kick buttons
                        if (info.ButtonID >= 2000)
                        {
                            int selection = info.ButtonID - 2000;

                            if (selection < this.m_WorkingList.Count)
                            {
                                TeamDeathmatchGauntlet.ChallengeEntry entry = (TeamDeathmatchGauntlet.ChallengeEntry)this.m_WorkingList[selection];
                                // find the master participants list entry with the same participant
                                if (this.m_gauntlet.Participants != null)
                                {
                                    TeamDeathmatchGauntlet.ChallengeEntry kickentry = null;

                                    foreach (TeamDeathmatchGauntlet.ChallengeEntry masterentry in this.m_gauntlet.Participants)
                                    {
                                        if (entry == masterentry)
                                        {
                                            kickentry = masterentry;
                                            break;
                                        }
                                    }

                                    // and remove it
                                    if (kickentry != null)
                                    {
                                        this.m_gauntlet.Participants.Remove(kickentry);
                                    
                                        // refresh his gump and inform him that he has been kicked
                                        if (kickentry.Participant != null)
                                        {
                                            XmlPoints.SendText(kickentry.Participant, 100545, this.m_gauntlet.ChallengeName); // "You have been kicked from {0}"
                                            kickentry.Participant.SendGump(new TeamDeathmatchGump(this.m_gauntlet, kickentry.Participant, this.viewpage));
                                        }
                                    }
                                }

                                this.m_gauntlet.ResetAcceptance();
                            }

                            // refresh all gumps
                            RefreshAllGumps(this.m_gauntlet, true);
                        }
                        else if (info.ButtonID >= 1000)
                        {
                            int selection = info.ButtonID - 1000;
                            // set the acceptance flag of the participant
                            if (selection < this.m_WorkingList.Count)
                            {
                                TeamDeathmatchGauntlet.ChallengeEntry entry = (TeamDeathmatchGauntlet.ChallengeEntry)this.m_WorkingList[selection];

                                entry.Accepted = !entry.Accepted;
                            }

                            // refresh all gumps
                            RefreshAllGumps(this.m_gauntlet, true);
                        }
                        break;
                    }
            }
        }

        public class TeamsGump : Gump
        {
            public TeamsGump(TeamDeathmatchGauntlet gauntlet, Mobile from)
                : base(350,30)
            {
                if (gauntlet == null || from == null)
                    return;

                int yinc = 25;

                ArrayList Teams = gauntlet.GetTeams();

                // gump height determined by number of teams
                int height = Teams.Count * yinc + 80;

                this.AddBackground(0, 0, 260, height, 0xDAC);
                //AddAlphaRegion( 0, 0, 340, height );

                this.AddLabel(60, 10, 0, XmlPoints.GetText(from, 200595));   // "Deathmatch Team Status"

                this.AddLabel(20, 40, 0, XmlPoints.GetText(from, 200591));    // "Team"
                this.AddLabel(75, 40, 0, XmlPoints.GetText(from, 200596));    // "Members"
                this.AddLabel(135, 40, 0, XmlPoints.GetText(from, 200597));    // "Active"
                this.AddLabel(185, 40, 0, XmlPoints.GetText(from, 200598));     // "Score"

                int yoffset = 60;
                // list all of the teams and their status
                foreach (TeamInfo t in Teams)
                {
                    int teamhue = 0;
                    if (t.ID > 0)
                    {
                        teamhue = BaseChallengeGame.TeamColor(t.ID);
                    }
                    this.AddLabel(20, yoffset, teamhue, t.ID.ToString());
                    this.AddLabel(75, yoffset, teamhue, t.Members.Count.ToString());
                    this.AddLabel(135, yoffset, teamhue, t.NActive.ToString());
                    this.AddLabel(185, yoffset, teamhue, t.Score.ToString());
    
                    yoffset += yinc;
                }
            }
        }

        private class MemberTarget : Target
        {
            private readonly ArrayList m_list;
            private readonly TeamDeathmatchGauntlet m_gauntlet;
            public MemberTarget(TeamDeathmatchGauntlet gauntlet, ArrayList list)
                : base(30, false, TargetFlags.None)
            {
                this.m_list = list;
                this.m_gauntlet = gauntlet;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null || targeted == null || this.m_gauntlet == null || this.m_list == null)
                    return;

                if (targeted is Mobile && ((Mobile)targeted).Player)
                {
                    Mobile pm = targeted as Mobile;
                    
                    // make sure they qualify
                    if (!this.m_gauntlet.CheckQualify(pm))
                    {
                        return;
                    }

                    // see if they are already in the game
                    if (this.m_gauntlet.Participants != null)
                    {
                        foreach (TeamDeathmatchGauntlet.ChallengeEntry entry in this.m_gauntlet.Participants)
                        {
                            if (pm == entry.Participant)
                            {
                                XmlPoints.SendText(from, 100548, pm.Name);  // "{0} has already been added to the game."
                                return;
                            }
                        }
                    }

                    this.m_list.Add(new TeamDeathmatchGauntlet.ChallengeEntry(pm));

                    XmlPoints.SendText(from, 100549, pm.Name);   // "You have added {0} to the challenge."
                    
                    XmlPoints.SendText(pm, 100550, this.m_gauntlet.Name); // "You have been invited to participate in {0}."

                    // clear all acceptances
                    this.m_gauntlet.ResetAcceptance();

                    // refresh all gumps
                    RefreshAllGumps(this.m_gauntlet, true);
                }
            }
        }
    }
}