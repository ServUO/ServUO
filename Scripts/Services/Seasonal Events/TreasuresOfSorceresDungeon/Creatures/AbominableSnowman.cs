using Server.Items;
using Server.Mobiles;

namespace Server.Engines.SorcerersDungeon
{
    [CorpseName("an abominable snowman corpse")]
    public class AbominableSnowman : BaseCreature
    {
        [Constructable]
        public AbominableSnowman()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "the abominable snowman";
            Body = 241;
            BaseSoundID = 367;
            Hue = 1150;

            SetStr(800);
            SetDex(150);
            SetInt(1200);

            SetHits(8000);

            SetDamage(21, 27);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 100);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.Anatomy, 115, 120);
            SetSkill(SkillName.Poisoning, 120);
            SetSkill(SkillName.MagicResist, 115, 120);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 120, 130);

            Fame = 16000;
            Karma = -16000;

            SetWeaponAbility(WeaponAbility.ConcussionBlow);
            SetWeaponAbility(WeaponAbility.CrushingBlow);
            SetSpecialAbility(SpecialAbility.TrueFear);
        }

        public AbominableSnowman(Serial serial)
            : base(serial)
        {
        }

        public override bool CanFlee => false;
        public override bool AlwaysMurderer => true;
        public override Poison PoisonImmune => Poison.Deadly;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
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
