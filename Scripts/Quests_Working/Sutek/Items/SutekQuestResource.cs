#region References
using System;
#endregion

namespace Server.Items
{
	public enum SutekResourceType
	{
		BarrelHoops,
		BarrelStaves,
		Beeswax,
		BlackPowder,
		BluePotion,
		BluePowder,
		Bones,
		BrownStone,
		CopperIngots,
		CopperWire,
		DarkStone,
		Feathers,
		FetidEssence,
		Gears,
		GoldIngots,
		GoldWire,
		IronIngots,
		IronWire,
		Leather,
		MeltedWax,
		OilOfVitriol,
		PowerCrystal,
		PurplePotion,
		RedPotion,
		Rope,
		Scales,
		Shafts,
		SilverIngots,
		SilverWire,
		SpiritEssence,
		Thorns,
		VoidEssence,
		WhitePowder,
		WhiteStone,
		WoodenBoards,
		WoodenLogs,
		YellowPotion,
	}

	public class SutekQuestResource : Item
	{
		// actual resource, hue, label number, itemid
		private static readonly int[][] _Table =
		{
			new[] {(int)SutekResourceType.BarrelHoops, 0, 1011228, 0x1DB7},
			new[] {(int)SutekResourceType.BarrelStaves, 0, 1015102, 0x1EB1},
			new[] {(int)SutekResourceType.Beeswax, 0, 1025154, 0x1422},
			new[] {(int)SutekResourceType.BlackPowder, 1, 1112815, 0x0B48},
			new[] {(int)SutekResourceType.BluePotion, 0, 1023848, 0x182A},
			new[] {(int)SutekResourceType.BluePowder, 0, 1112817, 0x241E},
			new[] {(int)SutekResourceType.Bones, 0, 1023786, 0x318C},
			new[] {(int)SutekResourceType.BrownStone, 2413, 1112814, 0x1772},
			new[] {(int)SutekResourceType.CopperIngots, 0, 1027140, 0x1BE8},
			new[] {(int)SutekResourceType.CopperWire, 0, 1026265, 0x1879},
			new[] {(int)SutekResourceType.DarkStone, 2406, 1112866, 0x1776},
			new[] {(int)SutekResourceType.Feathers, 0, 1023578, 0x1BD3},
			new[] {(int)SutekResourceType.FetidEssence, 0, 1031066, 0x2D92},
			new[] {(int)SutekResourceType.Gears, 0, 1024177, 0x1051},
			new[] {(int)SutekResourceType.GoldIngots, 0, 1027146, 0x1BEE},
			new[] {(int)SutekResourceType.GoldWire, 0, 1026264, 0x1878},
			new[] {(int)SutekResourceType.IronIngots, 0, 1027151, 0x1BF4},
			new[] {(int)SutekResourceType.IronWire, 0, 1026262, 0x1876},
			new[] {(int)SutekResourceType.Leather, 0, 1024216, 0x1078},
			new[] {(int)SutekResourceType.MeltedWax, 0, 1016492, 0x142B},
			new[] {(int)SutekResourceType.OilOfVitriol, 0, 1077482, 0x098D},
			new[] {(int)SutekResourceType.PowerCrystal, 0, 1112811, 0x1F1C},
			new[] {(int)SutekResourceType.PurplePotion, 0, 1023853, 0x1839},
			new[] {(int)SutekResourceType.RedPotion, 0, 1023851, 0x1838},
			new[] {(int)SutekResourceType.Rope, 0, 1020934, 0x14F8}, 
			new[] {(int)SutekResourceType.Scales, 0, 1029905, 0x26B4},
			new[] {(int)SutekResourceType.Shafts, 0, 1015158, 0x1BD6},
			new[] {(int)SutekResourceType.SilverIngots, 0, 1027158, 0x1BFA},
			new[] {(int)SutekResourceType.SilverWire, 0, 1026263, 0x1877},
			new[] {(int)SutekResourceType.SpiritEssence, 1153, 1055029, 0x2D92},
			new[] {(int)SutekResourceType.Thorns, 66, 1112813, 0x0F42},
			new[] {(int)SutekResourceType.VoidEssence, 1, 1112327, 0x2D92},
			new[] {(int)SutekResourceType.WhitePowder, 0, 1112816, 0x241D},
			new[] {(int)SutekResourceType.WhiteStone, 0, 1112813, 0x177A},
			new[] {(int)SutekResourceType.WoodenBoards, 0, 1021189, 0x1BDC},
			new[] {(int)SutekResourceType.WoodenLogs, 0, 1021217, 0x1BDF},
			new[] {(int)SutekResourceType.YellowPotion, 0, 1023852, 0x183B}
		};

		private int _LabelNumber;
		private SutekResourceType _Type;

		[CommandProperty(AccessLevel.GameMaster)]
		public SutekResourceType ResourceType
		{
			get { return _Type; }
			set
			{
				_Type = value;
				Update();
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public SutekResourceType ActualType { get { return (SutekResourceType)_Table[(int)_Type][0]; } }

		public override bool DisplayWeight { get { return false; } }

		public override int LabelNumber { get { return _LabelNumber; } }

		[Constructable]
		public SutekQuestResource()
			: this(SutekResourceType.BarrelHoops)
		{ }

		[Constructable]
		public SutekQuestResource(SutekResourceType type)
			: base(0x1DB7)
		{
			ResourceType = type;
			Movable = false;
		}

		public SutekQuestResource(Serial serial)
			: base(serial)
		{ }

		public static SutekResourceType GetRandomResource()
		{
			var vals = (int[])Enum.GetValues(typeof(SutekResourceType));
			return (SutekResourceType)vals[Utility.Random((int)SutekResourceType.YellowPotion + 1)];
		}

		public static int GetLabelId(SutekResourceType type)
		{
			return _Table[(int)type][2];
		}

		public override void GetProperties(ObjectPropertyList list)
		{ }

		public override void OnSingleClick(Mobile from)
		{
			if (LabelNumber > 0)
			{
				LabelTo(from, LabelNumber);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			writer.Write((int)_Type);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					_Type = (SutekResourceType)reader.ReadInt();
					break;
			}
		}

		private void Update()
		{
			var vals = _Table[(int)_Type];

            if (_Type == SutekResourceType.Thorns)
                Amount = 2;
            else
                Amount = 1;

			Hue = vals[1];
			_LabelNumber = vals[2];
			ItemID = vals[3];

			InvalidateProperties();
		}
	}
}