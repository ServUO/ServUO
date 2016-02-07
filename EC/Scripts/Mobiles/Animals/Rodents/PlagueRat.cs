using System;

namespace Server.Mobiles
{
    [CorpseName("a plague rat corpse")]
	
    public class PlagueRat : BaseCreature
    {
        [Constructable]
        public PlagueRat()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Clan Ribbon Plague Rat";
            this.Body = 0xD7;
            this.Hue = 1710;
            this.BaseSoundID = 0x188;

            this.SetStr(59);
            this.SetDex(51);
            this.SetInt(17);

            this.SetHits(92);
            this.SetMana(0);

            this.SetDamage(4, 8);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 40);
            this.SetResistance(ResistanceType.Fire, 20, 25);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Energy, 35, 40); 

            this.SetSkill(SkillName.MagicResist, 25.1, 30.0);
            this.SetSkill(SkillName.Tactics, 34.5, 40.0);
            this.SetSkill(SkillName.Wrestling, 40.5, 45.0);

            this.Fame = 300;
            this.Karma = -300;

            this.VirtualArmor = 38;
        }

        public PlagueRat(Serial serial)
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
                return 6;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Fish | FoodType.Meat | FoodType.FruitsAndVegies | FoodType.Eggs;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
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