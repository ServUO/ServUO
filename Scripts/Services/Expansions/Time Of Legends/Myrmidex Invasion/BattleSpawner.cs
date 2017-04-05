using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Linq;
using System.Collections.Generic;
using Server.Commands;

namespace Server.Engines.MyrmidexInvasion
{
    public class BattleSpawner : Item
    {
        public const int MaxWaves = 2;
        public const int WaveCount = 15;
        public const int WaveDuration = 180;
        public const int MinCredit = 25;

        public static bool Active = true;
        public static BattleSpawner Instance { get; set; }

        private Rectangle2D MyrmidexLeg = new Rectangle2D(949, 1776, 59, 136);
        private Rectangle2D MiddleLeg = new Rectangle2D(889, 1776, 60, 136);
        private Rectangle2D TribeLeg = new Rectangle2D(832, 1776, 57, 136);

        public Rectangle2D Bounds = new Rectangle2D(828, 1762, 195, 158);

        private Rectangle2D _MyrmidexSpawnZone = new Rectangle2D(971, 1856, 31, 10);
        private Rectangle2D _TribeSpawnZone = new Rectangle2D(844, 1800, 42, 10);

        private Rectangle2D _MyrmidexObjective = new Rectangle2D(909, 1871, 10, 10);
        private Rectangle2D _TribalObjective = new Rectangle2D(908, 1782, 10, 10);

        public BattleRegion BattleRegion { get; set; }
        public Timer Timer { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BattleFlag MyrmidexFlag { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BattleFlag TribalFlag { get; set; }

        public Dictionary<int, List<BaseCreature>> MyrmidexTeam { get; set; }
        public Dictionary<int, List<BaseCreature>> TribeTeam { get; set; }

        public Dictionary<PlayerMobile, int> Players { get; set; }

        public DateTime LastPlayers { get; set; }
        public DateTime LastMyrmidexWave { get; set; }
        public DateTime LastTribeWave { get; set; }

        public BattleSpawner()
            : base(40106)
        {
            Visible = false;
            Movable = false;

            Instance = this;

            MyrmidexTeam = new Dictionary<int, List<BaseCreature>>();
            TribeTeam = new Dictionary<int, List<BaseCreature>>();
            Players = new Dictionary<PlayerMobile, int>();

            BattleRegion = Region.Regions.FirstOrDefault(r => r.GetType() == typeof(BattleRegion)) as BattleRegion;
            BattleRegion.Spawner = Instance;

            Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), OnTick);
            Timer.Start();
        }

        public static void Initialize()
        {
            if (Instance != null)
            {
                Instance.BattleRegion = Region.Regions.FirstOrDefault(r => r.GetType() == typeof(BattleRegion)) as BattleRegion;
                Instance.BattleRegion.Spawner = Instance;
            }

            CommandSystem.Register("CheckPoints", AccessLevel.GameMaster, e =>
            {
                if (Instance != null)
                    CheckPoints(e);
            });
        }

        public void OnTick()
        {
            if (MyrmidexFlag == null || TribalFlag == null)
                AssignFlags();

            CheckPlayers();
            CheckAdvance();
            CheckWaves();
        }

        public void CheckPlayers()
        {
            if (HasPlayers())
            {
                if (LastPlayers != DateTime.MinValue)
                    LastPlayers = DateTime.MinValue;

                if (LastMyrmidexWave + TimeSpan.FromSeconds(WaveDuration) < DateTime.UtcNow && MyrmidexTeam.Count < MaxWaves)
                    SpawnWave(Allegiance.Myrmidex);

                if (LastTribeWave + TimeSpan.FromSeconds(WaveDuration) < DateTime.UtcNow && TribeTeam.Count < MaxWaves)
                    SpawnWave(Allegiance.Tribes);
            }
            else if (LastPlayers == DateTime.MinValue)
            {
                LastPlayers = DateTime.UtcNow;
            }
            else if (LastPlayers + TimeSpan.FromMinutes(30) < DateTime.UtcNow)
            {
                Reset();
            }
        }

