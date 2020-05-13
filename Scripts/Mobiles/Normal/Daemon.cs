using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a daemon corpse")]
    public class Daemon : BaseCreature
    {
        [Constructable]
        public Daemon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("daemon");
            Body = 9;
            BaseSoundID = 357;

            SetStr(476, 505);
            SetDex(76, 95);
            SetInt(301, 325);

            SetHits(286, 303);

            SetDamage(7, 14);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 60);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 70.1, 80.0);
            SetSkill(SkillName.Magery, 70.1, 80.0);
            SetSkill(SkillName.MagicResist, 85.1, 95.0);
            SetSkill(SkillName.Tactics, 70.1, 80.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);

            Fame = 15000;
            Karma = -15000;

            ControlSlots = 4;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.RandomLootItem(new System.Type[] { typeof(LichFormScroll), typeof(PoisonStrikeScroll), typeof(StrangleScroll), typeof(VengefulSpiritScroll), typeof(WitherScroll) }, 25.0, 1, false, true));
        }

        public Daemon(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty => 125.0;

        public override double DispelFocus => 45.0;

        public override bool CanRummageCorpses => true;

        public override Poison PoisonImmune => Poison.Regular;

        public override int TreasureMapLevel => 4;

        public override int Meat => 1;

        public override bool CanFly => true;

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
