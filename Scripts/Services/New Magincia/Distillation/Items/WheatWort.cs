using Server;
using System;
using Server.Engines.Distillation;

namespace Server.Items
{
	public class WheatWort : Item
	{
		public override int LabelNumber { get { return 1150275; } } // wheat wort
		
		[Constructable]
		public WheatWort() : this(1)
		{
		}
		
		[Constructable]
		public WheatWort(int num) : base(3625)
		{
			Stackable = true;
			Amount = num;
            Hue = 1281;
		}
		
		public WheatWort(Serial serial) : base(serial)
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