        public void CheckAdvance()
        {
            List<BaseCreature> list = GetAll(Allegiance.Myrmidex);
            IEnumerable<PlayerMobile> winners = null;

            foreach (BaseCreature bc in list.Where(b => IsInMyrmidexObjective(b.Location) && 0.25 > Utility.RandomDouble()))
            {
                ClearWave(Allegiance.Myrmidex, GetWave(MyrmidexTeam, bc));
                winners = GetPlayers(Allegiance.Myrmidex);
                RegionMessage(1156630); // The Myrmidex are victorious!  If you are allied to the Myrmidex visit the Tinker in the Barrab village to continue the quest!  Otherwise, continue the fight until your team is victorious!
                break;
            }

            if (winners == null)
            {
                ColUtility.Free(list);
                list = GetAll(Allegiance.Tribes);

                foreach (BaseCreature bc in list.Where(b => IsInTribalObjective(b.Location) && 0.25 > Utility.RandomDouble()))
                {
                    ClearWave(Allegiance.Tribes, GetWave(TribeTeam, bc));
                    winners = GetPlayers(Allegiance.Tribes);
                    RegionMessage(1156631); // The Eodonians are victorious!  If you are allied to the Eodonians visit the Tinker in the Barrab village to continue the quest!  Otherwise, continue the fight until your team is victorious!
                    break;
                }
            }

            if (winners != null)
            {
                ColUtility.ForEach(winners.Where(pm => Players.ContainsKey(pm) && Players[pm] > MinCredit), pm =>
                {
                    AllianceEntry entry = MyrmidexInvasionSystem.GetEntry(pm);

                    if (entry != null && !entry.CanRecieveQuest)
                        entry.CanRecieveQuest = true;

                    if (Players.ContainsKey(pm))
                        Players.Remove(pm);
                });
            }

            ColUtility.Free(list);
        }

        private void ClearWave(Allegiance allegiance, int wave)
        {
            if (wave < 0)
                return;

            Dictionary<int, List<BaseCreature>> list;

            if (allegiance == Allegiance.Myrmidex)
            {
                list = MyrmidexTeam;
            }
            else
            {
                list = TribeTeam;
            }

            if (list.ContainsKey(wave))
            {
                ColUtility.ForEach(list[wave].Where(bc => bc.Alive), bc => bc.Delete());
                ColUtility.Free(list[wave]);
                list.Remove(wave);
            }
        }

        private void Reset()
        {
            if (BattleRegion == null)
                return;

            IEnumerable<BaseCreature> list = BattleRegion.GetEnumeratedMobiles().OfType<BaseCreature>();

            foreach (BaseCreature bc in list.Where(b => b.Alive && !b.Controlled && !b.Summoned && b.GetMaster() == null))
            {
                bc.Kill();
            }

            foreach (KeyValuePair<int, List<BaseCreature>> kvp in MyrmidexTeam)
            {
                ColUtility.Free(kvp.Value);
            }

            foreach (KeyValuePair<int, List<BaseCreature>> kvp in TribeTeam)
            {
                ColUtility.Free(kvp.Value);
            }

            MyrmidexTeam.Clear();
            TribeTeam.Clear();
        }

        public void CheckWaves()
        {
            ColUtility.For<int, List<BaseCreature>>(MyrmidexTeam, (i, key, value) =>
            {
                if (value.Where(bc => bc != null && !bc.Deleted && bc.Alive).Count() == 0)
                {
                    ColUtility.Free(MyrmidexTeam[key]);
                    MyrmidexTeam.Remove(key);

                    RegionMessage(i == 0 ? 1156604 : 1156605); // The Eodonians have secured new ground, the front line has moved up!
                }
            });

            ColUtility.For<int, List<BaseCreature>>(TribeTeam, (i, key, value) =>
            {
                if (value.Where(bc => bc != null && !bc.Deleted && bc.Alive).Count() == 0)
                {
                    ColUtility.Free(TribeTeam[key]);
                    TribeTeam.Remove(key);

                    RegionMessage(i == 0 ? 1156602 : 1156603); // The Myrmidex have secured new ground, the front line has moved up!
                }
            });
        }

