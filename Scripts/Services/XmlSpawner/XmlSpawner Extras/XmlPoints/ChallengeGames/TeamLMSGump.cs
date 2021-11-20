using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using System.Collections;
using Server.Targeting;
using Server.Engines.XmlSpawner2;

/*
** TeamLMSGump
** ArteGordon
** updated 12/06/04
**
*/

namespace Server.Gumps
{
	public class TeamLMSGump : Gump
	{
		private int y_inc = 25;

		private int PlayersPerPage = 10;
		
		private int MaxTeamSize = 0;        // zero means unlimited number of players

		private TeamLMSGauntlet m_gauntlet;
		private ArrayList m_WorkingList;
		private int viewpage;
		private const int textentrybackground = 0x24A8;

        public TeamLMSGump( TeamLMSGauntlet gauntlet, Mobile from ) : this( gauntlet, from, 0 )
		{
		}

		public TeamLMSGump( TeamLMSGauntlet gauntlet, Mobile from, int page ) : base( 20, 30 )
		{

			if(from == null || gauntlet == null || gauntlet.Deleted || gauntlet.Challenger == null) return;

			from.CloseGump(typeof(TeamLMSGump));

			m_gauntlet = gauntlet;

			viewpage = page;

            int height = 500;

			AddBackground( 0, 0, 350, height, 0xDAC );
			//AddAlphaRegion( 0, 0, 340, height );

			AddLabel( 100, 10, 0, XmlPoints.GetText(from, 200610)); // "Team LMS Challenge"
			AddLabel( 20, 30, 0, String.Format(XmlPoints.GetText(from, 200501), gauntlet.Challenger.Name));  // "Organized by: {0}"
			AddLabel( 20, 50, 0, String.Format(XmlPoints.GetText(from, 200502), m_gauntlet.EntryFee));      // "Entry Fee: {0}"
			AddLabel( 20, 70, 0, String.Format(XmlPoints.GetText(from, 200503), m_gauntlet.ArenaSize)); // "Arena Size: {0}"

			AddImageTiled( 15, 110, 320, 20, 0xdb3 );


			// display all of the current team members
			if(gauntlet.Participants != null)
			{
			    // copy the master list to a temporary working list
			    m_WorkingList = (ArrayList)gauntlet.Participants.Clone();

                AddLabel( 150, 50, 0, String.Format(XmlPoints.GetText(from, 200504), m_WorkingList.Count*m_gauntlet.EntryFee)); // "Total Purse: {0}"

                AddLabel( 150, 70, 0, String.Format(XmlPoints.GetText(from, 200505), m_gauntlet.Location, m_gauntlet.Map));  // "Loc: {0} {1}"

                AddLabel( 20, 90, 0, String.Format(XmlPoints.GetText(from, 200506), gauntlet.Participants.Count));   // "Players: {0}"
                
                AddLabel( 150, 90, 0, String.Format(XmlPoints.GetText(from, 200507), gauntlet.ActivePlayers()));  // "Active: {0}"

                int yoffset = 135;

                // page up and down buttons
                AddButton( 300, 110, 0x15E0, 0x15E4, 13, GumpButtonType.Reply, 0 );
                AddButton( 320, 110, 0x15E2, 0x15E6, 12, GumpButtonType.Reply, 0 );


                // find the players entry to determine the viewing page
                for(int i = 0;i<m_WorkingList.Count;i++)
                {

                    TeamLMSGauntlet.ChallengeEntry entry = (TeamLMSGauntlet.ChallengeEntry)m_WorkingList[i];

                    if(entry == null) continue;

                    if(entry.Participant == from)
                    {
                        viewpage = entry.PageBeingViewed;
                        break;
                    }
                }

                AddLabel( 220, 110, 0, String.Format(XmlPoints.GetText(from, 200508), viewpage+1, (int)(m_WorkingList.Count/PlayersPerPage)+1)); // "Page: {0}/{1}"

                AddLabel( 155, 110, 0, XmlPoints.GetText(from, 200591));  // "Team"

                for(int i = 0;i<m_WorkingList.Count;i++)
                {

                    // determine which page is being viewed

                    if((int)(i/PlayersPerPage) != viewpage) continue;

                    TeamLMSGauntlet.ChallengeEntry entry = (TeamLMSGauntlet.ChallengeEntry)m_WorkingList[i];

                    if(entry == null) continue;

                    // display the entry with a color indicating whether they have accepted or not
                    Mobile m = entry.Participant;

                    string statusmsg = XmlPoints.GetText(from, 200509);   // "Waiting"
                    int texthue = 0;

                    // this section is available during game setup
                    if(!m_gauntlet.GameLocked)
                    {
                        statusmsg = XmlPoints.GetText(from, 200509);  // "Waiting"
                        if(entry.Accepted)
                        {
                            texthue = 68;
                            statusmsg = XmlPoints.GetText(from, 200510);    // "Accepted"
                        }

                        // check to see if they have the Entry fee
                        if(!m_gauntlet.HasEntryFee(m))
                        {
                            texthue = 33;
                            statusmsg = XmlPoints.GetText(from, 200511);   // "Insufficient funds"
                        }


                    // if the game is still open then enable these buttons

                        // if this is the participant then add the accept button to the entry
                        if(m == from)
                        {
                            AddButton( 15, yoffset, entry.Accepted ? 0xd1 : 0xd0, entry.Accepted ? 0xd0 : 0xd1,  1000+i, GumpButtonType.Reply, 0 );
                        }
                        // if this is the organizer then add the kick button and the team assignment to each entry
                        if(from == m_gauntlet.Challenger)
                        {
                            AddImageTiled( 223, yoffset, 20, 19, textentrybackground );
                            AddTextEntry( 225, yoffset, 20, 19, 0, 500+i, entry.Team.ToString());
                        }
                        if(from == m_gauntlet.Challenger || from == entry.Participant)
                        {
                            AddButton( 190, yoffset, 0xFB1, 0xFB3,  2000+i, GumpButtonType.Reply, 0 );

                        }
                    } else
                    {

                        // this section is active after the game has started

                        // enable the forfeit button
                        if(m == from && entry.Status == ChallengeStatus.Active && !m_gauntlet.GameCompleted)
                        {
                            AddButton( 190, yoffset, 0xFB1, 0xFB3,  4000+i, GumpButtonType.Reply, 0 );
                        }

                        if(entry.Status == ChallengeStatus.Forfeit)
                        {
                            texthue = 33;
                            statusmsg = XmlPoints.GetText(from, 200520);  // "Forfeit"
                        } else
                        if(entry.Caution == ChallengeStatus.Hidden && entry.Status == ChallengeStatus.Active)
                        {
                            texthue = 53;
                            statusmsg = XmlPoints.GetText(from, 200521);   // "Hidden"
                        } else
                        if(entry.Caution == ChallengeStatus.OutOfBounds && entry.Status == ChallengeStatus.Active)
                        {
                            texthue = 53;
                            statusmsg = XmlPoints.GetText(from, 200522);  // "Out of Bounds"
                        } else
                        if(entry.Caution == ChallengeStatus.Offline && entry.Status == ChallengeStatus.Active)
                        {
                            texthue = 53;
                            statusmsg = XmlPoints.GetText(from, 200523);    // "Offline"
                        } else
                        if(entry.Status == ChallengeStatus.Active)
                        {

                            texthue = 68;
                            if(gauntlet.Winner == entry.Team)
                                statusmsg = XmlPoints.GetText(from, 200524);      // "Winner"
                            else
                                statusmsg = XmlPoints.GetText(from, 200525);      // "Active"
                        } else
                        if(entry.Status == ChallengeStatus.Dead)
                        {
                            texthue = 33;
                            statusmsg = XmlPoints.GetText(from, 200526);            // "Dead"
                        } else
                        if(entry.Status == ChallengeStatus.Disqualified)
                        {
                            texthue = 33;
                            statusmsg = XmlPoints.GetText(from, 200527);     // "Disqualified"
                        }


                    }

                    if(m != null)
                    {
                        int teamhue = 0;
                        if(entry.Team > 0)
                        {
                            teamhue = BaseChallengeGame.TeamColor(entry.Team);
                        }
                        AddLabel( 40, yoffset, teamhue, m.Name);
                        AddLabel( 165, yoffset, teamhue, entry.Team.ToString());
                        AddLabel( 255, yoffset, texthue, statusmsg);
                    }

                    yoffset += y_inc;
                }
			}


			// the challenger gets additional options
			if(from == gauntlet.Challenger && !m_gauntlet.GameLocked)
			{
			    AddImageTiled( 15, height - 110, 320, 20, 0xdb3 );

                AddButton( 130, height - 35, 0xFA8, 0xFAA, 100, GumpButtonType.Reply, 0 );
                AddLabel( 170, height - 35, 0, XmlPoints.GetText(from, 200528) );      // "Add"

                AddButton( 230, height - 35, 0xFB7, 0xFB9, 300, GumpButtonType.Reply, 0 );
                AddLabel( 270, height - 35, 0, XmlPoints.GetText(from, 200529) );     // "Start"

                // set teams
                AddButton( 230, height - 85, 0xFAE, 0xFAF, 11, GumpButtonType.Reply, 0 );
                AddLabel( 270, height - 85, 0, XmlPoints.GetText(from, 200592));   // "Set Teams"

                // set entry fee
                AddButton( 20, height - 85, 0xFAE, 0xFAF, 10, GumpButtonType.Reply, 0 );
                AddImageTiled( 160, height - 85, 60, 19, textentrybackground );
                AddTextEntry( 160, height - 85, 60, 25, 0, 10 ,m_gauntlet.EntryFee.ToString() );
                AddLabel( 60, height - 85, 0, XmlPoints.GetText(from, 200530) );    // "Set Entry Fee: "

                // set arena size
                AddButton( 20, height - 60, 0xFAE, 0xFAF, 20, GumpButtonType.Reply, 0 );
                AddImageTiled( 160, height - 60, 30, 19, textentrybackground );
                AddTextEntry( 160, height - 60, 30, 25, 0, 20 ,m_gauntlet.ArenaSize.ToString() );
                AddLabel( 60, height - 60, 0, XmlPoints.GetText(from, 200531) );  // "Set Arena Size: "
			} else
			{
                AddImageTiled( 15, height - 60, 320, 20, 0xdb3 );
			}

			AddButton( 20, height - 35, 0xFAB, 0xFAD, 1, GumpButtonType.Reply, 0 );
            AddLabel( 60, height - 35, 0, XmlPoints.GetText(from, 200532) );  // "Refresh"

            if(gauntlet.GameInProgress)
            {
                AddLabel( 150, height - 35, 68, XmlPoints.GetText(from, 200533));  // "Game is in progress!"
            } else
            if(gauntlet.Winner != 0)
            {
                AddLabel( 130, height - 35, 68, String.Format(XmlPoints.GetText(from, 200593), gauntlet.Winner) );   // "Team {0} is the winner!"
            }

			//AddButton( 30, height - 35, 0xFB7, 0xFB9, 0, GumpButtonType.Reply, 0 );
			//AddLabel( 70, height - 35, 0, "Close" );
			
			// display the teams gump
            from.CloseGump(typeof(TeamsGump));
			from.SendGump(new TeamsGump(m_gauntlet, from) );

		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
            if(state == null || info == null || state.Mobile == null || m_gauntlet == null || m_gauntlet.Challenger == null) return;

            XmlPoints afrom = (XmlPoints)XmlAttach.FindAttachment(state.Mobile, typeof(XmlPoints));

			switch ( info.ButtonID )
			{
                case 1:
                    // refresh

                    //m_gauntlet.CheckForDisqualification();

                    state.Mobile.SendGump( new TeamLMSGump( m_gauntlet, state.Mobile, viewpage));
                    break;

                case 10:
                    // Entry fee
                    int val = 0;
                    TextRelay tr = info.GetTextEntry( 10 );
                    if(tr != null)
                    {
                        try{val = int.Parse(tr.Text); } catch{}
                    }
                    m_gauntlet.EntryFee = val;

                    m_gauntlet.ResetAcceptance();

                    // update all the gumps
                    RefreshAllGumps(m_gauntlet, true);

                    state.Mobile.SendGump( new TeamLMSGump( m_gauntlet, state.Mobile, viewpage));
                    break;
                case 11:
                    // update teams
                    if(m_WorkingList != null)
                    for(int i = 0; i < m_WorkingList.Count; i++)
                    {
                        // is this on the visible page?
                        if((int)(i/PlayersPerPage) != viewpage) continue;
                        
                        TeamLMSGauntlet.ChallengeEntry entry = (TeamLMSGauntlet.ChallengeEntry) m_WorkingList[i];
                        if(entry != null)
                        {
                            int tval = 0;
                            tr = info.GetTextEntry( 500+i );
                            if(tr != null)
                            {
                                try{tval = int.Parse(tr.Text); } catch{}
                            }
                            entry.Team = tval;
                        }
                    }

                    m_gauntlet.ResetAcceptance();
                    // update all the gumps
                    RefreshAllGumps(m_gauntlet, true);

                    state.Mobile.SendGump( new TeamLMSGump( m_gauntlet, state.Mobile, viewpage));
                    break;
                case 12:
                    // page up
                    // try doing the default for non-participants
                    int nplayers = 0;
                    if(m_gauntlet.Participants != null)
                        nplayers = m_gauntlet.Participants.Count;

                    int page = viewpage+1;
                    if(page > (int)(nplayers/PlayersPerPage))
                    {
                        page = (int)(nplayers/PlayersPerPage);
                    }

                    if(m_WorkingList != null)
                    for(int i = 0; i < m_WorkingList.Count; i++)
                    {
                        TeamLMSGauntlet.ChallengeEntry entry = (TeamLMSGauntlet.ChallengeEntry) m_WorkingList[i];
                        if(entry != null)
                        {

                            if(entry.Participant == state.Mobile)
                            {
                                entry.PageBeingViewed++;

                                if(entry.PageBeingViewed > (int)(nplayers/PlayersPerPage)) entry.PageBeingViewed=(int)(nplayers/PlayersPerPage);

                                page = entry.PageBeingViewed;
                                //break;

                            }
                        }
                    }

                    state.Mobile.SendGump( new TeamLMSGump( m_gauntlet, state.Mobile, page));
                    break;
                case 13:
                    // page down
                    // try doing the default for non-participants
                    page = viewpage-1;
                    if(page < 0)
                    {
                        page = 0;
                    }
                    if(m_WorkingList != null)
                    for(int i = 0; i < m_WorkingList.Count; i++)
                    {
                        TeamLMSGauntlet.ChallengeEntry entry = (TeamLMSGauntlet.ChallengeEntry) m_WorkingList[i];
                        if(entry != null)
                        {

                            if(entry.Participant == state.Mobile)
                            {
                                entry.PageBeingViewed--;

                                if(entry.PageBeingViewed < 0) entry.PageBeingViewed=0;

                                page = entry.PageBeingViewed;
                                //break;

                            }
                        }
                    }

                    state.Mobile.SendGump( new TeamLMSGump( m_gauntlet, state.Mobile, page));
                    break;

                case 20:
                    // arena size
                    val = 0;
                    tr = info.GetTextEntry( 20 );
                    if(tr != null)
                    {
                        try{val = int.Parse(tr.Text); } catch{}
                    }
                    m_gauntlet.ArenaSize = val;

                    m_gauntlet.ResetAcceptance();

                    // update all the gumps
                    RefreshAllGumps(m_gauntlet, true);

                    state.Mobile.SendGump( new TeamLMSGump( m_gauntlet, state.Mobile, viewpage));
                    break;


                case 100:

                    // add to Participants
                    if(m_gauntlet.Participants == null)
                        m_gauntlet.Participants = new ArrayList();

                    if(MaxTeamSize > 0 && m_gauntlet.Participants.Count >= MaxTeamSize)
                    {
                        XmlPoints.SendText(state.Mobile, 100535);     // "Challenge is full!"
                    } else
                    {
                        state.Mobile.Target = new MemberTarget(m_gauntlet, m_gauntlet.Participants);
                    }


                    state.Mobile.SendGump( new TeamLMSGump( m_gauntlet, state.Mobile, viewpage));
                    break;

                case 300:
                    // Start game
                    if(m_WorkingList == null) return;

                    bool complete = true;
                    foreach(TeamLMSGauntlet.ChallengeEntry entry in m_WorkingList)
                    {
                        if(entry != null)
                        {
                            Mobile m = entry.Participant;

                            if(m == null) continue;

                            if(!m_gauntlet.CheckQualify(m))
                            {
                                complete = false;
                                break;
                            }

                            if(!entry.Accepted)
                            {
                                XmlPoints.SendText(m_gauntlet.Challenger, 100539, m.Name);  // "{0} has not accepted yet."
                                complete = false;
                                break;
                            }
                            
                            // and they have a team
                            if(entry.Team <= 0)
                            {
                                XmlPoints.SendText(m_gauntlet.Challenger, 100594, m.Name); // "{0} has not been assigned a team."
                                complete = false;
                            }
                        }
                    }

                    if(m_WorkingList.Count < 2)
                    {
                        XmlPoints.SendText(m_gauntlet.Challenger, 100540);  // "Insufficient number of players."
                        complete = false;
                    }
                    // copy all of the accepted entries to the final participants list


                    if(complete)
                    {
                        m_gauntlet.Participants = new ArrayList();
    
                        foreach(TeamLMSGauntlet.ChallengeEntry entry in m_WorkingList)
                        {
                            if(entry != null)
                            {
                                Mobile m = entry.Participant;

                                if(m == null) continue;
    
                                // try to collect any entry fee
                                if(!m_gauntlet.CollectEntryFee(m, m_gauntlet.EntryFee))
                                    continue;

                                // set up the challenge on each player
                                XmlPoints a = (XmlPoints)XmlAttach.FindAttachment(m, typeof(XmlPoints));
                                if(a != null)
                                {
                                    a.ChallengeGame = m_gauntlet;
                                }

                                entry.Status = ChallengeStatus.Active;
    
                                m_gauntlet.Participants.Add(entry);
    
                            }
                        }

                        // and lock the game
                        m_gauntlet.StartGame();

                        // refresh all gumps
                        RefreshAllGumps(m_gauntlet, true);
                    } else
                    {
                        state.Mobile.SendGump( new TeamLMSGump( m_gauntlet, state.Mobile, viewpage));
                    }

                    break;

				default:
				{
				    // forfeit buttons
				    if(info.ButtonID >= 4000)
                    {
                        int selection = info.ButtonID - 4000;

                        if(selection < m_WorkingList.Count)
                        {
                            TeamLMSGauntlet.ChallengeEntry entry = (TeamLMSGauntlet.ChallengeEntry)m_WorkingList[selection];

                            // find the master participants list entry with the same participant
                            if(m_gauntlet.Participants != null)
                            {
                                TeamLMSGauntlet.ChallengeEntry forfeitentry = null;

                                foreach(TeamLMSGauntlet.ChallengeEntry masterentry in m_gauntlet.Participants)
                                {
                                    if(entry == masterentry)
                                    {
                                        forfeitentry = masterentry;
                                        break;
                                    }
                                }

                                // and remove it
                                if(forfeitentry != null)
                                {
                                    forfeitentry.Status = ChallengeStatus.Forfeit;

                                    // inform him that he has been kicked
                                    m_gauntlet.Forfeit(forfeitentry.Participant);
                                }
                            }
                        }
                    }
				    // kick buttons
				    if(info.ButtonID >= 2000)
                    {
                        int selection = info.ButtonID - 2000;

                        if(selection < m_WorkingList.Count)
                        {
                            TeamLMSGauntlet.ChallengeEntry entry = (TeamLMSGauntlet.ChallengeEntry)m_WorkingList[selection];
                            // find the master participants list entry with the same participant
                            if(m_gauntlet.Participants != null)
                            {
                                TeamLMSGauntlet.ChallengeEntry kickentry = null;

                                foreach(TeamLMSGauntlet.ChallengeEntry masterentry in m_gauntlet.Participants)
                                {
                                    if(entry == masterentry)
                                    {
                                        kickentry = masterentry;
                                        break;
                                    }
                                }

                                // and remove it
                                if(kickentry != null)
                                {
                                    m_gauntlet.Participants.Remove(kickentry);
                                    
                                    // refresh his gump and inform him that he has been kicked
                                    if(kickentry.Participant != null)
                                    {
                                        XmlPoints.SendText(kickentry.Participant, 100545, m_gauntlet.ChallengeName); // "You have been kicked from {0}"
                                        kickentry.Participant.SendGump( new TeamLMSGump( m_gauntlet, kickentry.Participant, viewpage));
                                    }
                                }
                            }

                            m_gauntlet.ResetAcceptance();
                        }

                        // refresh all gumps
                        RefreshAllGumps(m_gauntlet, true);
                        //state.Mobile.SendGump( new TeamLMSGump( m_gauntlet, state.Mobile));
                    } else
				    // accept buttons
                    if(info.ButtonID >= 1000)
                    {
                        int selection = info.ButtonID - 1000;
                        // set the acceptance flag of the participant
                        if(selection < m_WorkingList.Count)
                        {
                            TeamLMSGauntlet.ChallengeEntry entry = (TeamLMSGauntlet.ChallengeEntry)m_WorkingList[selection];

                            entry.Accepted = !entry.Accepted;
                        }

                        // refresh all gumps
                        RefreshAllGumps(m_gauntlet, true);

                        //state.Mobile.SendGump( new TeamLMSGump( m_gauntlet, state.Mobile));
                    }


				    break;
				}
			}
		}
		
