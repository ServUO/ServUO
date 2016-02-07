using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a hell hound corpse")]
    public class HellHound : BaseCreature
    {
        [Constructable]
        public HellHound()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a hell hound";
            this.Body = 98;
            this.BaseSoundID = 229;

            this.SetStr(102, 150);
            this.SetDex(81, 105);
            this.SetInt(36, 60);

            this.SetHits(66, 125);

            this.SetDamage(11, 17);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 80);

            this.SetResistance(ResistanceType.Physical, 25, 35);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.Fame = 3400;
            this.Karma = -3400;

            this.VirtualArmor = 30;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 85.5;

            this.PackItem(new SulfurousAsh(5));
        }

        public HellHound(Serial serial)
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
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Canine;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Meager);
        }
        public override void OnDeath(Container c)
        {

            base.OnDeath(c);
            Region reg = Region.Find(c.GetWorldLocation(), c.Map);
            if (0.25 > Utility.RandomDouble() && reg.Name == "Fire Temple Ruins")
            {
                if (Utility.RandomDouble() < 0.6)
                    c.DropItem(new EssenceOrder());
            }
            if (0.25 > Utility.RandomDouble() && reg.Name == "Lava Caldera")
            {
                if (Utility.RandomDouble() < 0.6)
                    c.DropItem(new EssencePassion());
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