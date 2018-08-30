using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	[Flipable( 0x230E, 0x230D )]
	public class YellowDragoonDress : BaseDragoonClothing
	{
		[Constructable]
		public YellowDragoonDress() : this(0)
		{
		}

		[Constructable]
        public YellowDragoonDress(int hue)  : base(0x230E, Layer.OuterTorso)
        {
            Resource = CraftResource.YellowScales;
            Name = "Dress "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public YellowDragoonDress(Serial serial)
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

    [Flipable(0x2684, 0x2683)]
    public class YellowDragoonShroud : BaseDragoonClothing
	{
		[Constructable]
		public YellowDragoonShroud() : this(0)
		{
		}

		[Constructable]
        public YellowDragoonShroud(int hue)
            : base(0x2684, Layer.OuterTorso)
        {
            Resource = CraftResource.YellowScales;
            Name = "Shroud "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public YellowDragoonShroud(Serial serial) : base(serial)
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

	[Flipable(0x2FB9, 0x3173)]
    public class YellowDragoonMaleElvenRobe : BaseDragoonClothing
	{
		public override Race RequiredRace { get { return Race.Elf; } }

		[Constructable]
		public YellowDragoonMaleElvenRobe() : this(0)
		{
		}

		[Constructable]
        public YellowDragoonMaleElvenRobe(int hue)
            : base(0x2FB9, Layer.OuterTorso)
        {
            Resource = CraftResource.YellowScales;
            Name = "Robe "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public YellowDragoonMaleElvenRobe(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	[Flipable(0x2FBA, 0x3174)]
	public class YellowDragoonFemaleElvenRobe : BaseDragoonClothing
	{
		public override Race RequiredRace { get { return Race.Elf; } }
        public override bool AllowMaleWearer { get { return false; } }

		[Constructable]
		public YellowDragoonFemaleElvenRobe() : this(0)
		{
		}

		[Constructable]
        public YellowDragoonFemaleElvenRobe(int hue)
            : base(0x2FBA, Layer.OuterTorso)
        {
            Resource = CraftResource.YellowScales;
            Name = "Robe "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public YellowDragoonFemaleElvenRobe(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}
}