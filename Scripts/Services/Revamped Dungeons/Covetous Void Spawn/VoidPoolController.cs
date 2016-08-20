using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using System.Linq;
using Server.Engines.Points;
using Server.Regions;
using System.Text.RegularExpressions;

namespace Server.Engines.VoidPool
{
	public enum VoidType
	{
		Abyss,
		Repond, 
		Undead,
		Reptile,
		Elemental
	}

	public class VoidPoolController : Item
	{
		public static VoidPoolController InstanceTram { get; set; }
		public static VoidPoolController InstanceFel { get; set; }

        private readonly int RestartSpan = 15;
		private readonly int StartMessage = 5;
		private readonly int PoolStartHits = 15;
		private readonly int StartPointVariance = 8;
		
		private readonly Point3D StartPoint1 = new Point3D(5592, 2012, 0);
		private readonly Point3D StartPoint2 = new Point3D(5466, 2007, 0);
		
		private readonly Point3D EndPoint = new Point3D(5500, 1998, 5);
		private readonly Rectangle2D PoolWalls = new Rectangle2D(5495, 1993, 10, 10);
		
		private bool _Active;

        [CommandProperty(AccessLevel.GameMaster)]
		public bool Active 
		{ 
			get { return _Active; } 
			set 
			{
				if(!value)
				{
					if(Timer != null)
					{
						Timer.Stop();
						Timer = null;
					}

                    if (Region != null)
                    {
                        Region.Unregister();
                        Region = null;
                    }
				}
				else
				{
                    if (Region == null)
                    {
                        Region = new VoidPoolRegion(this, this.Map);
                        Region.Register();
                    }

					if(Timer == null)
					{
						Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
						Timer.Start();
						
						NextStart = DateTime.UtcNow + TimeSpan.FromMinutes(RestartSpan);

                        if(Region != null)
						    Region.SendRegionMessage(1152526, RestartSpan.ToString()); // The battle for the Void Pool will begin in ~1_VALUE~ minutes.
					}
				}
				
				_Active = value; 
			} 
		}
		
