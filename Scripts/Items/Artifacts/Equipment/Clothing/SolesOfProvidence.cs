using System;

namespace Server.Items
{
    public class SolesOfProvidence : Sandals, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1113376; } } // Soles of Providence
		
        [Constructable]
		public SolesOfProvidence()
		{
            Attributes.Luck = 80;
            Hue = 1177;
		}

        public SolesOfProvidence(Serial serial)
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