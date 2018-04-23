using System;

namespace Server.Items
{
	[FlipableAttribute(19298, 19307)]
	public class TrinsicBanner : Item
	{
		public override string DefaultName{ get { return "A Trinsic Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public TrinsicBanner ()
			: base(19298)
		{
		}

		public TrinsicBanner (Serial serial)
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
	
	[FlipableAttribute(19299, 19308)]
	public class MoonglowBanner : Item
	{
		public override string DefaultName{ get { return "A Moonglow Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public MoonglowBanner ()
			: base(19299)
		{
		}

		public MoonglowBanner(Serial serial)
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
	
	[FlipableAttribute(19300, 19309)]
	public class BritianBanner : Item
	{
		public override string DefaultName{ get { return "A Britian Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public BritianBanner ()
			: base(19300)
		{
		}

		public BritianBanner(Serial serial)
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
	
	[FlipableAttribute(19301, 19310)]
	public class JhelomBanner : Item
	{
		public override string DefaultName{ get { return "A Jhelom Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public JhelomBanner ()
			: base(19301)
		{
		}

		public JhelomBanner(Serial serial)
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
	
	[FlipableAttribute(19302, 19311)]
	public class YewBanner : Item
	{
		public override string DefaultName{ get { return "A Yew Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public YewBanner ()
			: base(19302)
		{
		}

		public YewBanner(Serial serial)
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
	
	[FlipableAttribute(19303, 19312)]
	public class MinocBanner : Item
	{
		public override string DefaultName{ get { return "A Minoc Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public MinocBanner ()
			: base(19303)
		{
		}

		public MinocBanner(Serial serial)
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
	
	[FlipableAttribute(19304, 19313)]
	public class VesperBanner : Item
	{
		public override string DefaultName{ get { return "A Vesper Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public VesperBanner ()
			: base(19304)
		{
		}

		public VesperBanner(Serial serial)
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
	
	[FlipableAttribute(19305, 19314)]
	public class NewMaginciaBanner : Item
	{
		public override string DefaultName{ get { return "A New Magincia Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public NewMaginciaBanner ()
			: base(19305)
		{
		}

		public NewMaginciaBanner(Serial serial)
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
	
	[FlipableAttribute(19306, 19315)]
	public class SkaraBraeBanner : Item
	{
		public override string DefaultName{ get { return "A Skara Brae Banner"; } }
		public override double DefaultWeight{ get { return 5.0; } }
		
		[Constructable]
		public SkaraBraeBanner ()
			: base(19306)
		{
		}

		public SkaraBraeBanner(Serial serial)
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