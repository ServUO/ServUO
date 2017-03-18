﻿using System;
using Server.Items;
using System.Collections;
using Server.Engines.Quests;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a djinn corpse")]
    public class Djinn : BaseCreature
    {
        private static readonly ArrayList m_Instances = new ArrayList();
        public static ArrayList Instances { get { return m_Instances; } }
        private SummonEfreetTimer m_Timer;

        [Constructable]
        public Djinn()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Instances.Add(this);

            this.Body = 0x2EA;
            this.Name = "Djinn";

            this.SetStr(476, 505);
            this.SetDex(76, 95);
            this.SetInt(301, 325);

            this.SetHits(586, 603);

            this.SetDamage(16, 20);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Fire, 40);
            this.SetDamageType(ResistanceType.Energy, 60);

            this.SetResistance(ResistanceType.Physical, 45, 60);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.Wrestling, 60.0, 80.0);
            this.SetSkill(SkillName.Tactics, 70.0, 80.0);
            this.SetSkill(SkillName.MagicResist, 100.0, 110.0);
            this.SetSkill(SkillName.Magery, 120.0, 130.0);
            this.SetSkill(SkillName.EvalInt, 100.0, 110.0);
            this.SetSkill(SkillName.Meditation, 100.0, 110.0);

            this.Fame = 15000;
            this.Karma = -15000;

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();

            this.m_Timer = new SummonEfreetTimer(this);
            this.m_Timer.Start();
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

            base.OnDeath(c);
        }

        public static Djinn Spawn(Point3D platLoc, Map platMap)
        {
            if (m_Instances.Count > 0)
                return null;

            Djinn creature = new Djinn();
            creature.Home = platLoc;
            creature.RangeHome = 4;
            creature.MoveToWorld(platLoc, platMap);

            return creature;
        }

        public class InternalSelfDeleteTimer : Timer
        {
            private Djinn Mare;

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
                    this.Stop();
                }
            }
        }

        public Djinn(Serial serial)
            : base(serial)
        {
            m_Instances.Add(this);
        }

        public override void OnAfterDelete()
        {
            m_Instances.Remove(this);

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;

            base.OnAfterDelete();
        }

        private class SummonEfreetTimer : Timer
        {
            //private static readonly ArrayList m_ToDrain = new ArrayList();
            private readonly Djinn m_Owner;

            public SummonEfreetTimer(Djinn owner)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                this.m_Owner = owner;
                this.Priority = TimerPriority.TwoFiftyMS;
            }
            //Type type = m_Owner.GetType();
            protected override void OnTick()
            {
                if (this.m_Owner.Deleted)
                {
                    this.Stop();
                    return;
                }

                foreach (Mobile m in this.m_Owner.GetMobilesInRange(10))
                {
                    if (m == null || !(m is PlayerMobile))
                        continue;

                    if (this.m_Owner.CanBeHarmful(m) && m_Owner.Mana >= 100)
                    {
                        m_Owner.Mana -= 50;
                        int ownerlocX = this.m_Owner.Location.X + Utility.RandomMinMax(-5, 5);
                        int ownerlocY = this.m_Owner.Location.Y + Utility.RandomMinMax(-5, 5);
                        int ownerlocZ = this.m_Owner.Location.Z;
                        Efreet NewMobile = new Efreet();
                        NewMobile.MoveToWorld(new Point3D(ownerlocX, ownerlocY, ownerlocZ), this.m_Owner.Map);
                        NewMobile.Combatant = m;
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }
    }
}
