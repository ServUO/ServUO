using System;

namespace Server.Mobiles
{
    [CorpseName("a hell cat corpse")]
    [TypeAlias("Server.Mobiles.Preditorhellcat")]
    public class PredatorHellCat : BaseCreature
    {
        [Constructable]
        public PredatorHellCat()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a hell cat";
            this.Body = 127;
            this.BaseSoundID = 0xBA;

            this.SetStr(161, 185);
            this.SetDex(96, 115);
            this.SetInt(76, 100);

            this.SetHits(97, 131);

            this.SetDamage(5, 17);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Fire, 25);

            this.SetResistance(ResistanceType.Physical, 25, 35);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Energy, 5, 15);

            this.SetSkill(SkillName.MagicResist, 75.1, 90.0);
            this.SetSkill(SkillName.Tactics, 50.1, 65.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 65.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.VirtualArmor = 30;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 89.1;
        }

        public PredatorHellCat(Serial serial)
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