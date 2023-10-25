using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public class NewbieGiftBox : GiftBox
	{
		[Constructable]
		public NewbieGiftBox() { }
		[Constructable]
		public NewbieGiftBox(Mobile m)
		{
			Movable = false;
			Name = "Newbie Gift Box";
			Hue = 1174;

			DropItem(new BankCheck(50000));
			if (m.Race != Race.Gargoyle)
			{
				DropItem(new NewbieLRCArmor());
			} else {
				DropItem(new NewbieGargoyleArmor());
			}
		}

		public NewbieGiftBox(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt((int)0); // version 
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}
}
