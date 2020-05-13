using Server.Engines.MyrmidexInvasion;

namespace Server.Mobiles
{
    [CorpseName("a myrmidex corpse")]
    public class MyrmidexLarvae : BaseCreature
    {
        [Constructable]
        public MyrmidexLarvae()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "a myrmidex larvae";

            Body = 1293;
            BaseSoundID = 959;

            SetStr(79, 100);
            SetDex(82, 95);
            SetInt(38, 75);

            SetDamage(5, 13);

            SetHits(446, 588);
            SetMana(0);

            SetResistance(ResistanceType.Physical, 20);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetSkill(SkillName.MagicResist, 30.1, 43.5);
            SetSkill(SkillName.Tactics, 30.1, 49.0);
            SetSkill(SkillName.Wrestling, 40, 50);

            Fame = 2500;
            Karma = -2500;
        }

        public override Poison HitPoison => Poison.Lesser;
        public override Poison PoisonImmune => Poison.Lesser;
        public override int TreasureMapLevel => 1;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.LootGold(20, 40));
        }

        public override bool IsEnemy(Mobile m)
        {
            if (MyrmidexInvasionSystem.Active && MyrmidexInvasionSystem.IsAlliedWithEodonTribes(m))
                return true;

            if (MyrmidexInvasionSystem.Active && MyrmidexInvasionSystem.IsAlliedWithMyrmidex(m))
                return false;

            return base.IsEnemy(m);
        }

        public MyrmidexLarvae(Serial serial) : base(serial)
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
