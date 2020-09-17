using Server.Engines.Quests;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a djinn corpse")]
    public class Djinn : BaseCreature
    {
        public static List<Djinn> Instances { get; set; }
        private SummonEfreetTimer m_Timer;

        [Constructable]
        public Djinn()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 0x311;
            Hue = 33072;
            Name = "Djinn";

            SetStr(320, 500);
            SetDex(200, 300);
            SetInt(600, 700);

            SetHits(2003);

            SetDamage(11, 13);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Fire, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 80, 90);

            SetSkill(SkillName.Wrestling, 60.0, 80.0);
            SetSkill(SkillName.Tactics, 60.0, 80.0);
            SetSkill(SkillName.MagicResist, 60.0, 80.0);
            SetSkill(SkillName.Magery, 100.0, 120.0);
            SetSkill(SkillName.EvalInt, 60.0, 110.0);
            SetSkill(SkillName.DetectHidden, 55.0);

            Fame = 15000;
            Karma = -15000;

            if (Instances == null)
                Instances = new List<Djinn>();

            Instances.Add(this);

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();

            m_Timer = new SummonEfreetTimer(this);
            m_Timer.Start();
        }

        public override void OnDeath(Container c)
        {
            List<DamageStore> rights = GetLootingRights();

            foreach (Mobile m in rights.Select(x => x.m_Mobile).Distinct())
            {
                if (m is PlayerMobile)
                {
                    PlayerMobile pm = m as PlayerMobile;

                    if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
                    {
                        Item item = new AquaGem();

                        if (m.Backpack == null || !m.Backpack.TryDropItem(m, item, false))
                        {
                            m.BankBox.DropItem(item);
                        }

                        m.SendLocalizedMessage(1154489); // You received a Quest Item!
                    }
                }
            }

            if (Instances != null && Instances.Contains(this))
                Instances.Remove(this);

            base.OnDeath(c);
        }

        public static Djinn Spawn(Point3D platLoc, Map platMap)
        {
            if (Instances != null && Instances.Count > 0)
                return null;

            Djinn creature = new Djinn
            {
                Home = platLoc,
                RangeHome = 4
            };
            creature.MoveToWorld(platLoc, platMap);

            return creature;
        }

        public class InternalSelfDeleteTimer : Timer
        {
            private readonly Djinn Mare;

            public InternalSelfDeleteTimer(Mobile p) : base(TimeSpan.FromMinutes(60))
            {
                Priority = TimerPriority.FiveSeconds;
                Mare = ((Djinn)p);
            }
            protected override void OnTick()
            {
                if (Mare.Map != Map.Internal)
                {
                    Mare.Delete();
                    Stop();
                }
            }
        }

        public Djinn(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            Instances.Remove(this);

            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;

            base.OnAfterDelete();
        }

        private class SummonEfreetTimer : Timer
        {
            //private static readonly ArrayList m_ToDrain = new ArrayList();
            private readonly Djinn m_Owner;

            public SummonEfreetTimer(Djinn owner)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_Owner = owner;
                Priority = TimerPriority.TwoFiftyMS;
            }
            //Type type = m_Owner.GetType();
            protected override void OnTick()
            {
                if (m_Owner.Deleted)
                {
                    Stop();
                    return;
                }

                IPooledEnumerable eable = m_Owner.GetMobilesInRange(10);

                foreach (Mobile m in eable)
                {
                    if (m == null || !(m is PlayerMobile))
                        continue;

                    if (m_Owner.CanBeHarmful(m) && m_Owner.Mana >= 100)
                    {
                        m_Owner.Mana -= 50;
                        int ownerlocX = m_Owner.Location.X + Utility.RandomMinMax(-5, 5);
                        int ownerlocY = m_Owner.Location.Y + Utility.RandomMinMax(-5, 5);
                        int ownerlocZ = m_Owner.Location.Z;
                        Efreet NewMobile = new Efreet();
                        NewMobile.MoveToWorld(new Point3D(ownerlocX, ownerlocY, ownerlocZ), m_Owner.Map);
                        NewMobile.Combatant = m;
                    }
                }

                eable.Free();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Instances = new List<Djinn>();
            Instances.Add(this);

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }
    }
}
