using System;
using Server.Network;

namespace Server.Items
{
	public class BlessBag : Container
	{
		[Constructable]
		public BlessBag() : base(0xE76)
		{

			Name = "A Blessing Bag";
			Weight = 0.0;
            Hue = 0;
			LootType = LootType.Blessed;

		}

		public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
		{
			if (!base.OnDragDropInto(from, item, p))
			return false;
			item.LootType = LootType.Blessed;
			from.SendMessage("Your Stuff Is Blessed");
			return true;
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if (!base.OnDragDrop(from, dropped))
			return false;
			dropped.LootType = LootType.Blessed;
			from.SendMessage("Your Stuff Is Blessed");
			return true;
		}

		public BlessBag(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version
         }

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
            int version = reader.ReadInt();

			
		}


	}
}