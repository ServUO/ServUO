using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class SampGear : Bag
	{
		[Constructable]
		public SampGear() : this(1)
		{
			Movable = true;
			Name = "Sampire Gear Set";
			Hue = 1150;
		}
		[Constructable]
		public SampGear(int amount)
		{
			DropItem(new MaceAndShieldGlasses());
			DropItem(new AnimatedLegsoftheInsaneTinker());
			DropItem(new ShroudOfTheCondemned());
			DropItem(new CrimsonCincture());
			DropItem(new CorgulsEnchantedSash());
			DropItem(new JumusSacredHide());
			DropItem(new MinaxsSandles());
			DropItem(new EarringsOfProtection(AosElementAttribute.Fire));
			DropItem(new SampCameo(11));
			DropItem(new SampCameo(13));
			DropItem(new SampCameo(14));
			DropItem(new SampCameo(15));
			DropItem(new SampCameo(16));
			DropItem(new SampCameo(17));
			DropItem(new SampAxe(true));
			DropItem(new SampAxe(false));
			DropItem(new SampNeck());
			DropItem(new SampChest());
			DropItem(new SampArms());
			DropItem(new SampGloves());
			DropItem(new SampRing());
			DropItem(new SampBracelet());
		}

		public SampGear(Serial serial) : base(serial)
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