        public void SpawnWave(Allegiance allegiance)
        {
            if (allegiance == Allegiance.Myrmidex)
            {
                int wave = MyrmidexTeam.Count;
                MyrmidexTeam[wave] = new List<BaseCreature>();
                List<BaseCreature> list = MyrmidexTeam[wave];

                for (int i = 0; i < WaveCount; i++)
                {
                    BaseCreature bc;
                    Type type = _MyrmidexTypes[Utility.Random(_MyrmidexTypes.Length)];

                    if (type.IsSubclassOf(typeof(BaseEodonTribesman)))
                        bc = Activator.CreateInstance(type, new object[] { EodonTribe.Barrab }) as BaseCreature;
                    else
                        bc = Activator.CreateInstance(type) as BaseCreature;

                    bc.NoLootOnDeath = true;

                    if (bc != null)
                    {
                        for (int j = 0; j < 20; j++)
                        {
                            Point3D p = _MyrmidexSpawnZone.GetRandomSpawnPoint(Map.TerMur);

                            if (Map.TerMur.CanSpawnMobile(p.X, p.Y, p.Z))
                            {
                                bc.MoveToWorld(p, Map.TerMur);
                                list.Add(bc);

                                AssignNavpoints(bc, Allegiance.Myrmidex);
                                break;
                            }
                        }
                    }
                }

                LastMyrmidexWave = DateTime.UtcNow;
            }
            else
            {
                int wave = TribeTeam.Count;
                TribeTeam[wave] = new List<BaseCreature>();
                List<BaseCreature> list = TribeTeam[wave];

                for (int i = 0; i < WaveCount; i++)
                {
                    BaseCreature bc;
                    Type type = _TribeTypes[Utility.Random(_TribeTypes.Length)];

                    if (type.IsSubclassOf(typeof(BaseEodonTribesman)))
                    {
                        EodonTribe tribe = Utility.RandomList<EodonTribe>(EodonTribe.Jukari, EodonTribe.Kurak, EodonTribe.Barako, EodonTribe.Urali, EodonTribe.Sakkhra);
                        bc = Activator.CreateInstance(type, new object[] { tribe }) as BaseCreature;
                    }
                    else
                        bc = Activator.CreateInstance(type) as BaseCreature;

                    bc.NoLootOnDeath = true;

                    if (bc != null)
                    {
                        for (int j = 0; j < 20; j++)
                        {
                            Point3D p = _TribeSpawnZone.GetRandomSpawnPoint(Map.TerMur);

                            if (Map.TerMur.CanSpawnMobile(p.X, p.Y, p.Z))
                            {
                                bc.MoveToWorld(p, Map.TerMur);
                                list.Add(bc);

                                AssignNavpoints(bc, Allegiance.Tribes);
                                break;
                            }
                        }
                    }
                }

                LastTribeWave = DateTime.UtcNow;
            }
        }

        public void OnLeaveRegion(PlayerMobile pm)
        {
            if (Players.ContainsKey(pm))
                Players.Remove(pm);
        }

        public bool IsFrontLine(Mobile killer, BaseCreature bc)
        {
            if (bc == null)
                return false;

            Map map = bc.Corpse != null ? bc.Corpse.Map : bc.Map;
            Point3D p = bc.Corpse != null ? bc.Corpse.Location : bc.Location;

            if (map != null)
            {
                IPooledEnumerable eable = map.GetMobilesInRange(p, 12);

                foreach (Mobile m in eable)
                {
                    if (m != killer && IsSameLeg(bc, m) && MyrmidexInvasionSystem.IsEnemies(bc, m))
                    {
                        eable.Free();
                        return true;
                    }
                }

                eable.Free();
            }

            return false;
        }

        public IEnumerable<PlayerMobile> GetPlayers(Allegiance allegiance)
        {
            if (BattleRegion == null)
                return null;

            return BattleRegion.GetEnumeratedMobiles().OfType<PlayerMobile>().Where(p => MyrmidexInvasionSystem.IsAlliedWith(p, allegiance));
        }

        public bool HasPlayers(bool ignorestaff = true)
        {
            if (BattleRegion == null)
                return false;

            return BattleRegion.GetEnumeratedMobiles().Where(m => m is PlayerMobile && (!ignorestaff || m.AccessLevel == AccessLevel.Player)).Count() > 0;
        }

        public bool IsInMyrmidexObjective(Point3D p)
        {
            return _MyrmidexObjective.Contains(p);
        }

        public bool IsInTribalObjective(Point3D p)
        {
            return _TribalObjective.Contains(p);
        }

        public void RegionMessage(int message)
        {
            ColUtility.ForEach(BattleRegion.GetEnumeratedMobiles().OfType<PlayerMobile>(), pm => pm.SendLocalizedMessage(message));
        }

        public bool IsSameLeg(IPoint2D p1, IPoint2D p2)
        {
            return (MyrmidexLeg.Contains(p1) && MyrmidexLeg.Contains(p2)) ||
                   (MiddleLeg.Contains(p1) && MiddleLeg.Contains(p2)) ||
                   (TribeLeg.Contains(p1) && TribeLeg.Contains(p2));
        }

