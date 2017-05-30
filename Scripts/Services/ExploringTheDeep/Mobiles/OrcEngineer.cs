using Server.Engines.Quests;
using Server.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("an orcish corpse")]
    public class OrcEngineer : Orc
    {
        private static readonly ArrayList m_Instances = new ArrayList();
        public static ArrayList Instances { get { return m_Instances; } }

        [Constructable]
        public OrcEngineer()
            : base()
        {
            m_Instances.Add(this);

            this.Title = "the Orcish Engineer";

            this.SetStr(500, 510);
            this.SetDex(200, 210);
            this.SetInt(200, 210);

            this.SetHits(3500, 3700);

            this.SetDamage(8, 14);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 65, 70);
            this.SetResistance(ResistanceType.Fire, 65, 70);
            this.SetResistance(ResistanceType.Cold, 65, 70);
            this.SetResistance(ResistanceType.Poison, 60);
            this.SetResistance(ResistanceType.Energy, 65, 70);

            this.SetSkill(SkillName.Parry, 100.0, 110.0);
            this.SetSkill(SkillName.MagicResist, 70.0, 80.0);
            this.SetSkill(SkillName.Tactics, 110.0, 120.0);
            this.SetSkill(SkillName.Wrestling, 120.0, 130.0);
            this.SetSkill(SkillName.DetectHidden, 100.0, 110.0);

            this.Fame = 1500;
            this.Karma = -1500;

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }

        public static OrcEngineer Spawn(Point3D platLoc, Map platMap)
        {
            if (m_Instances.Count > 0)
                return null;

            OrcEngineer creature = new OrcEngineer();
            creature.Home = platLoc;
            creature.RangeHome = 4;
            creature.MoveToWorld(platLoc, platMap);

            return creature;
        }
        
        public class InternalSelfDeleteTimer : Timer
        {
            private OrcEngineer Mare;

            public InternalSelfDeleteTimer(Mobile p) : base(TimeSpan.FromMinutes(10))
            {
                Priority = TimerPriority.FiveSeconds;
                Mare = ((OrcEngineer)p);
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
						Item item = new OrcishSchematics();
						
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

        public override void OnAfterDelete()
        {
            m_Instances.Remove(this);

            base.OnAfterDelete();
        }

        public OrcEngineer(Serial serial)
            : base(serial)
        {
            m_Instances.Add(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

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