using System;
using Server.Items;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    public enum VoidEvolution
    {
        None = 0,
        Killing = 1,
        Grouping = 2,
        Survival = 3
    }

    public class BaseVoidCreature : BaseCreature
    {
        public static int MutateCheck { get { return Utility.RandomMinMax(30, 120); } }

        public static bool RemoveFromSpawners { get { return true; } }

        private DateTime m_NextMutate;
        private bool m_BuddyMutate;

        public virtual int GroupAmount { get { return 2; } }
        public virtual VoidEvolution Evolution { get { return VoidEvolution.None; } }
        public virtual int Stage { get { return 0; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool BuddyMutate { get { return m_BuddyMutate; } set { m_BuddyMutate = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextMutate { get { return m_NextMutate; } set { m_NextMutate = value; } }

        public override bool PlayerRangeSensitive { get { return Evolution != VoidEvolution.Killing && Stage < 3; } }
        public override bool AlwaysMurderer { get { return true; } }

        public BaseVoidCreature(AIType aiType, FightMode fightMode, int perception, int range, double passive, double active)
            : base(aiType, FightMode.Good, perception, range, passive, active)
        {
            m_NextMutate = DateTime.UtcNow + TimeSpan.FromMinutes(MutateCheck);
            m_BuddyMutate = true;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Stage >= 3 || m_NextMutate > DateTime.UtcNow)
                return;

            if (!MutateGrouped() && Alive && !Deleted)
            {
                Mutate(VoidEvolution.Survival);
            }
        }

        public bool MutateGrouped()
        {
            if (!m_BuddyMutate)
                return false;

            List<BaseVoidCreature> buddies = new List<BaseVoidCreature>();
            IPooledEnumerable eable = this.GetMobilesInRange(12);

            foreach (Mobile m in eable)
            {
                if (m != this && IsEvolutionType(m) && !m.Deleted && m.Alive && !buddies.Contains((BaseVoidCreature)m))
                {
                    if (m is BaseVoidCreature && ((BaseVoidCreature)m).BuddyMutate)
                        buddies.Add((BaseVoidCreature)m);
                }
            }

            eable.Free();

            if (buddies.Count >= GroupAmount)
            {
                Mutate(VoidEvolution.Grouping);

                foreach (BaseVoidCreature k in buddies)
                    k.Mutate(VoidEvolution.Grouping);

                ColUtility.Free(buddies);

                return true;
            }

            ColUtility.Free(buddies);
            return false;
        }

        public bool IsEvolutionType(Mobile from)
        {
            if (Stage == 0 && from.GetType() != this.GetType())
                return false;

            return from is BaseVoidCreature;
        }

        public Type[][] m_EvolutionCycle = new Type[][]
        {
            new Type[] { typeof(Betballem),     typeof(Ballem),     typeof(UsagralemBallem) },
            new Type[] { typeof(Anlorzen),      typeof(Anlorlem),   typeof(Anlorvaglem) },
            new Type[] { typeof(Anzuanord),     typeof(Relanord),   typeof(Vasanord) }
        };

        private BaseCreature _MutateTo;

        public void Mutate(VoidEvolution evolution)
        {
            if (!Alive || Deleted || Stage == 3)
                return;

            VoidEvolution evo = evolution;

            if (Stage > 0)
                evo = this.Evolution;

            if (0.05 > Utility.RandomDouble())
            {
                SpawnOrtanords();
            }

            Type type = m_EvolutionCycle[(int)evo - 1][Stage];

            BaseCreature bc = (BaseCreature)Activator.CreateInstance(type);

            _MutateTo = bc;

            if (bc != null)
            {
                //TODO: Effents/message?

                bc.MoveToWorld(this.Location, this.Map);

                bc.Home = this.Home;
                bc.RangeHome = this.RangeHome;

                if (0.05 > Utility.RandomDouble())
                    SpawnOrtanords();

                if (bc is BaseVoidCreature)
                    ((BaseVoidCreature)bc).BuddyMutate = m_BuddyMutate;

                this.Delete();
            }
        }

        public void SpawnOrtanords()
        {
            BaseCreature ortanords = new Ortanord();

            Point3D spawnLoc = this.Location;

            for (int i = 0; i < 25; i++)
            {
                int x = Utility.RandomMinMax(this.X - 5, this.X + 5);
                int y = Utility.RandomMinMax(this.Y - 5, this.Y + 5);
                int z = this.Map.GetAverageZ(x, y);

                Point3D p = new Point3D(x, y, z);

                if (this.Map.CanSpawnMobile(p))
                {
                    spawnLoc = p;
                    break;
                }
            }

            ortanords.MoveToWorld(spawnLoc, this.Map);
            ortanords.BoltEffect(0);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            double baseChance = 0.02;
            double chance = 0.0;

            if (Stage > 0)
                chance = baseChance * (Stage + 3);

            if (Stage > 0 && Utility.RandomDouble() < chance)
                c.DropItem(new VoidEssence());

            if (Stage == 3 && Utility.RandomDouble() < 0.12)
                c.DropItem(new VoidCore());
        }

        public override void Delete()
        {
            if (_MutateTo != null)
            {
                ISpawner s = Spawner;

                if (s is XmlSpawner)
                {
                    XmlSpawner xml = (XmlSpawner)s;

                    if (xml.SpawnObjects == null)
                        return;

                    foreach (XmlSpawner.SpawnObject so in xml.SpawnObjects)
                    {
                        for (int i = 0; i < so.SpawnedObjects.Count; ++i)
                        {
                            if (so.SpawnedObjects[i] == this)
                            {
                                //so.SpawnedObjects.Remove(spawn);
                                so.SpawnedObjects[i] = _MutateTo;

                                Spawner = null;
                                base.Delete();
                                return;
                            }
                        }
                    }
                }
            }

            base.Delete();
        }

        public BaseVoidCreature(Serial serial) : base(serial)
        {
        }

        private static bool _CheckSpawners;

        public static void RemoveVoidSpawners()
        {
            List<XmlSpawner> list = new List<XmlSpawner>();

            foreach (XmlSpawner spawner in World.Items.Values.OfType<XmlSpawner>())
            {
                if (list.Contains(spawner))
                    break;

                foreach (XmlSpawner.SpawnObject obj in spawner.SpawnObjects)
                {
                    if(obj == null || obj.TypeName == null)
                        continue;

                    Type t = ScriptCompiler.FindTypeByName(obj.TypeName, true);

                    if (t != null && t.IsSubclassOf(typeof(BaseVoidCreature)) || obj.TypeName.ToLower().StartsWith("korpre"))
                    {
                        list.Add(spawner);
                        break;
                    }
                }
            }

            list.ForEach(spawner => spawner.DoReset = true);
            Console.WriteLine("Reset {0} Void Spawn Spawners.", list.Count);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write(m_NextMutate);
            writer.Write(m_BuddyMutate);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0 && !_CheckSpawners && RemoveFromSpawners)
            {
                Console.WriteLine("Removing Spawners...");
                Timer.DelayCall(TimeSpan.FromSeconds(30), RemoveVoidSpawners);

                _CheckSpawners = true;
            }

            m_NextMutate = reader.ReadDateTime();

            if (version > 0)
                m_BuddyMutate = reader.ReadBool();
            else
                m_BuddyMutate = true;
        }
    }
}