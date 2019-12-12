using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.MiniChamps
{
    public class MiniChampSpawnInfo
    {
        private MiniChamp Owner;
        public List<Mobile> Creatures;

        public Type MonsterType { get; set; }
        public int Killed { get; set; }
        public int Spawned { get; set; }
        public int Required { get; set; }
        public int MaxSpawned { get { return (Required * 2) - 1; } }
        public bool Done { get { return Killed >= Required; } }

        public MiniChampSpawnInfo(MiniChamp controller, MiniChampTypeInfo typeInfo)
        {
            Owner = controller;

            Required = typeInfo.Required;
            MonsterType = typeInfo.SpawnType;

            Creatures = new List<Mobile>();
            Killed = 0;
            Spawned = 0;
        }

        public bool Slice()
        {
            bool killed = false;
            var list = new List<Mobile>(Creatures);

            for (int i = 0; i < list.Count; i++)
            {
                Mobile creature = list[i];

                if (creature == null || creature.Deleted)
                {
                    Creatures.Remove(creature);
                    Killed++;

                    killed = true;
                }
                else if (!creature.InRange(Owner.Location, Owner.SpawnRange + 10))
                {
                    // bring to home
                    Map map = Owner.Map;
                    Point3D loc = map.GetSpawnPosition(Owner.Location, Owner.SpawnRange);

                    creature.MoveToWorld(loc, map);
                }
            }

            ColUtility.Free(list);
            return killed;
        }        

        public bool Respawn()
        {
            bool spawned = false;

            while (Creatures.Count < Required && Spawned < MaxSpawned)
            {
                BaseCreature bc = Activator.CreateInstance(MonsterType) as BaseCreature;

                Map map = Owner.Map;
                Point3D loc = map.GetSpawnPosition(Owner.Location, Owner.SpawnRange);

                if (Owner.BossSpawnPoint != Point3D.Zero)
                {
                    loc = Owner.BossSpawnPoint;
                }

                bc.Home = Owner.Location;
                bc.RangeHome = Owner.SpawnRange;
                bc.Tamable = false;
                bc.OnBeforeSpawn(loc, map);
                bc.MoveToWorld(loc, map);

                if (bc.Fame > Utility.Random(100000) || bc is BaseRenowned)
                {
                    DropEssence(bc);
                }

                Creatures.Add(bc);

                ++Spawned;

                spawned = true;
            }

            return spawned;
        }

        private void DropEssence(BaseCreature bc)
        {
            Type essenceType = MiniChampInfo.GetInfo(Owner.Type).EssenceType;

            Item essence = null;

            try { essence = (Item)Activator.CreateInstance(essenceType); }
            catch { }

            if (essence != null)
            {
                bc.PackItem(essence);
            }
        }

        public void AddProperties(ObjectPropertyList list, int cliloc)
        {
            list.Add(cliloc, "{0}: Killed {1}/{2}, Spawned {3}/{4}",
                MonsterType.Name, Killed, Required, Spawned, MaxSpawned);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteItem<MiniChamp>(Owner);
            writer.Write(Killed);
            writer.Write(Spawned);
            writer.Write(Required);
            writer.Write(MonsterType.FullName);
            writer.Write(Creatures);
        }

        public MiniChampSpawnInfo(GenericReader reader)
        {
            Creatures = new List<Mobile>();

            Owner = reader.ReadItem<MiniChamp>();
            Killed = reader.ReadInt();
            Spawned = reader.ReadInt();
            Required = reader.ReadInt();
            MonsterType = ScriptCompiler.FindTypeByFullName(reader.ReadString());
            Creatures = reader.ReadStrongMobileList();
        }
    }   
}