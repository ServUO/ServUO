using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Spells.Fourth;
using Server.Targeting;

namespace Server
{
    public partial class Ability
    {
        #region SimpleFlame
        public static void SimpleFlame(Mobile from, Mobile target, int damage)
        {
            SimpleFlame(from, target, damage, false);
        }

        public static void SimpleFlame(Mobile from, Mobile target, int damage, bool skip)
        {
            if (!CanUse(from, target))
                return;

            if (!skip)
            {
                from.Say("*Ul Flam*");

                Effects.SendLocationParticles(
                    EffectItem.Create(new Point3D(from.X - 1, from.Y - 1, from.Z), from.Map, EffectItem.DefaultDuration),
                    0x3709, 10, 30, 0, 4, 0, 0);
                Effects.SendLocationParticles(
                    EffectItem.Create(new Point3D(from.X - 1, from.Y + 1, from.Z), from.Map, EffectItem.DefaultDuration),
                    0x3709, 10, 30, 0, 4, 0, 0);
                Effects.SendLocationParticles(
                    EffectItem.Create(new Point3D(from.X + 1, from.Y - 1, from.Z), from.Map, EffectItem.DefaultDuration),
                    0x3709, 10, 30, 0, 4, 0, 0);
                Effects.SendLocationParticles(
                    EffectItem.Create(new Point3D(from.X + 1, from.Y + 1, from.Z), from.Map, EffectItem.DefaultDuration),
                    0x3709, 10, 30, 0, 4, 0, 0);
            }

            new SimpleFlameTimer(from, target).Start();
        }

        public class SimpleFlameTimer : Timer
        {
            private Mobile m_From;
            private Mobile m_Target;
            //private int m_Damage;
            private int m_Count;
                        
            private Point3D Point;                       

            public SimpleFlameTimer(Mobile from, Mobile target)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                m_From = from;
                m_Target = target;
                m_Count = 0;
                Point = m_From.Location; 
            }

            protected override void OnTick()
            {
                if (m_From == null || m_From.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_Count == 0)
                {
                    for (int i = -2; i < 3; i++)
                        for (int j = -2; j < 5; j++)
                            if ((i == -2 || i == 2) || (j == -2 || j == 2))
                                Effects.SendMovingParticles(
                                    new Entity(Serial.Zero, new Point3D(m_From.X + i, m_From.Y + j, m_From.Z + 14), m_From.Map),
                                    m_Target, 0x46E9, 2, 0, false, false, 0, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                            else
                                continue;
                }
                else
                { // It looked like it delt 67 damage, presuming 70% fire res thats about 223 damage delt before resistance.                                          
                    AOS.Damage(m_Target, m_From, Utility.RandomMinMax(210, 230), 0, 100, 0, 0, 0); 

                    Stop();   
                }

                m_Count++;

                Effects.PlaySound(Point, m_From.Map, 0x160); 
            }
        }
        #endregion

        #region SoulDrain
        // since the video shows living people nearby only the players present at the time of casting are effected.
        public static void SoulDrain(Mobile from)
        {
            from.Say("*Vas Grav Hur*");

            List<PlayerMobile> list = new List<PlayerMobile>();
            IPooledEnumerable eable = from.GetMobilesInRange(8);

            foreach (Mobile m in eable)
                if (m != null)
                    if (m is PlayerMobile)
                        list.Add((PlayerMobile)m);

            eable.Free();

            new SoulDrainTimer(from, list).Start();
        }
		
        public class SoulDrainTimer : Timer
        {
            private Mobile m_From;
            //private int m_Damage;
            private List<PlayerMobile> m_List;
            private int m_Count;

            public SoulDrainTimer(Mobile from, List<PlayerMobile> list)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_From = from;
                //m_Damage = damage;
                m_List = list;
                m_Count = 0;
            }

