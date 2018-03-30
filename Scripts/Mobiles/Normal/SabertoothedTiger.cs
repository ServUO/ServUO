using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a saber-toothed tiger corpse")]
    public class SabertoothedTiger : BaseCreature
    {
        public override double HealChance { get { return .167; } }

        [Constructable]
        public SabertoothedTiger()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "saber-toothed tiger";
            Body = 0x588;
            Female = true;

            SetStr(496, 523);
            SetDex(386, 403);
            SetInt(443, 469);

            SetHits(362, 423);

            SetDamage(21, 28);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Parry, 105.0, 110.0);
            SetSkill(SkillName.Tactics, 90.0, 100.0);
            SetSkill(SkillName.Wrestling, 100.0, 105.0);
            SetSkill(SkillName.DetectHidden, 75.0);
            SetSkill(SkillName.Focus, 95.0, 105.0);

            Fame = 11000;
            Karma = -11000;
            
            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 102.0;

            SetMagicalAbility(MagicalAbility.Slashing);
        }

        public override int GetIdleSound() { return 0x673; }
        public override int GetAngerSound() { return 0x670; }
        public override int GetHurtSound() { return 0x672; }
        public override int GetDeathSound() { return 0x671; }

        public override double WeaponAbilityChance { get { return 0.5; } }
        
        public override int Hides { get { return 11; } }
        public override HideType HideType { get { return HideType.Regular; } }
        public override int Meat { get { return 3; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public override bool CanAngerOnTame { get { return true; } }

        public override void OnAfterTame(Mobile master)
        {
            int intel = RawInt;
            int mana = ManaMax;

            // UO Stratics say they keep their intelligence. Crappy but hey!
            Server.SkillHandlers.AnimalTaming.ScaleStats(this, 0.5);

            SetInt(intel);
            SetMana(mana);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 1);
        }

        public SabertoothedTiger(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                SetMagicalAbility(MagicalAbility.Slashing);
            }
        }
    }
}