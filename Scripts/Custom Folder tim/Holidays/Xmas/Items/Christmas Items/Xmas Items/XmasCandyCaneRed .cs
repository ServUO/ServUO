//Lagatha the Shield Maiden

using System;

namespace Server.Items
{
	[FlipableAttribute(40475, 40476)]
	public class XmasCandyCaneRed  : Item
	{
		public override string DefaultName{ get { return "XmasCandyCaneRed"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public XmasCandyCaneRed  ()
			: base(40475)
		{
		}

		public XmasCandyCaneRed  (Serial serial)
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