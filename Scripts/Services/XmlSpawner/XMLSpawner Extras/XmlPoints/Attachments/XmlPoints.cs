#define FACTIONS
using System;
using System.IO;
using System.Xml;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Targeting;
using Server.Gumps;
using System.Text;
using Server.Regions;
using Server.Spells;
using Server.Commands;
using Server.Commands.Generic;

namespace Server.Engines.XmlSpawner2
{

	public class XmlPoints : XmlAttachment
	{
		private const int DefaultStartingPoints = 100;  // 100 default starting points
		
		private int m_Points = DefaultStartingPoints;
		private bool m_Broadcast = true;       // flag that determines whether pvp results will be broadcast.  Broadcast is determined by the killer's flag.
		private int m_Kills;    // cumulative kill count
		private int m_Deaths;   // cumulative death count
		private Mobile m_Challenger;                                // supports 1-on-1 challenge duels
		private BaseChallengeGame  m_ChallengeGame;                      // supports multiplayer challenge games
		private BaseChallengeGame  m_ChallengeSetup;
		private DateTime m_LastDeath = DateTime.MinValue;
		private DateTime m_LastKill = DateTime.MinValue;
		private ArrayList KillList = new ArrayList();
		private int m_Credits = 0;  // 0 default starting credits
		private DateTime m_WhenRanked;
		private int m_Rank;
		private int m_DeltaRank;
		private DateTime m_LastDecay;
		private bool m_ReceiveBroadcasts = true;
		
		public DateTime m_CancelEnd;
		public CancelTimer m_CancelTimer;

