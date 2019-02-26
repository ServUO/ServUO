using System;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Mobiles;
using Server.Items;
using Server.Commands;
using Server.Gumps;

namespace Server.Engines.SorcerersDungeon
{
    public class TOSDSpawner
    {
        public static void Initialize()
        {
            CommandSystem.Register("TOSDSpawner", AccessLevel.Administrator, e =>
                {
                    if (e.Mobile is PlayerMobile)
                    {
                        if (Instance != null)
                        {
                            BaseGump.SendGump(new TOSDSpawnerGump((PlayerMobile)e.Mobile));
                        }
                        else
                        {
                            e.Mobile.SendMessage("This spawner is not set up at this time. Enabled Treasures of Sorcerer's Dungeon to enable the spawner.");
                        }
                    }
                });

            if (Instance != null)
            {
                Instance.BeginTimer();
            }
        }

        public static TOSDSpawner Instance { get; set; }

        public BaseCreature Boss { get; set; }
        public int Index { get; set; }
        public int KillCount { get; set; }
        public List<BaseCreature> Spawn { get; set; }

        public bool Spawning { get; set; }
        public Timer Timer { get; set; }
        public List<TOSDSpawnEntry> Entries { get; set; }

        public TOSDSpawner()
        {
            Instance = this;

            Entries = new List<TOSDSpawnEntry>();
            Spawn = new List<BaseCreature>();

            BuildEntries();
            Activate();
        }

        private void Activate()
        {
            foreach (var rec in Entries.Select(e => e.SpawnArea))
            {
                IPooledEnumerable eable = Map.Ilshenar.GetItemsInBounds(rec);

                foreach (Item item in eable)
                {
                    if (item is XmlSpawner)
                    {
                        ((XmlSpawner)item).DoReset = true;
                    }
                }
            }
        }

        public void Deactivate()
        {
            foreach (var rec in Entries.Select(e => e.SpawnArea))
            {
                IPooledEnumerable eable = Map.Ilshenar.GetItemsInBounds(rec);

                foreach (Item item in eable)
                {
                    if (item is XmlSpawner)
                    {
                        ((XmlSpawner)item).DoRespawn = true;
                    }
                }
            }

            EndTimer();

            foreach (var bc in Spawn)
            {
                bc.Delete();
            }

            Instance = null;
        }

        public void BeginTimer()
        {
            EndTimer();

            Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
            Timer.Start();
        }

        public void EndTimer()
        {
            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }
        }

