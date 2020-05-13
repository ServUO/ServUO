using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a treefellow corpse")]
    public class FeralTreefellow : BaseCreature
    {
        [Constructable]
        public FeralTreefellow()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a feral treefellow";
            Body = 301;

            SetStr(1351, 1600);
            SetDex(301, 550);
            SetInt(651, 900);

            SetHits(1170, 1320);

            SetDamage(26, 35);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.MagicResist, 40.1, 55.0);// Unknown
            SetSkill(SkillName.Tactics, 65.1, 90.0);// Unknown
            SetSkill(SkillName.Wrestling, 65.1, 85.0);// Unknown

            Fame = 1000;  //Unknown
            Karma = -3000;  //Unknown

            SetWeaponAbility(WeaponAbility.Dismount);
        }

        public FeralTreefellow(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune => true;

        public override int GetIdleSound()
        {
            return 443;
        }

        public override int GetDeathSound()
        {
            return 31;
        }

        public override int GetAttackSound()
        {
            return 672;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average); //Unknown
            AddLoot(LootPack.LootItem<Log>(Utility.RandomMinMax(23, 34)));
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
