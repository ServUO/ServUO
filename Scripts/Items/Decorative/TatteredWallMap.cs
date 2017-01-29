using Server;
using System;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
	public class TatteredWallMapSouth : Item
	{
        public override int LabelNumber { get { return 1031635; } } // tattered wall map

		[Constructable]
		public TatteredWallMapSouth() : base(11636)
		{
		}
	
        public TatteredWallMapSouth(Serial serial)
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

    public class TatteredWallMapEast : Item
    {
        public override int LabelNumber { get { return 1031635; } } // tattered wall map

        [Constructable]
        public TatteredWallMapEast()
            : base(11635)
        {
        }

        public TatteredWallMapEast(Serial serial)
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