        private void OnTick()
        {
            if (Spawning)
            {
                return;
            }

            if (Boss != null && Boss.Deleted)
            {
                if (Index == Entries.Count - 1)
                {
                    Index = 0;
                }
                else
                {
                    Index++;
                }

                Reset();
            }
            else if (Boss == null && KillCount >= Entries[Index].ToKill)
            {
                DoSpawn(Entries[Index].Boss, true);
            }
            else if (Spawn.Count < Entries[Index].MaxSpawn && (Boss == null || Entries[Index].SpawnArea.Contains(Boss)))
            {
                Spawning = true;

                Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(3, 8)), () =>
                {
                    DoSpawn(Entries[Index].Spawn[Utility.Random(Entries[Index].Spawn.Length)], false);

                    Spawning = false;
                });
            }
        }

        public void DoSpawn(Type t, bool boss)
        {
            var spawn = Activator.CreateInstance(t) as BaseCreature;

            for (int i = 0; i < 20; i++)
            {
                var p = Map.Ilshenar.GetRandomSpawnPoint(Entries[Index].SpawnArea);

                if (Map.Ilshenar.CanSpawnMobile(p))
                {
                    spawn.MoveToWorld(p, Map.Ilshenar);

                    if (boss)
                    {
                        Boss = spawn;
                    }
                    else
                    {
                        AddSpawn(spawn);
                    }

                    return;
                }
            }

            spawn.Delete();
        }

        public void AddSpawn(BaseCreature bc)
        {
            Spawn.Add(bc);
        }

        public void OnCreatureDeath(BaseCreature bc)
        {
            if (Spawn.Contains(bc))
            {
                Spawn.Remove(bc);

                KillCount++;
            }
        }

        public void Reset()
		{
			ColUtility.Free(Spawn);
            Boss = null;
            Spawning = false;

			KillCount = 0;
			
			EndTimer();
			
			Timer.DelayCall(TimeSpan.FromMinutes(Utility.RandomMinMax(1, 3)), () =>
			{
				BeginTimer();
			});
		}

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Boss);
            writer.Write(KillCount);
            writer.Write(Index);

            writer.Write(Spawn.Count);

            foreach (var bc in Spawn)
            {
                writer.Write(bc);
            }
        }
		
		public void Deserialize(GenericReader reader)
		{
			reader.ReadInt(); // version
			
			Boss = reader.ReadMobile() as BaseCreature;
			KillCount = reader.ReadInt();
			Index = reader.ReadInt();
			
			int count = reader.ReadInt();
			
			for(int i = 0; i < count; i++)
			{
				var bc = reader.ReadMobile() as BaseCreature;
				
				if(bc != null)
				{
					AddSpawn(bc);
				}
			}
		}

        public void BuildEntries()
        {
            Entries.Add(new TOSDSpawnEntry(typeof(NightmareFairy), new Type[] { typeof(Zombie), typeof(Skeleton), typeof(Gargoyle), typeof(Lich), typeof(LichLord) }, new Rectangle2D(327, 26, 29, 32), 80, 15));
            Entries.Add(new TOSDSpawnEntry(typeof(JackInTheBox), new Type[] { typeof(Zombie), typeof(Skeleton), typeof(Gargoyle), typeof(Lich), typeof(LichLord) }, new Rectangle2D(296, 10, 17, 26), 80, 15));
            Entries.Add(new TOSDSpawnEntry(typeof(HeadlessElf), new Type[] { typeof(Zombie), typeof(Skeleton), typeof(Gargoyle), typeof(Lich), typeof(LichLord) }, new Rectangle2D(271, 4, 20, 33), 80, 15));
            Entries.Add(new TOSDSpawnEntry(typeof(AbominableSnowman), new Type[] { typeof(Zombie), typeof(Skeleton), typeof(Gargoyle), typeof(Lich), typeof(LichLord) }, new Rectangle2D(227, 39, 21, 19), 80, 15));
            Entries.Add(new TOSDSpawnEntry(typeof(TwistedHolidayTree), new Type[] { typeof(Zombie), typeof(Skeleton), typeof(Gargoyle), typeof(Lich), typeof(LichLord) }, new Rectangle2D(251, 68, 25, 32), 80, 15));
            Entries.Add(new TOSDSpawnEntry(typeof(RabidReindeer), new Type[] { typeof(Zombie), typeof(Skeleton), typeof(Gargoyle), typeof(Lich), typeof(LichLord) }, new Rectangle2D(144, 5, 23, 19), 80, 15));
            Entries.Add(new TOSDSpawnEntry(typeof(GarishGingerman), new Type[] { typeof(Zombie), typeof(Skeleton), typeof(Gargoyle), typeof(Lich), typeof(LichLord) }, new Rectangle2D(60, 53, 13, 34), 80, 15));
            Entries.Add(new TOSDSpawnEntry(typeof(StockingSerpent), new Type[] { typeof(Zombie), typeof(Skeleton), typeof(Gargoyle), typeof(Lich), typeof(LichLord) }, new Rectangle2D(152, 48, 16, 23), 80, 15));
            Entries.Add(new TOSDSpawnEntry(typeof(JackThePumpkinKing), new Type[] { typeof(Zombie), typeof(Skeleton), typeof(Gargoyle), typeof(Lich), typeof(LichLord) }, new Rectangle2D(291, 73, 37, 36), 80, 15));
        }
	}
	
	public class TOSDSpawnEntry
	{
		public Type Boss { get; set; }
		public Type[] Spawn { get; set; }
		public Rectangle2D SpawnArea { get; set; }
		
		public int ToKill { get; set; }
		public int MaxSpawn { get; set; }
		
		public TOSDSpawnEntry(Type boss, Type[] spawn, Rectangle2D area, int toKill, int maxSpawn)
		{
			Boss = boss;
			Spawn = spawn;
			SpawnArea = area;
			ToKill = toKill;
			MaxSpawn = maxSpawn;
		}
	}

    public class TOSDSpawnerGump : BaseGump
    {
        public TOSDSpawnerGump(PlayerMobile pm)
            : base(pm, 50, 50)
        {
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 500, 300, 9300);
            AddHtml(0, 10, 500, 20, Center("Treasures of Sorcerer's Dungeon Spawner"), false, false);

            var spawner = TOSDSpawner.Instance;

            if (spawner == null)
            {
                AddHtml(10, 40, 150, 20, "Spawner Disabled", false, false);
            }
            else
            {
                int y = 60;

                AddLabel(10, 40, 0, "Go");
                AddLabel(40, 40, 0, "Boss");
                AddLabel(240, 40, 0, "Current");
                AddLabel(320, 40, 0, "Max");
                AddLabel(400, 40, 0, "Killed");

                for (int i = 0; i < spawner.Entries.Count; i++)
                {
                    var entry = spawner.Entries[i];
                    var hue = i == spawner.Index ? "green" : "red";

                    AddButton(7, y, 1531, 1532, i + 100, GumpButtonType.Reply, 0);
                    AddHtml(40, y, 200, 20, Color(hue, entry.Boss.Name), false, false);
                    AddHtml(320, y, 80, 20, Color(hue, entry.MaxSpawn.ToString()), false, false);

                    if (hue == "green")
                    {
                        AddHtml(240, y, 80, 20, Color(hue, spawner.Spawn.Count.ToString()), false, false);
                        AddHtml(400, y, 80, 20, Color(hue, spawner.KillCount.ToString()), false, false);
                    }
                    else
                    {
                        AddHtml(240, y, 80, 20, Color(hue, "0"), false, false);
                        AddHtml(400, y, 80, 20, Color(hue, "0"), false, false);
                    }

                    y += 22;
                }
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID > 0)
            {
                int id = info.ButtonID - 100;
                var spawner = TOSDSpawner.Instance;

                if (spawner != null && id >= 0 && id < spawner.Entries.Count)
                {
                    var entry = spawner.Entries[id];

                    do
                    {
                        var p = Map.Ilshenar.GetRandomSpawnPoint(entry.SpawnArea);

                        if (Map.Ilshenar.CanSpawnMobile(p))
                        {
                            User.MoveToWorld(p, Map.Ilshenar);
                            Refresh();

                            break;
                        }
                    }
                    while (true);
                }
            }
        }
    }
}
			
