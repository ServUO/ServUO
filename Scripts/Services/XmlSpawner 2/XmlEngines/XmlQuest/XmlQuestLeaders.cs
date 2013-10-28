#define FACTIONS

#region References
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

using CustomsFramework;

using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
#endregion

namespace Server.Engines.XmlSpawner2
{
	public class XmlQuestLeaders
	{
		// default directory for saving quest leaderboard xml information
		private const string _QuestLeaderboardSaveDirectory = "Leaderboard";

		// default time interval between quest leaderboard saves
		private static TimeSpan _QuestLeaderboardSaveInterval = TimeSpan.FromMinutes(15);

		// number of ranked players to save to the quest leaderboard.  0 means save all players.
		private static int _QuestLeaderboardSaveRanks = 20;

		private static string _QuestLeaderboardFile;
		private static bool _NeedsUpdate = true;

		private static QuestLeaderboardTimer _QuestLeaderboardTimer;
		private static List<QuestRankEntry> _QuestRankList = new List<QuestRankEntry>();

		private static void RefreshQuestRankList()
		{
			if (!_NeedsUpdate || _QuestRankList == null)
			{
				return;
			}

			_QuestRankList.Sort();

			int rank = 0;
			//int prevPoints = 0;
			foreach (QuestRankEntry p in _QuestRankList.Cast<object>().Select(t => t as QuestRankEntry))
			{
				/*
				// bump the rank for every change in point level
				// this means that people with the same points score will have the same rank				
                if(p.QuestPointsAttachment.Points != prevPoints)
                {
					rank++;
                }

                prevpoints = p.QuestPointsAttachment.Points;
                */

				// bump the rank for every successive player in the list.  Players with the same points total will be
				// ordered by quests completed
				rank++;

				p.Rank = rank;
			}

			_NeedsUpdate = false;
		}

		public static int GetQuestRanking(Mobile m)
		{
			if (_QuestRankList == null || m == null)
			{
				return 0;
			}

			RefreshQuestRankList();

			// go through the sorted list and calculate rank
			// rank 0 means unranked
			return (_QuestRankList.Where(p => p.Quester == m).Select(p => p.Rank)).FirstOrDefault();
		}

		public static void UpdateQuestRanking(Mobile m, XmlQuestPoints attachment)
		{
			if (_QuestRankList == null)
			{
				_QuestRankList = new List<QuestRankEntry>();
			}

			// flag the rank list for updating on the next attempt to retrieve a rank
			_NeedsUpdate = true;

			bool found = false;

			// rank the entries
			foreach (QuestRankEntry p in _QuestRankList.Where(p => p != null && p.Quester == m))
			{
				// update the entry with the new points value
				p.QuestPointsAttachment = attachment;
				found = true;
				break;
			}

			// a new entry so add it
			if (!found)
			{
				_QuestRankList.Add(new QuestRankEntry(m, attachment));
			}
		}

		public static void Initialize()
		{
			CommandSystem.Register("QuestLeaderboardSave", AccessLevel.Administrator, QuestLeaderboardSave_OnCommand);
			CommandSystem.Register("QuestRanking", AccessLevel.Player, QuestRanking_OnCommand);
		}

		[Usage("QuestRanking")]
		[Description("Displays the top players in quest points")]
		public static void QuestRanking_OnCommand(CommandEventArgs e)
		{
			if (e == null || e.Mobile == null)
			{
				return;
			}

			// if this player has an XmlQuestPoints attachment, find it
			XmlQuestPoints p = (XmlQuestPoints)XmlAttach.FindAttachment(e.Mobile, typeof(XmlQuestPoints));

			e.Mobile.CloseGump(typeof(TopQuestPlayersGump));
			e.Mobile.SendGump(new TopQuestPlayersGump(p));
		}