        public int GetWave(Dictionary<int, List<BaseCreature>> list, BaseCreature bc)
        {
            foreach (KeyValuePair<int, List<BaseCreature>> kvp in list)
            {
                if (kvp.Value.Contains(bc))
                    return kvp.Key;
            }

            return -1;
        }

        public List<BaseCreature> GetAll(Allegiance allegiance)
        {
            Dictionary<int, List<BaseCreature>> list;

            if (allegiance == Allegiance.Myrmidex)
                list = MyrmidexTeam;
            else
                list = TribeTeam;

            List<BaseCreature> bclist = new List<BaseCreature>();

            ColUtility.ForEach(list, (key, value) =>
            {
                bclist.AddRange(value.Where(bc => bc != null && !bc.Deleted && bc.Alive));
            });

            return bclist;
        }

        public void RegisterDeath(BaseCreature bc)
        {
            List<DamageStore> rights = bc.GetLootingRights();

            ColUtility.ForEach(rights.Where(ds => ds.m_Mobile is PlayerMobile && ds.m_HasRight && MyrmidexInvasionSystem.IsEnemies(ds.m_Mobile, bc)), ds =>
            {
                if (MyrmidexInvasionSystem.IsAlliedWith(bc, Allegiance.Myrmidex))
                {
                    int points = 1;
                    if (IsFrontLine(ds.m_Mobile, bc))
                    {
                        ds.m_Mobile.SendLocalizedMessage(1156599); // You assist the Eodonians in pushing back the Myrmidex!
                        points *= 4;
                    }
                    else
                        ds.m_Mobile.SendLocalizedMessage(1156600); // You kill one of the Myrmidex away from the front ranks and gain little recognition.

                    if (!Players.ContainsKey((PlayerMobile)ds.m_Mobile))
                        Players[(PlayerMobile)ds.m_Mobile] = points;
                    else
                        Players[(PlayerMobile)ds.m_Mobile] += points;
                }
                else
                {
                    int points = 1;
                    if (IsFrontLine(ds.m_Mobile, bc))
                    {
                        ds.m_Mobile.SendLocalizedMessage(1156598); // You assist the Myrmidex in pushing back the Eodonians!
                        points *= 4;
                    }
                    else
                        ds.m_Mobile.SendLocalizedMessage(1156601); // You kill one of the Eodonians away from the front ranks and gain little recognition.

                    if (!Players.ContainsKey((PlayerMobile)ds.m_Mobile))
                        Players[(PlayerMobile)ds.m_Mobile] = points;
                    else
                        Players[(PlayerMobile)ds.m_Mobile] += points;
                }
            });
        }

        private Type[] _MyrmidexTypes =
        {
            typeof(MyrmidexDrone), typeof(MyrmidexWarrior), typeof(TribeWarrior), typeof(TribeShaman)
        };

        private Type[] _TribeTypes =
        {
            typeof(BritannianInfantry)
        };

        public void AssignNavPoints()
        {
            int myrcount = 0;
            int trcount = 0;

            ColUtility.ForEach<int, List<BaseCreature>>(MyrmidexTeam, (key, value) => myrcount += value.Count);
            ColUtility.ForEach<int, List<BaseCreature>>(TribeTeam, (key, value) => trcount += value.Count);

            ColUtility.ForEach<int, List<BaseCreature>>(MyrmidexTeam, (key, value) =>
            {
                value.ForEach(bc => AssignNavpoints(bc, Allegiance.Myrmidex));
            });

            ColUtility.ForEach<int, List<BaseCreature>>(TribeTeam, (key, value) =>
            {
                value.ForEach(bc => AssignNavpoints(bc, Allegiance.Tribes));
            });
        }

