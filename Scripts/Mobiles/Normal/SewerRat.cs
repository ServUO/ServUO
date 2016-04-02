using System;

namespace Server.Mobiles
{
    [CorpseName("a sewer rat corpse")]
    public class Sewerrat : BaseCreature
    {
        [Constructable]
        public Sewerrat()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a sewer rat";
            this.Body = 238;
            this.BaseSoundID = 0xCC;

            this.SetStr(9);
            this.SetDex(25);
            this.SetInt(6, 10);

            this.SetHits(6);
            this.SetMana(0);

            this.SetDamage(1, 2);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 10);
            this.SetResistance(ResistanceType.Poison, 15, 25);
            this.SetResistance(ResistanceType.Energy, 5, 10);

            this.SetSkill(SkillName.MagicResist, 5.0);
            this.SetSkill(SkillName.Tactics, 5.0);
            this.SetSkill(SkillName.Wrestling, 5.0);

            this.Fame = 300;
            this.Karma = -300;

            this.VirtualArmor = 6;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = -0.9;
        }

        public Sewerrat(Serial serial)
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
                return FoodType.Meat | FoodType.Eggs | FoodType.FruitsAndVegies;
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