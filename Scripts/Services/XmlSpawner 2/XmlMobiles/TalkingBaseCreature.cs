using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Network;

/*
** TalkingBaseCreature
** A mobile that can be programmed with branching conversational sequences that are advanced by keywords at each sequence point.
**
** 2/10/05
** modified to use the XmlDialog attachment
*/
namespace Server.Mobiles
{
    public class TalkingBaseCreature : BaseCreature
    { 
        private XmlDialog m_DialogAttachment;
        private DateTime lasteffect;
        private int m_EItemID = 0;// 0 = disable, 14202 = sparkle, 6251 = round stone, 7885 = light pyramid
        private int m_Duration = 70;
        private Point3D m_Offset = new Point3D(0,0,20);// overhead
        private int m_EHue = 68;// green
        private string m_TalkText;
        public TalkingBaseCreature(AIType ai,
            FightMode mode,
            int iRangePerception,
            int iRangeFight,
            double dActiveSpeed,
            double dPassiveSpeed)
            : base(ai, mode, iRangePerception, iRangeFight, dActiveSpeed, dPassiveSpeed)
        {
            // add the XmlDialog attachment
            this.m_DialogAttachment = new XmlDialog((string)null);
            XmlAttach.AttachTo(this, this.m_DialogAttachment);
        }

        public TalkingBaseCreature(Serial serial)
            : base(serial)
        {
        }

