using Server.Items;
using Server.Spells;
using Server.Spells.Mysticism;
using System;

namespace Server.Mobiles
{
    [CorpseName("a void corpse")]
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

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 30);
            SetResistance(ResistanceType.Fire, 50, 65);
            SetResistance(ResistanceType.Cold, 50, 65);
            SetResistance(ResistanceType.Poison, 50, 65);
            SetResistance(ResistanceType.Energy, 50, 65);

            SetSkill(SkillName.MagicResist, 140.0);
            SetSkill(SkillName.Tactics, 130);
            SetSkill(SkillName.Magery, 130.0);
            SetSkill(SkillName.EvalInt, 130.0);
            SetSkill(SkillName.Mysticism, 120);
            SetSkill(SkillName.Focus, 120);
            SetSkill(SkillName.Meditation, 120);
            SetSkill(SkillName.Wrestling, 130);
            SetSkill(SkillName.Necromancy, 120);
            SetSkill(SkillName.SpiritSpeak, 120);

            Fame = 15000;
            Karma = -15000;

            m_NextSummon = DateTime.UtcNow;
            m_NextAIChange = DateTime.UtcNow;
        }

        public override Poison PoisonImmune => Poison.Parasitic;
        public override bool AlwaysMurderer => true;
        public override bool ReacquireOnMovement => true;
        public override bool AcquireOnApproach => true;
        public override int AcquireOnApproachRange => 8;

        public override WeaponAbility GetWeaponAbility()
        {
            if (Weapon is BaseWeapon)
            {
                if (Utility.RandomBool())
                    return ((BaseWeapon)Weapon).PrimaryAbility;
                return ((BaseWeapon)Weapon).SecondaryAbility;
            }

            return WeaponAbility.WhirlwindAttack;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.HighScrolls, 3);
        }

        private DateTime m_NextSummon;
        private DateTime m_NextAIChange;

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null)
                return;

            if (m_NextSummon < DateTime.UtcNow && Mana > 40 && Followers + 5 <= FollowersMax)
            {
                Spell spell = new RisingColossusSpell(this, null);
                spell.Cast();
                m_NextSummon = DateTime.UtcNow + TimeSpan.FromSeconds(30);
            }

            IDamageable combatant = Combatant;

            if (m_NextAIChange < DateTime.UtcNow)
            {
                if (AIObject is MageAI)
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
            writer.Write(0);

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