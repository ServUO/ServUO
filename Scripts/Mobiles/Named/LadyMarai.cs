using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Lady Marai corpse")]
    public class LadyMarai : SkeletalKnight
    {
        [Constructable]
        public LadyMarai()
        {
            Name = "Lady Marai";
            Hue = 0x21;

            SetStr(221, 304);
            SetDex(98, 138);
            SetInt(54, 99);

            SetHits(694, 846);

            SetDamage(15, 25);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 60);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.Wrestling, 126.6, 137.2);
            SetSkill(SkillName.Tactics, 128.7, 134.5);
            SetSkill(SkillName.MagicResist, 102.1, 119.1);
            SetSkill(SkillName.Anatomy, 126.2, 136.5);

            Fame = 18000;
            Karma = -18000;

            SetWeaponAbility(WeaponAbility.CrushingBlow);
        }

        public LadyMarai(Serial serial)
            : base(serial)
        {
        }

        public override bool CanBeParagon => false;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
            AddLoot(LootPack.ArcanistScrolls);
            AddLoot(LootPack.LootItem<DisintegratingThesisNotes>(15.0));
            AddLoot(LootPack.Parrot);
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