		public static void RefreshAllGumps( TeamLMSGauntlet gauntlet, bool force)
		{
            if(gauntlet.Participants != null)
            {
                foreach(TeamLMSGauntlet.ChallengeEntry entry in gauntlet.Participants)
                {
                    if(entry.Participant != null)
                    {
                        if(force || entry.Participant.HasGump( typeof( TeamLMSGump )))
                        {
                            entry.Participant.SendGump( new TeamLMSGump( gauntlet, entry.Participant));
                        }
                    }
                }
            }
            
            // update for the organizer
            if(gauntlet.Challenger != null)
            {
                if(force || gauntlet.Challenger.HasGump( typeof( TeamLMSGump )))
                {
                    gauntlet.Challenger.SendGump( new TeamLMSGump( gauntlet, gauntlet.Challenger));
                }
            }
        }


		private class MemberTarget : Target
        {
            private ArrayList m_list;
            private TeamLMSGauntlet m_gauntlet;


            public MemberTarget(TeamLMSGauntlet gauntlet, ArrayList list) :  base ( 30, false, TargetFlags.None )
            {
                m_list = list;
                m_gauntlet = gauntlet;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if(from == null || targeted == null || m_gauntlet == null || m_list == null) return;

                if(targeted is Mobile && ((Mobile)targeted).Player)
                {
                    Mobile pm = targeted as Mobile;
                    
                    // make sure they qualify
                    if(!m_gauntlet.CheckQualify(pm))
                    {
                        return;
                    }

                    // see if they are already in the game
                    if(m_gauntlet.Participants != null)
                    {
                        foreach(TeamLMSGauntlet.ChallengeEntry entry in m_gauntlet.Participants)
                        {
                            if(pm == entry.Participant)
                            {
                                XmlPoints.SendText(from, 100548, pm.Name);  // "{0} has already been added to the game."
                                return;
                            }
                        }
                    }

                    m_list.Add(new TeamLMSGauntlet.ChallengeEntry(pm));

                    XmlPoints.SendText(from, 100549, pm.Name);   // "You have added {0} to the challenge."

                    XmlPoints.SendText(pm, 100550, m_gauntlet.Name); // "You have been invited to participate in {0}."

                    // clear all acceptances
                    m_gauntlet.ResetAcceptance();

                    // refresh all gumps
                    RefreshAllGumps(m_gauntlet, true);

                    // refresh the gump with the new member
                    //from.SendGump( new TeamLMSGump( m_gauntlet, from));
                }
            }
        }
        

