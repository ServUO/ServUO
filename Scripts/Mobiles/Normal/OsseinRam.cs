using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ossein ram corpse")]
    public class OsseinRam : BaseCreature
    {
        [Constructable]
        public OsseinRam() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Ossein Ram";
            Body = 0x591;
            Female = true;

            SetStr(300, 400);
            SetDex(80, 100);
            SetInt(100, 120);

            SetHits(450, 550);

            SetDamage(18, 23);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 25);
            SetDamageType(ResistanceType.Energy, 25);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 70.0, 80.0);
            SetSkill(SkillName.Tactics, 80.0, 90.0);
            SetSkill(SkillName.Wrestling, 90.0, 100.0);
            SetSkill(SkillName.DetectHidden, 40.0, 50.0);
            SetSkill(SkillName.Anatomy, 75.0, 85.0);
            SetSkill(SkillName.Necromancy, 20.0);
            SetSkill(SkillName.SpiritSpeak, 20.0);

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 72.0;

            SetMagicalAbility(MagicalAbility.BattleDefense);
            SetWeaponAbility(WeaponAbility.MortalStrike);
            SetSpecialAbility(SpecialAbility.LifeLeech);
        }

        public override int Meat { get { return 3; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

        public OsseinRam(Serial serial) : base(serial)
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