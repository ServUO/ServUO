using Server.Mobiles;
using Server.Engines.RisingTide;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public class PlunderBeaconAddon : BaseAddon
    {
        public static readonly int MaxSpawn = 5;
        private static readonly string _TimerID = "PlunderBeacon";

        [CommandProperty(AccessLevel.GameMaster)]
        public PlunderBeacon Beacon { get; set; }

        public List<BaseCreature> Crew { get; set; }
        public Dictionary<BaseCreature, bool> Spawn { get; set; }
        public List<MannedCannon> Cannons { get; set; }

        public bool CannonsOperational => Crew.Any(c => c.Alive && !c.Deleted);
        public bool BeaconVulnerable => !CannonsOperational;

        public override BaseAddonDeed Deed => null;

        public DateTime NextShoot { get; set; }
        public DateTime NextSpawn { get; set; }
        public bool InitialSpawn { get; set; }

        [Constructable]
        public PlunderBeaconAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);

            AddComplexComponent(this, 2572, 0, 2, 37, 0, 5, "", 1);
            AddComplexComponent(this, 2567, 2, 0, 37, 0, 5, "", 1);

            Crew = new List<BaseCreature>();
            Spawn = new Dictionary<BaseCreature, bool>();
            Cannons = new List<MannedCannon>();

            Beacon = new PlunderBeacon(this);
            Beacon.MoveToWorld(new Point3D(X + 1, Y + 1, Z + 14), Map);

            AddCannon(Direction.South, CannonPower.Massive, -3, 5, 7);
            AddCannon(Direction.South, CannonPower.Massive, -1, 5, 7);
            AddCannon(Direction.South, CannonPower.Massive, 1, 5, 7);
            AddCannon(Direction.South, CannonPower.Massive, 3, 5, 7);

            AddCannon(Direction.North, CannonPower.Massive, -3, -4, 7);
            AddCannon(Direction.North, CannonPower.Massive, -1, -4, 7);
            AddCannon(Direction.North, CannonPower.Massive, 1, -4, 7);
            AddCannon(Direction.North, CannonPower.Massive, 3, -4, 7);

            AddCannon(Direction.West, CannonPower.Light, -2, -2, 12, false);
            AddCannon(Direction.West, CannonPower.Light, -2, 0, 12, false);
            AddCannon(Direction.West, CannonPower.Light, -2, 2, 12, false);

            AddCannon(Direction.East, CannonPower.Light, 2, -2, 12, false);
            AddCannon(Direction.East, CannonPower.Light, 2, 0, 12, false);
            AddCannon(Direction.East, CannonPower.Light, 2, 2, 12, false);
        }

        private void AddCannon(Direction d, CannonPower type, int xOffset, int yOffset, int zOffset, bool oper = true)
        {
            MannedCannon cannon;
            BaseCreature mob = null;

            if (oper)
            {
                mob = new PirateCrew
                {
                    CantWalk = true
                };

                Crew.Add(mob);
            }

            switch (type)
            {
                default:
                case CannonPower.Light:
                    cannon = new MannedCulverin(mob, d); break;
                case CannonPower.Heavy:
                    cannon = new MannedCarronade(mob, d); break;
                case CannonPower.Massive:
                    cannon = new MannedBlundercannon(mob, d); break;
            }

            if (mob == null)
            {
                cannon.CanFireUnmanned = true;
            }

            cannon.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, Z + zOffset), Map);
            Cannons.Add(cannon);

            if (mob != null)
            {
                Point3D offset;

                switch (d)
                {
                    default:
                    case Direction.South:
                        offset = new Point3D(0, -1, 0); break;
                    case Direction.North:
                        offset = new Point3D(0, 1, 0); break;
                    case Direction.West:
                        offset = new Point3D(1, 0, 0); break;
                    case Direction.East:
                        offset = new Point3D(-1, 0, 0); break;
                }

                mob.MoveToWorld(new Point3D(cannon.X + offset.X, cannon.Y + offset.Y, cannon.Z + offset.Z), Map);
            }
        }

        public override void OnLocationChange(Point3D old)
        {
            base.OnLocationChange(old);

            foreach (MannedCannon c in Cannons)
            {
                c.Location = new Point3D(X + (c.X - old.X), Y + (c.Y - old.Y), Z + (c.Z - old.Z));
            }

            foreach (BaseCreature c in Crew)
            {
                c.Location = new Point3D(X + (c.X - old.X), Y + (c.Y - old.Y), Z + (c.Z - old.Z));
            }

            foreach (BaseCreature c in Spawn.Keys.Where(c => c != null && !c.Deleted))
            {
                c.Location = new Point3D(X + (c.X - old.X), Y + (c.Y - old.Y), Z + (c.Z - old.Z));
            }

            if (Beacon != null)
            {
                Beacon.Location = new Point3D(X + (Beacon.X - old.X), Y + (Beacon.Y - old.Y), Z + (Beacon.Z - old.Z));
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            foreach (MannedCannon c in Cannons)
            {
                c.Map = Map;
            }

            foreach (BaseCreature c in Crew.Where(c => c != null && !c.Deleted))
            {
                c.Map = Map;
            }

            foreach (BaseCreature c in Spawn.Keys.Where(c => c != null && !c.Deleted))
            {
                c.Map = Map;
            }

            if (Beacon != null)
            {
                Beacon.Map = Map;
            }
        }

        public void OnBeaconDestroyed()
        {
            if (Deleted)
                return;

            for (int i = 0; i < 4; i++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(3 + (i * 3)), stage =>
                {
                    Z -= 1;

                    if (stage == 3)
                    {
                        Delete();
                    }
                }, i);
            }
        }

        public override void OnSectorActivate()
        {
            TimerRegistry.Register(_TimerID, this, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), false, addon => addon.OnTick());
        }

        public override void OnSectorDeactivate()
        {
            TimerRegistry.RemoveFromRegistry(_TimerID, this);
        }

        private bool _CheckSpawn;

        public void OnTick()
        {
            if (_CheckSpawn)
            {
                if (BaseCreature.IsSoulboundEnemies && Spawn != null)
                {
                    foreach (BaseCreature bc in Spawn.Keys)
                    {
                        if (!bc.Deleted)
                        {
                            bc.IsSoulBound = true;
                        }
                    }

                }

                _CheckSpawn = false;
            }

            Map map = Map;

            if (map == null)
            {
                return;
            }
            else if (!InitialSpawn)
            {
                for (int i = 0; i < MaxSpawn; i++)
                {
                    SpawnHelper(true);
                    InitialSpawn = true;
                }
            }
            else if (CannonsOperational && NextShoot < DateTime.UtcNow)
            {
                foreach (MannedCannon cannon in Cannons.Where(c => c != null && !c.Deleted && (c.CanFireUnmanned || (c.Operator != null && !c.Operator.Deleted && c.Operator.Alive))))
                {
                    cannon.Scan(true);
                }

                NextShoot = DateTime.UtcNow + TimeSpan.FromSeconds(2);
            }

            if (NextSpawn < DateTime.UtcNow)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    if (SpawnCount() < MaxSpawn)
                    {
                        SpawnHelper(false);
                    }
                });
            }
        }

        private void SpawnHelper(bool initial)
        {
            if (Map == null || Beacon == null)
                return;

            Point3D p = Location;
            Map map = Map;
            int range = 15;

            if (Beacon.LastDamager != null && Beacon.LastDamager.InRange(Location, 20))
            {
                p = Beacon.LastDamager.Location;
                range = 8;
            }

            BaseCreature creature = Activator.CreateInstance(_SpawnTypes[Utility.Random(_SpawnTypes.Length)]) as BaseCreature;

            for (int i = 0; i < 50; i++)
            {
                Point3D spawnLoc = new Point3D(Utility.RandomMinMax(p.X - range, p.X + range), Utility.RandomMinMax(p.Y - range, p.Y + range), -5);

                if (map.CanFit(spawnLoc.X, spawnLoc.Y, spawnLoc.Z, 16, true, true, false, creature))
                {
                    if (creature != null)
                    {
                        creature.MoveToWorld(spawnLoc, map);
                        creature.Home = spawnLoc;
                        creature.RangeHome = 10;

                        if (BaseCreature.IsSoulboundEnemies)
                            creature.IsSoulBound = true;

                        Spawn.Add(creature, initial);

                        NextSpawn = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));
                        return;
                    }
                }
            }

            creature.Delete();
        }

        private int SpawnCount()
        {
            return Spawn.Keys.Where(s => s != null && !s.Deleted).Count();
        }

        private readonly Type[] _SpawnTypes =
        {
            typeof(WaterElemental),
            typeof(SeaSerpent),
            typeof(DeepSeaSerpent)
        };

        public override void Delete()
        {
            base.Delete();

            if (Beacon != null && !Beacon.Deleted)
            {
                Beacon.Delete();
            }

            foreach (BaseCreature bc in Crew.Where(c => c != null && !c.Deleted))
            {
                bc.Kill();
            }

            foreach (BaseCreature bc in Spawn.Keys.Where(sp => sp != null && !sp.Deleted))
            {
                bc.Kill();
            }

            foreach (MannedCannon cannon in Cannons)
            {
                cannon.Delete();
            }

            if (PlunderBeaconSpawner.Spawner != null)
            {
                PlunderBeaconSpawner.Spawner.RemovePlunderBeacon(this);
            }
        }

        public bool Contains(IPoint3D p)
        {
            return p.X >= X - 8 && p.X <= X + 8 && p.Y >= Y - 8 && p.Y <= Y + 8;
        }

        public PlunderBeaconAddon(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // Version

            writer.Write(InitialSpawn);

            writer.WriteItem(Beacon);

            writer.WriteItemList(Cannons, true);
            writer.WriteMobileList(Crew, true);
            //writer.WriteMobileList(Spawn, true);

            writer.Write(Spawn.Count);

            foreach (KeyValuePair<BaseCreature, bool> kvp in Spawn)
            {
                writer.WriteMobile(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    InitialSpawn = reader.ReadBool();
                    goto case 0;
                case 0:
                    Beacon = reader.ReadItem<PlunderBeacon>();

                    Cannons = reader.ReadStrongItemList<MannedCannon>();
                    Crew = reader.ReadStrongMobileList<BaseCreature>();
                    Spawn = new Dictionary<BaseCreature, bool>();

                    if (version == 0)
                    {
                        //Spawn = reader.ReadStrongMobileList<BaseCreature>();
                        List<BaseCreature> list = reader.ReadStrongMobileList<BaseCreature>();

                        foreach (BaseCreature bc in list)
                        {
                            Spawn[bc] = true;
                        }
                    }
                    else
                    {
                        int count = reader.ReadInt();

                        for (int i = 0; i < count; i++)
                        {
                            BaseCreature bc = reader.ReadMobile<BaseCreature>();
                            bool initial = reader.ReadBool();

                            if (bc != null)
                            {
                                Spawn[bc] = initial;
                            }
                        }
                    }

                    break;
            }

            _CheckSpawn = true;
        }

        #region Components
        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null, 1);
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
        {
            AddonComponent ac;
            ac = new AddonComponent(item);
            if (!string.IsNullOrEmpty(name))
                ac.Name = name;
            if (hue != 0)
                ac.Hue = hue;
            if (amount > 1)
            {
                ac.Stackable = true;
                ac.Amount = amount;
            }
            if (lightsource != -1)
                ac.Light = (LightType)lightsource;
            addon.AddComponent(ac, xoffset, yoffset, zoffset);
        }

        private static readonly int[,] m_AddOnSimpleComponents = new int[,] {
              {16017, -5, -3, 4}, {16011, -2, 4, 4}// 1	 2	 3	 
			, {16011, -2, -4, 4}, {16020, -5, -5, 4}, {16008, -2, -5, 4}// 4	 5	 6	 
			, {16014, -4, -3, 4}, {16011, 3, -4, 4}, {16008, -2, 3, 4}// 7	 8	 9	 
			, {16021, -6, 4, 4}, {15998, 3, -3, 4}, {16016, -4, -5, 4}// 10	 11	 12	 
			, {16008, 1, 3, 4}, {16011, -1, -3, 4}, {16014, -4, 5, 4}// 13	 14	 15	 
			, {16011, 4, -4, 4}, {16011, 1, 5, 4}, {16011, 1, -4, 4}// 16	 17	 18	 
			, {16011, 2, 5, 4}, {16010, 0, -6, 4}, {16012, -3, -3, 4}// 19	 20	 21	 
			, {15996, 4, -3, 4}, {16011, 3, 4, 4}, {16010, 1, 2, 4}// 22	 23	 24	 
			, {15997, 4, -5, 4}, {16010, 0, 2, 4}// 25	 26	 27	 
			, {15993, 5, -3, 4}, {16010, 2, -6, 4}, {15996, 4, 5, 4}// 28	 29	 30	 
			, {16011, -3, -4, 4}, {16005, 1, -2, 4}, {16011, 0, -4, 4}// 31	 32	 33	 
			, {15998, 3, 5, 4}, {16011, 1, -3, 4}, {16005, 2, 6, 4}// 34	 35	 36	 
			, {16017, -5, 5, 4}, {16008, 2, 3, 4}, {16011, -3, 4, 4}// 37	 38	 39	 
			, {16008, 0, 3, 4}, {16010, 2, 2, 4}, {16010, -1, 2, 4}// 40	 41	 42	 
			, {16011, 2, -3, 4}, {16015, -4, -4, 4}, {16005, 2, -2, 4}// 43	 44	 45	 
			, {16010, -2, -6, 4}, {16008, 0, -5, 4}// 46	 47	 48	 
			, {16010, -2, 2, 4}, {16011, 4, 4, 4}// 49	 50	 51	 
			, {16013, -3, -5, 4}, {15997, 4, 3, 4}, {16008, -1, 3, 4}// 52	 53	 54	 
			, {16005, 0, -2, 4}, {15999, 3, -5, 4}, {16011, 0, 5, 4}// 55	 56	 57	 
			, {16011, 0, -3, 4}, {16005, 1, 6, 4}, {15993, 5, 5, 4}// 58	 59	 60	 
			, {15999, 3, 3, 4}, {16011, 0, 4, 4}// 61	 62	 63	 
			, {16011, 2, -4, 4}, {16005, 0, 6, 4}, {16010, 1, -6, 4}// 64	 65	 66	 
			, {16008, 2, -5, 4}, {16011, -2, -3, 4}// 67	 68	 69	 
			, {15995, 5, -5, 4}, {16015, -4, 4, 4}, {16011, 1, 4, 4}// 70	 71	 72	 
			, {16021, -6, -4, 4}, {16005, -2, 6, 4 } // 73	 74	 75	 
			, {16012, -3, 5, 4}, {16008, 1, -5, 4}, {16013, -3, 3, 4}// 76	 77	 78	 
			, {16011, 2, 4, 4}, {16016, -4, 3, 4}// 79	 80	 81	 
			, {15990, 6, 4, 4}, {16020, -5, 3, 4}, {15995, 5, 3, 4}// 82	 83	 84	 
			, {16011, -1, 5, 4}, {16011, -2, 5, 4}, {16008, -1, -5, 4}// 85	 86	 87	 
			, {15990, 6, -4, 4}, {5367, 3, 0, 4}, {2462, -1, 2, 17}// 88	 89	90	
			, {4014, 1, 1, 17}, {4014, 1, 2, 12} // 91	92	93	
			, {4014, 1, 1, 10}, {16933, 1, 2, 17}, {15991, 5, 5, 4}// 94	95	96	
			, {19341, 0, 2, 12}, {16035, -2, -1, 9}  // 97	98	99	
			, {16011, -1, 4, 4}, {4014, -1, 1, 12}, {4334, 4, 4, 7}// 100	101	102	
			, {4014, -1, 2, 12}, {16019, -5, 4, 4}, {16036, 2, 1, 9}// 104	105	106	
			, {16011, 1, -1, 9}, {16011, 1, 0, 9}, {16035, -2, 0, 9}// 107	108	109	
			, {30715, -1, 3, 5}, {30715, 0, 3, 5}, {16036, 2, -1, 9}// 110	111	112	
			, {16036, 2, 0, 9}, {16011, -1, -1, 9}// 113	114	115	
			, {16011, -1, 0, 9}, {16036, 2, 2, 9}, {16011, -1, 1, 9}// 116	117	118	
			, {4335, 2, 4, 7}, {16011, -1, 2, 9}, {16011, 0, -1, 9}// 119	120	121	
			, {16011, 0, 0, 9}, {16011, 0, 1, 9}, {7846, 3, -1, 0}// 122	123	124	
			, {6941, 3, 2, 35}, {16011, 0, 2, 9}, {16011, 1, 2, 9}// 125	126	127	
			, {6942, 1, 2, 24}, {30715, -2, 3, 5} // 128	129	130	
			, {7846, 3, 2, 0}, {16035, -2, 1, 9}, {16011, 1, 1, 9}// 131	132	133	
			, {30715, 2, 3, 5}, {4335, -3, 5, 7}, {30715, 1, 3, 5}// 134	135	136	
			, {16010, -1, 2, 7}, {16035, -2, 2, 9}, {16005, -1, 6, 4}// 137	138	139	
			, {16011, 5, 4, 8}, {16933, 0, -2, 17}, {4335, 4, -4, 7}// 140	141	142	
			, {30717, -2, -3, 5}, {15991, 5, -3, 4}, {30717, -1, -3, 5}// 143	144	145	
			, {30717, 0, -3, 5}, {16035, -2, -2, 9}, {30717, 2, -3, 5}// 146	147	148	
			, {4334, -2, -4, 7}, {16019, -5, -4, 4}, {4335, 3, -4, 7}// 149	150	151	
			, {30717, 1, -3, 5}, {16011, 1, -2, 9}, {16036, 2, -2, 9}// 152	153	154	
			, {4014, 0, -2, 12}, {16011, -1, -2, 9}// 155	156	157	
			, {16011, 0, -2, 9}, {16011, -1, -4, 4}, {16010, -1, -6, 4}// 158	159	160	
			, {16005, -1, -2, 7}, {16011, 5, -4, 8}// 161	162	
		};

        #endregion

        public static List<PlunderBeaconAddon> Beacons { get; set; }

        public static void AddBeacon(PlunderBeaconAddon beacon)
        {
            if (Beacons == null)
            {
                Beacons = new List<PlunderBeaconAddon>();
            }

            Beacons.Add(beacon);
        }

        public static void RemoveBeacon(PlunderBeaconAddon beacon)
        {
            if (Beacons != null && Beacons.Contains(beacon))
            {
                Beacons.Remove(beacon);
            }
        }

        public static void Initialize()
        {
            if (RisingTideEvent.Instance.Running)
            {
                EventSink.CreatureDeath += OnCreatureDeath;
            }
        }

        public static void OnCreatureDeath(CreatureDeathEventArgs e)
        {
            var killed = e.Creature as BaseCreature;

            if (killed != null && Beacons != null && Beacons.Any(b => b.Spawn != null && b.Spawn.ContainsKey(killed)))
            {
                double chance = killed is PirateCrew ? 0.15 : 0.025;

                if (chance >= Utility.RandomDouble())
                {
                    var m = killed.RandomPlayerWithLootingRights();

                    if (m != null)
                    {
                        m.AddToBackpack(new MaritimeCargo());
                        m.SendLocalizedMessage(1158907); // You recover maritime trade cargo!
                    }
                }
            }
        }
    }
}
