using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a renegade changeling corpse")]
    public class RenegadeChangeling : Changeling
    {
        [Constructable]
        public RenegadeChangeling()
        {
            Name = "a renegade changeling";
            Body = 264;
            BaseSoundID = 0x470;

            SetStr(180, 200);
            SetDex(300, 330);
            SetInt(500, 550);

            SetHits(2500, 3000);
            SetStam(212, 262);
            SetMana(317, 399);

            SetDamage(22, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Wrestling, 120.0, 130.0);
            SetSkill(SkillName.Tactics, 120.0, 130.0);
            SetSkill(SkillName.MagicResist, 120.0, 150.0);
            SetSkill(SkillName.Magery, 110, 120);
            SetSkill(SkillName.EvalInt, 110, 120);
            SetSkill(SkillName.Meditation, 110, 120);

            Fame = 18900;
            Karma = -18900;
        }

        public RenegadeChangeling(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
            AddLoot(LootPack.MageryScrolls, 1, 7);
            AddLoot(LootPack.Gems, 2);
            AddLoot(LootPack.LootItem<Arrow>(35));
            AddLoot(LootPack.LootItem<Bolt>(25));
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

