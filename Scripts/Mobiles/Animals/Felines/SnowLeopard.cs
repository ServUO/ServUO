using System;

namespace Server.Mobiles
{
    [CorpseName("a leopard corpse")]
    [TypeAlias("Server.Mobiles.Snowleopard")]
    public class SnowLeopard : BaseCreature
    {
        [Constructable]
        public SnowLeopard()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a snow leopard";
            this.Body = Utility.RandomList(64, 65);
            this.BaseSoundID = 0x73;

            this.SetStr(56, 80);
            this.SetDex(66, 85);
            this.SetInt(26, 50);

            this.SetHits(34, 48);
            this.SetMana(0);

            this.SetDamage(3, 9);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.MagicResist, 25.1, 35.0);
            this.SetSkill(SkillName.Tactics, 45.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 40.1, 50.0);

            this.Fame = 450;
            this.Karma = 0;

            this.VirtualArmor = 24;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 53.1;
        }

        public SnowLeopard(Serial serial)
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
                return 8;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat | FoodType.Fish;
            }
        }
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Feline;
            }
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