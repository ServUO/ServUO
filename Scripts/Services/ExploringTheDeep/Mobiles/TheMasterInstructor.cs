using Server.Items;
using System;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("an ancient liche's corpse")]
    public class TheMasterInstructor : BaseCreature
    {
        private static readonly ArrayList m_Instances = new ArrayList();
        public static ArrayList Instances => m_Instances;
        private readonly SorcerersPlateController m_Controller;

        [CommandProperty(AccessLevel.GameMaster)]
        public SorcerersPlateController Controller => m_Controller;

        [Constructable]
        public TheMasterInstructor(SorcerersPlateController controller)
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Instances.Add(this);
            m_Controller = controller;

            Name = "Anshu";
            Title = "The Master Instructor";
            Body = 0x4e;
            BaseSoundID = 412;
            Hue = 1284;

            SetStr(216, 305);
            SetDex(96, 115);
            SetInt(966, 1045);

            SetHits(700, 800);

            SetDamage(15, 27);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 40);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.EvalInt, 120.1, 130.0);
            SetSkill(SkillName.Magery, 120.1, 130.0);
            SetSkill(SkillName.Meditation, 100.1, 101.0);
            SetSkill(SkillName.Poisoning, 100.1, 101.0);
            SetSkill(SkillName.MagicResist, 175.2, 200.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 75.1, 100.0);
            SetSkill(SkillName.Necromancy, 120.0);
            SetSkill(SkillName.SpiritSpeak, 120.0);
            SetSkill(SkillName.DetectHidden, 100.0);

            Fame = 23000;
            Karma = -23000;

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }

        public class InternalSelfDeleteTimer : Timer
        {
            private readonly TheMasterInstructor Mare;

            public InternalSelfDeleteTimer(Mobile p) : base(TimeSpan.FromMinutes(10))
            {
                Priority = TimerPriority.FiveSeconds;
                Mare = ((TheMasterInstructor)p);
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

        public TheMasterInstructor(Serial serial)
            : base(serial)
        {
            m_Instances.Add(this);
        }

        public override bool AlwaysMurderer => true;

        public override bool Unprovokable => true;

        public override bool BleedImmune => true;

        public override Poison PoisonImmune => Poison.Lethal;

        public override int TreasureMapLevel => 5;

        public override bool AutoDispel => true;

        public override int GetIdleSound() { return 0x19D; }
        public override int GetAngerSound() { return 0x175; }
        public override int GetDeathSound() { return 0x108; }
        public override int GetAttackSound() { return 0xE2; }
        public override int GetHurtSound() { return 0x28B; }

        public override bool OnBeforeDeath()
        {
            Mobile killer = RandomPlayerWithLootingRights();

            if (killer != null)
            {
                Item item = new StrongboxKey();

                Container pack = killer.Backpack;

                if (pack == null || !pack.TryDropItem(killer, item, false))
                    killer.BankBox.DropItem(item);

                killer.SendLocalizedMessage(1154489); // You received a Quest Item!
            }

            Timer.DelayCall(TimeSpan.FromMinutes(10.0), m_Controller.CreateSorcerersPlates);

            return base.OnBeforeDeath();
        }

        public static TheMasterInstructor Spawn(Point3D platLoc, Map platMap, SorcerersPlateController controller)
        {
            if (m_Instances.Count > 0)
                return null;

            TheMasterInstructor creature = new TheMasterInstructor(controller)
            {
                Home = platLoc,
                RangeHome = 4
            };
            creature.MoveToWorld(platLoc, platMap);

            return creature;
        }

        public override void OnAfterDelete()
        {
            m_Instances.Remove(this);

            base.OnAfterDelete();
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.NecroRegs, 30, 275);
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

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }
    }
}
