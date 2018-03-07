using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a blood fox corpse")]
    public class BloodFox : BaseCreature
    {
        [Constructable]
        public BloodFox() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Blood Fox";
            this.Body = 0x58f;
            this.Female = true;

            this.SetStr(300, 320);
            this.SetDex(190, 200);
            this.SetInt(170, 210);

            this.SetHits(190, 200);

            this.SetDamage(16, 22);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 55, 65);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 35);

            this.SetSkill(SkillName.MagicResist, 40.0, 50.0);
            this.SetSkill(SkillName.Tactics, 50.0, 70.0);
            this.SetSkill(SkillName.Wrestling, 75.0, 90.0);
            this.SetSkill(SkillName.DetectHidden, 50.0, 60.0);

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 72.0;
        }

        public BloodFox(Serial serial) : base(serial)
        {
        }

        public override int Meat { get { return 5; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
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