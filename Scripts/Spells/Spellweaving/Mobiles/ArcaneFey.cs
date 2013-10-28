using System;

namespace Server.Mobiles
{
    [CorpseName("a pixie corpse")]
    public class ArcaneFey : BaseCreature
    {
        [Constructable]
        public ArcaneFey()
            : base(AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4)
        {
            this.Name = NameList.RandomName("pixie");
            this.Body = 128;
            this.BaseSoundID = 0x467;

            this.SetStr(20);
            this.SetDex(150);
            this.SetInt(125);

            this.SetDamage(9, 15);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 80, 90);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.EvalInt, 70.1, 80.0); 
            this.SetSkill(SkillName.Magery, 70.1, 80.0);
            this.SetSkill(SkillName.Meditation, 70.1, 80.0);
            this.SetSkill(SkillName.MagicResist, 50.5, 100.0);
            this.SetSkill(SkillName.Tactics, 10.1, 20.0);
            this.SetSkill(SkillName.Wrestling, 10.1, 12.5);

            this.Fame = 0;
            this.Karma = 0;

            this.ControlSlots = 1;
        }

        public ArcaneFey(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty
        {
            get
            {
                return 70.0;
            }
        }
        public override double DispelFocus
        {
            get
            {
                return 20.0;
            }
        }
        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.FeyAndUndead;
            }
        }
        public override bool InitialInnocent
        {
            get
            {
                return true;
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