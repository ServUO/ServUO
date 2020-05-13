using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Lady Sabrix corpse")]
    public class LadySabrix : GiantBlackWidow
    {
        [Constructable]
        public LadySabrix()
        {
            Name = "Lady Sabrix";
            Hue = 0x497;

            SetStr(82, 130);
            SetDex(117, 146);
            SetInt(50, 98);

            SetHits(233, 361);
            SetStam(117, 146);
            SetMana(50, 98);

            SetDamage(15, 22);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 39);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 35, 44);

            SetSkill(SkillName.Wrestling, 109.8, 122.8);
            SetSkill(SkillName.Tactics, 102.8, 120.0);
            SetSkill(SkillName.MagicResist, 79.4, 95.1);
            SetSkill(SkillName.Anatomy, 68.8, 105.1);
            SetSkill(SkillName.Poisoning, 97.8, 116.7);

            Fame = 18900;
            Karma = -18900;

            SetWeaponAbility(WeaponAbility.ArmorIgnore);
        }

        public LadySabrix(Serial serial)
            : base(serial)
        {
        }

        public override bool CanBeParagon => false;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.ArcanistScrolls);
            AddLoot(LootPack.LootItem<SabrixsEye>(20.0));
            AddLoot(LootPack.Parrot);
            AddLoot(LootPack.RandomLootItem(new System.Type[] { typeof(PaladinArms), typeof(HunterLegs) }, 25.0, 1, false, false));
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