        public bool AssignNavpoints(BaseCreature bc, Allegiance allegiance)
        {
            int lane = 0;
            int leg = GetLeg(new Point2D(bc.X, bc.Y), out lane, allegiance);

            if (leg == -1)
            {
                bc.Kill();
                return false;
            }

            Point2D[][][] uselist = allegiance == Allegiance.Myrmidex ? _NavPoints2 : _NavPoints1;
            Point2D[] list = uselist[lane][leg];

            int index = 0;
            Point2D nearest = GetNearest(list, bc, out index);

            if (nearest != Point2D.Zero)
            {
                List<Point2D> points = new List<Point2D>();
                points.AddRange(uselist[lane][0]);
                points.AddRange(uselist[lane][1]);

                if (allegiance == Allegiance.Myrmidex)
                    points.Reverse();

                bc.CurrentNavPoint = Array.IndexOf(points.ToArray(), nearest);

                //Randomize
                points.ForEach(point =>
                {
                    if (lane == 0)
                    {
                        point.X = point.X + (Utility.RandomMinMax(0, 2));
                    }
                    else if (lane == 3)
                    {
                        point.X = point.X - (Utility.RandomMinMax(0, 2));
                    }
                    else
                    {
                        point.X = point.X + (Utility.RandomMinMax(-2, 2));
                    }
                });

                bc.NavPoints[Map.TerMur] = points;
                bc.AIObject.Activate();

                bc.PassiveSpeed = .2;
                bc.ActiveSpeed = .15;

                return true;
            }
            else
                bc.Delete();

            return false;
        }

        private int GetLeg(Point2D p, out int lane, Allegiance allegiance)
        {
            bool tribe = allegiance == Allegiance.Tribes;

            int leg;
            Rectangle2D rec;

            if (TribeLeg.Contains(p))
            {
                leg = tribe ? 0 : -1;
                rec = TribeLeg;
            }
            else if (MiddleLeg.Contains(p))
            {
                leg = tribe ? 1 : 0;
                rec = MiddleLeg;
            }
            else
            {
                leg = tribe ? -1 : 1;
                rec = MyrmidexLeg;
            }

            int div = rec.Width / 4;

            if (p.X <= rec.X + div)
                lane = 0;
            else if (p.X <= rec.X + (div * 2))
                lane = 1;
            else if (p.X <= rec.X + (div * 3))
                lane = 2;
            else
                lane = 3;

            return leg;
        }

        private Point2D GetNearest(Point2D[] list, Mobile m, out int index)
        {
            double closest = 20;
            index = 0;
            Point2D p = Point2D.Zero;

            for (int i = 0; i < list.Length; i++)
            {
                Point2D current = list[i];

                double dist = m.GetDistanceToSqrt(current);

                if (dist <= closest)
                {
                    p = current;
                    closest = dist;
                    index = i;

                    if (closest <= 2)
                    {
                        return p;
                    }
                }
            }

            return p;
        }

        public static void CheckPoints(CommandEventArgs e)
        {
            int hue = 1160;
            foreach (Point2D[][] ps in _NavPoints1)
            {
                hue++;
                foreach (Point2D[] pss in ps)
                {
                    foreach (Point2D p in pss)
                    {
                        var st = new Static(14089);
                        st.Hue = hue;
                        st.MoveToWorld(new Point3D(p.X, p.Y, Map.TerMur.GetAverageZ(p.X, p.Y)), Map.TerMur);
                    }
                }
            }

            foreach (Point2D[][] ps in _NavPoints2)
            {
                hue--;
                foreach (Point2D[] pss in ps)
                {
                    foreach (Point2D p in pss)
                    {
                        var st = new Static(14089);
                        st.Hue = hue;
                        st.MoveToWorld(new Point3D(p.X, p.Y, Map.TerMur.GetAverageZ(p.X, p.Y)), Map.TerMur);
                    }
                }
            }
        }

