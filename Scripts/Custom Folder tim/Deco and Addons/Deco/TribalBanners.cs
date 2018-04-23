using System;

namespace Server.Items
{
	[FlipableAttribute(40275, 40276)]
	public class JukariBanner : Item
	{
		public override string DefaultName{ get { return "A Jukari Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public JukariBanner ()
			: base(40275)
		{
		}

		public JukariBanner (Serial serial)
			: base(serial)
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
	
	[FlipableAttribute(40277, 40278)]
	public class KurakBanner : Item
	{
		public override string DefaultName{ get { return "A Kurak Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public KurakBanner ()
			: base(40277)
		{
		}

		public KurakBanner(Serial serial)
			: base(serial)
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
	
	[FlipableAttribute(40279, 40280)]
	public class BarakoBanner : Item
	{
		public override string DefaultName{ get { return "A Barako Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public BarakoBanner ()
			: base(40279)
		{
		}

		public BarakoBanner(Serial serial)
			: base(serial)
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
	
	[FlipableAttribute(40281, 40282)]
	public class SakkhraBanner : Item
	{
		public override string DefaultName{ get { return "A Sakkhra Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public SakkhraBanner ()
			: base(40281)
		{
		}

		public SakkhraBanner(Serial serial)
			: base(serial)
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
	
	[FlipableAttribute(40283, 40284)]
	public class BarrabBanner : Item
	{
		public override string DefaultName{ get { return "A Barrab Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public BarrabBanner ()
			: base(40283)
		{
		}

		public BarrabBanner(Serial serial)
			: base(serial)
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
	
	[FlipableAttribute(40285, 40286)]
	public class UraliBanner : Item
	{
		public override string DefaultName{ get { return "A Urali Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public UraliBanner ()
			: base(40285)
		{
		}

		public UraliBanner(Serial serial)
			: base(serial)
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