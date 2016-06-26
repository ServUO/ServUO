using System;

namespace Server.Mobiles
{
    [CorpseName("a maddening horror corpse")]
    public class MaddeningHorror : BaseCreature
    {
        [Constructable]
        public MaddeningHorror()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a maddening horror";
            this.Body = 721;

            this.SetStr(285);
            this.SetDex(80);
            this.SetInt(17);

            this.SetHits(330);

            this.SetDamage(15, 27);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Cold, 40);
            this.SetDamageType(ResistanceType.Energy, 40);

            this.SetResistance(ResistanceType.Physical, 55);
            this.SetResistance(ResistanceType.Fire, 29);
            this.SetResistance(ResistanceType.Cold, 50);
            this.SetResistance(ResistanceType.Poison, 41);
            this.SetResistance(ResistanceType.Energy, 57);

            this.SetSkill(SkillName.EvalInt, 125.9);
            this.SetSkill(SkillName.Magery, 120.4);
            this.SetSkill(SkillName.Meditation, 100.8);
            this.SetSkill(SkillName.MagicResist, 185.5);
            this.SetSkill(SkillName.Tactics, 94.0);
            this.SetSkill(SkillName.Wrestling, 87.4);

            this.Fame = 23000;
            this.Karma = -23000;

            this.QLPoints = 25;
        }

        public MaddeningHorror(Serial serial)
            : base(serial)
        {
        }

        public override int GetIdleSound()
        {
            return 1553;
        }

        public override int GetAngerSound()
        {
            return 1550;
        }

        public override int GetHurtSound()
        {
            return 1552;
        }

        public override int GetDeathSound()
        {
            return 1551;
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