using System;

namespace Server.Mobiles
{
    [CorpseName("an imp corpse")]
    public class ArcaneFiend : BaseCreature
    {
        [Constructable]
        public ArcaneFiend()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an imp";
            this.Body = 74;
            this.BaseSoundID = 422;

            this.SetStr(55);
            this.SetDex(40);
            this.SetInt(60);

            this.SetDamage(10, 14);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Fire, 50);
            this.SetDamageType(ResistanceType.Poison, 50);

            this.SetResistance(ResistanceType.Physical, 25, 35);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 20, 30);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.EvalInt, 20.1, 30.0);
            this.SetSkill(SkillName.Magery, 60.1, 70.0);
            this.SetSkill(SkillName.MagicResist, 30.1, 50.0);
            this.SetSkill(SkillName.Tactics, 42.1, 50.0);
            this.SetSkill(SkillName.Wrestling, 40.1, 44.0);

            this.Fame = 0;
            this.Karma = 0;

            this.ControlSlots = 1;
        }

        public ArcaneFiend(Serial serial)
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
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Daemon;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }//TODO: Verify on OSI.  Guide says this.
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