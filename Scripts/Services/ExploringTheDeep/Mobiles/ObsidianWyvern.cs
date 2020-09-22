using Server.Engines.Quests;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("an obsidian wyvern corpse")]
    public class ObsidianWyvern : BaseCreature
    {
        public static List<ObsidianWyvern> Instances { get; set; }

        [Constructable]
        public ObsidianWyvern()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Obsidian Wyvern";
            Body = 0x2E;
            Hue = 1910;
            BaseSoundID = 362;

            SetStr(1377, 1450);
            SetDex(125, 180);
            SetInt(780, 900);

            SetHits(1225, 1400);

            SetDamage(29, 35);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Fire, 25);

            SetResistance(ResistanceType.Physical, 67);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.Magery, 108.7, 115.0);
            SetSkill(SkillName.Meditation, 60.0, 87.6);
            SetSkill(SkillName.EvalInt, 110.0, 115.0);
            SetSkill(SkillName.Wrestling, 110.0, 115.0);
            SetSkill(SkillName.Tactics, 119.6, 125.0);
            SetSkill(SkillName.MagicResist, 115.0, 130.8);
            SetSkill(SkillName.Parry, 75.0, 85.0);
            SetSkill(SkillName.DetectHidden, 100.0);

            Fame = 24000;
            Karma = -24000;

            if (Instances == null)
                Instances = new List<ObsidianWyvern>();

            Instances.Add(this);

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public static ObsidianWyvern Spawn(Point3D platLoc, Map platMap)
        {
            if (Instances != null && Instances.Count > 0)
                return null;

            ObsidianWyvern creature = new ObsidianWyvern
            {
                Home = platLoc,
                RangeHome = 4
            };
            creature.MoveToWorld(platLoc, platMap);

            return creature;
        }

        public class InternalSelfDeleteTimer : Timer
        {
            private readonly ObsidianWyvern Mare;

            public InternalSelfDeleteTimer(Mobile p) : base(TimeSpan.FromMinutes(10))
            {
                Priority = TimerPriority.FiveSeconds;
                Mare = ((ObsidianWyvern)p);
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

        public override void OnAfterDelete()
        {
            Instances.Remove(this);

            base.OnAfterDelete();
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
                        Item item = new WillemHartesHat();

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

        public override bool ReacquireOnMovement => true;

        public override Poison PoisonImmune => Poison.Deadly;

        public override Poison HitPoison => Poison.Deadly;

        public override bool AutoDispel => true;

        public override bool BardImmune => true;

        public override int GetIdleSound() { return 0x2D3; }
        public override int GetHurtSound() { return 0x2D1; }

        public ObsidianWyvern(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Instances = new List<ObsidianWyvern>();
            Instances.Add(this);

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }
    }
}
