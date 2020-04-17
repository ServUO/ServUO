using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a blood fox corpse")]
    public class BloodFox : BaseCreature
    {
        [Constructable]
        public BloodFox() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Blood Fox";
            Body = 0x58f;
            Female = true;

            SetStr(300, 320);
            SetDex(190, 200);
            SetInt(170, 210);

            SetHits(190, 200);

            SetDamage(16, 22);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 55, 65);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 35);

            SetSkill(SkillName.MagicResist, 40.0, 50.0);
            SetSkill(SkillName.Tactics, 50.0, 70.0);
            SetSkill(SkillName.Wrestling, 75.0, 90.0);
            SetSkill(SkillName.DetectHidden, 50.0, 60.0);

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 72.0;

            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetSpecialAbility(SpecialAbility.GraspingClaw);
        }

        public BloodFox(Serial serial) : base(serial)
        {
        }

        public override int Meat => 5;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override bool CanAngerOnTame => true;
        public override bool StatLossAfterTame => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                SetSpecialAbility(SpecialAbility.GraspingClaw);
                SetWeaponAbility(WeaponAbility.BleedAttack);
            }
        }
    }
}