		public static void WriteQuestLeaderboardXml(string filename, int nranks)
		{
			string dirname = Path.Combine(_QuestLeaderboardSaveDirectory, filename);

			StreamWriter sw = new StreamWriter(dirname);

			XmlTextWriter xf = new XmlTextWriter(sw)
			{
				Formatting = Formatting.Indented
			};

			xf.WriteStartDocument(true);
			xf.WriteStartElement("QuestLeaderboard");

			xf.WriteAttributeString(
				"nentries",
				nranks > 0
					? nranks.ToString(CultureInfo.InvariantCulture)
					: _QuestRankList.Count.ToString(CultureInfo.InvariantCulture));

			// go through the sorted list and display the top ranked players

			for (int i = 0; i < _QuestRankList.Count; i++)
			{
				if (nranks > 0 && i >= nranks)
				{
					break;
				}

				QuestRankEntry r = _QuestRankList[i];
				XmlQuestPoints a = r.QuestPointsAttachment;

				if (r.Quester == null || r.Quester.Deleted || r.Rank <= 0 || a == null || a.Deleted)
				{
					continue;
				}

				string guildName = null;

				if (r.Quester.Guild != null)
				{
					guildName = r.Quester.Guild.Abbreviation;
				}

#if(FACTIONS)
				string factionName = null;

				if (r.Quester is PlayerMobile && ((PlayerMobile)r.Quester).FactionPlayerState != null)
				{
					factionName = ((PlayerMobile)r.Quester).FactionPlayerState.Faction.ToString();
				}
#endif

				// check for any ranking change and update rank date
				if (r.Rank != a.Rank)
				{
					a.WhenRanked = DateTime.UtcNow;

					if (a.Rank > 0)
					{
						a.DeltaRank = a.Rank - r.Rank;
					}

					a.Rank = r.Rank;
				}

				TimeSpan timeRanked = DateTime.UtcNow - a.WhenRanked;

				// write out the entry information

				xf.WriteStartElement("Entry");
				xf.WriteAttributeString("number", i.ToString(CultureInfo.InvariantCulture));

				xf.WriteStartElement("Player");
				xf.WriteString(r.Quester.RawName ?? String.Empty);
				xf.WriteEndElement();

				xf.WriteStartElement("Guild");
				xf.WriteString(guildName ?? String.Empty);
				xf.WriteEndElement();
#if(FACTIONS)
				xf.WriteStartElement("Faction");
				xf.WriteString(factionName ?? String.Empty);
				xf.WriteEndElement();
#endif
				xf.WriteStartElement("Points");
				xf.WriteString(a.Points.ToString(CultureInfo.InvariantCulture));
				xf.WriteEndElement();

				string quests = "???";

				try
				{
					quests = a.QuestsCompleted.ToString(CultureInfo.InvariantCulture);
				}
				catch
				{ }

				xf.WriteStartElement("Quests");
				xf.WriteString(quests);
				xf.WriteEndElement();

				xf.WriteStartElement("Rank");
				xf.WriteString(a.Rank.ToString(CultureInfo.InvariantCulture));
				xf.WriteEndElement();

				xf.WriteStartElement("Change");
				xf.WriteString(a.DeltaRank.ToString(CultureInfo.InvariantCulture));
				xf.WriteEndElement();

				xf.WriteStartElement("Duration");
				xf.WriteString(timeRanked.ToString());
				xf.WriteEndElement();

				// end the entry
				xf.WriteEndElement();
			}

			xf.WriteEndElement();

			xf.Close();
		}

		public static void WriteQuestLeaderboardHtml(string filename, int nranks)
		{
			string dirName = Path.Combine(_QuestLeaderboardSaveDirectory, filename);

			StreamWriter sw = new StreamWriter(dirName);

			sw.WriteLine("<TABLE border=\"1\" summary=\"This table gives quest leaderboard stats\"> ");
			sw.WriteLine("<CAPTION><B>Quest Leaderboard</B></CAPTION>");

#if(FACTIONS)
			sw.WriteLine(
				"<TR><TH><TH>Player Name<TH>Guild<TH>Faction<TH>Points<TH>Quests<TH>Rank<TH>Change<TH>Time at current rank");
#else
            sw.WriteLine(
				"<TR><TH><TH>Player Name<TH>Guild<TH>Points<TH>Quests<TH>Rank<TH>Change<TH>Time at current rank");
#endif

			// go through the sorted list and display the top ranked players
			for (int i = 0; i < _QuestRankList.Count; i++)
			{
				if (nranks > 0 && i >= nranks)
				{
					break;
				}

				QuestRankEntry r = _QuestRankList[i];
				XmlQuestPoints a = r.QuestPointsAttachment;

				if (r.Quester == null || r.Quester.Deleted || r.Rank <= 0 || a == null || a.Deleted)
				{
					continue;
				}

				string guildName = null;

				if (r.Quester.Guild != null)
				{
					guildName = r.Quester.Guild.Abbreviation;
				}

#if(FACTIONS)
				string factionName = null;

				if (r.Quester is PlayerMobile && ((PlayerMobile)r.Quester).FactionPlayerState != null)
				{
					factionName = ((PlayerMobile)r.Quester).FactionPlayerState.Faction.ToString();
				}
#endif

				// check for any ranking change and update rank date
				if (r.Rank != a.Rank)
				{
					a.WhenRanked = DateTime.UtcNow;

					if (a.Rank > 0)
					{
						a.DeltaRank = a.Rank - r.Rank;
					}

					a.Rank = r.Rank;
				}

				TimeSpan timeRanked = DateTime.UtcNow - a.WhenRanked;

				string quests = "???";

				try
				{
					quests = a.QuestsCompleted.ToString(CultureInfo.InvariantCulture);
				}
				catch
				{ }

#if(FACTIONS)
				// write out the entry information
				sw.WriteLine(
					"<TR><TH><TD>{0}<TD>{1}<TD>{2}<TD>{3}<TD>{4}<TD>{5}<TD>{6}<TD>{7}",
					r.Quester.Name,
					guildName,
					factionName,
					a.Points,
					quests,
					a.Rank,
					a.DeltaRank,
					timeRanked);
#else
	// write out the entry information
                sw.WriteLine( "<TR><TH><TD>{0}<TD>{1}<TD>{2}<TD>{3}<TD>{4}<TD>{5}<TD>{6}",
                    r.Quester.Name,
                    guildname,
                    a.Points,
                    quests,
                    a.Rank,
                    a.DeltaRank,
                    timeranked);
#endif
			}
			sw.WriteLine("</TABLE>");
			sw.Close();
		}

