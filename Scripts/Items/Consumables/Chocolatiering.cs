#region Header
// **********
// ServUO - Chocolatiering.cs
// **********
#endregion

using System;
using Server.Engines.Craft;

namespace Server.Items
{
	public class CocoaLiquor : Item
	{
		public override int LabelNumber { get { return 1080007; } } // Cocoa liquor
		public override double DefaultWeight { get { return 1.0; } }

		[Constructable]
		public CocoaLiquor()
			: base(0x103F)
		{
			Hue = 1130;
		}

		public CocoaLiquor(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class SackOfSugar : Item
	{
		public override int LabelNumber { get { return 1080003; } } // Sack of sugar
		public override double DefaultWeight { get { return 1.0; } }

		[Constructable]
		public SackOfSugar()
			: this(1)
		{ }

		[Constructable]
		public SackOfSugar(int amount)
			: base(0x1039)
		{
			Hue = 1121;
			Stackable = true;
			Amount = amount;
		}

		public SackOfSugar(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

    public class CocoaButter : Item, IQuality
    {
        private ItemQuality _Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

        public override int LabelNumber { get { return 1080005; } } // Cocoa butter
        public override double DefaultWeight { get { return 1.0; } }

        public bool PlayerConstructed { get { return true; } }

        [Constructable]
        public CocoaButter()
            : base(0x1044)
        {
            Hue = 1111;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            return quality;
        }

        public CocoaButter(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version > 0)
                _Quality = (ItemQuality)reader.ReadInt();
        }
    }

    public class SweetCocoaButter : Item, IQuality
    {
        private ItemQuality _Quality;

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality { get { return _Quality; } set { _Quality = value; InvalidateProperties(); } }

        public override int LabelNumber { get { return 1156401; } } // Sweet Cocoa butter
        public override double DefaultWeight { get { return 1.0; } }

        public bool PlayerConstructed { get { return true; } }

        [Constructable]
        public SweetCocoaButter()
            : base(0x103D)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            return quality;
        }

        public SweetCocoaButter(Serial serial)
            : base(serial)
        { }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)_Quality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _Quality = (ItemQuality)reader.ReadInt();
        }
    }

	public class Vanilla : Item
	{
		public override int LabelNumber { get { return 1080009; } } // Vanilla
		public override double DefaultWeight { get { return 1.0; } }

		[Constructable]
		public Vanilla()
			: this(1)
		{ }

		[Constructable]
		public Vanilla(int amount)
			: base(0xE2A)
		{
			Hue = 1122;
			Stackable = true;
			Amount = amount;
		}

		public Vanilla(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

	public class CocoaPulp : Item
	{
		public override int LabelNumber { get { return 1080530; } } // cocoa pulp
		public override double DefaultWeight { get { return 1.0; } }

		[Constructable]
		public CocoaPulp()
			: this(1)
		{ }

		[Constructable]
		public CocoaPulp(int amount)
			: base(0xF7C)
		{
			Hue = 537;
			Stackable = true;
			Amount = amount;
		}

		public CocoaPulp(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}

    public class DarkChocolate : BaseSweet
    {
        public override int LabelNumber { get { return 1079994; } } // Dark chocolate
        public override double DefaultWeight { get { return 1.0; } }

        [Constructable]
        public DarkChocolate()
            : base(0xF10)
        {
            Hue = 1125;
            LootType = LootType.Regular;
        }

        public DarkChocolate(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class MilkChocolate : BaseSweet
    {
        public override int LabelNumber { get { return 1079995; } } // Milk chocolate
        public override double DefaultWeight { get { return 1.0; } }

        [Constructable]
        public MilkChocolate()
            : base(0xF18)
        {
            Hue = 1121;
            LootType = LootType.Regular;
        }

        public MilkChocolate(Serial serial)
            : base(serial)
        { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

	public class WhiteChocolate : BaseSweet
	{
		public override int LabelNumber { get { return 1079996; } } // White chocolate
		public override double DefaultWeight { get { return 1.0; } }

		[Constructable]
		public WhiteChocolate()
			: base(0xF11)
		{
			Hue = 1150;
			LootType = LootType.Regular;
		}

		public WhiteChocolate(Serial serial)
			: base(serial)
		{ }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
	}
}