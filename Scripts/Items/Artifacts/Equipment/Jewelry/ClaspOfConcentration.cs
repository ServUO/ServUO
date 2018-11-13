using System;

namespace Server.Items
{
    public class ClaspOfConcentration : SilverBracelet
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ClaspOfConcentration()
        {
            LootType = LootType.Blessed;
            Attributes.RegenStam = 2;
            Attributes.RegenMana = 1;
            Resistances.Fire = 5;
            Resistances.Cold = 5;
        }

        public ClaspOfConcentration(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1077695;
            }
        }// Clasp of Concentration
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}