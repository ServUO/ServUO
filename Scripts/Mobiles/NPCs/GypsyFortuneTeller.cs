#region References
using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class GypsyFortuneTeller : BaseHealer
	{
		[Constructable]
		public GypsyFortuneTeller()
		{
			Title = "the fortune teller";
			SetSkill(SkillName.Begging, 64.0, 100.0);
		}

		public GypsyFortuneTeller(Serial serial)
			: base(serial)
		{ }

        public override bool IsInvulnerable => true;

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