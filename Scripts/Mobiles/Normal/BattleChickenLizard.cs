namespace Server.Mobiles
{
    [CorpseName("a chicken lizard corpse")]
    public class BattleChickenLizard : BaseCreature
    {
        [Constructable]
        public BattleChickenLizard()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.05, 0.1)
        {
            Name = "a battle chicken lizard";
            Body = 716;

            SetStr(94, 177);
            SetDex(78, 124);
            SetInt(6, 13);

            SetHits(94, 177);

            SetDamage(5, 15);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 5, 15);

            SetSkill(SkillName.MagicResist, 30.0, 53.0);
            SetSkill(SkillName.Tactics, 50.0, 62.0);
            SetSkill(SkillName.Wrestling, 50.0, 62.0);

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 0.0;
        }

        public override int Meat => 3;
        public override MeatType MeatType => MeatType.Bird;
        public override FoodType FavoriteFood => FoodType.GrainsAndHay;

        public override int GetIdleSound() { return 1511; }
        public override int GetAngerSound() { return 1508; }
        public override int GetHurtSound() { return 1510; }
        public override int GetDeathSound() { return 1509; }

        public override double FleeChance => 1.0;
        public override double BreakFleeChance => 0.05;

        public override bool CheckFlee()
        {
            return true;
        }

        public override bool CheckBreakFlee()
        {
            return Controlled;
        }

        public override bool BreakFlee()
        {
            NextFleeCheck = Core.TickCount + 1500;

            return true;
        }

        public override void OnAfterTame(Mobile tamer)
        {
            ActiveSpeed = 0.2;
            PassiveSpeed = 0.4;

            if (Frozen)
                Frozen = false;
        }

        public BattleChickenLizard(Serial serial)
            : base(serial)
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
