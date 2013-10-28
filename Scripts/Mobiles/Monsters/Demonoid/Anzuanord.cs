using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an anzuanord corpse")]
    public class Anzuanord : BaseCreature
    {
        [Constructable]
        public Anzuanord()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an Anzuanord";
            this.Body = 74;
            this.Hue = 2071;
            this.BaseSoundID = 422;

            this.SetStr(91, 115);
            this.SetDex(61, 80);
            this.SetInt(86, 105);

            this.SetHits(255, 270);

            this.SetDamage(10, 14);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Fire, 50);
            this.SetDamageType(ResistanceType.Poison, 50);

            this.SetResistance(ResistanceType.Physical, 25, 35);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.EvalInt, 20.1, 30.0);
            this.SetSkill(SkillName.Magery, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 30.1, 50.0);
            this.SetSkill(SkillName.Tactics, 42.1, 50.0);
            this.SetSkill(SkillName.Wrestling, 40.1, 44.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.VirtualArmor = 50;
        }

        public Anzuanord(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
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
                return 1;
            }
        }
        public override int Hides
        {
            get
            {
                return 7;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Daemon;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
            this.AddLoot(LootPack.MedScrolls, 2);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.5)
                c.DropItem(new VoidOrb());
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