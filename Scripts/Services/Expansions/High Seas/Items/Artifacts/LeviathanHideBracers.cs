using Server;
using System;

namespace Server.Items
{
    public class LeviathanHideBracers : LeatherArms
    {
        public override int LabelNumber { get { return 1116619; } }

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public LeviathanHideBracers()
        {
            Hue = 1274;

            AbsorptionAttributes.CastingFocus = 2;
            Attributes.BonusInt = 6;
            Attributes.AttackChance = 5;
            Attributes.RegenStam = 2;
            Attributes.RegenMana = 2;
            Attributes.LowerManaCost = 8;
            Attributes.LowerRegCost = 10;

            switch (Utility.Random(4))
            {
                case 0: SkillBonuses.SetValues(0, SkillName.Inscribe, 15.0); break;
                case 1: SkillBonuses.SetValues(0, SkillName.Wrestling, 15.0); break;
                case 2: SkillBonuses.SetValues(0, SkillName.Alchemy, 15.0); break;
                case 3: SkillBonuses.SetValues(0, SkillName.Poisoning, 15.0); break;
            }
        }

        public LeviathanHideBracers(Serial serial)
            : base(serial)
        {
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