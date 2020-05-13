using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a treefellow guardian corpse")]
    public class TreefellowGuardian : BaseCreature
    {
        [Constructable]
        public TreefellowGuardian()
            : base(AIType.AI_Mystic, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            Name = "a Treefellow Guardian";
            Body = 301;

            SetStr(511, 695);
            SetDex(30, 55);
            SetInt(403, 491);

            SetHits(724, 900);

            SetDamage(12, 16);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 35);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 80, 90);

            SetSkill(SkillName.MagicResist, 40.1, 55.0);
            SetSkill(SkillName.Tactics, 65.1, 90.0);
            SetSkill(SkillName.Wrestling, 65.1, 85.0);
            SetSkill(SkillName.Spellweaving, 120.0);

            Fame = 500;
            Karma = 1500;

            SetWeaponAbility(WeaponAbility.Dismount);
        }

        public TreefellowGuardian(Serial serial)
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
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LootItem<TreefellowWood>(5.0));
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
