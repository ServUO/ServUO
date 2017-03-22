using System;

namespace Server.Mobiles
{
    [CorpseName("a rat corpse")]
    public class PrisonRat : BaseCreature
    {
        [Constructable]
        public PrisonRat()
            : base(AIType.AI_Animal, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a prison rat";
            this.Body = 238;
            this.BaseSoundID = 0xCC;

            this.SetStr(9);
            this.SetDex(35);
            this.SetInt(5);

            this.SetHits(6);
            this.SetMana(0);

            this.SetDamage(5, 8);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 10);
            this.SetResistance(ResistanceType.Poison, 5, 10);

            this.SetSkill(SkillName.MagicResist, 11.0);
            this.SetSkill(SkillName.Tactics, 14.0);
            this.SetSkill(SkillName.Wrestling, 12.0);

            this.Fame = 150;
            this.Karma = -150;

            this.VirtualArmor = 6;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = -0.9;
        }

        public PrisonRat(Serial serial)
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
                return FoodType.Fish;
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