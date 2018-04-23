using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	[Flipable( 0x230E, 0x230D )]
	public class RandomColorDragoonDress : BaseDragoonClothing
	{
		[Constructable]
		public RandomColorDragoonDress() : this(0)
		{
		}

		[Constructable]
        public RandomColorDragoonDress(int hue)  : base(0x230E, Layer.OuterTorso)
		{
            Name = "Dress "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public RandomColorDragoonDress(Serial serial) : base(serial)
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
	public class RandomColorDragoonShroud : BaseDragoonClothing
	{
		[Constructable]
		public RandomColorDragoonShroud() : this(0)
		{
		}

		[Constructable]
        public RandomColorDragoonShroud(int hue) : base(0x2684, Layer.OuterTorso)
		{
            Name = "Shroud "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public RandomColorDragoonShroud(Serial serial) : base(serial)
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
	public class RandomColorDragoonMaleElvenRobe : BaseDragoonClothing
	{
		public override Race RequiredRace { get { return Race.Elf; } }

		[Constructable]
		public RandomColorDragoonMaleElvenRobe() : this(0)
		{
		}

		[Constructable]
        public RandomColorDragoonMaleElvenRobe(int hue)
            : base(0x2FB9, Layer.OuterTorso)
		{
            Name = "Robe "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public RandomColorDragoonMaleElvenRobe(Serial serial) : base(serial)
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
	public class RandomColorDragoonFemaleElvenRobe : BaseDragoonClothing
	{
		public override Race RequiredRace { get { return Race.Elf; } }
        public override bool AllowMaleWearer { get { return false; } }

		[Constructable]
		public RandomColorDragoonFemaleElvenRobe() : this(0)
		{
		}

		[Constructable]
        public RandomColorDragoonFemaleElvenRobe(int hue)
            : base(0x2FBA, Layer.OuterTorso)
		{
            Name = "Robe "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public RandomColorDragoonFemaleElvenRobe(Serial serial) : base(serial)
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