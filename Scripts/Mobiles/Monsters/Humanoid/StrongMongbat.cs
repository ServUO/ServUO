using System;

namespace Server.Mobiles
{
    [CorpseName("a mongbat corpse")]
    public class StrongMongbat : BaseCreature
    {
        [Constructable]
        public StrongMongbat()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a mongbat";
            this.Body = 39;
            this.BaseSoundID = 422;

            this.SetStr(6, 10);
            this.SetDex(26, 38);
            this.SetInt(6, 14);

            this.SetHits(4, 6);
            this.SetMana(0);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 25);

            this.SetSkill(SkillName.MagicResist, 15.1, 30.0);
            this.SetSkill(SkillName.Tactics, 35.1, 50.0);
            this.SetSkill(SkillName.Wrestling, 20.1, 35.0);

            this.Fame = 150;
            this.Karma = -150;

            this.VirtualArmor = 10;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 71.1;
        }

        public StrongMongbat(Serial serial)
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
                return FoodType.Meat;
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