using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;

using System;
using System.Collections.Generic;

namespace Server.Regions
{
    public class MazeOfDeathRegion : Region
    {
        private static readonly Point2D[] _Points =
        {
            new Point2D(1062, 1060), new Point2D(1062, 1059), new Point2D(1062, 1058), new Point2D(1061, 1058),
            new Point2D(1060, 1058), new Point2D(1060, 1057), new Point2D(1059, 1057), new Point2D(1059, 1056),
            new Point2D(1059, 1055), new Point2D(1060, 1055), new Point2D(1060, 1054), new Point2D(1060, 1053),
            new Point2D(1059, 1053), new Point2D(1059, 1052), new Point2D(1059, 1051), new Point2D(1059, 1050),
            new Point2D(1058, 1050), new Point2D(1058, 1049), new Point2D(1057, 1049), new Point2D(1057, 1048),
            new Point2D(1057, 1047), new Point2D(1057, 1046), new Point2D(1058, 1047), new Point2D(1059, 1047),
            new Point2D(1059, 1046), new Point2D(1059, 1045), new Point2D(1059, 1044), new Point2D(1060, 1044),
            new Point2D(1061, 1044), new Point2D(1061, 1043), new Point2D(1060, 1042), new Point2D(1059, 1042),
            new Point2D(1058, 1042), new Point2D(1057, 1042), new Point2D(1061, 1042), new Point2D(1061, 1041),
            new Point2D(1061, 1042), new Point2D(1062, 1042), new Point2D(1062, 1041), new Point2D(1062, 1040),
            new Point2D(1063, 1040), new Point2D(1063, 1041), new Point2D(1063, 1040), new Point2D(1063, 1039),
            new Point2D(1062, 1039), new Point2D(1062, 1038), new Point2D(1061, 1038), new Point2D(1061, 1037),
            new Point2D(1060, 1037), new Point2D(1059, 1037), new Point2D(1058, 1037), new Point2D(1057, 1037),
            new Point2D(1057, 1036), new Point2D(1057, 1035), new Point2D(1058, 1035), new Point2D(1059, 1035),
            new Point2D(1057, 1034), new Point2D(1057, 1033), new Point2D(1057, 1032), new Point2D(1058, 1032),
            new Point2D(1059, 1032), new Point2D(1060, 1032), new Point2D(1060, 1031), new Point2D(1060, 1030),
            new Point2D(1060, 1029), new Point2D(1061, 1029), new Point2D(1061, 1028), new Point2D(1061, 1027),
            new Point2D(1062, 1027), new Point2D(1063, 1027), new Point2D(1064, 1027), new Point2D(1061, 1026),
            new Point2D(1062, 1026), new Point2D(1063, 1026), new Point2D(1064, 1026), new Point2D(1061, 1026),
            new Point2D(1061, 1025), new Point2D(1061, 1024), new Point2D(1061, 1023), new Point2D(1061, 1022),
            new Point2D(1060, 1026), new Point2D(1059, 1026), new Point2D(1058, 1026), new Point2D(1058, 1025),
            new Point2D(1058, 1024), new Point2D(1058, 1023), new Point2D(1058, 1022), new Point2D(1058, 1021),
            new Point2D(1057, 1021), new Point2D(1057, 1020), new Point2D(1057, 1019), new Point2D(1057, 1018),
            new Point2D(1058, 1018), new Point2D(1059, 1018), new Point2D(1060, 1018), new Point2D(1061, 1018),
            new Point2D(1061, 1017), new Point2D(1061, 1016), new Point2D(1061, 1015), new Point2D(1061, 1014),
            new Point2D(1061, 1013), new Point2D(1061, 1012), new Point2D(1061, 1011), new Point2D(1061, 1010),
            new Point2D(1060, 1010), new Point2D(1059, 1010), new Point2D(1059, 1009), new Point2D(1059, 1008),
            new Point2D(1059, 1007), new Point2D(1059, 1006), new Point2D(1059, 1005), new Point2D(1059, 1004),
            new Point2D(1058, 1004), new Point2D(1057, 1004), new Point2D(1057, 1003), new Point2D(1057, 1002),
            new Point2D(1057, 1001), new Point2D(1057, 1000), new Point2D(1057, 999), new Point2D(1058, 999),
            new Point2D(1059, 999), new Point2D(1060, 999), new Point2D(1061, 999), new Point2D(1062, 999),
            new Point2D(1063, 999), new Point2D(1063, 998), new Point2D(1063, 997), new Point2D(1063, 996),
            new Point2D(1063, 995), new Point2D(1063, 994), new Point2D(1062, 994), new Point2D(1061, 994),
            new Point2D(1061, 993), new Point2D(1061, 992), new Point2D(1061, 991)
        };

