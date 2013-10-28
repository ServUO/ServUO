using System;

namespace Server.Mobiles
{
    [CorpseName("a gargoyle corpse")]
    public class GargoylePet : BaseCreature
    {
        [Constructable]
        public GargoylePet()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a gargoyle pet";
            this.Body = 730; 

            this.SetStr(500, 512);
            this.SetDex(90, 94);
            this.SetInt(100, 107);

            this.SetHits(300, 313);

            this.SetDamage(11, 17);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 60);
            this.SetResistance(ResistanceType.Fire, 40);
            this.SetResistance(ResistanceType.Cold, 40);
            this.SetResistance(ResistanceType.Poison, 40);
            this.SetResistance(ResistanceType.Energy, 40);

            this.SetSkill(SkillName.MagicResist, 75.5, 89.0);
            this.SetSkill(SkillName.Tactics, 80.3, 93.8);
            this.SetSkill(SkillName.Wrestling, 66.9, 81.5);

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 65.1;
        }

        public GargoylePet(Serial serial)
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