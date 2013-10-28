using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a scorpion corpse")]
    public class Scorpion : BaseCreature
    {
        [Constructable]
        public Scorpion()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a scorpion";
            this.Body = 48;
            this.BaseSoundID = 397;

            this.SetStr(73, 115);
            this.SetDex(76, 95);
            this.SetInt(16, 30);

            this.SetHits(50, 63);
            this.SetMana(0);

            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Poison, 40);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Fire, 10, 15);
            this.SetResistance(ResistanceType.Cold, 20, 25);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 10, 15);

            this.SetSkill(SkillName.Poisoning, 80.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 30.1, 35.0);
            this.SetSkill(SkillName.Tactics, 60.3, 75.0);
            this.SetSkill(SkillName.Wrestling, 50.3, 65.0);

            this.Fame = 2000;
            this.Karma = -2000;

            this.VirtualArmor = 28;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 47.1;

            this.PackItem(new LesserPoisonPotion());
        }

        public Scorpion(Serial serial)
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
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly);
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
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