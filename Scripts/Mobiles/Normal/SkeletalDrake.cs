/* Based on Skeletal Dragon, still have to get detailed info on Skeletal Drake
    2017/4/09 update: brought more into line with the one extant data point*/
using System;

namespace Server.Mobiles
{
    [CorpseName("a skeletal drake corpse")]
    public class SkeletalDrake : BaseCreature
    {
        [Constructable]
        public SkeletalDrake()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a skeletal drake";
            Body = 104;
            Hue = 2101;
            BaseSoundID = 0x488;

            SetStr(600, 700);
            SetDex(70, 100);
            SetInt(300, 400);

            SetHits(300, 400);

            SetDamage(29, 35);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Fire, 25);

            SetResistance(ResistanceType.Physical, 75, 80);
            SetResistance(ResistanceType.Fire, 40, 60);
            SetResistance(ResistanceType.Cold, 40, 60);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.EvalInt, 45, 60);
            SetSkill(SkillName.Magery, 50, 65);
            SetSkill(SkillName.MagicResist, 75, 90);
            SetSkill(SkillName.Tactics, 70, 85);
            SetSkill(SkillName.Wrestling, 60, 75);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 80;
        }

        public SkeletalDrake(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel { get { return true; } }
        public override bool BleedImmune { get { return true; } }
        public override bool ReacquireOnMovement { get { return true; } }
        public override int Hides { get { return 20; } }
        public override int Meat { get { return 19; } }// where's it hiding these? :)
        public override HideType HideType { get { return HideType.Barbed; } }
        public override OppositionGroup OppositionGroup { get { return OppositionGroup.FeyAndUndead; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override TribeType Tribe { get { return TribeType.Undead; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Gems, 4);
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
