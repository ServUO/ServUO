using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a lion corpse")]
    public class Lion : BaseCreature
    {
        [Constructable]
        public Lion()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Lion";
            this.Body = 0x592;
            this.Female = true;
            this.BaseSoundID = 0x3EF;

            this.SetStr(710, 720);
            this.SetDex(200, 220);
            this.SetInt(120, 140);

            this.SetHits(350, 370);

            this.SetDamage(16, 22);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 35, 45);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 20, 40);

            this.SetSkill(SkillName.Parry, 90.0, 100.0);
            this.SetSkill(SkillName.Tactics, 100.0, 110.0);
            this.SetSkill(SkillName.Wrestling, 100.0, 110.0);
            this.SetSkill(SkillName.DetectHidden, 80.0);

            this.Fame = 11000;
            this.Karma = -11000;
            
            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 96.0;
        }

        public override int GetIdleSound() { return 0x673; }
        public override int GetAngerSound() { return 0x670; }
        public override int GetHurtSound() { return 0x672; }
        public override int GetDeathSound() { return 0x671; }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            Paralyze(defender);
        }

        #region Paralyze
        private void Paralyze(Mobile defender)
        {
            defender.Paralyze(TimeSpan.FromSeconds(Utility.Random(3)));

            defender.FixedEffect(0x376A, 6, 1);
            defender.PlaySound(0x204);

            defender.SendLocalizedMessage(1060164); // The attack has temporarily paralyzed you!
        }
        #endregion

        public override double WeaponAbilityChance { get { return 0.5; } }

        public override WeaponAbility GetWeaponAbility()
        {
            switch(Utility.Random(3))
            {
                default:
                case 0: return WeaponAbility.ArmorIgnore; 
                case 1: return WeaponAbility.ArmorPierce; 
                case 2: return WeaponAbility.BleedAttack;
            }
        }
        
        public override int Hides { get { return 11; } }
        public override HideType HideType { get { return HideType.Regular; } }
        public override int Meat { get { return 5; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 1);
        }

        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

        public Lion(Serial serial)
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