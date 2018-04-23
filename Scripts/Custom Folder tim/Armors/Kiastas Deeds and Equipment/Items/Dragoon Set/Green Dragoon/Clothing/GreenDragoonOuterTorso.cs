using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	[Flipable( 0x230E, 0x230D )]
	public class GreenDragoonDress : BaseDragoonClothing
	{
		[Constructable]
		public GreenDragoonDress() : this(0)
		{
		}

		[Constructable]
        public GreenDragoonDress(int hue)  : base(0x230E, Layer.OuterTorso)
        {
            Resource = CraftResource.GreenScales;
            Name = "Dress "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public GreenDragoonDress(Serial serial)
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
    public class GreenDragoonShroud : BaseDragoonClothing
	{
		[Constructable]
		public GreenDragoonShroud() : this(0)
		{
		}

		[Constructable]
        public GreenDragoonShroud(int hue)
            : base(0x2684, Layer.OuterTorso)
        {
            Resource = CraftResource.GreenScales;
            Name = "Shroud "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public GreenDragoonShroud(Serial serial) : base(serial)
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
    public class GreenDragoonMaleElvenRobe : BaseDragoonClothing
	{
		public override Race RequiredRace { get { return Race.Elf; } }

		[Constructable]
		public GreenDragoonMaleElvenRobe() : this(0)
		{
		}

		[Constructable]
        public GreenDragoonMaleElvenRobe(int hue)
            : base(0x2FB9, Layer.OuterTorso)
        {
            Resource = CraftResource.GreenScales;
            Name = "Robe "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public GreenDragoonMaleElvenRobe(Serial serial) : base(serial)
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
	public class GreenDragoonFemaleElvenRobe : BaseDragoonClothing
	{
		public override Race RequiredRace { get { return Race.Elf; } }
        public override bool AllowMaleWearer { get { return false; } }

		[Constructable]
		public GreenDragoonFemaleElvenRobe() : this(0)
		{
		}

		[Constructable]
        public GreenDragoonFemaleElvenRobe(int hue)
            : base(0x2FBA, Layer.OuterTorso)
        {
            Resource = CraftResource.GreenScales;
            Name = "Robe "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public GreenDragoonFemaleElvenRobe(Serial serial)
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