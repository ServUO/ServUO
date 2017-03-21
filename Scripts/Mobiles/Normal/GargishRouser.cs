using System;
using Server;
using Server.Spells.Mysticism;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
	public class GargishRouser : BaseCreature
	{
		private readonly double m_ManifestChance = 0.05;

        private int m_Type;
        private bool m_Manifested;
        private DateTime m_NextBard;

        [Constructable]
        public GargishRouser() : this(0)
        { 
        }

        [Constructable]
        public GargishRouser(int type)
            : base(Utility.RandomBool() ? AIType.AI_Mystic : AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
            m_Type = type;
            m_Manifested = false;

			Race = Race.Gargoyle;
			Title = "the Gargish Rouser";
			
			SetStr(150);
			SetInt(150);
			SetDex(150);
			
			SetHits(1200, 1500);
			SetMana(700, 900);
			
			SetDamage(15, 19);
			
			if(Utility.RandomBool())
			{
				Name = NameList.RandomName("Gargoyle Male");
				Female = false;
				Body = 666;
			}
			else
			{
				Name = NameList.RandomName("Gargoyle Female");
				Female = true;
				Body = 667;
			}
			
			Utility.AssignRandomHair(this, true);
			if(!Female)
				Utility.AssignRandomFacialHair(this, true);

            this.Hue = Race.RandomSkinHue();

            AddImmovableItem(new MysticBook((uint)0xFFF)); // Check
            AddImmovableItem(new GargishClothChest(Utility.RandomNeutralHue()));
            AddImmovableItem(new GargishClothArms(Utility.RandomNeutralHue()));
            AddImmovableItem(new GargishClothLegs(Utility.RandomNeutralHue()));
            AddImmovableItem(new GargishClothKilt(Utility.RandomNeutralHue()));
			
			if(Utility.RandomBool())
                AddImmovableItem(new GargishRobe());
			
			this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 30);
            this.SetResistance(ResistanceType.Fire, 50, 65);
            this.SetResistance(ResistanceType.Cold, 50, 65);
            this.SetResistance(ResistanceType.Poison, 50, 65);
            this.SetResistance(ResistanceType.Energy, 50, 65);

            this.SetSkill(SkillName.MagicResist, 140.0);
            this.SetSkill(SkillName.Tactics, 130);
            this.SetSkill(SkillName.Anatomy, 0.0, 10.0);
			this.SetSkill(SkillName.Magery, 130.0);
			this.SetSkill(SkillName.EvalInt, 130.0);
			this.SetSkill(SkillName.Meditation, 120);
			this.SetSkill(SkillName.Wrestling, 90);

            this.SetSkill(SkillName.Necromancy, 120);
            this.SetSkill(SkillName.SpiritSpeak, 120);
            this.SetSkill(SkillName.Mysticism, 120);
            this.SetSkill(SkillName.Focus, 120);

            this.SetSkill(SkillName.Musicianship, 100);
            this.SetSkill(SkillName.Discordance, 100);
            this.SetSkill(SkillName.Provocation, 100);
            this.SetSkill(SkillName.Peacemaking, 100);

            this.Fame = 12000;
            this.Karma = -12000;

            m_NextSummon = DateTime.UtcNow;
            m_NextBard = DateTime.UtcNow;
		}

        private void AddImmovableItem(Item item)
        {
            item.LootType = LootType.Blessed;
            AddItem(item);
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override bool AlwaysMurderer { get { return true; } }
		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool AcquireOnApproach{ get { return true; } }
		public override int AcquireOnApproachRange { get { return 8; } }

        public override bool CanDiscord { get { return true; } }
        public override bool CanPeace { get { return true; } }
        public override bool CanProvoke { get { return true; } }


        public override TimeSpan DiscordInterval { get { return TimeSpan.FromSeconds(60); } }
        public override TimeSpan PeaceInterval { get { return TimeSpan.FromSeconds(60); } }
        public override TimeSpan ProvokeInterval { get { return TimeSpan.FromSeconds(60); } }

		public override WeaponAbility GetWeaponAbility()
        {
			if(Weapon is BaseWeapon)
            {
				if (Utility.RandomBool())
					return ((BaseWeapon)Weapon).PrimaryAbility;
				return ((BaseWeapon)Weapon).SecondaryAbility;
			}
			
			return WeaponAbility.WhirlwindAttack;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich);
            this.AddLoot(LootPack.MedScrolls, 2);
            this.AddLoot(LootPack.HighScrolls, 2);
        }
		
		private DateTime m_NextSummon;

		public override void OnThink()
		{
			base.OnThink();
			
			if(Combatant == null || m_NextSummon > DateTime.UtcNow)
				return;
				
			if(this.Mana > 40 && this.Followers + 5 <= this.FollowersMax)
			{
                if (!m_Manifested && m_ManifestChance > Utility.RandomDouble())
                {
                    IDamageable m = this.Combatant;

                    if (m is BaseCreature && (((BaseCreature)m).Summoned || ((BaseCreature)m).Controlled))
                        m = ((BaseCreature)m).GetMaster();

                    this.FixedParticles(0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot);
                    BaseCreature vm = new VoidManifestation(m_Type);
                    vm.MoveToWorld(this.Location, this.Map);
                    vm.PlaySound(vm.GetAngerSound());

                    if (m != null)
                        vm.Combatant = m;

                    m_Manifested = true;
                    m_NextSummon = DateTime.UtcNow + TimeSpan.FromMinutes(10);
                }
                else
                {
                    Spell spell = new RisingColossusSpell(this, null);
                    spell.Cast();
                    m_NextSummon = DateTime.UtcNow + TimeSpan.FromSeconds(30);
                }
			}
		}
		
		public override bool OnBeforeDeath()
		{
			if(m_ManifestChance > Utility.RandomDouble())
			{
				Mobile m = this.LastKiller;
                if (m is BaseCreature && (((BaseCreature)m).Summoned || ((BaseCreature)m).Controlled))
					m = ((BaseCreature)m).GetMaster();
					
				this.FixedParticles( 0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot );
                BaseCreature vm = new VoidManifestation(m_Type);
				vm.MoveToWorld(this.Location, this.Map);
				vm.PlaySound(vm.GetAngerSound());
				
				if(m != null)
					vm.Combatant = m;
			}
		
			return base.OnBeforeDeath();
		}
		
		public GargishRouser(Serial serial) : base(serial)
		{
		}
		
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Type);
            writer.Write(m_Manifested);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Type = reader.ReadInt();
            m_Manifested = reader.ReadBool();

            m_NextSummon = DateTime.UtcNow;
            m_NextBard = DateTime.UtcNow;
        }
	}
}