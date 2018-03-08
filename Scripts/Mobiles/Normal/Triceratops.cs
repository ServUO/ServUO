using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a triceratops corpse")]
    public class Triceratops : BaseCreature
    {
        [Constructable]
        public Triceratops()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Triceratops";
            this.Body = 0x587;
            this.Female = true;

            this.SetStr(1100, 1300);
            this.SetDex(150, 170);
            this.SetInt(280, 310);

            this.SetHits(1100 , 1200);

            this.SetDamage(21, 28);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 70, 80);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.Anatomy, 65.0, 75.0);
            this.SetSkill(SkillName.Tactics, 90.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 95.0, 105.0);
            this.SetSkill(SkillName.DetectHidden, 75.0);
            this.SetSkill(SkillName.Focus, 95.0, 105.0);
            this.SetSkill(SkillName.Parry, 0.0, 105.0);

            this.Tamable = true;
            this.ControlSlots = 3;
            this.MinTameSkill = 102.0;
        }

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
                case 0: return WeaponAbility.ArmorIgnore; 
                case 1: return WeaponAbility.ArmorPierce;
                case 2: return WeaponAbility.BleedAttack;
            }
        }
        
        public override int Hides { get { return 11; } }
        public override HideType HideType { get { return HideType.Regular; } }
        public override int Meat { get { return 3; } }
        public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies; } }

        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 1);
        }

        public Triceratops(Serial serial)
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