        public XmlDialog DialogAttachment
        {
            get
            {
                return this.m_DialogAttachment;
            }
            set
            {
                this.m_DialogAttachment = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int EItemID
        { 
            get
            {
                return this.m_EItemID;
            }
            set
            { 
                this.m_EItemID = value; 
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D EOffset 
        { 
            get
            {
                return this.m_Offset;
            }
            set 
            { 
                this.m_Offset = value; 
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int EDuration 
        { 
            get
            {
                return this.m_Duration;
            }
            set 
            { 
                this.m_Duration = value; 
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int EHue 
        { 
            get
            {
                return this.m_EHue;
            }
            set 
            { 
                this.m_EHue = value; 
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string TalkText
        {
            get
            {
                return this.m_TalkText;
            }
            set
            {
                this.m_TalkText = value;
            }
        }
        // properties below are modified to access the equivalent XmlDialog properties
        // this is largely for backward compatibility, but it does also add some convenience
        public Mobile ActivePlayer
        {
            get 
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.ActivePlayer;
                else
                    return null;
            }
            set 
            {
                if (this.DialogAttachment != null)
                    this.DialogAttachment.ActivePlayer = value;
            }
        }
        public ArrayList SpeechEntries 
        {
            get 
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.SpeechEntries;
                else
                    return null;
            }
            set 
            {
                if (this.DialogAttachment != null)
                    this.DialogAttachment.SpeechEntries = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan GameTOD
        {
            get
            {
                int hours;
                int minutes;

                Server.Items.Clock.GetTime(this.Map, this.Location.X, this.Location.Y, out hours, out minutes);
                return (new DateTime(DateTime.UtcNow.Year,DateTime.UtcNow.Month,DateTime.UtcNow.Day,hours, minutes,0).TimeOfDay);
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
                return Clock.GetMoonPhase(this.Map, this.Location.X, this.Location.Y);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public AccessLevel TriggerAccessLevel 
        {
            get 
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.TriggerAccessLevel;
                else
                    return AccessLevel.Player;
            }
            set 
            {
                if (this.DialogAttachment != null)
                    this.DialogAttachment.TriggerAccessLevel = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastInteraction 
        {
            get 
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.LastInteraction;
                else
                    return DateTime.MinValue;
            }
            set 
            {
                if (this.DialogAttachment != null)
                    this.DialogAttachment.LastInteraction = value;
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
                if (this.DialogAttachment != null)
                    this.DialogAttachment.DoReset = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsActive 
        {
            get 
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.IsActive;
                else
                    return false;
            }
            set 
            {
                if (this.DialogAttachment != null)
                    this.DialogAttachment.IsActive = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowGhostTrig 
        {
            get 
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.AllowGhostTrig;
                else
                    return false;
            }
            set 
            {
                if (this.DialogAttachment != null)
                    this.DialogAttachment.AllowGhostTrig = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Running
        {
            get
            { 
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.Running;
                else
                    return false;
            }
            set
            {
                if (this.DialogAttachment != null)
                    this.DialogAttachment.Running = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan ResetTime
        {
            get
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.ResetTime;
                else
                    return TimeSpan.Zero;
            }
            set
            { 
                if (this.DialogAttachment != null)
                    this.DialogAttachment.ResetTime = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int SpeechPace
        {
            get
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.SpeechPace;
                else
                    return 0;
            }
            set
            { 
                if (this.DialogAttachment != null)
                    this.DialogAttachment.SpeechPace = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Keywords
        {
            get
            {
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                {
                    return this.DialogAttachment.CurrentEntry.Keywords;
                }
                else
                    return null;
            }
            set
            { 
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                    this.DialogAttachment.CurrentEntry.Keywords = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Action
        {
            get
            {
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                {
                    return this.DialogAttachment.CurrentEntry.Action;
                }
                else
                    return null;
            }
            set
            { 
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                    this.DialogAttachment.CurrentEntry.Action = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Condition
        {
            get
            {
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                {
                    return this.DialogAttachment.CurrentEntry.Condition;
                }
                else
                    return null;
            }
            set
            { 
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                    this.DialogAttachment.CurrentEntry.Condition = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string Text
        {
            get
            {
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                {
                    return this.DialogAttachment.CurrentEntry.Text;
                }
                else
                    return null;
            }
            set
            { 
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                    this.DialogAttachment.CurrentEntry.Text = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string DependsOn
        {
            get
            {
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                {
                    return this.DialogAttachment.CurrentEntry.DependsOn;
                }
                else
                    return "-1";
            }
            set
            { 
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                    this.DialogAttachment.CurrentEntry.DependsOn = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool LockConversation
        {
            get
            {
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                {
                    return this.DialogAttachment.CurrentEntry.LockConversation;
                }
                else
                    return false;
            }
            set
            { 
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                    this.DialogAttachment.CurrentEntry.LockConversation = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public MessageType SpeechStyle
        {
            get
            {
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                {
                    return this.DialogAttachment.CurrentEntry.SpeechStyle;
                }
                else
                    return MessageType.Regular;
            }
            set
            { 
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                    this.DialogAttachment.CurrentEntry.SpeechStyle = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowNPCTrigger
        {
            get
            {
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                {
                    return this.DialogAttachment.CurrentEntry.AllowNPCTrigger;
                }
                else
                    return false;
            }
            set
            { 
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                    this.DialogAttachment.CurrentEntry.AllowNPCTrigger = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Pause
        {
            get
            {
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                {
                    return this.DialogAttachment.CurrentEntry.Pause;
                }
                else
                    return -1;
            }
            set
            { 
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                    this.DialogAttachment.CurrentEntry.Pause = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int PrePause
        {
            get
            {
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                {
                    return this.DialogAttachment.CurrentEntry.PrePause;
                }
                else
                    return -1;
            }
            set
            { 
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                    this.DialogAttachment.CurrentEntry.PrePause = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ID
        {
            get
            {
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                {
                    return this.DialogAttachment.CurrentEntry.ID;
                }
                else
                    return -1;
            }
            set
            { 
                if (this.DialogAttachment != null && this.DialogAttachment.CurrentEntry != null)
                    this.DialogAttachment.CurrentEntry.ID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int EntryNumber
        {
            get 
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.EntryNumber;
                else
                    return -1;
            }
            set 
            {
                if (this.DialogAttachment != null)
                {
                    this.DialogAttachment.EntryNumber = value;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ProximityRange
        {
            get 
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.ProximityRange;
                else
                    return -1;
            }
            set 
            {
                if (this.DialogAttachment != null)
                {
                    this.DialogAttachment.ProximityRange = value;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string ConfigFile
        {
            get 
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.ConfigFile;
                else
                    return null;
            }
            set 
            {
                if (this.DialogAttachment != null)
                {
                    this.DialogAttachment.ConfigFile = value;
                }
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
                if (value == true && this.DialogAttachment != null)
                    this.DialogAttachment.DoLoadNPC(null, this.ConfigFile);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool SaveConfig
        {
            get
            {
                return false;
            }
            set
            { 
                if (value == true && this.DialogAttachment != null) 
                    this.DialogAttachment.DoSaveNPC(null, this.ConfigFile, false);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string TriggerOnCarried
        {
            get 
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.TriggerOnCarried;
                else
                    return null;
            }
            set 
            {
                if (this.DialogAttachment != null)
                {
                    this.DialogAttachment.TriggerOnCarried = value;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string NoTriggerOnCarried
        {
            get 
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.NoTriggerOnCarried;
                else
                    return null;
            }
            set 
            {
                if (this.DialogAttachment != null)
                {
                    this.DialogAttachment.NoTriggerOnCarried = value;
                }
            }
        }
        public XmlDialog.SpeechEntry CurrentEntry
        {
            get 
            {
                if (this.DialogAttachment != null)
                    return this.DialogAttachment.CurrentEntry;
                else
                    return null;
            }
            set 
            {
                if (this.DialogAttachment != null)
                {
                    this.DialogAttachment.CurrentEntry = value;
                }
            }
        }
        public static void SendOffsetTargetEffect(IEntity target, Point3D loc, int itemID, int speed, int duration, int hue, int renderMode)
        {
            if (target is Mobile)
                ((Mobile)target).ProcessDelta();

            Effects.SendPacket(loc, target.Map, new OffsetTargetEffect(target, loc, itemID, speed, duration, hue, renderMode));
        }

        public static void Initialize()
        {
            // reestablish the DialogAttachment assignment
            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is TalkingBaseCreature)
                {
                    XmlDialog xa = XmlAttach.FindAttachment(m, typeof(XmlDialog)) as XmlDialog;
                    ((TalkingBaseCreature)m).DialogAttachment = xa;
                }
            }
        }

        public void DisplayHighlight()
        {
            if (this.EItemID > 0)
            {
                //SendOffsetTargetEffect(this, new Point3D(Location.X + EOffset.X, Location.Y + EOffset.Y, Location.Z + EOffset.Z), EItemID, 10, EDuration, EHue, 0);
                Effects.SendLocationEffect(new Point3D(this.Location.X + this.EOffset.X, this.Location.Y + this.EOffset.Y, this.Location.Z + this.EOffset.Z), this.Map, this.EItemID, this.EDuration, this.EHue, 0);

                this.lasteffect = DateTime.UtcNow;
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (this.lasteffect + TimeSpan.FromSeconds(1) < DateTime.UtcNow)
            {
                this.DisplayHighlight();
            }
        }

        public override bool Move(Direction d)
        {
            bool didmove = base.Move(d);

            this.DisplayHighlight();

            return didmove;
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            return XmlQuest.RegisterGive(from, this, item);
            //return base.OnDragDrop(from, item);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive)
            {
                if (this.TalkText != null && this.TalkText.Length > 0 && this.DialogAttachment != null)
                {
                    list.Add(new TalkEntry(this));
                }
            }

            base.GetContextMenuEntries(from, list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)7); // version

            // version 7
            writer.Write(this.m_EItemID);
            writer.Write(this.m_Duration);
            writer.Write(this.m_Offset);
            writer.Write(this.m_EHue);

            // version 6
            writer.Write(this.m_TalkText);
            // Version 5
            // all serialized data now handled by the XmlDialog attachment
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 5)
            {
                // have to add the XmlDialog attachment
                this.m_DialogAttachment = new XmlDialog((string)null);
                XmlAttach.AttachTo(this, this.m_DialogAttachment);
            }

            switch ( version )
            {
                case 7:
                    this.m_EItemID = reader.ReadInt();
                    this.m_Duration = reader.ReadInt();
                    this.m_Offset = reader.ReadPoint3D();
                    this.m_EHue = reader.ReadInt();
                    goto case 6;
                case 6:
                    this.TalkText = reader.ReadString();
                    break;
                case 5:
                    {
                        break;
                    }
                case 4:
                    {
                        int count = reader.ReadInt();

                        this.SpeechEntries = new ArrayList();
                        for (int i = 0; i < count; i++)
                        {
                            XmlDialog.SpeechEntry newentry = new XmlDialog.SpeechEntry();

                            newentry.Condition = reader.ReadString();

                            this.SpeechEntries.Add(newentry);
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
                        this.SpeechPace = reader.ReadInt();

                        int count = reader.ReadInt();
                        if (version < 4)
                        {
                            this.SpeechEntries = new ArrayList();
                        }
                        for (int i = 0; i < count; i++)
                        {
                            if (version < 4)
                            {
                                XmlDialog.SpeechEntry newentry = new XmlDialog.SpeechEntry();
    
                                newentry.PrePause = reader.ReadInt();
                                newentry.LockConversation = reader.ReadBool();
                                newentry.AllowNPCTrigger = reader.ReadBool();
                                newentry.SpeechStyle = (MessageType)reader.ReadInt();
    
                                this.SpeechEntries.Add(newentry);
                            }
                            else 
                            {
                                XmlDialog.SpeechEntry newentry = (XmlDialog.SpeechEntry)this.SpeechEntries[i];

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
                        this.ActivePlayer = reader.ReadMobile();
                        goto case 0;
                    }
                case 0:
                    {
                        this.IsActive = reader.ReadBool();
                        this.ResetTime = reader.ReadTimeSpan();
                        this.LastInteraction = reader.ReadDateTime();
                        this.AllowGhostTrig = reader.ReadBool();
                        this.ProximityRange = reader.ReadInt();
                        this.Running = reader.ReadBool();
                        this.ConfigFile = reader.ReadString();
                        int count = reader.ReadInt();
                        if (version < 2)
                        {
                            this.SpeechEntries = new ArrayList();
                        }
                        for (int i = 0; i < count; i++)
                        {
                            if (version < 2)
                            {
                                XmlDialog.SpeechEntry newentry = new XmlDialog.SpeechEntry();

                                newentry.EntryNumber = reader.ReadInt();
                                newentry.ID = reader.ReadInt();
                                newentry.Text = reader.ReadString();
                                newentry.Keywords = reader.ReadString();
                                newentry.Action = reader.ReadString();
                                newentry.DependsOn = reader.ReadInt().ToString();
                                newentry.Pause = reader.ReadInt();

                                this.SpeechEntries.Add(newentry);
                            }
                            else
                            {
                                XmlDialog.SpeechEntry newentry = (XmlDialog.SpeechEntry)this.SpeechEntries[i];

                                newentry.EntryNumber = reader.ReadInt();
                                newentry.ID = reader.ReadInt();
                                newentry.Text = reader.ReadString();
                                newentry.Keywords = reader.ReadString();
                                newentry.Action = reader.ReadString();
                                newentry.DependsOn = reader.ReadInt().ToString();
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
                            if (this.DialogAttachment != null)
                                this.DialogAttachment.DoTimer(delay, trigmob);
                        }
                        break;
                    }
            }
        }

        public sealed class OffsetTargetEffect : HuedEffect
        {
            public OffsetTargetEffect(IEntity e, Point3D loc, int itemID, int speed, int duration, int hue, int renderMode)
                : base(EffectType.FixedFrom, e.Serial, Serial.Zero, itemID, loc, loc, speed, duration, true, false, hue, renderMode)
            {
            }
        }

        private class TalkEntry : ContextMenuEntry
        {
            private readonly TalkingBaseCreature m_NPC;
            public TalkEntry(TalkingBaseCreature npc)
                : base(6146)
            {
                this.m_NPC = npc;
            }

            public override void OnClick()
            {
                Mobile from = this.Owner.From;

                if (this.m_NPC == null || this.m_NPC.Deleted || !from.CheckAlive() || this.m_NPC.DialogAttachment == null)
                    return;

                // process the talk text
                //m_NPC.DialogAttachment.ProcessSpeech(from, m_NPC.TalkText);
                from.DoSpeech(this.m_NPC.TalkText, new int[] { }, MessageType.Regular, from.SpeechHue);
            }
        }
    }
}