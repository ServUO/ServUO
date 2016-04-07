using System;

namespace Server.Items
{
    public class Lavaliere : GoldNecklace
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public Lavaliere()
        {
            this.Name = ("Lavaliere");
		
            this.Hue = 39;
			
            this.AbsorptionAttributes.EaterKinetic = 20;
            this.Attributes.DefendChance = 10;
            this.Resistances.Physical = 15;
            this.Attributes.LowerManaCost = 10;
            this.Attributes.LowerRegCost = 20;
        }

        public Lavaliere(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072937;
            }
        }// Pendant of the Magi
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