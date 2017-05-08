using System;
using Server.Items;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a giant serpent corpse")]
    [TypeAlias("Server.Mobiles.Serpant")]
    public class GiantSerpent : BaseCreature
    {
        [Constructable]
        public GiantSerpent()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a giant serpent";
            this.Body = 0x15;
            this.Hue = Utility.RandomSnakeHue();
            this.BaseSoundID = 219;

            this.SetStr(186, 215);
            this.SetDex(56, 80);
            this.SetInt(66, 85);

            this.SetHits(112, 129);
            this.SetMana(0);

            this.SetDamage(7, 17);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Poison, 60);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 70, 90);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Poisoning, 70.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 25.1, 40.0);
            this.SetSkill(SkillName.Tactics, 65.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.VirtualArmor = 32;

            this.PackItem(new Bone());
            // TODO: Body parts
        }

        public GiantSerpent(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly);
            }
        }
        public override bool DeathAdderCharmable
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 4;
            }
        }
        public override int Hides
        {
            get
            {
                return 15;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.BaseSoundID == -1)
                this.BaseSoundID = 219;

            if (version == 0 && !_FixedSpawners)
            {
                _FixedSpawners = true;
                Timer.DelayCall(TimeSpan.FromSeconds(10), FixSpawners);
            }
        }

        private static bool _FixedSpawners = false;

        private void FixSpawners()
        {
            long tc = Core.TickCount;
            Console.Write("Replacing spawner entries GiantSerpent1234 with SerpentNest...");
            foreach (var spawner in World.Items.Values.OfType<XmlSpawner>())
            {
                bool changed = false;
                foreach (XmlSpawner.SpawnObject obj in spawner.SpawnObjects)
                {
                    if (obj.TypeName != null)
                    {
                        string name = obj.TypeName.ToLower();

                        if (name == "giantserpent1" || name == "giantserpent2" || name == "giantserpent3" || name == "giantserpent4")
                        {
                            obj.TypeName = "SerpentNest";
                        }
                    }
                }

                if (changed)
                    spawner.DoRespawn = true;
            }
            Console.Write("Done!");
            Console.WriteLine("Took {0} milliseconds.", Core.TickCount - tc);
        }
    }

    /*[CorpseName("a giant serpent corpse")]
    [TypeAlias("Server.Mobiles.Serpant")]
    public class GiantSerpent1 : BaseCreature
    {
        [Constructable]
        public GiantSerpent1()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a giant serpent";
            this.Body = 0x15;
            this.Hue = Utility.RandomSnakeHue();
            this.BaseSoundID = 219;

            this.SetStr(186, 215);
            this.SetDex(56, 80);
            this.SetInt(66, 85);

            this.SetHits(112, 129);
            this.SetMana(0);

            this.SetDamage(7, 17);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Poison, 60);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 70, 90);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Poisoning, 70.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 25.1, 40.0);
            this.SetSkill(SkillName.Tactics, 65.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.VirtualArmor = 32;

            this.PackItem(new Bone());
            // TODO: Body parts
        }

        public GiantSerpent1(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly);
            }
        }
        public override bool DeathAdderCharmable
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 4;
            }
        }
        public override int Hides
        {
            get
            {
                return 15;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.10)
            {
                c.DropItem(new RareSerpentEgg1());
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.BaseSoundID == -1)
                this.BaseSoundID = 219;
        }
    }

    [CorpseName("a giant serpent corpse")]
    [TypeAlias("Server.Mobiles.Serpant")]
    public class GiantSerpent2 : BaseCreature
    {
        [Constructable]
        public GiantSerpent2()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a giant serpent";
            this.Body = 0x15;
            this.Hue = Utility.RandomSnakeHue();
            this.BaseSoundID = 219;

            this.SetStr(186, 215);
            this.SetDex(56, 80);
            this.SetInt(66, 85);

            this.SetHits(112, 129);
            this.SetMana(0);

            this.SetDamage(7, 17);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Poison, 60);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 70, 90);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Poisoning, 70.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 25.1, 40.0);
            this.SetSkill(SkillName.Tactics, 65.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.VirtualArmor = 32;

            this.PackItem(new Bone());
            // TODO: Body parts
        }

        public GiantSerpent2(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly);
            }
        }
        public override bool DeathAdderCharmable
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 4;
            }
        }
        public override int Hides
        {
            get
            {
                return 15;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.10)
            {
                c.DropItem(new RareSerpentEgg2());
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.BaseSoundID == -1)
                this.BaseSoundID = 219;
        }
    }

    [CorpseName("a giant serpent corpse")]
    [TypeAlias("Server.Mobiles.Serpant")]
    public class GiantSerpent3 : BaseCreature
    {
        [Constructable]
        public GiantSerpent3()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a giant serpent";
            this.Body = 0x15;
            this.Hue = Utility.RandomSnakeHue();
            this.BaseSoundID = 219;

            this.SetStr(186, 215);
            this.SetDex(56, 80);
            this.SetInt(66, 85);

            this.SetHits(112, 129);
            this.SetMana(0);

            this.SetDamage(7, 17);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Poison, 60);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 70, 90);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Poisoning, 70.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 25.1, 40.0);
            this.SetSkill(SkillName.Tactics, 65.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.VirtualArmor = 32;

            this.PackItem(new Bone());
            // TODO: Body parts
        }

        public GiantSerpent3(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly);
            }
        }
        public override bool DeathAdderCharmable
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 4;
            }
        }
        public override int Hides
        {
            get
            {
                return 15;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.10)
            {
                c.DropItem(new RareSerpentEgg3());
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.BaseSoundID == -1)
                this.BaseSoundID = 219;
        }
    }

    [CorpseName("a giant serpent corpse")]
    [TypeAlias("Server.Mobiles.Serpant")]
    public class GiantSerpent4 : BaseCreature
    {
        [Constructable]
        public GiantSerpent4()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a giant serpent";
            this.Body = 0x15;
            this.Hue = Utility.RandomSnakeHue();
            this.BaseSoundID = 219;

            this.SetStr(186, 215);
            this.SetDex(56, 80);
            this.SetInt(66, 85);

            this.SetHits(112, 129);
            this.SetMana(0);

            this.SetDamage(7, 17);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Poison, 60);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 70, 90);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Poisoning, 70.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 25.1, 40.0);
            this.SetSkill(SkillName.Tactics, 65.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.VirtualArmor = 32;

            this.PackItem(new Bone());
            // TODO: Body parts
        }

        public GiantSerpent4(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly);
            }
        }
        public override bool DeathAdderCharmable
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 4;
            }
        }
        public override int Hides
        {
            get
            {
                return 15;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.10)
            {
                c.DropItem(new RareSerpentEgg4());
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.BaseSoundID == -1)
                this.BaseSoundID = 219;
        }
    }*/
}