            protected override void OnTick()
            {
                if (m_From == null || m_From.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_Count == 0)
                    for (int i = 0; i < m_List.Count; i++)
                    { 
                        m_List[i].Frozen = true;
                        m_List[i].Kill();
                    }
                else if (m_Count < 10)
                {
                    for (int i = 0; i < m_List.Count; i++)
                    {
                        if (m_Count == 1)
                            m_List[i].SendMessage("Unnatural forces hold you free from the ground and swirl around you!"); //TODO find cliloc.

                        // Prevent them from resing during this trick.
                        if (m_List[i].Alive)
                            m_List[i].Kill();

                        m_List[i].Z++;
                        int effects = Utility.RandomMinMax(3, 5) + 1;
                        int x = 0, y = 0;

                        for (int j = 0; j < effects; j++)
                        {
                            x = Utility.RandomMinMax(-1, 2);
                            y = Utility.RandomMinMax(-1, 2);

                            //TODO Match the look
                            Effects.SendMovingParticles(
                                new Entity(Serial.Zero, new Point3D(m_List[i].X + x, m_List[i].Y + y, m_List[i].Z - 5), m_List[i].Map),
                                new Entity(Serial.Zero, new Point3D(m_List[i].X + x, m_List[i].Y + y, m_List[i].Z + 60), m_List[i].Map),
                                0x378A + Utility.Random(19)/*ItemID*/, 10, 0, false, false, 0, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < m_List.Count; i++)
                    {
                        m_List[i].Z -= 0;
                        m_List[i].Frozen = false;
                    }
                    Stop();
                }

                m_Count++;
            }
        }
        #endregion

        #region FlameWave
        public static void FlameWave(Mobile from)
        {
            if (!CanUse(from))
                return;

            from.Say("*Vas Grav Consume !*");

            new FlameWaveTimer(from).Start();
        }

        internal class FlameWaveTimer : Timer
        {
            private Mobile m_From;
            private Point3D m_StartingLocation;
            private Map m_Map;
            private int m_Count;
            private Point3D m_Point;

            public FlameWaveTimer(Mobile from)
                : base(TimeSpan.FromMilliseconds(300.0), TimeSpan.FromMilliseconds(300.0))
            {
                m_From = from;
                m_StartingLocation = from.Location;
                m_Map = from.Map;
                m_Count = 0;
                m_Point = new Point3D();
                SetupDamage(from);
            }

            protected override void OnTick()
            {
                if (m_From == null || m_From.Deleted)
                {
                    Stop();
                    return;
                }
 
                double dist = 0.0;

                for (int i = -m_Count; i < m_Count + 1; i++)
                {
                    for (int j = -m_Count; j < m_Count + 1; j++)
                    {
                        m_Point.X = m_StartingLocation.X + i;
                        m_Point.Y = m_StartingLocation.Y + j;
                        m_Point.Z = m_Map.GetAverageZ(m_Point.X, m_Point.Y);
                        dist = GetDist(m_StartingLocation, m_Point);
                        if (dist < ((double)m_Count + 0.1) && dist > ((double)m_Count - 3.1))
                        {
                            Effects.SendLocationParticles(EffectItem.Create(m_Point, m_Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                        }
                    }
                }

                m_Count += 3;

                if (m_Count > 15)
                    Stop();
            }

            private void SetupDamage(Mobile from)
            {
                IPooledEnumerable eable = from.GetMobilesInRange(10);
                foreach (Mobile m in eable)
                {
                    if (CanTarget(from, m, true, false, false))
                    {
                        Timer.DelayCall(TimeSpan.FromMilliseconds(300 * (GetDist(m_StartingLocation, m.Location) / 3)), new TimerStateCallback(Hurt), m);
                    }
                }

                eable.Free();
            }

            public void Hurt(object o)
            {
                Mobile m = o as Mobile;

                if (m_From == null || m == null || m.Deleted)
                    return;

                int damage = m_From.Hits / 4;

                if (damage > 200)
                    damage = 400;  

                AOS.Damage(m, m_From, damage, 0, 100, 0, 0, 0);
                m.SendMessage("You are being burnt alive by the seering heat!");
            }

            private double GetDist(Point3D start, Point3D end)
            {
                int xdiff = start.X - end.X;
                int ydiff = start.Y - end.Y;
                return Math.Sqrt((xdiff * xdiff) + (ydiff * ydiff));
            }
        }
        #endregion

        #region FlameCross
        public static void FlameCross(Mobile from)
        {
            if (!CanUse(from))
                return;

            Point3D point = from.Location;
            Direction d = Direction.North;
            int itemid = 0x3996;
            Map map = from.Map;
                     
            for (int i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 1:
                        {
                            d = Direction.Right;
                            itemid = 0;
                        }
                        break;
                    case 2:
                        {
                            d = Direction.East;
                            itemid = 0x398C;
                        }
                        break;
                    case 3:
                        {
                            d = Direction.Down;
                            itemid = 0;
                        }
                        break;
                    case 4:
                        {
                            d = Direction.South;
                            itemid = 0x3996;
                        }
                        break;
                    case 5:
                        {
                            d = Direction.Left;
                            itemid = 0;
                        }
                        break;
                    case 6:
                        {
                            d = Direction.West;
                            itemid = 0x398C;
                        }
                        break;
                    case 7:
                        {
                            d = Direction.Up;
                            itemid = 0;
                        }
                        break;
                }

                for (int j = 0; j < 16; j++)
                {
                    Ability.IncreaseByDirection(ref point, d);

                    if (from.CanSee(point))
                    {
                        // Damage was 2 on the nightmare which has 30~40% fire res. 4 - 35% = 2.6, close enough for me.
                        if (itemid != 0)
                            new FireFieldSpell.FireFieldItem(itemid, point, from, from.Map, TimeSpan.FromSeconds(30), 100);
                        else
                        {
                            new OtherFireFieldItem(0x3996, point, from, from.Map, TimeSpan.FromSeconds(30), 1, 80);
                            new OtherFireFieldItem(0x398C, point, from, from.Map, TimeSpan.FromSeconds(30), 1, 80);
                        }
                    }
                }

                point = from.Location;
            }
 
            Effects.PlaySound(point, map, 0x44B); 
        }

        /*public static void IncreaseByDirection(ref Point3D point, Direction d)
        {
        switch (d)
        {
        case (Direction)0x0:
        case (Direction)0x80: point.Y--; break; //North
        case (Direction)0x1:
        case (Direction)0x81: { point.X++; point.Y--; break; } //Right
        case (Direction)0x2:
        case (Direction)0x82: point.X++; break; //East
        case (Direction)0x3:
        case (Direction)0x83: { point.X++; point.Y++; break; } //Down
        case (Direction)0x4:
        case (Direction)0x84: point.Y++; break; //South
        case (Direction)0x5:
        case (Direction)0x85: { point.X--; point.Y++; break; } //Left
        case (Direction)0x6:
        case (Direction)0x86: point.X--; break; //West
        case (Direction)0x7:
        case (Direction)0x87: { point.X--; point.Y--; break; } //Up
        default: { break; }
        }
        }*/
        public class OtherFireFieldItem : FireFieldSpell.FireFieldItem
        {
            public override bool BlocksFit
            {
                get
                {
                    return false;
                }
            }

            public OtherFireFieldItem(int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val, int damage)
                : base(itemID, loc, caster, map, duration, damage)
            {
            }

            public OtherFireFieldItem(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();
            }
        }
        #endregion

        #region CrimsonMeteor
        public static void CrimsonMeteor(Mobile from, int damage)
        {
            if (!CanUse(from))
                return;

            from.Say("*Des-ailem Flam*");  

            new CrimsonMeteorTimer(from, damage).Start();
        }

        public class CrimsonMeteorTimer : Timer
        {
            private Mobile m_From;
            private int m_Damage;
            private int m_Count;
            private int m_MaxCount;
            private Point3D m_LastTarget;
            private Point3D m_ShowerLocation;

            public CrimsonMeteorTimer(Mobile from, int damage)
                : base(TimeSpan.FromMilliseconds(300.0), TimeSpan.FromMilliseconds(300.0))
            {
                m_From = from;
                m_Damage = damage;
                m_Count = 0;
                m_MaxCount = 30;
                m_LastTarget = new Point3D(0, 0, 0);
                m_ShowerLocation = new Point3D(from.Location);
            }

            protected override void OnTick()
            {
                if (m_From == null || m_From.Deleted)
                {
                    Stop();
                    return;
                }

                new FireField(m_From, 50, m_Damage, m_Damage, Utility.RandomBool(), m_LastTarget, m_From.Map);

                Point3D point = new Point3D();
                int tries = 0;
                                
                while (tries < 5)
                {
                    point.X = m_ShowerLocation.X += Utility.RandomMinMax(-5, 5);
                    point.Y = m_ShowerLocation.Y += Utility.RandomMinMax(-5, 5);
                    point.Z = m_From.Map.GetAverageZ(point.X, point.Y);

                    if (m_From.CanSee(point))
                        break;

                    tries++;
                }

                Effects.SendMovingParticles(
                    new Entity(Serial.Zero, new Point3D(point.X, point.Y, point.Z + 30), m_From.Map),
                    new Entity(Serial.Zero, point, m_From.Map),
                    0x36D4, 5, 0, false, false, 0, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                Effects.PlaySound(point, m_From.Map, 0x11D);   
 
                m_LastTarget = point;
                m_Count++;

                if (m_Count >= m_MaxCount)
                {
                    Stop();
                    return;
                }
            }
        }
        #endregion

        #region JaggedFire
        public static void JaggedLineEffect(Mobile from, int range, int speed)
        {
            if (CanUse(from))
                new JaggedLineTimer(from, range, speed).Start();
        }

        public static Direction JaggedLine(Direction d)
        {
            int number = (int)d + Utility.RandomMinMax(-1, 1);

            if (number < 0)
                number = 8;

            number %= 8;

            return (Direction)number;
        }

        public class JaggedLineTimer : Timer
        {
            private Mobile m_From;
            private Direction m_D;
            private Point3D m_Point;
            private Map m_Map;
            private int m_Count;
            private int m_MaxCount;

            public JaggedLineTimer(Mobile from, int range, int speed)
                : base(TimeSpan.FromMilliseconds(speed), TimeSpan.FromMilliseconds(speed))
            {
                m_From = from;
                m_D = from.Direction;
                m_Point = new Point3D(from.Location);
                m_Map = from.Map;
                m_Count = 0;
                m_MaxCount = range;
            }

            protected override void OnTick()
            {
                if (m_From == null || m_From.Deleted)
                {
                    Stop();
                    return;
                }

                m_Count++;

                if (m_Count == 0)
                    Ability.IncreaseByDirection(ref m_Point, m_D);
                else
                    Ability.IncreaseByDirection(ref m_Point, JaggedLine(m_D));

                Point3D p = new Point3D(m_Point.X, m_Point.Y, m_Map.GetAverageZ(m_Point.X, m_Point.Y));

                if (m_Map.CanFit(p, 16, false, false))
                {
                    bool canplace = true;

                    foreach (Item item in m_Map.GetItemsInRange(p, 0))
                    {
                        if (item != null)
                        {
                            if (item is FireField && item.Visible == false)
                            {
                                canplace = false;
                                break;
                            }
                        }
                    }

                    if (canplace)
                    {
                        new FireField(m_From, 30, 25, 35, false, new Point3D(p.X, p.Y, p.Z), m_Map).Visible = false;
                        new FireField(m_From, 30, 0, 0, true, new Point3D(p.X, p.Y + 1, p.Z), m_Map);
                        new FireField(m_From, 30, 0, 0, false, new Point3D(p.X + 1, p.Y, p.Z), m_Map);
                    }
                }
                else
                    m_Count = 999;

                if (m_Count > m_MaxCount)
                {
                    Stop();
                }
            }
        }
        #endregion

        #region FireField
        public class FireField : Item
        {
            private Mobile m_Owner;
            private int m_MinDamage;
            private int m_MaxDamage;
            private DateTime m_Destroy;
            private Point3D m_MoveToPoint;
            private Map m_MoveToMap;
            private Timer m_Timer;
            private List<Mobile> m_List;

            [Constructable]
            public FireField(int duration, int min, int max, bool south, Point3D point, Map map)
                : this(null, duration, min, max, south, point, map)
            {
            }

            [Constructable]
            public FireField(Mobile owner, int duration, int min, int max, bool south, Point3D point, Map map)
                : base(GetItemID(south))
            {
                Movable = false;

                m_Owner = owner;
                m_MinDamage = min;
                m_MaxDamage = max;
                m_Destroy = DateTime.UtcNow + TimeSpan.FromSeconds((double)duration + 1.5);
                m_MoveToPoint = point;
                m_MoveToMap = map;
                m_List = new List<Mobile>();
                m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), new TimerCallback(OnTick));
                Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1.5), new TimerCallback(Move));
            }

