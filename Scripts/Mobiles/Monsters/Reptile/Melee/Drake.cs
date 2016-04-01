using System;

namespace Server.Mobiles
{
    [CorpseName("a drake corpse")]
    public class Drake : BaseCreature
    {
        [Constructable]
        public Drake()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a drake";
            this.Body = Utility.RandomList(60, 61);
            this.BaseSoundID = 362;

            this.SetStr(401, 430);
            this.SetDex(133, 152);
            this.SetInt(101, 140);

            this.SetHits(241, 258);

            this.SetDamage(11, 17);

            this.SetDamageType(ResistanceType.Physical, 80);
            this.SetDamageType(ResistanceType.Fire, 20);

            this.SetResistance(ResistanceType.Physical, 45, 50);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 65.1, 80.0);
            this.SetSkill(SkillName.Tactics, 65.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 65.1, 80.0);

            this.Fame = 5500;
            this.Karma = -5500;

            this.VirtualArmor = 46;

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 84.3;

            this.PackReg(3);
        }

        public Drake(Serial serial)
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
        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }// fire breath enabled
        public override int TreasureMapLevel
        {
            get
            {
                return 2;
            }
        }
        public override int Meat
        {
            get
            {
                return 10;
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
                return HideType.Horned;
            }
        }
        public override int Scales
        {
            get
            {
                return 2;
            }
        }
        public override ScaleType ScaleType
        {
            get
            {
                return (this.Body == 60 ? ScaleType.Yellow : ScaleType.Red);
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat | FoodType.Fish;
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
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.MedScrolls, 2);
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