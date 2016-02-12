#region Header
// **********
// ServUO - MiscSAResources.cs
// **********
#endregion

#region References
using Server.Engines.Plants;
#endregion

namespace Server.Items
{
	public class DryReeds : Item
	{
		public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled; } }
		public override int LabelNumber { get { return 1112248; } } // Dry Reeds

		private PlantHue m_PlantHue;

		[CommandProperty(AccessLevel.GameMaster)]
		public PlantHue PlantHue
		{
			get { return m_PlantHue; }
			set
			{
				m_PlantHue = value;
				Hue = PlantHueInfo.GetInfo(value).Hue;
				InvalidateProperties();
			}
		}

		public virtual bool RetainsColor { get { return true; } }

		[Constructable]
		public DryReeds()
			: this(1)
		{ }

		[Constructable]
		public DryReeds(int amount)
			: base(0x1BD5)
		{
			Stackable = true;
			Amount = amount;
		}

		public DryReeds(Serial serial)
			: base(serial)
		{ }

		public override void AddNameProperty(ObjectPropertyList list)
		{
			list.Add(1112289, "#" + PlantHueInfo.GetInfo(m_PlantHue).Name); // ~1_COLOR~ dry reeds
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write((int)m_PlantHue);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					m_PlantHue = (PlantHue)reader.ReadInt();
					break;
			}
		}
	}

	public class SoftenedReeds : Item
	{
		public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled; } }
		public override int LabelNumber { get { return 1112246; } } // Softened Reeds

		private PlantHue m_PlantHue;

		[CommandProperty(AccessLevel.GameMaster)]
		public PlantHue PlantHue
		{
			get { return m_PlantHue; }
			set
			{
				m_PlantHue = value;
				Hue = PlantHueInfo.GetInfo(value).Hue;
				InvalidateProperties();
			}
		}

		public virtual bool RetainsColor { get { return true; } }

		[Constructable]
		public SoftenedReeds()
			: this(1)
		{ }

		[Constructable]
		public SoftenedReeds(int amount)
			: base(0x4006)
		{
			Stackable = true;
			Amount = amount;
		}

		public SoftenedReeds(Serial serial)
			: base(serial)
		{ }

		public override void AddNameProperty(ObjectPropertyList list)
		{
			list.Add(1112346, "#" + PlantHueInfo.GetInfo(m_PlantHue).Name); // ~1_COLOR~ Softened Reeds
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}