		public static void WriteQuestLeaderboard(string filename, int nranks)
		{
			if (_QuestRankList == null)
			{
				return;
			}

			// force an update of the quest leaderboard rankings
			_NeedsUpdate = true;
			RefreshQuestRankList();

			if (!Directory.Exists(_QuestLeaderboardSaveDirectory))
			{
				Directory.CreateDirectory(_QuestLeaderboardSaveDirectory);
			}

			WriteQuestLeaderboardXml(filename + ".xml", nranks);

			WriteQuestLeaderboardHtml(filename + ".html", nranks);
		}

		[Usage("QuestLeaderboardSave [filename [minutes[nentries]]][off]")]
		[Description("Periodically save .xml quest leaderboard information to the specified file")]
		public static void QuestLeaderboardSave_OnCommand(CommandEventArgs e)
		{
			if (e.Arguments.Length > 0)
			{
				if (_QuestLeaderboardTimer != null)
				{
					_QuestLeaderboardTimer.Stop();
				}

				if (e.Arguments[0].ToLower() != "off")
				{
					_QuestLeaderboardFile = e.Arguments[0];

					if (e.Arguments.Length > 1)
					{
						double d;

						if (Double.TryParse(e.Arguments[1], out d))
						{
							_QuestLeaderboardSaveInterval = TimeSpan.FromMinutes(d);
						}
					}

					if (e.Arguments.Length > 2)
					{
						e.Arguments[2].IsDigit(out _QuestLeaderboardSaveRanks);
					}

					_QuestLeaderboardTimer = new QuestLeaderboardTimer(
						_QuestLeaderboardFile, _QuestLeaderboardSaveInterval, _QuestLeaderboardSaveRanks);
					_QuestLeaderboardTimer.Start();
				}
			}

			if (_QuestLeaderboardTimer != null && _QuestLeaderboardTimer.Running)
			{
				e.Mobile.SendMessage(
					"Quest Leaderboard is saving to {0} every {1} minutes. Nranks = {2}",
					_QuestLeaderboardFile,
					_QuestLeaderboardSaveInterval.TotalMinutes,
					_QuestLeaderboardSaveRanks);
			}
			else
			{
				e.Mobile.SendMessage("Quest Leaderboard saving is off.");
			}
		}

		public static void QuestLBSSerialize(GenericWriter writer)
		{
			// version
			writer.Write(0);

			// version 0
			writer.Write(_QuestLeaderboardTimer != null && _QuestLeaderboardTimer.Running);

			writer.Write(_QuestLeaderboardSaveInterval);
			writer.Write(_QuestLeaderboardSaveRanks);
			writer.Write(_QuestLeaderboardFile);
		}

		public static void QuestLBSDeserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						bool running = reader.ReadBool();

						_QuestLeaderboardSaveInterval = reader.ReadTimeSpan();
						_QuestLeaderboardSaveRanks = reader.ReadInt();
						_QuestLeaderboardFile = reader.ReadString();

