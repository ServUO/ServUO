using System;
using System.Collections;
using Server.Gumps;

namespace Server.Engines.CannedEvil
{
    public class ChampionSpawnController : Item
    {
        private readonly SpawnRecord[] m_Dungeons = new SpawnRecord[]
        {
            new SpawnRecord((int)ChampionSpawnType.UnholyTerror, 5178, 708, 20), //Deceit
            new SpawnRecord((int)ChampionSpawnType.VerminHorde, 5557, 824, 65), // Despise
            new SpawnRecord((int)ChampionSpawnType.ColdBlood, 5259, 837, 61), // Destard
            new SpawnRecord((int)ChampionSpawnType.Abyss, 5814, 1350, 2), // Fire
            new SpawnRecord((int)ChampionSpawnType.Arachnid, 5190, 1605, 20), // TerathanKeep
            new SpawnRecord((int)ChampionSpawnType.Terror, 6995, 733, 76), // Abyssal Lair
            new SpawnRecord((int)ChampionSpawnType.Infuse, 7000, 1004, 5), // Land of Liches
        };
        private readonly SpawnRecord[] m_LostLands = new SpawnRecord[]
        {
            new SpawnRecord(0xff, 5636, 2916, 37), // Desert
            new SpawnRecord(0xff, 5724, 3991, 42), // Tortoise
            new SpawnRecord(0xff, 5511, 2360, 40), // Ice West

            new SpawnRecord(0xff, 5549, 2640, 15), // Oasis
            new SpawnRecord(0xff, 6035, 2944, 52), // Terra Sanctum
            new SpawnRecord((int)ChampionSpawnType.ForestLord, 5559, 3757, 21), // Lord Oaks

            new SpawnRecord(0xff, 5267, 3171, 104), // Marble
            new SpawnRecord(0xff, 5954, 3475, 25), // Hoppers Boog
            new SpawnRecord(0xff, 5982, 3882, 20), // Khaldun

            new SpawnRecord(0xff, 6038, 2400, 46), // Ice East
            new SpawnRecord(0xff, 5281, 3368, 51), // Damwin
            new SpawnRecord(0xff, 5207, 3637, 20), // City of Death
        };
        private readonly SpawnRecord[] m_Ilshenar = new SpawnRecord[]
        {
            new SpawnRecord(0xff, 382, 328, -30), // Valor
            new SpawnRecord(0xff, 462, 926, -67), // Humility
            new SpawnRecord((int)ChampionSpawnType.ForestLord, 1645, 1107, 8), // Spirituality
            new SpawnRecord((int)ChampionSpawnType.Glade, 2212, 1260, 25), // Twisted Glade
        };
        private readonly SpawnRecord[] m_Tokuno = new SpawnRecord[]
        {
            new SpawnRecord((int)ChampionSpawnType.SleepingDragon, 948, 434, 29), // Isamu Jima
        };
        private readonly SpawnRecord[] m_Malas = new SpawnRecord[]
        {
            new SpawnRecord((int)ChampionSpawnType.Corrupt, 174, 1629, 8), // Bedlam
        };
        bool m_Active;
        private ArrayList m_AllSpawn;
        private ArrayList m_DungeonsSpawn;
        private ArrayList m_LostLandsSpawn;
        private ArrayList m_IlshenarSpawn;
        private ArrayList m_TokunoSpawn;
        private ArrayList m_MalasSpawn;
        //private int m_SpawnRange;
        private TimeSpan m_ExpireDelay;
        private TimeSpan m_RestartDelay;
        private TimeSpan m_RandomizeDelay;
        private int m_ActiveAltars;
        private RandomizeTimer m_Timer;
        [Constructable]
        public ChampionSpawnController()
            : base(0x1B7A)
        {
            if (this.Check())
            {
                World.Broadcast(0x35, true, "Another champion spawn controller exist in the world !");
                this.Delete();
                return;
            }

            this.Visible = false;
            this.Movable = false;
            this.Name = "Champion Spawn Controller";

            this.m_Active = false;

            this.m_AllSpawn = new ArrayList();
            this.m_DungeonsSpawn = new ArrayList();
            this.m_LostLandsSpawn = new ArrayList();
            this.m_IlshenarSpawn = new ArrayList();
            this.m_TokunoSpawn = new ArrayList();
            this.m_MalasSpawn = new ArrayList();

            this.m_ActiveAltars = 1;

            //m_SpawnRange = 24;
            this.m_ExpireDelay = TimeSpan.FromMinutes(10.0);
            this.m_RestartDelay = TimeSpan.FromMinutes(5.0);

            this.m_RandomizeDelay = TimeSpan.FromHours(72.0);

            this.DeleteAll();
            this.Generate();

            World.Broadcast(0x35, true, "Champion spawn generation complete. Old spawns removed.");
        }

