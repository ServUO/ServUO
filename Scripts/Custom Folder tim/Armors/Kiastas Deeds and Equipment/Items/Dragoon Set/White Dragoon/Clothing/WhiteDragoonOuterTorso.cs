using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	[Flipable( 0x230E, 0x230D )]
	public class WhiteDragoonDress : BaseDragoonClothing
	{
		[Constructable]
		public WhiteDragoonDress() : this(0)
		{
		}

		[Constructable]
        public WhiteDragoonDress(int hue)  : base(0x230E, Layer.OuterTorso)
        {
            Resource = CraftResource.WhiteScales;
            Name = "Dress "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public WhiteDragoonDress(Serial serial)
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
    public class WhiteDragoonShroud : BaseDragoonClothing
	{
		[Constructable]
		public WhiteDragoonShroud() : this(0)
		{
		}

		[Constructable]
        public WhiteDragoonShroud(int hue)
            : base(0x2684, Layer.OuterTorso)
        {
            Resource = CraftResource.WhiteScales;
            Name = "Shroud "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public WhiteDragoonShroud(Serial serial) : base(serial)
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
    public class WhiteDragoonMaleElvenRobe : BaseDragoonClothing
	{
		public override Race RequiredRace { get { return Race.Elf; } }

		[Constructable]
		public WhiteDragoonMaleElvenRobe() : this(0)
		{
		}

		[Constructable]
        public WhiteDragoonMaleElvenRobe(int hue)
            : base(0x2FB9, Layer.OuterTorso)
        {
            Resource = CraftResource.WhiteScales;
            Name = "Robe "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

		public WhiteDragoonMaleElvenRobe(Serial serial) : base(serial)
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
	public class WhiteDragoonFemaleElvenRobe : BaseDragoonClothing
	{
		public override Race RequiredRace { get { return Race.Elf; } }
        public override bool AllowMaleWearer { get { return false; } }

		[Constructable]
		public WhiteDragoonFemaleElvenRobe() : this(0)
		{
		}

		[Constructable]
        public WhiteDragoonFemaleElvenRobe(int hue)
            : base(0x2FBA, Layer.OuterTorso)
        {
            Resource = CraftResource.WhiteScales;
            Name = "Robe "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public WhiteDragoonFemaleElvenRobe(Serial serial)
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