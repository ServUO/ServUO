using System;

namespace Server.Mobiles
{
    [CorpseName("a white wyrm corpse")]
    public class WhiteWyrm : BaseCreature
    {
        [Constructable]
        public WhiteWyrm()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Body = Utility.RandomBool() ? 180 : 49;
            this.Name = "a white wyrm";
            this.BaseSoundID = 362;

            this.SetStr(721, 760);
            this.SetDex(101, 130);
            this.SetInt(386, 425);

            this.SetHits(433, 456);

            this.SetDamage(17, 25);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Cold, 50);

            this.SetResistance(ResistanceType.Physical, 55, 70);
            this.SetResistance(ResistanceType.Fire, 15, 25);
            this.SetResistance(ResistanceType.Cold, 80, 90);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.EvalInt, 99.1, 100.0);
            this.SetSkill(SkillName.Magery, 99.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 99.1, 100.0);
            this.SetSkill(SkillName.Tactics, 97.6, 100.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);

            this.Fame = 18000;
            this.Karma = -18000;

            this.VirtualArmor = 64;

            this.Tamable = true;
            this.ControlSlots = 3;
            this.MinTameSkill = 96.3;
        }

        public WhiteWyrm(Serial serial)
            : base(serial)
        {
        }

        public override bool ReacquireOnMovement
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 4;
            }
        }
        public override int Meat
        {
            get
            {
                return 19;
            }
        }
        public override int DragonBlood
        {
            get
            {
                return 8;
            }
        }
        public override int Hides
        {
            get
            {
                return 20;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Barbed;
            }
        }
        public override int Scales
        {
            get
            {
                return 9;
            }
        }
        public override ScaleType ScaleType
        {
            get
            {
                return ScaleType.White;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat | FoodType.Gold;
            }
        }
        public override bool CanAngerOnTame
        {
            get
            {
                return true;
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
            this.AddLoot(LootPack.FilthyRich, 2);
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Gems, Utility.Random(1, 5));
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