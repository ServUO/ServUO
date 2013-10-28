using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a fire elemental corpse")]
    public class SummonedFireElemental : BaseCreature
    {
        [Constructable]
        public SummonedFireElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a fire elemental";
            this.Body = 15;
            this.BaseSoundID = 838;

            this.SetStr(200);
            this.SetDex(200);
            this.SetInt(100);

            this.SetDamage(9, 14);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Fire, 100);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 70, 80);
            this.SetResistance(ResistanceType.Cold, 0, 10);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.EvalInt, 90.0);
            this.SetSkill(SkillName.Magery, 90.0);
            this.SetSkill(SkillName.MagicResist, 85.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 92.0);

            this.VirtualArmor = 40;
            this.ControlSlots = 4;

            this.AddItem(new LightSource());
        }

        public SummonedFireElemental(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty
        {
            get
            {
                return 117.5;
            }
        }
        public override double DispelFocus
        {
            get
            {
                return 45.0;
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