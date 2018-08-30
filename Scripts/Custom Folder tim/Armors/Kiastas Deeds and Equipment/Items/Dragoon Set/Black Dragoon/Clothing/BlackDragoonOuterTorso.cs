using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	[Flipable( 0x230E, 0x230D )]
	public class BlackDragoonDress : BaseDragoonClothing
	{
		[Constructable]
		public BlackDragoonDress() : this(0)
		{
		}

		[Constructable]
        public BlackDragoonDress(int hue)  : base(0x230E, Layer.OuterTorso)
        {
            Resource = CraftResource.BlackScales;
            Name = "Dress "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public BlackDragoonDress(Serial serial)
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
    public class BlackDragoonShroud : BaseDragoonClothing
	{
		[Constructable]
		public BlackDragoonShroud() : this(0)
		{
		}

		[Constructable]
        public BlackDragoonShroud(int hue)
            : base(0x2684, Layer.OuterTorso)
        {
            Resource = CraftResource.BlackScales;
            Name = "Shroud "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public BlackDragoonShroud(Serial serial) : base(serial)
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
    public class BlackDragoonMaleElvenRobe : BaseDragoonClothing
	{
		public override Race RequiredRace { get { return Race.Elf; } }

		[Constructable]
		public BlackDragoonMaleElvenRobe() : this(0)
		{
		}

		[Constructable]
        public BlackDragoonMaleElvenRobe(int hue)
            : base(0x2FB9, Layer.OuterTorso)
        {
            Resource = CraftResource.BlackScales;
            Name = "Robe "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public BlackDragoonMaleElvenRobe(Serial serial) : base(serial)
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
	public class BlackDragoonFemaleElvenRobe : BaseDragoonClothing
	{
		public override Race RequiredRace { get { return Race.Elf; } }
        public override bool AllowMaleWearer { get { return false; } }

		[Constructable]
		public BlackDragoonFemaleElvenRobe() : this(0)
		{
		}

		[Constructable]
        public BlackDragoonFemaleElvenRobe(int hue)
            : base(0x2FBA, Layer.OuterTorso)
        {
            Resource = CraftResource.BlackScales;
            Name = "Robe "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public BlackDragoonFemaleElvenRobe(Serial serial)
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