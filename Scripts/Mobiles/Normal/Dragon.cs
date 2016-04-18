using System;

namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class Dragon : BaseCreature
    {
        [Constructable]
        public Dragon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a dragon";
            this.Body = Utility.RandomList(12, 59);
            this.BaseSoundID = 362;

            this.SetStr(796, 825);
            this.SetDex(86, 105);
            this.SetInt(436, 475);

            this.SetHits(478, 495);

            this.SetDamage(16, 22);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 35, 45);

            this.SetSkill(SkillName.EvalInt, 30.1, 40.0);
            this.SetSkill(SkillName.Magery, 30.1, 40.0);
            this.SetSkill(SkillName.MagicResist, 99.1, 100.0);
            this.SetSkill(SkillName.Tactics, 97.6, 100.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 92.5);

            this.Fame = 15000;
            this.Karma = -15000;

            this.VirtualArmor = 60;

            this.Tamable = true;
            this.ControlSlots = 3;
            this.MinTameSkill = 93.9;
        }

        public Dragon(Serial serial)
            : base(serial)
        {
        }

        public override bool ReacquireOnMovement
        {
            get
            {
                return !this.Controlled;
            }
        }
        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }// fire breath enabled
        public override bool AutoDispel
        {
            get
            {
                return !this.Controlled;
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
                return 7;
            }
        }
        public override ScaleType ScaleType
        {
            get
            {
                return (this.Body == 12 ? ScaleType.Yellow : ScaleType.Red);
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
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
            this.AddLoot(LootPack.Gems, 8);
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