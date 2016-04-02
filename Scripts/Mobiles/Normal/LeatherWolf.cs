using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a wolf corpse")]
    public class LeatherWolf : BaseCreature
    {
        [Constructable]
        public LeatherWolf()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a leather wolf";
            this.Body = 739;

            this.SetStr(104, 104);
            this.SetDex(111, 111);
            this.SetInt(22, 22);

            this.SetHits(221, 221);
            this.SetStam(111, 111);
            this.SetMana(22, 22);

            this.SetDamage(9, 20);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 0, 40);
            this.SetResistance(ResistanceType.Fire, 0, 19);
            this.SetResistance(ResistanceType.Cold, 0, 25);
            this.SetResistance(ResistanceType.Poison, 0, 16);
            this.SetResistance(ResistanceType.Energy, 0, 11);

            this.SetSkill(SkillName.Anatomy, 0.0, 0.0);
            this.SetSkill(SkillName.MagicResist, 65.2, 70.1);
            this.SetSkill(SkillName.Tactics, 55.2, 71.5);
            this.SetSkill(SkillName.Wrestling, 60.7, 70.9);
        }

        public LeatherWolf(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Canine;
            }
        }
        public override int Hides
        {
            get
            {
                return 7;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager, 2);
        }

        public override int GetIdleSound()
        {
            return 1545;
        }

        public override int GetAngerSound()
        {
            return 1542;
        }

        public override int GetHurtSound()
        {
            return 1544;
        }

        public override int GetDeathSound()
        {
            return 1543;
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
            //return WeaponAbility.SummonPack;
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
        }
    }
}