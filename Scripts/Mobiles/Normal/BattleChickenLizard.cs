using System;

namespace Server.Mobiles
{
    [CorpseName("a chicken lizard corpse")]
    public class BattleChickenLizard : BaseCreature
    {
        [Constructable]
        public BattleChickenLizard()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a battle chicken lizard";
            this.Body = 716;

            this.SetStr(94, 177);
            this.SetDex(78, 124);
            this.SetInt(6, 13);

            this.SetHits(94, 177);

            this.SetDamage(5, 15);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);
            this.SetResistance(ResistanceType.Fire, 5, 15);

            this.SetSkill(SkillName.MagicResist, 30.0, 53.0);
            this.SetSkill(SkillName.Tactics, 50.0, 62.0);
            this.SetSkill(SkillName.Wrestling, 50.0, 62.0);

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 0.0;
        }

        public BattleChickenLizard(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 3;
            }
        }
        public override MeatType MeatType
        {
            get
            {
                return MeatType.Bird;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.GrainsAndHay;
            }
        }
        public override int GetIdleSound()
        {
            return 1511;
        }

        public override int GetAngerSound()
        {
            return 1508;
        }

        public override int GetHurtSound()
        {
            return 1510;
        }

        public override int GetDeathSound()
        {
            return 1509;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}