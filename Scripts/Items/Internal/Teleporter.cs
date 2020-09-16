#region References
using Server.Engines.CityLoyalty;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Regions;
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Server.Items
{
    public interface ITeleporter
    {
        void DoTeleport(Mobile m);
    }

    public class Teleporter : Item, ITeleporter
    {
        private bool m_Active, m_Creatures, m_CombatCheck, m_CriminalCheck;
        private Point3D m_PointDest;
        private Map m_MapDest;
        private bool m_SourceEffect;
        private bool m_DestEffect;
        private int m_SoundID;
        private TimeSpan m_Delay;

        [Constructable]
        public Teleporter()
            : this(new Point3D(0, 0, 0), null, false)
        { }

        [Constructable]
        public Teleporter(Point3D pointDest, Map mapDest)
            : this(pointDest, mapDest, false)
        { }

        [Constructable]
        public Teleporter(Point3D pointDest, Map mapDest, bool creatures)
            : base(0x1BC3)
        {
            Movable = false;
            Visible = false;

            m_Active = true;
            m_PointDest = pointDest;
            m_MapDest = mapDest;
            m_Creatures = creatures;

            m_CombatCheck = false;
            m_CriminalCheck = false;
        }

        public Teleporter(Serial serial)
            : base(serial)
        { }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SourceEffect
        {
            get { return m_SourceEffect; }
            set
            {
                m_SourceEffect = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DestEffect
        {
            get { return m_DestEffect; }
            set
            {
                m_DestEffect = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SoundID
        {
            get { return m_SoundID; }
            set
            {
                m_SoundID = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Delay
        {
            get { return m_Delay; }
            set
            {
                m_Delay = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return m_Active; }
            set
            {
                m_Active = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D PointDest
        {
            get { return m_PointDest; }
            set
            {
                m_PointDest = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map MapDest
        {
            get { return m_MapDest; }
            set
            {
                m_MapDest = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Creatures
        {
            get { return m_Creatures; }
            set
            {
                m_Creatures = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CombatCheck
        {
            get { return m_CombatCheck; }
            set
            {
                m_CombatCheck = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CriminalCheck
        {
            get { return m_CriminalCheck; }
            set
            {
                m_CriminalCheck = value;
                InvalidateProperties();
            }
        }

        public override int LabelNumber => 1026095;  // teleporter
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Active)
            {
                list.Add(1060742); // active
            }
            else
            {
                list.Add(1060743); // inactive
            }

            if (m_MapDest != null)
            {
                list.Add(1060658, "Map\t{0}", m_MapDest);
            }

            if (m_PointDest != Point3D.Zero)
            {
                list.Add(1060659, "Coords\t{0}", m_PointDest);
            }

            list.Add(1060660, "Creatures\t{0}", m_Creatures ? "Yes" : "No");
        }

        public virtual bool CanTeleport(Mobile m)
        {
            if (!m_Active)
            {
                return false;
            }

            if (!m_Creatures && !m.Player)
            {
                return false;
            }

            if (m.Holding != null)
            {
                m.SendLocalizedMessage(1071955); // You cannot teleport while dragging an object.
                return false;
            }

            if (m_CriminalCheck && m.Criminal)
            {
                m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                return false;
            }

            if (m_CombatCheck && SpellHelper.CheckCombat(m))
            {
                m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }

            if (!CheckDestination(m) || (Siege.SiegeShard && m_MapDest == Map.Trammel))
            {
                return false;
            }

            if (CityTradeSystem.HasTrade(m))
            {
                m.SendLocalizedMessage(1151733); // You cannot do that while carrying a Trade Order.
                return false;
            }

            return true;
        }

        private bool CheckDestination(Mobile m)
        {
            Map map = m_MapDest;

            if (map == null || map == Map.Internal)
            {
                map = Map;
            }

            Region myRegion = Region.Find(m.Location, m.Map);
            Region toRegion = Region.Find(m_PointDest, map);

            if (myRegion != toRegion)
            {
                return toRegion.OnMoveInto(m, m.Direction, m_PointDest, m.Location);
            }

            return true;
        }

        public virtual void StartTeleport(Mobile m)
        {
            if (!m.CanBeginAction(typeof(Teleporter)))
            {
                return;
            }

            if (!m_Active || !CanTeleport(m))
            {
                return;
            }

            DelayedTeleport(m);
        }

        private void DelayedTeleport(Mobile m)
        {
            m.BeginAction(typeof(Teleporter));

            m.Frozen = true;

            Timer.DelayCall(m_Delay > TeleportRegion.Delay ? m_Delay : TeleportRegion.Delay, DelayedTeleportCallback, m);
        }

        private void DelayedTeleportCallback(Mobile m)
        {
            Timer.DelayCall(TimeSpan.FromMilliseconds(250), () => m.EndAction(typeof(Teleporter)));

            m.Frozen = false;
            DoTeleport(m);
        }

        public virtual void DoTeleport(Mobile m)
        {
            Map map = m_MapDest;

            if (map == null || map == Map.Internal)
            {
                map = Map;
            }

            Point3D p = m_PointDest;

            if (p == Point3D.Zero)
            {
                p = m.Location;
            }

            bool sendEffect = (!m.Hidden || m.IsPlayer());

            if (m_SourceEffect && sendEffect)
            {
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
            }

            BaseCreature.TeleportPets(m, p, map);
            m.MoveToWorld(p, map);                     

            if (m_DestEffect && sendEffect)
            {
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);
            }

            if (m_SoundID > 0 && sendEffect)
            {
                Effects.PlaySound(m.Location, m.Map, m_SoundID);
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            StartTeleport(m);

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(4); // version

            writer.Write(m_CriminalCheck);
            writer.Write(m_CombatCheck);

            writer.Write(m_SourceEffect);
            writer.Write(m_DestEffect);
            writer.Write(m_Delay);
            writer.WriteEncodedInt(m_SoundID);

            writer.Write(m_Creatures);

            writer.Write(m_Active);
            writer.Write(m_PointDest);
            writer.Write(m_MapDest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 4:
                    {
                        m_CriminalCheck = reader.ReadBool();
                        goto case 3;
                    }
                case 3:
                    {
                        m_CombatCheck = reader.ReadBool();
                        goto case 2;
                    }
                case 2:
                    {
                        m_SourceEffect = reader.ReadBool();
                        m_DestEffect = reader.ReadBool();
                        m_Delay = reader.ReadTimeSpan();
                        m_SoundID = reader.ReadEncodedInt();

                        goto case 1;
                    }
                case 1:
                    {
                        m_Creatures = reader.ReadBool();

                        goto case 0;
                    }
                case 0:
                    {
                        m_Active = reader.ReadBool();
                        m_PointDest = reader.ReadPoint3D();
                        m_MapDest = reader.ReadMap();

                        break;
                    }
            }
        }
    }

    public class SkillTeleporter : Teleporter
    {
        private SkillName m_Skill;
        private double m_Required;
        private string m_MessageString;
        private int m_MessageNumber;

        [Constructable]
        public SkillTeleporter()
        { }

        public SkillTeleporter(Serial serial)
            : base(serial)
        { }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get { return m_Skill; }
            set
            {
                m_Skill = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public double Required
        {
            get { return m_Required; }
            set
            {
                m_Required = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string MessageString
        {
            get { return m_MessageString; }
            set
            {
                m_MessageString = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MessageNumber
        {
            get { return m_MessageNumber; }
            set
            {
                m_MessageNumber = value;
                InvalidateProperties();
            }
        }

        public override bool CanTeleport(Mobile m)
        {
            if (!base.CanTeleport(m))
            {
                return false;
            }

            Skill sk = m.Skills[m_Skill];

            if (sk == null || sk.Base < m_Required)
            {
                if (m.BeginAction(this))
                {
                    if (m_MessageString != null)
                    {
                        m.Send(new UnicodeMessage(Serial, ItemID, MessageType.Regular, 0x3B2, 3, "ENU", null, m_MessageString));
                    }
                    else if (m_MessageNumber != 0)
                    {
                        m.Send(new MessageLocalized(Serial, ItemID, MessageType.Regular, 0x3B2, 3, m_MessageNumber, null, ""));
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerStateCallback(EndMessageLock), m);
                }

                return false;
            }

            return true;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int skillIndex = (int)m_Skill;
            string skillName;

            if (skillIndex >= 0 && skillIndex < SkillInfo.Table.Length)
            {
                skillName = SkillInfo.Table[skillIndex].Name;
            }
            else
            {
                skillName = "(Invalid)";
            }

            list.Add(1060661, "{0}\t{1:F1}", skillName, m_Required);

            if (m_MessageString != null)
            {
                list.Add(1060662, "Message\t{0}", m_MessageString);
            }
            else if (m_MessageNumber != 0)
            {
                list.Add(1060662, "Message\t#{0}", m_MessageNumber);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write((int)m_Skill);
            writer.Write(m_Required);
            writer.Write(m_MessageString);
            writer.Write(m_MessageNumber);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Skill = (SkillName)reader.ReadInt();
                        m_Required = reader.ReadDouble();
                        m_MessageString = reader.ReadString();
                        m_MessageNumber = reader.ReadInt();

                        break;
                    }
            }
        }

        private void EndMessageLock(object state)
        {
            ((Mobile)state).EndAction(this);
        }
    }

    public class KeywordTeleporter : Teleporter
    {
        private string m_Substring;
        private int m_Keyword;
        private int m_Range;

        [Constructable]
        public KeywordTeleporter()
        {
            m_Keyword = -1;
            m_Substring = null;
        }

        public KeywordTeleporter(Serial serial)
            : base(serial)
        { }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Substring
        {
            get { return m_Substring; }
            set
            {
                m_Substring = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Keyword
        {
            get { return m_Keyword; }
            set
            {
                m_Keyword = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Range
        {
            get { return m_Range; }
            set
            {
                m_Range = value;
                InvalidateProperties();
            }
        }

        public override bool HandlesOnSpeech => true;

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (!e.Handled && Active)
            {
                Mobile m = e.Mobile;

                if (!m.InRange(GetWorldLocation(), m_Range) || CityTradeSystem.HasTrade(m))
                {
                    return;
                }

                bool isMatch = false;

                if (m_Keyword >= 0 && e.HasKeyword(m_Keyword))
                {
                    isMatch = true;
                }
                else if (m_Substring != null && e.Speech.ToLower().IndexOf(m_Substring.ToLower()) >= 0)
                {
                    isMatch = true;
                }

                if (!isMatch || !CanTeleport(m))
                {
                    return;
                }

                e.Handled = true;
                StartTeleport(m);
            }
        }

        public override void DoTeleport(Mobile m)
        {
            if (!m.InRange(GetWorldLocation(), m_Range) || m.Map != Map)
            {
                return;
            }

            base.DoTeleport(m);
        }

        public override bool OnMoveOver(Mobile m)
        {
            return true;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060661, "Range\t{0}", m_Range);

            if (m_Keyword >= 0)
            {
                list.Add(1060662, "Keyword\t{0}", m_Keyword);
            }

            if (m_Substring != null)
            {
                list.Add(1060663, "Substring\t{0}", m_Substring);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Substring);
            writer.Write(m_Keyword);
            writer.Write(m_Range);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Substring = reader.ReadString();
                        m_Keyword = reader.ReadInt();
                        m_Range = reader.ReadInt();

                        break;
                    }
            }
        }
    }

    public class WaitTeleporter : KeywordTeleporter
    {
        private static Dictionary<Mobile, TeleportingInfo> m_Table;
        private int m_StartNumber;
        private string m_StartMessage;
        private int m_ProgressNumber;
        private string m_ProgressMessage;
        private bool m_ShowTimeRemaining;

        [Constructable]
        public WaitTeleporter()
        { }

        public WaitTeleporter(Serial serial)
            : base(serial)
        { }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StartNumber { get { return m_StartNumber; } set { m_StartNumber = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string StartMessage { get { return m_StartMessage; } set { m_StartMessage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ProgressNumber { get { return m_ProgressNumber; } set { m_ProgressNumber = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string ProgressMessage { get { return m_ProgressMessage; } set { m_ProgressMessage = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowTimeRemaining { get { return m_ShowTimeRemaining; } set { m_ShowTimeRemaining = value; } }

        public static void Initialize()
        {
            m_Table = new Dictionary<Mobile, TeleportingInfo>();

            EventSink.Logout += EventSink_Logout;
        }

        public static void EventSink_Logout(LogoutEventArgs e)
        {
            Mobile from = e.Mobile;
            TeleportingInfo info;

            if (from == null || !m_Table.TryGetValue(from, out info))
            {
                return;
            }

            info.Timer.Stop();
            m_Table.Remove(from);
        }

        public static string FormatTime(TimeSpan ts)
        {
            if (ts.TotalHours >= 1)
            {
                int h = (int)Math.Round(ts.TotalHours);
                return string.Format("{0} hour{1}", h, (h == 1) ? "" : "s");
            }
            else if (ts.TotalMinutes >= 1)
            {
                int m = (int)Math.Round(ts.TotalMinutes);
                return string.Format("{0} minute{1}", m, (m == 1) ? "" : "s");
            }

            int s = Math.Max((int)Math.Round(ts.TotalSeconds), 0);
            return string.Format("{0} second{1}", s, (s == 1) ? "" : "s");
        }

        public override void StartTeleport(Mobile m)
        {
            TeleportingInfo info;

            if (m_Table.TryGetValue(m, out info))
            {
                if (info.Teleporter == this)
                {
                    if (m.BeginAction(this))
                    {
                        if (m_ProgressMessage != null)
                        {
                            m.SendMessage(m_ProgressMessage);
                        }
                        else if (m_ProgressNumber != 0)
                        {
                            m.SendLocalizedMessage(m_ProgressNumber);
                        }

                        if (m_ShowTimeRemaining)
                        {
                            m.SendMessage("Time remaining: {0}", FormatTime(m_Table[m].Timer.Next - DateTime.UtcNow));
                        }

                        Timer.DelayCall(TimeSpan.FromSeconds(5), EndLock, m);
                    }

                    return;
                }
                else
                {
                    info.Timer.Stop();
                }
            }

            if (m_StartMessage != null)
            {
                m.SendMessage(m_StartMessage);
            }
            else if (m_StartNumber != 0)
            {
                m.SendLocalizedMessage(m_StartNumber);
            }

            if (Delay == TimeSpan.Zero)
            {
                DoTeleport(m);
            }
            else
            {
                m_Table[m] = new TeleportingInfo(this, Timer.DelayCall(Delay, DoTeleport, m));
            }
        }

        public override void DoTeleport(Mobile m)
        {
            m_Table.Remove(m);

            base.DoTeleport(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_StartNumber);
            writer.Write(m_StartMessage);
            writer.Write(m_ProgressNumber);
            writer.Write(m_ProgressMessage);
            writer.Write(m_ShowTimeRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_StartNumber = reader.ReadInt();
            m_StartMessage = reader.ReadString();
            m_ProgressNumber = reader.ReadInt();
            m_ProgressMessage = reader.ReadString();
            m_ShowTimeRemaining = reader.ReadBool();
        }

        private void EndLock(Mobile m)
        {
            m.EndAction(this);
        }

        private class TeleportingInfo
        {
            private readonly WaitTeleporter m_Teleporter;
            private readonly Timer m_Timer;

            public TeleportingInfo(WaitTeleporter tele, Timer t)
            {
                m_Teleporter = tele;
                m_Timer = t;
            }

            public WaitTeleporter Teleporter => m_Teleporter;
            public Timer Timer => m_Timer;
        }
    }

    public class TimeoutTeleporter : Teleporter
    {
        private TimeSpan m_TimeoutDelay;
        private Dictionary<Mobile, Timer> m_Teleporting;

        [Constructable]
        public TimeoutTeleporter()
            : this(new Point3D(0, 0, 0), null, false)
        { }

        [Constructable]
        public TimeoutTeleporter(Point3D pointDest, Map mapDest)
            : this(pointDest, mapDest, false)
        { }

        [Constructable]
        public TimeoutTeleporter(Point3D pointDest, Map mapDest, bool creatures)
            : base(pointDest, mapDest, creatures)
        {
            m_Teleporting = new Dictionary<Mobile, Timer>();
        }

        public TimeoutTeleporter(Serial serial)
            : base(serial)
        { }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan TimeoutDelay { get { return m_TimeoutDelay; } set { m_TimeoutDelay = value; } }

        public void StartTimer(Mobile m)
        {
            StartTimer(m, m_TimeoutDelay);
        }

        public void StopTimer(Mobile m)
        {
            Timer t;

            if (m_Teleporting.TryGetValue(m, out t))
            {
                t.Stop();
                m_Teleporting.Remove(m);
            }
        }

        public override void DoTeleport(Mobile m)
        {
            m_Teleporting.Remove(m);

            base.DoTeleport(m);
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (Active)
            {
                if (!CanTeleport(m))
                {
                    return false;
                }

                StartTimer(m);
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_TimeoutDelay);
            writer.Write(m_Teleporting.Count);

            foreach (KeyValuePair<Mobile, Timer> kvp in m_Teleporting)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.Next);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_TimeoutDelay = reader.ReadTimeSpan();
            m_Teleporting = new Dictionary<Mobile, Timer>();

            int count = reader.ReadInt();

            for (int i = 0; i < count; ++i)
            {
                Mobile m = reader.ReadMobile();
                DateTime end = reader.ReadDateTime();

                StartTimer(m, end - DateTime.UtcNow);
            }
        }

        private void StartTimer(Mobile m, TimeSpan delay)
        {
            Timer t;

            if (m_Teleporting.TryGetValue(m, out t))
            {
                t.Stop();
            }

            m_Teleporting[m] = Timer.DelayCall(delay, StartTeleport, m);
        }
    }

    public class TimeoutGoal : Item
    {
        private TimeoutTeleporter m_Teleporter;

        [Constructable]
        public TimeoutGoal()
            : base(0x1822)
        {
            Movable = false;
            Visible = false;

            Hue = 1154;
        }

        public TimeoutGoal(Serial serial)
            : base(serial)
        { }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeoutTeleporter Teleporter { get { return m_Teleporter; } set { m_Teleporter = value; } }

        public override string DefaultName => "timeout teleporter goal";

        public override bool OnMoveOver(Mobile m)
        {
            if (m_Teleporter != null)
            {
                m_Teleporter.StopTimer(m);
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.WriteItem(m_Teleporter);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Teleporter = reader.ReadItem<TimeoutTeleporter>();
        }
    }

    public class ConditionTeleporter : Teleporter
    {
        private ConditionFlag m_Flags;

        [Constructable]
        public ConditionTeleporter()
        { }

        public ConditionTeleporter(Serial serial)
            : base(serial)
        { }

        [Flags]
        protected enum ConditionFlag
        {
            None = 0x00,
            DenyMounted = 0x01,
            DenyFollowers = 0x02,
            DenyPackContents = 0x04,
            DenyHolding = 0x08,
            DenyEquipment = 0x10,
            DenyTransformed = 0x20,
            StaffOnly = 0x40,
            DenyPackEthereals = 0x080,
            DeadOnly = 0x100
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ClilocNumber
        {
            get;
            set;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DisableMessage
        {
            get;
            set;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DenyMounted
        {
            get { return GetFlag(ConditionFlag.DenyMounted); }
            set
            {
                SetFlag(ConditionFlag.DenyMounted, value);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DenyFollowers
        {
            get { return GetFlag(ConditionFlag.DenyFollowers); }
            set
            {
                SetFlag(ConditionFlag.DenyFollowers, value);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DenyPackContents
        {
            get { return GetFlag(ConditionFlag.DenyPackContents); }
            set
            {
                SetFlag(ConditionFlag.DenyPackContents, value);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DenyHolding
        {
            get { return GetFlag(ConditionFlag.DenyHolding); }
            set
            {
                SetFlag(ConditionFlag.DenyHolding, value);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DenyEquipment
        {
            get { return GetFlag(ConditionFlag.DenyEquipment); }
            set
            {
                SetFlag(ConditionFlag.DenyEquipment, value);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DenyTransformed
        {
            get { return GetFlag(ConditionFlag.DenyTransformed); }
            set
            {
                SetFlag(ConditionFlag.DenyTransformed, value);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool StaffOnly
        {
            get { return GetFlag(ConditionFlag.StaffOnly); }
            set
            {
                SetFlag(ConditionFlag.StaffOnly, value);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DenyPackEthereals
        {
            get { return GetFlag(ConditionFlag.DenyPackEthereals); }
            set
            {
                SetFlag(ConditionFlag.DenyPackEthereals, value);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool DeadOnly
        {
            get { return GetFlag(ConditionFlag.DeadOnly); }
            set
            {
                SetFlag(ConditionFlag.DeadOnly, value);
                InvalidateProperties();
            }
        }

        public override bool CanTeleport(Mobile m)
        {
            if (!base.CanTeleport(m))
            {
                return false;
            }

            if (GetFlag(ConditionFlag.StaffOnly) && m.IsPlayer())
            {
                return false;
            }

            if (GetFlag(ConditionFlag.DenyMounted) && m.Mounted)
            {
                m.SendLocalizedMessage(1077252); // You must dismount before proceeding.
                return false;
            }

            if (GetFlag(ConditionFlag.DenyFollowers) &&
                (m.Followers != 0 || (m is PlayerMobile && ((PlayerMobile)m).AutoStabled.Count != 0)))
            {
                m.SendLocalizedMessage(1077250); // No pets permitted beyond this point.
                return false;
            }

            Container pack = m.Backpack;

            if (pack != null)
            {
                if (GetFlag(ConditionFlag.DenyPackContents) && pack.TotalItems != 0)
                {
                    if (!DisableMessage)
                        m.SendMessage("You must empty your backpack before proceeding.");
                    return false;
                }

                if (GetFlag(ConditionFlag.DenyPackEthereals) &&
                    (pack.FindItemByType(typeof(EtherealMount)) != null || pack.FindItemByType(typeof(BaseImprisonedMobile)) != null))
                {
                    if (!DisableMessage)
                        m.SendMessage("You must empty your backpack of ethereal mounts before proceeding.");
                    return false;
                }
            }

            if (GetFlag(ConditionFlag.DenyHolding) && m.Holding != null)
            {
                if (!DisableMessage)
                    m.SendMessage("You must let go of what you are holding before proceeding.");
                return false;
            }

            if (GetFlag(ConditionFlag.DenyEquipment))
            {
                foreach (Item item in m.Items)
                {
                    switch (item.Layer)
                    {
                        case Layer.Hair:
                        case Layer.FacialHair:
                        case Layer.Backpack:
                        case Layer.Mount:
                        case Layer.Bank:
                            {
                                continue; // ignore
                            }
                        default:
                            {
                                if (!DisableMessage)
                                    m.SendMessage("You must remove all of your equipment before proceeding.");
                                return false;
                            }
                    }
                }
            }

            if (GetFlag(ConditionFlag.DenyTransformed) && m.IsBodyMod)
            {
                if (!DisableMessage)
                    m.SendMessage("You cannot go there in this form.");
                return false;
            }

            if (GetFlag(ConditionFlag.DeadOnly) && m.Alive)
            {
                if (!DisableMessage)
                    m.SendLocalizedMessage(1060014); // Only the dead may pass.
                return false;
            }

            if (!DisableMessage && ClilocNumber != 0)
            {
                m.SendLocalizedMessage(ClilocNumber);
            }

            return true;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            StringBuilder props = new StringBuilder();

            if (GetFlag(ConditionFlag.DenyMounted))
            {
                props.Append("<BR>Deny Mounted");
            }

            if (GetFlag(ConditionFlag.DenyFollowers))
            {
                props.Append("<BR>Deny Followers");
            }

            if (GetFlag(ConditionFlag.DenyPackContents))
            {
                props.Append("<BR>Deny Pack Contents");
            }

            if (GetFlag(ConditionFlag.DenyPackEthereals))
            {
                props.Append("<BR>Deny Pack Ethereals");
            }

            if (GetFlag(ConditionFlag.DenyHolding))
            {
                props.Append("<BR>Deny Holding");
            }

            if (GetFlag(ConditionFlag.DenyEquipment))
            {
                props.Append("<BR>Deny Equipment");
            }

            if (GetFlag(ConditionFlag.DenyTransformed))
            {
                props.Append("<BR>Deny Transformed");
            }

            if (GetFlag(ConditionFlag.StaffOnly))
            {
                props.Append("<BR>Staff Only");
            }

            if (GetFlag(ConditionFlag.DeadOnly))
            {
                props.Append("<BR>Dead Only");
            }

            if (props.Length != 0)
            {
                props.Remove(0, 4);
                list.Add(props.ToString());
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version

            writer.Write(ClilocNumber);
            writer.Write(DisableMessage);
            writer.Write((int)m_Flags);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                    ClilocNumber = reader.ReadInt();
                    goto case 1;
                case 1:
                    DisableMessage = reader.ReadBool();
                    goto case 0;
                case 0:
                    m_Flags = (ConditionFlag)reader.ReadInt();
                    break;
            }
        }

        protected bool GetFlag(ConditionFlag flag)
        {
            return ((m_Flags & flag) != 0);
        }

        protected void SetFlag(ConditionFlag flag, bool value)
        {
            if (value)
            {
                m_Flags |= flag;
            }
            else
            {
                m_Flags &= ~flag;
            }
        }
    }

    public class ClickTeleporter : Teleporter
    {
        [Constructable]
        public ClickTeleporter(int itemID)
            : this(itemID, new Point3D(0, 0, 0), null)
        { }

        public ClickTeleporter(int itemID, Point3D pointDest, Map mapDest)
            : base(pointDest, mapDest)
        {
            ItemID = itemID;
            Movable = false;
            Visible = true;
            Weight = 0;

            Active = true;
            PointDest = pointDest;
            MapDest = mapDest;
        }

        public ClickTeleporter(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(Location, 3) || !from.InLOS(this) || !from.CanSee(this))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else
            {
                OnMoveOver(from);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version           
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
