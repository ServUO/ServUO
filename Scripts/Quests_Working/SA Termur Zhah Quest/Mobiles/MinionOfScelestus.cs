using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a minion of scelestus corpse")]
    public class MinionOfScelestus : BaseCreature
    {
        private static readonly int MaxWanderDistance = 45;

        [Constructable]
        public MinionOfScelestus()
            : base(AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.2, 0.4)
        {
            Name = "a minion of scelestus";
            Body = 9;	
            BaseSoundID = 357;
            Hue = 1159;

            SetStr(375, 405);
            SetDex(175, 200);
            SetInt(200, 225);

            SetHits(30000);

            SetDamage(19, 21);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 50);
            SetDamageType(ResistanceType.Energy, 10);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 50);
            SetResistance(ResistanceType.Cold, 100);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 50);

            SetSkill(SkillName.MagicResist, 130.8, 140.0);
            SetSkill(SkillName.Tactics, 110.0, 120.0);
            SetSkill(SkillName.Wrestling, 110.2, 120.0);
            SetSkill(SkillName.Poisoning, 120.0);
            SetSkill(SkillName.Magery, 115.0, 125.0);
            SetSkill(SkillName.EvalInt, 100.0, 120.0);

            Fame = 12500;
            Karma = -12500;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.SuperBoss);
            this.AddLoot(LootPack.UltraRich);
        }

        public override Poison PoisonImmune { get { return Poison.Parasitic; } }
        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override bool HasBreath { get { return true; } }
        public override bool TaintedLifeAura { get { return true; } }
        public override bool ReacquireOnMovement { get { return true; } }
        public override bool AcquireOnApproach { get { return true; } }
        public override int AcquireOnApproachRange { get { return 12; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            if (0.20 > Utility.RandomDouble() && (defender.Mounted || defender.Flying))
            {
                if (defender is PlayerMobile)
                {
                    if (Server.Spells.Ninjitsu.AnimalForm.UnderTransformation(defender))
                    {
                        defender.SendLocalizedMessage(1114066, this.Name); // ~1_NAME~ knocked you out of animal form!
                    }
                    else if (defender.Mounted)
                    {
                        defender.SendLocalizedMessage(1040023); // You have been knocked off of your mount!
                    }

                    ((PlayerMobile)defender).SetMountBlock(BlockMountType.Dazed, TimeSpan.FromSeconds(10), true);
                }
                else if (defender.Mount != null)
                    defender.Mount.Rider = null;

                defender.PlaySound(0x140);
                defender.FixedParticles(0x3728, 10, 15, 9955, EffectLayer.Waist);
            }

            base.OnGaveMeleeAttack(defender);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (0.50 > Utility.RandomDouble())
            {
                Item item = Loot.Construct(m_Types);

                if (item != null)
                    c.DropItem(item);
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (this.GetDistanceToSqrt(Home) > MaxWanderDistance && (Combatant == null || 0.01 > Utility.RandomDouble()))
            {
                IPooledEnumerable eable = this.GetMobilesInRange(10);
                foreach (Mobile m in eable)
                {
                    if (m.NetState != null)
                        m.SendMessage("The minion has returned to its home.");
                }
                eable.Free();

                this.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                MoveToWorld(Home, this.Map);
            }
        }

        private Type[] m_Types = new Type[]
		{
			typeof(ChallengeRite), 			typeof(AnthenaeumDecree), 		typeof(LetterFromTheKing),
			typeof(OnTheVoid),				typeof(ShilaxrinarsMemorial),	typeof(ToTheHighScholar),
			typeof(ToTheHighBroodmother),	typeof(ReplyToTheHighScholar),	typeof(AccessToTheIsle),
			typeof(InMemory)
		};

        public MinionOfScelestus(Serial serial)
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
        }
    }
}