        private static Point2D[][][] _NavPoints1 =
        {       
            //Lane 1                                                                                                                                               
            new Point2D[][] { new Point2D[] { new Point2D(853, 1785), new Point2D(853, 1800), new Point2D(853, 1815), new Point2D(855, 1830), new Point2D(855, 1845), new Point2D(855, 1860), new Point2D(855, 1875), /*shares with next*/ new Point2D(860, 1883), new Point2D(872, 1887), new Point2D(891, 1887) },
                              new Point2D[] { new Point2D(903, 1875), new Point2D(903, 1860), new Point2D(903, 1845), new Point2D(903, 1830), new Point2D(903, 1815), /*shares with next*/ new Point2D(903, 1807), new Point2D(905, 1797), new Point2D(906, 1787) } },

            //Lane 2
            new Point2D[][] { new Point2D[] { new Point2D(859, 1785), new Point2D(859, 1800), new Point2D(859, 1815), new Point2D(859, 1830), new Point2D(859, 1845), new Point2D(859, 1860), new Point2D(859, 1875), /*shares with prev*/ new Point2D(860, 1883), new Point2D(872, 1887), new Point2D(891, 1887) },
                              new Point2D[] { new Point2D(909, 1875), new Point2D(909, 1860), new Point2D(909, 1845), new Point2D(909, 1830), new Point2D(909, 1815), /*shares with prev*/ new Point2D(909, 1807), new Point2D(910, 1797), new Point2D(912, 1787) } },
            
            //Lance 3
            new Point2D[][] { new Point2D[] { new Point2D(865, 1785), new Point2D(865, 1800), new Point2D(865, 1815), new Point2D(865, 1830), new Point2D(865, 1845), new Point2D(865, 1860), new Point2D(865, 1875), /*shares with next*/ new Point2D(872, 1880), new Point2D(879, 1887), new Point2D(891, 1887), new Point2D(903, 1885) },
                              new Point2D[] { new Point2D(915, 1875), new Point2D(915, 1860), new Point2D(915, 1845), new Point2D(915, 1830), new Point2D(915, 1815), /*shares with next*/ new Point2D(915, 1807), new Point2D(915, 1797), new Point2D(915, 1787) } },

            // Lane 4
            new Point2D[][] { new Point2D[] { new Point2D(871, 1785), new Point2D(871, 1800), new Point2D(871, 1815), new Point2D(871, 1830), new Point2D(871, 1845), new Point2D(871, 1860), new Point2D(871, 1875), /*shares with prev*/ new Point2D(872, 1880), new Point2D(879, 1887), new Point2D(891, 1887), new Point2D(903, 1885), new Point2D(912, 1882) },
                              new Point2D[] { new Point2D(921, 1875), new Point2D(921, 1860), new Point2D(921, 1845), new Point2D(921, 1830), new Point2D(921, 1815), /*shares with prev*/ new Point2D(921, 1807), new Point2D(921, 1797), new Point2D(921, 1787) } }
        };

        private static Point2D[][][] _NavPoints2 =
        {       
            //Lane 1                                                                                                                                               
            new Point2D[][] { 
                              new Point2D[] { new Point2D(906, 1877), new Point2D(904, 1860), new Point2D(903, 1845), new Point2D(903, 1830), new Point2D(903, 1815), /*shares with next*/ new Point2D(909, 1807), new Point2D(921, 1807), new Point2D(935, 1801), new Point2D(950, 1797), new Point2D(959, 1798), new Point2D(973, 1802) },
                              new Point2D[] { new Point2D(973, 1815), new Point2D(973, 1830), new Point2D(973, 1845), new Point2D(973, 1860), new Point2D(973, 1875) } },

            //Lane 2
            new Point2D[][] { 
                              new Point2D[] { new Point2D(909, 1877), new Point2D(909, 1860), new Point2D(909, 1845), new Point2D(909, 1830), new Point2D(909, 1815), /*shares with prev*/ new Point2D(909, 1807), new Point2D(921, 1807), new Point2D(935, 1801), new Point2D(950, 1797), new Point2D(959, 1798), new Point2D(973, 1802), new Point2D(979, 1805) },
                              new Point2D[] { new Point2D(979, 1820), new Point2D(979, 1820), new Point2D(979, 1835), new Point2D(979, 1850), new Point2D(979, 1865), new Point2D(979, 1880) } },
            
            //Lance 3
            new Point2D[][] { 
                              new Point2D[] { new Point2D(915, 1877), new Point2D(915, 1860), new Point2D(915, 1845), new Point2D(915, 1830), new Point2D(915, 1815), /*shares with next*/ new Point2D(921, 1807), new Point2D(935, 1801), new Point2D(950, 1797), new Point2D(959, 1798), new Point2D(973, 1802), new Point2D(979, 1805), new Point2D(985, 1809) },
                              new Point2D[] { new Point2D(985, 1825), new Point2D(985, 1840), new Point2D(985, 1855), new Point2D(985, 1870), new Point2D(985, 1885) } },

            // Lane 4
            new Point2D[][] { 
                              new Point2D[] { new Point2D(919, 1877), new Point2D(920, 1860), new Point2D(921, 1845), new Point2D(921, 1830), new Point2D(921, 1815), /*shares with prev*/ new Point2D(921, 1807), new Point2D(935, 1801), new Point2D(950, 1797), new Point2D(959, 1798), new Point2D(973, 1802), new Point2D(979, 1805), new Point2D(985, 1809), new Point2D(991, 1813) },
                              new Point2D[] { new Point2D(991, 1830), new Point2D(991, 1845), new Point2D(991, 1860), new Point2D(991, 1875), new Point2D(991, 1885) } }
        };

