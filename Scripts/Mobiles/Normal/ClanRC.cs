namespace Server.Mobiles
{
    [CorpseName("a clan ribbon courtier corpse")]
    public class ClanRC : BaseCreature
    {
        [Constructable]
        public ClanRC()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Clan Ribbon Courtier";
            Body = 42;
            Hue = 2207;
            BaseSoundID = 437;

            SetStr(231);
            SetDex(252);
            SetInt(125);

            SetHits(2054, 2100);

            SetDamage(7, 14);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40);
            SetResistance(ResistanceType.Fire, 10, 12);
            SetResistance(ResistanceType.Cold, 15, 20);
            SetResistance(ResistanceType.Poison, 10, 12);
            SetResistance(ResistanceType.Energy, 10, 12);

            SetSkill(SkillName.MagicResist, 113.5, 115.0);
            SetSkill(SkillName.Tactics, 65.1, 70.0);
            SetSkill(SkillName.Wrestling, 50.5, 55.0);

            Fame = 1500;
            Karma = -1500;
        }

        public ClanRC(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses => true;
        public override int Hides => 8;
        public override HideType HideType => HideType.Spined;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);
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