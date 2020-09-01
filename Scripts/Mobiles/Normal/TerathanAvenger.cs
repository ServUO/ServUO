using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a terathan avenger corpse")]
    public class TerathanAvenger : BaseCreature
    {
        [Constructable]
        public TerathanAvenger()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a terathan avenger";
            Body = 152;
            BaseSoundID = 0x24D;

            SetStr(467, 645);
            SetDex(77, 95);
            SetInt(126, 150);

            SetHits(296, 372);
            SetMana(46, 70);

            SetDamage(18, 22);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 90, 100);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.EvalInt, 70.3, 100.0);
            SetSkill(SkillName.Magery, 70.3, 100.0);
            SetSkill(SkillName.Poisoning, 60.1, 80.0);
            SetSkill(SkillName.MagicResist, 65.1, 80.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 15000;
            Karma = -15000;
        }

        public TerathanAvenger(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Deadly;
        public override Poison HitPoison => Poison.Deadly;
        public override int TreasureMapLevel => 3;
        public override int Meat => 2;

        public override TribeType Tribe => TribeType.Terathan;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.LootItem<TerathanAvengerArms>(25.0));
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
