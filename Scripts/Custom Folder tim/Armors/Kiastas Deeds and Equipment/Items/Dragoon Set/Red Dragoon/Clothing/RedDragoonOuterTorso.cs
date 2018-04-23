using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	[Flipable( 0x230E, 0x230D )]
	public class RedDragoonDress : BaseDragoonClothing
	{
		[Constructable]
		public RedDragoonDress() : this(0)
		{
		}

		[Constructable]
        public RedDragoonDress(int hue)  : base(0x230E, Layer.OuterTorso)
        {
            Resource = CraftResource.RedScales;
            Name = "Dress "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public RedDragoonDress(Serial serial)
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
    public class RedDragoonShroud : BaseDragoonClothing
	{
		[Constructable]
		public RedDragoonShroud() : this(0)
		{
		}

		[Constructable]
        public RedDragoonShroud(int hue)
            : base(0x2684, Layer.OuterTorso)
        {
            Resource = CraftResource.RedScales;
            Name = "Shroud "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public RedDragoonShroud(Serial serial) : base(serial)
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
    public class RedDragoonMaleElvenRobe : BaseDragoonClothing
	{
		public override Race RequiredRace { get { return Race.Elf; } }

		[Constructable]
		public RedDragoonMaleElvenRobe() : this(0)
		{
		}

		[Constructable]
        public RedDragoonMaleElvenRobe(int hue)
            : base(0x2FB9, Layer.OuterTorso)
        {
            Resource = CraftResource.RedScales;
            Name = "Robe "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public RedDragoonMaleElvenRobe(Serial serial) : base(serial)
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
	public class RedDragoonFemaleElvenRobe : BaseDragoonClothing
	{
		public override Race RequiredRace { get { return Race.Elf; } }
        public override bool AllowMaleWearer { get { return false; } }

		[Constructable]
		public RedDragoonFemaleElvenRobe() : this(0)
		{
		}

		[Constructable]
        public RedDragoonFemaleElvenRobe(int hue)
            : base(0x2FBA, Layer.OuterTorso)
        {
            Resource = CraftResource.RedScales;
            Name = "Robe "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public RedDragoonFemaleElvenRobe(Serial serial)
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