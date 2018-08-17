#region References
using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class GypsyBanker : Banker
	{
		[Constructable]
		public GypsyBanker()
		{
			Title = "the gypsy banker";
		}

		public GypsyBanker(Serial serial)
			: base(serial)
		{ }

		public override bool IsActiveVendor { get { return false; } }
		public override NpcGuild NpcGuild { get { return NpcGuild.None; } }
		public override bool ClickTitle { get { return false; } }

		public override void InitOutfit()
		{
			base.InitOutfit();

			switch (Utility.Random(4))
			{
				case 0:
					AddItem(new JesterHat(Utility.RandomBrightHue()));
					break;
				case 1:
					AddItem(new Bandana(Utility.RandomBrightHue()));
					break;
				case 2:
					AddItem(new SkullCap(Utility.RandomBrightHue()));
					break;
			}

			Item item = FindItemOnLayer(Layer.Pants);

			if (item != null)
			{
				item.Hue = Utility.RandomBrightHue();
			}

			item = FindItemOnLayer(Layer.Shoes);

			if (item != null)
			{
				item.Hue = Utility.RandomBrightHue();
			}

			item = FindItemOnLayer(Layer.OuterLegs);

			if (item != null)
			{
				item.Hue = Utility.RandomBrightHue();
			}

			item = FindItemOnLayer(Layer.InnerLegs);

			if (item != null)
			{
				item.Hue = Utility.RandomBrightHue();
			}

			item = FindItemOnLayer(Layer.OuterTorso);

			if (item != null)
			{
				item.Hue = Utility.RandomBrightHue();
			}

			item = FindItemOnLayer(Layer.InnerTorso);

			if (item != null)
			{
				item.Hue = Utility.RandomBrightHue();
			}

			item = FindItemOnLayer(Layer.Shirt);

			if (item != null)
			{
				item.Hue = Utility.RandomBrightHue();
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}