using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Sir Patrick corpse")]
    public class SirPatrick : SkeletalKnight
    {
        [Constructable]
        public SirPatrick()
        {
            Name = "Sir Patrick";
            Hue = 0x47E;

            SetStr(208, 319);
            SetDex(98, 132);
            SetInt(45, 91);

            SetHits(616, 884);

            SetDamage(15, 25);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 60);

            SetResistance(ResistanceType.Physical, 55, 62);
            SetResistance(ResistanceType.Fire, 40, 48);
            SetResistance(ResistanceType.Cold, 71, 80);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.Wrestling, 126.3, 136.5);
            SetSkill(SkillName.Tactics, 128.5, 143.8);
            SetSkill(SkillName.MagicResist, 102.8, 117.9);
            SetSkill(SkillName.Anatomy, 127.5, 137.2);

            Fame = 18000;
            Karma = -18000;

            SetSpecialAbility(SpecialAbility.LifeDrain);
        }

        public SirPatrick(Serial serial)
            : base(serial)
        {
        }

        public override bool CanBeParagon => false;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.ArcanistScrolls);
            AddLoot(LootPack.LootItem<DisintegratingThesisNotes>(15.0));
            AddLoot(LootPack.LootItem<AssassinChest>(5.0));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
