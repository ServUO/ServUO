using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an anlorvaglem corpse")]
    public class Anlorvaglem : BaseVoidCreature
    {
        public override VoidEvolution Evolution => VoidEvolution.Grouping;
        public override int Stage => 3;

        [Constructable]
        public Anlorvaglem()
            : base(AIType.AI_Melee, 10, 1, 0.6, 1.2)
        {
            Name = "anlorvaglem";
            Hue = 2071;
            Body = 152;

            SetStr(1000, 1200);
            SetDex(1000, 1200);
            SetInt(100, 1200);

            SetHits(3205);

            SetDamage(11, 13);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 20, 50);
            SetResistance(ResistanceType.Fire, 20, 60);
            SetResistance(ResistanceType.Cold, 20, 58);
            SetResistance(ResistanceType.Poison, 80, 100);
            SetResistance(ResistanceType.Energy, 30, 50);

            SetSkill(SkillName.Wrestling, 75.8, 100.0);
            SetSkill(SkillName.Tactics, 50.0, 100.0);
            SetSkill(SkillName.MagicResist, 50.9, 90.0);

            Fame = 8000;
            Karma = -8000;
        }

        public Anlorvaglem(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Lethal;

        public override bool Unprovokable => true;

        public override bool BardImmune => true;

        public override bool ReacquireOnMovement => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich);
            AddLoot(LootPack.LootItem<DaemonBone>(30, true));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
