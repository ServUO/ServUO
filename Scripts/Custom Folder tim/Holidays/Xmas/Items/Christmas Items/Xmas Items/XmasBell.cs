//Lagatha the Shield Maiden

using System;

namespace Server.Items
{
	[FlipableAttribute(40384, 40383)]
	public class XmasBell : Item
	{
		public override string DefaultName{ get { return "XmasBell"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public XmasBell  ()
			: base(40384)
		{
		}

		public XmasBell  (Serial serial)
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