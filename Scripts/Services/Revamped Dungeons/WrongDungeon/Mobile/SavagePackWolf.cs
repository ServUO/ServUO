using System;

namespace Server.Mobiles
{
    [CorpseName("a wolf corpse")]
    [TypeAlias("Server.Mobiles.SavagePackwolf")]
    public class SavagePackWolf : BaseCreature
    {
        [Constructable]
        public SavagePackWolf()
            : base(AIType.AI_Animal, FightMode.Weakest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a savage pack wolf";
            this.Body = 0xE1;
            this.BaseSoundID = 0xE5;

            this.SetStr(100, 116);
            this.SetDex(51, 61);
            this.SetInt(11, 21);

            this.SetHits(650, 671);
            this.SetMana(0);

            this.SetDamage(9, 12);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.MagicResist, 60.7, 74.0);
            this.SetSkill(SkillName.Tactics, 80.9, 94.4);
            this.SetSkill(SkillName.Wrestling, 89.0, 97.1);

            this.Fame = 450;
            this.Karma = 3000;

            this.VirtualArmor = 26;
            this.Tamable = false;
        }

        public SavagePackWolf(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override int Meat { get { return 1; } }
        public override int Hides { get { return 5; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Canine; } }

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