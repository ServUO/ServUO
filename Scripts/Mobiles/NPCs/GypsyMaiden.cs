#region Header
// **********
// ServUO - GypsyMaiden.cs
// **********
#endregion

#region References
using System.Collections.Generic;

using Server.Items;
#endregion

namespace Server.Mobiles
{
	public class GypsyMaiden : BaseVendor
	{
		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();

		[Constructable]
		public GypsyMaiden()
			: base("the gypsy maiden")
		{ }

		public GypsyMaiden(Serial serial)
			: base(serial)
		{ }

		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

		public override bool GetGender()
		{
			return true; // always female
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add(new SBProvisioner());
		}

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

			if (Utility.RandomBool())
			{
				AddItem(new HalfApron(Utility.RandomBrightHue()));
			}

			Item item = FindItemOnLayer(Layer.Pants);

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