using System;

namespace Server.Mobiles
{
    [CorpseName("a slimey corpse")]
    public class Slime : BaseCreature
    {
        [Constructable]
        public Slime()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a slime";
            this.Body = 51;
            this.BaseSoundID = 456;

            this.Hue = Utility.RandomSlimeHue();

            this.SetStr(22, 34);
            this.SetDex(16, 21);
            this.SetInt(16, 20);

            this.SetHits(15, 19);

            this.SetDamage(1, 5);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 10);
            this.SetResistance(ResistanceType.Poison, 10, 20);

            this.SetSkill(SkillName.Poisoning, 30.1, 50.0);
            this.SetSkill(SkillName.MagicResist, 15.1, 20.0);
            this.SetSkill(SkillName.Tactics, 19.3, 34.0);
            this.SetSkill(SkillName.Wrestling, 19.3, 34.0);

            this.Fame = 300;
            this.Karma = -300;

            this.VirtualArmor = 8;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 23.1;
        }

        public Slime(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lesser;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Lesser;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat | FoodType.Fish | FoodType.FruitsAndVegies | FoodType.GrainsAndHay | FoodType.Eggs;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Poor);
            this.AddLoot(LootPack.Gems);
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