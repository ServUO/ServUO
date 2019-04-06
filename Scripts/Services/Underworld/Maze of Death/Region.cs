using Server;
using System;
using Server.Mobiles;
using Server.Gumps;
using System.Collections.Generic;
using Server.Targeting;
using Server.Items;

namespace Server.Regions
{
	public class MazeOfDeathRegion : Region
	{
		public static void Initialize()
		{
			new MazeOfDeathRegion();
			
			m_Path = new List<Point3D>();
			
			m_Path.Add(new Point3D(1062, 1060, -42));
			m_Path.Add(new Point3D(1062, 1059, -42));
			m_Path.Add(new Point3D(1062, 1058, -42));
			m_Path.Add(new Point3D(1061, 1058, -42));
            m_Path.Add(new Point3D(1060, 1058, -42));
            m_Path.Add(new Point3D(1060, 1057, -42));
			m_Path.Add(new Point3D(1059, 1057, -42));
			m_Path.Add(new Point3D(1059, 1056, -42));
			m_Path.Add(new Point3D(1059, 1055, -42));
			m_Path.Add(new Point3D(1060, 1055, -42));
			m_Path.Add(new Point3D(1060, 1054, -42));
			m_Path.Add(new Point3D(1060, 1053, -42));
            m_Path.Add(new Point3D(1059, 1053, -42));
            m_Path.Add(new Point3D(1059, 1052, -42));
            m_Path.Add(new Point3D(1059, 1051, -42));
            m_Path.Add(new Point3D(1059, 1050, -42));
            m_Path.Add(new Point3D(1058, 1050, -42));
            m_Path.Add(new Point3D(1058, 1049, -42));
            m_Path.Add(new Point3D(1057, 1049, -42));
            m_Path.Add(new Point3D(1057, 1048, -42));
            m_Path.Add(new Point3D(1057, 1047, -42));
            m_Path.Add(new Point3D(1057, 1046, -42));
            m_Path.Add(new Point3D(1058, 1047, -42));
            m_Path.Add(new Point3D(1059, 1047, -42));
            m_Path.Add(new Point3D(1059, 1046, -42));
            m_Path.Add(new Point3D(1059, 1045, -42));
            m_Path.Add(new Point3D(1059, 1044, -42));
            m_Path.Add(new Point3D(1060, 1044, -42));
            m_Path.Add(new Point3D(1061, 1044, -42));
            m_Path.Add(new Point3D(1061, 1043, -42));
            m_Path.Add(new Point3D(1060, 1042, -42));

            m_Path.Add(new Point3D(1059, 1042, -42));
            m_Path.Add(new Point3D(1058, 1042, -42));
            m_Path.Add(new Point3D(1057, 1042, -42));

            m_Path.Add(new Point3D(1061, 1042, -42));
            m_Path.Add(new Point3D(1061, 1041, -42));
            m_Path.Add(new Point3D(1061, 1042, -42));
            m_Path.Add(new Point3D(1062, 1042, -42));
            m_Path.Add(new Point3D(1062, 1041, -42));
            m_Path.Add(new Point3D(1062, 1040, -42));
            m_Path.Add(new Point3D(1063, 1040, -42));
            m_Path.Add(new Point3D(1063, 1041, -42));
            m_Path.Add(new Point3D(1063, 1040, -42));
            m_Path.Add(new Point3D(1063, 1039, -42));
            m_Path.Add(new Point3D(1062, 1039, -42));
            m_Path.Add(new Point3D(1062, 1038, -42));
            m_Path.Add(new Point3D(1061, 1038, -42));
            m_Path.Add(new Point3D(1061, 1037, -42));
            m_Path.Add(new Point3D(1060, 1037, -42));
            m_Path.Add(new Point3D(1059, 1037, -42));
            m_Path.Add(new Point3D(1058, 1037, -42));
            m_Path.Add(new Point3D(1057, 1037, -42));
            m_Path.Add(new Point3D(1057, 1036, -42));
            m_Path.Add(new Point3D(1057, 1035, -42));

            m_Path.Add(new Point3D(1058, 1035, -42));
            m_Path.Add(new Point3D(1059, 1035, -42));

            m_Path.Add(new Point3D(1057, 1034, -42));
            m_Path.Add(new Point3D(1057, 1033, -42));
            m_Path.Add(new Point3D(1057, 1032, -42));
            m_Path.Add(new Point3D(1058, 1032, -42));
            m_Path.Add(new Point3D(1059, 1032, -42));
            m_Path.Add(new Point3D(1060, 1032, -42));
            m_Path.Add(new Point3D(1060, 1031, -42));
            m_Path.Add(new Point3D(1060, 1030, -42));
            m_Path.Add(new Point3D(1060, 1029, -42));
            m_Path.Add(new Point3D(1061, 1029, -42));
            m_Path.Add(new Point3D(1061, 1028, -42));

            m_Path.Add(new Point3D(1061, 1027, -42));
            m_Path.Add(new Point3D(1062, 1027, -42));
            m_Path.Add(new Point3D(1063, 1027, -42));
            m_Path.Add(new Point3D(1064, 1027, -42));
            m_Path.Add(new Point3D(1061, 1026, -42));
            m_Path.Add(new Point3D(1062, 1026, -42));
            m_Path.Add(new Point3D(1063, 1026, -42));
            m_Path.Add(new Point3D(1064, 1026, -42));

            m_Path.Add(new Point3D(1061, 1026, -42));
            m_Path.Add(new Point3D(1061, 1025, -42));
            m_Path.Add(new Point3D(1061, 1024, -42));
            m_Path.Add(new Point3D(1061, 1023, -42));
            m_Path.Add(new Point3D(1061, 1022, -42));

            m_Path.Add(new Point3D(1060, 1026, -42));
            m_Path.Add(new Point3D(1059, 1026, -42));
            m_Path.Add(new Point3D(1058, 1026, -42));

            m_Path.Add(new Point3D(1058, 1025, -42));
            m_Path.Add(new Point3D(1058, 1024, -42));
            m_Path.Add(new Point3D(1058, 1023, -42));
            m_Path.Add(new Point3D(1058, 1022, -42));
            m_Path.Add(new Point3D(1058, 1021, -42));

            m_Path.Add(new Point3D(1057, 1021, -42));

            m_Path.Add(new Point3D(1057, 1020, -42));
            m_Path.Add(new Point3D(1057, 1019, -42));
            m_Path.Add(new Point3D(1057, 1018, -42));

            m_Path.Add(new Point3D(1058, 1018, -42));
            m_Path.Add(new Point3D(1059, 1018, -42));
            m_Path.Add(new Point3D(1060, 1018, -42));
            m_Path.Add(new Point3D(1061, 1018, -42));

            m_Path.Add(new Point3D(1061, 1017, -42));
            m_Path.Add(new Point3D(1061, 1016, -42));
            m_Path.Add(new Point3D(1061, 1015, -42));
            m_Path.Add(new Point3D(1061, 1014, -42));
            m_Path.Add(new Point3D(1061, 1013, -42));
            m_Path.Add(new Point3D(1061, 1012, -42));
            m_Path.Add(new Point3D(1061, 1011, -42));
            m_Path.Add(new Point3D(1061, 1010, -42));

            m_Path.Add(new Point3D(1060, 1010, -42));
            m_Path.Add(new Point3D(1059, 1010, -42));

            m_Path.Add(new Point3D(1059, 1009, -42));
            m_Path.Add(new Point3D(1059, 1008, -42));
            m_Path.Add(new Point3D(1059, 1007, -42));
            m_Path.Add(new Point3D(1059, 1006, -42));
            m_Path.Add(new Point3D(1059, 1005, -42));
            m_Path.Add(new Point3D(1059, 1004, -42));

            m_Path.Add(new Point3D(1058, 1004, -42));
            m_Path.Add(new Point3D(1057, 1004, -42));

            m_Path.Add(new Point3D(1057, 1003, -42));
            m_Path.Add(new Point3D(1057, 1002, -42));
            m_Path.Add(new Point3D(1057, 1001, -42));
            m_Path.Add(new Point3D(1057, 1000, -42));
            m_Path.Add(new Point3D(1057, 999, -42));

            m_Path.Add(new Point3D(1058, 999, -42));
            m_Path.Add(new Point3D(1059, 999, -42));
            m_Path.Add(new Point3D(1060, 999, -42));
            m_Path.Add(new Point3D(1061, 999, -42));
            m_Path.Add(new Point3D(1062, 999, -42));
            m_Path.Add(new Point3D(1063, 999, -42));

            m_Path.Add(new Point3D(1063, 998, -42));
            m_Path.Add(new Point3D(1063, 997, -42));
            m_Path.Add(new Point3D(1063, 996, -42));
            m_Path.Add(new Point3D(1063, 995, -42));
            m_Path.Add(new Point3D(1063, 994, -42));

            m_Path.Add(new Point3D(1062, 994, -42));
            m_Path.Add(new Point3D(1061, 994, -42));

            m_Path.Add(new Point3D(1061, 993, -42));
            m_Path.Add(new Point3D(1061, 992, -42));
            m_Path.Add(new Point3D(1061, 991, -42));
            m_Path.Add(new Point3D(1061, 990, -42));

            //Add some randoms
            int toAdd = 33;

            while (toAdd > 0)
            {
                int x = Utility.RandomMinMax(m_TrapBounds.X, m_TrapBounds.X + m_TrapBounds.Width);
                int y = Utility.RandomMinMax(m_TrapBounds.Y, m_TrapBounds.Y + m_TrapBounds.Height);
                int z = -42;

                Point3D p = new Point3D(x, y, z);

                if (!m_Path.Contains(p))
                {
                    m_Path.Add(p);
                    toAdd--;
                }
            }
		}
		
