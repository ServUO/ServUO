using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a mimic corpse")]
    public class Mimic : BaseCreature
    {
        [Constructable]
        public Mimic()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a mimic";
            Body = 729;

            SetStr(281);
            SetDex(140);
            SetInt(261);

            SetHits(543);

            SetDamage(13, 20);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 63);
            SetResistance(ResistanceType.Fire, 43);
            SetResistance(ResistanceType.Cold, 36);
            SetResistance(ResistanceType.Poison, 37);
            SetResistance(ResistanceType.Energy, 42);

            SetSkill(SkillName.EvalInt, 100.0);
            SetSkill(SkillName.Magery, 107.5);
            SetSkill(SkillName.Meditation, 100.0);
            SetSkill(SkillName.MagicResist, 126.5);
            SetSkill(SkillName.Tactics, 98.5);
            SetSkill(SkillName.Wrestling, 92.2);
        }

        public Mimic(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 4);
            AddLoot(LootPack.MedScrolls);
            AddLoot(LootPack.MageryRegs, 20);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.03)
                c.DropItem(new LuckyCoin());
        }

        public override int GetIdleSound()
        {
            return 1561;
        }

        public override int GetAngerSound()
        {
            return 1558;
        }

        public override int GetHurtSound()
        {
            return 1560;
        }

        public override int GetDeathSound()
        {
            return 1559;
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
