using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a seaweed corpse")]
    public class DiabolicalSeaweed : BaseCreature
    {
        [Constructable]
        public DiabolicalSeaweed()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.0, 0.0)
        {
            Name = "Diabolical Seaweed";
            Body = 129;
            Hue = 1914;

            SetStr(452, 485);
            SetDex(401, 420);
            SetInt(126, 140);

            SetHits(501, 532);

            SetDamage(10, 23);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 35, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 50.1, 54.7);
            SetSkill(SkillName.Tactics, 100.3, 114.8);
            SetSkill(SkillName.Wrestling, 45.1, 59.5);

            Fame = 3000;
            Karma = -3000;
            CantWalk = true;
        }

        public DiabolicalSeaweed(Serial serial)
            : base(serial)
        {
        }

        public override bool TeleportsTo => true;
        public override bool CanRummageCorpses => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.RareGems);
            AddLoot(LootPack.MageryRegs, 20, 30);
            AddLoot(LootPack.LootItem<ParasiticPlant>(true));
            AddLoot(LootPack.LootItem<LuminescentFungi>(true));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
