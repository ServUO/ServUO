using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Engines.XmlSpawner2
{
    public class XmlDialog : XmlAttachment
    {
        public static int defProximityRange = 3;
        public static int defResetRange = 16;
        public static TimeSpan defResetTime = TimeSpan.FromSeconds(60);
        public static int defSpeechPace = 10;
        public static string DefsDir = "XmlQuestNPC";
        public ArrayList m_TextEntryBook;
        private const string NPCDataSetName = "XmlQuestNPC";
        private const string NPCPointName = "NPC";
        private const string SpeechPointName = "SpeechEntry";
        private ArrayList m_SpeechEntries = new ArrayList();// contains the list of speech entries
        private int m_CurrentEntryNumber = -1;// used to determine which entry will be subject to modification by various entry editing calls
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
        private Mobile m_ActivePlayer;// keep track of the player that is currently engaged in conversation so that other players speech can be ignored.
        private int m_SpeechPace = defSpeechPace;// used for automatic prepause delay calculation.  delayinsecs = speechlength/speechpace + 1
        bool m_HoldProcessing;
        private string m_ItemTriggerName;
        private string m_NoItemTriggerName;
        private string m_ResponseString;
        // a serial constructor is REQUIRED
        public XmlDialog(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlDialog(string ConfigFile)
        {
            this.DoLoadNPC(null, ConfigFile);
        }

        [Attachable]
        public XmlDialog()
        {
            this.EntryNumber = 0;
        }

        public string ResponseString
        {
            get
            {
                return this.m_ResponseString;
            }
            set
            {
                this.m_ResponseString = value;
            }
        }
        public ArrayList SpeechEntries
        {
            get
            {
                return this.m_SpeechEntries;
            }
            set
            {
                this.m_SpeechEntries = value;
            }
        }
        public Mobile ActivePlayer
        {
            get
            {
                return this.m_ActivePlayer;
            }
            set
            {
                this.m_ActivePlayer = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public AccessLevel TriggerAccessLevel
        {
            get
            {
                return this.m_TriggerAccessLevel;
            }
            set
            {
                this.m_TriggerAccessLevel = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastInteraction
        {
            get
            {
                return this.m_LastInteraction;
            }
            set
            {
                this.m_LastInteraction = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool DoReset
        {
            get
            {
                return false;
            }
            set
            {
                if (value)
                    this.Reset();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsActive
        {
            get
            {
                return this.m_IsActive;
            }
            set
            {
                this.m_IsActive = value;
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
                if (this.AttachedTo is Item)
                {
                    map = ((Item)this.AttachedTo).Map;
                    x = ((Item)this.AttachedTo).Location.X;
                    y = ((Item)this.AttachedTo).Location.Y;
                }
                else if (this.AttachedTo is Mobile)
                {
                    map = ((Mobile)this.AttachedTo).Map;
                    x = ((Mobile)this.AttachedTo).Location.X;
                    y = ((Mobile)this.AttachedTo).Location.Y;
                }

                Server.Items.Clock.GetTime(map, x, y, out hours, out minutes);
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
                if (this.AttachedTo is Item)
                {
                    map = ((Item)this.AttachedTo).Map;
                    x = ((Item)this.AttachedTo).Location.X;
                    y = ((Item)this.AttachedTo).Location.Y;
                }
                else if (this.AttachedTo is Mobile)
                {
                    map = ((Mobile)this.AttachedTo).Map;
                    x = ((Mobile)this.AttachedTo).Location.X;
                    y = ((Mobile)this.AttachedTo).Location.Y;
                }
                return Clock.GetMoonPhase(map, x, y);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowGhostTrig
        {
            get
            {
                return this.m_AllowGhostTriggering;
            }
            set
            {
                this.m_AllowGhostTriggering = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Running
        {
            get
            {
                return this.m_Running;
            }
            set
            {
                this.m_Running = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan ResetTime
        {
            get
            {
                return this.m_ResetTime;
            }
            set
            {
                this.m_ResetTime = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int SpeechPace
        {
            get
            {
                return this.m_SpeechPace;
            }
            set
            {
                this.m_SpeechPace = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Keywords
        {
            get
            {
                // return the keyword string for the current entry
                if (this.m_CurrentEntry == null)
                {
                    return null;
                }
                return this.m_CurrentEntry.Keywords;
            }
            set
            {
                // set the keyword string for the current entry
                if (this.m_CurrentEntry == null)
                {
                    return;
                }
                this.m_CurrentEntry.Keywords = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Action
        {
            get
            {
                if (this.m_CurrentEntry == null)
                {
                    return null;
                }
                return this.m_CurrentEntry.Action;
            }
            set
            {
                if (this.m_CurrentEntry == null)
                {
                    return;
                }
                this.m_CurrentEntry.Action = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Gump
        {
            get
            {
                if (this.m_CurrentEntry == null)
                {
                    return null;
                }
                return this.m_CurrentEntry.Gump;
            }
            set
            {
                if (this.m_CurrentEntry == null)
                {
                    return;
                }
                this.m_CurrentEntry.Gump = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int SpeechHue
        {
            get
            {
                if (this.m_CurrentEntry == null)
                {
                    return 0;
                }
                return this.m_CurrentEntry.SpeechHue;
            }
            set
            {
                if (this.m_CurrentEntry == null)
                {
                    return;
                }
                this.m_CurrentEntry.SpeechHue = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Condition
        {
            get
            {
                if (this.m_CurrentEntry == null)
                {
                    return null;
                }
                return this.m_CurrentEntry.Condition;
            }
            set
            {
                if (this.m_CurrentEntry == null)
                {
                    return;
                }
                this.m_CurrentEntry.Condition = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Text
        {
            get
            {
                if (this.m_CurrentEntry == null)
                {
                    return null;
                }
                return this.m_CurrentEntry.Text;
            }
            set
            {
                if (this.m_CurrentEntry == null)
                {
                    return;
                }
                this.m_CurrentEntry.Text = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string DependsOn
        {
            get
            {
                // return the keyword string for the current entry
                if (this.m_CurrentEntry == null)
                {
                    return "-1";
                }
                return this.m_CurrentEntry.DependsOn;
            }
            set
            {
                // set the keyword string for the current entry
                if (this.m_CurrentEntry == null)
                {
                    return;
                }
                this.m_CurrentEntry.DependsOn = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LockConversation
        {
            get
            {
                if (this.m_CurrentEntry == null)
                {
                    return true;
                }
                return this.m_CurrentEntry.LockConversation;
            }
            set
            {
                if (this.m_CurrentEntry == null)
                {
                    return;
                }
                this.m_CurrentEntry.LockConversation = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IgnoreCarried
        {
            get
            {
                if (this.m_CurrentEntry == null)
                {
                    return true;
                }
                return this.m_CurrentEntry.IgnoreCarried;
            }
            set
            {
                if (this.m_CurrentEntry == null)
                {
                    return;
                }
                this.m_CurrentEntry.IgnoreCarried = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public MessageType SpeechStyle
        {
            get
            {
                if (this.m_CurrentEntry == null)
                {
                    return MessageType.Regular;
                }
                return this.m_CurrentEntry.SpeechStyle;
            }
            set
            {
                if (this.m_CurrentEntry == null)
                {
                    return;
                }
                this.m_CurrentEntry.SpeechStyle = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowNPCTrigger
        {
            get
            {
                if (this.m_CurrentEntry == null)
                {
                    return false;
                }
                return this.m_CurrentEntry.AllowNPCTrigger;
            }
            set
            {
                if (this.m_CurrentEntry == null)
                {
                    return;
                }
                this.m_CurrentEntry.AllowNPCTrigger = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Pause
        {
            get
            {
                if (this.m_CurrentEntry == null)
                {
                    return -1;
                }
                return this.m_CurrentEntry.Pause;
            }
            set
            {
                if (this.m_CurrentEntry == null)
                {
                    return;
                }
                this.m_CurrentEntry.Pause = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int PrePause
        {
            get
            {
                if (this.m_CurrentEntry == null)
                {
                    return -1;
                }
                return this.m_CurrentEntry.PrePause;
            }
            set
            {
                if (this.m_CurrentEntry == null)
                {
                    return;
                }
                this.m_CurrentEntry.PrePause = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ID
        {
            get
            {
                if (this.m_CurrentEntry == null)
                {
                    return -1;
                }
                return this.m_CurrentEntry.ID;
            }
            set
            {
                if (this.m_CurrentEntry == null)
                {
                    return;
                }

                this.m_CurrentEntry.ID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int EntryNumber
        {
            get
            {
                return this.m_CurrentEntryNumber;
            }
            set
            {
                this.m_CurrentEntryNumber = value;
                // get the entry corresponding to the number
                this.m_CurrentEntry = this.GetEntry(this.m_CurrentEntryNumber);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ProximityRange
        {
            get
            {
                return this.m_ProximityRange;
            }
            set
            {
                this.m_ProximityRange = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ResetRange
        {
            get
            {
                return this.m_ResetRange;
            }
            set
            {
                this.m_ResetRange = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string ConfigFile
        {
            get
            {
                return this.m_ConfigFile;
            }
            set
            {
                this.m_ConfigFile = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LoadConfig
        {
            get
            {
                return false;
            }
            set
            {
                if (value == true)
                    this.DoLoadNPC(null, this.ConfigFile);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string TriggerOnCarried
        {
            get
            {
                return this.m_ItemTriggerName;
            }
            set
            {
                this.m_ItemTriggerName = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string NoTriggerOnCarried
        {
            get
            {
                return this.m_NoItemTriggerName;
            }
            set
            {
                this.m_NoItemTriggerName = value;
            }
        }
        public SpeechEntry CurrentEntry
        {
            get
            {
                return this.m_CurrentEntry;
            }
            set
            {
                // get the entry corresponding to the number
                this.m_CurrentEntry = value;
                if (this.m_CurrentEntry != null)
                    this.m_CurrentEntryNumber = this.m_CurrentEntry.EntryNumber;
                else
                    this.m_CurrentEntryNumber = -1;
            }
        }
        public override bool HandlesOnSpeech
        {
            get
            {
                return (this.m_Running);
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return (this.m_Running);
            }
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

        [Usage("XmlSaveNPC filename")]
        [Description("Saves the targeted Talking NPC to an xml file.")]
        public static void SaveNPC_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new SaveNPCTarget(e);
        }

        [Usage("XmlLoadNPC filename")]
        [Description("Loads the targeted Talking NPC to an xml file.")]
        public static void LoadNPC_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new LoadNPCTarget(e);
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

        public void DeleteTextEntryBook()
        {
            if (this.m_TextEntryBook != null)
            {
                foreach (Item s in this.m_TextEntryBook)
                    s.Delete();

                this.m_TextEntryBook = null;
            }
        }

        // see if the DependsOn property contains the specified id
        public bool CheckDependsOn(SpeechEntry s, int id)
        {
            if (s == null || s.DependsOn == null)
                return false;

            // parse the DependsOn string
            string[] args = s.DependsOn.Split(',');
            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    if (int.Parse(args[i].Trim()) == id)
                        return true;
                }
                catch
                {
                }
            }
            return false;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (e.Mobile == null)
                return;

            // dont handle your own speech
            if (e.Mobile == this.AttachedTo as Mobile || e.Mobile.AccessLevel > this.TriggerAccessLevel)
            {
                e.Handled = false;
                return;
            }

            if (this.m_HoldProcessing)
                return;

            bool lockconversation = true;
            bool ishandled = false;

            Point3D loc = new Point3D(0, 0, 0);
            Map map;

            if (this.AttachedTo is Mobile)
            {
                Mobile m = this.AttachedTo as Mobile;
                loc = m.Location;
                map = m.Map;
            }
            else if (this.AttachedTo is Item && ((Item)this.AttachedTo).Parent == null)
            {
                Item i = this.AttachedTo as Item;
                loc = i.Location;
                map = i.Map;
            }

            if (this.CurrentEntry != null)
            {
                lockconversation = this.CurrentEntry.LockConversation;
            }

            if (!e.Handled && this.m_Running && this.m_ProximityRange >= 0 && this.ValidSpeechTrig(e.Mobile) && ((e.Mobile == this.m_ActivePlayer) || !lockconversation || this.m_ActivePlayer == null))
            {
                if (!Utility.InRange(e.Mobile.Location, loc, this.m_ProximityRange))
                    return;

                this.CheckForReset();

                // process the current speech entry
                ishandled = this.ProcessSpeech(e.Mobile, e.Speech);

                // check to make sure the timer is running
                this.DoTimer(TimeSpan.FromSeconds(1), this.m_ActivePlayer);
            }

            if (!ishandled)
            {
                base.OnSpeech(e);
            }
        }

        public override void OnMovement(MovementEventArgs e)
        {
            Mobile m = e.Mobile;

            if (m == null || m.AccessLevel > this.TriggerAccessLevel)
                return;

            Point3D loc = new Point3D(0, 0, 0);
            Map map;

            if (this.AttachedTo is Mobile)
            {
                Mobile mob = this.AttachedTo as Mobile;
                loc = mob.Location;
                map = mob.Map;
            }
            else if (this.AttachedTo is Item && ((Item)this.AttachedTo).Parent == null)
            {
                Item i = this.AttachedTo as Item;
                loc = i.Location;
                map = i.Map;
            }

            // if proximity sensing is off, a speech entry has been activated, or player is an admin then ignore
            if (this.m_Running && this.m_ProximityRange >= 0 && this.ValidMovementTrig(m) && !this.IsActive && !this.m_HoldProcessing)
            {
                // check to see if player is within range of the npc
                if (Utility.InRange(m.Location, loc, this.m_ProximityRange))
                {
                    TimeSpan pause = TimeSpan.FromSeconds(0);
                    if (this.CurrentEntry != null && this.CurrentEntry.Pause > 0)
                    {
                        pause = TimeSpan.FromSeconds(this.CurrentEntry.Pause);
                    }
                    // check to see if the current pause interval has elapsed
                    if (DateTime.UtcNow - pause > this.m_LastInteraction)
                    {
                        // process speech that is not keyword dependent
                        this.CheckForReset();

                        this.ProcessSpeech(m, null);
                    }
                    // turn on the timer that will run until the speech list is reset
                    // it will control paused speech and will allow the speech entry to be reset after ResetTime timeout
                    this.DoTimer(TimeSpan.FromSeconds(1), m);
                }
            }
            else
            {
                this.CheckForReset();
            }
            base.OnMovement(e);
        }

        public bool ProcessSpeech(Mobile m, string speech)
        {
            if (this.m_HoldProcessing)
                return true;

            // check the speech against the entries that depend on the present entry
            SpeechEntry matchentry = this.FindMatchingKeyword(m, speech, this.ID);

            if (matchentry == null)
                return false;

            // when attempting to process speech-triggered speech, check for oncarried dependencies
            // This will not apply to movement-triggered speech (banter with -1 dependson) which will continue to be activated
            // regardless of oncarried status
            // dependson of -2 will allow non-speech triggering but will still apply oncarried dependencies

            // if player-carried item triggering is set then test for the presence of an item on the player an in their pack
            if ((speech != null || this.CheckDependsOn(matchentry, -2)) && this.TriggerOnCarried != null && this.TriggerOnCarried.Length > 0)
            {
                bool found = BaseXmlSpawner.CheckForCarried(m, this.TriggerOnCarried) || matchentry.IgnoreCarried;

                // is the player carrying the right item, if not then dont process
                if (!found)
                    return false;
            }

            // if player-carried noitem triggering is set then test for the presence of an item in the players pack that should block triggering
            if ((speech != null || this.CheckDependsOn(matchentry, -2)) && this.NoTriggerOnCarried != null && this.NoTriggerOnCarried.Length > 0)
            {
                bool notfound = BaseXmlSpawner.CheckForNotCarried(m, this.NoTriggerOnCarried) || matchentry.IgnoreCarried;

                // is the player carrying the right item, if so then dont process
                if (!notfound)
                    return false;
            }

            this.ResponseString = speech;

            // the player that successfully activates a conversation by speech becomes the exclusive conversationalist until the npc resets
            if (speech != null && m != null)
                this.m_ActivePlayer = m;

            // calculate the delay before activating the entry
            int prepause = 1;    // 1 sec by default
            if (matchentry.PrePause < 0)
            {
                if (this.SpeechPace > 0 && speech != null)
                {
                    // do the auto delay calculation based on the length of the triggering speech
                    prepause = (speech.Length / this.SpeechPace) + 1;    // make 1 sec the min pause
                }
            }
            else
            {
                prepause = matchentry.PrePause;
            }

            // and switch to the one that matches

            this.m_HoldProcessing = true;
            Timer.DelayCall(TimeSpan.FromSeconds(prepause), new TimerStateCallback(DelayedSpeech), new object[] { matchentry, m });

            return true;
        }

        public void DoTimer(TimeSpan delay, Mobile trigmob)
        {
            if (!this.m_Running)
                return;

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = new InternalTimer(this, delay, trigmob);
            this.m_Timer.Start();
        }

        public void DoLoadNPC(Mobile from, string filename)
        {
            if (filename == null || filename.Length <= 0)
                return;

            string dirname;
            if (System.IO.Directory.Exists(DefsDir) == true)
            {
                // look for it in the defaults directory
                dirname = String.Format("{0}/{1}.npc", DefsDir, filename);

                // Check if the file exists
                if (System.IO.File.Exists(dirname) == false)
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
            if (System.IO.File.Exists(dirname) == true)
            {
                FileStream fs = null;
                try
                {
                    fs = File.Open(dirname, FileMode.Open, FileAccess.Read);
                }
                catch
                {
                }

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
                catch
                {
                    fileerror = true;
                }

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
                            if (this.AttachedTo is Item)
                            {
                                ((Item)this.AttachedTo).Name = (string)dr["Name"];
                            }
                            else if (this.AttachedTo is Mobile)
                            {
                                ((Mobile)this.AttachedTo).Name = (string)dr["Name"];
                            }
                        }
                        catch
                        {
                        }
                        try
                        {
                            this.ProximityRange = int.Parse((string)dr["ProximityRange"]);
                        }
                        catch
                        {
                        }
                        try
                        {
                            this.ResetRange = int.Parse((string)dr["ResetRange"]);
                        }
                        catch
                        {
                        }
                        try
                        {
                            this.TriggerOnCarried = (string)dr["TriggerOnCarried"];
                        }
                        catch
                        {
                        }
                        try
                        {
                            this.NoTriggerOnCarried = (string)dr["NoTriggerOnCarried"];
                        }
                        catch
                        {
                        }
                        try
                        {
                            this.m_AllowGhostTriggering = bool.Parse((string)dr["AllowGhost"]);
                        }
                        catch
                        {
                        }
                        try
                        {
                            this.m_SpeechPace = int.Parse((string)dr["SpeechPace"]);
                        }
                        catch
                        {
                        }
                        try
                        {
                            this.Running = bool.Parse((string)dr["Running"]);
                        }
                        catch
                        {
                        }
                        try
                        {
                            this.ResetTime = TimeSpan.FromMinutes(double.Parse((string)dr["ResetTime"]));
                        }
                        catch
                        {
                        }
                        try
                        {
                            this.ConfigFile = (string)dr["ConfigFile"];
                        }
                        catch
                        {
                        }

                        int entrycount = 0;
                        try
                        {
                            entrycount = int.Parse((string)dr["SpeechEntries"]);
                        }
                        catch
                        {
                        }
                    }

                    // get the speech entry info
                    if (ds.Tables[NPCPointName] != null && ds.Tables[NPCPointName].Rows.Count > 0)
                    {
                        this.m_SpeechEntries = new ArrayList();
                        foreach (DataRow dr in ds.Tables[SpeechPointName].Rows)
                        {
                            SpeechEntry s = new SpeechEntry();
                            // Populate the speech entry data
                            try
                            {
                                s.EntryNumber = int.Parse((string)dr["EntryNumber"]);
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.ID = int.Parse((string)dr["ID"]);
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.Text = (string)dr["Text"];
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.Keywords = (string)dr["Keywords"];
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.Action = (string)dr["Action"];
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.Condition = (string)dr["Condition"];
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.DependsOn = (string)dr["DependsOn"];
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.Pause = int.Parse((string)dr["Pause"]);
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.PrePause = int.Parse((string)dr["PrePause"]);
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.LockConversation = bool.Parse((string)dr["LockConversation"]);
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.IgnoreCarried = bool.Parse((string)dr["IgnoreCarried"]);
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.AllowNPCTrigger = bool.Parse((string)dr["AllowNPCTrigger"]);
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.SpeechStyle = (MessageType)Enum.Parse(typeof(MessageType), (string)dr["SpeechStyle"]);
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.SpeechHue = int.Parse((string)dr["SpeechHue"]);
                            }
                            catch
                            {
                            }
                            try
                            {
                                s.Gump = (string)dr["Gump"];
                            }
                            catch
                            {
                            }

                            this.m_SpeechEntries.Add(s);
                        }
                    }

                    this.Reset();

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
            if (filename == null || filename.Length <= 0)
                return;

            // Create the data set
            DataSet ds = new DataSet(NPCDataSetName);

            // Load the data set up
            ds.Tables.Add(NPCPointName);
            ds.Tables.Add(SpeechPointName);

            // Create a schema for the npc
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

            // Populate the npc data
            if (this.AttachedTo is Item)
            {
                dr["Name"] = (string)((Item)this.AttachedTo).Name;
            }
            else if (this.AttachedTo is Mobile)
            {
                dr["Name"] = (string)((Mobile)this.AttachedTo).Name;
            }

            dr["Running"] = (bool)this.Running;
            dr["ProximityRange"] = (int)this.m_ProximityRange;
            dr["ResetRange"] = (int)this.m_ResetRange;
            dr["TriggerOnCarried"] = (string)this.TriggerOnCarried;
            dr["NoTriggerOnCarried"] = (string)this.NoTriggerOnCarried;
            dr["AllowGhost"] = (bool)this.m_AllowGhostTriggering;
            dr["SpeechPace"] = (int)this.SpeechPace;
            dr["ResetTime"] = (double)this.ResetTime.TotalMinutes;
            dr["ConfigFile"] = (string)this.ConfigFile;
            int entrycount = 0;
            if (this.SpeechEntries != null)
            {
                entrycount = this.SpeechEntries.Count;
            }
            dr["SpeechEntries"] = (int)entrycount;

            // Add the row the the table
            ds.Tables[NPCPointName].Rows.Add(dr);

            for (int i = 0; i < entrycount; i++)
            {
                SpeechEntry s = (SpeechEntry)this.SpeechEntries[i];

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

            if (System.IO.Directory.Exists(DefsDir) == true)
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

            if (System.IO.File.Exists(dirname) == true)
            {
                // prompt the user to save over it
                if (from != null)
                {
                    from.SendGump(new ConfirmSaveGump(this, ds, dirname, filename, updateconfig));
                }
            }
            else
            {
                this.SaveFile(from, ds, dirname, filename, updateconfig);
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
            catch
            {
                file_error = true;
            }

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
                    this.ConfigFile = configname;
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)9); // version
            // Version 9 added the ResetRange property
            writer.Write(this.m_ResetRange);
            // Version 8 added the IgnoreCarried property
            if (this.m_SpeechEntries != null)
            {
                writer.Write((int)this.m_SpeechEntries.Count);
                foreach (SpeechEntry s in this.m_SpeechEntries)
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
            if (this.m_SpeechEntries != null)
            {
                writer.Write((int)this.m_SpeechEntries.Count);
                foreach (SpeechEntry s in this.m_SpeechEntries)
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
            if (this.m_SpeechEntries != null)
            {
                writer.Write((int)this.m_SpeechEntries.Count);
                foreach (SpeechEntry s in this.m_SpeechEntries)
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
            if (this.m_SpeechEntries != null)
            {
                writer.Write((int)this.m_SpeechEntries.Count);
                foreach (SpeechEntry s in this.m_SpeechEntries)
                {
                    writer.Write(s.Condition);
                }
            }
            else
            {
                writer.Write((int)0);
            }
            // Version 3
            writer.Write(this.TriggerOnCarried);
            writer.Write(this.NoTriggerOnCarried);
            // Version 2
            writer.Write(this.m_SpeechPace);
            // write out the additional speech entry fields
            if (this.m_SpeechEntries != null)
            {
                writer.Write((int)this.m_SpeechEntries.Count);
                foreach (SpeechEntry s in this.m_SpeechEntries)
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
            writer.Write(this.m_ActivePlayer);

            // Version 0
            writer.Write(this.m_IsActive);
            writer.Write(this.m_ResetTime);
            writer.Write(this.m_LastInteraction);
            writer.Write(this.m_AllowGhostTriggering);
            writer.Write(this.m_ProximityRange);
            writer.Write(this.m_Running);
            writer.Write(this.m_ConfigFile);
            // write out the speech entries
            if (this.m_SpeechEntries != null)
            {
                writer.Write((int)this.m_SpeechEntries.Count);
                foreach (SpeechEntry s in this.m_SpeechEntries)
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
            writer.Write(this.m_CurrentEntryNumber);
            // check to see if the timer is running
            if (this.m_Timer != null && this.m_Timer.Running)
            {
                writer.Write(true);
                writer.Write(this.m_Timer.m_trigmob);
                writer.Write(this.m_Timer.m_delay);
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
                        this.m_ResetRange = reader.ReadInt();
                        goto case 8;
                    }
                case 8:
                    {
                        int count = reader.ReadInt();
                        this.m_SpeechEntries = new ArrayList();
                        for (int i = 0; i < count; i++)
                        {
                            SpeechEntry newentry = new SpeechEntry();

                            newentry.IgnoreCarried = reader.ReadBool();

                            this.m_SpeechEntries.Add(newentry);
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
                            this.m_SpeechEntries = new ArrayList();
                        }
                        for (int i = 0; i < count; i++)
                        {
                            if (version < 8)
                            {
                                SpeechEntry newentry = new SpeechEntry();

                                newentry.SpeechHue = reader.ReadInt();

                                this.m_SpeechEntries.Add(newentry);
                            }
                            else
                            {
                                SpeechEntry newentry = (SpeechEntry)this.m_SpeechEntries[i];

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
                            this.m_SpeechEntries = new ArrayList();
                        }
                        for (int i = 0; i < count; i++)
                        {
                            if (version < 6)
                            {
                                SpeechEntry newentry = new SpeechEntry();

                                newentry.Gump = reader.ReadString();

                                this.m_SpeechEntries.Add(newentry);
                            }
                            else
                            {
                                SpeechEntry newentry = (SpeechEntry)this.m_SpeechEntries[i];

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
                            this.m_SpeechEntries = new ArrayList();
                        }
                        for (int i = 0; i < count; i++)
                        {
                            if (version < 5)
                            {
                                SpeechEntry newentry = new SpeechEntry();

                                newentry.Condition = reader.ReadString();

                                this.m_SpeechEntries.Add(newentry);
                            }
                            else
                            {
                                SpeechEntry newentry = (SpeechEntry)this.m_SpeechEntries[i];

                                newentry.Condition = reader.ReadString();
                            }
                        }

                        goto case 3;
                    }
                case 3:
                    {
                        this.TriggerOnCarried = reader.ReadString();
                        this.NoTriggerOnCarried = reader.ReadString();
                        goto case 2;
                    }
                case 2:
                    {
                        this.m_SpeechPace = reader.ReadInt();

                        int count = reader.ReadInt();
                        if (version < 4)
                        {
                            this.m_SpeechEntries = new ArrayList();
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

                                this.m_SpeechEntries.Add(newentry);
                            }
                            else
                            {
                                SpeechEntry newentry = (SpeechEntry)this.m_SpeechEntries[i];

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
                        this.m_ActivePlayer = reader.ReadMobile();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_IsActive = reader.ReadBool();
                        this.m_ResetTime = reader.ReadTimeSpan();
                        this.m_LastInteraction = reader.ReadDateTime();
                        this.m_AllowGhostTriggering = reader.ReadBool();
                        this.m_ProximityRange = reader.ReadInt();
                        this.m_Running = reader.ReadBool();
                        this.m_ConfigFile = reader.ReadString();
                        int count = reader.ReadInt();
                        if (version < 2)
                        {
                            this.m_SpeechEntries = new ArrayList();
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

                                this.m_SpeechEntries.Add(newentry);
                            }
                            else
                            {
                                SpeechEntry newentry = (SpeechEntry)this.m_SpeechEntries[i];

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
                        this.EntryNumber = reader.ReadInt();
                        // restart the timer if it was active
                        bool isrunning = reader.ReadBool();
                        if (isrunning)
                        {
                            Mobile trigmob = reader.ReadMobile();
                            TimeSpan delay = reader.ReadTimeSpan();
                            this.DoTimer(delay, trigmob);
                        }
                        break;
                    }
            }
        }

        private SpeechEntry GetEntry(int entryid)
        {
            if (entryid < 0)
                return null;
            if (this.m_SpeechEntries == null)
            {
                this.m_SpeechEntries = new ArrayList();
            }
            // find the speech entry that matches the current entry number
            foreach (SpeechEntry s in this.m_SpeechEntries)
            {
                if (s.EntryNumber == entryid)
                    return s;
            }
            // didnt find it so make a new entry
            SpeechEntry newentry = new SpeechEntry();
            newentry.EntryNumber = entryid;
            newentry.ID = entryid;
            this.m_SpeechEntries.Add(newentry);
            return newentry;
        }

        private bool ValidMovementTrig(Mobile m)
        {
            if (m == null || m.Deleted)
                return false;

            return (
                    ((m is PlayerMobile && (m.AccessLevel <= this.TriggerAccessLevel))) &&
                    ((!m.Body.IsGhost && !this.m_AllowGhostTriggering) || (m.Body.IsGhost && this.m_AllowGhostTriggering)));
        }

        private bool ValidSpeechTrig(Mobile m)
        {
            if (m == null || m.Deleted)
                return false;

            bool allownpctrigger = false;
            if (this.CurrentEntry != null)
            {
                allownpctrigger = this.CurrentEntry.AllowNPCTrigger;
            }

            return (
                    ((m is PlayerMobile && (m.AccessLevel <= this.TriggerAccessLevel)) || (allownpctrigger && !(m is PlayerMobile))) &&
                    ((!m.Body.IsGhost && !this.m_AllowGhostTriggering) || (m.Body.IsGhost && this.m_AllowGhostTriggering)));
        }

        private SpeechEntry FindMatchingKeyword(Mobile from, string keyword, int currententryid)
        {
            if (this.m_SpeechEntries == null)
                return null;
            ArrayList matchlist = new ArrayList();

            // go through all of the speech entries and find those that depend on the current entry
            foreach (SpeechEntry s in this.m_SpeechEntries)
            {
                // ignore self-referencing entries
                if (this.CheckDependsOn(s, s.ID))
                    continue;

                // start processing if set for spontaneous activation (banter), already active, or waiting in the default state
                if (((this.CheckDependsOn(s, -1) || this.CheckDependsOn(s, -2)) && !this.IsActive) || (this.CheckDependsOn(s, currententryid) && (this.IsActive || currententryid == 0)))
                {
                    // now check for any conditions as well
                    // check for any condition that must be met for this entry to be processed
                    if (s.Condition != null)
                    {
                        string status_str;

                        if (!BaseXmlSpawner.CheckPropertyString(null, this, s.Condition, from, out status_str))
                        {
                            continue;
                        }
                    }

                    // testing for keyword = null will handle calls from the OnTick
                    if ((keyword == null && s.Keywords == null))
                    {
                        // add it to the list of match candidates
                        matchlist.Add(s);
                    }
                    else if (keyword != null && s.Keywords != null)
                    {
                        string[] arglist = s.Keywords.Split(",".ToCharArray());
                        for (int i = 0; i < arglist.Length; i++)
                        {
                            if (arglist[i] == "*")
                            {
                                // special match anything expression (regex doesnt parse this well)
                                matchlist.Add(s);
                                break;
                            }
                            else
                            {
                                bool error = false;
                                Regex r = null;
                                string status_str = null;
                                try
                                {
                                    r = new Regex(arglist[i], RegexOptions.IgnoreCase);
                                }
                                catch (Exception e)
                                {
                                    error = true;
                                    status_str = e.Message;
                                }

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
                                    this.ReportError(from, String.Format("Bad regular expression: {0} ", status_str));
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

                return (SpeechEntry)matchlist[select];
            }
            else
            {
                // didnt find a match
                return null;
            }
        }

        private void ExecuteGump(Mobile mob, string gumpstring)
        {
            if (gumpstring == null || gumpstring.Length <= 0)
                return;

            string status_str = null;

            Server.Mobiles.XmlSpawner.SpawnObject TheSpawn = new Server.Mobiles.XmlSpawner.SpawnObject(null, 0);

            TheSpawn.TypeName = gumpstring;
            string substitutedtypeName = BaseXmlSpawner.ApplySubstitution(null, this, mob, gumpstring);
            string typeName = BaseXmlSpawner.ParseObjectType(substitutedtypeName);

            Point3D loc = new Point3D(0, 0, 0);
            Map map = null;

            if (this.AttachedTo is Mobile)
            {
                Mobile m = this.AttachedTo as Mobile;
                loc = m.Location;
                map = m.Map;
            }
            else if (this.AttachedTo is Item && ((Item)this.AttachedTo).Parent == null)
            {
                Item i = this.AttachedTo as Item;
                loc = i.Location;
                map = i.Map;
            }

            if (typeName == "GUMP")
            {
                BaseXmlSpawner.SpawnTypeKeyword(this, TheSpawn, typeName, substitutedtypeName, true, mob, loc, map, new XmlGumpCallback(DialogGumpCallback), out status_str);
                // hold processing until the gump response is completed

                this.m_HoldProcessing = true;
            }
            else
            {
                status_str = "not a GUMP specification";
            }

            this.ReportError(mob, status_str);
        }

        private void ExecuteAction(Mobile mob, string action)
        {
            if (action == null || action.Length <= 0)
                return;
            string status_str = null;
            Server.Mobiles.XmlSpawner.SpawnObject TheSpawn = new Server.Mobiles.XmlSpawner.SpawnObject(null, 0);

            TheSpawn.TypeName = action;
            string substitutedtypeName = BaseXmlSpawner.ApplySubstitution(null, this, mob, action);
            string typeName = BaseXmlSpawner.ParseObjectType(substitutedtypeName);

            Point3D loc = new Point3D(0, 0, 0);
            Map map = null;

            if (this.AttachedTo is Mobile)
            {
                Mobile m = this.AttachedTo as Mobile;
                loc = m.Location;
                map = m.Map;
            }
            else if (this.AttachedTo is Item && ((Item)this.AttachedTo).Parent == null)
            {
                Item i = this.AttachedTo as Item;
                loc = i.Location;
                map = i.Map;
            }

            if (BaseXmlSpawner.IsTypeOrItemKeyword(typeName))
            {
                BaseXmlSpawner.SpawnTypeKeyword(this.AttachedTo, TheSpawn, typeName, substitutedtypeName, true, mob, loc, map, out status_str);
            }
            else
            {
                // its a regular type descriptor so find out what it is
                Type type = SpawnerType.GetType(typeName);
                try
                {
                    string[] arglist = BaseXmlSpawner.ParseString(substitutedtypeName, 3, "/");
                    object o = Server.Mobiles.XmlSpawner.CreateObject(type, arglist[0]);

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

                        BaseXmlSpawner.ApplyObjectStringProperties(null, substitutedtypeName, m, mob, this.AttachedTo, out status_str);
                    }
                    else if (o is Item)
                    {
                        Item item = (Item)o;
                        BaseXmlSpawner.AddSpawnItem(null, this.AttachedTo, TheSpawn, item, loc, map, mob, false, substitutedtypeName, out status_str);
                    }
                }
                catch
                {
                }
            }

            this.ReportError(mob, status_str);
        }

        private void ReportError(Mobile mob, string status_str)
        {
            if (status_str != null && mob != null && !mob.Deleted && mob is PlayerMobile && mob.AccessLevel > AccessLevel.Player)
            {
                mob.SendMessage(33, String.Format("{0}:{1}", this.AttachedTo.GetType().Name, status_str));
            }
        }

        private bool IsInRange(IEntity e1, IEntity e2, int range)
        {
            if (e1 == null || e2 == null)
                return false;

            if (e1.Map != e2.Map)
                return false;

            return Utility.InRange(e1.Location, e2.Location, range);
        }

        private void CheckForReset()
        {
            // check to see if the interaction time has elapsed or player has gone out of range.  If so then reset to entry zero
            if (!this.m_HoldProcessing &&
                ((DateTime.UtcNow - this.ResetTime > this.m_LastInteraction) ||
                 (this.AttachedTo is IEntity && this.m_ActivePlayer != null && !this.IsInRange(this.m_ActivePlayer, (IEntity)this.AttachedTo, this.ResetRange))))
            {
                this.Reset();
            }
        }

        private void Reset()
        {
            this.EntryNumber = 0;
            this.IsActive = false;
            this.m_ActivePlayer = null;
            // turn off the timer
            if (this.m_Timer != null)
                this.m_Timer.Stop();
        }

        private void DelayedSpeech(object state)
        {
            object[] states = (object[])state;

            SpeechEntry matchentry = (SpeechEntry)states[0];
            Mobile m = (Mobile)states[1];

            if (matchentry != null)
            {
                this.CurrentEntry = matchentry;

                string text = BaseXmlSpawner.ApplySubstitution(null, this, m, this.CurrentEntry.Text);

                if (text != null)
                {
                    // dont know why emote doesnt work, but we'll just do it manually
                    if (this.CurrentEntry.SpeechStyle == MessageType.Emote)
                    {
                        text = String.Format("*{0}*", text);
                    }

                    // items cannot produce actual speech
                    // display a message over the item it was attached to
                    if (this.AttachedTo is Item)
                    {
                        int speechhue = 0x3B2;
                        if (this.CurrentEntry.SpeechHue >= 0)
                        {
                            speechhue = this.CurrentEntry.SpeechHue;
                        }
                        ((Item)this.AttachedTo).PublicOverheadMessage(MessageType.Regular, speechhue, true, text);
                    }
                    else if (this.AttachedTo is Mobile)
                    {
                        // mobiles can produce actual speech
                        // so let them.  This allows mobiles to talk with one another
                        int speechhue = ((Mobile)this.AttachedTo).SpeechHue;
                        if (this.CurrentEntry.SpeechHue >= 0)
                        {
                            speechhue = this.CurrentEntry.SpeechHue;
                        }

                        ((Mobile)this.AttachedTo).DoSpeech(text, new int[] { }, this.CurrentEntry.SpeechStyle, speechhue);
                        //((Mobile)AttachedTo).PublicOverheadMessage( MessageType.Regular, 0x3B2, true, text );
                    }
                }

                this.IsActive = true;
                this.m_LastInteraction = DateTime.UtcNow;

                // execute any action associated with it
                // allow for multiple action strings on a single line separated by a semicolon
                if (this.CurrentEntry.Action != null && this.CurrentEntry.Action.Length > 0)
                {
                    string[] args = this.CurrentEntry.Action.Split(';');

                    for (int j = 0; j < args.Length; j++)
                    {
                        this.ExecuteAction(m, args[j]);
                    }
                }

                // execute any GUMP associated with it
                this.ExecuteGump(m, this.CurrentEntry.Gump);
            }

            this.m_HoldProcessing = false;
        }

        // speech entry
        public class SpeechEntry
        {
            public int EntryNumber;
            public string Text;// text displayed when the entry is activated
            public string Keywords;// comma separated list of keywords that can be matched to activate the entry.  If no keywords are present then it is automatically activated
            public string Action;// action string
            public string Condition;// condition test string
            public string DependsOn;// the previous entrynumber required to activate this entry
            public int Pause = 1;// pause in seconds before advancing to the next entry
            public int PrePause = -1;// pause in seconds before saying the speech for this entry.  -1 indicates the use of auto pause calculation based on triggering speech length.
            public bool LockConversation = true;// flag to determine if the conversation locks to one player
            public bool AllowNPCTrigger = false;// flag to determine if npc speech can trigger it
            public MessageType SpeechStyle = MessageType.Regular;
            public string Gump;// GUMP specification string
            public int m_SpeechHue = -1;// speech hue
            public bool IgnoreCarried = false;// ignore the TriggerOnCarried/NoTriggerOnCarried settings for the dialog when activating this entry
            private int m_ID;
            public int SpeechHue
            {
                get
                {
                    return this.m_SpeechHue;
                }
                set
                {
                    // dont allow invalid hues
                    this.m_SpeechHue = value;
                    if (this.m_SpeechHue > 37852)
                        this.m_SpeechHue = 37852;
                }
            }
            public int ID
            {
                get
                {
                    return this.m_ID;
                }
                set
                {
                    // dont allow ID modification of entry 0
                    if (this.EntryNumber == 0)
                        return;
                    this.m_ID = value;
                }
            }
        }

        public class ConfirmSaveGump : Gump
        {
            private readonly XmlDialog m_dialog;
            private readonly DataSet m_ds;
            private readonly string m_filename;
            private readonly string m_configname;
            private readonly bool m_updateconfig;
            public ConfirmSaveGump(XmlDialog dialog, DataSet ds, string filename, string configname, bool updateconfig)
                : base(0, 0)
            {
                this.m_dialog = dialog;
                this.m_ds = ds;
                this.m_filename = filename;
                this.m_configname = configname;
                this.m_updateconfig = updateconfig;

                this.Closable = false;
                this.Dragable = true;
                this.AddPage(0);
                this.AddBackground(10, 200, 200, 130, 5054);

                this.AddLabel(20, 210, 33, String.Format("{0} exists.", filename));
                this.AddLabel(20, 230, 33, String.Format("Overwrite?", filename));
                this.AddRadio(35, 255, 9721, 9724, false, 1); // accept/yes radio
                this.AddRadio(135, 255, 9721, 9724, true, 2); // decline/no radio
                this.AddHtmlLocalized(72, 255, 200, 30, 1049016, 0x7fff, false, false); // Yes
                this.AddHtmlLocalized(172, 255, 200, 30, 1049017, 0x7fff, false, false); // No
                this.AddButton(80, 289, 2130, 2129, 3, GumpButtonType.Reply, 0); // Okay button
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                if (info == null || state == null || state.Mobile == null)
                    return;

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
                            { // accept
                                if (this.m_dialog != null)
                                    this.m_dialog.SaveFile(state.Mobile, this.m_ds, this.m_filename, this.m_configname, this.m_updateconfig);
                            }
                            else
                            {
                                state.Mobile.SendMessage("File {0} not saved.", this.m_filename);
                            }
                            break;
                        }
                }
            }
        }

        private class InternalTimer : Timer
        {
            public readonly Mobile m_trigmob;
            public readonly TimeSpan m_delay;
            private readonly XmlDialog m_npc;
            public InternalTimer(XmlDialog npc, TimeSpan delay, Mobile trigmob)
                : base(delay, delay)
            {
                this.Priority = TimerPriority.OneSecond;

                this.m_npc = npc;
                this.m_trigmob = trigmob;
                this.m_delay = delay;
            }

            protected override void OnTick()
            {
                if (this.m_npc != null && !this.m_npc.Deleted)
                {
                    // check to see if any speech needs to be processed
                    TimeSpan pause = TimeSpan.FromSeconds(0);
                    if (this.m_npc.CurrentEntry != null && this.m_npc.CurrentEntry.Pause > 0)
                    {
                        pause = TimeSpan.FromSeconds(this.m_npc.CurrentEntry.Pause);
                    }
                    // check to see if the current pause interval has elapsed
                    if (DateTime.UtcNow - pause > this.m_npc.LastInteraction)
                    {
                        // process speech that is not keyword dependent
                        this.m_npc.ProcessSpeech(this.m_trigmob, null);
                        this.m_npc.CheckForReset();
                    }
                }
                else
                {
                    this.Stop();
                }
            }
        }

        private class SaveNPCTarget : Target
        {
            private readonly CommandEventArgs m_e;
            public SaveNPCTarget(CommandEventArgs e)
                : base(30, false, TargetFlags.None)
            {
                this.m_e = e;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                string filename = this.m_e.GetString(0);

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

        private class LoadNPCTarget : Target
        {
            private readonly CommandEventArgs m_e;
            public LoadNPCTarget(CommandEventArgs e)
                : base(30, false, TargetFlags.None)
            {
                this.m_e = e;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                string filename = this.m_e.GetString(0);

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
    }
}