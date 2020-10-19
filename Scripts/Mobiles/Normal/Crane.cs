namespace Server.Mobiles
{
    [CorpseName("a crane corpse")]
    public class Crane : BaseCreature
    {
        [Constructable]
        public Crane()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a crane";
            Body = 254;
            BaseSoundID = 0x4D7;

            SetStr(26, 35);
            SetDex(16, 25);
            SetInt(11, 15);

            SetHits(26, 35);
            SetMana(0);

            SetDamage(1, 1);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 5, 5);

            SetSkill(SkillName.MagicResist, 4.1, 5.0);
            SetSkill(SkillName.Tactics, 10.1, 11.0);
            SetSkill(SkillName.Wrestling, 10.1, 11.0);

            Fame = 0;
            Karma = 200;
        }

        public Crane(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 1;
        public override int Feathers => 25;
        public override int GetAngerSound()
        {
            return 0x4D9;
        }

        public override int GetIdleSound()
        {
            return 0x4D8;
        }

        public override int GetAttackSound()
        {
            return 0x4D7;
        }

        public override int GetHurtSound()
        {
            return 0x4DA;
        }

        public override int GetDeathSound()
        {
            return 0x4D6;
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
