using Server.Items;
using Server.Spells;
using Server.Spells.Mysticism;
using System;

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

            if (Utility.RandomBool())
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
            if (!Female)
                Utility.AssignRandomFacialHair(this, true);

            Hue = Race.RandomSkinHue();

            AddImmovableItem(new MysticBook((uint)0xFFF)); // Check
            AddImmovableItem(new GargishClothChest(Utility.RandomNeutralHue()));
            AddImmovableItem(new GargishClothArms(Utility.RandomNeutralHue()));
            AddImmovableItem(new GargishClothLegs(Utility.RandomNeutralHue()));
            AddImmovableItem(new GargishClothKilt(Utility.RandomNeutralHue()));

            if (Utility.RandomBool())
                AddImmovableItem(new GargishRobe());

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 30);
            SetResistance(ResistanceType.Fire, 50, 65);
            SetResistance(ResistanceType.Cold, 50, 65);
            SetResistance(ResistanceType.Poison, 50, 65);
            SetResistance(ResistanceType.Energy, 50, 65);

            SetSkill(SkillName.MagicResist, 140.0);
            SetSkill(SkillName.Tactics, 130);
            SetSkill(SkillName.Anatomy, 0.0, 10.0);
            SetSkill(SkillName.Magery, 130.0);
            SetSkill(SkillName.EvalInt, 130.0);
            SetSkill(SkillName.Meditation, 120);
            SetSkill(SkillName.Wrestling, 90);

            SetSkill(SkillName.Necromancy, 120);
            SetSkill(SkillName.SpiritSpeak, 120);
            SetSkill(SkillName.Mysticism, 120);
            SetSkill(SkillName.Focus, 120);

            SetSkill(SkillName.Musicianship, 100);
            SetSkill(SkillName.Discordance, 100);
            SetSkill(SkillName.Provocation, 100);
            SetSkill(SkillName.Peacemaking, 100);

            Fame = 12000;
            Karma = -12000;

            m_NextSummon = DateTime.UtcNow;
            m_NextBard = DateTime.UtcNow;
        }

        private void AddImmovableItem(Item item)
        {
            item.LootType = LootType.Blessed;
            AddItem(item);
        }

        public override Poison PoisonImmune => Poison.Lethal;
        public override bool AlwaysMurderer => true;
        public override bool ReacquireOnMovement => true;
        public override bool AcquireOnApproach => true;
        public override int AcquireOnApproachRange => 8;

        public override bool CanDiscord => true;
        public override bool CanPeace => true;
        public override bool CanProvoke => true;

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
            AddLoot(LootPack.UltraRich);
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.HighScrolls, 2);
        }

        private DateTime m_NextSummon;

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null || m_NextSummon > DateTime.UtcNow)
                return;

            if (Mana > 40 && Followers + 5 <= FollowersMax)
            {
                if (!m_Manifested && m_ManifestChance > Utility.RandomDouble())
                {
                    IDamageable m = Combatant;

                    if (m is BaseCreature && (((BaseCreature)m).Summoned || ((BaseCreature)m).Controlled))
                        m = ((BaseCreature)m).GetMaster();

                    FixedParticles(0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot);
                    BaseCreature vm = new VoidManifestation(m_Type);
                    vm.MoveToWorld(Location, Map);
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
            if (m_ManifestChance > Utility.RandomDouble())
            {
                Mobile m = LastKiller;
                if (m is BaseCreature && (((BaseCreature)m).Summoned || ((BaseCreature)m).Controlled))
                    m = ((BaseCreature)m).GetMaster();

                FixedParticles(0x3709, 1, 30, 9904, 1108, 6, EffectLayer.RightFoot);
                BaseCreature vm = new VoidManifestation(m_Type);
                vm.MoveToWorld(Location, Map);
                vm.PlaySound(vm.GetAngerSound());

                if (m != null)
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
            writer.Write(0);

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
