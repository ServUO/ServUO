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

            this.SetStr(305, 330);
            this.SetDex(114, 119);
            this.SetInt(191, 260);

            this.SetHits(228, 310);

            this.SetDamage(5, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 45, 55);
            this.SetResistance(ResistanceType.Cold, 25, 40);
            this.SetResistance(ResistanceType.Poison, 55, 65);
            this.SetResistance(ResistanceType.Energy, 26, 40);

            this.SetSkill(SkillName.EvalInt, 90.8, 99.7);
            this.SetSkill(SkillName.Magery, 100.0, 115.0);
            this.SetSkill(SkillName.Meditation, 69.7, 73.7);
            this.SetSkill(SkillName.MagicResist, 75.3, 82.6);
            this.SetSkill(SkillName.Tactics, 20.1, 24.2);
            this.SetSkill(SkillName.Wrestling, 22.9, 32.7);

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
                return 6;
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