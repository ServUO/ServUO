using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a gaman corpse")]
    public class Gaman : BaseCreature
    {
        [Constructable]
        public Gaman()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a gaman";
            this.Body = 248;

            this.SetStr(146, 175);
            this.SetDex(111, 150);
            this.SetInt(46, 60);

            this.SetHits(131, 160);
            this.SetMana(0);

            this.SetDamage(6, 11);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 70);
            this.SetResistance(ResistanceType.Fire, 30, 50);
            this.SetResistance(ResistanceType.Cold, 30, 50);
            this.SetResistance(ResistanceType.Poison, 40, 60);
            this.SetResistance(ResistanceType.Energy, 30, 50);

            this.SetSkill(SkillName.MagicResist, 37.6, 42.5);
            this.SetSkill(SkillName.Tactics, 70.6, 83.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 57.5);

            this.Fame = 2000;
            this.Karma = -2000;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 68.7;
        }

        public Gaman(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 10;
            }
        }
        public override int Hides
        {
            get
            {
                return 15;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.GrainsAndHay;
            }
        }
        public override int GetAngerSound()
        {
            return 0x4F8;
        }

        public override int GetIdleSound()
        {
            return 0x4F7;
        }

        public override int GetAttackSound()
        {
            return 0x4F6;
        }

        public override int GetHurtSound()
        {
            return 0x4F9;
        }

        public override int GetDeathSound()
        {
            return 0x4F5;
        }

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);

			if(Core.ML)
				c.AddItem(Loot.Construct(typeof(GamanHorns)));
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