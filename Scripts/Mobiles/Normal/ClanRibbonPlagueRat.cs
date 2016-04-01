using System;

namespace Server.Mobiles
{
    [CorpseName("a rat corpse")]
    public class ClanRibbonPlagueRat : BaseCreature
    {
        [Constructable]
        public ClanRibbonPlagueRat()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "Clan Ribbon Plague Rat";
            this.Body = 238;
            this.BaseSoundID = 0xCC;

            this.SetStr(59);
            this.SetDex(51);
            this.SetInt(17);

            this.SetHits(92);
            this.SetStam(51);

            this.SetDamage(4, 8);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 30, 40);
            this.SetResistance(ResistanceType.Poison, 5, 10);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);
			
            this.SetSkill(SkillName.MagicResist, 30.0);
            this.SetSkill(SkillName.Tactics, 34.0);
            this.SetSkill(SkillName.Wrestling, 40.0);

            this.Fame = 150;
            this.Karma = -150;

            this.VirtualArmor = 6;

            this.Hue = 52;
			
            this.Tamable = false;
            this.ControlSlots = 1;
            this.MinTameSkill = -0.9;
        }

        public ClanRibbonPlagueRat(Serial serial)
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
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat | FoodType.Fish | FoodType.Eggs | FoodType.GrainsAndHay;
            }
        }
        public override void GenerateLoot()
        {
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
        }
    }
}