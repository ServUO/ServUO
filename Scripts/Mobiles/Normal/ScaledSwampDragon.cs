using System;

namespace Server.Mobiles
{
    [CorpseName("a swamp dragon corpse")]
    public class ScaledSwampDragon : BaseMount
    {
        [Constructable]
        public ScaledSwampDragon()
            : this("a swamp dragon")
        {
        }

        [Constructable]
        public ScaledSwampDragon(string name)
            : base(name, 0x31F, 0x3EBE, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.SetStr(201, 300);
            this.SetDex(66, 85);
            this.SetInt(61, 100);

            this.SetHits(121, 180);

            this.SetDamage(3, 4);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Poison, 25);

            this.SetResistance(ResistanceType.Physical, 35, 40);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 20, 40);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.Anatomy, 45.1, 55.0);
            this.SetSkill(SkillName.MagicResist, 45.1, 55.0);
            this.SetSkill(SkillName.Tactics, 45.1, 55.0);
            this.SetSkill(SkillName.Wrestling, 45.1, 55.0);

            this.Fame = 2000;
            this.Karma = -2000;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 93.9;
        }

        public ScaledSwampDragon(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel
        {
            get
            {
                return !this.Controlled;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override double GetControlChance(Mobile m, bool useBaseSkill)
        {
            return 1.0;
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
        }
    }
}