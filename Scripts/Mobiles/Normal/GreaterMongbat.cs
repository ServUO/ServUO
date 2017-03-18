using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a mongbat corpse")]
    public class GreaterMongbat : BaseCreature
    {
        [Constructable]
        public GreaterMongbat()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a greater mongbat";
            this.Body = 39;
            this.BaseSoundID = 422;

            this.SetStr(56, 80);
            this.SetDex(61, 80);
            this.SetInt(26, 50);

            this.SetHits(34, 48);
            this.SetStam(61, 80);
            this.SetMana(26, 50);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 25);

            this.SetSkill(SkillName.MagicResist, 15.1, 30.0);
            this.SetSkill(SkillName.Tactics, 35.1, 50.0);
            this.SetSkill(SkillName.Wrestling, 20.1, 35.0);

            this.Fame = 450;
            this.Karma = -450;

            this.VirtualArmor = 10;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 71.1;
        }

        public GreaterMongbat(Serial serial)
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