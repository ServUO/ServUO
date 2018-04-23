//Lagatha the Shield Maiden

using System;

namespace Server.Items
{
	[FlipableAttribute(40386, 40385)]
	public class XmasBow : Item
	{
		public override string DefaultName{ get { return "XmasBow"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public XmasBow  ()
			: base(40386)
		{
		}

		public XmasBow  (Serial serial)
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