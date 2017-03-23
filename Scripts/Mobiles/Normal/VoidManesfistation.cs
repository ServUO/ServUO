using System;
using Server;
using Server.Spells.Mysticism;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "a void corpse" )]
	public class VoidManifestation : BaseCreature
	{
        private int m_Type;

        [Constructable]
        public VoidManifestation() : this(0)
        {
        }

        [Constructable]
		public VoidManifestation(int type) : base(AIType.AI_Mystic, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
            m_Type = type;

			Name = "a void manifestation";
			Body = 740;
			Hue = 2071;
			BaseSoundID = 684;
			
			SetStr(500);
			SetInt(105);
			SetDex(150);
			
			SetHits(2400);
			SetMana(60000);
			SetStam(200);
			
			SetDamage(25, 31);
			
			this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 30);
            this.SetResistance(ResistanceType.Fire, 50, 65);
            this.SetResistance(ResistanceType.Cold, 50, 65);
            this.SetResistance(ResistanceType.Poison, 50, 65);
            this.SetResistance(ResistanceType.Energy, 50, 65);

            this.SetSkill(SkillName.MagicResist, 140.0);
            this.SetSkill(SkillName.Tactics, 130);
			this.SetSkill(SkillName.Magery, 130.0);
			this.SetSkill(SkillName.EvalInt, 130.0);
			this.SetSkill(SkillName.Mysticism, 120);
			this.SetSkill(SkillName.Focus, 120);
			this.SetSkill(SkillName.Meditation, 120);
			this.SetSkill(SkillName.Wrestling, 130);
			this.SetSkill(SkillName.Necromancy, 120);
			this.SetSkill(SkillName.SpiritSpeak, 120);

            this.Fame = 15000;
            this.Karma = -15000;

            m_NextSummon = DateTime.UtcNow;
            m_NextAIChange = DateTime.UtcNow;
		}

        public override Poison PoisonImmune { get { return Poison.Parasitic; } }
		public override bool AlwaysMurderer { get { return true; } }
		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool AcquireOnApproach{ get { return true; } }
		public override int AcquireOnApproachRange { get { return 8; } }
		
		public override WeaponAbility GetWeaponAbility()
        {
			if(this.Weapon is BaseWeapon)
            {
				if (Utility.RandomBool())
					return ((BaseWeapon)Weapon).PrimaryAbility;
				return ((BaseWeapon)Weapon).SecondaryAbility;
			}
			
			return WeaponAbility.WhirlwindAttack;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 3);
            this.AddLoot(LootPack.MedScrolls, 2);
            this.AddLoot(LootPack.HighScrolls, 3);
        }
		
		private DateTime m_NextSummon;
		private DateTime m_NextAIChange;

		public override void OnThink()
		{
			base.OnThink();
			
			if(Combatant == null)
				return;
				
			if(m_NextSummon < DateTime.UtcNow && this.Mana > 40 && this.Followers + 5 <= this.FollowersMax)
			{
				Spell spell = new RisingColossusSpell(this, null);
				spell.Cast();
				m_NextSummon = DateTime.UtcNow + TimeSpan.FromSeconds(30);
			}

            IDamageable combatant = Combatant;

			if(m_NextAIChange < DateTime.UtcNow)
			{
				if(AIObject is MageAI)
					ChangeAIType(AIType.AI_Mystic);
				else
					ChangeAIType(AIType.AI_Mage);

                Combatant = combatant;

                m_NextAIChange = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 30));
			}
		}

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            switch (m_Type)
            {
                default: break;
                case 1: c.DropItem(new VoidCrystalOfCorruptedArcaneEssence()); break;
                case 2: c.DropItem(new VoidCrystalOfCorruptedSpiritualEssence()); break;
                case 3: c.DropItem(new VoidCrystalOfCorruptedMysticalEssence()); break;
            }
        }
		
		public VoidManifestation(Serial serial) : base(serial)
		{
		}
		
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Type = reader.ReadInt();

			m_NextSummon = DateTime.UtcNow;
			m_NextAIChange = DateTime.UtcNow;
        }

	}
}