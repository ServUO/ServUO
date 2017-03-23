using System;
using Server;
using Server.Spells.Mysticism;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    public class GargishOutcast : BaseCreature
    {
        [Constructable]
        public GargishOutcast()
            : base(AIType.AI_Mystic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Race = Race.Gargoyle;
            Title = "the Gargish Outcast";

            SetStr(150);
            SetInt(150);
            SetDex(150);

            SetHits(1000, 1200);
            SetMana(450, 600);

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

            this.Hue = Race.RandomSkinHue();

            BaseWeapon wep;

            switch (Utility.Random(3))
            {
                default:
                case 0: wep = new Cyclone(); break;
                case 1: wep = new SoulGlaive(); break;
                case 2: wep = new Boomerang(); break;
            }

            wep.Attributes.SpellChanneling = 1;
            AddImmovableItem(wep);
            AddImmovableItem(new GargishClothChest(Utility.RandomNeutralHue()));
            AddImmovableItem(new GargishClothArms(Utility.RandomNeutralHue()));
            AddImmovableItem(new GargishClothLegs(Utility.RandomNeutralHue()));
            AddImmovableItem(new GargishClothKilt(Utility.RandomNeutralHue()));

            if (Utility.RandomBool())
                AddImmovableItem(new GargishRobe());

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 10, 25);
            this.SetResistance(ResistanceType.Fire, 40, 65);
            this.SetResistance(ResistanceType.Cold, 40, 65);
            this.SetResistance(ResistanceType.Poison, 40, 65);
            this.SetResistance(ResistanceType.Energy, 40, 65);

            this.SetSkill(SkillName.MagicResist, 120.0);
            this.SetSkill(SkillName.Tactics, 50.1, 60.0);
            this.SetSkill(SkillName.Throwing, 120.0);
            this.SetSkill(SkillName.Anatomy, 0.0, 10.0);
            this.SetSkill(SkillName.Magery, 50.0, 80.0);
            this.SetSkill(SkillName.EvalInt, 50.0, 80.0);
            this.SetSkill(SkillName.Meditation, 120);

            this.Fame = 12000;
            this.Karma = -12000;

            if (.5 > Utility.RandomDouble())
            {
                ChangeAIType(AIType.AI_Mage);

                this.SetSkill(SkillName.Necromancy, 90, 105);
                this.SetSkill(SkillName.SpiritSpeak, 90, 105);
            }
            else
            {
                this.SetSkill(SkillName.Mysticism, 90, 105);
                this.SetSkill(SkillName.Focus, 90, 105);
            }

            m_NextSummon = DateTime.UtcNow;
        }

        private void AddImmovableItem(Item item)
        {
            item.LootType = LootType.Blessed;
            AddItem(item);
        }

        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool ReacquireOnMovement { get { return true; } }
        public override bool AcquireOnApproach { get { return true; } }
        public override int AcquireOnApproachRange { get { return 8; } }

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
            this.AddLoot(LootPack.UltraRich);
            this.AddLoot(LootPack.MedScrolls, 2);
            this.AddLoot(LootPack.HighScrolls, 2);
        }

        private DateTime m_NextSummon;

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null || m_NextSummon > DateTime.UtcNow)
                return;

            if (this.Mana > 40 && this.Followers + 4 <= this.FollowersMax)
            {
                Spell spell = new AnimatedWeaponSpell(this, null);
                spell.Cast();
                m_NextSummon = DateTime.UtcNow + TimeSpan.FromSeconds(30);
            }
        }

        public GargishOutcast(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_NextSummon = DateTime.UtcNow;
        }
    }
}