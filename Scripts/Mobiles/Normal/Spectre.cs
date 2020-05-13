namespace Server.Mobiles
{
    [CorpseName("a ghostly corpse")]
    public class Spectre : BaseCreature
    {
        [Constructable]
        public Spectre()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a spectre";
            Body = 26;
            Hue = 0x4001;
            BaseSoundID = 0x482;

            SetStr(76, 100);
            SetDex(76, 95);
            SetInt(36, 60);

            SetHits(46, 60);

            SetDamage(7, 11);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 25, 30);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 10, 20);

            SetSkill(SkillName.EvalInt, 55.1, 70.0);
            SetSkill(SkillName.Magery, 55.1, 70.0);
            SetSkill(SkillName.MagicResist, 55.1, 70.0);
            SetSkill(SkillName.Tactics, 45.1, 60.0);
            SetSkill(SkillName.Wrestling, 45.1, 55.0);

            Fame = 4000;
            Karma = -4000;
        }

        public Spectre(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune => true;

        public override TribeType Tribe => TribeType.Undead;

        public override Poison PoisonImmune => Poison.Lethal;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.MageryRegs, 10);
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
