using System;

namespace Server.Mobiles
{
    [CorpseName("a timber wolf corpse")]
    [TypeAlias("Server.Mobiles.Timberwolf")]
    public class TimberWolf : BaseCreature
    {
        [Constructable]
        public TimberWolf()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a timber wolf";
            this.Body = 225;
            this.BaseSoundID = 0xE5;

            this.SetStr(56, 80);
            this.SetDex(56, 75);
            this.SetInt(11, 25);

            this.SetHits(34, 48);
            this.SetMana(0);

            this.SetDamage(5, 9);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 10, 15);
            this.SetResistance(ResistanceType.Poison, 5, 10);
            this.SetResistance(ResistanceType.Energy, 5, 10);

            this.SetSkill(SkillName.MagicResist, 27.6, 45.0);
            this.SetSkill(SkillName.Tactics, 30.1, 50.0);
            this.SetSkill(SkillName.Wrestling, 40.1, 60.0);

            this.Fame = 450;
            this.Karma = 0;

            this.VirtualArmor = 16;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 23.1;
        }

        public TimberWolf(Serial serial)
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
                return 5;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Canine;
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
        }
    }
}