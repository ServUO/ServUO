using System;
using System.Linq;

namespace Server.Misc
{
    public partial class RaceDefinitions
	{
        private class Gargoyle : Race
		{
			public override string Name { get; set; } = "Gargoyle";
			public override string PluralName { get; set; } = "Gargoyles";

			public override int MaleBody { get; set; } = 666;
			public override int FemaleBody { get; set; } = 667;

			public override int MaleGhostBody { get; set; } = 695;
			public override int FemaleGhostBody { get; set; } = 694;

			public override Expansion RequiredExpansion { get; set; } = Expansion.SA;

			public override int[] ExclusiveEquipment { get; set; } =
			{
				0x283, 0x284, 0x285, 0x286, 0x287, 0x288, 0x289, 0x28A, // Armor
				0x301, 0x302, 0x303, 0x304, 0x305, 0x306, 0x310, 0x311,
				0x307, 0x308, 0x309, 0x30A, 0x30B, 0x30C, 0x30D, 0x30E,
				0x403, 0x404, 0x405, 0x406, 0x407, 0x408, 0x409, 0x40A,

				0x41D8, 0x41D9, 0x42DE, 0x42DF,                         // Talons

				0x08FD, 0x08FE, 0x08FF, 0x0900, 0x0901, 0x0902, 0x0903, // Weapons
				0x0904, 0x0905, 0x0906, 0x0907, 0x0908, 0x0909, 0x090A,
				0x090B, 0x090C, 0x48AE, 0x48AF, 0x48B0, 0x48B1, 0x48B2,
				0x48B3, 0x48B4, 0x48B5, 0x48B6, 0x48B7, 0x48B8, 0x48B9,
				0x48BA, 0x48BB, 0x48BC, 0x48BD, 0x48BE, 0x48BF, 0x48C0,
				0x48C1, 0x48C2, 0x48C3, 0x48C4, 0x48C5, 0x48C6, 0x48C7,
				0x48C8, 0x48C9, 0x48CA, 0x48CB, 0x48CC, 0x48CD, 0x48CE,
				0x48CF, 0x48D0, 0x48D1, 0x48D2, 0x48D3,

				0x4000, 0x4001, 0x4002, 0x4003,                         // Robes
				0x9986,                                                 // Epaulets

				0x4047, 0x4048, 0x4049, 0x404A, 0x404B, 0x404C, 0x404D, // Armor/Weapons
				0x404E, 0x404F, 0x4050, 0x4051, 0x4052, 0x4053, 0x4054,
				0x4055, 0x4056, 0x4057, 0x4058, 0x4058, 0x4059, 0x405A,
				0x405B, 0x405C, 0x405D, 0x405E, 0x405F, 0x4060, 0x4061,
				0x4062, 0x4063, 0x4064, 0x4065, 0x4066, 0x4067, 0x4068,
				0x4068, 0x4069, 0x406A, 0x406B, 0x406C, 0x406D, 0x406E,
				0x406F, 0x4070, 0x4071, 0x4072, 0x4072, 0x7073, 0x4074,
				0x4075, 0x4076,

				0x4200, 0x4201, 0x4202, 0x4203, 0x4204, 0x4205, 0x4206, // Shields
				0x4207, 0x4208, 0x4209, 0x420A, 0x420B, 0x4228, 0x4229,
				0x422A, 0x422C,

				0x4210, 0x4211, 0x4212, 0x4213, 0x4D0A, 0x4D0B,         // Jewelry

				0x450D, 0x450E, 0x50D8,                                 // Belts/Half Apron
				0x457E, 0x457F, 0x45A4, 0x45A5,                         // Wing Armor
				0x45B1, 0x45B3, 0x4644, 0x4645,                         // Glasses
				0x46AA, 0x46AB, 0x46B4, 0x46B5,                         // Sashes

				0xA1C9, 0xA1CA                                          // Special
			};

			public override int[][] HairTable { get; set; } =
			{
				new[] // Female
				{
					Hair.Gargoyle.Bald,
					Hair.Gargoyle.FemaleHorns1,
					Hair.Gargoyle.FemaleHorns2,
					Hair.Gargoyle.FemaleHorns3,
					Hair.Gargoyle.FemaleHorns4,
					Hair.Gargoyle.FemaleHorns5,
					Hair.Gargoyle.FemaleHorns6,
					Hair.Gargoyle.FemaleHorns7,
					Hair.Gargoyle.FemaleHorns8,
					Hair.Gargoyle.FemaleHorns9,
				},
				new[] // Male
				{
					Hair.Gargoyle.Bald,
					Hair.Gargoyle.MaleHorns1,
					Hair.Gargoyle.MaleHorns2,
					Hair.Gargoyle.MaleHorns3,
					Hair.Gargoyle.MaleHorns4,
					Hair.Gargoyle.MaleHorns5,
					Hair.Gargoyle.MaleHorns6,
					Hair.Gargoyle.MaleHorns7,
					Hair.Gargoyle.MaleHorns8,
				}
			};

			public override int[][] BeardTable { get; set; } =
			{
				new[] // Female
				{
					Beard.Gargoyle.Clean,
				},
				new[] // Male
				{
					Beard.Gargoyle.Clean,
					Beard.Gargoyle.Horns1,
					Beard.Gargoyle.Horns2,
					Beard.Gargoyle.Horns3,
					Beard.Gargoyle.Horns4,
				}
			};

			public override int[][] FaceTable { get; set; } =
			{
				new[] // Female
				{
					Face.Gargoyle.None,
					Face.Gargoyle.Face1,
					Face.Gargoyle.Face2,
					Face.Gargoyle.Face3,
					Face.Gargoyle.Face4,
					Face.Gargoyle.Face5,
				},
				new[] // Male
				{
					Face.Gargoyle.None,
					Face.Gargoyle.Face1,
					Face.Gargoyle.Face2,
					Face.Gargoyle.Face3,
					Face.Gargoyle.Face4,
					Face.Gargoyle.Face5,
				}
			};

			public override int[] SkinHues { get; set; } = Enumerable.Range(1755, 25).ToArray();

			public override int[] HairHues { get; set; } =
			{
				0x709, 0x70B, 0x70D, 0x70F, 0x711, 0x763,
				0x765, 0x768, 0x76B, 0x6F3, 0x6F1, 0x6EF,
				0x6E4, 0x6E2, 0x6E0
			};

			public Gargoyle(int raceID, int raceIndex)
				: base(raceID, raceIndex)
			{ }
		}
	}
}