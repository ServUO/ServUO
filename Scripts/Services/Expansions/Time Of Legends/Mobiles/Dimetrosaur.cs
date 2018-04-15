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
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "a dimetrosaur";
            Body = 1285;

            SetStr(526, 601);

            SetDex(166, 184);
            SetInt(373, 435);

            SetDamage(18, 21);
            SetHits(5300, 5400);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 65, 75);
            SetResistance(ResistanceType.Energy, 65, 75);

            SetDamageType(ResistanceType.Physical, 90);
            SetDamageType(ResistanceType.Poison, 10);

            SetSkill(SkillName.MagicResist, 120.0, 140.0);
            SetSkill(SkillName.Tactics, 100.0, 120.0);
            SetSkill(SkillName.Wrestling, 115.0, 125.0);
            SetSkill(SkillName.Anatomy, 70.0, 80.0);
            SetSkill(SkillName.Poisoning, 85.0, 95.0);
            SetSkill(SkillName.DetectHidden, 70.0, 80.0);
            SetSkill(SkillName.Parry, 95.0, 105.0);

            Fame = 17000;
            Karma = -17000;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 102.0;

            SetAreaEffect(AreaEffect.PoisonBreath);
            SetWeaponAbility(WeaponAbility.MortalStrike);
            SetWeaponAbility(WeaponAbility.Dismount);
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

        public override void OnAfterTame(Mobile tamer)
        {
            SetHits(HitsMax / 4);
        }

        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }
        public override int Meat { get { return 1; } }
        public override int Hides { get { return 11; } }
        public override HideType HideType { get { return HideType.Spined; } }
        public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies; } }

        public override Poison HitAreaPoison { get { return Poison.Lethal; } }
        public override int AreaPoisonDamage { get { return 50; } }

        public override void GenerateLoot()
        {
            if (IsChampionSpawn)
                AddLoot(LootPack.FilthyRich, 2);
            else
                AddLoot(LootPack.UltraRich, 2);
        }

        public Dimetrosaur(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                if (Controlled)
                {
                    SetHits(HitsMax / 4);
                }

                SetAreaEffect(AreaEffect.PoisonBreath);
                SetWeaponAbility(WeaponAbility.MortalStrike);
                SetWeaponAbility(WeaponAbility.Dismount);
            }

            if (version < 2)
            {
                SetMagicalAbility(MagicalAbility.Poisoning);
            }
        }
    }
}
