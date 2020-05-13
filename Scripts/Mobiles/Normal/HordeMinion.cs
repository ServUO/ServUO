using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a horde minion corpse")]
    public class HordeMinion : BaseCreature
    {
        [Constructable]
        public HordeMinion()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a horde minion";
            Body = 776;
            BaseSoundID = 357;

            SetStr(16, 40);
            SetDex(31, 60);
            SetInt(11, 25);

            SetHits(10, 24);

            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 5, 10);

            SetSkill(SkillName.MagicResist, 10.0);
            SetSkill(SkillName.Tactics, 0.1, 15.0);
            SetSkill(SkillName.Wrestling, 25.1, 40.0);

            Fame = 500;
            Karma = -500;

            AddItem(new LightSource());
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.LootItem<Bone>(3));
        }

        public HordeMinion(Serial serial)
            : base(serial)
        {
        }

        public override int GetIdleSound()
        {
            return 338;
        }

        public override int GetAngerSound()
        {
            return 338;
        }

        public override int GetDeathSound()
        {
            return 338;
        }

        public override int GetAttackSound()
        {
            return 406;
        }

        public override int GetHurtSound()
        {
            return 194;
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
