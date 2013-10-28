using System;

namespace Server.Mobiles
{
    [CorpseName("a war horse corpse")]
    public abstract class BaseWarHorse : BaseMount
    {
        public BaseWarHorse(int bodyID, int itemID, AIType aiType, FightMode fightMode, int rangePerception, int rangeFight, double activeSpeed, double passiveSpeed)
            : base("a war horse", bodyID, itemID, aiType, fightMode, rangePerception, rangeFight, activeSpeed, passiveSpeed)
        {
            this.BaseSoundID = 0xA8;

            this.InitStats(Utility.Random(300, 100), 125, 60);

            this.SetStr(400);
            this.SetDex(125);
            this.SetInt(51, 55);

            this.SetHits(240);
            this.SetMana(0);

            this.SetDamage(5, 8);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 25.1, 30.0);
            this.SetSkill(SkillName.Tactics, 29.3, 44.0);
            this.SetSkill(SkillName.Wrestling, 29.3, 44.0);

            this.Fame = 300;
            this.Karma = 300;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 29.1;
        }

        public BaseWarHorse(Serial serial)
            : base(serial)
        {
        }

        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}