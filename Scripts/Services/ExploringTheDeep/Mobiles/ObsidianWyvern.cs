using System;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("an obsidian wyvern corpse")]
    public class ObsidianWyvern : BaseCreature
    {
        private static readonly ArrayList m_Instances = new ArrayList();
        public static ArrayList Instances { get { return m_Instances; } }

        [Constructable]
        public ObsidianWyvern()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Instances.Add(this);

            this.Name = "Obsidian Wyvern";
            this.Body = 46;
            this.Hue = 1175;
            this.BaseSoundID = 362;

            this.SetStr(1377);
            this.SetDex(125);
            this.SetInt(780);

            this.SetHits(1225);

            this.SetDamage(29, 35);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Fire, 25);

            this.SetResistance(ResistanceType.Physical, 67);
            this.SetResistance(ResistanceType.Fire, 82);
            this.SetResistance(ResistanceType.Cold, 72);
            this.SetResistance(ResistanceType.Poison, 62);
            this.SetResistance(ResistanceType.Energy, 66);

            this.SetSkill(SkillName.Magery, 108.7);
            this.SetSkill(SkillName.Meditation, 87.6);
            this.SetSkill(SkillName.EvalInt, 113.5);
            this.SetSkill(SkillName.Wrestling, 111.8);
            this.SetSkill(SkillName.Tactics, 119.6);
            this.SetSkill(SkillName.MagicResist, 130.8);

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();

            this.Fame = 24000;
            this.Karma = -24000;
            
            this.VirtualArmor = 70;
        }

        public static ObsidianWyvern Spawn(Point3D platLoc, Map platMap)
        {
            if (m_Instances.Count > 0)
                return null;

            ObsidianWyvern creature = new ObsidianWyvern();
            creature.Home = platLoc;
            creature.RangeHome = 4;
            creature.MoveToWorld(platLoc, platMap);

            return creature;
        }

        public class InternalSelfDeleteTimer : Timer
        {
            private ObsidianWyvern Mare;

            public InternalSelfDeleteTimer(Mobile p) : base(TimeSpan.FromMinutes(60))
            {
                Priority = TimerPriority.FiveSeconds;
                Mare = ((ObsidianWyvern)p);
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

        public override bool OnBeforeDeath()
        {
            Mobile killer = DemonKnight.FindRandomPlayer(this);

            if (killer != null)
            {
                Item item = new WillemHartesHat();

                Container pack = killer.Backpack;

                if (pack == null || !pack.TryDropItem(killer, item, false))
                    killer.BankBox.DropItem(item);

                killer.SendLocalizedMessage(1154489); // You received a Quest Item!
            }

            return base.OnBeforeDeath();
        }        

        public override bool HasBreath { get { return true; } }
        public override bool ReacquireOnMovement { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override bool AutoDispel { get { return true; } }
        public override bool BardImmune { get { return true; } }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
            this.AddLoot(LootPack.Gems, 5);
        }

        public override int GetIdleSound() { return 0x2D3; }
        public override int GetHurtSound() { return 0x2D1; }

        public ObsidianWyvern(Serial serial)
            : base(serial)
        {
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
