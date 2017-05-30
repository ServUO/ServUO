using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a nightmare corpse")]
    public class Nightmare : BaseMount
    {
        [Constructable]
        public Nightmare()
            : this("a nightmare")
        {
        }

        [Constructable]
        public Nightmare(string name)
            : base(name, 0x74, 0x3EA7, AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.BaseSoundID = Core.AOS ? 0xA8 : 0x16A;

            this.SetStr(496, 525);
            this.SetDex(86, 105);
            this.SetInt(86, 125);

            this.SetHits(298, 315);

            this.SetDamage(16, 22);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Fire, 40);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.EvalInt, 10.4, 50.0);
            this.SetSkill(SkillName.Magery, 10.4, 50.0);
            this.SetSkill(SkillName.MagicResist, 85.3, 100.0);
            this.SetSkill(SkillName.Tactics, 97.6, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.5, 92.5);

            this.Fame = 14000;
            this.Karma = -14000;

            this.VirtualArmor = 60;

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 95.1;

			switch (Utility.Random(12))
            {
                case 0: PackItem(new BloodOathScroll()); break;
                case 1: PackItem(new HorrificBeastScroll()); break;
                case 2: PackItem(new StrangleScroll()); break;
                case 3: PackItem(new VengefulSpiritScroll()); break;
			}

            switch (Utility.Random(4))
            {
                case 0:
                    {
                        BodyValue = 116;
                        ItemID = 16039;
                        break;
                    }
                case 1:
                    {
                        BodyValue = 177;
                        ItemID = 16053;
                        break;
                    }
                case 2:
                    {
                        BodyValue = 178;
                        ItemID = 16041;
                        break;
                    }
                case 3:
                    {
                        BodyValue = 179;
                        ItemID = 16055;
                        break;
                    }
            }

            if (Utility.RandomDouble() < 0.05)
                Hue = 1910;

            this.PackItem(new SulfurousAsh(Utility.RandomMinMax(3, 5)));
        }

        public Nightmare(Serial serial)
            : base(serial)
        {
        }

        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }// fire breath enabled
        public override int Meat
        {
            get
            {
                return 5;
            }
        }
        public override int Hides
        {
            get
            {
                return 10;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Barbed;
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
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.LowScrolls);
            this.AddLoot(LootPack.Potions);
        }

        public override int GetAngerSound()
        {
            if (!this.Controlled)
                return 0x16A;

            return base.GetAngerSound();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Core.AOS && this.BaseSoundID == 0x16A)
                this.BaseSoundID = 0xA8;
            else if (!Core.AOS && this.BaseSoundID == 0xA8)
                this.BaseSoundID = 0x16A;
        }
    }
}
