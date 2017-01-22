using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a archaeosaurus corpse")]
    public class Archaeosaurus : BaseCreature
    {
        public override bool AttacksFocus { get { return true; } }

        [Constructable]
        public Archaeosaurus()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "an Archaeosaurus";
            Body = 1287;
            BaseSoundID = 422;

            SetStr(405, 421);
            SetDex(301, 320);
            SetInt(201, 224);

            SetDamage(14, 16);

            SetHits(1818, 2500);

            SetResistance(ResistanceType.Physical, 2, 3);
            SetResistance(ResistanceType.Fire, 4, 5);
            SetResistance(ResistanceType.Cold, 2, 3);
            SetResistance(ResistanceType.Poison, 3, 4);
            SetResistance(ResistanceType.Energy, 3);

            SetDamageType(ResistanceType.Poison, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetSkill(SkillName.MagicResist, 100, 115);
            SetSkill(SkillName.Tactics, 90, 110);
            SetSkill(SkillName.Wrestling, 90, 110);

            PackItem(new DragonBlood(6));

            Fame = 8100;
            Karma = -8100;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
        }

        public override int Meat { get { return 1; } }
        public override int Hides { get { return 7; } }

        public override WeaponAbility GetWeaponAbility()
        {
            if (Utility.RandomBool())
                return WeaponAbility.BleedAttack;

            return WeaponAbility.TalonStrike;
        }

        public Archaeosaurus(Serial serial)
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