		public Point3D m_StartingLoc;
		public Map m_StartingMap;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Points { get{ return m_Points; } set { m_Points = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Rank { get{ return m_Rank; } set { m_Rank = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int DeltaRank { get{ return m_DeltaRank; } set { m_DeltaRank = value; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime WhenRanked { get{ return m_WhenRanked; } set { m_WhenRanked = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Credits { get{ return m_Credits; } set { m_Credits = value; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Broadcast { get{ return m_Broadcast; } set { m_Broadcast = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool ReceiveBroadcasts { get{ return m_ReceiveBroadcasts; } set { m_ReceiveBroadcasts = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Kills { get{ return m_Kills; } set { m_Kills = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Deaths { get{ return m_Deaths; } set { m_Deaths = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime LastKill { get{ return m_LastKill; } }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime LastDeath { get{ return m_LastDeath; } }
		
		public bool HasChallenge { get{ return ((m_Challenger != null && !m_Challenger.Deleted)|| (m_ChallengeGame != null && !m_ChallengeGame.Deleted)); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Challenger { get{ return m_Challenger; } set { m_Challenger = value; } }
		
		public BaseChallengeGame ChallengeGame { get{ return m_ChallengeGame; } set { m_ChallengeGame = value; } }

		public BaseChallengeGame ChallengeSetup { get{ return m_ChallengeSetup; } set { m_ChallengeSetup = value; } }

		public static bool TeleportOnDuel = true;                          // are players automatically teleported to and from the specified dueling location

		private static TimeSpan m_DeathDelay = TimeSpan.FromSeconds(60);    // 60 seconds default min time between deaths for point loss
		private static TimeSpan m_KillDelay = TimeSpan.FromHours(6);       // 6 hour default min interval between kills of the same player for point gain

		// set these scalings to determine points gained/lost based on the difference in points between the killer and the killed
		// default is set to 5% of the point difference (0.05).  Note, regardless of the scaling at least 1 point will be gained/lost per kill
		private static double m_WinScale = 0.05;                               // set to zero for no scaling for points gained for killing (fixed 1 point per kill)
		private static double m_LoseScale = 0.05;                              // set to zero for no scaling for points lost when killed (fixed 1 point per death)
		private static double m_CreditScale = 0.05;

		// admin control of pvp-kill broadcasts. If false then no broadcasting. If true, then broadcasting is controlled by the player settings
		private static bool m_SystemBroadcast = true;

		private static TimeSpan m_LeaderboardSaveInterval = TimeSpan.FromMinutes(15);   // default time interval between leaderboard saves
		private static string m_LeaderboardSaveDirectory = "Leaderboard";     // default directory for saving leaderboard xml information
		private static int m_LeaderboardSaveRanks = 20;               // number of ranked players to save to the leaderboard.  0 means save all players.

		private static TimeSpan m_PointsDecayTime = TimeSpan.FromDays(15);      // default time interval for automatic point loss for no pvp activity
		// set m_PointsDecay to zero to disable the automatic points decay feature
		private static int m_PointsDecay = 10;                                  // default point loss if no kills are made within the PointsDecayTime
		
		// set m_CancelTimeout to determine how long it takes to cancel a challenge after it is requested
		public static TimeSpan CancelTimeout = TimeSpan.FromMinutes(15);
		
		public static bool AllowWithinGuildPoints = true;              // allow within-guild challenge duels for points.

		public static bool UnrestrictedChallenges = false;              // allow the normal waiting time between kills for points to be overridden for challenges
		
		// allows players to be autores'd following 1-on-1 duels
		// Team Challenge type matches handle their own autores behavior
		public static bool AutoResAfterDuel = true;
		
		public static bool GainHonorFromDuel = false;
		public static bool LogKills = true;			// log all kills that award points to the kills.log file

		private static LeaderboardTimer m_LeaderboardTimer;
		private static string m_LeaderboardFile;
		private static ArrayList RankList = new ArrayList();
		private static bool needsupdate = true;

		// this enum lists all supported languages
		public enum LanguageType
		{
			ENGLISH,
			SPANISH,
			PORTUGUESE
		}

		// there MUST be as many hashtable array entries as languages
		private static Hashtable [] TextHash = new Hashtable[3];

		private LanguageType m_CurrentLanguage = LanguageType.ENGLISH; // player selected language setting
		
		private static LanguageType m_SystemLanguage = LanguageType.ENGLISH; // system default language setting

		public static DuelLocationEntry[] DuelLocations = new DuelLocationEntry[]
			{
				new DuelLocationEntry("Jhelom Fighting Pit", 1398, 3742, -21, Map.Felucca, 14),
				new DuelLocationEntry("Luna Grand Arena", 940, 637, -90, Map.Malas, 4),
		};

		public class DuelLocationEntry
		{
			public Point3D DuelLocation;
			public Map DuelMap;
			public int DuelRange;
			public string Name;

			public DuelLocationEntry(string name, int X, int Y, int Z, Map map, int range)
			{
				Name = name;
				DuelLocation = new Point3D(X,Y,Z);
				DuelMap = map;
				DuelRange = range;
			}
		}
		
		public static bool DuelLocationAvailable(DuelLocationEntry duelloc)
		{
			// check to see whether there are any players at the location
			if(duelloc == null || duelloc.DuelMap == null) return true;

			int duelrange = duelloc.DuelRange;

			if(duelloc.DuelRange <= 0) duelrange = 16;
            IPooledEnumerable eable = duelloc.DuelMap.GetMobilesInRange(duelloc.DuelLocation, duelrange);

			foreach(Mobile m in eable)
			{
                if (m.Player)
                {
                    eable.Free();
                    return false;
                }
			}

            eable.Free();
			return true;
		}
		
		public static bool CheckCombat(Mobile m)
		{
			return ( m != null && (m.Aggressors.Count > 0 || m.Aggressed.Count > 0) );
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public LanguageType CurrentLanguage
		{
			get{ return m_CurrentLanguage; }
			set { m_CurrentLanguage = value; }
		}


		public static void AddText(LanguageType t, int index, string text)
		{
			int tindex = (int)t;

			if(tindex >= TextHash.Length)
			{
				Console.WriteLine("XmlPoints: Invalid language type {0}: increase hashtable size", t);
				return;
			}


			if(TextHash[tindex] == null)
			{
				TextHash[tindex] = new Hashtable();
			}
		   
			Hashtable h = TextHash[tindex];

			h.Add(index,text);
		}
		
		public static string SystemText(int index)
		{
			if((int)m_SystemLanguage >= TextHash.Length)
			{
				// unsupported language
				// this should never happen
				return String.Format("??Language {0}??",m_SystemLanguage);
			}

			Hashtable h = TextHash[(int)m_SystemLanguage];

			if(h == null || !h.Contains(index))
			{
				return String.Format("??Entry {0}??", index);
			}

			return (string)h[index];
		}

		public string Text(int index)
		{
			if((int)m_CurrentLanguage >= TextHash.Length)
			{
				// unsupported language
				// this should never happen
				Console.WriteLine("XmlPoints: Unsupported language {0}", m_CurrentLanguage);
				return String.Format("??Language {0}??",m_CurrentLanguage);
			}

			Hashtable h = TextHash[(int)m_CurrentLanguage];

			if(h == null || !h.Contains(index))
			{
				Console.WriteLine("XmlPoints: Missing entry for {0} in language {1}", index, m_CurrentLanguage);
				// missing entry in the current language.  Try the default system language
				h = TextHash[(int)m_SystemLanguage];

				// still no entry, so make a final try in the default english
				if(h == null || !h.Contains(index))
				{
					Console.WriteLine("XmlPoints: Also missing entry for {0} in system language {1}", index, m_SystemLanguage);

					// missing entry in the system language.  Try the default english
					h = TextHash[(int)LanguageType.ENGLISH];
	
					// still no entry, so return null
					if(h == null || !h.Contains(index))
					{
						Console.WriteLine("XmlPoints: And finally missing entry for {0} in default language {1}", index, LanguageType.ENGLISH);
						return String.Format("??Entry {0}??", index);
					}
				}
			}

			return (string)h[index];
		}
		
		public static string GetText(Mobile from, int msgindex)
		{
			// go through the participant list and send all participants the message
			if(from == null) return "???";

			XmlPoints a = (XmlPoints)XmlAttach.FindAttachment(from, typeof(XmlPoints));

			if(a != null)
			{
				return a.Text(msgindex);
			} 
			else
			{
				return SystemText(msgindex);
			}
		}
		
		public void SendText(int msgindex)
		{
			this.SendColorText(0, msgindex);
		}
		
		public void SendText(int msgindex, object arg)
		{
			this.SendColorText(0, msgindex, arg);
		}
		
		public void SendText(int msgindex, object arg, object arg2)
		{
			this.SendColorText(0, msgindex, arg, arg2);
		}
		
		public void SendColorText(int color, int msgindex)
		{
			Mobile from = null;

			if(AttachedTo is Mobile)
			{
				from = (Mobile)AttachedTo;
			}
			// go through the participant list and send all participants the message
			if(from == null) return;

			from.SendMessage(color,String.Format(Text(msgindex)));

		}
		
		public void SendColorText(int color, int msgindex, object arg, object arg2)
		{
			Mobile from = null;

			if(AttachedTo is Mobile)
			{
				from = (Mobile)AttachedTo;
			}
			// go through the participant list and send all participants the message
			if(from == null) return;

			from.SendMessage(color,String.Format(Text(msgindex), arg, arg2));

		}
		
		public void SendColorText(int color, int msgindex, object arg)
		{
			Mobile from = null;

			if(AttachedTo is Mobile)
			{
				from = (Mobile)AttachedTo;
			}
			// go through the participant list and send all participants the message
			if(from == null) return;

			from.SendMessage(color,String.Format(Text(msgindex), arg));

		}
		
		public static void SendText(Mobile from, int msgindex)
		{
			SendColorText(from, 0, msgindex);
		}
		
		public static void SendText(Mobile from, int msgindex, object arg)
		{
			SendColorText(from, 0, msgindex, arg);
		}
		
		public static void SendText(Mobile from, int msgindex, object arg, object arg2)
		{
			SendColorText(from, 0, msgindex, arg, arg2);
		}
		
		public static void SendColorText(Mobile from, int color, int msgindex)
		{
			// go through the participant list and send all participants the message
			if(from == null) return;

			XmlPoints a = (XmlPoints)XmlAttach.FindAttachment(from, typeof(XmlPoints));

			if(a != null)
			{
				from.SendMessage(color,a.Text(msgindex));
			}
		}

		public static void SendColorText(Mobile from, int color, int msgindex, object arg)
		{
			// go through the participant list and send all participants the message
			if(from == null) return;

			XmlPoints a = (XmlPoints)XmlAttach.FindAttachment(from, typeof(XmlPoints));

			if(a != null)
			{
				from.SendMessage(color,String.Format(a.Text(msgindex), arg));
			}
		}
		
		public static void SendColorText(Mobile from, int color, int msgindex, object arg, object arg2)
		{
			// go through the participant list and send all participants the message
			if(from == null) return;

			XmlPoints a = (XmlPoints)XmlAttach.FindAttachment(from, typeof(XmlPoints));

			if(a != null)
			{
				from.SendMessage(color,String.Format(a.Text(msgindex), arg, arg2));
			}
		}

		private class KillEntry
		{
			public Mobile      Killed;
			public DateTime    WhenKilled;

			public KillEntry(Mobile m, DateTime t)
			{
				Killed = m;
				WhenKilled = t;
			}
		}

		public class RankEntry : IComparable
		{
			public Mobile      Killer;
			public int    Rank;
			public XmlPoints    PointsAttachment;

			public RankEntry(Mobile m, XmlPoints attachment)
			{
				Killer = m;
				PointsAttachment = attachment;
			}

			public int CompareTo( object obj )
			{
				RankEntry p = (RankEntry)obj;

				if(p.PointsAttachment == null || PointsAttachment == null) return 0;

				// break points ties with kills (more kills means higher rank)
				if(p.PointsAttachment.Points - PointsAttachment.Points == 0)
				{
					// if kills are the same then compare deaths (fewer deaths means higher rank)
					if(p.PointsAttachment.Kills - PointsAttachment.Kills == 0)
					{
						// if deaths are the same then use previous ranks
						if(p.PointsAttachment.Deaths - PointsAttachment.Deaths == 0)
						{
							return p.PointsAttachment.Rank - PointsAttachment.Rank;
						}

						return PointsAttachment.Deaths - p.PointsAttachment.Deaths;
					}

					return p.PointsAttachment.Kills - PointsAttachment.Kills;
				}

				return p.PointsAttachment.Points - PointsAttachment.Points;
			}
		}

		private static bool SameGuild(Mobile killed, Mobile killer)
		{
			return ( killer.Guild == killed.Guild && killer.Guild != null && killed.Guild != null);
		}

		private static void RefreshRankList()
		{
			if(needsupdate && RankList != null)
			{
				RankList.Sort();
				
				int rank = 0;
				//int prevpoints = 0;
				for(int i= 0; i<RankList.Count;i++)
				{
					RankEntry p = RankList[i] as RankEntry;
	
					// bump the rank for every change in point level
					// this means that people with the same points score will have the same rank
					/*
					if(p.PointsAttachment.Points != prevpoints)
					{
						rank++;
					}

					prevpoints = p.PointsAttachment.Points;
					*/
	
					// bump the rank for every successive player in the list.  Players with the same points total will be
					// ordered by kills
					rank++;

					p.Rank = rank;
	
				}
				needsupdate = false;
			}
		}

		public static int GetRanking(Mobile m)
		{
			if(RankList == null || m == null) return 0;

			RefreshRankList();

			// go through the sorted list and calculate rank

			for(int i= 0; i<RankList.Count;i++)
			{
				RankEntry p = RankList[i] as RankEntry;
				// found the person?
				if(p.Killer == m)
				{
					return p.Rank;
				}
			}

			// rank 0 means unranked
			return 0;
		}

		private static void UpdateRanking(Mobile m, XmlPoints attachment)
		{
			if(RankList == null) RankList = new ArrayList();

			// flag the rank list for updating on the next attempt to retrieve a rank
			needsupdate = true;

			bool found = false;

			// rank the entries
			for(int i= 0; i<RankList.Count;i++)
			{
				RankEntry p = RankList[i] as RankEntry;
				
				// found a match
				if(p != null && p.Killer == m)
				{
					// update the entry with the new points value

					p.PointsAttachment = attachment;
					found = true;
					break;
				}
			}

			// a new entry so add it
			if(!found)
			{
				RankList.Add(new RankEntry(m, attachment));
			}

			// if points statistics are being displayed in player name properties, then update them
			if(m != null) m.InvalidateProperties();
		}

		public static int GetCredits(Mobile m)
		{
			int val = 0;

			XmlPoints x = XmlAttach.FindAttachment(m, typeof(XmlPoints)) as XmlPoints;
			if(x != null)
			{
				val = x.Credits;
			}
			
			return val;
		}
		
		public static int GetPoints(Mobile m)
		{
			int val = 0;

			XmlPoints x = XmlAttach.FindAttachment(m, typeof(XmlPoints)) as XmlPoints;
			if(x != null)
			{
				val = x.Points;
			}

			return val;
		}

		public static bool HasCredits(Mobile m, int credits)
		{
			if(m == null || m.Deleted) return false;

			XmlPoints x = XmlAttach.FindAttachment(m,typeof(XmlPoints)) as XmlPoints;
			if(x != null)
			{
				if(x.Credits >= credits)
				{
					return true;
				}
			}

			return false;
		}

		public static bool TakeCredits(Mobile m, int credits)
		{
			if(m == null || m.Deleted) return false;

			XmlPoints x = XmlAttach.FindAttachment(m, typeof(XmlPoints)) as XmlPoints;
			if(x != null)
			{
				if(x.Credits >= credits)
				{
					x.Credits -= credits;
					return true;
				}
			}

			return false;
		}
		
		public static void BroadcastMessage ( AccessLevel ac, int hue, string message )
		{ 
			foreach ( NetState state in NetState.Instances )
			{
				Mobile m = state.Mobile;

				if ( m != null && m.AccessLevel >= ac )
				{
					// check to see if they have a points attachment with ReceiveBroadcasts enabled
					XmlPoints x = XmlAttach.FindAttachment(m, typeof(XmlPoints)) as XmlPoints;
					if(x != null)
					{
						if(!x.ReceiveBroadcasts)
							return;
					}

					m.SendMessage( hue, message );
				}
			}
		}

		public static void EventSink_Speech( SpeechEventArgs args )
		{
			Mobile from = args.Mobile;

			if(from == null || from.Map == null || !from.Player) return;

			if(args.Speech != null && args.Speech.ToLower() == "showpoints")
				ShowPointsOverhead(from);

			if(args.Speech != null && args.Speech.ToLower() == "i wish to duel")
				from.Target = new ChallengeTarget(from);
		}
		
		public static void ShowPointsOverhead( Mobile from )
		{
			if(from == null) return;

			from.PublicOverheadMessage( MessageType.Regular, 0x3B2, false, GetPoints(from).ToString());
		}

		public static new void Initialize()
		{
			// Register our speech handler
			EventSink.Speech += new SpeechEventHandler( EventSink_Speech );

			CommandSystem.Register( "PointsLanguage", AccessLevel.Player, new CommandEventHandler( Language_OnCommand ) );
			CommandSystem.Register( "Challenge", AccessLevel.Player, new CommandEventHandler( Challenge_OnCommand ) );
			CommandSystem.Register( "LMSChallenge", AccessLevel.Player, new CommandEventHandler( LMSChallenge_OnCommand ) );
			CommandSystem.Register( "TeamLMSChallenge", AccessLevel.Player, new CommandEventHandler( TeamLMSChallenge_OnCommand ) );
			CommandSystem.Register( "Deathmatch", AccessLevel.Player, new CommandEventHandler( Deathmatch_OnCommand ) );
			CommandSystem.Register( "TeamDeathmatch", AccessLevel.Player, new CommandEventHandler( TeamDeathmatch_OnCommand ) );
			CommandSystem.Register( "DeathBall", AccessLevel.Player, new CommandEventHandler( DeathBall_OnCommand ) );
			CommandSystem.Register( "KingOfTheHill", AccessLevel.Player, new CommandEventHandler( KingOfTheHill_OnCommand ) );
			CommandSystem.Register( "TeamDeathBall", AccessLevel.Player, new CommandEventHandler( TeamDeathBall_OnCommand ) );
			CommandSystem.Register( "TeamKotH", AccessLevel.Player, new CommandEventHandler( TeamKotH_OnCommand ) );
			CommandSystem.Register( "CTFChallenge", AccessLevel.Player, new CommandEventHandler( CTFChallenge_OnCommand ) );
			CommandSystem.Register( "SystemBroadcastKills", AccessLevel.GameMaster, new CommandEventHandler( SystemBroadcastKills_OnCommand ) );
			CommandSystem.Register( "SeeKills", AccessLevel.Player, new CommandEventHandler( SeeKills_OnCommand ) );
			CommandSystem.Register( "BroadcastKills", AccessLevel.Player, new CommandEventHandler( BroadcastKills_OnCommand ) );
			CommandSystem.Register( "CheckPoints", AccessLevel.Player, new CommandEventHandler( CheckPoints_OnCommand ) );
			CommandSystem.Register( "TopPlayers", AccessLevel.Player, new CommandEventHandler( TopPlayers_OnCommand ) );
			CommandSystem.Register( "AddAllPoints", AccessLevel.Administrator, new CommandEventHandler( AddAllPoints_OnCommand ) );
			CommandSystem.Register( "RemoveAllPoints", AccessLevel.Administrator, new CommandEventHandler( RemoveAllPoints_OnCommand ) );
			CommandSystem.Register( "LeaderboardSave", AccessLevel.Administrator, new CommandEventHandler( LeaderboardSave_OnCommand ) );

			foreach(Item i in World.Items.Values)
			{
				if(i is BaseChallengeGame && !((BaseChallengeGame)i).GameCompleted)
				{
					// find the region it is in
					// is this in a challenge game region?
					Region r = Region.Find(i.Location, i.Map);
					if(r is ChallengeGameRegion)
					{
						ChallengeGameRegion cgr = r as ChallengeGameRegion;
						
						cgr.ChallengeGame = i as BaseChallengeGame;
					}
				}
			}
		}
		
		public static void WriteLeaderboardXml(string filename, int nranks)
		{
			string dirname = Path.Combine( m_LeaderboardSaveDirectory, filename );

			StreamWriter sw = new StreamWriter( dirname  );

			XmlTextWriter xf = new XmlTextWriter( sw );

			if(xf == null)
			{
				Console.WriteLine("Error: unable to save XML leaderboard to {0}", dirname);
				return;
			}

			xf.Formatting = Formatting.Indented;

			xf.WriteStartDocument( true );

			xf.WriteStartElement( "Leaderboard" );
				
			if(nranks > 0)
				xf.WriteAttributeString( "nentries", nranks.ToString() );
			else
				xf.WriteAttributeString( "nentries", RankList.Count.ToString() );

			// go through the sorted list and display the top ranked players

			for(int i= 0; i<RankList.Count;i++)
			{
				if(nranks > 0 && i >= nranks) break;

				RankEntry r = RankList[i] as RankEntry;
				XmlPoints a = r.PointsAttachment;


				if(r.Killer != null && !r.Killer.Deleted && r.Rank > 0 && a != null && !a.Deleted)
				{
					string guildname = null;

					if(r.Killer.Guild != null) 
						guildname = r.Killer.Guild.Abbreviation;
#if(FACTIONS)
					string factionname = null;

					if(r.Killer is PlayerMobile && ((PlayerMobile)r.Killer).FactionPlayerState != null) 
						factionname = ((PlayerMobile)r.Killer).FactionPlayerState.Faction.ToString();
#endif
					// check for any ranking change and update rank date
					if(r.Rank != a.Rank)
					{
						a.WhenRanked = DateTime.UtcNow;
						if(a.Rank > 0)
							a.DeltaRank = a.Rank - r.Rank;
						a.Rank = r.Rank;

					}

					TimeSpan timeranked = DateTime.UtcNow - a.WhenRanked;

					// write out the entry information
					
					xf.WriteStartElement( "Entry" );
					xf.WriteAttributeString( "number", i.ToString() );

					xf.WriteStartElement( "Player" );
					xf.WriteString( r.Killer.Name );
					xf.WriteEndElement();

					xf.WriteStartElement( "Guild" );
					xf.WriteString( guildname );
					xf.WriteEndElement();
#if(FACTIONS)
					xf.WriteStartElement( "Faction" );
					xf.WriteString( factionname );
					xf.WriteEndElement();
#endif
					xf.WriteStartElement( "Points" );
					xf.WriteString( a.Points.ToString() );
					xf.WriteEndElement();

					string kills = "???";
					try
					{
						kills = a.Kills.ToString();
					} 
					catch{}
					xf.WriteStartElement( "Kills" );
					xf.WriteString( kills );
					xf.WriteEndElement();

					string deaths = "???";
					try
					{
						deaths = a.Deaths.ToString();
					} 
					catch{}
					xf.WriteStartElement( "Deaths" );
					xf.WriteString( deaths );
					xf.WriteEndElement();

					xf.WriteStartElement( "Rank" );
					xf.WriteString( a.Rank.ToString() );
					xf.WriteEndElement();

					xf.WriteStartElement( "Change" );
					xf.WriteString( a.DeltaRank.ToString() );
					xf.WriteEndElement();

					xf.WriteStartElement( "Duration" );
					xf.WriteString( timeranked.ToString() );
					xf.WriteEndElement();

					// end the entry
					xf.WriteEndElement();

				}
			}

			xf.WriteEndElement();

			xf.Close();
		}

		public static string HtmlSpecialEncoding(string text)
		{
			if (text == null) return null;

			string encoded = "";

			// replace each char with the special ascii encoded equivalent
			for (int i = 0; i < text.Length; i++)
			{
				encoded += String.Format("&#{0};", (int)text[i]);
			}

			return encoded;
		}

		public static void WriteLeaderboardHtml(string filename, int nranks)
		{
			string dirname = Path.Combine( m_LeaderboardSaveDirectory, filename );

			StreamWriter sw = new StreamWriter( dirname  );

			if(sw == null)
			{
				Console.WriteLine("Error: unable to save HTML leaderboard to {0}", dirname);
				return;
			}
			sw.WriteLine("<TABLE border=\"1\" summary=\"This table gives leaderboard stats\"> ");
			sw.WriteLine( "<CAPTION><B>Leaderboard</B></CAPTION>");
#if(FACTIONS)
			sw.WriteLine( "<TR><TH><TH>Player Name<TH>Guild<TH>Faction<TH>Points<TH>Kills<TH>Deaths<TH>Rank<TH>Change<TH>Time at current rank");
#else
			sw.WriteLine( "<TR><TH><TH>Player Name<TH>Guild<TH>Points<TH>Kills<TH>Deaths<TH>Rank<TH>Change<TH>Time at current rank");
#endif
			// go through the sorted list and display the top ranked players

			for(int i= 0; i<RankList.Count;i++)
			{
				if(nranks > 0 && i >= nranks) break;

				RankEntry r = RankList[i] as RankEntry;
				XmlPoints a = r.PointsAttachment;

				if(r.Killer != null && !r.Killer.Deleted && r.Rank > 0 && a != null && !a.Deleted)
				{
					string guildname = null;

					if(r.Killer.Guild != null)
						guildname = HtmlSpecialEncoding(r.Killer.Guild.Abbreviation);
#if(FACTIONS)
					string factionname = null;

					if(r.Killer is PlayerMobile && ((PlayerMobile)r.Killer).FactionPlayerState != null) 
						factionname = ((PlayerMobile)r.Killer).FactionPlayerState.Faction.ToString();
#endif
					// check for any ranking change and update rank date
					if(r.Rank != a.Rank)
					{
						a.WhenRanked = DateTime.UtcNow;
						if(a.Rank > 0)
							a.DeltaRank = a.Rank - r.Rank;
						a.Rank = r.Rank;

					}

					TimeSpan tr = DateTime.UtcNow - a.WhenRanked;
					string timeranked = null;
					int days = (int)tr.TotalDays;
					int hours = (int)(tr - TimeSpan.FromDays(days)).TotalHours;
					int minutes = (int)(tr - TimeSpan.FromHours(hours)).TotalMinutes;
					int seconds = (int)(tr - TimeSpan.FromMinutes(minutes)).TotalSeconds;

					if (days > 0)
					{
						timeranked = String.Format("{0} days {1} hrs", days, hours);
					}
					else
						if (hours > 0)
						{
							timeranked = String.Format("{0} hrs {1} mins", hours, minutes);
						}
						else
						{
							timeranked = String.Format("{0} mins {1} secs", minutes, seconds);
						}
					
					string kills = "???";
					try
					{
						kills = a.Kills.ToString();
					} 
					catch{}

					string deaths = "???";
					try
					{
						deaths = a.Deaths.ToString();
					} 
					catch{}

#if(FACTIONS)
					// write out the entry information
					sw.WriteLine( "<TR><TH><TD>{0}<TD>{1}<TD>{2}<TD>{3}<TD>{4}<TD>{5}<TD>{6}<TD>{7}<TD>{8}",
						r.Killer.Name,
						guildname,
						factionname,
						a.Points,
						kills,
						deaths,
						a.Rank,
						a.DeltaRank,
						timeranked
						);
#else
					// write out the entry information
					sw.WriteLine( "<TR><TH><TD>{0}<TD>{1}<TD>{2}<TD>{3}<TD>{4}<TD>{5}<TD>{6}<TD>{7}",
					r.Killer.Name,
					guildname,
					a.Points,
					kills,
					deaths,
					a.Rank,
					a.DeltaRank,
					timeranked
					);

#endif

				}
			}
			sw.WriteLine( "</TABLE>");
			sw.Close();
		}


		public static void WriteLeaderboard(string filename, int nranks)
		{
			if(RankList == null) return;

			// force an update of the leaderboard rankings
			needsupdate = true;
			RefreshRankList();

			if ( !Directory.Exists( m_LeaderboardSaveDirectory ) )
				Directory.CreateDirectory( m_LeaderboardSaveDirectory );

			WriteLeaderboardXml(filename + ".xml",  nranks);

			WriteLeaderboardHtml(filename + ".html",  nranks);

		}


		[Usage( "LeaderboardSave [filename [minutes[nentries]]][off]" )]
		[Description( "Periodically save .xml leaderboard information to the specified file" )]
		public static void LeaderboardSave_OnCommand( CommandEventArgs e )
		{
			if(e.Arguments.Length > 0)
			{
				if(m_LeaderboardTimer != null) m_LeaderboardTimer.Stop();

				if(e.Arguments[0].ToLower() != "off")
				{
					m_LeaderboardFile = e.Arguments[0];

					if(e.Arguments.Length > 1)
					{
						try
						{
							m_LeaderboardSaveInterval = TimeSpan.FromMinutes(double.Parse(e.Arguments[1]));
						} 
						catch{}
					}
					
					if(e.Arguments.Length > 2)
					{
						try
						{
							m_LeaderboardSaveRanks = int.Parse(e.Arguments[2]);
						} 
						catch{}
					}


					m_LeaderboardTimer = new LeaderboardTimer(m_LeaderboardFile, m_LeaderboardSaveInterval, m_LeaderboardSaveRanks);
					m_LeaderboardTimer.Start();
				}
			}


			if(m_LeaderboardTimer != null && m_LeaderboardTimer.Running)
			{
				e.Mobile.SendMessage("Leaderboard is saving to {0} every {1} minutes. Nranks = {2}",
					m_LeaderboardFile, m_LeaderboardSaveInterval.TotalMinutes, m_LeaderboardSaveRanks);
			} 
			else
			{
				e.Mobile.SendMessage("Leaderboard saving is off.");
			}
		}

		public static void LBSSerialize( GenericWriter writer )
		{
			// version
			writer.Write( (int) 1 );

			// version 1
			writer.Write(m_SystemBroadcast);
			// version 0
			if(m_LeaderboardTimer != null && m_LeaderboardTimer.Running)
			{
				writer.Write((bool)true);
			} 
			else
				writer.Write((bool)false);
			writer.Write(m_LeaderboardSaveInterval);
			writer.Write(m_LeaderboardSaveRanks);
			writer.Write(m_LeaderboardFile);
		}
		
		public static void LBSDeserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch(version)
			{
				case 1:
					m_SystemBroadcast = reader.ReadBool();
					goto case 0;
				case 0:
					bool running = reader.ReadBool();
					m_LeaderboardSaveInterval = reader.ReadTimeSpan();
					m_LeaderboardSaveRanks = reader.ReadInt();
					m_LeaderboardFile = reader.ReadString();

					if(running)
					{
						if(m_LeaderboardTimer != null) m_LeaderboardTimer.Stop();
						m_LeaderboardTimer = new LeaderboardTimer(m_LeaderboardFile, m_LeaderboardSaveInterval, m_LeaderboardSaveRanks);
						m_LeaderboardTimer.Start();
					}
					break;
			}

		}

		// added the duration timer that begins on spawning
		private class LeaderboardTimer : Timer
		{
			private string m_filename;
			private int m_nranks;

			public LeaderboardTimer( string filename, TimeSpan delay, int nranks ) : base( delay, delay )
			{
				m_filename = filename;
				m_nranks = nranks;
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				WriteLeaderboard(m_filename, m_nranks);
			}
		}
		
		[Usage( "PointsLanguage" )]
		[Description( "Displays or sets the language used by the points system" )]
		public static void Language_OnCommand( CommandEventArgs e )
		{
			XmlPoints a = (XmlPoints)XmlAttach.FindAttachment(e.Mobile,typeof(XmlPoints));

			if(a == null || e.Mobile == null) return;

			if(e.Arguments.Length > 0)
			{
				try
				{
					a.CurrentLanguage = (LanguageType) Enum.Parse(typeof(LanguageType), e.Arguments[0], true);
				} 
				catch {}
			}
			
			e.Mobile.SendMessage("Current language is {0}", a.CurrentLanguage);
		}

		[Usage( "CheckPoints" )]
		[Description( "Displays the players points and ranking" )]
		public static void CheckPoints_OnCommand( CommandEventArgs e )
		{
			string msg = null;

			XmlPoints x = XmlAttach.FindAttachment(e.Mobile,typeof(XmlPoints)) as XmlPoints;
			if(x != null)
			{
				msg = x.OnIdentify(e.Mobile);
			}
			if(msg != null)
				e.Mobile.SendMessage(msg);
		}


		[Usage( "SeeKills [true/false]" )]
		[Description( "Determines whether a player sees others pvp broadcast results." )]
		public static void SeeKills_OnCommand( CommandEventArgs e )
		{

			XmlPoints x = XmlAttach.FindAttachment(e.Mobile,typeof(XmlPoints)) as XmlPoints;
			if(x != null)
			{
				if(e.Arguments.Length > 0)
				{
					bool result;
					if(e.Arguments[0]=="si" || e.Arguments[0]=="ok" || e.Arguments[0]=="vero")
						result=true;
					else
						bool.TryParse(e.Arguments[0], out result);
					x.ReceiveBroadcasts=result;
				}

				e.Mobile.SendMessage("SeeKills is {0}", x.ReceiveBroadcasts);
			}
		}

		[Usage( "BroadcastKills [true/false]" )]
		[Description( "Determines whether pvp results will be broadcast.  The killers (winner) flag setting is used. " )]
		public static void BroadcastKills_OnCommand( CommandEventArgs e )
		{
			XmlPoints x = XmlAttach.FindAttachment(e.Mobile,typeof(XmlPoints)) as XmlPoints;
			if(x != null)
			{
				if(e.Arguments.Length > 0)
				{
					bool result;
					if(e.Arguments[0]=="si" || e.Arguments[0]=="ok" || e.Arguments[0]=="vero")
						result=true;
					else
						bool.TryParse(e.Arguments[0], out result);
					x.Broadcast=result;
				}

				e.Mobile.SendMessage("BroadcastKills is {0}", x.Broadcast);
			}
		}

		[Usage( "SystemBroadcastKills [true/false]" )]
		[Description( "GM override of broadcasting of pvp results.  False means no broadcasting. True means players settings are used." )]
		public static void SystemBroadcastKills_OnCommand( CommandEventArgs e )
		{
			if(e.Arguments.Length > 0)
			{
				bool.TryParse(e.Arguments[0], out m_SystemBroadcast);
			} 

			e.Mobile.SendMessage("SystemBroadcastKills is {0}.",m_SystemBroadcast);
		}

		[Usage( "TopPlayers" )]
		[Description( "Displays the top players in points" )]
		public static void TopPlayers_OnCommand( CommandEventArgs e )
		{
			XmlPoints attachment = XmlAttach.FindAttachment(e.Mobile, typeof(XmlPoints)) as XmlPoints;
			e.Mobile.CloseGump(typeof(TopPlayersGump));
			if(attachment!=null)
				e.Mobile.SendGump(new TopPlayersGump(attachment));
		}

		public static bool AreChallengers(Mobile from, Mobile target)
		{
			if(from == null || target == null) return false;

			// both must be players
			if(!(from.Player && target.Player)) return false;

			// check for points attachments on each
			XmlPoints afrom = (XmlPoints)XmlAttach.FindAttachment(from, typeof(XmlPoints));

			if(afrom == null || afrom.Deleted)
				return false;

			XmlPoints atarget = (XmlPoints)XmlAttach.FindAttachment(target, typeof(XmlPoints));

			if(atarget == null || atarget.Deleted)
				return false;

			// check the individual challenger status on each
			if(afrom.Challenger == target && atarget.Challenger == from)
				return true;

			// check the team challenge status
			if(afrom.m_ChallengeGame != null && !afrom.m_ChallengeGame.Deleted && atarget.m_ChallengeGame == afrom.m_ChallengeGame  )
			{
				return afrom.m_ChallengeGame.AreChallengers(from, target);
			}

			return false;
		}

		public static bool AreInAnyGame(Mobile target)
		{

			if(target == null) return false;

			// must be a player
			if(!target.Player) return false;

			// get the challenge game info from the points attachment
			XmlPoints atarget = (XmlPoints)XmlAttach.FindAttachment(target, typeof(XmlPoints));

			if(atarget != null && !atarget.Deleted && atarget.m_ChallengeGame != null && !atarget.m_ChallengeGame.Deleted)
			{
				return atarget.m_ChallengeGame.AreInGame(target);
			}

			return false;

		}

		public static bool AreInSameGame(Mobile from, Mobile target)
		{

			if(from == null || target == null) return false;

			// both must be players
			if(!(from.Player && target.Player)) return false;

			// check for points attachments on each
			XmlPoints afrom = (XmlPoints)XmlAttach.FindAttachment(from, typeof(XmlPoints));

			if(afrom == null || afrom.Deleted)
				return false;

			XmlPoints atarget = (XmlPoints)XmlAttach.FindAttachment(target, typeof(XmlPoints));

			if(atarget == null || atarget.Deleted)
				return false;

			// check the team challenge status
			if(afrom.m_ChallengeGame != null && !afrom.m_ChallengeGame.Deleted && afrom.m_ChallengeGame == atarget.m_ChallengeGame)
			{
				return afrom.m_ChallengeGame.AreInGame(target) && afrom.m_ChallengeGame.AreInGame(target);
			}

			return false;

		}

		public static bool AreTeamMembers(Mobile from, Mobile target)
		{
			if(from == null || target == null) return false;

			// both must be players
			if(!(from.Player && target.Player)) return false;

			// check for points attachments on each
			XmlPoints afrom = (XmlPoints)XmlAttach.FindAttachment(from, typeof(XmlPoints));

			if(afrom == null || afrom.Deleted)
				return false;

			XmlPoints atarget = (XmlPoints)XmlAttach.FindAttachment(target, typeof(XmlPoints));

			if(atarget == null || atarget.Deleted)
				return false;

			// check the team challenge status
			if(afrom.m_ChallengeGame != null && !afrom.m_ChallengeGame.Deleted && afrom.m_ChallengeGame == atarget.m_ChallengeGame)
			{
				return afrom.m_ChallengeGame.AreTeamMembers(from, target);
			}

			return false;
		}

		public static bool InsuranceIsFree(Mobile from, Mobile awardto)
		{
			if(from == null || awardto == null) return false;

			// both must be players
			if(!(from.Player && awardto.Player)) return false;

			// check for points attachments on each
			XmlPoints afrom = (XmlPoints)XmlAttach.FindAttachment(from, typeof(XmlPoints));

			if(afrom == null || afrom.Deleted)
				return false;

			XmlPoints atarget = (XmlPoints)XmlAttach.FindAttachment(awardto, typeof(XmlPoints));

			if(atarget == null || atarget.Deleted)
				return false;

			// check the team challenge status
			if(afrom.m_ChallengeGame != null && !afrom.m_ChallengeGame.Deleted && afrom.m_ChallengeGame == atarget.m_ChallengeGame)
			{
				return afrom.m_ChallengeGame.InsuranceIsFree(from, awardto);
			}
			
			// uncomment the line below if you want to prevent insurance awards for normal 1on1 duels
			//if(atarget.Challenger == from) return true;
			
			return false;
		}
		
		public static bool YoungProtection(Mobile from, Mobile target)
		{
			// newbie protection
			if( ((target.SkillsTotal < 6000 && (from.SkillsTotal - target.SkillsTotal ) > 1000) ||
				(target.RawStatTotal <= 200 && (from.RawStatTotal - target.RawStatTotal) > 20 ) ))
				return true;

			// dont allow young players to be challenged by experienced players
			// note, this will allow young players to challenge other young players
			if(from is PlayerMobile && target is PlayerMobile)
			{
				if(((PlayerMobile)target).Young && !((PlayerMobile)from).Young)
					return true;
			}

			return false;

		}

		public static bool AllowChallengeGump(Mobile from, Mobile target)
		{
			if (from == null || target == null) return false;

			// uncomment the code below if you want to restrict challenges to towns only
			/*
			if (!from.Region.IsPartOf<Regions.TownRegion>() || !target.Region.IsPartOf<Regions.TownRegion>())
			{
				from.SendMessage("You must be in a town to issue a challenge"); 
				return false;
			}
			*/

			if (from.Region.IsPartOf<Regions.Jail>() || target.Region.IsPartOf<Regions.Jail>())
			{
				from.SendLocalizedMessage(1042632); // You'll need a better jailbreak plan then that!
				return false;
			}

			return true;
		}

		private class ChallengeTarget : Target
		{
			private Mobile m_challenger;

			public ChallengeTarget(Mobile m) :  base ( 30, false, TargetFlags.None )
			{
				m_challenger = m;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if(from == null || targeted == null) return;

				if(targeted is Mobile && ((Mobile)targeted).Player)
				{
					Mobile pm = targeted as Mobile;
					
					// test them for young status
					if(YoungProtection(from, pm))
					{
						SendText(from, 100207, pm.Name); // "{0} is too inexperience to be challenged"
						return;
					}

					// check the owner for existing challenges
					XmlPoints a = (XmlPoints)XmlAttach.FindAttachment(from, typeof(XmlPoints));

					if(a != null && !a.Deleted)
					{
						// issuing a challenge when one is already in place will initiate cancellation of the current challenge
						if(a.Challenger != null)
						{

							// this will initiate the challenge cancellation timer

							if(a.m_CancelTimer != null && a.m_CancelTimer.Running)
							{
								// timer is running
								a.SendText(100208, a.m_CancelEnd - DateTime.UtcNow); // "{0} mins remaining until current challenge is cancelled."
							} 
							else
							{

								a.SendText(100209, XmlPoints.CancelTimeout.TotalMinutes);  // "Canceling current challenge.  Please wait {0} minutes"

								SendText(a.Challenger, 100210, from.Name, XmlPoints.CancelTimeout.TotalMinutes); // "{0} is canceling the current challenge. {1} minutes remain"

								// start up the cancel challenge timer
								a.DoTimer(XmlPoints.CancelTimeout);

								// update the points gumps on the challenger if they are open
								if(from.HasGump(typeof(PointsGump)))
								{
									a.OnIdentify(from);
								}
								// update the points gumps on the challenge target if they are open
								if(a.Challenger.HasGump(typeof(PointsGump)))
								{
									XmlPoints ca = (XmlPoints)XmlAttach.FindAttachment(a.Challenger, typeof(XmlPoints));
									if(ca != null && !ca.Deleted)
										ca.OnIdentify(a.Challenger);
								}
							}
							return;
						}

						// check the target for existing challengers
						XmlPoints xa = (XmlPoints)XmlAttach.FindAttachment(pm, typeof(XmlPoints));

						if(xa != null && !xa.Deleted)
						{
							if(xa.Challenger != null)
							{
								from.SendMessage(String.Format(a.Text(100211),pm.Name));  // "{0} is already being challenged."
								return;
							}
						}

						if(from == targeted)
						{
							from.SendMessage(a.Text(100212));  // "You cannot challenge yourself."
						} 
						else
						{
							// send the confirmation gump to the challenged player
							from.SendGump(new IssueChallengeGump(m_challenger, pm));
						}
					} 
					else
					{
						from.SendMessage(SystemText(100213)); // "No XmlPoints support."
					}
				}
			}
		}
		
		public void DoTimer( TimeSpan delay )
		{
			if ( m_CancelTimer != null )
				m_CancelTimer.Stop();

			m_CancelTimer = new CancelTimer(this, delay);
			m_CancelEnd = DateTime.UtcNow + delay;
			m_CancelTimer.Start();
		}


		public class CancelTimer : Timer
		{
			private XmlPoints m_attachment;

			public CancelTimer( XmlPoints a, TimeSpan delay ) : base( delay )
			{
				Priority = TimerPriority.OneSecond;
				m_attachment = a;
			}

			protected override void OnTick()
			{
				if(m_attachment == null || m_attachment.Deleted) return;

				Mobile from = m_attachment.AttachedTo as Mobile;
				 
				if(from != null && m_attachment.Challenger != null)
				{
					SendText(from, 100214, m_attachment.Challenger.Name); // "Challenge with {0} has been cancelled"
					 
					if(from.HasGump(typeof(PointsGump)))
					{
						m_attachment.OnIdentify(from);
					}
				}

				// clear the challenger on the challengers attachment
				XmlPoints xa = (XmlPoints)XmlAttach.FindAttachment(m_attachment.Challenger, typeof(XmlPoints));

				if(xa != null && !xa.Deleted)
				{
					// check the challenger field to see if it matches the current
					if(xa.Challenger == from)
					{

						if(m_attachment.Challenger != null && from != null)
						{
							m_attachment.Challenger.SendMessage(String.Format(xa.Text(100214), from.Name)); // "Challenge with {0} has been cancelled"
						}
						// then clear it
						xa.Challenger = null;
					}
				}
				// clear challenger on this attachment
				m_attachment.Challenger = null;
				
				// refresh any open gumps
				if(from != null && xa != null && xa.AttachedTo is Mobile)
				{

					if(from.HasGump(typeof(PointsGump)))
					{
						m_attachment.OnIdentify(from);
					}

					// and update the gumps if they are open
					if(((Mobile)xa.AttachedTo).HasGump(typeof(PointsGump)))
					{
						xa.OnIdentify((Mobile)xa.AttachedTo);
					}
				}
			}
		}

		[Usage( "Challenge" )]
		[Description( "Challenge another player to a duel for points" )]
		public static void Challenge_OnCommand( CommandEventArgs e )
		{
			// target the player you wish to challenge
			e.Mobile.Target = new ChallengeTarget(e.Mobile);
		}
		
		[Usage( "LMSChallenge" )]
		[Description( "Creates a Last Man Standing challenge game" )]
		public static void LMSChallenge_OnCommand( CommandEventArgs e )
		{
			BaseChallengeGame.DoSetupChallenge(e.Mobile, 100302, typeof(LastManStandingGauntlet));
		}

		[Usage( "TeamLMSChallenge" )]
		[Description( "Creates a Team Last Man Standing challenge game" )]
		public static void TeamLMSChallenge_OnCommand( CommandEventArgs e )
		{
			BaseChallengeGame.DoSetupChallenge(e.Mobile, 100413, typeof(TeamLMSGauntlet));
		}

		[Usage( "Deathmatch" )]
		[Description( "Creates a Deathmatch challenge game" )]
		public static void Deathmatch_OnCommand( CommandEventArgs e )
		{
			BaseChallengeGame.DoSetupChallenge(e.Mobile, 100400, typeof(DeathmatchGauntlet));
		}
		
		[Usage( "TeamDeathmatch" )]
		[Description( "Creates a Team Deathmatch challenge game" )]
		public static void TeamDeathmatch_OnCommand( CommandEventArgs e )
		{
			BaseChallengeGame.DoSetupChallenge(e.Mobile, 100415, typeof(TeamDeathmatchGauntlet));
		}

		[Usage( "DeathBall" )]
		[Description( "Creates a DeathBall challenge game" )]
		public static void DeathBall_OnCommand( CommandEventArgs e )
		{
			BaseChallengeGame.DoSetupChallenge(e.Mobile, 100411, typeof(DeathBallGauntlet));
		}
		
		[Usage( "TeamDeathball" )]
		[Description( "Creates a Team Deathball challenge game" )]
		public static void TeamDeathBall_OnCommand( CommandEventArgs e )
		{
			BaseChallengeGame.DoSetupChallenge(e.Mobile, 100416, typeof(TeamDeathballGauntlet));
		}

		[Usage( "KingOfTheHill" )]
		[Description( "Creates a King of the Hill challenge game" )]
		public static void KingOfTheHill_OnCommand( CommandEventArgs e )
		{
			BaseChallengeGame.DoSetupChallenge(e.Mobile, 100410, typeof(KingOfTheHillGauntlet));
		}

		[Usage( "TeamKotH" )]
		[Description( "Creates a Team King of the Hill challenge game" )]
		public static void TeamKotH_OnCommand( CommandEventArgs e )
		{
			BaseChallengeGame.DoSetupChallenge(e.Mobile, 100417, typeof(TeamKotHGauntlet));
		}
		
		[Usage( "CTFChallenge" )]
		[Description( "Creates a CTF challenge game" )]
		public static void CTFChallenge_OnCommand( CommandEventArgs e )
		{
			BaseChallengeGame.DoSetupChallenge(e.Mobile, 100418, typeof(CTFGauntlet));
		}

		[Usage( "AddAllPoints" )]
		[Description( "Adds the XmlPoints attachment to all players" )]
		public static void AddAllPoints_OnCommand( CommandEventArgs e )
		{
			int count = 0;
			foreach(Mobile m in World.Mobiles.Values)
			{
				if(m.Player)
				{
					// does this player already have a points attachment?
					XmlAttachment x = XmlAttach.FindAttachment(m, typeof(XmlPoints));
					if(x == null)
					{
						x = new XmlPoints();
						XmlAttach.AttachTo(e.Mobile, m, x);
						count++;
					}
				}
			}
			e.Mobile.SendMessage("Added XmlPoints attachments to {0} players", count);
		}

		[Usage( "RemoveAllPoints" )]
		[Description( "Removes the XmlPoints attachment from all players" )]
		public static void RemoveAllPoints_OnCommand( CommandEventArgs e )
		{
			int count = 0;
			foreach(Mobile m in World.Mobiles.Values)
			{
				if(m.Player)
				{
					List<XmlAttachment> list = XmlAttach.FindAttachments(m, typeof(XmlPoints));
					if(list != null && list.Count > 0)
					{
						foreach(XmlAttachment x in list)
						{
							x.Delete();
						}
					}
					count++;
				}
			}
			e.Mobile.SendMessage("Removed XmlPoints attachments from {0} players",count);
		}

		// These are the various ways in which the message attachment can be constructed.
		// These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
		// Other overloads could be defined to handle other types of arguments

		// a serial constructor is REQUIRED
		public XmlPoints(ASerial serial) : base(serial)
		{
		}

		[Attachable]
		public XmlPoints()
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			
			// check for points decay
			if(m_Kills > 0 && m_PointsDecay > 0 && m_Points > DefaultStartingPoints && (DateTime.UtcNow - m_LastDecay) > m_PointsDecayTime &&
				(DateTime.UtcNow - m_LastKill) >  m_PointsDecayTime && (DateTime.UtcNow - m_LastDeath) >  m_PointsDecayTime)
			{
				m_Points -= m_PointsDecay;
				if(m_Points < DefaultStartingPoints) m_Points = DefaultStartingPoints;
				m_LastDecay = DateTime.UtcNow;
			}

			writer.Write( (int) 8 );
			// version 8
			writer.Write(m_StartingLoc);

			if(m_StartingMap != null)
			{
				writer.Write(m_StartingMap.ToString());
			} 
			else
			{
				writer.Write((string)String.Empty);
			}
			// version 7
			writer.Write(m_CurrentLanguage.ToString());
			// version 6
			writer.Write(m_ChallengeGame);
			writer.Write(m_ChallengeSetup);
			// version 5
			writer.Write(m_CancelEnd - DateTime.UtcNow);
			// version 4
			writer.Write(m_ReceiveBroadcasts);
			// version 3
			writer.Write(m_Rank);
			writer.Write(m_DeltaRank);
			writer.Write(m_WhenRanked);
			writer.Write(m_LastDecay);
			// version 2
			writer.Write(m_Credits);
			// version 1
			writer.Write(m_Broadcast);
			// version 0
			writer.Write(m_Points);
			writer.Write(m_Kills);
			writer.Write(m_Deaths);
			writer.Write(m_Challenger);
			writer.Write(m_LastKill);
			writer.Write(m_LastDeath);
			// write out the kill list
			if(KillList != null)
			{
				writer.Write(KillList.Count);
				foreach(KillEntry k in KillList)
				{
					writer.Write(k.Killed);
					writer.Write(k.WhenKilled);
				}
			} 
			else
			{
				writer.Write((int)0);
			}
			
			// need this in order to rebuild the rankings on deser
			if(AttachedTo is Mobile)
				writer.Write(AttachedTo as Mobile);
			else
				writer.Write((Mobile)null);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch(version)
			{
				case 8:
				{
					m_StartingLoc = reader.ReadPoint3D();
					try
					{
						m_StartingMap = Map.Parse(reader.ReadString());
					} 
					catch{}
					goto case 7;
				}
				case 7:
				{
					string langstr = reader.ReadString();
					try
					{
						m_CurrentLanguage = (LanguageType) Enum.Parse(typeof(LanguageType), langstr);
					} 
					catch {}
					goto case 6;
				}
				case 6:
				{
					m_ChallengeGame = (BaseChallengeGame)reader.ReadItem();
					m_ChallengeSetup = (BaseChallengeGame)reader.ReadItem();
					goto case 5;
				}
				case 5:
				{
					TimeSpan remaining = reader.ReadTimeSpan();
					if(remaining > TimeSpan.Zero)
					{
						DoTimer(remaining);
					}
					goto case 4;
				}
				case 4:
				{
					m_ReceiveBroadcasts = reader.ReadBool();
					goto case 3;
				}
				case 3:
				{
					m_Rank = reader.ReadInt();
					m_DeltaRank = reader.ReadInt();
					m_WhenRanked = reader.ReadDateTime();
					m_LastDecay = reader.ReadDateTime();
					goto case 2;
				}
				case 2:
				{
					m_Credits = reader.ReadInt();
					goto case 1;
				}
				case 1:
				{
					m_Broadcast = reader.ReadBool();
					goto case 0;
				}
				case 0:
				{
					m_Points = reader.ReadInt();
					m_Kills = reader.ReadInt();
					m_Deaths = reader.ReadInt();
					m_Challenger = reader.ReadMobile();
					m_LastKill = reader.ReadDateTime();
					m_LastDeath = reader.ReadDateTime();

					// read in the kill list
					int count = reader.ReadInt();
					KillList = new ArrayList();
					for(int i = 0;i<count;i++)
					{
						Mobile m = reader.ReadMobile();
						DateTime t = reader.ReadDateTime();
						if(m != null && !m.Deleted)
						{
							KillList.Add(new KillEntry(m,t));
						}
					}

					// get the owner of this in order to rebuild the rankings
					Mobile killer = reader.ReadMobile();

					// rebuild the ranking list
					// if they have never made a kill, then dont rank
					if(killer != null && m_Kills > 0)
					{
						UpdateRanking(killer, this);
					}
					break;
				}
			}
		}


		// updates the attachment kill list and removes expired entries
		private void RefreshKillList()
		{
			if(KillList != null)
			{
				ArrayList deletelist = new ArrayList();

				foreach(KillEntry k in KillList)
				{
					if(k.WhenKilled + m_KillDelay <= DateTime.UtcNow)
					{
						// expired so remove it from the list
						deletelist.Add(k);
					}
				}

				// clear out any expired entries
				if(deletelist.Count > 0)
				{
					foreach(KillEntry k in deletelist)
					{
						KillList.Remove(k);
					}
				}
			}
		}

		public override bool HandlesOnKill { get { return true; } }

		// handles point gain when the player kills someone
		public override void OnKill(Mobile killed, Mobile killer )
		{
			if(killer == null || killed == null || !(killed.Player) || killer == killed) return;

			bool awardpoints = true;

			// if this was a team or challenge duel then clear agressor list
			if(killed == m_Challenger || killer == m_Challenger || AreInSameGame(killed, killer))
			{
				// and remove the challenger from the aggressor list so that the res noto is not affected
				ClearAggression(killed, killer);
			}

			// handle challenge team kills
			if(ChallengeGame != null && !ChallengeGame.Deleted)
			{
				ChallengeGame.OnKillPlayer(killer, killed);
			}

			// check to see whether points can be given
			if(!(AttachedTo is Mobile) || !CanAffectPoints((Mobile)AttachedTo, killer, killed, false))
			{
				awardpoints = false;
			}

			// if this was a challenge duel then clear the challenger field
			if(killed == m_Challenger || killer == m_Challenger)
			{
				m_Challenger = null;
			}
			
			// begin the section to award points

			if(!awardpoints) return;

			if (LogKills)
			{
				try
				{
					using (StreamWriter op = new StreamWriter("kills.log", true))
					{
						op.WriteLine("{0}: {1} killed {2}", DateTime.UtcNow, killer, killed);
					}
				}
				catch { }
			}

			int killedpoints = 0;
			// give the killer his points, either a fixed amount or scaled by the difference with the points of the killed
			// if the killed has more points than the killed then gain more
			XmlPoints x = XmlAttach.FindAttachment(killed, typeof(XmlPoints)) as XmlPoints;

			if(x != null)
			{
				killedpoints = x.Points;
			}

			int val = (int)((killedpoints - Points)* m_WinScale);
			if(val <= 0) val = 1;

			Points += val;
			
			int cval = (int)((killedpoints - Points)* m_CreditScale);
			if(cval <= 0) cval = 1;

			Credits += cval;

			m_LastKill = DateTime.UtcNow;

			killer.SendMessage(String.Format(Text(100215), val, killed.Name));  // "You receive {0} points for killing {1}"
			
			if(GainHonorFromDuel)
			{
				bool gainedPath = false;
				if ( VirtueHelper.Award( killer, VirtueName.Honor, val, ref gainedPath ) )
				{
					if ( gainedPath )
					{
						killer.SendLocalizedMessage( 1063226 ); // You have gained a path in Honor!
					}
					else
					{
						killer.SendLocalizedMessage( 1063225 ); // You have gained in Honor.
					}
				}
			}


			// add to the recently killed list
			//KillList.Add(new KillEntry(killed, DateTime.UtcNow));

			// add to the cumulative death count
			Kills++;

			// update the overall ranking list
			UpdateRanking(killer, this);

			// if broadcast is enabled then announce it
			if(Broadcast && m_SystemBroadcast)
			{
				BroadcastMessage( AccessLevel.Player, 0x482, String.Format(SystemText(100216),killer.Name,killed.Name) );  // "{0} has defeated {1} in combat."
			}

			// update the points gump if it is open
			if(killer.HasGump(typeof(PointsGump)))
			{
				// redisplay it with the new info
				OnIdentify(killer);
			}

			// update the top players gump if it is open
			if(killer.HasGump(typeof(TopPlayersGump)))
			{
				killer.CloseGump(typeof(TopPlayersGump));
				killer.SendGump(new TopPlayersGump(this));
			}
		}

		public void ReportPointLoss_Callback(object state)
		{
			object[] args = (object[])state;
			int points = (int)args[0];
			string name = (string)args[1];
			Mobile m = (Mobile)args[2];

			if(m != null)
			{
				SendText(m, 100217, points, name); // "You lost {0} point(s) for being killed by {1}"
			}
		}

		public static void AutoRes_Callback(object state)
		{
			object[] args = (object[])state;

			Mobile m = (Mobile)args[0];
			bool refresh = (bool)args[1];

			if(m != null)
			{
				// auto tele ghosts to the corpse
				m.PlaySound( 0x214 );
				m.FixedEffect( 0x376A, 10, 16 );
				m.Resurrect();
				if(m.Corpse != null)
				{
					m.MoveToWorld(m.Corpse.Location, m.Corpse.Map);
					m.Corpse.OnDoubleClick(m);
					m.Corpse.LootType = LootType.Regular;
				}

				if(refresh)
				{
					m.Hits = m.HitsMax;
					m.Mana = int.MaxValue;
					m.Stam = int.MaxValue;
				}
			}
		}
		
		public static void Return_Callback(object state)
		{
			object[] args = (object[])state;

			Mobile killer = (Mobile)args[0];
			Mobile killed = (Mobile)args[1];
			Point3D loc = (Point3D)args[2];
			Map map = (Map)args[3];


			if(killer != null && killed != null && map != null && map != Map.Internal)
			{
				// auto tele players and corpses
				// if there were nearby pets/mounts then tele those as well

				ArrayList petlist = new ArrayList();
                IPooledEnumerable eable = killer.GetMobilesInRange(16);

				foreach(Mobile m in eable)
				{
					if(m is BaseCreature && ((BaseCreature)m).ControlMaster == killer)
					{
						petlist.Add(m);
					}
				}

                eable.Free();
                eable = killed.GetMobilesInRange(16);

				foreach(Mobile m in eable)
				{
					if(m is BaseCreature && ((BaseCreature)m).ControlMaster == killed)
					{
						petlist.Add(m);
					}
				}
                eable.Free();
				
				// port the pets
				foreach(Mobile m in petlist)
				{
					m.MoveToWorld(loc, map);
				}

				// port the killer and corpse
				killer.PlaySound( 0x214 );
				killer.FixedEffect( 0x376A, 10, 16 );
				killer.MoveToWorld(loc, map);
				if(killer.Corpse != null)
				{
					killer.Corpse.MoveToWorld(loc, map);
				}
				
				// port the killed and corpse
				killed.PlaySound( 0x214 );
				killed.FixedEffect( 0x376A, 10, 16 );
				killed.MoveToWorld(loc, map);
				if(killed.Corpse != null)
				{
					killed.Corpse.MoveToWorld(loc, map);
				}
				




			}
		}


		public bool CanAffectPoints(Mobile from, Mobile killer, Mobile killed, bool assumechallenge)
		{
			// uncomment this for newbie protection
			//if( ((killed.SkillsTotal < 6000 && (killer.SkillsTotal - killed.SkillsTotal ) > 1000) ||
			//(killed.RawStatTotal <= 200 && (killer.RawStatTotal - killed.RawStatTotal) > 20 ) ) && m_Challenger != killer && m_Challenger != killed) return false;

			// check for within guild kills and ignore them if this has been disabled
			if(!AllowWithinGuildPoints &&  SameGuild(killed,killer)) return false;

			// check for within team kills and ignore them
			if(AreTeamMembers(killer, killed)) return false;

			// are the players challengers?
			bool inchallenge = false;
			if((from == killer && m_Challenger == killed) || (from == killed && m_Challenger == killer))
			{
				inchallenge = true;
			}
			
			bool norestriction = UnrestrictedChallenges;
			
			// check for team challenges
			if(ChallengeGame != null && !ChallengeGame.Deleted)
			{
				// check to see if points have been disabled in this game
				if(!ChallengeGame.AllowPoints) return false;

				inchallenge = true;
				
				// check for kill delay limitations on points awards
				norestriction = !ChallengeGame.UseKillDelay;
			}

			// if UnlimitedChallenges has been set then allow points
			// otherwise, challenges have to obey the same restrictions on minimum time between kills as normal pvp
			if(norestriction && (inchallenge || assumechallenge)) return true;

			// only allow guild kills to yield points if in a challenge
			if(!(assumechallenge || inchallenge) &&  SameGuild(killed,killer)) return false;

			// uncomment the line below to limit points to challenges. regular pvp will not give points
			//if(!inchallenge && !assumechallenge) return false;

			// check to see whether killing the target would yield points
			// get a point for killing if they havent been killed recent

			// get the points attachment on the killer if this isnt the killer
			XmlPoints a = this;
			if (from != killer)
			{
				a = (XmlPoints)XmlAttach.FindAttachment(killer, typeof(XmlPoints));
			}
			if(a != null)
			{
				a.RefreshKillList();

				// check the kill list if there is one
				if(a.KillList != null)
				{
					foreach(KillEntry k in a.KillList)
					{
						if(k.WhenKilled + m_KillDelay > DateTime.UtcNow)
						{
							// found a match on the list so dont give any points
							if(k.Killed == killed)
							{
								return false;
							}
						}
					}
				}
			}

			// check to see whether the killed target could yield points
			if(from == killed)
			{
				// is it still within the minimum delay for being killed?
				if(DateTime.UtcNow < m_LastDeath + m_DeathDelay) return false;
			}

			return true;
		}

		public void ClearAggression(Mobile source, Mobile target)
		{

			// and remove the challenger from the aggressor list so that the res noto is not affected
			List<AggressorInfo> klist = target.Aggressors;
			if(klist != null && klist.Count > 0)
			{
				for(int j=0;j<klist.Count;j++)
				{
					if(((AggressorInfo)klist[j]).Attacker == source || ((AggressorInfo)klist[j]).Defender == source)
					{
						klist.Remove(klist[j]);
						break;
					}
				}
			}

			klist = target.Aggressed;
			if(klist != null && klist.Count > 0)
			{
				for(int j=0;j<klist.Count;j++)
				{
					if(((AggressorInfo)klist[j]).Attacker == source || ((AggressorInfo)klist[j]).Defender == source)
					{
						klist.Remove(klist[j]);
						break;
					}
				}
			}
		}


		public override bool HandlesOnKilled { get { return true; } }

		// handles point loss when the player is killed
		public override void OnKilled(Mobile killed, Mobile killer )
		{
			if(killer == null || killed == null || !(killer.Player) || killer == killed) return;

			bool awardpoints = true;

			// if this was a challenge duel then clear agression
			if(killed == m_Challenger || killer == m_Challenger || AreInSameGame(killer, killed))
			{
				ClearAggression( killer, killed);

				// and remove the challenger from the corpse aggressor list so that the corpse noto is not affected
				if(killed.Corpse is Corpse)
				{
					List<Mobile> klist = ((Corpse)killed.Corpse).Aggressors;
					if(klist != null && klist.Count > 0)
					{
						for(int j=0;j<klist.Count;j++)
						{
							if(klist[j] == killer)
							{
								klist.Remove(killer);
								break;
							}
						}
					}
				}
			}


			// check to see whether points can be taken
			if(!(AttachedTo is Mobile) || !CanAffectPoints((Mobile)AttachedTo, killer, killed, false))
			{
				awardpoints = false;
			}

			// handle challenge team kills
			if (ChallengeGame != null && !ChallengeGame.Deleted && ChallengeGame.GetParticipant(killer) != null && ChallengeGame.GetParticipant(killed) != null)
			{
				ChallengeGame.OnPlayerKilled(killer, killed);
			}


			if(killed == m_Challenger || killer == m_Challenger)
			{
				m_Challenger = null;

				if(AutoResAfterDuel)
				{
					// immediately bless the corpse to prevent looting
					if(killed.Corpse != null)
						killed.Corpse.LootType = LootType.Blessed;

					// prepare the autores callback
					Timer.DelayCall( TimeSpan.FromSeconds(6), new TimerStateCallback( AutoRes_Callback ),
						new object[]{ killed, false } );
				}
				
				if(TeleportOnDuel)
				{
					// teleport back to original location
					Timer.DelayCall( TimeSpan.FromSeconds(7), new TimerStateCallback( Return_Callback ),
						new object[]{ killer, killed, m_StartingLoc, m_StartingMap } );
				}

			}

			// begin the section to award points

			if(!awardpoints) return;

			int killerpoints = 0;

			// take points from the killed, either a fixed amount or scaled by the difference with the points of the killer
			// if the killer has fewer points than the killed then lose more
			XmlPoints xp = XmlAttach.FindAttachment(killer, typeof(XmlPoints)) as XmlPoints;

			if(xp != null)
			{
				killerpoints = xp.Points;


				// add to the recently killed list
				xp.KillList.Add(new KillEntry(killed, DateTime.UtcNow));
			}

			int val = (int)((Points - killerpoints)* m_LoseScale);
			if(val <= 0) val = 1;

			int startpoints = Points;

			Points -= val;

			// comment out this code if you dont want to have a zero floor and want to allow negative points
			if(Points < 0) 
				Points = 0;

			// add to the cumulative death count
			Deaths++;

			if(startpoints - Points > 0)
			{
				// prepare the message to report the point loss.  Need the delay otherwise it wont show up due to the death sequence
				Timer.DelayCall( TimeSpan.FromSeconds(5), new TimerStateCallback( ReportPointLoss_Callback ),
					new object[]{ startpoints - Points, killer.Name, killed } );

				// update the overall ranking list
				UpdateRanking(killed, this);
			}


			m_LastDeath = DateTime.UtcNow;

		}


		public override void OnAttach()
		{
			base.OnAttach();
			
			// only allow attachment to players
			if(!(AttachedTo is Mobile && ((Mobile)AttachedTo).Player))
				Delete();
		}


		public override string OnIdentify(Mobile from)
		{
			// uncomment this if you dont want players being able to check points/rank on other players
			//if((from != null) && (AttachedTo != from) && (from.AccessLevel == AccessLevel.Player)) return null;

			if(!(AttachedTo is Mobile)) return null;

			int val = GetRanking((Mobile)AttachedTo);

			StringBuilder msg = new StringBuilder();

			if(val > 0)
			{
				msg.AppendFormat(GetText(from, 100218), Points);  // "Current Points = {0}"
				msg.AppendFormat("\n");
				msg.AppendFormat(GetText(from, 100219), val);  // "Rank = {0}"
			}
			else
			{
				msg.AppendFormat(GetText(from, 100218), Points);  // "Current Points = {0}"
				msg.AppendFormat("\n");
				msg.AppendFormat(GetText(from, 100220));  // "No ranking."
			}

			// report the number of Credits available if the player is checking.  Dont display this if others are checking (unless they are staff).
			if((from != null) && ((AttachedTo == from) || (from.AccessLevel > AccessLevel.Player)))
			{
				msg.AppendFormat("\n");
				msg.AppendFormat(GetText(from, 100221), Credits);  // "Available Credits = {0}"
			}

			msg.AppendFormat("\n");
			msg.AppendFormat(GetText(from, 100222), Kills, Deaths); // "Total Kills = {0}\nTotal Deaths = {1}\nRecent Kill List"

			RefreshKillList();

			if(KillList != null && KillList.Count > 0)
			{
				foreach(KillEntry k in KillList)
				{
					if(k.Killed != null && !k.Killed.Deleted)
					{
						msg.AppendFormat("\n");
						msg.AppendFormat(GetText(from, 100223),k.Killed.Name, k.WhenKilled); // "{0} killed at {1}"
					}
				}
			}

			// display the points info gump
			if(from != null)
			{
				from.CloseGump(typeof(PointsGump));
				from.SendGump(new PointsGump(this, from, (Mobile)AttachedTo, msg.ToString()));
			}

			return null;
		}



		/*
		********************************************************************
		** Gumps section
		********************************************************************
		*/

		public class PointsGump : Gump
		{
			XmlPoints m_attachment;
			Mobile m_target;
			string m_text;

			public PointsGump( XmlPoints a, Mobile from, Mobile target, string text) : base( 0,0)
			{
				if(target == null || a == null) return;
				
				m_attachment = a;
				m_target = target;
				m_text = text;

				// prepare the page
				AddPage( 0 );
	
				if(from == target)
				{
					AddBackground( 0, 0, 440, 295, 5054 );
					AddAlphaRegion( 0, 0, 440, 295 );
				} 
				else
				{
					AddBackground( 0, 0, 440, 190, 5054 );
					AddAlphaRegion( 0, 0, 440, 190 );
				}
				AddLabel( 20, 2, 55, String.Format(GetText(from, 200224),target.Name) );  // "Points Standing for {0}"

				// 1 on 1 duel status
				if(a.Challenger != null)
				{
					int challengehue = 68;

					if(a.m_CancelTimer != null && a.m_CancelTimer.Running)
						challengehue = 33;
					// also check the challenger timer to see if he is cancelling
					XmlPoints ca = (XmlPoints)XmlAttach.FindAttachment(a.Challenger, typeof(XmlPoints));
					if(ca != null && !ca.Deleted)
					{
						if((ca.m_CancelTimer != null && ca.m_CancelTimer.Running) || (ca.ChallengeGame != null && ca.ChallengeGame.ChallengeBeingCancelled))
							challengehue = 33;
					}


					AddLabel( 20, 143, challengehue, String.Format(GetText(from, 200225),a.Challenger.Name) );  // "Currently challenging {0}"
				} 
				else
					// challenge game status
					if(a.ChallengeGame != null && !a.ChallengeGame.Deleted)
				{
					AddLabel( 50, 143, 68, String.Format("{0}",a.ChallengeGame.ChallengeName) );
					// add the info button that will open the game gump
					AddButton( 23, 143, 0x5689, 0x568A, 310, GumpButtonType.Reply, 0);

				}

				AddHtml( 20,20, 400, 120, text, true , true );

				int x1 = 20;
				int x2 = 150;
				int x3 = 290;

				if(from == target)
				{
					// add the see kills checkbox
					AddLabel( x1 + 30, 165, 55, a.Text(200226) );  // "See kills"
					AddButton( x1, 165, (a.ReceiveBroadcasts ? 0xD3 :0xD2), (a.ReceiveBroadcasts ? 0xD2 :0xD3), 100, GumpButtonType.Reply, 0);

					// add the broadcast kills checkbox
					AddLabel( x2 + 30, 165, 55, a.Text(200227) );  // "Broadcast kills"
					AddButton( x2, 165, (a.Broadcast ? 0xD3 :0xD2), (a.Broadcast ? 0xD2 :0xD3), 200, GumpButtonType.Reply, 0);

					// add the topplayers button
					AddLabel( x3 + 30, 165, 55, a.Text(200228) );  // "Top players"
					AddButton( x3, 165, 0xFAB, 0xFAD, 300, GumpButtonType.Reply, 0);

					// add the challenge button
					AddLabel( x1 + 30, 190, 55, a.Text(200229) );   // "Challenge"
					AddButton( x1, 190, 0xFAB, 0xFAD, 400, GumpButtonType.Reply, 0);

					// add the last man standing challenge button
					AddLabel( x2 + 30, 190, 55, a.Text(200230) );   // "LMS"
					AddButton( x2, 190, 0xFAB, 0xFAD, 401, GumpButtonType.Reply, 0);

					// add the deathmatch challenge button
					AddLabel( x3 + 30, 190, 55, a.Text(200231) );  // "Deathmatch"
					AddButton( x3, 190, 0xFAB, 0xFAD, 403, GumpButtonType.Reply, 0);

					// add the kingofthehill challenge button
					AddLabel( x1 + 30, 215, 55, a.Text(200232) );  // "KotH"
					AddButton( x1, 215, 0xFAB, 0xFAD, 404, GumpButtonType.Reply, 0);

					// add the deathball challenge button
					AddLabel( x2 + 30, 215, 55, a.Text(200233) );  // "DeathBall"
					AddButton( x2, 215, 0xFAB, 0xFAD, 405, GumpButtonType.Reply, 0);

					// add the teamlms challenge button
					AddLabel( x3 + 30, 215, 55, a.Text(200234) );  // "Team LMS"
					AddButton( x3, 215, 0xFAB, 0xFAD, 406, GumpButtonType.Reply, 0);

					// add the team deathmatch challenge button
					AddLabel( x1 + 30, 240, 55, a.Text(200235) );   // "Team DMatch"
					AddButton( x1, 240, 0xFAB, 0xFAD, 407, GumpButtonType.Reply, 0);

					// add the team deathball challenge button
					AddLabel( x2 + 30, 240, 55, a.Text(200236) );  // "Team DBall"
					AddButton( x2, 240, 0xFAB, 0xFAD, 408, GumpButtonType.Reply, 0);

					// add the team KotH challenge button
					AddLabel( x3 + 30, 240, 55, a.Text(200237) );  // "Team KotH"
					AddButton( x3, 240, 0xFAB, 0xFAD, 409, GumpButtonType.Reply, 0);
					
					// add the CTF challenge button
					AddLabel( x1 + 30, 265, 55, a.Text(200238) );  // "CTF"
					AddButton( x1, 265, 0xFAB, 0xFAD, 410, GumpButtonType.Reply, 0);
				}
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{

				if(m_attachment == null || state == null || state.Mobile == null || info == null) return;

				switch(info.ButtonID)
				{
					case 100:
						// toggle see kills
						m_attachment.ReceiveBroadcasts = !m_attachment.ReceiveBroadcasts;

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;
					case 200:
						// toggle broadcast my kills
						m_attachment.Broadcast = !m_attachment.Broadcast;

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;
					case 300:
						// top players
						state.Mobile.CloseGump(typeof( TopPlayersGump ));
						state.Mobile.SendGump(new TopPlayersGump(m_attachment));

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;
					case 310:
						// Challenge game info
						if(m_attachment.ChallengeGame != null && !m_attachment.ChallengeGame.Deleted)
							m_attachment.ChallengeGame.OnDoubleClick(state.Mobile);

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;
					case 400:
						// 1 on 1 challenge duel
						state.Mobile.Target = new ChallengeTarget(state.Mobile);

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;
					case 401:
						// last man standing
						BaseChallengeGame.DoSetupChallenge(state.Mobile, 100302, typeof(LastManStandingGauntlet));

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;
					case 403:
						// deathmatch challenge
						BaseChallengeGame.DoSetupChallenge(state.Mobile, 100400, typeof(DeathmatchGauntlet));

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;
					case 404:
						// kingofthehill challenge
						BaseChallengeGame.DoSetupChallenge(state.Mobile, 100410, typeof(KingOfTheHillGauntlet));

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;
					case 405:
						// deathball challenge
						BaseChallengeGame.DoSetupChallenge(state.Mobile, 100411, typeof(DeathBallGauntlet));

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;
					case 406:
						// team lms challenge
						BaseChallengeGame.DoSetupChallenge(state.Mobile, 100413, typeof(TeamLMSGauntlet));

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;
					case 407:
						// team deathmatch challenge
						BaseChallengeGame.DoSetupChallenge(state.Mobile, 100415, typeof(TeamDeathmatchGauntlet));

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;
					case 408:
						// team deathball challenge
						BaseChallengeGame.DoSetupChallenge(state.Mobile, 100416, typeof(TeamDeathballGauntlet));

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;
					case 409:
						// team KotH challenge
						BaseChallengeGame.DoSetupChallenge(state.Mobile, 100417, typeof(TeamKotHGauntlet));

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;
					case 410:
						// CTF challenge
						BaseChallengeGame.DoSetupChallenge(state.Mobile, 100418, typeof(CTFGauntlet));

						state.Mobile.SendGump( new PointsGump(m_attachment, state.Mobile, m_target, m_text));
						break;

				}
			}
		}

		private string guildFilter;
		private string nameFilter;

		public class TopPlayersGump : Gump
		{
			private XmlPoints m_attachment;

			public TopPlayersGump(XmlPoints attachment) : base( 0,0)
			{
				if(RankList == null || attachment == null) return;

				m_attachment = attachment;

				int numberToDisplay = 20;
				int height = numberToDisplay*20 + 65;

				// prepare the page
				AddPage( 0 );

				int width = 740;
#if(FACTIONS)
				width = 790;
#endif

				AddBackground( 0, 0, width, height, 5054 );
				AddAlphaRegion( 0, 0, width, height );
				AddImageTiled( 20, 20, width - 40, height - 45, 0xBBC );
				AddLabel( 20, 2, 55, attachment.Text(200239) ); // "Top Player Rankings"

				// guild filter
				AddLabel( 40, height - 20, 55, attachment.Text(200240) );  // "Filter by Guild"
				string filter = null;
				if(m_attachment != null)
					filter = m_attachment.guildFilter;

				AddImageTiled( 140, height - 20, 160, 19, 0xBBC );
				AddTextEntry( 140, height - 20, 160, 19, 0, 200, filter );

				AddButton( 20, height - 20, 0x15E1, 0x15E5, 200, GumpButtonType.Reply, 0 );

				// name filter
				AddLabel( 340, height - 20, 55, attachment.Text(200241) );  // "Filter by Name"
				string nfilter = null;
				if(m_attachment != null)
					nfilter = m_attachment.nameFilter;

				AddImageTiled( 440, height - 20, 160, 19, 0xBBC );
				AddTextEntry( 440, height - 20, 160, 19, 0, 100, nfilter );

				AddButton( 320, height - 20, 0x15E1, 0x15E5, 100, GumpButtonType.Reply, 0 );

				RefreshRankList();

				int xloc = 23;
				AddLabel( xloc, 20, 0, attachment.Text(200242) );  // "Name"
				xloc += 177;
				AddLabel( xloc, 20, 0, attachment.Text(200243) );  // "Guild"
#if(FACTIONS)
				xloc += 35;
				AddLabel( xloc, 20, 0, attachment.Text(200640) );  // "Faction"
				xloc += 15;
#endif
				xloc += 50;
				AddLabel( xloc, 20, 0, attachment.Text(200244) );   // "Points"
				xloc += 50;
				AddLabel( xloc, 20, 0, attachment.Text(200245) );    // "Kills"
				xloc += 50;
				AddLabel( xloc, 20, 0, attachment.Text(200246) );   // "Deaths"
				xloc += 70;
				AddLabel( xloc, 20, 0, attachment.Text(200247) );     // "Rank"
				xloc += 45;
				AddLabel( xloc, 20, 0, attachment.Text(200248) );   // "Change"
				xloc += 45;
				AddLabel( xloc, 20, 0, attachment.Text(200249) );  // "Time at Rank"

				// go through the sorted list and display the top ranked players

				int y = 40;
				int count = 0;
				for(int i= 0; i<RankList.Count;i++)
				{
					if(count >= numberToDisplay) break;

					RankEntry r = RankList[i] as RankEntry;

					if(r == null) continue;

					XmlPoints a = r.PointsAttachment;

					if(a == null) continue;

					if(r.Killer != null && !r.Killer.Deleted && r.Rank > 0 && a != null && !a.Deleted)
					{
						string guildname = null;

						if(r.Killer.Guild != null) guildname = r.Killer.Guild.Abbreviation;

#if(FACTIONS)
						string factionname = null;
	
						if(r.Killer is PlayerMobile && ((PlayerMobile)r.Killer).FactionPlayerState != null) 
							factionname = ((PlayerMobile)r.Killer).FactionPlayerState.Faction.ToString();
#endif
						// check for any ranking change and update rank date
						if(r.Rank != a.Rank)
						{
							a.WhenRanked = DateTime.UtcNow;
							if(a.Rank > 0)
								a.DeltaRank = a.Rank - r.Rank;
							a.Rank = r.Rank;

						}

						// check for guild filter
						if(m_attachment != null && m_attachment.guildFilter != null && m_attachment.guildFilter.Length > 0)
						{
							// parse the comma separated list
							string [] args = m_attachment.guildFilter.Split(',');
							if(args != null)
							{
								bool found = false;
								foreach(string arg in args)
								{
									if(arg != null && guildname == arg.Trim())
									{
										found = true;
										break;
									}
								}
								if(!found) continue;
							}
						}

						// check for name filter
						if(m_attachment != null && m_attachment.nameFilter != null && m_attachment.nameFilter.Length > 0)
						{
							// parse the comma separated list
							string [] args = m_attachment.nameFilter.Split(',');

							if(args != null)
							{
								bool found = false;
								foreach(string arg in args)
								{
									if(arg != null && r.Killer.Name != null && (r.Killer.Name.ToLower().IndexOf(arg.Trim().ToLower()) >= 0))
									{
										found = true;
										break;
									}
								}
								if(!found) continue;
							}
						}

						count++;

						TimeSpan timeranked = DateTime.UtcNow - a.WhenRanked;

						int days = (int)timeranked.TotalDays;
						int hours = (int)(timeranked.TotalHours - days*24);
						int mins = (int)(timeranked.TotalMinutes - ((int)timeranked.TotalHours)*60);

						string kills = "???";
						try
						{
							kills = a.Kills.ToString();
						} 
						catch{}

						string deaths = "???";
						try
						{
							deaths = a.Deaths.ToString();
						} 
						catch{}

						xloc = 23;
						AddLabel( xloc, y, 0, r.Killer.Name );
						xloc += 177;
						AddLabel( xloc, y, 0, guildname );
#if(FACTIONS)
						xloc += 35;
						AddLabelCropped( xloc, y, 60, 21, 0, factionname );
						xloc += 15;
#endif
						xloc += 50;
						AddLabel( xloc, y, 0, a.Points.ToString() );
						xloc += 50;
						AddLabel( xloc, y, 0, kills );
						xloc += 50;
						AddLabel( xloc, y, 0, deaths );
						xloc += 70;
						AddLabel( xloc, y, 0, a.Rank.ToString() );

						string label=null;

						if(days > 0)
							label += String.Format(attachment.Text(200250),days);  // "{0} days "
						if(hours > 0)
							label += String.Format(attachment.Text(200251),hours);  // "{0} hours "
						if(mins > 0)
							label += String.Format(attachment.Text(200252),mins);   // "{0} mins"

						if(label == null)
						{
							label = attachment.Text(200253);   // "just changed"
						}

						string deltalabel = a.DeltaRank.ToString();
						int deltahue = 0;
						if(a.DeltaRank > 0)
						{
							deltalabel = String.Format("+{0}",a.DeltaRank);
							deltahue = 68;
						} 
						else
							if(a.DeltaRank < 0)
						{
							deltahue = 33;
						}
						xloc += 50;
						AddLabel( xloc, y, deltahue, deltalabel );
						xloc += 40;
						AddLabel( xloc, y, 0, label);

						y += 20;
					}
				}
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{
				if(state == null || state.Mobile == null || info == null) return;
				// Get the current name
				if(m_attachment != null)
				{
					TextRelay entry = info.GetTextEntry( 200 );
					if(entry != null)
						m_attachment.guildFilter = entry.Text;
						
					entry = info.GetTextEntry( 100 );
					if(entry != null)
						m_attachment.nameFilter = entry.Text;
				}
				
				switch(info.ButtonID)
				{
					case 100:
					case 200:
					{
						// redisplay the gump
						state.Mobile.SendGump(new TopPlayersGump(m_attachment));
						break;
					}
				}
			}
		}
		
		private class IssueChallengeGump : Gump
		{
			private Mobile m_From;
			private Mobile m_Target;


			public IssueChallengeGump(Mobile from, Mobile target) : base ( 0, 0 )
			{
				if(from == null || target == null) return;

				if (!AllowChallengeGump(from, target))
				{
					from.SendMessage(SystemText(100267)); // "You cannot issue a challenge here."
					return;
				}

				m_From = from;
				m_Target = target;

				XmlPoints a = (XmlPoints)XmlAttach.FindAttachment(from, typeof(XmlPoints));

				XmlPoints atarg = (XmlPoints)XmlAttach.FindAttachment(target, typeof(XmlPoints));
				
				from.CloseGump(typeof( IssueChallengeGump ));
				
				if(a == null || a.Deleted || atarg == null || atarg.Deleted)
				{
					from.SendMessage(SystemText(100213)); // "No XmlPoints support."
					return;
				}
				
				// figure out how many duel locations
				int locsize = XmlPoints.DuelLocations.Length;
				
				if(!TeleportOnDuel) locsize = 0;

				int height = 170 + locsize*30;

				Closable = false;
				Dragable = true;
				AddPage( 0 );
				AddBackground( 10, 200, 200, height, 5054 );

				AddLabel( 20, 205, 68, String.Format(GetText(from, 200254)) ); // "You are challenging"
				AddLabel( 20, 225, 68, String.Format(GetText(from, 200255),target.Name) );  // "{0}. Continue?"


				// display the available duel locations

				int y = 250;
				int texthue = 0;

				AddLabel( 55, y, texthue, String.Format(GetText(from, 200660)) );  // "Cancel"
				AddRadio( 20, y, 9721, 9724, true, 0 );
				y += 30;

				AddLabel( 55, y, texthue, String.Format(GetText(from, 200661)) );  // "Duel here"
				AddRadio( 20, y, 9721, 9724, false, 1 );
				y+= 30;

				// block teleporting if in a recall-restricted region
				if(TeleportOnDuel  && SpellHelper.CheckTravel(from.Map, from.Location, TravelCheckType.RecallFrom) && SpellHelper.CheckTravel(target.Map, target.Location, TravelCheckType.RecallFrom))
				{
					for(int i = 0; i < XmlPoints.DuelLocations.Length; i++)
					{
						// check availability

						if(!DuelLocationAvailable(XmlPoints.DuelLocations[i]))
						{
							texthue = 33;
						} 
						else
						{
							texthue = 0;
						}
						AddLabel( 55, y, texthue, XmlPoints.DuelLocations[i].Name );
						AddRadio( 20, y, 9721, 9724, false, i+2 );
						y += 30;
	
					}
				}

				// check to see if points can be gained from this
				if(a == null || a.Deleted || atarg == null || atarg.Deleted || !a.CanAffectPoints(from, from, target, true))
				{
					AddLabel( 20, y, 33, String.Format(GetText(from, 200256)) );  // "You will NOT gain points!"

				}
				y += 30;
				/*
				y += 25;
				AddRadio( 35, y, 9721, 9724, false, 1 ); // accept/yes radio
				AddRadio( 135, y, 9721, 9724, true, 2 ); // decline/no radio

				AddHtmlLocalized(72, y, 200, 30, 1049016, 0x7fff , false , false ); // Yes
				AddHtmlLocalized(172, y, 200, 30, 1049017, 0x7fff , false , false ); // No

				*/

				AddButton( 80, y, 2130, 2129, 3, GumpButtonType.Reply, 0 ); // Okay button
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{

				if(info == null || state == null || state.Mobile == null) return;



				int radiostate = -1;
				if(info.Switches.Length > 0)
				{
					radiostate = info.Switches[0];
				}



				switch(info.ButtonID)
				{
					default:
					{
						if(radiostate > 1)
						{
							// issue the challenge
							m_Target.SendGump(new ConfirmChallengeGump(m_From, m_Target, XmlPoints.DuelLocations[radiostate-2]));
							m_From.SendMessage(GetText(m_From, 100257), m_Target.Name);  // "You have issued a challenge to {0}."
						} 
						else
							if(radiostate == 1)
						{
							// issue the challenge
							m_Target.SendGump(new ConfirmChallengeGump(m_From, m_Target, null));
							m_From.SendMessage(GetText(m_From, 100257), m_Target.Name);  // "You have issued a challenge to {0}."

						} 
						else
							if(radiostate == 0)
						{
							if(m_From != null)
								m_From.SendMessage(GetText(m_From, 100258), m_Target.Name);  // "You decided against challenging {0}."
						}
						break;
					}
				}
			}
		}


		private class ConfirmChallengeGump : Gump
		{
			private Mobile m_From;
			private Mobile m_Target;
			private DuelLocationEntry m_DuelLocation;

			public ConfirmChallengeGump(Mobile from, Mobile target, DuelLocationEntry duelloc) : base ( 0, 0 )
			{

				if(target == null || from == null) return;

				// uncomment the line below to log all challenges into the command log
				//CommandLogging.WriteLine( from, "{0} {1} challenged {2}", from.AccessLevel, CommandLogging.Format( from ), CommandLogging.Format( target ));

				m_From = from;
				m_Target = target;
				m_DuelLocation = duelloc;

				XmlPoints a = (XmlPoints)XmlAttach.FindAttachment(from, typeof(XmlPoints));

				XmlPoints atarg = (XmlPoints)XmlAttach.FindAttachment(target, typeof(XmlPoints));

				Closable = false;
				Dragable = true;
				AddPage( 0 );
				AddBackground( 10, 200, 200, 150, 5054 );

				AddLabel( 20, 205, 68, String.Format(GetText(target, 200259)) );  // "You have been challenged by"
				AddLabel( 20, 225, 68, String.Format(GetText(target, 200260),from.Name) );      // "{0}. Accept?"
				
				int y = 250;
				if(m_DuelLocation != null)
				{
					AddLabel( 20, y, 0, String.Format(GetText(target, 200649), m_DuelLocation.Name) );  // "Location: {0}"
				} 
				else
				{
					AddLabel( 20, y, 0, String.Format(GetText(target, 200649), GetText(target, 200661)) );  // "Location: Duel Here"
				}
				y+= 20;


				if(a == null || a.Deleted || atarg == null || atarg.Deleted || !atarg.CanAffectPoints(target, target, from, true))
				{
					AddLabel( 20, y, 33, String.Format(GetText(target, 200256)) );   // "You will NOT gain points!"
				}

				AddRadio( 35, 290, 9721, 9724, false, 1 ); // accept/yes radio
				AddRadio( 135, 290, 9721, 9724, true, 2 ); // decline/no radio
				AddHtmlLocalized(72, 290, 200, 30, 1049016, 0x7fff , false , false ); // Yes
				AddHtmlLocalized(172, 290, 200, 30, 1049017, 0x7fff , false , false ); // No

				AddButton( 80, 320, 2130, 2129, 3, GumpButtonType.Reply, 0 ); // Okay button
			}

			public override void OnResponse( NetState state, RelayInfo info )
			{

				if(info == null || state == null || state.Mobile == null || m_From == null || m_Target == null) return;

				int radiostate = -1;
				if(info.Switches.Length > 0)
				{
					radiostate = info.Switches[0];
				}


				switch(info.ButtonID)
				{
					default:
					{
						if(radiostate == 1)
						{
							// challenge accept
							// check to make sure the duel location is available
							if(m_DuelLocation != null && !XmlPoints.DuelLocationAvailable(m_DuelLocation))
							{
								SendText(m_Target, 200650, m_DuelLocation.Name); // "{0} is occupied."
								return;
							}
							
							// make sure neither participant is in combat
							if(CheckCombat(m_From))
							{
								SendText(m_From, 100214, m_Target.Name); // "Challenge with {0} has been cancelled"
								SendText(m_From, 100670, m_From.Name);  // "{0} is in combat."
								SendText(m_Target, 100214, m_From.Name); // "Challenge with {0} has been cancelled"
								SendText(m_Target, 100670, m_From.Name);  // "{0} is in combat."
								return;
							}
							
							// make sure neither participant is in combat
							if(CheckCombat(m_Target))
							{
								SendText(m_From, 100214, m_Target.Name); // "Challenge with {0} has been cancelled"
								SendText(m_From, 100670, m_Target.Name);  // "{0} is in combat."
								SendText(m_Target, 100214, m_From.Name); // "Challenge with {0} has been cancelled"
								SendText(m_Target, 100670, m_Target.Name);  // "{0} is in combat."
								return;
							}

							XmlPoints a = (XmlPoints)XmlAttach.FindAttachment(m_From, typeof(XmlPoints));

							// first confirm that they dont already have a challenge going
							if(a != null && !a.Deleted && (a.Challenger != null || a.ChallengeGame != null))
							{
								SendText(m_Target, 100261, m_From.Name); // "{0} has already been challenged."
								
								SendText(m_From, 100262);  // "You are already being challenged."
								return;
							}

							XmlPoints ta = (XmlPoints)XmlAttach.FindAttachment(m_Target, typeof(XmlPoints));

							// first confirm that they dont already have a challenge going
							if(ta != null && !ta.Deleted && (ta.Challenger != null || ta.ChallengeGame != null))
							{
								SendText(m_Target, 100262);  // "You are already being challenged."

								SendText(m_From, 100261, m_Target.Name);  // "{0} has already been challenged."
								return;
							}
							
							// if they accept then assign the challenger fields on their points attachments
							if(a != null && !a.Deleted)
							{
								a.Challenger = m_Target;
							}

							// assign the challenger field on the target points attachment
							if(ta != null && !ta.Deleted)
							{
								ta.Challenger = m_From;
							}

							// notify the challenger and set up noto
							SendText(m_From, 100263, m_Target.Name);   // "{0} accepted your challenge!"
							m_From.Send( new MobileMoving( m_Target, Notoriety.Compute( m_From, m_Target ) ) );

							// update the points gump if it is open
							if(m_From.HasGump(typeof(PointsGump)))
							{
								// redisplay it with the new info
								if(a != null && !a.Deleted)
									a.OnIdentify(m_From);
							}

							// notify the challenged and set up noto
							SendText(m_Target, 100264, m_From.Name);  // "You have accepted the challenge from {0}!"
							m_Target.Send( new MobileMoving( m_From, Notoriety.Compute( m_Target, m_From ) ) );
							
							// update the points gump if it is open
							if(m_Target.HasGump(typeof(PointsGump)))
							{
								// redisplay it with the new info
								if(ta != null && !ta.Deleted)
									ta.OnIdentify(m_Target);
							}

							// cancel any precast spells
							m_Target.Spell = null;
							m_Target.Target = null;
							m_From.Spell = null;
							m_From.Target = null;

							// let the challenger pick the dueling site
							if(TeleportOnDuel && m_DuelLocation != null)
							{

								Point3D duelloc = m_DuelLocation.DuelLocation;
								Map duelmap = m_DuelLocation.DuelMap;
								a.m_StartingLoc = m_From.Location;
								a.m_StartingMap = m_From.Map;
								ta.m_StartingLoc = m_Target.Location;
								ta.m_StartingMap = m_Target.Map;
								m_Target.MoveToWorld(duelloc, duelmap);
								
								// move over by 1
								duelloc.X += 1;
								m_From.MoveToWorld(duelloc, duelmap);
							} 
							else
							{
								a.m_StartingMap = null;
								ta.m_StartingMap = null;
							}

						} 
						else
						{
							SendText(m_From, 100265, m_Target.Name); // "Your challenge to {0} was declined."
							SendText(m_Target, 100266, m_From.Name);  // "You declined the challenge by {0}."
						}
						break;
					}
				}
			}
		}
	}
}
