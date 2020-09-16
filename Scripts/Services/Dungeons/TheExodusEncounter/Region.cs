using Server.Items;
using Server.Mobiles;
using Server.Regions;
using Server.Spells;
using System;
using System.Linq;
using System.Xml;

namespace Server.Engines.Exodus
{
    public class VerLorRegCity : DungeonRegion
    {
        private static readonly Point3D[] Random_Locations =
        {
            new Point3D(1217, 469, -13),
            new Point3D(720, 1356, -60),
            new Point3D(748, 728, -29),
            new Point3D(287, 1016, 0),
            new Point3D(987, 1007, -35),
            new Point3D(1175, 1287, -30),
            new Point3D(1532, 1341, -3),
            new Point3D(527, 218, -44),
        };

        public VerLorRegCity(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public class ExitTimer : Timer
        {
            private static TimeSpan m_Delay = TimeSpan.FromMinutes(2);
            private static TimeSpan m_Warning = TimeSpan.FromMinutes(8);
            readonly VerLorRegCity m_region;

            public ExitTimer(VerLorRegCity region) : base(m_Warning)
            {
                m_region = region;
            }
            protected override void OnTick()
            {
                foreach (Mobile m in m_region.GetEnumeratedMobiles().Where(m => m is PlayerMobile && m.AccessLevel == AccessLevel.Player))
                {
                    m.SendLocalizedMessage(1010589);
                }

                DelayCall(m_Delay, m_region.MoveLocation);
            }
        }

        public void MoveLocation()
        {
            foreach (Mobile m in GetEnumeratedMobiles().Where(m => m is PlayerMobile && m.AccessLevel == AccessLevel.Player))
            {
                Point3D p = Random_Locations[Utility.Random(Random_Locations.Length)];

                m.MoveToWorld(p, m.Map);
                BaseCreature.TeleportPets(m, p, m.Map);
            }

            VerLorRegController.Start();
        }

        public override bool CheckTravel(Mobile traveller, Point3D p, TravelCheckType type)
        {
            if (traveller.AccessLevel > AccessLevel.Player)
                return true;

            return type > TravelCheckType.Mark;
        }

        public override void OnDeath(Mobile m)
        {
            if (VerLorRegController.Mobile != null && VerLorRegController.Mobile == m)
            {
                VerLorRegController.Stop();
                new ExitTimer(this).Start();
            }
        }
    }

    public class ExodusDungeonRegion : DungeonRegion
    {
        public ExodusDungeonRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        private static readonly Type[] m_Mobile = new Type[]
        {
            typeof(ExodusDrone), typeof(ExodusMinion), typeof(ExodusMinionLord), typeof(ExodusSentinel), typeof(ExodusOverseer),
            typeof(EnslavedGargoyle), typeof(ExodusZealot), typeof(ExodusJuggernaut), typeof(Golem), typeof(GolemController),
            typeof(GargoyleDestroyer), typeof(DupresChampion) , typeof(DupresKnight), typeof(DupresSquire) 
        };

        private static bool IsDropKeyMobile(BaseCreature bc)
        {
            return m_Mobile.Any(t => t == bc.GetType());
        }

        public override void OnDeath(Mobile m)
        {
            base.OnDeath(m);

            if (m is BaseCreature bc && IsDropKeyMobile(bc) && !bc.Controlled && Utility.RandomDouble() < 0.1)
            {
                Mobile killer = m.LastKiller;

                if (killer != null)
                {
                    if (killer is BaseCreature bct && bct.GetMaster() is PlayerMobile pm && bct.InRange(pm, 18))
                    {
                        killer = bct.GetMaster();
                    }

                    if (killer is PlayerMobile)
                    {
                        ExodusChest.GiveRituelItem(killer);
                    }
                }                
            }            
        }
    }
}
