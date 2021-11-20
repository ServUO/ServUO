using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.Gumps;
using Server.Targeting;
using System.Reflection;
using Server.Commands;
using CPA = Server.CommandPropertyAttribute;
using System.Xml;
using Server.Spells;
using System.Text;
using Server.Accounting;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
	public class XmlDialog : XmlAttachment
	{
		// speech entry
		public class SpeechEntry : ISpawnObjectFinder
		{
			public string TypeName
			{
				get
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(this.Action);
					sb.Append('/');
					sb.Append(this.Condition);
					sb.Append('/');
					sb.Append(this.Text);
					sb.Append('/');
					sb.Append(this.Gump);
					return sb.ToString();
				}
			}
			public int EntryNumber;
			private int m_ID;
			public string Text;     // text displayed when the entry is activated
			public string Keywords; // comma separated list of keywords that can be matched to activate the entry.  If no keywords are present then it is automatically activated
			public string Action;   // action string
			public string Condition;    // condition test string
			public string DependsOn;   // the previous entrynumber required to activate this entry
			public int Pause = 1;       // pause in seconds before advancing to the next entry
			public int PrePause = -1;    // pause in seconds before saying the speech for this entry.  -1 indicates the use of auto pause calculation based on triggering speech length.
			public bool LockConversation = true;    // flag to determine if the conversation locks to one player
			public bool AllowNPCTrigger = false; // flag to determine if npc speech can trigger it
			public MessageType SpeechStyle = MessageType.Regular;
			public string Gump;		// GUMP specification string
			public int m_SpeechHue = -1;			// speech hue
			public bool IgnoreCarried = false; // ignore the TriggerOnCarried/NoTriggerOnCarried settings for the dialog when activating this entry

			public int SpeechHue
			{
				get { return m_SpeechHue; }
				set
				{
					// dont allow invalid hues
					m_SpeechHue = value;
					if (m_SpeechHue > 37852) m_SpeechHue = 37852;
				}
			}

			public int ID
			{
				get { return m_ID; }
				set
				{
					// dont allow ID modification of entry 0
					if (EntryNumber == 0) return;
					m_ID = value;
				}
			}
		}

		public static int defProximityRange = 3;
		public static int defResetRange = 12;
		public static TimeSpan defResetTime = TimeSpan.FromSeconds(60);
		public static int defSpeechPace = 10;

		private const string NPCDataSetName = "XmlQuestNPC";
		private const string NPCPointName = "NPC";
		private const string SpeechPointName = "SpeechEntry";
		public static string DefsDir = "XmlQuestNPC";

		private List<SpeechEntry> m_SpeechEntries = new List<SpeechEntry>();  // contains the list of speech entries
		private int m_CurrentEntryNumber = -1;         // used to determine which entry will be subject to modification by various entry editing calls
		private SpeechEntry m_CurrentEntry;
		private bool m_Running = true;
		private int m_ProximityRange = defProximityRange;
		private bool m_AllowGhostTriggering = false;
		private AccessLevel m_TriggerAccessLevel = AccessLevel.Player;
		private DateTime m_LastInteraction;
		private TimeSpan m_ResetTime = defResetTime;
		private int m_ResetRange = defResetRange;
		private bool m_IsActive = false;
		private InternalTimer m_Timer;
		private string m_ConfigFile;
		private Mobile m_ActivePlayer;  // keep track of the player that is currently engaged in conversation so that other players speech can be ignored.
		private int m_SpeechPace = defSpeechPace;   // used for automatic prepause delay calculation.  delayinsecs = speechlength/speechpace + 1
		bool m_HoldProcessing;
		private string m_ItemTriggerName;
		private string m_NoItemTriggerName;
		public List<XmlTextEntryBook> m_TextEntryBook;
		private string m_ResponseString;

		public string ResponseString
		{
			get { return m_ResponseString; }
			set { m_ResponseString = value; }
		}

		public List<SpeechEntry> SpeechEntries
		{
			get
			{
				return m_SpeechEntries;
			}
			set
			{
				m_SpeechEntries = value;
			}
		}

		public Mobile ActivePlayer
		{
			get { return m_ActivePlayer; }
			set { m_ActivePlayer = value; }
		}



		// a serial constructor is REQUIRED
		public XmlDialog(ASerial serial)
			: base(serial)
		{
		}

		[Attachable]
		public XmlDialog(string ConfigFile)
		{
			DoLoadNPC(null, ConfigFile);
		}

		[Attachable]
		public XmlDialog()
		{
			EntryNumber = 0;
		}

		public void DeleteTextEntryBook()
		{
			if (m_TextEntryBook != null)
			{
				foreach (Item s in m_TextEntryBook)
					s.Delete();

				m_TextEntryBook = null;
			}
		}

		private SpeechEntry GetEntry(int entryid)
		{
			if (entryid < 0) return null;
			if (m_SpeechEntries == null)
			{
				m_SpeechEntries = new List<SpeechEntry>();
			}
			// find the speech entry that matches the current entry number
			foreach (SpeechEntry s in m_SpeechEntries)
			{
				if (s.EntryNumber == entryid)
					return s;
			}
			// didnt find it so make a new entry
			SpeechEntry newentry = new SpeechEntry();
			newentry.EntryNumber = entryid;
			newentry.ID = entryid;
			m_SpeechEntries.Add(newentry);
			return newentry;
		}

		private bool ValidMovementTrig(Mobile m)
		{
			if (m == null || m.Deleted) return false;

			return (
				((m is PlayerMobile && (m.AccessLevel <= TriggerAccessLevel))) &&
				((!m.Body.IsGhost && !m_AllowGhostTriggering) || (m.Body.IsGhost && m_AllowGhostTriggering)));
		}

		private bool ValidSpeechTrig(Mobile m)
		{
			if (m == null || m.Deleted) return false;
			bool isplayer=m is PlayerMobile;

			return (
				((isplayer && (m.AccessLevel <= TriggerAccessLevel)) || (AllowNPCTrigger && !isplayer)) &&
				((!m.Body.IsGhost && !m_AllowGhostTriggering) || (m.Body.IsGhost && m_AllowGhostTriggering)));
		}


		[CommandProperty(AccessLevel.GameMaster)]
		public AccessLevel TriggerAccessLevel
		{
			get
			{
				return m_TriggerAccessLevel;
			}
			set
			{
				m_TriggerAccessLevel = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastInteraction
		{
			get
			{
				return m_LastInteraction;
			}
			set
			{
				m_LastInteraction = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool DoReset
		{
			get { return false; }
			set
			{
				if (value) Reset();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsActive
		{
			get
			{
				return m_IsActive;
			}
			set
			{
				m_IsActive = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan GameTOD
		{
			get
			{
				int hours;
				int minutes;
				Map map = null;
				int x = 0;
				int y = 0;
				if (AttachedTo is Item)
				{
					Item it = (Item)AttachedTo;
					map = it.Map;
					x = it.X;
					y = it.Y;

				}
				else if (AttachedTo is Mobile)
				{
					Mobile mob = (Mobile)AttachedTo;
					map = mob.Map;
					x = mob.X;
					y = mob.Y;
				}

				Server.Items.Clock.GetTime(map, x, y, out  hours, out  minutes);
				return (new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hours, minutes, 0).TimeOfDay);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan RealTOD
		{
			get
			{
				return DateTime.UtcNow.TimeOfDay;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int RealDay
		{
			get
			{
				return DateTime.UtcNow.Day;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int RealMonth
		{
			get
			{
				return DateTime.UtcNow.Month;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public DayOfWeek RealDayOfWeek
		{
			get
			{
				return DateTime.UtcNow.DayOfWeek;
			}
		}


		[CommandProperty(AccessLevel.GameMaster)]
		public MoonPhase MoonPhase
		{
			get
			{
				Map map = null;
				int x = 0;
				int y = 0;
				if (AttachedTo is Item)
				{
					Item it = (Item)AttachedTo;
					map = it.Map;
					x = it.X;
					y = it.Y;

				}
				else if (AttachedTo is Mobile)
				{
					Mobile mob = (Mobile)AttachedTo;
					map = mob.Map;
					x = mob.X;
					y = mob.Y;
				}
				return Clock.GetMoonPhase(map, x, y);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowGhostTrig
		{
			get { return m_AllowGhostTriggering; }
			set
			{
				m_AllowGhostTriggering = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Running
		{
			get { return m_Running; }
			set { m_Running = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan ResetTime
		{
			get { return m_ResetTime; }
			set { m_ResetTime = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int SpeechPace
		{
			get { return m_SpeechPace; }
			set { m_SpeechPace = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string Keywords
		{
			get
			{
				// return the keyword string for the current entry
				if (m_CurrentEntry == null)
				{
					return null;
				}
				return m_CurrentEntry.Keywords;
			}
			set
			{
				// set the keyword string for the current entry
				if (m_CurrentEntry == null)
				{
					return;
				}
				m_CurrentEntry.Keywords = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string Action
		{
			get
			{
				if (m_CurrentEntry == null)
				{
					return null;
				}
				return m_CurrentEntry.Action;
			}
			set
			{
				if (m_CurrentEntry == null)
				{
					return;
				}
				m_CurrentEntry.Action = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string Gump
		{
			get
			{
				if (m_CurrentEntry == null)
				{
					return null;
				}
				return m_CurrentEntry.Gump;
			}
			set
			{
				if (m_CurrentEntry == null)
				{
					return;
				}
				m_CurrentEntry.Gump = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int SpeechHue
		{
			get
			{
				if (m_CurrentEntry == null)
				{
					return 0;
				}
				return m_CurrentEntry.SpeechHue;
			}
			set
			{
				if (m_CurrentEntry == null)
				{
					return;
				}
				m_CurrentEntry.SpeechHue = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string Condition
		{
			get
			{
				if (m_CurrentEntry == null)
				{
					return null;
				}
				return m_CurrentEntry.Condition;
			}
			set
			{
				if (m_CurrentEntry == null)
				{
					return;
				}
				m_CurrentEntry.Condition = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string Text
		{
			get
			{
				if (m_CurrentEntry == null)
				{
					return null;
				}
				return m_CurrentEntry.Text;
			}
			set
			{
				if (m_CurrentEntry == null)
				{
					return;
				}
				m_CurrentEntry.Text = value;
			}
		}



		[CommandProperty(AccessLevel.GameMaster)]
		public string DependsOn
		{
			get
			{
				// return the keyword string for the current entry
				if (m_CurrentEntry == null)
				{
					return "-1";
				}
				return m_CurrentEntry.DependsOn;
			}
			set
			{
				// set the keyword string for the current entry
				if (m_CurrentEntry == null)
				{
					return;
				}
				m_CurrentEntry.DependsOn = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool LockConversation
		{
			get
			{
				if (m_CurrentEntry == null)
				{
					return true;
				}
				return m_CurrentEntry.LockConversation;
			}
			set
			{
				if (m_CurrentEntry == null)
				{
					return;
				}
				m_CurrentEntry.LockConversation = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IgnoreCarried
		{
			get
			{
				if (m_CurrentEntry == null)
				{
					return true;
				}
				return m_CurrentEntry.IgnoreCarried;
			}
			set
			{
				if (m_CurrentEntry == null)
				{
					return;
				}
				m_CurrentEntry.IgnoreCarried = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public MessageType SpeechStyle
		{
			get
			{
				if (m_CurrentEntry == null)
				{
					return MessageType.Regular;
				}
				return m_CurrentEntry.SpeechStyle;
			}
			set
			{
				if (m_CurrentEntry == null)
				{
					return;
				}
				m_CurrentEntry.SpeechStyle = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AllowNPCTrigger
		{
			get
			{
				if (m_CurrentEntry == null)
				{
					return false;
				}
				return m_CurrentEntry.AllowNPCTrigger;
			}
			set
			{
				if (m_CurrentEntry == null)
				{
					return;
				}
				m_CurrentEntry.AllowNPCTrigger = value;
			}
		}


		[CommandProperty(AccessLevel.GameMaster)]
		public int Pause
		{
			get
			{
				if (m_CurrentEntry == null)
				{
					return -1;
				}
				return m_CurrentEntry.Pause;
			}
			set
			{
				if (m_CurrentEntry == null)
				{
					return;
				}
				m_CurrentEntry.Pause = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int PrePause
		{
			get
			{
				if (m_CurrentEntry == null)
				{
					return -1;
				}
				return m_CurrentEntry.PrePause;
			}
			set
			{
				if (m_CurrentEntry == null)
				{
					return;
				}
				m_CurrentEntry.PrePause = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int ID
		{
			get
			{
				if (m_CurrentEntry == null)
				{
					return -1;
				}
				return m_CurrentEntry.ID;
			}
			set
			{
				if (m_CurrentEntry == null)
				{
					return;
				}


				m_CurrentEntry.ID = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int EntryNumber
		{
			get { return m_CurrentEntryNumber; }
			set
			{
				m_CurrentEntryNumber = value;
				// get the entry corresponding to the number
				m_CurrentEntry = GetEntry(m_CurrentEntryNumber);
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int ProximityRange
		{
			get { return m_ProximityRange; }
			set
			{
				m_ProximityRange = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int ResetRange
		{
			get { return m_ResetRange; }
			set
			{
				m_ResetRange = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string ConfigFile
		{
			get { return m_ConfigFile; }
			set
			{
				m_ConfigFile = value;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool LoadConfig
		{
			get { return false; }
			set { if (value == true) DoLoadNPC(null, ConfigFile); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public string TriggerOnCarried
		{
			get { return m_ItemTriggerName; }
			set
			{
				m_ItemTriggerName = value;
			}

		}
		[CommandProperty(AccessLevel.GameMaster)]
		public string NoTriggerOnCarried
		{
			get { return m_NoItemTriggerName; }
			set
			{
				m_NoItemTriggerName = value;
			}

		}

		public SpeechEntry CurrentEntry
		{
			get { return m_CurrentEntry; }
			set
			{

				// get the entry corresponding to the number
				m_CurrentEntry = value;
				if (m_CurrentEntry != null)
					m_CurrentEntryNumber = m_CurrentEntry.EntryNumber;
				else
					m_CurrentEntryNumber = -1;
			}
		}

		// see if the DependsOn property contains the specified id
		public bool CheckDependsOn(SpeechEntry s, int id)
		{
			if (s == null || s.DependsOn == null) return false;

			// parse the DependsOn string
			string[] args = s.DependsOn.Split(',');
			int check;
			for (int i = 0; i < args.Length; i++)
			{
				if(int.TryParse(args[i].Trim(), out check) && check == id)
					return true;
			}
			return false;
		}

		private SpeechEntry FindMatchingKeyword(Mobile from, string keyword, int currententryid)
		{
			if (m_SpeechEntries == null) return null;
			List<SpeechEntry> matchlist = new List<SpeechEntry>();

			// go through all of the speech entries and find those that depend on the current entry
			foreach (SpeechEntry s in m_SpeechEntries)
			{
				// ignore self-referencing entries
				if (CheckDependsOn(s, s.ID)) continue;

				// start processing if set for spontaneous activation (banter), already active, or waiting in the default state
				if (((CheckDependsOn(s, -1) || CheckDependsOn(s, -2)) && !IsActive) 
					|| (CheckDependsOn(s, currententryid) && (IsActive || currententryid == 0)))
				{
					// now check for any conditions as well
					// check for any condition that must be met for this entry to be processed
					if (s.Condition != null)
					{
						string error;
						if (!BaseXmlSpawner.CheckPropertyString(null, this, s.Condition , out error))
						{
							status_str = error;
							continue;
						}
						status_str = error;
					}

					// testing for keyword = null will handle calls from the OnTick
					if ((keyword == null && s.Keywords == null))
					{
						// add it to the list of match candidates
						matchlist.Add(s);
					}
					else
						// parse the keyword string
						if (keyword != null && s.Keywords != null)
						{
							string[] arglist = s.Keywords.Split(",".ToCharArray());
							for (int i = 0; i < arglist.Length; i++)
							{
								if(arglist[i]=="$TRIGNAME")
								{
									string[] tocheck = from.Name.ToLower().Split(" ".ToCharArray());
									string result = string.Empty;
									
									for(int x = 0; x<tocheck.Length;x++)
									{
										if(keyword.ToLower().Contains(tocheck[x]))
											result=tocheck[x];
									}
									if(result != string.Empty)
									{
										SpeechEntry entry = CopyEntry(s);
										entry.Keywords = result;
										return entry;
									}
								}
								//USAGE: ON "KEYWORDS" $GOTO 120, will switch to entry 120, so you can jump anywhere if needed
								else if(arglist[i].Contains("$GOTO"))
								{
									string tocheck = arglist[i].Replace("$GOTO","");
									if(tocheck!=string.Empty)
									{
										int togo=0;
										if(int.TryParse(tocheck.Trim(), out togo))
										{
											if(togo>0 && togo!=currententryid && m_SpeechEntries[togo]!=null)
											{
												return m_SpeechEntries[togo];
											}
										}
									}
								}
								else if (arglist[i] == "*")
								{
									// special match anything expression (regex doesnt parse this well)
									matchlist.Add(s);
									break;
								}
								else
								{
									bool error = false;
									Regex r = null;
									try
									{
										r = new Regex(arglist[i], RegexOptions.IgnoreCase);
									}
									catch (Exception e) { error = true; status_str = e.Message; }

									if (!error && r != null)
									{

										Match m = r.Match(keyword);
										if (m.Success)
										{
											// add it to the list of match candidates
											matchlist.Add(s);
											break;
										}
									}
									else
									{
										ReportError(from, String.Format("Bad regular expression: {0} ", status_str));
									}
								}
							}
						}
				}
			}
			if (matchlist.Count > 0)
			{
				// found at least one match
				// if there is more than one, then randomly pick one
				int select = Utility.Random(matchlist.Count);

				return matchlist[select];
			}
			else
			{
				// didnt find a match
				return null;
			}
		}

		public static SpeechEntry CopyEntry(SpeechEntry s)
		{
			SpeechEntry entry = new XmlDialog.SpeechEntry();
			entry.Action = s.Action;
			entry.AllowNPCTrigger = s.AllowNPCTrigger;
			entry.Condition = s.Condition;
			entry.DependsOn = s.DependsOn;
			entry.EntryNumber = s.EntryNumber;
			entry.Gump = s.Gump;
			entry.ID = s.ID;
			entry.IgnoreCarried = s.IgnoreCarried;
			entry.Keywords = s.Keywords;
			entry.LockConversation = s.LockConversation;
			entry.Pause = s.Pause;
			entry.PrePause = s.PrePause;
			entry.SpeechHue = s.SpeechHue;
			entry.SpeechStyle = s.SpeechStyle;
			entry.Text = s.Text;
			return entry;
		}

		public static void DialogGumpCallback(Mobile from, object invoker, string response)
		{
			// insert the response into the triggering speech of the invoking attachment
			if (invoker is XmlDialog)
			{
				XmlDialog xd = (XmlDialog)invoker;
				xd.m_HoldProcessing = false;

				xd.ProcessSpeech(from, response);
			}
		}

		private void ExecuteGump(Mobile mob, string gumpstring)
		{
			if (gumpstring == null || gumpstring.Length <= 0) return;

			Server.Mobiles.XmlSpawner.SpawnObject TheSpawn = new Server.Mobiles.XmlSpawner.SpawnObject(null, 0);

			TheSpawn.TypeName = gumpstring;
			string substitutedtypeName = BaseXmlSpawner.ApplySubstitution(null, this, gumpstring);
			string typeName = BaseXmlSpawner.ParseObjectType(substitutedtypeName);

			Point3D loc = new Point3D(0, 0, 0);
			Map map = null;

			if (AttachedTo is Mobile)
			{
				Mobile m = AttachedTo as Mobile;
				loc = m.Location;
				map = m.Map;
			}
			else if (AttachedTo is Item)
			{
				Item i = (Item)AttachedTo;
				if(i.Parent == null)
				{
					loc = i.Location;
					map = i.Map;
				}
			}

			if (typeName == "GUMP")
			{
				string error;
				BaseXmlSpawner.SpawnTypeKeyword(this, TheSpawn, typeName, substitutedtypeName, mob, loc, map , out error, 0);
				// hold processing until the gump response is completed
				status_str = error;
				m_HoldProcessing = true;
			}
			else
			{
				status_str = "not a GUMP specification";
			}

			ReportError(mob, status_str);
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string status_str{get;private set;}
		private void ExecuteAction(Mobile mob, string action)
		{
			if (string.IsNullOrEmpty(action)) return;

			Server.Mobiles.XmlSpawner.SpawnObject TheSpawn = new Server.Mobiles.XmlSpawner.SpawnObject(null, 0);

			TheSpawn.TypeName = action;
			string substitutedtypeName = BaseXmlSpawner.ApplySubstitution(null, this, action);
			string typeName = BaseXmlSpawner.ParseObjectType(substitutedtypeName);

			Point3D loc = new Point3D(0, 0, 0);
			Map map = null;

			if (AttachedTo is Mobile)
			{
				Mobile m = AttachedTo as Mobile;
				loc = m.Location;
				map = m.Map;
			}
			else if (AttachedTo is Item)
			{
				Item i = (Item)AttachedTo;
				if(i.Parent == null)
				{
					loc = i.Location;
					map = i.Map;
				}
			}

			if (BaseXmlSpawner.IsTypeOrItemKeyword(typeName))
			{
				string error;
				BaseXmlSpawner.SpawnTypeKeyword(AttachedTo, TheSpawn, typeName, substitutedtypeName, mob, map, out error);
				status_str = error;
			}
			else
			{
				// its a regular type descriptor so find out what it is
				Type type = SpawnerType.GetType(typeName);
				try
				{
					string[] arglist = BaseXmlSpawner.ParseString(substitutedtypeName, 3, "/");
					object o = Server.Mobiles.XmlSpawner.CreateObject(type, arglist[0]);
					string error;

					if (o == null)
					{
						status_str = "invalid type specification: " + arglist[0];
					}
					else if (o is Mobile)
					{
						Mobile m = (Mobile)o;
						if (m is BaseCreature)
						{
							BaseCreature c = (BaseCreature)m;
							c.Home = loc; // Spawners location is the home point
						}

						m.Location = loc;
						m.Map = map;
						BaseXmlSpawner.ApplyObjectStringProperties(null, substitutedtypeName, m, mob, AttachedTo, out error);
						status_str = error;
					}
					else if (o is Item)
					{
						Item item = (Item)o;
						BaseXmlSpawner.AddSpawnItem(null, AttachedTo, TheSpawn, item, loc, map, mob, false, substitutedtypeName, out error);
						status_str = error;
					}
				}
				catch { }
			}

			ReportError(mob, status_str);
		}

		private void ReportError(Mobile mob, string status_str)
		{
			if (status_str != null && mob != null && !mob.Deleted && mob is PlayerMobile && mob.AccessLevel > AccessLevel.Player)
			{
				mob.SendMessage(33, String.Format("{0}:{1}", AttachedTo.GetType().Name, status_str));
			}
		}

		public override bool HandlesOnSpeech
		{
			get { return (m_Running); }
		}

		public override void OnSpeech(SpeechEventArgs e)
		{
			if (e.Mobile == null) return;

			// dont handle your own speech
			if (e.Mobile == AttachedTo as Mobile || e.Mobile.AccessLevel > TriggerAccessLevel || ( e.Mobile.Hidden && e.Mobile.Alive ))
			{
				e.Handled = false;
				return;
			}

			if (m_HoldProcessing) return;

			bool ishandled = false;

			Point3D loc = new Point3D(0, 0, 0);
			Map map;

			if (AttachedTo is Mobile)
			{
				Mobile m = AttachedTo as Mobile;
				loc = m.Location;
				map = m.Map;
			}
			else if (AttachedTo is Item)
			{
				Item i = (Item)AttachedTo;
				if(i.Parent == null)
				{
					loc = i.Location;
					map = i.Map;
				}
			}

			if (!e.Handled && m_Running && m_ProximityRange >= 0 && ValidSpeechTrig(e.Mobile) && ((e.Mobile == m_ActivePlayer) || !LockConversation || m_ActivePlayer == null))
			{

				if (!Utility.InRange(e.Mobile.Location, loc, m_ProximityRange))
					return;

				CheckForReset();

				// process the current speech entry
				ishandled = ProcessSpeech(e.Mobile, e.Speech);

				// check to make sure the timer is running
				DoTimer(TimeSpan.FromSeconds(1), m_ActivePlayer);
			}

			if (!ishandled)
			{
				base.OnSpeech(e);
			}
		}

		public override bool HandlesOnMovement { get { return (m_Running); } }

		public override void OnMovement(MovementEventArgs e)
		{
			Mobile m = e.Mobile;

			if (m == null || m.AccessLevel > TriggerAccessLevel) return;

			Point3D loc = new Point3D(0, 0, 0);
			Map map;

			if (AttachedTo is Mobile)
			{
				Mobile mob = AttachedTo as Mobile;
				loc = mob.Location;
				map = mob.Map;
			}
			else if (AttachedTo is Item)
			{
				Item i = (Item)AttachedTo;
				if(i.Parent == null)
				{
					loc = i.Location;
					map = i.Map;
				}
			}

			// if proximity sensing is off, a speech entry has been activated, or player is an admin then ignore
			if (m_Running && m_ProximityRange >= 0 && ValidMovementTrig(m) && !IsActive && !m_HoldProcessing)
			{
				// check to see if player is within range of the npc
				if (Utility.InRange(m.Location, loc, m_ProximityRange))
				{

					TimeSpan pause = TimeSpan.FromSeconds(0);
					if (CurrentEntry != null && CurrentEntry.Pause > 0)
					{
						pause = TimeSpan.FromSeconds(CurrentEntry.Pause);
					}
					// check to see if the current pause interval has elapsed
					if (DateTime.UtcNow - pause > m_LastInteraction)
					{
						// process speech that is not keyword dependent
						CheckForReset();

						ProcessSpeech(m, null);
					}
					// turn on the timer that will run until the speech list is reset
					// it will control paused speech and will allow the speech entry to be reset after ResetTime timeout
					DoTimer(TimeSpan.FromSeconds(1), m);
				}
			}
			else
			{
				CheckForReset();
			}
			base.OnMovement(e);
		}

		private bool IsInRange(IEntity e1, IEntity e2, int range)
		{
			if (e1 == null || e2 == null) return false;

			if (e1.Map != e2.Map) return false;

			return Utility.InRange(e1.Location, e2.Location, range);
		}


		private void CheckForReset()
		{
			// check to see if the interaction time has elapsed or player has gone out of range.  If so then reset to entry zero
			if (!m_HoldProcessing && 
				((DateTime.UtcNow - ResetTime > m_LastInteraction) ||
				(AttachedTo is IEntity && m_ActivePlayer != null && !IsInRange(m_ActivePlayer, (IEntity)AttachedTo, ResetRange))))
			{
				Reset();
			}

		}

		private void Reset()
		{
			EntryNumber = 0;
			IsActive = false;
			m_ActivePlayer = null;
			// turn off the timer
			if (m_Timer != null)
				m_Timer.Stop();
		}

		private void DelayedSpeech(object state)
		{
			object[] states = (object[])state;

			SpeechEntry matchentry = (SpeechEntry)states[0];
			Mobile m = (Mobile)states[1];

			if (matchentry != null && m!=null)
			{
				CurrentEntry = matchentry;

				string text = BaseXmlSpawner.ApplySubstitution(null, this, CurrentEntry.Text);

				if (text != null)
				{
					// dont know why emote doesnt work, but we'll just do it manually
					if (CurrentEntry.SpeechStyle == MessageType.Emote)
					{
						text = String.Format("*{0}*", text);
					}

					// items cannot produce actual speech
					// display a message over the item it was attached to
					if (AttachedTo is Item)
					{
						int speechhue = 0x3B2;
						if (CurrentEntry.SpeechHue >= 0)
						{
							speechhue = CurrentEntry.SpeechHue;
						}
						((Item)AttachedTo).PublicOverheadMessage(MessageType.Regular, speechhue, false, text);
					}
					else if (AttachedTo is Mobile)
					{
						Mobile mob = (Mobile)AttachedTo;
						// mobiles can produce actual speech
						// so let them.  This allows mobiles to talk with one another
						int speechhue = mob.SpeechHue;
						if (CurrentEntry.SpeechHue >= 0)
						{
							speechhue = CurrentEntry.SpeechHue;
						}

						mob.DoSpeech(text, new int[] { }, CurrentEntry.SpeechStyle, speechhue);
						//((Mobile)AttachedTo).PublicOverheadMessage( MessageType.Regular, 0x3B2, true, text );
					}
				}

				IsActive = true;
				m_LastInteraction = DateTime.UtcNow;

				// execute any action associated with it
				// allow for multiple action strings on a single line separated by a semicolon
				if (CurrentEntry.Action != null && CurrentEntry.Action.Length > 0)
				{
					string[] args = CurrentEntry.Action.Split(';');

					for (int j = 0; j < args.Length; j++)
					{
						ExecuteAction(m, args[j]);
					}
				}

				// execute any GUMP associated with it
				ExecuteGump(m, CurrentEntry.Gump);
			}

			m_HoldProcessing = false;
		}

		public bool ProcessSpeech(Mobile m, string speech)
		{
			if (m_HoldProcessing) return true;

			// check the speech against the entries that depend on the present entry
			SpeechEntry matchentry = FindMatchingKeyword(m, speech, ID);

			if (matchentry == null) return false;

			// when attempting to process speech-triggered speech, check for oncarried dependencies
			// This will not apply to movement-triggered speech (banter with -1 dependson) which will continue to be activated
			// regardless of oncarried status
			// dependson of -2 will allow non-speech triggering but will still apply oncarried dependencies

			// if player-carried item triggering is set then test for the presence of an item on the player an in their pack
			if ((speech != null || CheckDependsOn(matchentry, -2)) && TriggerOnCarried != null && TriggerOnCarried.Length > 0)
			{
				bool found = BaseXmlSpawner.CheckForCarried(m, TriggerOnCarried) || matchentry.IgnoreCarried;

				// is the player carrying the right item, if not then dont process
				if (!found) return false;
			}

			// if player-carried noitem triggering is set then test for the presence of an item in the players pack that should block triggering
			if ((speech != null || CheckDependsOn(matchentry, -2)) && NoTriggerOnCarried != null && NoTriggerOnCarried.Length > 0)
			{
				bool notfound = BaseXmlSpawner.CheckForNotCarried(m, NoTriggerOnCarried) || matchentry.IgnoreCarried;

				// is the player carrying the right item, if so then dont process
				if (!notfound) return false;
			}

			// the player that successfully activates a conversation by speech becomes the exclusive conversationalist until the npc resets
			if ((speech != null || LockConversation) && m != null)
				m_ActivePlayer = m;

			ResponseString = speech;

			// calculate the delay before activating the entry
			int prepause = 1;    // 1 sec by default
			if (matchentry.PrePause < 0)
			{
				if (SpeechPace > 0 && speech != null)
				{
					// do the auto delay calculation based on the length of the triggering speech
					prepause = (speech.Length / SpeechPace) + 1;    // make 1 sec the min pause

				}
			}
			else
			{
				prepause = matchentry.PrePause;
			}

			// and switch to the one that matches

			m_HoldProcessing = true;
			Timer.DelayCall(TimeSpan.FromSeconds(prepause), new TimerStateCallback(DelayedSpeech), new object[] { matchentry, m });

			return true;
		}

		public void DoTimer(TimeSpan delay, Mobile trigmob)
		{
			if (!m_Running)
				return;

			if (m_Timer != null)
				m_Timer.Stop();

			m_Timer = new InternalTimer(this, delay, trigmob);
			m_Timer.Start();
		}

		private class InternalTimer : Timer
		{
			private XmlDialog m_npc;
			public Mobile m_trigmob;
			public TimeSpan m_delay;

			public InternalTimer(XmlDialog npc, TimeSpan delay, Mobile trigmob)
				: base(delay, delay)
			{

				Priority = TimerPriority.OneSecond;

				m_npc = npc;
				m_trigmob = trigmob;
				m_delay = delay;
			}

			protected override void OnTick()
			{
				if(m_npc != null && !m_npc.Deleted && m_trigmob!=null)
				{
					// check to see if any speech needs to be processed
					TimeSpan pause = TimeSpan.Zero;
					if (m_npc.CurrentEntry != null)
					{
						if(m_npc.CurrentEntry.Pause > 0)
							pause = TimeSpan.FromSeconds(m_npc.CurrentEntry.Pause);
						else if(m_npc.CurrentEntry.Pause < 0 && m_npc.CurrentEntry.Pause > -11 && m_npc.CurrentEntry.Text!=null)
						{
							double d = Math.Abs(m_npc.CurrentEntry.Pause * 0.05)*m_npc.CurrentEntry.Text.Length;
				   			pause = TimeSpan.FromSeconds(d);
						}
					}
					// check to see if the current pause interval has elapsed
					if (DateTime.UtcNow - pause > m_npc.LastInteraction)
					{
						// process speech that is not keyword dependent

						m_npc.ProcessSpeech(m_trigmob, null);
						m_npc.CheckForReset();
					}
				}
				else
				{
					Stop();
				}
			}
		}

		public void DoLoadNPC(Mobile from, string filename)
		{
			if (filename == null || filename.Length <= 0) return;

			string dirname;
			if (System.IO.Directory.Exists(DefsDir))
			{
				// look for it in the defaults directory
				dirname = String.Format("{0}/{1}.npc", DefsDir, filename);

				// Check if the file exists
				if (!System.IO.File.Exists(dirname))
				{
					// didnt find it so just look in the main install dir
					dirname = String.Format("{0}.npc", filename);
				}
			}
			else
			{
				// look in the main installation dir
				dirname = String.Format("{0}.npc", filename);
			}

			// Check if the file exists
			if (System.IO.File.Exists(dirname))
			{
				FileStream fs = null;
				try
				{
					fs = File.Open(dirname, FileMode.Open, FileAccess.Read);
				}
				catch { }

				if (fs == null)
				{
					from.SendMessage("Unable to open {0} for loading", dirname);
					return;
				}

				// Create the data set
				DataSet ds = new DataSet(NPCDataSetName);

				// Read in the file
				bool fileerror = false;

				try
				{
					ds.ReadXml(fs);
				}
				catch { fileerror = true; }

				// close the file
				fs.Close();

				if (fileerror)
				{
					if (from != null && !from.Deleted)
						from.SendMessage(33, "Error reading npc file {0}", dirname);
					return;
				}

				// Check that at least a single table was loaded
				if (ds.Tables != null && ds.Tables.Count > 0)
				{
					// get the npc info
					if (ds.Tables[NPCPointName] != null && ds.Tables[NPCPointName].Rows.Count > 0)
					{
						DataRow dr = ds.Tables[NPCPointName].Rows[0];

						try
						{
							if (AttachedTo is Item)
							{
								((Item)AttachedTo).Name = (string)dr["Name"];
							}
							else if (AttachedTo is Mobile)
							{
								((Mobile)AttachedTo).Name = (string)dr["Name"];
							}
						}
						catch { }
						try { this.ProximityRange = int.Parse((string)dr["ProximityRange"]); }
						catch { }
						try { this.ResetRange = int.Parse((string)dr["ResetRange"]); }
						catch { }
						try { this.TriggerOnCarried = (string)dr["TriggerOnCarried"]; }
						catch { }
						try { this.NoTriggerOnCarried = (string)dr["NoTriggerOnCarried"]; }
						catch { }
						try { this.m_AllowGhostTriggering = bool.Parse((string)dr["AllowGhost"]); }
						catch { }
						try { this.m_SpeechPace = int.Parse((string)dr["SpeechPace"]); }
						catch { }
						try { this.Running = bool.Parse((string)dr["Running"]); }
						catch { }
						try { this.ResetTime = TimeSpan.FromMinutes(double.Parse((string)dr["ResetTime"])); }
						catch { }
						try { this.ConfigFile = (string)dr["ConfigFile"]; }
						catch { }

						int entrycount=0;
						try { entrycount = int.Parse((string)dr["SpeechEntries"]); }
						catch { }
					}

					// get the speech entry info
					if (ds.Tables[NPCPointName] != null && ds.Tables[NPCPointName].Rows.Count > 0)
					{
						m_SpeechEntries = new List<SpeechEntry>();
						foreach (DataRow dr in ds.Tables[SpeechPointName].Rows)
						{
							SpeechEntry s = new SpeechEntry();
							// Populate the speech entry data
							try { s.EntryNumber = int.Parse((string)dr["EntryNumber"]); }
							catch { }
							try { s.ID = int.Parse((string)dr["ID"]); }
							catch { }
							try { s.Text = (string)dr["Text"]; }
							catch { }
							try { s.Keywords = (string)dr["Keywords"]; }
							catch { }
							try { s.Action = (string)dr["Action"]; }
							catch { }
							try { s.Condition = (string)dr["Condition"]; }
							catch { }
							try { s.DependsOn = (string)dr["DependsOn"]; }
							catch { }
							try { s.Pause = int.Parse((string)dr["Pause"]); }
							catch { }
							try { s.PrePause = int.Parse((string)dr["PrePause"]); }
							catch { }
							try { s.LockConversation = bool.Parse((string)dr["LockConversation"]); }
							catch { }
							try { s.IgnoreCarried = bool.Parse((string)dr["IgnoreCarried"]); }
							catch { }
							try { s.AllowNPCTrigger = bool.Parse((string)dr["AllowNPCTrigger"]); }
							catch { }
							try { s.SpeechStyle = (MessageType)Enum.Parse(typeof(MessageType), (string)dr["SpeechStyle"]); }
							catch { }
							try { s.SpeechHue = int.Parse((string)dr["SpeechHue"]); }
							catch { }
							try { s.Gump = (string)dr["Gump"]; }
							catch { }

							m_SpeechEntries.Add(s);
						}
					}

					Reset();

					if (from != null && !from.Deleted)
						from.SendMessage("Loaded npc from file {0}", dirname);
				}
				else
				{
					if (from != null && !from.Deleted)
						from.SendMessage(33, "No npc data found in: {0}", dirname);
				}
			}
			else
			{
				if (from != null && !from.Deleted)
					from.SendMessage(33, "File not found: {0}", dirname);
			}
		}

		public void DoSaveNPC(Mobile from, string filename, bool updateconfig)
		{
			if (filename == null || filename.Length <= 0) return;

			// Create the data set
			DataSet ds = new DataSet(NPCDataSetName);

			// Load the data set up
			ds.Tables.Add(NPCPointName);
			ds.Tables.Add(SpeechPointName);

			// Create a schema for the npc
			ds.Tables[NPCPointName].Columns.Add("AccountOwner");
			ds.Tables[NPCPointName].Columns.Add("Name");
			ds.Tables[NPCPointName].Columns.Add("Running");
			ds.Tables[NPCPointName].Columns.Add("ProximityRange");
			ds.Tables[NPCPointName].Columns.Add("ResetRange");
			ds.Tables[NPCPointName].Columns.Add("TriggerOnCarried");
			ds.Tables[NPCPointName].Columns.Add("NoTriggerOnCarried");
			ds.Tables[NPCPointName].Columns.Add("AllowGhost");
			ds.Tables[NPCPointName].Columns.Add("SpeechPace");
			ds.Tables[NPCPointName].Columns.Add("ResetTime");
			ds.Tables[NPCPointName].Columns.Add("ConfigFile");
			ds.Tables[NPCPointName].Columns.Add("SpeechEntries");

			// Create a schema for the speech entries
			ds.Tables[SpeechPointName].Columns.Add("EntryNumber");
			ds.Tables[SpeechPointName].Columns.Add("ID");
			ds.Tables[SpeechPointName].Columns.Add("Text");
			ds.Tables[SpeechPointName].Columns.Add("Keywords");
			ds.Tables[SpeechPointName].Columns.Add("Action");
			ds.Tables[SpeechPointName].Columns.Add("Condition");
			ds.Tables[SpeechPointName].Columns.Add("DependsOn");
			ds.Tables[SpeechPointName].Columns.Add("Pause");
			ds.Tables[SpeechPointName].Columns.Add("PrePause");
			ds.Tables[SpeechPointName].Columns.Add("LockConversation");
			ds.Tables[SpeechPointName].Columns.Add("IgnoreCarried");
			ds.Tables[SpeechPointName].Columns.Add("AllowNPCTrigger");
			ds.Tables[SpeechPointName].Columns.Add("SpeechStyle");
			ds.Tables[SpeechPointName].Columns.Add("SpeechHue");
			ds.Tables[SpeechPointName].Columns.Add("Gump");

			// Create a new data row
			DataRow dr = ds.Tables[NPCPointName].NewRow();

			// the account owner
			string owner=string.Empty;
			if(from!=null && !from.Deleted && from.Account!=null)
				owner=from.Account.Username;
			dr["AccountOwner"]=owner;

			// Populate the npc data
			if (AttachedTo is Item)
			{
				dr["Name"] = (string)((Item)AttachedTo).Name;
			}
			else
				if (AttachedTo is Mobile)
				{
					dr["Name"] = (string)((Mobile)AttachedTo).Name;
				}

			dr["Running"] = (bool)this.Running;
			dr["ProximityRange"] = (int)this.m_ProximityRange;
			dr["ResetRange"] = (int)this.m_ResetRange;
			dr["TriggerOnCarried"] = (string)this.TriggerOnCarried;
			dr["NoTriggerOnCarried"] = (string)this.NoTriggerOnCarried;
			dr["AllowGhost"] = (bool)this.m_AllowGhostTriggering;
			dr["SpeechPace"] = (int)this.SpeechPace;
			dr["ResetTime"] = (double)this.ResetTime.TotalMinutes;
			dr["ConfigFile"] = (string)filename;
			int entrycount = 0;
			if (SpeechEntries != null)
			{
				entrycount = SpeechEntries.Count;
			}
			dr["SpeechEntries"] = (int)entrycount;

			// Add the row the the table
			ds.Tables[NPCPointName].Rows.Add(dr);

			for (int i = 0; i < entrycount; i++)
			{
				SpeechEntry s = SpeechEntries[i];

				// Create a new data row
				dr = ds.Tables[SpeechPointName].NewRow();

				// Populate the speech entry data
				dr["EntryNumber"] = (int)s.EntryNumber;
				dr["ID"] = (int)s.ID;
				dr["Text"] = (string)s.Text;
				dr["Keywords"] = (string)s.Keywords;
				dr["Action"] = (string)s.Action;
				dr["Condition"] = (string)s.Condition;
				dr["DependsOn"] = (string)s.DependsOn;
				dr["Pause"] = (int)s.Pause;
				dr["PrePause"] = (int)s.PrePause;
				dr["LockConversation"] = (bool)s.LockConversation;
				dr["IgnoreCarried"] = (bool)s.IgnoreCarried;
				dr["AllowNPCTrigger"] = (bool)s.AllowNPCTrigger;
				dr["SpeechStyle"] = (MessageType)s.SpeechStyle;
				dr["SpeechHue"] = (int)s.SpeechHue;
				dr["Gump"] = (string)s.Gump;

				// Add the row the the table
				ds.Tables[SpeechPointName].Rows.Add(dr);
			}

			// Write out the file

			string dirname;

			if (System.IO.Directory.Exists(DefsDir))
			{
				// put it in the defaults directory if it exists
				dirname = String.Format("{0}/{1}.npc", DefsDir, filename);
			}
			else
			{
				// otherwise just put it in the main installation dir
				dirname = String.Format("{0}.npc", filename);
			}

			// check to see if the file already exists

			if (System.IO.File.Exists(dirname))
			{
				// prompt the user to save over it
				if (from != null)
				{
					if(from.AccessLevel < XmlSpawner.DiskAccessLevel)
					{
						try
						{
							StreamReader op = new StreamReader(dirname, false);
							if (op == null)
							{
								from.SendMessage("Cannot access file {0}", dirname);
								return;
							}
							op.ReadLine();//discard first three lines
							op.ReadLine();
							op.ReadLine();
							string line = op.ReadLine().Trim();
							owner=string.Format("<AccountOwner>{0}</AccountOwner>", owner);
							if(line!=owner || line=="<AccountOwner />")//allow overwrite of unowned files
							{
								from.SendMessage("Cannot overwrite file {0} : not owner", dirname);
								return;
							}
						}
						catch
						{
							from.SendMessage("Error in writing file {0} : catched exception", dirname);
							return;
						}
					}
					from.SendGump(new ConfirmSaveGump(this, ds, dirname, filename, updateconfig));
				}
			}
			else
			{
				SaveFile(from, ds, dirname, filename, updateconfig);
			}
		}

		public bool SaveFile(Mobile from, DataSet ds, string dirname, string configname, bool updateconfig)
		{
			if (ds == null)
			{
				if (from != null && !from.Deleted)
					from.SendMessage("Empty dataset. File {0} not saved.", dirname);
				return false;
			}

			bool file_error = false;

			try
			{
				ds.WriteXml(dirname);
			}
			catch { file_error = true; }

			if (file_error)
			{
				if (from != null && !from.Deleted)
					from.SendMessage("Error trying to save to file {0}", dirname);
				return false;
			}
			else
			{
				if (from != null && !from.Deleted)
					from.SendMessage("Saved npc to file {0}", dirname);
				if (updateconfig)
					ConfigFile = configname;
			}

			return true;
		}

		public class ConfirmSaveGump : Gump
		{
			private XmlDialog m_dialog;
			private DataSet m_ds;
			private string m_filename;
			private string m_configname;
			private bool m_updateconfig;


			public ConfirmSaveGump(XmlDialog dialog, DataSet ds, string filename, string configname, bool updateconfig)
				: base(0, 0)
			{
				m_dialog = dialog;
				m_ds = ds;
				m_filename = filename;
				m_configname = configname;
				m_updateconfig = updateconfig;

				Closable = false;
				Dragable = true;
				AddPage(0);
				AddBackground(10, 200, 200, 130, 5054);

				//AddLabelCropped(15, 195, 185, 55, 33, String.Format("{0} esiste.", filename));
				
				//AddHtml(15, 205, 186, 45, String.Format("{0} esiste.", filename), false, false);
				AddLabel(15, 210, 33, "Il file: ./"+DefsDir+"/ ...");
				AddLabel(15, 230, 33, filename.Replace(DefsDir+"/", ""));
				AddLabel(20, 250, 33, "Esiste, sovrascrivi?");
				AddRadio(35, 275, 9721, 9724, false, 1); // accept/yes radio
				AddRadio(135, 275, 9721, 9724, true, 2); // decline/no radio
				AddHtmlLocalized(72, 275, 200, 30, 1049016, 0x7fff, false, false); // Yes
				AddHtmlLocalized(172, 275, 200, 30, 1049017, 0x7fff, false, false); // No
				AddButton(80, 309, 2130, 2129, 3, GumpButtonType.Reply, 0); // Okay button

			}
			public override void OnResponse(NetState state, RelayInfo info)
			{
				if (info == null || state == null || state.Mobile == null) return;

				int radiostate = -1;
				if (info.Switches.Length > 0)
				{
					radiostate = info.Switches[0];
				}
				switch (info.ButtonID)
				{
					default:
						{
							if (radiostate == 1)
							{    // accept
								if (m_dialog != null)
									m_dialog.SaveFile(state.Mobile, m_ds, m_filename, m_configname, m_updateconfig);
							}
							else
							{
								state.Mobile.SendMessage("File {0} not saved.", m_filename);
							}
							break;
						}
				}
			}
		}

		[Usage("XmlSaveNPC filename")]
		[Description("Saves the targeted Talking NPC to an xml file.")]
		public static void SaveNPC_OnCommand(CommandEventArgs e)
		{
			e.Mobile.Target = new SaveNPCTarget(e);
		}

		private class SaveNPCTarget : Target
		{
			private CommandEventArgs m_e;
			public SaveNPCTarget(CommandEventArgs e)
				: base(30, false, TargetFlags.None)
			{
				m_e = e;
			}
			protected override void OnTarget(Mobile from, object targeted)
			{
				string filename = m_e.GetString(0);

				if (filename == null || filename.Length <= 0)
				{
					from.SendMessage("must specify a save filename");
					return;
				}

				// save the XmlDialog attachment info to xml
				XmlDialog xa = XmlAttach.FindAttachment(targeted, typeof(XmlDialog)) as XmlDialog;

				if (xa != null)
				{
					xa.DoSaveNPC(from, filename, false);

				}
				else
				{
					from.SendMessage("Must target a Talking NPC");
				}
			}
		}

		[Usage("XmlLoadNPC filename")]
		[Description("Loads the targeted Talking NPC to an xml file.")]
		public static void LoadNPC_OnCommand(CommandEventArgs e)
		{
			e.Mobile.Target = new LoadNPCTarget(e);
		}

		private class LoadNPCTarget : Target
		{
			private CommandEventArgs m_e;
			public LoadNPCTarget(CommandEventArgs e)
				: base(30, false, TargetFlags.None)
			{
				m_e = e;
			}
			protected override void OnTarget(Mobile from, object targeted)
			{
				string filename = m_e.GetString(0);

				if (filename == null || filename.Length <= 0)
				{
					from.SendMessage("must specify a load filename");
					return;
				}

				// load the XmlDialog attachment
				XmlDialog xa = XmlAttach.FindAttachment(targeted, typeof(XmlDialog)) as XmlDialog;

				if (xa != null)
				{
					xa.DoLoadNPC(from, filename);

				}
				else
				{
					// doesnt have a dialog attachment, so add and load
					xa = new XmlDialog(filename);
					XmlAttach.AttachTo(targeted, xa);
				}
			}
		}

		public static bool AssignSettings(string argname, string value)
		{
			switch (argname)
			{
				case "XmlQuestNPCDir":
					DefsDir = value;
					break;
				case "defResetTime":
					defResetTime = TimeSpan.FromSeconds(XmlSpawner.ConvertToInt(value));
					break;
				case "defProximityRange":
					defProximityRange = XmlSpawner.ConvertToInt(value);
					break;
				case "defResetRange":
					defResetRange = XmlSpawner.ConvertToInt(value);
					break;
				case "defSpeechPace":
					defSpeechPace = XmlSpawner.ConvertToInt(value);
					break;
				default:
					return false;
			}

			return true;
		}

		public new static void Initialize()
		{
			XmlSpawner.LoadSettings(new XmlSpawner.AssignSettingsHandler(AssignSettings), "XmlDialog");

			CommandSystem.Register("SaveNPC", AccessLevel.Administrator, new CommandEventHandler(SaveNPC_OnCommand));
			CommandSystem.Register("LoadNPC", AccessLevel.Administrator, new CommandEventHandler(LoadNPC_OnCommand));
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)9); // version
			// Version 9 added the ResetRange property
			writer.Write(m_ResetRange);
			// Version 8 added the IgnoreCarried property
			if (m_SpeechEntries != null)
			{
				writer.Write((int)m_SpeechEntries.Count);
				foreach (SpeechEntry s in m_SpeechEntries)
				{
					writer.Write(s.IgnoreCarried);
				}
			}
			else
			{
				writer.Write((int)0);
			}

			// Version 7
			// changed DependsOn to a string
			// Version 6
			// write out the additional speech entry fields
			if (m_SpeechEntries != null)
			{
				writer.Write((int)m_SpeechEntries.Count);
				foreach (SpeechEntry s in m_SpeechEntries)
				{
					writer.Write(s.SpeechHue);
				}
			}
			else
			{
				writer.Write((int)0);
			}
			// Version 5
			// write out the additional speech entry fields
			if (m_SpeechEntries != null)
			{
				writer.Write((int)m_SpeechEntries.Count);
				foreach (SpeechEntry s in m_SpeechEntries)
				{
					writer.Write(s.Gump);
				}
			}
			else
			{
				writer.Write((int)0);
			}
			// Version 4
			// write out the additional speech entry fields
			if (m_SpeechEntries != null)
			{
				writer.Write((int)m_SpeechEntries.Count);
				foreach (SpeechEntry s in m_SpeechEntries)
				{
					writer.Write(s.Condition);
				}
			}
			else
			{
				writer.Write((int)0);
			}
			// Version 3
			writer.Write(TriggerOnCarried);
			writer.Write(NoTriggerOnCarried);
			// Version 2
			writer.Write(m_SpeechPace);
			// write out the additional speech entry fields
			if (m_SpeechEntries != null)
			{
				writer.Write((int)m_SpeechEntries.Count);
				foreach (SpeechEntry s in m_SpeechEntries)
				{
					writer.Write(s.PrePause);
					writer.Write(s.LockConversation);
					writer.Write(s.AllowNPCTrigger);
					writer.Write((int)s.SpeechStyle);
				}
			}
			else
			{
				writer.Write((int)0);
			}

			// Version 1
			writer.Write(m_ActivePlayer);

			// Version 0
			writer.Write(m_IsActive);
			writer.Write(m_ResetTime);
			writer.Write(m_LastInteraction);
			writer.Write(m_AllowGhostTriggering);
			writer.Write(m_ProximityRange);
			writer.Write(m_Running);
			writer.Write(m_ConfigFile);
			// write out the speech entries
			if (m_SpeechEntries != null)
			{
				writer.Write((int)m_SpeechEntries.Count);
				foreach (SpeechEntry s in m_SpeechEntries)
				{
					writer.Write(s.EntryNumber);
					writer.Write(s.ID);
					writer.Write(s.Text);
					writer.Write(s.Keywords);
					writer.Write(s.Action);
					writer.Write(s.DependsOn);
					writer.Write(s.Pause);
				}
			}
			else
			{
				writer.Write((int)0);
			}
			writer.Write(m_CurrentEntryNumber);
			// check to see if the timer is running
			if (m_Timer != null && m_Timer.Running)
			{
				writer.Write(true);
				writer.Write(m_Timer.m_trigmob);
				writer.Write(m_Timer.m_delay);
			}
			else
			{
				writer.Write(false);
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 9:
					{
						m_ResetRange = reader.ReadInt();
						goto case 8;
					}
				case 8:
					{
						int count = reader.ReadInt();
						m_SpeechEntries = new List<SpeechEntry>();
						for (int i = 0; i < count; i++)
						{
							SpeechEntry newentry = new SpeechEntry();

							newentry.IgnoreCarried = reader.ReadBool();

							m_SpeechEntries.Add(newentry);
						}

						goto case 7;
					}
				case 7:
					{
						goto case 6;
					}
				case 6:
					{
						int count = reader.ReadInt();
						if (version < 8)
						{
							m_SpeechEntries = new List<SpeechEntry>();
						}
						for (int i = 0; i < count; i++)
						{
							if (version < 8)
							{
								SpeechEntry newentry = new SpeechEntry();

								newentry.SpeechHue = reader.ReadInt();

								m_SpeechEntries.Add(newentry);
							}
							else
							{
								SpeechEntry newentry = m_SpeechEntries[i];

								newentry.SpeechHue = reader.ReadInt();
							}
						}

						goto case 5;
					}
				case 5:
					{
						int count = reader.ReadInt();
						if (version < 6)
						{
							m_SpeechEntries = new List<SpeechEntry>();
						}
						for (int i = 0; i < count; i++)
						{
							if (version < 6)
							{
								SpeechEntry newentry = new SpeechEntry();

								newentry.Gump = reader.ReadString();

								m_SpeechEntries.Add(newentry);
							}
							else
							{
								SpeechEntry newentry = m_SpeechEntries[i];

								newentry.Gump = reader.ReadString();
							}
						}

						goto case 4;
					}
				case 4:
					{
						int count = reader.ReadInt();
						if (version < 5)
						{
							m_SpeechEntries = new List<SpeechEntry>();
						}
						for (int i = 0; i < count; i++)
						{
							if (version < 5)
							{
								SpeechEntry newentry = new SpeechEntry();

								newentry.Condition = reader.ReadString();

								m_SpeechEntries.Add(newentry);
							}
							else
							{
								SpeechEntry newentry = m_SpeechEntries[i];

								newentry.Condition = reader.ReadString();
							}
						}

						goto case 3;
					}
				case 3:
					{
						TriggerOnCarried = reader.ReadString();
						NoTriggerOnCarried = reader.ReadString();
						goto case 2;
					}
				case 2:
					{
						m_SpeechPace = reader.ReadInt();

						int count = reader.ReadInt();
						if (version < 4)
						{
							m_SpeechEntries = new List<SpeechEntry>();
						}
						for (int i = 0; i < count; i++)
						{
							if (version < 4)
							{
								SpeechEntry newentry = new SpeechEntry();

								newentry.PrePause = reader.ReadInt();
								newentry.LockConversation = reader.ReadBool();
								newentry.AllowNPCTrigger = reader.ReadBool();
								newentry.SpeechStyle = (MessageType)reader.ReadInt();

								m_SpeechEntries.Add(newentry);
							}
							else
							{
								SpeechEntry newentry = m_SpeechEntries[i];

								newentry.PrePause = reader.ReadInt();
								newentry.LockConversation = reader.ReadBool();
								newentry.AllowNPCTrigger = reader.ReadBool();
								newentry.SpeechStyle = (MessageType)reader.ReadInt();
							}
						}
						goto case 1;
					}
				case 1:
					{
						m_ActivePlayer = reader.ReadMobile();
						goto case 0;
					}
				case 0:
					{
						m_IsActive = reader.ReadBool();
						m_ResetTime = reader.ReadTimeSpan();
						m_LastInteraction = reader.ReadDateTime();
						m_AllowGhostTriggering = reader.ReadBool();
						m_ProximityRange = reader.ReadInt();
						m_Running = reader.ReadBool();
						m_ConfigFile = reader.ReadString();
						int count = reader.ReadInt();
						if (version < 2)
						{
							m_SpeechEntries = new List<SpeechEntry>();
						}
						for (int i = 0; i < count; i++)
						{

							if (version < 2)
							{
								SpeechEntry newentry = new SpeechEntry();

								newentry.EntryNumber = reader.ReadInt();
								newentry.ID = reader.ReadInt();
								newentry.Text = reader.ReadString();
								newentry.Keywords = reader.ReadString();
								newentry.Action = reader.ReadString();
								newentry.DependsOn = reader.ReadInt().ToString();
								newentry.Pause = reader.ReadInt();

								m_SpeechEntries.Add(newentry);
							}
							else
							{
								SpeechEntry newentry = m_SpeechEntries[i];

								newentry.EntryNumber = reader.ReadInt();
								newentry.ID = reader.ReadInt();
								newentry.Text = reader.ReadString();
								newentry.Keywords = reader.ReadString();
								newentry.Action = reader.ReadString();
								if (version < 7)
								{
									newentry.DependsOn = reader.ReadInt().ToString();
								}
								else
								{
									newentry.DependsOn = reader.ReadString();
								}
								newentry.Pause = reader.ReadInt();
							}
						}
						// read in the current entry number. Note this will also set the current entry
						EntryNumber = reader.ReadInt();
						// restart the timer if it was active
						bool isrunning = reader.ReadBool();
						if (isrunning)
						{
							Mobile trigmob = reader.ReadMobile();
							TimeSpan delay = reader.ReadTimeSpan();
							DoTimer(delay, trigmob);
						}
						break;
					}
			}
		}
	}
}
