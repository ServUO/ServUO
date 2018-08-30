//Created by DelBoy aka Fury on 17/02/14

using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a tiger corpse")]
    public class Tiger : BaseMount
    {
        [Constructable]
        public Tiger()
            : this("a Tiger")
        {
        }

        [Constructable]
        public Tiger(string name)
            : base(name, 0x4E6, 0x3EC7, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a Tiger";

            switch (Utility.Random(2))
            {
                case 0:
                    {
                        BodyValue = 1254;
                        ItemID = 16071; //male
                        break;
                    }
                case 1:
                    {
                        BodyValue = 1255;
                        ItemID = 16072; //female
                        break;
                    }

            }

            //Add a low chance of the tiger being a white tiger
            int hueValue = Utility.Random(500);

            if (hueValue <= 1)
            {
                this.Hue = 0x481;
            }

            this.SetStr(1200, 1250);
            this.SetDex(150, 180);
            this.SetInt(250, 285);

            this.SetHits(1010, 1275);

            this.SetDamage(20, 30);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 65);
            this.SetResistance(ResistanceType.Fire, 70, 90);
            this.SetResistance(ResistanceType.Cold, 25, 45);
            this.SetResistance(ResistanceType.Poison, 30, 50);
            this.SetResistance(ResistanceType.Energy, 70, 85);

            this.SetSkill(SkillName.Anatomy, 75.1, 110.0);
            this.SetSkill(SkillName.MagicResist, 85.1, 100.0);
            this.SetSkill(SkillName.Tactics, 100.1, 120.0);
            this.SetSkill(SkillName.Wrestling, 100.1, 120.0);

            this.Fame = 10000;
            this.Karma = 10000;

            this.Tamable = true;
            this.ControlSlots = 4;
            this.MinTameSkill = 101.7;

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

        public override bool StatLossAfterTame
        {
            get
            {
                return true;
            }
        }

        public override int Meat
        {
            get
            {
                return 3;
            }
        }

        public override int Hides
        {
            get
            {
                return 10;
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosFilthyRich, 5);
        }
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.1)
                c.DropItem(new TigerFur());
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
        }

        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Greater;
            }
        }

        public override int GetAngerSound()
        {
            return 0x518;
        }

        public override int GetIdleSound()
        {
            return 0x517;
        }

        public override int GetAttackSound()
        {
            return 0x516;
        }

        public override int GetHurtSound()
        {
            return 0x519;
        }

        public override int GetDeathSound()
        {
            return 0x515;
        }

        public Tiger(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}