        [CommandProperty(AccessLevel.GameMaster)]
		public int Wave { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
		public int Stage 
        { 
            get 
            {
                if (Wave < 2)
                    return 0;
                //TODO: Make this like EA?
                return Math.Max(1, Wave / 5); 
            } 
        }
		
		public List<WaveInfo> Waves { get; set; }
        public List<WayPoint> WaypointsA { get; set; }
        public List<WayPoint> WaypointsB { get; set; }
        public int WaypointACount { get; set; }
        public int WaypointBCount { get; set; }

		public VoidPoolRegion Region { get; set; }
		public Timer Timer { get; set; }
		public Dictionary<Mobile, long> CurrentScore { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
		public bool OnGoing { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
		public VoidType VoidType { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
		public DateTime NextStart { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
		public DateTime NextWave { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
		public int PoolHits { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RespawnMin { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int RespawnMax { get; set; }

        public VoidPoolController(Map map)
            : base(3803)
		{
            Name = "Void Pool Controller";
			Visible = false;
			Movable = false;

            PoolHits = PoolStartHits;
			
			if(map == Map.Trammel)
			{
				if(InstanceTram != null)
					Delete();
				else
					InstanceTram = this;
			}
			else if(map == Map.Felucca)
			{
				if(InstanceFel != null)
					Delete();
				else
					InstanceFel = this;
			}
			else
				Delete();

            WaypointsA = new List<WayPoint>();
            WaypointsB = new List<WayPoint>();

            Region = new VoidPoolRegion(this, map);
            Region.Register();

            RespawnMin = 60;
            RespawnMax = 90;

            ClearSpawners();
			Active = true;
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                from.SendGump(new Server.Gumps.PropertiesGump(from, this));
        }
		
		private void OnTick()
		{
			if(!OnGoing && DateTime.UtcNow > NextStart && Region != null && Region.GetPlayerCount() > 0)
			{
				NextStart = DateTime.MaxValue;
				OnGoing = true;
				VoidType = (VoidType)Utility.Random(5);
                PoolHits = PoolStartHits;
				Wave = 0;

                if (CurrentScore != null)
                    CurrentScore.Clear();

                if (Waves != null)
                {
                    Waves.Clear();
                    Waves.TrimExcess();
                }

				CurrentScore = new Dictionary<Mobile, long>();
				Waves = new List<WaveInfo>();
				
				Region.SendRegionMessage(1152527); // The battle for the Void Pool is beginning now!

                if (WaypointACount != WaypointsA.Count || WaypointBCount != WaypointsB.Count)
                    Generate.AddWaypoints();

				SpawnWave();
			}
			else if(OnGoing)
			{
				if(DateTime.UtcNow > NextWave)
				{
					SpawnWave();
				}
				
				IPooledEnumerable eable = this.Map.GetMobilesInBounds(PoolWalls);
				foreach(Mobile m in eable)
				{
                    if (!OnGoing)
                        break;

					if(m is BaseCreature && !((BaseCreature)m).Controlled && !((BaseCreature)m).Summoned && Utility.RandomDouble() > 0.25)
						OnVoidWallDamaged(m);
				}
				eable.Free();
			}
		}
		
		public void SpawnWave()
		{
			Wave++;
			
			Region.SendRegionMessage(1152528, Wave.ToString());

            int toSpawn = (int)Math.Ceiling(Math.Max(5, Math.Sqrt(Wave) * 2) * 1.5);
			List<BaseCreature> creatures = new List<BaseCreature>();

			for(int i = 0; i < toSpawn; i++)
			{
				Point3D start = i % 2 == 0 ? StartPoint1 : StartPoint2;
				
				for(int j = 0; j < 25; j++)
				{
					int x = start.X + Utility.RandomMinMax(start.X - (StartPointVariance / 2), start.X + (StartPointVariance / 2));
					int y = start.Y + Utility.RandomMinMax(start.Y - (StartPointVariance / 2), start.Y + (StartPointVariance / 2));
					int z = this.Map.GetAverageZ(x, y);
					
					if(this.Map.CanSpawnMobile(x, y, z))
					{
						start = new Point3D(x, y, z);
						break;
					}
				}
			
				int ran = Utility.RandomMinMax(0, Stage < 10 ? 12 : Stage < 15 ? 14 : 15);
				Type t;
				
				switch(ran)
				{
					default:
					case 0: case 1: case 3: case 4: t = SpawnTable[(int)VoidType][0]; break;
					case 5: case 6: case 7: case 8: t = SpawnTable[(int)VoidType][1]; break;
					case 9: case 10: case 11: t = SpawnTable[(int)VoidType][2]; break;
					case 12: case 13: t = SpawnTable[(int)VoidType][3]; break;
					case 14: case 15: t = SpawnTable[(int)VoidType][4]; break;
				}
				
				BaseCreature bc = Activator.CreateInstance(t,  Wave, true) as BaseCreature;
				
				if(bc != null)
				{
					Timer.DelayCall(TimeSpan.FromSeconds((double)i * .75), () =>
					{
                        if (OnGoing)
                        {
                            bc.MoveToWorld(start, this.Map);
                            bc.Home = EndPoint;
                            bc.RangeHome = 1;

                            creatures.Add(bc);

                            bc.CurrentWayPoint = GetNearestWaypoint(bc);
                        }
                        else
                            bc.Delete();
					});
				}
			}
			
			Waves.Add(new WaveInfo(Wave, creatures));
			NextWave = GetNextWaveTime();
		}

        public WayPoint GetNearestWaypoint(Mobile m, int range = 15)
        {
            IPooledEnumerable eable = this.Map.GetItemsInRange(m.Location, range);

            int closestRange = 15;
            WayPoint closest = null;

            foreach (Item item in eable)
            {
                int dist = 0;
                if (item is WayPoint)
                {
                    dist = (int)m.GetDistanceToSqrt(item);

                    if (dist < closestRange || closest == null)
                    {
                        closest = item as WayPoint;
                        closestRange = dist;
                    }
                }
            }

            return closest;
        }

        public Item GetNearestVoidPoolWall(Mobile m)
        {
            IPooledEnumerable eable = this.Map.GetItemsInRange(m.Location, 5);

            int closestRange = 5;
            Item closest = null;

            foreach (Item item in eable)
            {
                int dist = 0;
                if (item.Name == "Void Pool")
                {
                    dist = (int)m.GetDistanceToSqrt(item);

                    if (dist < closestRange || closest == null)
                    {
                        closest = item;
                        closestRange = dist;
                    }
                }
            }

            return closest;
        }
		
		public DateTime GetNextWaveTime()
		{
			if(Wave == 1)
				return DateTime.UtcNow + TimeSpan.FromSeconds(10);
				
			int min = Math.Max(30, RespawnMin - Wave) + Utility.RandomMinMax(0, 10);
			int max = Math.Max(45, RespawnMax - Wave) - Utility.RandomMinMax(0, 5);
			
			return DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(min, max));
		}
		
		public void OnVoidWallDamaged(Mobile damager)
		{
            if(0.5 > Utility.RandomDouble())
			    PoolHits--;

			Region.SendRegionMessage(1152529); // The Void Pool walls have been damaged! Defend the Void Pool!

            Item item = GetNearestVoidPoolWall(damager);

            if (item != null)
            {
                Point3D p = new Point3D(item.X, item.Y, item.Z + 5);
                Effects.SendLocationParticles(EffectItem.Create(p, item.Map, EffectItem.DefaultDuration), Utility.RandomList(0x36BD, 0x36B0, 0x3728), 20, 10, 5044);
                Effects.PlaySound(p, item.Map, 0x307);
            }

            if (PoolHits <= 0 && OnGoing)
            {
                OnGoing = false;
                EndInvasion();
            }
		}
		
		public void EndInvasion()
		{
			Region.SendRegionMessage(1152530); // Cora's forces have destroyed the Void Pool walls. The battle is lost!
	
            VoidPoolStats.OnInvasionEnd(CurrentScore, Wave);
			
			NextStart = DateTime.UtcNow + TimeSpan.FromMinutes(RestartSpan);
			
			Region.SendRegionMessage(1152526, RestartSpan.ToString()); // The battle for the Void Pool will begin in ~1_VALUE~ minutes.
			
			List<Mobile> list = Region.GetPlayers();

            foreach (Mobile m in list.Where(m => GetCurrentPoints(m) > 0))
                PointsSystem.VoidPool.AwardPoints(m, GetCurrentPoints(m));
				
			foreach(Mobile m in list.Where(m => CurrentScore.ContainsKey(m)))
            {
                m.SendLocalizedMessage(1152650, String.Format("{0}\t{1}\t{2}\t{3}", GetTotalWaves(m), Wave.ToString(), Wave.ToString(), CurrentScore[m])); 
				// During the battle, you helped fight back ~1_COUNT~ out of ~2_TOTAL~ waves of enemy forces. Your final wave was ~3_MAX~. Your total score for the battle was ~4_SCORE~ points.
            }

            list.Clear();
            list.TrimExcess();
            ClearSpawn(true);
		}
		
		public void OnCreatureKilled(BaseCreature killed)
		{
            if (Waves == null)
                return;

            Waves.ForEach(info =>
            {
                if (info.Creatures.Contains(killed))
                {
                    List<DamageStore> list = killed.GetLootingRights();
                    list.Sort();

                    for (int i = 0; i < list.Count; i++)
                    {
                        DamageStore ds = list[i];
                        Mobile m = ds.m_Mobile;

                        if (ds.m_Mobile is BaseCreature && ((BaseCreature)ds.m_Mobile).GetMaster() is PlayerMobile)
                            m = ((BaseCreature)ds.m_Mobile).GetMaster();

                        if (!info.Credit.Contains(m))
                            info.Credit.Add(m);

                        if (!CurrentScore.ContainsKey(m))
                            CurrentScore[m] = killed.Fame / 998;
                        else
                            CurrentScore[m] += killed.Fame / 998;
                    }

                    list.Clear();
                    list.TrimExcess();

                    info.Creatures.Remove(killed);

                    if (info.Creatures.Count == 0)
                    {
                        foreach (Mobile m in info.Credit.Where(m => m.Region == this.Region && m is PlayerMobile))
                        {
                            double award = Math.Max(0, this.Map == Map.Felucca ? Stage * 2 : Stage);

                            if (award > 0)
                            {
                                //Score Bonus
                                if (!CurrentScore.ContainsKey(m))
                                    CurrentScore[m] = Stage * 125;
                                else
                                    CurrentScore[m] += Stage * 125;
                            }
                        }
                    }

                    if (killed.Corpse != null && !killed.Corpse.Deleted)
                        ((Corpse)killed.Corpse).BeginDecay(TimeSpan.FromMinutes(1));
                }
            });
		}

        public void ClearSpawners()
        {
            if (Region == null)
                return;

            foreach (Item item in Region.GetEnumeratedItems().Where(i => i is XmlSpawner))
            {
                ((XmlSpawner)item).DoReset = true;
            }
        }

        public void ClearSpawn()
        {
            ClearSpawn(false);
        }

        public void ClearSpawn(bool effects)
        {
            List<Mobile> list = Region.GetMobiles();

            foreach (Mobile m in list.Where(m => m is CovetousCreature))
            {
                if (effects)
                    Effects.SendLocationEffect(m.Location, m.Map, 0xDDA, 30, 10, 0, 0);

                m.Delete();
            }
            list.Clear();
            list.TrimExcess();
        }

		public int GetCurrentPoints(Mobile from)
		{
            if (Waves == null)
                return 0;

			int points = 0;

            foreach (var info in Waves.Where(i => i.Credit.Contains(from)))
            {
                points += this.Map == Map.Felucca ? Stage * 2 : Stage;
            }
		
			return points;
		}
		
		public int GetTotalWaves(Mobile from)
		{
            return Waves.Where(i => i.Wave > 2 && i.Credit.Contains(from)).Count();
		}
		
		public static int GetPlayerScore(Dictionary<Mobile, long> score, Mobile m)
		{
			if(score == null || m == null || !score.ContainsKey(m))
				return 0;
				
			return (int)score[m];
		}
		
		private Type[][] SpawnTable = new Type[][]
		{
			new Type[] { typeof(DaemonMongbat), 		typeof(GargoyleAssassin), 	typeof(CovetousDoppleganger), 	typeof(LesserOni),       typeof(CovetousFireDaemon) },
			new Type[] { typeof(LizardmanWitchdoctor), 	typeof(OrcFootSoldier), 	typeof(RatmanAssassin),         typeof(OgreBoneCrusher), typeof(TitanRockHunter) },
			new Type[] { typeof(AngeredSpirit), 		typeof(BoneSwordSlinger), 	typeof(VileCadaver), 	        typeof(DiseasedLich), 	 typeof(CovetousRevenant) },
			new Type[] { typeof(WarAlligator), 			typeof(MagmaLizard), 		typeof(ViciousDrake), 	        typeof(CorruptedWyvern), typeof(CovetousWyrm) },
			new Type[] { typeof(CovetousEarthElemental),typeof(CovetousWaterElemental), typeof(VortexElemental),    typeof(SearingElemental),typeof(VenomElemental) },
		};
		
		public override void Delete()
		{
			if(OnGoing)
				EndInvasion();

			if(Region != null)
			{
				Region.Unregister();
				Region = null;
			}
			
			if(Timer != null)
			{
				Timer.Stop();
				Timer = null;
			}

            foreach (var wp in WaypointsA.Where(w => w != null && !w.Deleted))
                wp.Delete();

            foreach (var wp in WaypointsB.Where(w => w != null && !w.Deleted))
                wp.Delete();

            base.Delete();
		}

		public VoidPoolController(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);

            writer.Write(RespawnMin);
            writer.Write(RespawnMax);

            writer.Write(_Active);
            writer.Write(WaypointsA.Count);
            writer.Write(WaypointsB.Count);

            WaypointsA.ForEach(w => writer.Write(w));
            WaypointsB.ForEach(w => writer.Write(w));
		}

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            RespawnMin = reader.ReadInt();
            RespawnMax = reader.ReadInt();

            WaypointsA = new List<WayPoint>();
            WaypointsB = new List<WayPoint>();

            Active = reader.ReadBool();

            int counta = reader.ReadInt();
            int countb = reader.ReadInt();

            for (int i = 0; i < counta; i++)
            {
                WayPoint wp = reader.ReadItem() as WayPoint;

                if (wp != null)
                    WaypointsA.Add(wp);
            }

            for (int i = 0; i < countb; i++)
            {
                WayPoint wp = reader.ReadItem() as WayPoint;

                if (wp != null)
                    WaypointsB.Add(wp);
            }

            if (Map == Map.Felucca)
                InstanceFel = this;
            else
                InstanceTram = this;

            Timer.DelayCall(TimeSpan.FromSeconds(10), ClearSpawn);
        }
	}
}

/*
wave 1: 13:30:00
wave 2: 13:30:10 +10
wave 3: 13:31:43 +93
wave 4: 13:33:05 +83
wave 5: 13:34:17 +75
wave 6: 13:35:08 +51
wave 7: 13:36:37 +89
wave 8: 13:37:38 +61
wave 9: 13:38:19 +41
wave 10: 13:39:32 +73
wave 11: 13:40:02 +30
wave 12: 13:40:54 +52
wave 13: 13:41:45 +51
wave 14: 13:42:36 +51
wave 15: 13:43:28 +52
wave 16: 13:44:09 +41
wave 17: 13:45:01 +52
*/
