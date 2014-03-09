using System;

namespace Server.Mobiles
{
    [CorpseName("a skree corpse")]
    public class Skree : BaseCreature
    {
        [Constructable]
        public Skree()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a skree";
            this.Body = 733; 

            this.SetStr(297, 330);
            this.SetDex(96, 124);
            this.SetInt(188, 260);

            this.SetHits(205, 300);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 45, 55);
            this.SetResistance(ResistanceType.Cold, 25, 40);
            this.SetResistance(ResistanceType.Poison, 55, 65);
            this.SetResistance(ResistanceType.Energy, 25, 40);

            this.SetSkill(SkillName.EvalInt, 90.6, 100.0);
            this.SetSkill(SkillName.Magery, 90.2, 114.2);
            this.SetSkill(SkillName.Meditation, 65.3, 75.0);
            this.SetSkill(SkillName.MagicResist, 75.1, 90.0);
            this.SetSkill(SkillName.Tactics, 20.2, 24.7);
            this.SetSkill(SkillName.Wrestling, 20.2, 34.8);

            this.Tamable = true;
            this.ControlSlots = 4;
            this.MinTameSkill = 95.1;
        }

        public Skree(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 3;
            }
        }
        public override MeatType MeatType
        {
            get
            {
                return MeatType.Bird;
            }
        }
        public override int Hides
        {
            get
            {
                return 5;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
        }

        public override int GetIdleSound()
        {
            return 1585;
        }

        public override int GetAngerSound()
        {
            return 1582;
        }

        public override int GetHurtSound()
        {
            return 1584;
        }

        public override int GetDeathSound()
        {
            return 1583;
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