        public static void Initialize()
        {
            new MazeOfDeathRegion();

            Path = new List<Point2D>();

            foreach (var point in _Points)
            {
                Path.Add(point);
            }

            //Add some randoms
            int toAdd = 33;

            while (toAdd > 0)
            {
                int x = Utility.RandomMinMax(m_TrapCorridor.X, m_TrapCorridor.X + m_TrapCorridor.Width);
                int y = Utility.RandomMinMax(m_TrapCorridor.Y, m_TrapCorridor.Y + m_TrapCorridor.Height);

                Point2D p = new Point2D(x, y);

                if (!Path.Contains(p))
                {
                    Path.Add(p);
                    toAdd--;
                }
            }
        }

        private static Rectangle2D m_FrontEntrance = new Rectangle2D(1057, 1062, 8, 6);
        private static Rectangle2D m_Room = new Rectangle2D(1065, 1055, 11, 11);
        private static Rectangle2D m_Corridor = new Rectangle2D(1056, 990, 9, 72);        
        private static Rectangle2D m_PuzzleRoom = new Rectangle2D(1065, 1023, 8, 8);
        private static Rectangle2D m_RearEntrance = new Rectangle2D(1056, 986, 9, 4);

        private static readonly Rectangle2D[] m_Bounds =
        {
            m_FrontEntrance,
            m_Room,
            m_Corridor,
            m_PuzzleRoom,
            m_RearEntrance
        };

        private static Rectangle2D m_DeathBounds = new Rectangle2D(1057, 1062, 7, 5);
        private static Rectangle2D m_TrapCorridor = new Rectangle2D(1056, 991, 9, 70);

        public static List<Point2D> Path { get; private set; }

        public MazeOfDeathRegion()
            : base("Maze of Death", Map.TerMur, DefaultPriority, m_Bounds)
        {
            Register();
        }

        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if (m.AccessLevel > AccessLevel.Player)
                return true;

            if (s is Spells.Sixth.MarkSpell || s is Spells.Seventh.GateTravelSpell || s is Spells.Third.TeleportSpell)
            {
                m.SendLocalizedMessage(501802); // that spell doesn't seem to work.
                return false;
            }

            return base.OnBeginSpellCast(m, s);
        }

        public override bool OnTarget(Mobile m, Target t, object o)
        {
            if (m.AccessLevel == AccessLevel.Player && t is Spells.Third.TeleportSpell.InternalTarget)
            {
                m.SendLocalizedMessage(501802); // that spell doesn't seem to work.
                return false;
            }

            return base.OnTarget(m, t, o);
        }

        public override void OnEnter(Mobile m)
        {
            if (m is PlayerMobile pm && pm.Alive && (m_FrontEntrance.Contains(pm.Location) || m_RearEntrance.Contains(pm.Location)))
            {
                pm.Frozen = true;
                pm.LocalOverheadMessage(MessageType.Regular, 33, 1113580); // You are filled with a sense of dread and impending doom!

                Timer.DelayCall(TimeSpan.FromSeconds(2.0), () =>
                {
                    if (pm.Backpack != null && pm.Backpack.FindItemByType<GoldenCompass>(false) != null)
                    {
                        pm.LocalOverheadMessage(MessageType.Regular, 946, 1113582); // I better proceed with caution.
                    }
                    else
                    {
                        pm.LocalOverheadMessage(MessageType.Regular, 946, 1113581); // I might need something to help me navigate through this.
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(2.0), () => { pm.Frozen = false; });
                });
            }
        }

