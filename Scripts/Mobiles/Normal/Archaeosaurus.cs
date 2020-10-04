using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an archaeosaurus corpse")]
    public class Archaeosaurus : BaseCreature
    {
        public override bool AttacksFocus => true;

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

            SetSkill(SkillName.MagicResist, 100.0, 115.0);
            SetSkill(SkillName.Tactics, 90.0, 110.0);
            SetSkill(SkillName.Wrestling, 90.0, 110.0);
            SetSkill(SkillName.DetectHidden, 60.0, 70.0);
            SetSkill(SkillName.EvalInt, 95.0, 105.0);
            SetSkill(SkillName.Ninjitsu, 120.0);

            Fame = 8100;
            Karma = -8100;

            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetWeaponAbility(WeaponAbility.TalonStrike);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
        }

        public override int Meat => 1;
        public override MeatType MeatType => MeatType.DinoRibs;
        public override int Hides => 7;
        public override int DragonBlood => 6;
        public override int TreasureMapLevel => 1;

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
