using System;

namespace Server.Misc
{
    public partial class RaceDefinitions
	{
        private class Elf : Race
		{
			public override string Name { get; set; } = "Elf";
			public override string PluralName { get; set; } = "Elves";

			public override int MaleBody { get; set; } = 605;
			public override int FemaleBody { get; set; } = 606;

			public override int MaleGhostBody { get; set; } = 607;
			public override int FemaleGhostBody { get; set; } = 608;

			public override Expansion RequiredExpansion { get; set; } = Expansion.ML;

			public override int[] ExclusiveEquipment { get; set; } =
			{
				0x2B67, 0x2B68, 0x2B69, 0x2B6A, 0x2B6B, 0x2B6C, 0x2B6D, // Armor
				0x2B6E, 0x2B6F, 0x2B70, 0x2B71, 0x2B72, 0x2B73, 0x2B74,
				0x2B75, 0x2B76, 0x2B77, 0x2B78, 0x2B79, 0x3160, 0x3161,
				0x3162, 0x3163, 0x3164, 0x3165, 0x3166, 0x3167, 0x3168,
				0x3169, 0x316A, 0x316B, 0x316C, 0x316D, 0x316E, 0x316F,
				0x3170, 0x3171, 0x3172, 0x3173, 0x3174, 0x3175, 0x3176,
				0x3177, 0x3178, 0x3179, 0x317A, 0x317B, 0x317C, 0x317D,
				0x317E, 0x317F, 0x3180, 0x3181,

				0x2FB9, 0x2FBA,                                         // Robes

				0x2FC3, 0x2FC4, 0x2FC5, 0x2FC6, 0x2FC7, 0x2FC8, 0x2FC9, // Clothes
				0x2FCA, 0x2FCB,

				0x315F,                                                 // Belt
			};

			public override int[][] HairTable { get; set; } =
			{
				new[] // Female
				{
					Hair.Elf.Bald,
					Hair.Elf.LongFeather,
					Hair.Elf.Short,
					Hair.Elf.Mullet,
					Hair.Elf.Knob,
					Hair.Elf.Braided,
					Hair.Elf.Spiked,
					Hair.Elf.Flower,
					Hair.Elf.Bun,
				},
				new[] // Male
				{
					Hair.Elf.Bald,
					Hair.Elf.LongFeather,
					Hair.Elf.Short,
					Hair.Elf.Mullet,
					Hair.Elf.Knob,
					Hair.Elf.Braided,
					Hair.Elf.Spiked,
					Hair.Elf.MidLong,
					Hair.Elf.Long,
				}
			};

			public override int[][] BeardTable { get; set; } =
			{
				new[] // Female
				{
					Beard.Elf.Clean,
				},
				new[] // Male
				{
					Beard.Elf.Clean,
				}
			};

			public override int[][] FaceTable { get; set; } =
			{
				new[] // Female
				{
					Face.Elf.None,
					Face.Elf.Face1,
					Face.Elf.Face2,
					Face.Elf.Face3,
					Face.Elf.Face4,
					Face.Elf.Face5,
					Face.Elf.Face6,
					Face.Elf.Face7,
					Face.Elf.Face8,
					Face.Elf.Face9,
					Face.Elf.Face10,
				},
				new[] // Male
				{
					Face.Elf.None,
					Face.Elf.Face1,
					Face.Elf.Face2,
					Face.Elf.Face3,
					Face.Elf.Face4,
					Face.Elf.Face5,
					Face.Elf.Face6,
					Face.Elf.Face7,
					Face.Elf.Face8,
					Face.Elf.Face9,
					Face.Elf.Face10,
				}
			};

			public override int[] SkinHues { get; set; } =
			{
				0x4DE, 0x76C, 0x835, 0x430, 0x24D, 0x24E, 0x24F, 0x0BF,
				0x4A7, 0x361, 0x375, 0x367, 0x3E8, 0x3DE, 0x353, 0x903,
				0x76D, 0x384, 0x579, 0x3E9, 0x374, 0x389, 0x385, 0x376,
				0x53F, 0x381, 0x382, 0x383, 0x76B, 0x3E5, 0x51D, 0x3E6
			};

			public override int[] HairHues { get; set; } =
			{
				0x034, 0x035, 0x036, 0x037, 0x038, 0x039, 0x058, 0x08E,
				0x08F, 0x090, 0x091, 0x092, 0x101, 0x159, 0x15A, 0x15B,
				0x15C, 0x15D, 0x15E, 0x128, 0x12F, 0x1BD, 0x1E4, 0x1F3,
				0x207, 0x211, 0x239, 0x251, 0x26C, 0x2C3, 0x2C9, 0x31D,
				0x31E, 0x31F, 0x320, 0x321, 0x322, 0x323, 0x324, 0x325,
				0x326, 0x369, 0x386, 0x387, 0x388, 0x389, 0x38A, 0x59D,
				0x6B8, 0x725, 0x853
			};

			public Elf(int raceID, int raceIndex)
				: base(raceID, raceIndex)
			{ }
		}
	}
}