		private static Rectangle2D[] m_Bounds = new Rectangle2D[]
		{
			new Rectangle2D(1057, 1028, 16, 40),
            new Rectangle2D(1057, 990, 16, 38)
		};

        private static Rectangle2D m_TrapBounds = new Rectangle2D(1057, 990, 7, 71);
		
		private static List<Point3D> m_Path;
		public static List<Point3D> Path { get { return m_Path; } }
		
		private static Rectangle2D m_Entrance = new Rectangle2D(1057, 1062, 7, 5);
		
		public MazeOfDeathRegion() : base("Maze of Death", Map.TerMur, Region.DefaultPriority, m_Bounds)
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
	
		public override bool OnTarget( Mobile m, Target t, object o )
		{
			if(m.AccessLevel == AccessLevel.Player && t is Server.Spells.Third.TeleportSpell.InternalTarget)
			{
                m.SendLocalizedMessage(501802); // that spell doesn't seem to work.
				return false;
			}
				
			return base.OnTarget(m, t, o);
		}

		public override void OnEnter( Mobile m )
		{
			if(m.Backpack == null)
				return;

            if (m.NetState != null)
            {
                m.Paralyze(TimeSpan.FromSeconds(2));
                m.PrivateOverheadMessage(Server.Network.MessageType.Regular, 33, 1113580, m.NetState); // You are filled with a sense of dread and impending doom!
                m.PrivateOverheadMessage(Server.Network.MessageType.Regular, 0x3B2, 1113581, m.NetState); // I might need something to help me navigate through this.

                if (m.Backpack.FindItemByType(typeof(GoldenCompass)) != null)
                {
                    m.CloseGump(typeof(CompassDirectionGump));
                    m.SendGump(new CompassDirectionGump(m));
                }
            }

			base.OnEnter(m);
		}
		
