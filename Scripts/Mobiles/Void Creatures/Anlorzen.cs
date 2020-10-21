using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an anlorzen corpse")]
    public class Anlorzen : BaseVoidCreature
    {
        public override VoidEvolution Evolution => VoidEvolution.Grouping;
        public override int Stage => 1;

        [Constructable]
        public Anlorzen()
            : base(AIType.AI_Melee, 10, 1, 0.2, 0.4)
        {
            Name = "anlorzen";
            Body = 11;
            BaseSoundID = 1170;

            SetStr(600, 750);
            SetDex(666, 800);
            SetInt(850, 1000);

            SetHits(300, 400);

            SetDamage(15, 18);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 30, 50);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.MagicResist, 30.1, 60.0);
            SetSkill(SkillName.Tactics, 30.1, 70.0);
            SetSkill(SkillName.Wrestling, 50.1, 70.0);

            Fame = 5000;
            Karma = -5000;
        }

        public Anlorzen(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Lethal;

        public override Poison HitPoison => Poison.Lethal;

        public override bool BardImmune => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.LootItem<DaemonBone>(5, true));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
