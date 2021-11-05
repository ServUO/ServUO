using System;
using System.Linq;

namespace Server.Misc
{
    public partial class RaceDefinitions
	{
		private class Human : Race
		{
			public override string Name { get; set; } = "Human";
			public override string PluralName { get; set; } = "Humans";

			public override int MaleBody { get; set; } = 400;
			public override int FemaleBody { get; set; } = 401;

			public override int MaleGhostBody { get; set; } = 402;
			public override int FemaleGhostBody { get; set; } = 403;

			public override Expansion RequiredExpansion { get; set; } = Expansion.None;

			public override int[] ExclusiveEquipment { get; set; } = 
			{
			};

			public override int[][] HairTable { get; set; } =
			{
				new[] // Female
				{
					Hair.Human.Bald,
					Hair.Human.Short,
					Hair.Human.Long,
					Hair.Human.PonyTail,
					Hair.Human.Mohawk,
					Hair.Human.Pageboy,
					Hair.Human.Buns,
					Hair.Human.Afro,
					Hair.Human.PigTails,
					Hair.Human.Krisna,
				},
				new[] // Male
				{
					Hair.Human.Bald,
					Hair.Human.Short,
					Hair.Human.Long,
					Hair.Human.PonyTail,
					Hair.Human.Mohawk,
					Hair.Human.Pageboy,
					Hair.Human.Buns,
					Hair.Human.Afro,
					Hair.Human.Receeding,
					Hair.Human.Krisna,
				}
			};

			public override int[][] BeardTable { get; set; } =
			{
				new[] // Female
				{
					Beard.Human.Clean,
				},
				new[] // Male
				{
					Beard.Human.Clean,
					Beard.Human.Long,
					Beard.Human.Short,
					Beard.Human.Goatee,
					Beard.Human.Mustache,
					Beard.Human.MidShort,
					Beard.Human.MidLong,
					Beard.Human.Vandyke,
				}
			};

			public override int[][] FaceTable { get; set; } =
			{
				new[] // Female
				{
					Face.Human.None,
					Face.Human.Face1,
					Face.Human.Face2,
					Face.Human.Face3,
					Face.Human.Face4,
					Face.Human.Face5,
					Face.Human.Face6,
					Face.Human.Face7,
					Face.Human.Face8,
					Face.Human.Face9,
					Face.Human.Face10,
				},
				new[] // Male
				{
					Face.Human.None,
					Face.Human.Face1,
					Face.Human.Face2,
					Face.Human.Face3,
					Face.Human.Face4,
					Face.Human.Face5,
					Face.Human.Face6,
					Face.Human.Face7,
					Face.Human.Face8,
					Face.Human.Face9,
					Face.Human.Face10,
				}
			};

			public override int[] SkinHues { get; set; } = Enumerable.Range(1002, 57).ToArray();
			public override int[] HairHues { get; set; } = Enumerable.Range(1102, 48).ToArray();

			public Human(int raceID, int raceIndex)
				: base(raceID, raceIndex)
			{ }
		}
	}
}