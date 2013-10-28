using System;

namespace Server.Mobiles
{
    [CorpseName("a giant ice worm corpse")]
    public class GiantIceWorm : BaseCreature
    {
        [Constructable]
        public GiantIceWorm()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Body = 89;
            this.Name = "a giant ice worm";
            this.BaseSoundID = 0xDC;

            this.SetStr(216, 245);
            this.SetDex(76, 100);
            this.SetInt(66, 85);

            this.SetHits(130, 147);

            this.SetDamage(7, 17);

            this.SetDamageType(ResistanceType.Physical, 10);
            this.SetDamageType(ResistanceType.Cold, 90);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Fire, 0);
            this.SetResistance(ResistanceType.Cold, 80, 90);
            this.SetResistance(ResistanceType.Poison, 15, 25);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Poisoning, 75.1, 95.0);
            this.SetSkill(SkillName.MagicResist, 45.1, 60.0);
            this.SetSkill(SkillName.Tactics, 75.1, 80.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 4500;
            this.Karma = -4500;

            this.VirtualArmor = 40;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 71.1;
        }

        public GiantIceWorm(Serial serial)
            : base(serial)
        {
        }

        public override bool SubdueBeforeTame
        {
            get
            {
                return true;
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
                return Poison.Greater;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
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