            private static int GetItemID(bool south)
            {
                if (south)
                    return 0x398C;
                else
                    return 0x3996;
            }

            public override void OnAfterDelete()
            {
                if (m_Timer != null)
                    m_Timer.Stop();
            }

            private void Move()
            {
                if (!Visible)
                    ItemID = 0x36FE;

                MoveToWorld(m_MoveToPoint, m_MoveToMap);
            }

            private void OnTick()
            {
                if (DateTime.UtcNow > m_Destroy)
                    Delete();
                else if (m_MinDamage != 0)
                {
                    IPooledEnumerable eable = GetMobilesInRange(0);
                    foreach (Mobile m in eable)
                    {
                        if (m == null)
                            continue;
                        else if (m_Owner != null)
                        {
                            if (Ability.CanTarget(m_Owner, m, true, true, false))
                                m_List.Add(m);
                        }
                        else
                            m_List.Add(m);
                    }

                    eable.Free();

                    for (int i = 0; i < m_List.Count; i++)
                    {
                        if (m_List[i] != null)
                            DealDamage(m_List[i]);
                    }

                    m_List.Clear();
                    m_List = new List<Mobile>();
                }
            }

            public override bool OnMoveOver(Mobile m)
            {
                if (m_MinDamage != 0)
                    DealDamage(m);

                return true;
            }

