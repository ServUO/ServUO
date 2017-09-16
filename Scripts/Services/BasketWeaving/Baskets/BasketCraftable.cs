using System;
using Server;

namespace Server.Items
{
	public class BasketCraftable : BaseContainer
	{
        public override int LabelNumber { get { return 1022448; } } //basket

		[Constructable]
		public BasketCraftable() : base(9431)
		{
		}
		
		public BasketCraftable(Serial serial) : base(serial)
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
			int v = reader.ReadInt();
		}
	}
}