using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a saber-toothed tiger corpse")]
    public class SabertoothedTiger : BaseCreature
    {
        [Constructable]
        public SabertoothedTiger()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "saber-toothed tiger";
            this.Body = 0x588;
            this.Female = true;

            this.SetStr(521);
            this.SetDex(403);
            this.SetInt(448);

            this.SetHits(404);

            this.SetDamage(21, 28);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Parry, 105.0, 110.0);
            this.SetSkill(SkillName.Tactics, 90.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0, 105.0);
            this.SetSkill(SkillName.DetectHidden, 75.0);
            this.SetSkill(SkillName.Focus, 95.0, 105.0);

            this.Fame = 11000;
            this.Karma = -11000;
            
            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 102.0;
        }

        public override int GetIdleSound() { return 0x673; }
        public override int GetAngerSound() { return 0x670; }
        public override int GetHurtSound() { return 0x672; }
        public override int GetDeathSound() { return 0x671; }

        public override double WeaponAbilityChance { get { return 0.5; } }

        public override WeaponAbility GetWeaponAbility()
        {
            switch(Utility.Random(3))
            {
                default:
                case 0: return WeaponAbility.Disarm; 
                case 1: return WeaponAbility.ArmorIgnore; 
                case 2: return WeaponAbility.NerveStrike;
            }
        }
        
        public override int Hides { get { return 11; } }
        public override HideType HideType { get { return HideType.Regular; } }
        public override int Meat { get { return 3; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}