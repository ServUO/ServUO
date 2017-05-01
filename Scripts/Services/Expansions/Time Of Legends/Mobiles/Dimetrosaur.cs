using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a dimetrosaur corpse")]
    public class Dimetrosaur : BaseCreature
    {
        public override bool AttacksFocus { get { return !Controlled; } }

        [Constructable]
        public Dimetrosaur()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, .2, .4)
        {
            this.Name = "a dimetrosaur";
            this.Body = 1285;

            this.SetStr(526, 601);

            this.SetDex(166, 184);
            this.SetInt(373, 435);

            this.SetDamage(18, 21);
            this.SetHits(5300, 5400);

            this.SetResistance(ResistanceType.Physical, 80, 90);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 65, 75);
            this.SetResistance(ResistanceType.Energy, 65, 75);

            this.SetDamageType(ResistanceType.Physical, 90);
            this.SetDamageType(ResistanceType.Poison, 10);

            this.SetSkill(SkillName.MagicResist, 120.0, 140.0);
            this.SetSkill(SkillName.Tactics, 100.0, 120.0);
            this.SetSkill(SkillName.Wrestling, 115.0, 125.0);
            this.SetSkill(SkillName.Anatomy, 70.0, 80.0);
            this.SetSkill(SkillName.Poisoning, 85.0, 95.0);
            this.SetSkill(SkillName.DetectHidden, 70.0, 80.0);
            this.SetSkill(SkillName.Parry, 95.0, 105.0);

            this.Fame = 17000;
            this.Karma = -17000;

            this.Tamable = true;
            this.ControlSlots = 3;
            this.MinTameSkill = 102.0;
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
            SetStr(271, 299);
            SetHits(360, 380);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            switch (Utility.Random(2))
            {
                default:
                case 0: return WeaponAbility.MortalStrike;
                case 1: return WeaponAbility.Dismount;
            }
        }

        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }
        public override int Meat { get { return 1; } }
        public override int Hides { get { return 11; } }
        public override HideType HideType { get { return HideType.Spined; } }
        public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies; } }

        public override bool CanAreaPoison { get { return !this.Controlled; } }
        public override Poison HitAreaPoison { get { return Poison.Lethal; } }
        public override int AreaPoisonDamage { get { return 0; } }
        public override double AreaPosionChance { get { return 1.0; } }
        public override TimeSpan AreaPoisonDelay { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(20, 40)); } }

        public override bool HasBreath { get { return true; } }
        public override int BreathPoisonDamage { get { return 100; } }

        public override void GenerateLoot()
        {
            if (IsChampionSpawn)
                this.AddLoot(LootPack.FilthyRich, 2);
            else
                this.AddLoot(LootPack.UltraRich, 2);
        }

        public Dimetrosaur(Serial serial)
            : base(serial)
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