using System;

namespace Server.Mobiles
{
    [CorpseName("a kepetch corpse")]
    public class KepetchAmbusher : BaseCreature
    {
        [Constructable]
        public KepetchAmbusher()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a kepetch ambusher";
            this.Body = 726;

            this.SetStr(440, 446);
            this.SetDex(229, 254);
            this.SetInt(46, 46);

            this.SetHits(533, 544);

            this.SetDamage(7, 17);

            this.SetDamageType(ResistanceType.Physical, 80);
            this.SetDamageType(ResistanceType.Poison, 20);

            this.SetResistance(ResistanceType.Physical, 73, 95);
            this.SetResistance(ResistanceType.Fire, 57, 70);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 55, 65);
            this.SetResistance(ResistanceType.Energy, 70, 95);

            this.SetSkill(SkillName.Anatomy, 104.3, 114.1);
            this.SetSkill(SkillName.MagicResist, 94.6, 97.4);
            this.SetSkill(SkillName.Tactics, 110.4, 123.5);
            this.SetSkill(SkillName.Wrestling, 107.3, 113.9);
        }

        public KepetchAmbusher(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 7;
            }
        }
        public override int Hides
        {
            get
            {
                return 14;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Horned;
            }
        }
        // add fur drop
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average, 2);
        }

        public override int GetIdleSound()
        {
            return 1545;
        }

        public override int GetAngerSound()
        {
            return 1542;
        }

        public override int GetHurtSound()
        {
            return 1544;
        }

        public override int GetDeathSound()
        {
            return 1543;
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