        public override void OnLocationChanged(Mobile m, Point3D oldLocation)
        {
            base.OnLocationChanged(m, oldLocation);

            if (m != null)
            {
                if (m.Alive)
                {
                    if (m_TrapCorridor.Contains(m.Location) && !Path.Contains(new Point2D(m.Location.X, m.Location.Y)))
                    {
                        SpringTrap(m);
                    }
                    else if (m_Corridor.Contains(m.Location) && m.Backpack != null)
                    {
                        Item item = m.Backpack.FindItemByType(typeof(GoldenCompass));

                        if (item != null)
                        {
                            m.CloseGump(typeof(CompassDirectionGump));
                            m.SendGump(new CompassDirectionGump(m));
                        }
                    }
                    else if (m.HasGump(typeof(CompassDirectionGump)))
                    {
                        m.CloseGump(typeof(CompassDirectionGump));
                    }
                }
                else if (m_TrapCorridor.Contains(m.Location))
                {
                    m.MoveToWorld(new Point3D(1060, 1066, -42), Map.TerMur);
                }
            }
        }

        public override void OnDeath(Mobile m)
        {
            base.OnDeath(m);

            if (m.Player && m_TrapCorridor.Contains(m.Location))
                Timer.DelayCall(TimeSpan.FromSeconds(3), new TimerStateCallback(Kick_Callback), m);
        }

        public void SpringTrap(Mobile from)
        {
            if (from == null || !from.Alive)
                return;

            int cliloc;
            int damage = Utility.RandomMinMax(75, 150);

            switch (Utility.Random(4))
            {
                default:
                case 0:
                    Effects.SendLocationEffect(from, from.Map, 0x3709, 30);
                    from.PlaySound(0x54);
                    cliloc = 1010524; // Searing heat scorches thy skin.
                    AOS.Damage(from, damage, 0, 100, 0, 0, 0);
                    break;
                case 1:
                    from.PlaySound(0x223);
                    cliloc = 1010525; // Pain lances through thee from a sharp metal blade.
                    AOS.Damage(from, damage, 100, 0, 0, 0, 0);
                    break;
                case 2:
                    from.BoltEffect(0);
                    cliloc = 1010526; // Lightning arcs through thy body.
                    AOS.Damage(from, damage, 0, 0, 0, 0, 100);
                    break;
                case 3:
                    Effects.SendLocationEffect(from, from.Map, 0x113A, 20, 10);
                    from.PlaySound(0x231);
                    from.ApplyPoison(from, Poison.Deadly);
                    cliloc = 1010523; // A toxic vapor envelops thee.
                    AOS.Damage(from, damage, 0, 0, 0, 100, 0);
                    break;
            }

            from.LocalOverheadMessage(MessageType.Regular, 0xEE, cliloc);
        }

        public void Kick_Callback(object o)
        {
            Mobile m = (Mobile)o;

            if (m != null)
                KickToEntrance(m);
        }

        public void KickToEntrance(Mobile from)
        {
            if (from == null || from.Map == null)
                return;

            int x = Utility.RandomMinMax(m_DeathBounds.X, m_DeathBounds.X + m_DeathBounds.Width);
            int y = Utility.RandomMinMax(m_DeathBounds.Y, m_DeathBounds.Y + m_DeathBounds.Height);
            int z = from.Map.GetAverageZ(x, y);

            Point3D p = new Point3D(x, y, z);

            from.MoveToWorld(p, Map.TerMur);

            if (from.Player && !from.Alive && from.Corpse != null)
                from.Corpse.MoveToWorld(p, Map.TerMur);

            from.SendLocalizedMessage(1113566); // You will find your remains at the entrance of the maze.
        }
    }
}
