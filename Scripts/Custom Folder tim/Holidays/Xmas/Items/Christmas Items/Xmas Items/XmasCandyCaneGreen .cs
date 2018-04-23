//Lagatha the Shield Maiden

using System;

namespace Server.Items
{
	[FlipableAttribute(40387, 40388)]
	public class XmasCandyCaneGreen  : Item
	{
		public override string DefaultName{ get { return "XmasCandyCaneGreen"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public XmasCandyCaneGreen  ()
			: base(40387)
		{
		}

		public XmasCandyCaneGreen  (Serial serial)
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