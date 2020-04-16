
namespace Server.Mobiles
{
    public class DazzledHarpy : Harpy
    {
        [Constructable]
        public DazzledHarpy()
        {
            Name = "a dazzled harpy";
            FightMode = FightMode.Aggressor;

            SetHits(120, 140);
            SetStam(90, 110);
            SetMana(50, 80);

            SetDamage(5, 7);

            SetSkill(SkillName.MagicResist, 50, 65);
            SetSkill(SkillName.Tactics, 70, 100);
            SetSkill(SkillName.Wrestling, 60, 90);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }

        public DazzledHarpy(Serial serial) : base(serial)
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

    public class VampireMongbat : Mongbat
    {
        [Constructable]
        public VampireMongbat()
        {
            Name = "a vampire mongbat";
            FightMode = FightMode.Aggressor;
            Hue = 1461;

            SetHits(76, 171);

            SetStr(60, 100);
            SetDex(60, 80);
            SetInt(10, 30);

            SetHits(70, 170);
            SetStam(60, 80);
            SetMana(10, 30);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 10, 20);
            SetSkill(SkillName.Tactics, 10, 20);
            SetSkill(SkillName.MagicResist, 25, 40);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }

        public VampireMongbat(Serial serial) : base(serial)
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

    public class HeadlessMiner : HeadlessOne
    {
        [Constructable]
        public HeadlessMiner()
        {
            Name = "headless miner";
            FightMode = FightMode.Aggressor;

            SetStr(60, 100);
            SetDex(40, 60);
            SetInt(10, 30);

            SetHits(10, 40);
            SetStam(40, 60);
            SetMana(10, 35);

            SetDamage(7, 12);

            SetSkill(SkillName.MagicResist, 30, 40);
            SetSkill(SkillName.Tactics, 40, 60);
            SetSkill(SkillName.Wrestling, 40, 60);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }

        public HeadlessMiner(Serial serial) : base(serial)
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

    public class StrangeGazer : Gazer
    {
        [Constructable]
        public StrangeGazer()
        {
            Name = "a strange gazer";

            SetStr(100, 130);
            SetDex(90, 120);
            SetInt(150, 160);

            SetHits(60, 90);
            SetStam(90, 120);
            SetMana(153, 158);

            SetDamage(5, 10);

            SetSkill(SkillName.MagicResist, 60, 80);
            SetSkill(SkillName.Tactics, 50, 70);
            SetSkill(SkillName.Wrestling, 50, 70);
            SetSkill(SkillName.Magery, 50, 70);
            SetSkill(SkillName.EvalInt, 50, 70);
            SetSkill(SkillName.Meditation, 50, 100);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager, 2);
        }

        public StrangeGazer(Serial serial) : base(serial)
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