        public ChampionSpawnController(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get
            {
                return this.m_Active;
            }
            set
            {
                if (value)
                    this.Start();
                else
                    this.Stop();

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ActiveAltars
        {
            get
            {
                return this.m_ActiveAltars;
            }
            set
            {
                this.m_ActiveAltars = value;
            }
        }
        /*[CommandProperty( AccessLevel.GameMaster )]
        public int SpawnRange
        {
        get
        {
        return m_SpawnRange;
        }
        set
        {
        m_SpawnRange = value;

        foreach( ChampionSpawn cs in m_AllSpawn )
        {
        cs.SpawnRange = m_SpawnRange;
        }
        }
        }*/
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan ExpireDelay
        {
            get
            {
                return this.m_ExpireDelay;
            }
            set
            {
                this.m_ExpireDelay = value;

                foreach (ChampionSpawn cs in this.m_AllSpawn)
                {
                    cs.ExpireDelay = this.m_ExpireDelay;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan RestartDelay
        {
            get
            {
                return this.m_RestartDelay;
            }
            set
            {
                this.m_RestartDelay = value;

                foreach (ChampionSpawn cs in this.m_IlshenarSpawn)
                {
                    cs.RestartDelay = this.m_RestartDelay;
                }

                foreach (ChampionSpawn cs in this.m_TokunoSpawn)
                {
                    cs.RestartDelay = this.m_RestartDelay;
                }

                foreach (ChampionSpawn cs in this.m_MalasSpawn)
                {
                    cs.RestartDelay = this.m_RestartDelay;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan RandomizeDelay
        {
            get
            {
                return this.m_RandomizeDelay;
            }
            set
            {
                this.m_RandomizeDelay = value;
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Active)
            {
                list.Add(1060742); // active
            }
            else
            {
                list.Add(1060743); // inactive
            }
        }

        public override void OnDelete()
        {
            this.Stop();

            if (this.m_AllSpawn != null)
            {
                foreach (ChampionSpawn cs in this.m_AllSpawn)
                {
                    if (!cs.Deleted)
                        cs.Delete();
                }
            }

            base.OnDelete();
        }

        public void Start()
        {
            if (this.m_Active || this.Deleted)
                return;

            this.m_Active = true;

            if (this.m_Timer == null)
            {
                this.m_Timer = new RandomizeTimer(this, this.m_RandomizeDelay);
            }

            this.Randomize(this.m_DungeonsSpawn);
            this.Randomize(this.m_LostLandsSpawn);

            foreach (ChampionSpawn cs in this.m_IlshenarSpawn)
            {
                if (!cs.Deleted)
                    cs.Active = true;
            }

            foreach (ChampionSpawn cs in this.m_TokunoSpawn)
            {
                if (!cs.Deleted)
                    cs.Active = true;
            }

            foreach (ChampionSpawn cs in this.m_MalasSpawn)
            {
                if (!cs.Deleted)
                    cs.Active = false;
            }
            
            this.m_Timer.Start();
        }

        public void Stop()
        {
            if (!this.m_Active || this.Deleted)
                return;

            this.m_Active = false;

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            foreach (ChampionSpawn cs in this.m_AllSpawn)
            {
                if (!cs.Deleted && cs.Active)
                {
                    cs.Active = false;
                    //cs.IsValorUsed = false;
                }
            }
        }

        public void Randomize(ArrayList list)
        {
            foreach (ChampionSpawn cs in list)
            {
                if (!cs.Deleted && cs.Active)
                    cs.Active = false;
            }

            for (int i = 0; i < this.m_ActiveAltars; i++)
            {
                int trynum = 0;

                while (trynum < 9)
                {
                    int index = Utility.Random(list.Count);

                    if (!((ChampionSpawn)list[index]).Active)
                    {
                        ((ChampionSpawn)list[index]).Active = true;
                        break;
                    }

                    trynum++;
                }
            }
        }

        public void Slice()
        {
            if (!this.m_Active || this.Deleted)
            {
                if (this.m_Timer != null)
                    this.m_Timer.Stop();
                return;
            }

            this.Randomize(this.m_DungeonsSpawn);
            this.Randomize(this.m_LostLandsSpawn);

            this.m_Timer.Start();
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendGump(new PropertiesGump(from, this));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
           
            writer.WriteItemList(this.m_MalasSpawn);

            writer.Write(this.m_Active);

            writer.WriteItemList(this.m_AllSpawn);
            writer.WriteItemList(this.m_DungeonsSpawn);
            writer.WriteItemList(this.m_LostLandsSpawn);
            writer.WriteItemList(this.m_IlshenarSpawn);
            writer.WriteItemList(this.m_TokunoSpawn);

            writer.Write(this.m_RandomizeDelay);

            writer.Write(this.m_ActiveAltars);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        this.m_MalasSpawn = reader.ReadItemList();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Active = reader.ReadBool();

                        this.m_AllSpawn = reader.ReadItemList();
                        this.m_DungeonsSpawn = reader.ReadItemList();
                        this.m_LostLandsSpawn = reader.ReadItemList();
                        this.m_IlshenarSpawn = reader.ReadItemList();
                        this.m_TokunoSpawn = reader.ReadItemList();
                       
                        this.m_RandomizeDelay = reader.ReadTimeSpan();

                        this.m_ActiveAltars = reader.ReadInt();

                        //m_SpawnRange = 24;
                        this.m_ExpireDelay = TimeSpan.FromMinutes(10.0);
                        this.m_RestartDelay = TimeSpan.FromMinutes(5.0);

                        if (this.m_Active)
                        {
                            this.m_Timer = new RandomizeTimer(this, this.m_RandomizeDelay);

                            this.m_Timer.Start();
                        }
                        break;
                    }
            }
        }

        private bool Check()
        {
            foreach (Item item in World.Items.Values)
            {
                if (item is ChampionSpawnController && !item.Deleted && item != this)
                    return true;
            }

            return false;
        }

        private void DeleteAll()
        {
            ArrayList list = new ArrayList();

            foreach (Item item in World.Items.Values)
            {
                if (item is ChampionSpawn && !item.Deleted)
                    list.Add(item);
            }

            foreach (ChampionSpawn cs in list)
            {
                cs.Delete();
            }
        }

        private ChampionSpawn CreateAltar(SpawnRecord r, Map m, bool restartdisable)
        {
            ChampionSpawn cs = new ChampionSpawn();

            Point3D loc = new Point3D(r.x, r.y, r.z);

            if (r.type == 0xff)
            {
                cs.RandomizeType = true;

                switch (Utility.Random(11))
                {
                    case 0:
                        cs.Type = ChampionSpawnType.Abyss; break;
                    case 1:
                        cs.Type = ChampionSpawnType.Arachnid; break;
                    case 2:
                        cs.Type = ChampionSpawnType.ColdBlood; break;
                    case 3:
                        cs.Type = ChampionSpawnType.ForestLord; break;
                    case 4:
                        cs.Type = ChampionSpawnType.VerminHorde; break;
                    case 5:
                        cs.Type = ChampionSpawnType.UnholyTerror; break;
                    case 6:
                        cs.Type = ChampionSpawnType.SleepingDragon; break;
                    case 7:
                        cs.Type = ChampionSpawnType.Glade; break;
                    case 8:
                        cs.Type = ChampionSpawnType.Corrupt; break;
                    case 9:
                        cs.Type = ChampionSpawnType.Terror; break;
                    case 10:
                        cs.Type = ChampionSpawnType.Infuse; break;
                }
            }
            else
            {
                cs.RandomizeType = false;
                cs.Type = (ChampionSpawnType)r.type;
            }

            // Prevent autorestart of felucca & t2a the spawns

            if (restartdisable)
                cs.RestartDelay = TimeSpan.FromDays(9999);

            cs.MoveToWorld(loc, m);

            return cs;
        }

        private void Generate()
        {
            int i = 0;

            for (i = 0; i < this.m_Dungeons.Length; i++)
            {
                ChampionSpawn cs = this.CreateAltar(this.m_Dungeons[i], Map.Felucca, true);

                this.m_AllSpawn.Add(cs);
                this.m_DungeonsSpawn.Add(cs);
            }

            for (i = 0; i < this.m_LostLands.Length; i++)
            {
                ChampionSpawn cs = this.CreateAltar(this.m_LostLands[i], Map.Felucca, true);

                this.m_AllSpawn.Add(cs);
                this.m_LostLandsSpawn.Add(cs);
            }

            for (i = 0; i < this.m_Ilshenar.Length; i++)
            {
                ChampionSpawn cs = this.CreateAltar(this.m_Ilshenar[i], Map.Ilshenar, false);

                this.m_IlshenarSpawn.Add(cs);
                this.m_AllSpawn.Add(cs);
            }

            for (i = 0; i < this.m_Tokuno.Length; i++)
            {
                ChampionSpawn cs = this.CreateAltar(this.m_Tokuno[i], Map.Tokuno, false);

                this.m_TokunoSpawn.Add(cs);
                this.m_AllSpawn.Add(cs);
            }

            for (i = 0; i < this.m_Malas.Length; i++)
            {
                ChampionSpawn cs = this.CreateAltar(this.m_Malas[i], Map.Malas, true);

                this.m_MalasSpawn.Add(cs);
                this.m_AllSpawn.Add(cs);
            }
        }

        public struct SpawnRecord
        {
            public int type, x, y, z;
            public SpawnRecord(int type, int x, int y, int z)
            {
                this.type = type;
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }
    }
}