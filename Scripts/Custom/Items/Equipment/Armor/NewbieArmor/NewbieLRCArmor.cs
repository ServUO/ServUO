using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class NewbieLRCArmor : Bag
	{
		[Constructable]
		public NewbieLRCArmor() : this(1)
		{
			Movable = true;
			Name = "Newbie Promised LRC Gear";
			Hue = 1150;
		}
		[Constructable]
		public NewbieLRCArmor(int amount)
		{
			DropItem(new NewbieCap(0x592));
			DropItem(new NewbieChest(0x592));
			DropItem(new NewbieLegs(0x592));
			DropItem(new NewbieArms(0x592));
			DropItem(new NewbieGloves(0x592));
			DropItem(new NewbieNeck(0x592));
		}

		public NewbieLRCArmor(Serial serial) : base(serial)
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
