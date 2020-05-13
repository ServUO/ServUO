using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a treefellow corpse")]
    public class Treefellow : BaseCreature
    {
        [Constructable]
        public Treefellow()
            : base(AIType.AI_Melee, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            Name = "a treefellow";
            Body = 301;

            SetStr(196, 220);
            SetDex(31, 55);
            SetInt(66, 90);

            SetHits(118, 132);

            SetDamage(12, 16);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20, 25);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 30, 35);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 40.1, 55.0);
            SetSkill(SkillName.Tactics, 65.1, 90.0);
            SetSkill(SkillName.Wrestling, 65.1, 85.0);

            Fame = 500;
            Karma = 1500;

            SetWeaponAbility(WeaponAbility.Dismount);
        }

        public Treefellow(Serial serial)
            : base(serial)
        {
        }

        public override TribeType Tribe => TribeType.Fey;

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
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LootItem<Log>(23, 34));
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
