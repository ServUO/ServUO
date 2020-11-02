using Server.Mobiles;

namespace Server.Engines.SorcerersDungeon
{
    [CorpseName("a twisted holiday tree corpse")]
    public class TwistedHolidayTree : BaseCreature
    {
        [Constructable]
        public TwistedHolidayTree()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a twisted holiday tree";
            Body = 47;
            BaseSoundID = 442;
            Hue = 1175;

            SetStr(400);
            SetDex(150);
            SetInt(1200);

            SetHits(8000);

            SetDamage(21, 27);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 80, 90);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 100);

            SetSkill(SkillName.EvalInt, 200);
            SetSkill(SkillName.Magery, 120);
            SetSkill(SkillName.Meditation, 100);
            SetSkill(SkillName.MagicResist, 200);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.Wrestling, 120);

            Fame = 16000;
            Karma = -16000;

            SetAreaEffect(AreaEffect.ExplosiveGoo);
        }

        public TwistedHolidayTree(Serial serial)
            : base(serial)
        {
        }

        public override bool CanFlee => false;
        public override bool AlwaysMurderer => true;
        public override Poison PoisonImmune => Poison.Deadly;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
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
