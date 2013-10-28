using System;

namespace Server.Mobiles
{
    [CorpseName("a rat corpse")]
    public class Rat : BaseCreature
    {
        [Constructable]
        public Rat()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a rat";
            this.Body = 238;
            this.BaseSoundID = 0xCC;

            this.SetStr(9);
            this.SetDex(35);
            this.SetInt(5);

            this.SetHits(6);
            this.SetMana(0);

            this.SetDamage(1, 2);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 10);
            this.SetResistance(ResistanceType.Poison, 5, 10);

            this.SetSkill(SkillName.MagicResist, 4.0);
            this.SetSkill(SkillName.Tactics, 4.0);
            this.SetSkill(SkillName.Wrestling, 4.0);

            this.Fame = 150;
            this.Karma = -150;

            this.VirtualArmor = 6;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = -0.9;
        }

        public Rat(Serial serial)
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
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat | FoodType.Fish | FoodType.Eggs | FoodType.GrainsAndHay;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Poor);
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