        public class TeamsGump : Gump
        {

            public TeamsGump(TeamLMSGauntlet gauntlet, Mobile from) : base(350,30)
            { 
               if(gauntlet == null || from == null) return;

                int yinc = 25;

                ArrayList Teams = gauntlet.GetTeams();

                // gump height determined by number of teams
                int height  = Teams.Count*yinc + 80;

                AddBackground( 0, 0, 210, height, 0xDAC );
    			//AddAlphaRegion( 0, 0, 340, height );

    			AddLabel( 50, 10, 0, XmlPoints.GetText(from, 200611));  // "LMS Team Status"

    			AddLabel( 20, 40, 0, XmlPoints.GetText(from, 200591));    // "Team"
    			AddLabel( 75, 40, 0, XmlPoints.GetText(from, 200596));    // "Members"
    			AddLabel( 135, 40, 0, XmlPoints.GetText(from, 200597));    // "Active"

    			int yoffset = 60;
    			// list all of the teams and their status
    			foreach(TeamInfo t in Teams)
    			{
                    int teamhue = 0;
                    if(t.ID > 0)
                    {
                        teamhue = BaseChallengeGame.TeamColor(t.ID);
                    }
        			AddLabel( 20, yoffset, teamhue, t.ID.ToString());
        			AddLabel( 75, yoffset, teamhue, t.Members.Count.ToString());
        			AddLabel( 135, yoffset, teamhue, t.NActive.ToString());
    
        			yoffset += yinc;
    			}
            }
        }
	}
}
