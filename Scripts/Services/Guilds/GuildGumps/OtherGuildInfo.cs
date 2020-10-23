using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Guilds
{
    public class OtherGuildInfo : BaseGuildGump
    {
        private readonly Guild m_Other;
        public OtherGuildInfo(PlayerMobile pm, Guild g, Guild otherGuild)
            : base(pm, g, 10, 40)
        {
            m_Other = otherGuild;

            g.CheckExpiredWars();

            PopulateGump();
        }

        public void AddButtonAndBackground(int x, int y, int buttonID, int locNum)
        {
            AddBackground(x, y, 225, 26, 0x2486);
            AddButton(x + 5, y + 5, 0x845, 0x846, buttonID, GumpButtonType.Reply, 0);
            AddHtmlLocalized(x + 30, y + 3, 185, 26, locNum, 0x0, false, false);
        }

        public override void PopulateGump()
        {
            Guild g = Guild.GetAllianceLeader(guild);
            Guild other = Guild.GetAllianceLeader(m_Other);

            WarDeclaration war = g.FindPendingWar(other);
            WarDeclaration activeWar = g.FindActiveWar(other);

            AllianceInfo alliance = guild.Alliance;
            AllianceInfo otherAlliance = m_Other.Alliance;
            //NOTE TO SELF: Only only alliance leader can see pending guild alliance statuses

            bool PendingWar = (war != null);
            bool ActiveWar = (activeWar != null);
            AddPage(0);

            AddBackground(0, 0, 520, 335, 0x242C);
            AddHtmlLocalized(20, 15, 480, 26, 1062975, 0x0, false, false); // <div align=center><i>Guild Relationship</i></div>
            AddImageTiled(20, 40, 480, 2, 0x2711);
            AddHtmlLocalized(20, 50, 120, 26, 1062954, 0x0, true, false); // <i>Guild Name</i>
            AddHtml(150, 53, 360, 26, m_Other.Name, false, false);

            AddHtmlLocalized(20, 80, 120, 26, 1063025, 0x0, true, false); // <i>Alliance</i>

            if (otherAlliance != null)
            {
                if (otherAlliance.IsMember(m_Other))
                {
                    AddHtml(150, 83, 360, 26, otherAlliance.Name, false, false);
                }
                //else if( otherAlliance.Leader == guild && ( otherAlliance.IsPendingMember( m_Other ) || otherAlliance.IsPendingMember( guild ) ) )
                /*		else if( (otherAlliance.Leader == guild && otherAlliance.IsPendingMember( m_Other ) ) || ( otherAlliance.Leader == m_Other && otherAlliance.IsPendingMember( guild ) ) )
                {
                AddHtml( 150, 83, 360, 26, Color( alliance.Name, 0xF), false, false );
                }
                //AddHtml( 150, 83, 360, 26, ( alliance.PendingMembers.Contains( guild ) || alliance.PendingMembers.Contains( m_Other ) ) ? String.Format( "<basefont color=#blue>{0}</basefont>", alliance.Name ) : alliance.Name, false, false );
                //AddHtml( 150, 83, 360, 26, ( otherAlliance == alliance &&  otherAlliance.PendingMembers.Contains( guild ) || otherAlliance.PendingMembers.Contains( m_Other ) ) ? String.Format( "<basefont color=#blue>{0}</basefont>", otherAlliance.Name ) : otherAlliance.Name, false, false );
                */
            }

            AddHtmlLocalized(20, 110, 120, 26, 1063139, 0x0, true, false); // <i>Abbreviation</i>
            AddHtml(150, 113, 120, 26, m_Other.Abbreviation, false, false);

            string kills = "0/0";
            string time = "00:00";
            string otherKills = "0/0";

            WarDeclaration otherWar;

            if (ActiveWar)
            {
                kills = string.Format("{0}/{1}", activeWar.Kills, activeWar.MaxKills);

                TimeSpan timeRemaining = TimeSpan.Zero;

                if (activeWar.WarLength != TimeSpan.Zero && (activeWar.WarBeginning + activeWar.WarLength) > DateTime.UtcNow)
                    timeRemaining = (activeWar.WarBeginning + activeWar.WarLength) - DateTime.UtcNow;

                //time = String.Format( "{0:D2}:{1:D2}", timeRemaining.Hours.ToString(), timeRemaining.Subtract( TimeSpan.FromHours( timeRemaining.Hours ) ).Minutes );	//Is there a formatter for htis? it's 2AM and I'm tired and can't find it
                time = string.Format("{0:D2}:{1:mm}", timeRemaining.Hours, DateTime.MinValue + timeRemaining);

                otherWar = m_Other.FindActiveWar(guild);
                if (otherWar != null)
                    otherKills = string.Format("{0}/{1}", otherWar.Kills, otherWar.MaxKills);
            }
            else if (PendingWar)
            {
                kills = Color(string.Format("{0}/{1}", war.Kills, war.MaxKills), 0x990000);
                //time = Color( String.Format( "{0}:{1}", war.WarLength.Hours, ((TimeSpan)(war.WarLength - TimeSpan.FromHours( war.WarLength.Hours ))).Minutes ), 0xFF0000 );
                time = Color(string.Format("{0:D2}:{1:mm}", war.WarLength.Hours, DateTime.MinValue + war.WarLength), 0x990000);

                otherWar = m_Other.FindPendingWar(guild);
                if (otherWar != null)
                    otherKills = Color(string.Format("{0}/{1}", otherWar.Kills, otherWar.MaxKills), 0x990000);
            }

            AddHtmlLocalized(280, 110, 120, 26, 1062966, 0x0, true, false); // <i>Your Kills</i>
            AddHtml(410, 113, 120, 26, kills, false, false);

            AddHtmlLocalized(20, 140, 120, 26, 1062968, 0x0, true, false); // <i>Time Remaining</i>
            AddHtml(150, 143, 120, 26, time, false, false);

            AddHtmlLocalized(280, 140, 120, 26, 1062967, 0x0, true, false); // <i>Their Kills</i>
            AddHtml(410, 143, 120, 26, otherKills, false, false);

            AddImageTiled(20, 172, 480, 2, 0x2711);

            int number = 1062973;// <div align=center>You are at peace with this guild.</div>

            if (PendingWar)
            {
                if (war.WarRequester)
                {
                    number = 1063027; // <div align=center>You have challenged this guild to war!</div>
                }
                else
                {
                    number = 1062969; // <div align=center>This guild has challenged you to war!</div>

                    AddButtonAndBackground(20, 260, 5, 1062981); // Accept Challenge
                    AddButtonAndBackground(275, 260, 6, 1062983); //Modify Terms
                }

                AddButtonAndBackground(20, 290, 7, 1062982); // Dismiss Challenge
            }
            else if (ActiveWar)
            {
                number = 1062965; // <div align=center>You are at war with this guild!</div>
                AddButtonAndBackground(20, 290, 8, 1062980); // Surrender
            }
            else if (alliance != null && alliance == otherAlliance) //alliance, Same Alliance
            {
                if (alliance.IsMember(guild) && alliance.IsMember(m_Other))	//Both in Same alliance, full members
                {
                    number = 1062970; // <div align=center>You are allied with this guild.</div>

                    if (alliance.Leader == guild)
                    {
                        AddButtonAndBackground(20, 260, 12, 1062984); // Remove Guild from Alliance
                        AddButtonAndBackground(275, 260, 13, 1063433); // Promote to Alliance Leader	//Note: No 'confirmation' like the other leader guild promotion things
                        //Remove guild from alliance	//Promote to Alliance Leader
                    }

                    //Show roster, Centered, up
                    AddButtonAndBackground(148, 215, 10, 1063164); //Show Alliance Roster
                    //Leave Alliance
                    AddButtonAndBackground(20, 290, 11, 1062985); // Leave Alliance
                }
                else if (alliance.Leader == guild && alliance.IsPendingMember(m_Other))
                {
                    number = 1062971; // <div align=center>You have requested an alliance with this guild.</div>

                    //Show Alliance Roster, Centered, down.
                    AddButtonAndBackground(148, 245, 10, 1063164); //Show Alliance Roster
                    //Withdraw Request
                    AddButtonAndBackground(20, 290, 14, 1062986); // Withdraw Request

                    AddHtml(150, 83, 360, 26, Color(alliance.Name, 0x99), false, false);
                }
                else if (alliance.Leader == m_Other && alliance.IsPendingMember(guild))
                {
                    number = 1062972; // <div align=center>This guild has requested an alliance.</div>

                    //Show alliance Roster, top
                    AddButtonAndBackground(148, 215, 10, 1063164); //Show Alliance Roster
                    //Deny Request
                    //Accept Request
                    AddButtonAndBackground(20, 260, 15, 1062988); // Deny Request
                    AddButtonAndBackground(20, 290, 16, 1062987); // Accept Request

                    AddHtml(150, 83, 360, 26, Color(alliance.Name, 0x99), false, false);
                }
            }
            else
            {
                AddButtonAndBackground(20, 260, 2, 1062990); // Request Alliance 
                AddButtonAndBackground(20, 290, 1, 1062989); // Declare War!
            }

            AddButtonAndBackground(275, 290, 0, 3000091); //Cancel

            AddHtmlLocalized(20, 180, 480, 30, number, 0x0, true, false);
            AddImageTiled(20, 245, 480, 2, 0x2711);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile pm = sender.Mobile as PlayerMobile;

            if (!IsMember(pm, guild))
                return;

            RankDefinition playerRank = pm.GuildRank;

            Guild guildLeader = Guild.GetAllianceLeader(guild);
            Guild otherGuild = Guild.GetAllianceLeader(m_Other);

            WarDeclaration war = guildLeader.FindPendingWar(otherGuild);
            WarDeclaration activeWar = guildLeader.FindActiveWar(otherGuild);
            WarDeclaration otherWar = otherGuild.FindPendingWar(guildLeader);

            AllianceInfo alliance = guild.Alliance;
            AllianceInfo otherAlliance = otherGuild.Alliance;

            switch (info.ButtonID)
            {
                #region War
                case 5:	//Accept the war
                    {
                        if (war != null && !war.WarRequester && activeWar == null)
                        {
                            if (!playerRank.GetFlag(RankFlags.ControlWarStatus))
                            {
                                pm.SendLocalizedMessage(1063440); // You don't have permission to negotiate wars.
                            }
                            else if (alliance != null && alliance.Leader != guild)
                            {
                                pm.SendLocalizedMessage(1063239, string.Format("{0}\t{1}", guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                                pm.SendLocalizedMessage(1070707, alliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                            }
                            else
                            {
                                //Accept the war
                                guild.PendingWars.Remove(war);
                                war.WarBeginning = DateTime.UtcNow;
                                guild.AcceptedWars.Add(war);

                                if (alliance != null && alliance.IsMember(guild))
                                {
                                    alliance.AllianceMessage(1070769, ((otherAlliance != null) ? otherAlliance.Name : otherGuild.Name)); // Guild Message: Your guild is now at war with ~1_GUILDNAME~
                                    alliance.InvalidateMemberProperties();
                                }
                                else
                                {
                                    guild.GuildMessage(1070769, ((otherAlliance != null) ? otherAlliance.Name : otherGuild.Name)); // Guild Message: Your guild is now at war with ~1_GUILDNAME~
                                    guild.InvalidateMemberProperties();
                                }
                                //Technically  SHOULD say Your guild is now at war w/out any info, intentional diff.

                                otherGuild.PendingWars.Remove(otherWar);
                                otherWar.WarBeginning = DateTime.UtcNow;
                                otherGuild.AcceptedWars.Add(otherWar);

                                if (otherAlliance != null && m_Other.Alliance.IsMember(m_Other))
                                {
                                    otherAlliance.AllianceMessage(1070769, ((alliance != null) ? alliance.Name : guild.Name)); // Guild Message: Your guild is now at war with ~1_GUILDNAME~
                                    otherAlliance.InvalidateMemberProperties();
                                }
                                else
                                {
                                    otherGuild.GuildMessage(1070769, ((alliance != null) ? alliance.Name : guild.Name)); // Guild Message: Your guild is now at war with ~1_GUILDNAME~
                                    otherGuild.InvalidateMemberProperties();
                                }
                            }
                        }

                        break;
                    }
                case 6:	//Modify war terms
                    {
                        if (war != null && !war.WarRequester && activeWar == null)
                        {
                            if (!playerRank.GetFlag(RankFlags.ControlWarStatus))
                            {
                                pm.SendLocalizedMessage(1063440); // You don't have permission to negotiate wars.
                            }
                            else if (alliance != null && alliance.Leader != guild)
                            {
                                pm.SendLocalizedMessage(1063239, string.Format("{0}\t{1}", guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                                pm.SendLocalizedMessage(1070707, alliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                            }
                            else
                            {
                                pm.SendGump(new WarDeclarationGump(pm, guild, otherGuild));
                            }
                        }
                        break;
                    }
                case 7:	//Dismiss war
                    {
                        if (war != null)
                        {
                            if (!playerRank.GetFlag(RankFlags.ControlWarStatus))
                            {
                                pm.SendLocalizedMessage(1063440); // You don't have permission to negotiate wars.
                            }
                            else if (alliance != null && alliance.Leader != guild)
                            {
                                pm.SendLocalizedMessage(1063239, string.Format("{0}\t{1}", guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                                pm.SendLocalizedMessage(1070707, alliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                            }
                            else
                            {
                                //Dismiss the war
                                guild.PendingWars.Remove(war);
                                otherGuild.PendingWars.Remove(otherWar);
                                pm.SendLocalizedMessage(1070752); // The proposal has been updated.
                                //Messages to opposing guild? (Testing on OSI says no)
                            }
                        }
                        break;
                    }
                case 8:	//Surrender
                    {
                        if (!playerRank.GetFlag(RankFlags.ControlWarStatus))
                        {
                            pm.SendLocalizedMessage(1063440); // You don't have permission to negotiate wars.
                        }
                        else if (alliance != null && alliance.Leader != guild)
                        {
                            pm.SendLocalizedMessage(1063239, string.Format("{0}\t{1}", guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                            pm.SendLocalizedMessage(1070707, alliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                        }
                        else
                        {
                            if (activeWar != null)
                            {
                                if (alliance != null && alliance.IsMember(guild))
                                {
                                    alliance.AllianceMessage(1070740, ((otherAlliance != null) ? otherAlliance.Name : otherGuild.Name));// You have lost the war with ~1_val~.
                                    alliance.InvalidateMemberProperties();
                                }
                                else
                                {
                                    guild.GuildMessage(1070740, ((otherAlliance != null) ? otherAlliance.Name : otherGuild.Name));// You have lost the war with ~1_val~.
                                    guild.InvalidateMemberProperties();
                                }

                                guild.AcceptedWars.Remove(activeWar);

                                if (otherAlliance != null && otherAlliance.IsMember(otherGuild))
                                {
                                    otherAlliance.AllianceMessage(1070739, ((guild.Alliance != null) ? guild.Alliance.Name : guild.Name));// You have won the war against ~1_val~!
                                    otherAlliance.InvalidateMemberProperties();
                                }
                                else
                                {
                                    otherGuild.GuildMessage(1070739, ((guild.Alliance != null) ? guild.Alliance.Name : guild.Name));// You have won the war against ~1_val~!
                                    otherGuild.InvalidateMemberProperties();
                                }

                                otherGuild.AcceptedWars.Remove(otherGuild.FindActiveWar(guild));
                            }
                        }
                        break;
                    }
                case 1:	//Declare War
                    {
                        if (war == null && activeWar == null)
                        {
                            if (!playerRank.GetFlag(RankFlags.ControlWarStatus))
                            {
                                pm.SendLocalizedMessage(1063440); // You don't have permission to negotiate wars.
                            }
                            else if (alliance != null && alliance.Leader != guild)
                            {
                                pm.SendLocalizedMessage(1063239, string.Format("{0}\t{1}", guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                                pm.SendLocalizedMessage(1070707, alliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                            }
                            else if (otherAlliance != null && otherAlliance.Leader != m_Other)
                            {
                                pm.SendLocalizedMessage(1063239, string.Format("{0}\t{1}", m_Other.Name, otherAlliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                                pm.SendLocalizedMessage(1070707, otherAlliance.Leader.Name); // You need to negotiate via ~1_val~ instead.
                            }
                            else
                            {
                                pm.SendGump(new WarDeclarationGump(pm, guild, m_Other));
                            }
                        }
                        break;
                    }
                #endregion
                case 2:	//Request Alliance
                    {
                        #region New alliance
                        if (alliance == null)
                        {
                            if (!playerRank.GetFlag(RankFlags.AllianceControl))
                            {
                                pm.SendLocalizedMessage(1070747); // You don't have permission to create an alliance.
                            }
                            else if (otherAlliance != null)
                            {
                                // ~1_val~ is currently considering another alliance proposal. | // ~1_val~ already belongs to an alliance.
                                pm.SendLocalizedMessage(otherAlliance.IsPendingMember(m_Other) ? 1063416 : 1063426, m_Other.Name);
                            }
                            else if (m_Other.AcceptedWars.Count > 0 || m_Other.PendingWars.Count > 0)
                            {
                                pm.SendLocalizedMessage(1063427, m_Other.Name); // ~1_val~ is currently involved in a guild war.
                            }
                            else if (guild.AcceptedWars.Count > 0 || guild.PendingWars.Count > 0)
                            {
                                pm.SendLocalizedMessage(1063427, guild.Name); // ~1_val~ is currently involved in a guild war.
                            }
                            else
                            {
                                pm.SendLocalizedMessage(1063439); // Enter a name for the new alliance:
                                pm.BeginPrompt(CreateAlliance_Callback);
                            }
                        }
                        #endregion
                        #region Existing Alliance
                        else
                        {
                            if (!playerRank.GetFlag(RankFlags.AllianceControl))
                            {
                                pm.SendLocalizedMessage(1063436); // You don't have permission to negotiate an alliance.
                            }
                            else if (alliance.Leader != guild)
                            {
                                pm.SendLocalizedMessage(1063239, string.Format("{0}\t{1}", guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                            }
                            else if (otherAlliance != null)
                            {
                                // ~1_val~ is currently considering another alliance proposal. | // ~1_val~ already belongs to an alliance.
                                pm.SendLocalizedMessage(otherAlliance.IsPendingMember(m_Other) ? 1063416 : 1063426, m_Other.Name);
                            }
                            else if (alliance.IsPendingMember(guild))
                            {
                                pm.SendLocalizedMessage(1063416, guild.Name); // ~1_val~ is currently considering another alliance proposal.
                            }
                            else if (m_Other.AcceptedWars.Count > 0 || m_Other.PendingWars.Count > 0)
                            {
                                pm.SendLocalizedMessage(1063427, m_Other.Name); // ~1_val~ is currently involved in a guild war.
                            }
                            else if (guild.AcceptedWars.Count > 0 || guild.PendingWars.Count > 0)
                            {
                                pm.SendLocalizedMessage(1063427, guild.Name); // ~1_val~ is currently involved in a guild war.
                            }
                            else
                            {
                                pm.SendLocalizedMessage(1070750, m_Other.Name); // An invitation to join your alliance has been sent to ~1_val~.

                                m_Other.GuildMessage(1070780, guild.Name); // ~1_val~ has proposed an alliance.

                                m_Other.Alliance = alliance;	//Calls addPendingGuild
                                //alliance.AddPendingGuild( m_Other );
                            }
                        }
                        #endregion
                        break;
                    }
                case 10:	//Show Alliance Roster
                    {
                        if (alliance != null && alliance == otherAlliance)
                            pm.SendGump(new AllianceInfo.AllianceRosterGump(pm, guild, alliance));

                        break;
                    }
                case 11:	//Leave Alliance
                    {
                        if (!playerRank.GetFlag(RankFlags.AllianceControl))
                        {
                            pm.SendLocalizedMessage(1063436); // You don't have permission to negotiate an alliance.
                        }
                        else if (alliance != null && alliance.IsMember(guild))
                        {
                            guild.Alliance = null; //Calls alliance.Removeguild
                                                        //						alliance.RemoveGuild( guild );

                            m_Other.InvalidateWarNotoriety();

                            guild.InvalidateMemberNotoriety();
                        }
                        break;
                    }
                case 12:	//Remove Guild from alliance
                    {
                        if (!playerRank.GetFlag(RankFlags.AllianceControl))
                        {
                            pm.SendLocalizedMessage(1063436); // You don't have permission to negotiate an alliance.
                        }
                        else if (alliance != null && alliance.Leader != guild)
                        {
                            pm.SendLocalizedMessage(1063239, string.Format("{0}\t{1}", guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                        }
                        else if (alliance != null && alliance.IsMember(guild) && alliance.IsMember(m_Other))
                        {
                            m_Other.Alliance = null;

                            m_Other.InvalidateMemberNotoriety();

                            guild.InvalidateWarNotoriety();
                        }
                        break;
                    }
                case 13:	//Promote to Alliance leader
                    {
                        if (!playerRank.GetFlag(RankFlags.AllianceControl))
                        {
                            pm.SendLocalizedMessage(1063436); // You don't have permission to negotiate an alliance.
                        }
                        else if (alliance != null && alliance.Leader != guild)
                        {
                            pm.SendLocalizedMessage(1063239, string.Format("{0}\t{1}", guild.Name, alliance.Name)); // ~1_val~ is not the leader of the ~2_val~ alliance.
                        }
                        else if (alliance != null && alliance.IsMember(guild) && alliance.IsMember(m_Other))
                        {
                            pm.SendLocalizedMessage(1063434, string.Format("{0}\t{1}", m_Other.Name, alliance.Name)); // ~1_val~ is now the leader of ~2_val~.

                            alliance.Leader = m_Other;
                        }
                        break;
                    }
                case 14:	//Withdraw Request
                    {
                        if (!playerRank.GetFlag(RankFlags.AllianceControl))
                        {
                            pm.SendLocalizedMessage(1063436); // You don't have permission to negotiate an alliance.
                        }
                        else if (alliance != null && alliance.Leader == guild && alliance.IsPendingMember(m_Other))
                        {
                            m_Other.Alliance = null;
                            pm.SendLocalizedMessage(1070752); // The proposal has been updated.
                        }
                        break;
                    }
                case 15: //Deny Alliance Request
                    {
                        if (!playerRank.GetFlag(RankFlags.AllianceControl))
                        {
                            pm.SendLocalizedMessage(1063436); // You don't have permission to negotiate an alliance.
                        }
                        else if (alliance != null && otherAlliance != null && alliance.Leader == m_Other && otherAlliance.IsPendingMember(guild))
                        {
                            pm.SendLocalizedMessage(1070752); // The proposal has been updated.
                            //m_Other.GuildMessage( 1070782 ); // ~1_val~ has responded to your proposal.	//Per OSI commented out.

                            guild.Alliance = null;
                        }
                        break;
                    }
                case 16: //Accept Alliance Request
                    {
                        if (!playerRank.GetFlag(RankFlags.AllianceControl))
                        {
                            pm.SendLocalizedMessage(1063436); // You don't have permission to negotiate an alliance.
                        }
                        else if (otherAlliance != null && otherAlliance.Leader == m_Other && otherAlliance.IsPendingMember(guild))
                        {
                            pm.SendLocalizedMessage(1070752); // The proposal has been updated.

                            otherAlliance.TurnToMember(m_Other); //No need to verify it's in the guild or already a member, the function does this

                            otherAlliance.TurnToMember(guild);
                        }
                        break;
                    }
            }
        }

        public void CreateAlliance_Callback(Mobile from, string text)
        {
            PlayerMobile pm = from as PlayerMobile;

            AllianceInfo alliance = guild.Alliance;
            AllianceInfo otherAlliance = m_Other.Alliance;

            if (!IsMember(from, guild) || alliance != null)
                return;

            RankDefinition playerRank = pm.GuildRank;

            if (!playerRank.GetFlag(RankFlags.AllianceControl))
            {
                pm.SendLocalizedMessage(1070747); // You don't have permission to create an alliance.
            }
            else if (otherAlliance != null)
            {
                // ~1_val~ is currently considering another alliance proposal. | // ~1_val~ already belongs to an alliance.
                pm.SendLocalizedMessage(otherAlliance.IsPendingMember(m_Other) ? 1063416 : 1063426, m_Other.Name);
            }
            else if (m_Other.AcceptedWars.Count > 0 || m_Other.PendingWars.Count > 0)
            {
                pm.SendLocalizedMessage(1063427, m_Other.Name); // ~1_val~ is currently involved in a guild war.
            }
            else if (guild.AcceptedWars.Count > 0 || guild.PendingWars.Count > 0)
            {
                pm.SendLocalizedMessage(1063427, guild.Name); // ~1_val~ is currently involved in a guild war.
            }
            else
            {
                string name = Utility.FixHtml(text.Trim());

                if (!CheckProfanity(name))
                    pm.SendLocalizedMessage(1070886); // That alliance name is not allowed.
                else if (name.Length > Guild.NameLimit)
                    pm.SendLocalizedMessage(1070887, Guild.NameLimit.ToString()); // An alliance name cannot exceed ~1_val~ characters in length.
                else if (AllianceInfo.Alliances.ContainsKey(name.ToLower()))
                    pm.SendLocalizedMessage(1063428); // That alliance name is not available.
                else
                {
                    pm.SendLocalizedMessage(1070750, m_Other.Name); // An invitation to join your alliance has been sent to ~1_val~.

                    m_Other.GuildMessage(1070780, guild.Name); // ~1_val~ has proposed an alliance.

                    new AllianceInfo(guild, name, m_Other);
                }
            }
        }
    }
}
