using Server.Items;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("an ancient liche's corpse")]
    public class TheMasterInstructor : BaseCreature
    {
        private static readonly ArrayList m_Instances = new ArrayList();
        public static ArrayList Instances { get { return m_Instances; } }
        private SorcerersPlateController m_Controller;

        [CommandProperty(AccessLevel.GameMaster)]
        public SorcerersPlateController Controller
        {
            get
            {
                return this.m_Controller;
            }
        }

        [Constructable]
        public TheMasterInstructor(SorcerersPlateController controller)
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Instances.Add(this);
            m_Controller = controller;

            this.Name = "Anshu";
            this.Title = "The Master Instructor";
            this.Body = 0x4e;
            this.BaseSoundID = 412;
            this.Hue = 1284;

            this.SetStr(216, 305);
            this.SetDex(96, 115);
            this.SetInt(966, 1045);

            this.SetHits(700, 800);

            this.SetDamage(15, 27);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Cold, 40);
            this.SetDamageType(ResistanceType.Energy, 40);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.EvalInt, 120.1, 130.0);
            this.SetSkill(SkillName.Magery, 120.1, 130.0);
            this.SetSkill(SkillName.Meditation, 100.1, 101.0);
            this.SetSkill(SkillName.Poisoning, 100.1, 101.0);
            this.SetSkill(SkillName.MagicResist, 175.2, 200.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 75.1, 100.0);
            this.SetSkill(SkillName.Necromancy, 120.0);
            this.SetSkill(SkillName.SpiritSpeak, 120.0);
			this.SetSkill(SkillName.DetectHidden, 100.0);

            this.Fame = 23000;
            this.Karma = -23000;

            this.VirtualArmor = 60;
            this.PackNecroReg(30, 275);

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }
        
        public class InternalSelfDeleteTimer : Timer
        {
            private TheMasterInstructor Mare;

            public InternalSelfDeleteTimer(Mobile p) : base(TimeSpan.FromMinutes(60))
            {
                Priority = TimerPriority.FiveSeconds;
                Mare = ((TheMasterInstructor)p);
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

        public TheMasterInstructor(Serial serial)
            : base(serial)
        {
            m_Instances.Add(this);
        }
        
        public override bool AlwaysMurderer { get { return true; } }
        public override OppositionGroup OppositionGroup { get { return OppositionGroup.FeyAndUndead; } }
        public override bool Unprovokable { get { return true; } }
        public override bool BleedImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override int TreasureMapLevel { get { return 5; } }
		public override bool AutoDispel { get { return true; } }

        public override int GetIdleSound() { return 0x19D; }
        public override int GetAngerSound() { return 0x175; }
        public override int GetDeathSound() { return 0x108; }
        public override int GetAttackSound() { return 0xE2; }
        public override int GetHurtSound() { return 0x28B; }

        public override bool OnBeforeDeath()
        {
            Mobile killer = DemonKnight.FindRandomPlayer(this);

            if (killer != null)
            {
                Item item = new StrongboxKey();

                Container pack = killer.Backpack;

                if (pack == null || !pack.TryDropItem(killer, item, false))
                    killer.BankBox.DropItem(item);

                killer.SendLocalizedMessage(1154489); // You received a Quest Item!
            }

            Timer.DelayCall(TimeSpan.FromMinutes(10.0), new TimerCallback(m_Controller.CreateSorcerersPlates));

            return base.OnBeforeDeath();
        }

        public static TheMasterInstructor Spawn(Point3D platLoc, Map platMap, SorcerersPlateController controller)
        {
            if (m_Instances.Count > 0)
                return null;
            
            TheMasterInstructor creature = new TheMasterInstructor(controller);
            creature.Home = platLoc;
            creature.RangeHome = 4;
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
            this.AddLoot(LootPack.FilthyRich, 3);
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
