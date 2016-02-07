using System;

namespace Server.Mobiles
{
    [CorpseName("a crane corpse")]
    public class Crane : BaseCreature
    {
        [Constructable]
        public Crane()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a crane";
            this.Body = 254;
            this.BaseSoundID = 0x4D7;

            this.SetStr(26, 35);
            this.SetDex(16, 25);
            this.SetInt(11, 15);

            this.SetHits(26, 35);
            this.SetMana(0);

            this.SetDamage(1, 1);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 5);

            this.SetSkill(SkillName.MagicResist, 4.1, 5.0);
            this.SetSkill(SkillName.Tactics, 10.1, 11.0);
            this.SetSkill(SkillName.Wrestling, 10.1, 11.0);

            this.Fame = 0;
            this.Karma = 200;

            this.VirtualArmor = 5;
        }

        public Crane(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int Feathers
        {
            get
            {
                return 25;
            }
        }
        public override int GetAngerSound()
        {
            return 0x4D9;
        }

        public override int GetIdleSound()
        {
            return 0x4D8;
        }

        public override int GetAttackSound()
        {
            return 0x4D7;
        }

        public override int GetHurtSound()
        {
            return 0x4DA;
        }

        public override int GetDeathSound()
        {
            return 0x4D6;
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