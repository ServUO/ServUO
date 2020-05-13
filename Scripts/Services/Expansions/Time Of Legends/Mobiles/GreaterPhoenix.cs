namespace Server.Mobiles
{
    [CorpseName("a greater phoenix corpse")]
    public class GreaterPhoenix : BaseCreature
    {
        [Constructable]
        public GreaterPhoenix()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "a greater phoenix";
            Body = 832;
            BaseSoundID = 0x8F;

            SetStr(332, 386);
            SetDex(97, 113);
            SetInt(182, 258);

            SetDamage(11, 14);

            SetHits(119, 240);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Fire, 60);

            SetSkill(SkillName.Wrestling, 60, 77);
            SetSkill(SkillName.MagicResist, 90, 105);
            SetSkill(SkillName.Tactics, 50, 70);
            SetSkill(SkillName.Magery, 90, 100);
            SetSkill(SkillName.EvalInt, 90, 100);

            Fame = 10000;
            Karma = -10000;

            SetSpecialAbility(SpecialAbility.GraspingClaw);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.LootGold(500, 700));
        }

        public GreaterPhoenix(Serial serial) : base(serial)
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
