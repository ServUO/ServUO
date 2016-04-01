using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a fire steed corpse")]
    public class FireSteed : BaseMount
    {
        [Constructable]
        public FireSteed()
            : this("a fire steed")
        {
        }

        [Constructable]
        public FireSteed(string name)
            : base(name, 0xBE, 0x3E9E, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.BaseSoundID = 0xA8;

            this.SetStr(376, 400);
            this.SetDex(91, 120);
            this.SetInt(291, 300);

            this.SetHits(226, 240);

            this.SetDamage(11, 30);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Fire, 80);

            this.SetResistance(ResistanceType.Physical, 30, 40);
            this.SetResistance(ResistanceType.Fire, 70, 80);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 100.0, 120.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0);

            this.Fame = 20000;
            this.Karma = -20000;

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 106.0;
        }

        public FireSteed(Serial serial)
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
                return PackInstinct.Daemon | PackInstinct.Equine;
            }
        }
        public override void GenerateLoot()
        {
            this.PackItem(new SulfurousAsh(Utility.RandomMinMax(151, 300)));
            this.PackItem(new Ruby(Utility.RandomMinMax(16, 30)));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.BaseSoundID <= 0)
                this.BaseSoundID = 0xA8;

            if (version < 1)
            {
                for (int i = 0; i < this.Skills.Length; ++i)
                {
                    this.Skills[i].Cap = Math.Max(100.0, this.Skills[i].Cap * 0.9);

                    if (this.Skills[i].Base > this.Skills[i].Cap)
                    {
                        this.Skills[i].Base = this.Skills[i].Cap;
                    }
                }
            }
        }
    }
}