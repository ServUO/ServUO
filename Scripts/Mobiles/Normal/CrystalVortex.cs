using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a crystal vortex corpse")]
    public class CrystalVortex : BaseCreature
    {
        [Constructable]
        public CrystalVortex()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a crystal vortex";
            Body = 0xD;
            Hue = 0x2B2;
            BaseSoundID = 0x107;

            SetStr(800, 900);
            SetDex(500, 600);
            SetInt(200);

            SetHits(350, 400);
            SetMana(0);

            SetDamage(15, 20);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Cold, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 60, 80);
            SetResistance(ResistanceType.Fire, 0, 10);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 60, 90);

            SetSkill(SkillName.MagicResist, 120.0);
            SetSkill(SkillName.Tactics, 120.0);
            SetSkill(SkillName.Wrestling, 120.0);

            Fame = 17000;
            Karma = -17000;
        }

        public CrystalVortex(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Parrot);
            AddLoot(LootPack.MedScrolls);
            AddLoot(LootPack.HighScrolls);
            AddLoot(LootPack.ArcanistScrolls, Utility.RandomMinMax(0, 2));
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.06)
                c.DropItem(new JaggedCrystals());
        }

        public override int GetAngerSound()
        {
            return 0x15;
        }

        public override int GetAttackSound()
        {
            return 0x28;
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
