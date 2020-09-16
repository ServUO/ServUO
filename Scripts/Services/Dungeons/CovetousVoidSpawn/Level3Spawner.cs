using Server.ContextMenus;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.VoidPool
{
    [PropertyObject]
    public class Level3Spawner : ISpawner
    {
        private bool _Active;

        [CommandProperty(AccessLevel.GameMaster)]
        public VoidPoolController Controller { get; set; }

        public List<SpawnEntry> Spawns { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EntryCount => Spawns == null ? 0 : Spawns.Count;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return _Active; }
            set
            {
                bool active = _Active;

                if (value != active)
                {
                    _Active = value;

                    if (!_Active)
                    {
                        Deactivate();
                    }
                    else
                    {
                        if (Spawns != null)
                        {
                            Deactivate(false);
                        }

                        LoadSpawns();
                        StartSpawn();
                    }
                }
            }
        }

        #region ISpawner Stuff
        public bool UnlinkOnTaming => true;
        public Point3D HomeLocation => Point3D.Zero;
        public int HomeRange => 20;

        public void Remove(ISpawnable spawnable)
        {
            RemoveFromSpawner(spawnable);
        }

        public virtual void GetSpawnProperties(ISpawnable spawn, ObjectPropertyList list)
        { }

        public virtual void GetSpawnContextEntries(ISpawnable spawn, Mobile user, List<ContextMenuEntry> list)
        { }
        #endregion

        public Level3Spawner(VoidPoolController controller)
        {
            Controller = controller;

            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                {
                    Active = true;
                });
        }

        public override string ToString()
        {
            return "...";
        }

        public void LoadSpawns()
        {
            if (Spawns != null)
                Spawns.Clear();

            Spawns = new List<SpawnEntry>();
            Spawns.Add(new SpawnEntry(this, new Rectangle2D[] { new Rectangle2D(5564, 1888, 26, 31) }));
            Spawns.Add(new SpawnEntry(this, new Rectangle2D[] { new Rectangle2D(5554, 1888, 8, 28) }));
            Spawns.Add(new SpawnEntry(this, new Rectangle2D[] { new Rectangle2D(5596, 1888, 8, 28) }));
            Spawns.Add(new SpawnEntry(this, new Rectangle2D[] { new Rectangle2D(5536, 1876, 8, 8), new Rectangle2D(5552, 1840, 11, 42) }));
            Spawns.Add(new SpawnEntry(this, new Rectangle2D[] { new Rectangle2D(5600, 1860, 8, 8), new Rectangle2D(5596, 1872, 15, 12) }));
            Spawns.Add(new SpawnEntry(this, new Rectangle2D[] { new Rectangle2D(5608, 1832, 15, 15), new Rectangle2D(5616, 1848, 11, 34) }));
            Spawns.Add(new SpawnEntry(this, new Rectangle2D[] { new Rectangle2D(5568, 1852, 22, 14) }, 15));
            Spawns.Add(new SpawnEntry(this, new Rectangle2D[] { new Rectangle2D(5568, 1824, 22, 11) }));
            Spawns.Add(new SpawnEntry(this, new Rectangle2D[] { new Rectangle2D(5501, 1800, 42, 19) }, 15));
        }

        public void RemoveFromSpawner(ISpawnable spawnable)
        {
            BaseCreature bc = spawnable as BaseCreature;

            if (bc != null)
            {
                SpawnEntry entry = FindEntryFor(bc);

                if (entry != null)
                {
                    entry.Spawn.Remove(bc);
                    bc.Spawner = null;

                    if (entry.CurrentCount == 0)
                    {
                        Timer.DelayCall(TimeSpan.FromMinutes(Utility.RandomMinMax(2, 5)), en =>
                            {
                                Reset(en);
                            }, entry);
                    }

                    List<DamageStore> list = bc.GetLootingRights();

                    foreach (DamageStore ds in list)
                    {
                        if (ds.m_HasRight)
                        {
                            int points = (bc.Fame / 998) / list.Count;

                            if (points > 0)
                            {
                                AddToOverallTotal(ds.m_Mobile, points);
                            }
                        }
                    }
                }
            }
        }

        public void AddToOverallTotal(Mobile m, int points)
        {
            VoidPoolStats stats = VoidPoolStats.GetStats(Controller);

            if (stats != null)
            {
                if (!stats.OverallTotal.ContainsKey(m))
                    stats.OverallTotal[m] = points;
                else
                    stats.OverallTotal[m] += points;
            }
        }

        public void Reset(SpawnEntry entry)
        {
            if (!Active)
                return;

            VoidType type = entry.SpawnType;

            do
            {
                entry.SpawnType = (VoidType)Utility.RandomMinMax(0, 4);
            }
            while (entry.SpawnType == type);

            entry.DoSpawn();
        }

        public void StartSpawn()
        {
            foreach (SpawnEntry entry in Spawns)
            {
                entry.SpawnType = (VoidType)Utility.RandomMinMax(0, 4);
                entry.DoSpawn();
            }
        }

        public SpawnEntry FindEntryFor(BaseCreature bc)
        {
            return Spawns.FirstOrDefault(sp => sp.Spawn.Contains(bc));
        }

        public void Deactivate(bool deactivate = true)
        {
            if (deactivate)
            {
                Active = false;
            }

            if (Spawns != null)
            {
                foreach (SpawnEntry entry in Spawns)
                {
                    List<BaseCreature> list = new List<BaseCreature>();

                    foreach (BaseCreature bc in entry.Spawn)
                    {
                        list.Add(bc);
                    }

                    foreach (BaseCreature creature in list)
                        creature.Delete();

                    ColUtility.Free(list);
                }

                Spawns.Clear();
            }
        }

        public Level3Spawner(GenericReader reader, VoidPoolController controller)
        {
            Controller = controller;
            LoadSpawns();

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                case 1:
                    {
                        if (version == 1)
                            Active = controller.Active;

                        _Active = reader.ReadBool();
                    }
                    goto case 0;
                case 0:
                    {
                        int count = reader.ReadInt();

                        for (int i = 0; i < count; i++)
                        {
                            Spawns[i].Deserialize(reader);
                        }
                    }
                    break;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(2);

            writer.Write(_Active);

            writer.Write(Spawns == null ? 0 : Spawns.Count);

            if (Spawns != null)
            {
                for (int i = 0; i < Spawns.Count; i++)
                {
                    Spawns[i].Serialize(writer);
                }
            }
        }

        public class SpawnEntry
        {
            public Level3Spawner Spawner { get; set; }

            public Rectangle2D[] Bounds { get; private set; }
            public int MaxCount { get; private set; }

            public int CurrentCount => Spawn.Count;
            public VoidType SpawnType { get; set; }

            public List<BaseCreature> Spawn { get; set; }

            public SpawnEntry(Level3Spawner spawner, Rectangle2D[] bounds, int maxCount = 8)
            {
                Bounds = bounds;
                MaxCount = maxCount;
                Spawner = spawner;
                Spawn = new List<BaseCreature>();
            }

            public void DoSpawn()
            {
                if (Spawner == null || Spawner.Controller == null || Spawner.Controller.Map == null || !Spawner.Active)
                {
                    return;
                }

                Map map = Spawner.Controller.Map;
                Type[] types = VoidPoolController.SpawnTable[(int)SpawnType];

                for (int i = 0; i < MaxCount; i++)
                {
                    BaseCreature bc = Activator.CreateInstance(types[Utility.Random(types.Length)], Utility.RandomMinMax(5, 15), false) as BaseCreature;

                    if (bc != null)
                    {
                        Rectangle2D rec = Bounds[Utility.Random(Bounds.Length)];
                        Point3D p = Point3D.Zero;

                        for (int j = 0; j < 50; j++)
                        {
                            p = map.GetRandomSpawnPoint(rec);

                            if (map.CanSpawnMobile(p))
                                break;
                        }

                        if (p != Point3D.Zero)
                        {
                            bc.Spawner = Spawner;

                            bc.Home = p;
                            bc.RangeHome = 20;

                            bc.OnBeforeSpawn(p, map);
                            bc.MoveToWorld(p, map);
                            bc.OnAfterSpawn();

                            Spawn.Add(bc);
                        }
                        else
                        {
                            bc.Delete();
                        }
                    }
                }
            }

            public void Serialize(GenericWriter writer)
            {
                writer.Write(0);

                writer.Write(Spawn.Count);
                Spawn.ForEach(s => writer.Write(s));
            }

            public void Deserialize(GenericReader reader)
            {
                int version = reader.ReadInt();

                Spawn = new List<BaseCreature>();
                int count = reader.ReadInt();

                for (int i = 0; i < count; i++)
                {
                    BaseCreature bc = reader.ReadMobile() as BaseCreature;

                    if (bc != null)
                    {
                        bc.Spawner = Spawner;
                        Spawn.Add(bc);
                    }
                }

                if (Spawn.Count == 0)
                    Timer.DelayCall(DoSpawn);
            }
        }
    }
}