using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a giant black widow spider corpse")] // stupid corpse name
    public class GiantBlackWidow : BaseCreature
    {
        [Constructable]
        public GiantBlackWidow()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a giant black widow";
            this.Body = 0x9D;
            this.BaseSoundID = 0x388; // TODO: validate

            this.SetStr(76, 100);
            this.SetDex(96, 115);
            this.SetInt(36, 60);

            this.SetHits(46, 60);

            this.SetDamage(5, 17);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 30);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Anatomy, 30.3, 75.0);
            this.SetSkill(SkillName.Poisoning, 60.1, 80.0);
            this.SetSkill(SkillName.MagicResist, 45.1, 60.0);
            this.SetSkill(SkillName.Tactics, 65.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 70.1, 85.0);

            this.Fame = 3500;
            this.Karma = -3500;

            this.VirtualArmor = 24;

            this.PackItem(new SpidersSilk(5));
            this.PackItem(new LesserPoisonPotion());
            this.PackItem(new LesserPoisonPotion());
        }

        public GiantBlackWidow(Serial serial)
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
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Deadly;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Deadly;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
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