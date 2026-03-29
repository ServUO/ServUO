using System;

namespace Server.Items
{
    public class ClockworkLeggings : PlateLegs
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ClockworkLeggings()
        {
            Hue = 0xA91;
            Attributes.RegenStam = 5;
            Attributes.DefendChance = 25;
            Attributes.BonusDex = 5;

            switch (Utility.Random(8))
            {
                case 0: SkillBonuses.SetValues(0, SkillName.Mining, 20.0); break;
                case 1: SkillBonuses.SetValues(0, SkillName.Lumberjacking, 20.0); break;
                case 2: SkillBonuses.SetValues(0, SkillName.Blacksmith, 20.0); break;
                case 3: SkillBonuses.SetValues(0, SkillName.Tailoring, 20.0); break;
                case 4: SkillBonuses.SetValues(0, SkillName.Cooking, 20.0); break;
                case 5: SkillBonuses.SetValues(0, SkillName.Fletching, 20.0); break;
                case 6: SkillBonuses.SetValues(0, SkillName.Carpentry, 20.0); break;
                case 7: SkillBonuses.SetValues(0, SkillName.Alchemy, 20.0); break;
            }
        }

        public ClockworkLeggings(Serial serial)  : base(serial)
        {
        }

        public override int LabelNumber { get { return 1153536; } }

        public override int BasePhysicalResistance { get { return 15; } }
        public override int BaseFireResistance { get { return 15; } }
        public override int BaseColdResistance { get { return 15; } }
        public override int BasePoisonResistance { get { return 15; } }
        public override int BaseEnergyResistance { get { return 15; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