		public override void OnExit( Mobile m )
		{
			m.CloseGump(typeof(CompassDirectionGump));
			base.OnExit(m);
		}
		
		public override void OnLocationChanged( Mobile m, Point3D oldLocation )
		{
			base.OnLocationChanged(m, oldLocation);

            if (oldLocation.X > 1063 && m.Location.X <= 1063)
            {
                if (m.Backpack != null && m.Backpack.FindItemByType(typeof(GoldenCompass)) != null)
                    m.SendLocalizedMessage(1113582); // I better proceed with caution.
                else
                    m.SendLocalizedMessage(1113581); // I might need something to help me navigate through this.
            }
            else if (oldLocation.Y == 991 && m.Location.Y == 990)
            {
                if(m.HasGump(typeof(CompassDirectionGump)))
                    m.SendLocalizedMessage(1113585); // The compass' arrows flicker. You must be near the right location.
            }
			
			if(m != null && m.Backpack != null)
			{
				Item item = m.Backpack.FindItemByType(typeof(GoldenCompass));

                if (m.Alive && !m_Path.Contains(m.Location) && m_TrapBounds.Contains(m.Location))
                    SpringTrap(m);

				else if (m.Alive && m.HasGump(typeof(CompassDirectionGump)))
				{
					//May need to check old gump to get x,y so new gump opens in same spot!
					m.CloseGump(typeof(CompassDirectionGump));
					m.SendGump(new CompassDirectionGump(m));
				}
			}
		}
		
		public override void OnDeath( Mobile m )
		{
			base.OnDeath(m);
			
			if(m.Player && m_TrapBounds.Contains(m.Location))
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
                    Effects.SendLocationEffect( from, from.Map, 0x3709, 30 );
					from.PlaySound( 0x54 );
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
                    Effects.SendLocationEffect( from, from.Map, 0x113A, 20, 10 );
					from.PlaySound( 0x231 );
                    from.ApplyPoison(from, Poison.Deadly);
                    cliloc = 1010523; // A toxic vapor envelops thee.
                    AOS.Damage(from, damage, 0, 0, 0, 100, 0);
                    break;
            }

            from.LocalOverheadMessage(Server.Network.MessageType.Regular, 0xEE, cliloc);
        }

		public void Kick_Callback(object o)
		{
            Mobile m = (Mobile)o;

            if(m != null)
			    KickToEntrance(m);
		}
		
		public void KickToEntrance(Mobile from)
		{
			if(from == null || from.Map == null)
				return;
				
			int x = Utility.RandomMinMax(m_Entrance.X, m_Entrance.X + m_Entrance.Width);
			int y = Utility.RandomMinMax(m_Entrance.Y, m_Entrance.Y + m_Entrance.Height);
			int z = from.Map.GetAverageZ(x, y);
			
			Point3D p = new Point3D(x, y, z);
			
			from.MoveToWorld(p, Map.TerMur);
			
			if(from.Player && !from.Alive && from.Corpse != null)
				from.Corpse.MoveToWorld(p, Map.TerMur);
				
			from.SendMessage("You have been teleported to the beginning of the maze.");
		}
	}
}
