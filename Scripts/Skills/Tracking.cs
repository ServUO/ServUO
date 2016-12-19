using System;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Spells;
using Server.Spells.Necromancy;

namespace Server.SkillHandlers
{
    public delegate bool TrackTypeDelegate(Mobile m);

    public class Tracking
    {
        private static readonly Dictionary<Mobile, TrackingInfo> m_Table = new Dictionary<Mobile, TrackingInfo>();
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Tracking].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.SendLocalizedMessage(1011350); // What do you wish to track?

            m.CloseGump(typeof(TrackWhatGump));
            m.CloseGump(typeof(TrackWhoGump));
            m.SendGump(new TrackWhatGump(m));

            return TimeSpan.FromSeconds(10.0); // 10 second delay before beign able to re-use a skill
        }

        public static void AddInfo(Mobile tracker, Mobile target)
        {
            TrackingInfo info = new TrackingInfo(tracker, target);
            m_Table[tracker] = info;
        }

        public static double GetStalkingBonus(Mobile tracker, Mobile target)
        {
            TrackingInfo info = null;
            m_Table.TryGetValue(tracker, out info);

            if (info == null || info.m_Target != target || info.m_Map != target.Map)
                return 0.0;

            int xDelta = info.m_Location.X - target.X;
            int yDelta = info.m_Location.Y - target.Y;

            double bonus = Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));

            m_Table.Remove(tracker);	//Reset as of Pub 40, counting it as bug for Core.SE.

            if (Core.ML)
                return Math.Min(bonus, 10 + tracker.Skills.Tracking.Value / 10);

            return bonus;
        }

        public static void ClearTrackingInfo(Mobile tracker)
        {
            m_Table.Remove(tracker);
        }

        public class TrackingInfo
        {
            public Mobile m_Tracker;
            public Mobile m_Target;
            public Point2D m_Location;
            public Map m_Map;
            public TrackingInfo(Mobile tracker, Mobile target)
            {
                this.m_Tracker = tracker;
                this.m_Target = target;
                this.m_Location = new Point2D(target.X, target.Y);
                this.m_Map = target.Map;
            }
        }
    }

    public class TrackWhatGump : Gump
    {
        private readonly Mobile m_From;
        private readonly bool m_Success;
        public TrackWhatGump(Mobile from)
            : base(20, 30)
        {
            this.m_From = from;
            this.m_Success = from.CheckSkill(SkillName.Tracking, 0.0, 21.1);

            this.AddPage(0);

            this.AddBackground(0, 0, 440, 135, 5054);

            this.AddBackground(10, 10, 420, 75, 2620);
            this.AddBackground(10, 85, 420, 25, 3000);

            this.AddItem(20, 20, 9682);
            this.AddButton(20, 110, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(20, 90, 100, 20, 1018087, false, false); // Animals

            this.AddItem(120, 20, 9607);
            this.AddButton(120, 110, 4005, 4007, 2, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(120, 90, 100, 20, 1018088, false, false); // Monsters

            this.AddItem(220, 20, 8454);
            this.AddButton(220, 110, 4005, 4007, 3, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(220, 90, 100, 20, 1018089, false, false); // Human NPCs

            this.AddItem(320, 20, 8455);
            this.AddButton(320, 110, 4005, 4007, 4, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(320, 90, 100, 20, 1018090, false, false); // Players
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID >= 1 && info.ButtonID <= 4)
                TrackWhoGump.DisplayTo(this.m_Success, this.m_From, info.ButtonID - 1);
        }
    }

    public class TrackWhoGump : Gump
    {
        private static readonly TrackTypeDelegate[] m_Delegates = new TrackTypeDelegate[]
        {
            new TrackTypeDelegate(IsAnimal),
            new TrackTypeDelegate(IsMonster),
            new TrackTypeDelegate(IsHumanNPC),
            new TrackTypeDelegate(IsPlayer)
        };
        private readonly Mobile m_From;
        private readonly int m_Range;
        private readonly List<Mobile> m_List;
        private TrackWhoGump(Mobile from, List<Mobile> list, int range)
            : base(20, 30)
        {
            this.m_From = from;
            this.m_List = list;
            this.m_Range = range;

            this.AddPage(0);

            this.AddBackground(0, 0, 440, 155, 5054);

            this.AddBackground(10, 10, 420, 75, 2620);
            this.AddBackground(10, 85, 420, 45, 3000);

            if (list.Count > 4)
            {
                this.AddBackground(0, 155, 440, 155, 5054);

                this.AddBackground(10, 165, 420, 75, 2620);
                this.AddBackground(10, 240, 420, 45, 3000);

                if (list.Count > 8)
                {
                    this.AddBackground(0, 310, 440, 155, 5054);

                    this.AddBackground(10, 320, 420, 75, 2620);
                    this.AddBackground(10, 395, 420, 45, 3000);
                }
            }

            for (int i = 0; i < list.Count && i < 12; ++i)
            {
                Mobile m = list[i];

                this.AddItem(20 + ((i % 4) * 100), 20 + ((i / 4) * 155), ShrinkTable.Lookup(m));
                this.AddButton(20 + ((i % 4) * 100), 130 + ((i / 4) * 155), 4005, 4007, i + 1, GumpButtonType.Reply, 0);

                if (m.Name != null)
                    this.AddHtml(20 + ((i % 4) * 100), 90 + ((i / 4) * 155), 90, 40, m.Name, false, false);
            }
        }

        public static void DisplayTo(bool success, Mobile from, int type)
        {
            if (!success)
            {
                from.SendLocalizedMessage(1018092); // You see no evidence of those in the area.
                return;
            }

            Map map = from.Map;

            if (map == null)
                return;

            TrackTypeDelegate check = m_Delegates[type];

            from.CheckSkill(SkillName.Tracking, 21.1, 100.0); // Passive gain

            int range = 10 + (int)(from.Skills[SkillName.Tracking].Value / 10);

            List<Mobile> list = new List<Mobile>();

            foreach (Mobile m in from.GetMobilesInRange(range))
            {
                // Ghosts can no longer be tracked 
                if (m != from && (!Core.AOS || m.Alive) && (!m.Hidden || m.IsPlayer() || from.AccessLevel > m.AccessLevel) && check(m) && CheckDifficulty(from, m))
                    list.Add(m);
            }

            if (list.Count > 0)
            {
                list.Sort(new InternalSorter(from));

                from.SendGump(new TrackWhoGump(from, list, range));
                from.SendLocalizedMessage(1018093); // Select the one you would like to track.
            }
            else
            {
                if (type == 0)
                    from.SendLocalizedMessage(502991); // You see no evidence of animals in the area.
                else if (type == 1)
                    from.SendLocalizedMessage(502993); // You see no evidence of creatures in the area.
                else
                    from.SendLocalizedMessage(502995); // You see no evidence of people in the area.
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            int index = info.ButtonID - 1;

            if (index >= 0 && index < this.m_List.Count && index < 12)
            {
                Mobile m = this.m_List[index];

                this.m_From.QuestArrow = new TrackArrow(this.m_From, m, this.m_Range * 2);

                if (Core.SE)
                    Tracking.AddInfo(this.m_From, m);
            }
        }

        // Tracking players uses tracking and detect hidden vs. hiding and stealth 
        private static bool CheckDifficulty(Mobile from, Mobile m)
        {
            if (!Core.AOS || !m.Player)
                return true;

            int tracking = from.Skills[SkillName.Tracking].Fixed;	
            int detectHidden = from.Skills[SkillName.DetectHidden].Fixed;

            if (Core.ML && m.Race == Race.Elf)
                tracking /= 2; //The 'Guide' says that it requires twice as Much tracking SKILL to track an elf.  Not the total difficulty to track.

            int hiding = m.Skills[SkillName.Hiding].Fixed;
            int stealth = m.Skills[SkillName.Stealth].Fixed;
            int divisor = hiding + stealth;

            // Necromancy forms affect tracking difficulty 
            if (TransformationSpellHelper.UnderTransformation(m, typeof(HorrificBeastSpell)))
                divisor -= 200;
            else if (TransformationSpellHelper.UnderTransformation(m, typeof(VampiricEmbraceSpell)) && divisor < 500)
                divisor = 500;
            else if (TransformationSpellHelper.UnderTransformation(m, typeof(WraithFormSpell)) && divisor <= 2000)
                divisor += 200;

            int chance;
            if (divisor > 0)
            {
                if (Core.SE)
                    chance = 50 * (tracking * 2 + detectHidden) / divisor;
                else
                    chance = 50 * (tracking + detectHidden + 10 * Utility.RandomMinMax(1, 20)) / divisor;
            }
            else
                chance = 100;

            return chance > Utility.Random(100);
        }

        private static bool IsAnimal(Mobile m)
        {
            return (!m.Player && m.Body.IsAnimal);
        }

        private static bool IsMonster(Mobile m)
        {
            return (!m.Player && m.Body.IsMonster);
        }

        private static bool IsHumanNPC(Mobile m)
        {
            return (!m.Player && m.Body.IsHuman);
        }

        private static bool IsPlayer(Mobile m)
        {
            return m.Player;
        }

        private class InternalSorter : IComparer<Mobile>
        {
            private readonly Mobile m_From;
            public InternalSorter(Mobile from)
            {
                this.m_From = from;
            }

            public int Compare(Mobile x, Mobile y)
            {
                if (x == null && y == null)
                    return 0;
                else if (x == null)
                    return -1;
                else if (y == null)
                    return 1;

                return this.m_From.GetDistanceToSqrt(x).CompareTo(this.m_From.GetDistanceToSqrt(y));
            }
        }
    }

    public class TrackArrow : QuestArrow
    {
        private readonly Timer m_Timer;
        private Mobile m_From;
        public TrackArrow(Mobile from, IEntity target, int range)
            : base(from, target)
        {
            this.m_From = from;
            this.m_Timer = new TrackTimer(from, target, range, this);
            this.m_Timer.Start();
        }

        public override void OnClick(bool rightClick)
        {
            if (rightClick)
            {
                Tracking.ClearTrackingInfo(this.m_From);

                this.m_From = null;

                this.Stop();
            }
        }

        public override void OnStop()
        {
            this.m_Timer.Stop();

            if (this.m_From != null)
            {
                Tracking.ClearTrackingInfo(this.m_From);

                this.m_From.SendLocalizedMessage(503177); // You have lost your quarry.
            }
        }
    }

    public class TrackTimer : Timer
    {
        private readonly Mobile m_From;
        private readonly IEntity m_Target;
        private readonly int m_Range;
        private readonly QuestArrow m_Arrow;
        private int m_LastX, m_LastY;
        public TrackTimer(Mobile from, IEntity target, int range, QuestArrow arrow)
            : base(TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(2.5))
        {
            this.m_From = from;
            this.m_Target = target;
            this.m_Range = range;

            this.m_Arrow = arrow;
        }

        protected override void OnTick()
        {
            if (!this.m_Arrow.Running)
            {
                this.Stop();
                return;
            }
            else if (this.m_From.NetState == null || this.m_From.Deleted || this.m_Target.Deleted || this.m_From.Map != this.m_Target.Map || !this.m_From.InRange(this.m_Target, this.m_Range) || this.m_Target is Mobile && (((Mobile)this.m_Target).Hidden && ((Mobile)this.m_Target).AccessLevel > this.m_From.AccessLevel))
            {
                this.m_Arrow.Stop();
                this.Stop();
                return;
            }

            if (this.m_LastX != this.m_Target.Location.X || this.m_LastY != this.m_Target.Location.Y)
            {
                this.m_LastX = this.m_Target.Location.X;
                this.m_LastY = this.m_Target.Location.Y;

                this.m_Arrow.Update();
            }
        }
    }
}