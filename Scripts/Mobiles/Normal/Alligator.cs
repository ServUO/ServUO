using System;

namespace Server.Mobiles
{
    [CorpseName("an alligator corpse")]
    public class Alligator : BaseCreature
    {
        [Constructable]
        public Alligator()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an alligator";
            this.Body = 0xCA;
            this.BaseSoundID = 660;

            this.SetStr(76, 100);
            this.SetDex(6, 25);
            this.SetInt(11, 20);

            this.SetHits(46, 60);
            this.SetStam(46, 65);
            this.SetMana(0);

            this.SetDamage(5, 15);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 35);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Poison, 5, 10);

            this.SetSkill(SkillName.MagicResist, 25.1, 40.0);
            this.SetSkill(SkillName.Tactics, 40.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 40.1, 60.0);

            this.Fame = 600;
            this.Karma = -600;

            this.VirtualArmor = 30;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 47.1;
        }

        public Alligator(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int Hides
        {
            get
            {
                return 12;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat | FoodType.Fish;
            }
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

            if (this.BaseSoundID == 0x5A)
                this.BaseSoundID = 660;
        }
    }
}