            public void DealDamage(Mobile m)
            {
                if (m != m_Owner)
                    AOS.Damage(m, (m_Owner == null) ? m : m_Owner, Utility.RandomMinMax(m_MinDamage, m_MaxDamage), 0, 100, 0, 0, 0);
            }

            public FireField(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                // Unsaved.
            }

            public override void Deserialize(GenericReader reader)
            {
            }
        }
        #endregion
    }
}

namespace Server.Commands
{
    public partial class TheSixCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("SimpleFlame", AccessLevel.Seer, new CommandEventHandler(SimpleFlame_OnCommand));
            CommandSystem.Register("SoulDrain", AccessLevel.Seer, new CommandEventHandler(SoulDrain_OnCommand));
            CommandSystem.Register("FlameWave", AccessLevel.Seer, new CommandEventHandler(FlameWave_OnCommand));
            CommandSystem.Register("FlameCross", AccessLevel.Seer, new CommandEventHandler(FlameCross_OnCommand));            
            CommandSystem.Register("CrimsonMeteor", AccessLevel.Seer, new CommandEventHandler(CrimsonMeteor_OnCommand));
            CommandSystem.Register("JaggedFire", AccessLevel.Seer, new CommandEventHandler(JaggedFire_OnCommand));
        }

        [Description("Use the Six's Simple Flame attack")]
        public static void SimpleFlame_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(10, false, TargetFlags.Harmful, new TargetCallback(SimpleFlame_CallBack));
        }

        public static void SimpleFlame_CallBack(Mobile from, object targeted)
        {
            if (targeted is Mobile)
                Ability.SimpleFlame(from, (Mobile)targeted, 35);
            else
                from.SendMessage("That is not a mobile");
        }

        [Description("Use the Six's Soul Drain attack")]
        public static void SoulDrain_OnCommand(CommandEventArgs e)
        {
            Ability.SoulDrain(e.Mobile);
        }

        [Description("Use the Six's Flame Wave attack")]
        public static void FlameWave_OnCommand(CommandEventArgs e)
        {
            Ability.FlameWave(e.Mobile);
        }

        [Description("Use the Six's Fire Field attack")]
        public static void FlameCross_OnCommand(CommandEventArgs e)
        {
            Ability.FlameCross(e.Mobile);
        }

        [Description("Use the Crimson Meteor attack")]
        public static void CrimsonMeteor_OnCommand(CommandEventArgs e)
        {
            Ability.CrimsonMeteor(e.Mobile, 35);
        }

        [Description("Shoot a Jagged line of fire")]
        public static void JaggedFire_OnCommand(CommandEventArgs e)
        {
            Ability.JaggedLineEffect(e.Mobile, 25, 500);
            Ability.JaggedLineEffect(e.Mobile, 25, 500);
            Ability.JaggedLineEffect(e.Mobile, 25, 500);
        }
    }
}
