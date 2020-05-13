using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a clan scratch tinkerer corpse")]
    public class ClanCT : BaseCreature
    {
        [Constructable]
        public ClanCT()
            : base(AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Clan Scratch Tinkerer";
            Body = 0x8E;
            BaseSoundID = 437;

            SetStr(300, 330);
            SetDex(220, 240);
            SetInt(240, 275);

            SetHits(2025, 2068);

            SetDamage(4, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20, 30);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 35, 50);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.Anatomy, 62.5, 82.6);
            SetSkill(SkillName.Archery, 80.1, 90.0);
            SetSkill(SkillName.MagicResist, 76.8, 99.3);
            SetSkill(SkillName.Tactics, 64.2, 84.4);
            SetSkill(SkillName.Wrestling, 62.8, 85.0);

            Fame = 6500;
            Karma = -6500;

            AddItem(new Bow());
        }

        public ClanCT(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses => true;
        public override int Hides => 8;
        public override HideType HideType => HideType.Spined;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.LootItem<Arrow>(Utility.RandomMinMax(50, 70)));
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
