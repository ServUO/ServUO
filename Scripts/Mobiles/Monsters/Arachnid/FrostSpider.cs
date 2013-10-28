using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a frost spider corpse")]
    public class FrostSpider : BaseCreature
    {
        [Constructable]
        public FrostSpider()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a frost spider";
            this.Body = 20;
            this.BaseSoundID = 0x388;

            this.SetStr(76, 100);
            this.SetDex(126, 145);
            this.SetInt(36, 60);

            this.SetHits(46, 60);
            this.SetMana(0);

            this.SetDamage(6, 16);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Cold, 80);

            this.SetResistance(ResistanceType.Physical, 25, 30);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.MagicResist, 25.1, 40.0);
            this.SetSkill(SkillName.Tactics, 35.1, 50.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 65.0);

            this.Fame = 775;
            this.Karma = -775;

            this.VirtualArmor = 28; 

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 74.7;

            this.PackItem(new SpidersSilk(7));
        }

        public FrostSpider(Serial serial)
            : base(serial)
        {
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
                return PackInstinct.Arachnid;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
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

            if (this.BaseSoundID == 387)
                this.BaseSoundID = 0x388;
        }
    }
}