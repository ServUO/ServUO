using Server.Mobiles;
using System;

namespace Server.Items
{
    public class KotlAutomaton : BaseCreature, IRepairableMobile
    {
        private CraftResource _Resource;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return _Resource; }
            set
            {
                CraftResource old = _Resource;
                _Resource = value;

                if (old != _Resource)
                    OnResourceChanged();

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual Type RepairResource
        {
            get
            {
                CraftResourceInfo resInfo = CraftResources.GetInfo(_Resource);

                if (resInfo == null || resInfo.ResourceTypes.Length == 0)
                    return typeof(IronIngot);

                return resInfo.ResourceTypes[0];
            }
        }

        [Constructable]
        public KotlAutomaton()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            _Resource = CraftResource.Iron;

            Name = "kotl automaton";
            Body = 1406;
            BaseSoundID = 541;

            SetStr(793, 875);
            SetDex(67, 74);
            SetInt(255, 263);

            SetHits(774, 876);

            SetDamage(15, 20);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 50);
            SetResistance(ResistanceType.Fire, 45, 50);
            SetResistance(ResistanceType.Cold, 45, 50);
            SetResistance(ResistanceType.Poison, 45, 50);
            SetResistance(ResistanceType.Energy, 45, 50);

            SetSkill(SkillName.Anatomy, 90.3, 99.9);
            SetSkill(SkillName.MagicResist, 121.0, 126.7);
            SetSkill(SkillName.Tactics, 82.0, 94.8);
            SetSkill(SkillName.Wrestling, 94.4, 108.4);
            SetSkill(SkillName.DetectHidden, 40.0);
            SetSkill(SkillName.Parry, 70.0, 80.0);

            Fame = 14000;
            Karma = -14000;

            ControlSlots = 4;
            SetWeaponAbility(WeaponAbility.ParalyzingBlow);
            SetWeaponAbility(WeaponAbility.Disarm);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 4);
        }

        public virtual void OnResourceChanged()
        {
            Hue = 0x8000 | CraftResources.GetHue(_Resource);

            CraftResourceInfo resInfo = CraftResources.GetInfo(_Resource);

            if (resInfo == null)
                return;

            CraftAttributeInfo attrs = resInfo.AttributeInfo;

            if (attrs == null)
                return;

            SetResistance(ResistanceType.Physical, Utility.RandomMinMax(45, 50) + attrs.ArmorPhysicalResist);
            SetResistance(ResistanceType.Fire, Utility.RandomMinMax(45, 50) + attrs.ArmorFireResist);
            SetResistance(ResistanceType.Cold, Utility.RandomMinMax(45, 50) + attrs.ArmorColdResist);
            SetResistance(ResistanceType.Poison, Utility.RandomMinMax(45, 50) + attrs.ArmorPoisonResist);
            SetResistance(ResistanceType.Energy, Utility.RandomMinMax(45, 50) + attrs.ArmorEnergyResist);

            int fire = attrs.WeaponFireDamage;
            int cold = attrs.WeaponColdDamage;
            int poison = attrs.WeaponPoisonDamage;
            int energy = attrs.WeaponEnergyDamage;
            int physical = 100 - fire - cold - poison - energy;

            SetDamageType(ResistanceType.Physical, physical);
            SetDamageType(ResistanceType.Fire, fire);
            SetDamageType(ResistanceType.Cold, cold);
            SetDamageType(ResistanceType.Poison, poison);
            SetDamageType(ResistanceType.Energy, energy);
        }

        public override double GetControlChance(Mobile m, bool useBaseSkill)
        {
            if (m.Skills[SkillName.Tinkering].Base < 100.0)
            {
                m.SendLocalizedMessage(1157043); // You lack the skill to command this Automaton.
                return 0;
            }

            return 1.0;
        }

        public override bool OnBeforeDeath()
        {
            if (!Controlled)
            {
                if (Region.IsPartOf("KotlCity"))
                {
                    AutomatonStatue.OnDeath(this);
                }
            }

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            Mobile master = GetMaster();

            if (Controlled && master != null && master.Backpack != null)
            {
                BrokenAutomatonHead broke = new BrokenAutomatonHead(this);

                ControlTarget = null;
                ControlOrder = OrderType.Stay;
                Internalize();

                IsStabled = true;
                Loyalty = MaxLoyalty;

                master.Backpack.DropItem(broke); // This needs to drop regardless of weight/item count, right?

                master.SendLocalizedMessage(1157048); // A broken automaton head has been placed in your backpack.
            }

            base.OnDeath(c);
        }

        public KotlAutomaton(Serial serial)
            : base(serial)
        {
        }

        // Missing Wrestling Mastery Ability

        public override double WeaponAbilityChance => 0.33;

        public override bool IsScaredOfScaryThings => false;
        public override bool IsScaryToPets => !Controlled;
        public override FoodType FavoriteFood => FoodType.None;
        public override bool DeleteOnRelease => true;
        public override bool AutoDispel => !Controlled;
        public override bool BleedImmune => true;
        public override bool BardImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;

        public override bool CanTransfer(Mobile m)
        {
            return false;
        }

        public override bool CanFriend(Mobile m)
        {
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write((int)_Resource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Resource = (CraftResource)reader.ReadInt();

            if (version == 0)
            {
                SetWeaponAbility(WeaponAbility.ParalyzingBlow);
                SetWeaponAbility(WeaponAbility.Disarm);
            }
        }
    }
}
