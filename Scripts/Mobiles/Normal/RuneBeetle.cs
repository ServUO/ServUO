using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a rune beetle corpse")]
    public class RuneBeetle : BaseCreature
    {
        private static readonly Hashtable m_Table = new Hashtable();
        [Constructable]
        public RuneBeetle()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a rune beetle";
            Body = 244;

            SetStr(401, 460);
            SetDex(121, 170);
            SetInt(376, 450);

            SetHits(301, 360);

            SetDamage(15, 22);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Poison, 10);
            SetDamageType(ResistanceType.Energy, 70);

            SetResistance(ResistanceType.Physical, 40, 65);
            SetResistance(ResistanceType.Fire, 35, 50);
            SetResistance(ResistanceType.Cold, 35, 50);
            SetResistance(ResistanceType.Poison, 75, 95);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.EvalInt, 100.1, 125.0);
            SetSkill(SkillName.Magery, 100.1, 110.0);
            SetSkill(SkillName.Poisoning, 120.1, 140.0);
            SetSkill(SkillName.MagicResist, 95.1, 110.0);
            SetSkill(SkillName.Tactics, 78.1, 93.0);
            SetSkill(SkillName.Wrestling, 70.1, 77.5);

            Fame = 15000;
            Karma = -15000;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 93.9;

            SetSpecialAbility(SpecialAbility.RuneCorruption);
            SetWeaponAbility(WeaponAbility.BleedAttack);
        }

        public RuneBeetle(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Greater;
        public override Poison HitPoison => Poison.Greater;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
        public override bool CanAngerOnTame => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.MedScrolls, 1);
            AddLoot(LootPack.BodyPartsAndBones);
            AddLoot(LootPack.BonsaiSeed);
        }

        public override int GetAngerSound()
        {
            return 0x4E8;
        }

        public override int GetIdleSound()
        {
            return 0x4E7;
        }

        public override int GetAttackSound()
        {
            return 0x4E6;
        }

        public override int GetHurtSound()
        {
            return 0x4E9;
        }

        public override int GetDeathSound()
        {
            return 0x4E5;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(3);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
