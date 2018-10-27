using System;

namespace Server.Items
{
    public class SongWovenMantle : LeafArms
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public SongWovenMantle()
        {
            Hue = 0x493;
            SkillBonuses.SetValues(0, SkillName.Musicianship, 10.0);

            Attributes.Luck = 100;
            Attributes.DefendChance = 5;
        }

        public SongWovenMantle(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072931;
            }
        }// Song Woven Mantle
        public override int BasePhysicalResistance
        {
            get
            {
                return 14;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 14;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 16;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}