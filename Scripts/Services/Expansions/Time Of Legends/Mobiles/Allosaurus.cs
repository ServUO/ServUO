using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an allosaurus corpse")]
    public class Allosaurus : BaseCreature
    {
        public override bool AttacksFocus { get { return true; } }

        [Constructable]
        public Allosaurus() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            this.Name = "an allosaurus";
            this.Body = 1290;

            this.SetStr(699, 828);
            this.SetDex(200);
            this.SetInt(127, 150);

            this.SetDamage(21, 23);

            this.SetHits(18000);
            this.SetMana(48, 70);

            this.SetResistance(ResistanceType.Physical, 65, 75);
            this.SetResistance(ResistanceType.Fire, 55, 65);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 90, 100);
            this.SetResistance(ResistanceType.Energy, 60, 70);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Fire, 50);

            this.SetSkill(SkillName.MagicResist, 100.0, 110.0);
            this.SetSkill(SkillName.Tactics, 120.0, 140.0);
            this.SetSkill(SkillName.Wrestling, 120.0, 150.0);
            this.SetSkill(SkillName.Poisoning, 50.0, 60.0);
            this.SetSkill(SkillName.Wrestling, 55.0, 65.0);
            this.SetSkill(SkillName.Parry, 80.0, 90.0);
            this.SetSkill(SkillName.Magery, 70.0, 80.0);
            this.SetSkill(SkillName.EvalInt, 75.0, 85.0);

            this.Fame = 21000;
            this.Karma = -21000;
        }

        public override void GenerateLoot()
        {
            if (IsChampionSpawn)
                this.AddLoot(LootPack.FilthyRich, 3);
            else
                this.AddLoot(LootPack.UltraRich, 3);
        }

        public override int GetIdleSound()
        {
            return 0x2C4;
        }

        public override int GetAttackSound()
        {
            return 0x2C0;
        }

        public override int GetDeathSound()
        {
            return 0x2C1;
        }

        public override int GetAngerSound()
        {
            return 0x2C4;
        }

        public override int GetHurtSound()
        {
            return 0x2C3;
        }

        public override void SetToChampionSpawn()
        {
            SetStr(347, 387);
            SetHits(940, 1000);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            switch (Utility.Random(3))
            {
                default:
                case 0: return WeaponAbility.Disarm;
                case 1: return WeaponAbility.ArmorPierce;
                case 2: return WeaponAbility.CrushingBlow;
            }
        }

        public override int Meat { get { return 3; } }
        public override int Hides { get { return 11; } }
        public override HideType HideType { get { return HideType.Horned; } }

        public Allosaurus(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}