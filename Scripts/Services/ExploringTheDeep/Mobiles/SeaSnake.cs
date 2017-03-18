using System;

namespace Server.Mobiles
{
    [CorpseName("a sea snake corpse")]
    public class SeaSnake : BaseCreature
    {
        [Constructable]
        public SeaSnake()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Body = 92;
            this.Name = "a sea snake";
            this.BaseSoundID = 219;
            this.Hue = 2041;

            this.SetStr(261);
            this.SetDex(193);
            this.SetInt(39);

            this.SetHits(194);

            this.SetDamage(5, 21);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Poison, 50);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 5, 10);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 5, 10);

            this.SetSkill(SkillName.Poisoning, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 95.1, 100.0);
            this.SetSkill(SkillName.Tactics, 80.1, 95.0);
            this.SetSkill(SkillName.Wrestling, 85.1, 100.0);

            this.Fame = 7000;
            this.Karma = -7000;

            this.VirtualArmor = 40;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems, 2);
        }

        public override bool DeathAdderCharmable { get { return true; } }
        public override int Meat { get { return 1; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override Poison HitPoison { get { return Poison.Deadly; } }

        public SeaSnake(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
