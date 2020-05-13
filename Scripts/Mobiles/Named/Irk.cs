using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an Irk corpse")]
    public class Irk : Changeling
    {
        [Constructable]
        public Irk()
        {
            Hue = DefaultHue;

            SetStr(23, 183);
            SetDex(259, 360);
            SetInt(374, 600);

            SetHits(1006, 1064);
            SetStam(259, 360);
            SetMana(374, 600);

            SetDamage(14, 20);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 41, 50);
            SetResistance(ResistanceType.Energy, 40, 49);

            SetSkill(SkillName.Wrestling, 120.3, 123.0);
            SetSkill(SkillName.Tactics, 120.1, 131.8);
            SetSkill(SkillName.MagicResist, 132.3, 165.8);
            SetSkill(SkillName.Magery, 108.9, 119.7);
            SetSkill(SkillName.EvalInt, 108.4, 120.0);
            SetSkill(SkillName.Meditation, 108.9, 119.1);

            Fame = 21000;
            Karma = -21000;

            SetSpecialAbility(SpecialAbility.AngryFire);
        }

        public override bool CanBeParagon => false;

        public Irk(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName => "Irk";
        public override int DefaultHue => 0x489;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.ArcanistScrolls);
            AddLoot(LootPack.LootItem<IrksBrain>(25.0));
            AddLoot(LootPack.LootItem<PaladinGloves>(2.5));
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
