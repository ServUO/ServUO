using System;

namespace Server.Mobiles
{
    [CorpseName("an ostard corpse")]
    public class ForestOstard : BaseMount
    {
        [Constructable]
        public ForestOstard()
            : this("a forest ostard")
        {
        }

        [Constructable]
        public ForestOstard(string name)
            : base(name, 0xDB, 0x3EA5, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Hue = Utility.RandomSlimeHue() | 0x8000;

            this.BaseSoundID = 0x270;

            this.SetStr(94, 170);
            this.SetDex(56, 75);
            this.SetInt(6, 10);

            this.SetHits(71, 88);
            this.SetMana(0);

            this.SetDamage(8, 14);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);

            this.SetSkill(SkillName.MagicResist, 27.1, 32.0);
            this.SetSkill(SkillName.Tactics, 29.3, 44.0);
            this.SetSkill(SkillName.Wrestling, 29.3, 44.0);

            this.Fame = 450;
            this.Karma = 0;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 29.1;
        }

        public ForestOstard(Serial serial)
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
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Ostard;
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