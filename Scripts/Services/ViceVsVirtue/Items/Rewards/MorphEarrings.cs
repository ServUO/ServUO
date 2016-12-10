using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.VvV
{
    public class MorphEarrings : GoldEarrings
	{
        public override int LabelNumber
        {
            get
            {
                return 1094746; // Morph Earrings
            }
        }

        public MorphEarrings()
        {
            IsVvVItem = true;
        }

        public MorphEarrings(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}