using System;

namespace Server.Mobiles
{
    [CorpseName("a mongbat corpse")]
    public class Mongbat : BaseCreature
    {
        [Constructable]
        public Mongbat()
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

            this.SetDamage(1, 2);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 10);

            this.SetSkill(SkillName.MagicResist, 5.1, 14.0);
            this.SetSkill(SkillName.Tactics, 5.1, 10.0);
            this.SetSkill(SkillName.Wrestling, 5.1, 10.0);

            this.Fame = 150;
            this.Karma = -150;

            this.VirtualArmor = 10;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = -18.9;
        }

        public Mongbat(Serial serial)
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
                return FoodType.Meat;
            }
        }

        public override bool CanFly
        {
            get
            {
                return true;
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