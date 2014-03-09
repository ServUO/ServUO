using System;

namespace Server.Mobiles
{
    [CorpseName("a raptor corpse")]
    public class Raptor : BaseCreature
    {
        [Constructable]
        public Raptor()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a raptor";
            this.Body = 730; 

            this.SetStr(401, 464);
            this.SetDex(131, 154);
            this.SetInt(102, 150);

            this.SetHits(342, 400);
			this.SetMana(102, 150);
			this.SetStam(131, 154);

            this.SetDamage(11, 17);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 45, 50);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 75.5, 89.0);
            this.SetSkill(SkillName.Tactics, 80.3, 93.8);
            this.SetSkill(SkillName.Wrestling, 66.9, 81.5);

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 107.1;
        }

        public Raptor(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 7;
            }
        }
        public override int Hides
        {
            get
            {
                return 11;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Horned;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 2);
        }

        public override int GetIdleSound()
        {
            return 1573;
        }

        public override int GetAngerSound()
        {
            return 1570;
        }

        public override int GetHurtSound()
        {
            return 1572;
        }

        public override int GetDeathSound()
        {
            return 1571;
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