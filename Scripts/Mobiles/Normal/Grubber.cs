namespace Server.Mobiles
{
    [CorpseName("a grubber corpse")]
    public class Grubber : BaseCreature
    {
        [Constructable]
        public Grubber()
            : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.06, 0.1)
        {
            Name = "a grubber";
            Body = 270;

            SetStr(15);
            SetDex(2000);
            SetInt(1000);

            SetHits(200);
            SetStam(500);
            SetMana(0);

            SetDamage(1);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.MagicResist, 200.0);
            SetSkill(SkillName.Tactics, 5.0);
            SetSkill(SkillName.Wrestling, 5.0);

            Fame = 1000;
            Karma = 0;
        }

        public Grubber(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 1;
        public override int Hides => 1;
        public override double FleeChance => 1.0;
        public override double BreakFleeChance => 0.05;

        public override bool CheckFlee()
        {
            return true;
        }

        public override bool CheckBreakFlee()
        {
            return false;
        }

        public override bool BreakFlee()
        {
            NextFleeCheck = Core.TickCount + 1500;

            return true;
        }

        public override int GetAttackSound()
        {
            return 0xC9;
        }

        public override int GetHurtSound()
        {
            return 0xCA;
        }

        public override int GetDeathSound()
        {
            return 0xCB;
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
