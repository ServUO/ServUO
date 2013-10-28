using System;

namespace Server.Mobiles
{
    [CorpseName("a slith corpse")]
    public class Slith : BaseCreature
    {
        [Constructable]
        public Slith()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a slith";
            this.Body = 734; 

            this.SetStr(129, 136);
            this.SetDex(72, 75);
            this.SetInt(12, 13);

            this.SetHits(84, 85);

            this.SetDamage(6, 24);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 40);
            this.SetResistance(ResistanceType.Fire, 35, 45);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 25, 30);

            this.SetSkill(SkillName.MagicResist, 59.1, 63.5);
            this.SetSkill(SkillName.Tactics, 74.6, 76.4);
            this.SetSkill(SkillName.Wrestling, 62.0, 77.1);

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 80.7;
        }

        public Slith(Serial serial)
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
        public override int Meat
        {
            get
            {
                return 6;
            }
        }
        public override int Hides
        {
            get
            {
                return 10;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average, 2);
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