        private void AssignFlags()
        {
            if (BattleRegion != null)
            {
                BattleFlag flag = BattleRegion.GetEnumeratedItems().OfType<BattleFlag>().FirstOrDefault(i => i != null && i.ItemID == 17099);
                if (flag != null)
                {
                    TribalFlag = flag;
                    flag.BattleSpawner = this;
                }

                flag = BattleRegion.GetEnumeratedItems().OfType<BattleFlag>().FirstOrDefault(i => i != null && i.ItemID == 1068);
                if (flag != null)
                {
                    MyrmidexFlag = flag;
                    flag.BattleSpawner = this;
                }
            }
        }

        public BattleSpawner(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(MyrmidexTeam.Count);
            foreach (KeyValuePair<int, List<BaseCreature>> kvp in MyrmidexTeam)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.Count);
                kvp.Value.ForEach(bc => writer.Write(bc));
            }

            writer.Write(TribeTeam.Count);
            foreach (KeyValuePair<int, List<BaseCreature>> kvp in TribeTeam)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.Count);
                kvp.Value.ForEach(bc => writer.Write(bc));
            }

            writer.Write(Players.Count);
            foreach (KeyValuePair<PlayerMobile, int> kvp in Players)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            Instance = this;

            MyrmidexTeam = new Dictionary<int, List<BaseCreature>>();
            TribeTeam = new Dictionary<int, List<BaseCreature>>();
            Players = new Dictionary<PlayerMobile, int>();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int wave = reader.ReadInt();
                int c = reader.ReadInt();

                MyrmidexTeam[wave] = new List<BaseCreature>();

                for (int j = 0; j < c; j++)
                {
                    BaseCreature bc = reader.ReadMobile() as BaseCreature;

                    if (bc != null)
                    {
                        MyrmidexTeam[wave].Add(bc);
                        //AssignNavpoints(bc, Allegiance.Myrmidex);
                    }
                }
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int wave = reader.ReadInt();
                int c = reader.ReadInt();

                TribeTeam[wave] = new List<BaseCreature>();

                for (int j = 0; j < c; j++)
                {
                    BaseCreature bc = reader.ReadMobile() as BaseCreature;

                    if (bc != null)
                    {
                        TribeTeam[wave].Add(bc);
                        //AssignNavpoints(bc, Allegiance.Tribes);
                    }
                }
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                PlayerMobile pm = reader.ReadMobile() as PlayerMobile;
                int score = reader.ReadInt();

                if (pm != null)
                    Players[pm] = score;
            }

            if (Active)
            {
                Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), OnTick);
                Timer.Start();

                Timer.DelayCall(TimeSpan.FromSeconds(10), AssignNavPoints);
            }
            else
            {
                MyrmidexTeam.Clear();
                TribeTeam.Clear();
                Players.Clear();
            }

            if (v == 0)
                Timer.DelayCall(TimeSpan.FromSeconds(20), FixFlags);
        }

        private void FixFlags()
        {
            if (BattleRegion != null)
            {
                foreach (Item item in BattleRegion.GetEnumeratedItems())
                {
                    if (item is Static && (item.ItemID == 9))
                    {
                        item.Delete();
                    }
                    else if (item is BattleFlag)
                    {
                        item.Delete();
                    }
                }
            }

            Item st = new Static(0xA1F);
            st.MoveToWorld(new Point3D(913, 1871, 0), Map.TerMur);

            st = new Static(0xA1F);
            st.MoveToWorld(new Point3D(914, 1871, 0), Map.TerMur);

            BattleFlag bflag = new BattleFlag(0x42CB, 0);
            bflag.MoveToWorld(new Point3D(914, 1872, 5), Map.TerMur);

            st = new Static(0xA1F);
            st.MoveToWorld(new Point3D(913, 1792, 0), Map.TerMur);

            bflag = new BattleFlag(0x42C, 2520);
            bflag.MoveToWorld(new Point3D(914, 1793, 6), Map.TerMur);

            bflag = new BattleFlag(0x42D, 2520);
            bflag.MoveToWorld(new Point3D(913, 1793, 6), Map.TerMur);
        }
    }
}