						if (running)
						{
							if (_QuestLeaderboardTimer != null)
							{
								_QuestLeaderboardTimer.Stop();
							}

							_QuestLeaderboardTimer = new QuestLeaderboardTimer(
								_QuestLeaderboardFile, _QuestLeaderboardSaveInterval, _QuestLeaderboardSaveRanks);
							_QuestLeaderboardTimer.Start();
						}
					}
					break;
			}
		}

		// added the duration timer that begins on spawning
		private class QuestLeaderboardTimer : Timer
		{
			private readonly string _FileName;
			private readonly int _Ranks;

			public QuestLeaderboardTimer(string filename, TimeSpan delay, int nranks)
				: base(delay, delay)
			{
				_FileName = filename;
				_Ranks = nranks;
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				WriteQuestLeaderboard(_FileName, _Ranks);
			}
		}

		//private readonly string _GuildFilter = null;
		//private readonly string _NameFilter = null;

		public class TopQuestPlayersGump : Gump
		{
			private readonly XmlQuestPoints m_attachment;

			public TopQuestPlayersGump(XmlQuestPoints attachment)
				: base(0, 0)
			{
				if (_QuestRankList == null || attachment == null)
				{
					return;
				}

				m_attachment = attachment;

				const int numberToDisplay = 20;
				const int height = 465;

				// prepare the page
				AddPage(0);

#if(FACTIONS)
				const int width = 790;
#else
				const int width = 740;
#endif

				AddBackground(0, 0, width, height, 5054);
				AddAlphaRegion(0, 0, width, height);
				AddImageTiled(20, 20, width - 40, height - 45, 0xBBC);
				AddLabel(20, 2, 55, "Top Quest Player Rankings");

				// guild filter
				AddLabel(40, height - 20, 55, "Filter by Guild");

				string filter = null;

				if (m_attachment != null)
				{
					filter = m_attachment.guildFilter;
				}

				AddImageTiled(140, height - 20, 160, 19, 0xBBC);
				AddTextEntry(140, height - 20, 160, 19, 0, 200, filter);

				AddButton(20, height - 20, 0x15E1, 0x15E5, 200, GumpButtonType.Reply, 0);

				// name filter
				AddLabel(340, height - 20, 55, "Filter by Name"); // 

				filter = null;

				if (m_attachment != null)
				{
					filter = m_attachment.nameFilter;
				}

				AddImageTiled(440, height - 20, 160, 19, 0xBBC);
				AddTextEntry(440, height - 20, 160, 19, 0, 100, filter);

				AddButton(320, height - 20, 0x15E1, 0x15E5, 100, GumpButtonType.Reply, 0);

				RefreshQuestRankList();

				int xloc = 23;
				AddLabel(xloc, 20, 0, "Name");

				xloc += 177;
				AddLabel(xloc, 20, 0, "Guild");

#if(FACTIONS)
				xloc += 35;
				AddLabel(xloc, 20, 0, "Faction");
				xloc += 15;
#endif

				xloc += 50;
				AddLabel(xloc, 20, 0, "Points");

				xloc += 50;
				AddLabel(xloc, 20, 0, "Quests");

				xloc += 50;
				//AddLabel( xloc, 20, 0, "" );

				xloc += 70;
				AddLabel(xloc, 20, 0, "Rank");

				xloc += 45;
				AddLabel(xloc, 20, 0, "Change");

				xloc += 45;
				AddLabel(xloc, 20, 0, "Time at Rank");

				// go through the sorted list and display the top ranked players

				int y = 40;
				int count = 0;

				foreach (QuestRankEntry t in _QuestRankList)
				{
					if (count >= numberToDisplay)
					{
						break;
					}

					QuestRankEntry r = t;

					if (r == null)
					{
						continue;
					}

					XmlQuestPoints a = r.QuestPointsAttachment;

					if (a == null)
					{
						continue;
					}

					if (r.Quester == null || r.Quester.Deleted || r.Rank <= 0 || a == null || a.Deleted)
					{
						continue;
					}

					string guildName = null;

					if (r.Quester.Guild != null)
					{
						guildName = r.Quester.Guild.Abbreviation;
					}

#if(FACTIONS)
					string factionName = null;

					if (r.Quester is PlayerMobile && ((PlayerMobile)r.Quester).FactionPlayerState != null)
					{
						factionName = ((PlayerMobile)r.Quester).FactionPlayerState.Faction.ToString();
					}
#endif

					// check for any ranking change and update rank date
					if (r.Rank != a.Rank)
					{
						a.WhenRanked = DateTime.UtcNow;

						if (a.Rank > 0)
						{
							a.DeltaRank = a.Rank - r.Rank;
						}

						a.Rank = r.Rank;
					}

					// check for guild filter
					if (m_attachment != null && m_attachment.guildFilter != null && m_attachment.guildFilter.Length > 0)
					{
						// parse the comma separated list
						var args = m_attachment.guildFilter.Split(',');

						if (args != null)
						{
							if (!args.Any(arg => arg != null && guildName == arg.Trim()))
							{
								continue;
							}
						}
					}

					// check for name filter
					if (m_attachment != null && m_attachment.nameFilter != null && m_attachment.nameFilter.Length > 0)
					{
						// parse the comma separated list
						var args = m_attachment.nameFilter.Split(',');

						if (args != null)
						{
							if (
								!args.Any(
									arg =>
									arg != null && r.Quester.Name != null &&
									(r.Quester.Name.ToLower().IndexOf(arg.Trim().ToLower(), StringComparison.Ordinal) >= 0)))
							{
								continue;
							}
						}
					}

					count++;

					TimeSpan timeranked = DateTime.UtcNow - a.WhenRanked;

					int days = (int)timeranked.TotalDays;
					int hours = (int)(timeranked.TotalHours - days * 24);
					int mins = (int)(timeranked.TotalMinutes - ((int)timeranked.TotalHours) * 60);

					string quests = "???";

					try
					{
						quests = a.QuestsCompleted.ToString(CultureInfo.InvariantCulture);
					}
					catch
					{ }

					xloc = 23;
					AddLabel(xloc, y, 0, r.Quester.Name);

					xloc += 177;
					AddLabel(xloc, y, 0, guildName);

#if(FACTIONS)
					xloc += 35;
					AddLabelCropped(xloc, y, 60, 21, 0, factionName);
					xloc += 15;
#endif

					xloc += 50;
					AddLabel(xloc, y, 0, a.Points.ToString(CultureInfo.InvariantCulture));

					xloc += 50;
					AddLabel(xloc, y, 0, quests);

					xloc += 50;
					//AddLabel( xloc, y, 0, "" );

					xloc += 70;
					AddLabel(xloc, y, 0, a.Rank.ToString(CultureInfo.InvariantCulture));

					string label = null;

					if (days > 0)
					{
						label += String.Format("{0} days ", days);
					}

					if (hours > 0)
					{
						label += String.Format("{0} hours ", hours);
					}

					if (mins > 0)
					{
						label += String.Format("{0} mins", mins);
					}

					if (label == null)
					{
						label = "just changed";
					}

					string deltalabel = a.DeltaRank.ToString(CultureInfo.InvariantCulture);
					int deltahue = 0;

					if (a.DeltaRank > 0)
					{
						deltalabel = String.Format("+{0}", a.DeltaRank);
						deltahue = 68;
					}
					else if (a.DeltaRank < 0)
					{
						deltahue = 33;
					}

					xloc += 50;
					AddLabel(xloc, y, deltahue, deltalabel);

					xloc += 40;
					AddLabel(xloc, y, 0, label);

					y += 20;
				}
			}

			public override void OnResponse(NetState state, RelayInfo info)
			{
				if (state == null || state.Mobile == null || info == null)
				{
					return;
				}

				// Get the current name
				if (m_attachment != null)
				{
					TextRelay entry = info.GetTextEntry(200);

					if (entry != null)
					{
						m_attachment.guildFilter = entry.Text;
					}

					entry = info.GetTextEntry(100);

					if (entry != null)
					{
						m_attachment.nameFilter = entry.Text;
					}
				}

				switch (info.ButtonID)
				{
					case 100:
					case 200:
						// refresh the gump
						state.Mobile.SendGump(new TopQuestPlayersGump(m_attachment));
						break;
				}
			}
		}

		public class QuestRankEntry : IComparable
		{
			public Mobile Quester { get; set; }
			public XmlQuestPoints QuestPointsAttachment { get; set; }
			public int Rank { get; set; }

			public QuestRankEntry(Mobile m, XmlQuestPoints attachment)
			{
				Quester = m;
				QuestPointsAttachment = attachment;
			}

			public int CompareTo(object obj)
			{
				QuestRankEntry p = (QuestRankEntry)obj;

				if (p.QuestPointsAttachment == null || QuestPointsAttachment == null)
				{
					return 0;
				}

				// break points ties with quests completed (more quests means higher rank)
				if (p.QuestPointsAttachment.Points - QuestPointsAttachment.Points == 0)
				{
					// if kills are the same then compare previous rank 
					if (p.QuestPointsAttachment.QuestsCompleted - QuestPointsAttachment.QuestsCompleted == 0)
					{
						return p.QuestPointsAttachment.Rank - QuestPointsAttachment.Rank;
					}

					return p.QuestPointsAttachment.QuestsCompleted - QuestPointsAttachment.QuestsCompleted;
				}

				return p.QuestPointsAttachment.Points - QuestPointsAttachment.Points;
			}
		}
	}
}