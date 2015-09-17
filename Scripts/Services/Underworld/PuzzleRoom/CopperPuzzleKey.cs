using System;
using Server;

namespace Server.Items
{
	public class CopperPuzzleKey : BaseDecayingItem
	{
		public override int Lifespan { get { return 1800; } }
        public override int LabelNumber { get { return 1024110; } } // copper key
		
		[Constructable]
		public CopperPuzzleKey() : base(4115)
		{
		}
		
		public CopperPuzzleKey(Serial serial) : base(serial)
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