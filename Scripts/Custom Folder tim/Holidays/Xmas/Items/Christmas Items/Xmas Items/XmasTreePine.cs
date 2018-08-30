//Lagatha the Shield Maiden

using System;

namespace Server.Items
{
	[FlipableAttribute(40379, 40376,40375)]
	public class XmasTreePine : Item
	{
		public override string DefaultName{ get { return "XmasTreePine"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public XmasTreePine ()
			: base(40379)
		{
		}

		public XmasTreePine (Serial serial)
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