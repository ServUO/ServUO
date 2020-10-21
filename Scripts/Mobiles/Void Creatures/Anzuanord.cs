using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an anzuanord corpse")]
    public class Anzuanord : BaseVoidCreature
    {
        public override VoidEvolution Evolution => VoidEvolution.Survival;
        public override int Stage => 1;

        [Constructable]
        public Anzuanord()
            : base(AIType.AI_Mage, 10, 1, 0.2, 0.4)
        {
            Name = "anzuanord";
            Body = 74;
            Hue = 2071;
            BaseSoundID = 422;

            SetStr(705);
            SetDex(900, 910);
            SetInt(900, 1000);

            SetHits(180);

            SetDamage(8, 10);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 0, 10);
            SetResistance(ResistanceType.Poison, 15, 20);
            SetResistance(ResistanceType.Physical, 15, 10);
            SetResistance(ResistanceType.Poison, 0, 20);
            SetResistance(ResistanceType.Physical, 100);

            SetSkill(SkillName.Anatomy, 5.0, 10.0);
            SetSkill(SkillName.MagicResist, 40.0, 50.0);
            SetSkill(SkillName.Tactics, 40.0, 50.0);
            SetSkill(SkillName.Wrestling, 40.0, 50.0);
            SetSkill(SkillName.Magery, 70.0, 80.0);
            SetSkill(SkillName.EvalInt, 80.0, 90.0);
            SetSkill(SkillName.Meditation, 50.0, 60.0);

            Fame = 2500;
            Karma = -2500;
        }

        public Anzuanord(Serial serial)
            : base(serial)
        {
        }

        public override bool BardImmune => true;

        public override int Meat => 1;

        public override int Hides => 7;

        public override HideType HideType => HideType.Spined;

        public override PackInstinct PackInstinct => PackInstinct.Daemon;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.MedScrolls, 2);
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
