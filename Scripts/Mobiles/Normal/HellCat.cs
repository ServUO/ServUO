using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a hell cat corpse")]
    [TypeAlias("Server.Mobiles.Hellcat")]
    public class HellCat : BaseCreature
    {
        [Constructable]
        public HellCat()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a hell cat";
            this.Body = 0xC9;
            this.Hue = Utility.RandomList(0x647, 0x650, 0x659, 0x662, 0x66B, 0x674);
            this.BaseSoundID = 0x69;

            this.SetStr(51, 100);
            this.SetDex(52, 150);
            this.SetInt(13, 85);

            this.SetHits(48, 67);

            this.SetDamage(6, 12);

            this.SetDamageType(ResistanceType.Physical, 40);
            this.SetDamageType(ResistanceType.Fire, 60);

            this.SetResistance(ResistanceType.Physical, 25, 35);
            this.SetResistance(ResistanceType.Fire, 80, 90);
            this.SetResistance(ResistanceType.Energy, 15, 20);

            this.SetSkill(SkillName.MagicResist, 45.1, 60.0);
            this.SetSkill(SkillName.Tactics, 40.1, 55.0);
            this.SetSkill(SkillName.Wrestling, 30.1, 40.0);

            this.Fame = 1000;
            this.Karma = -1000;

            this.VirtualArmor = 30;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 71.1;
        }

        public HellCat(Serial serial)
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
                return PackInstinct.Feline;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
        }
        public override void OnDeath(Container c)
        {

            base.OnDeath(c);            
            Region reg = Region.Find(c.GetWorldLocation(), c.Map);
            if (0.25 > Utility.RandomDouble() && reg.Name == "Fire Temple Ruins")
            {
                if (Utility.RandomDouble() < 0.25)
                    c.DropItem(new EssenceOrder());
            }
            if (0.25 > Utility.RandomDouble() && reg.Name == "Lava Caldera")
            {
                if (Utility.RandomDouble() < 0.25)
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