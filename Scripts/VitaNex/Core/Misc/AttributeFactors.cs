#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;

using VitaNex;
#endregion

namespace Server
{
	[PropertyObject]
	public class AttributeFactors
	{
		[CommandProperty(AccessLevel.Administrator)]
		public double Weight { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public int Min { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public int Max { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public int Inc { get; set; }

		public AttributeFactors(double weight = 1.0, int min = 0, int max = 1, int inc = 1)
		{
			Weight = weight;
			Min = min;
			Max = max;
			Inc = inc;
		}

		public AttributeFactors(GenericReader reader)
		{
			Deserialize(reader);
		}

		public double GetIntensity(int value)
		{
			value = Math.Max(Min, Math.Min(Max, value));

			if (value > 0)
				return value / Math.Max(1.0, Max);

			if (value < 0)
				return value / Math.Min(-1.0, Min);

			return 0;
		}

		public double GetWeight(int value)
		{
			return GetIntensity(value) * Weight;
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				{
					writer.Write(Min);
				}
				goto case 0;
				case 0:
				{
					writer.Write(Weight);
					writer.Write(Max);
					writer.Write(Inc);
				}
				break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.ReadInt();

			switch (version)
			{
				case 1:
				{
					Min = reader.ReadInt();
				}
				goto case 0;
				case 0:
				{
					Weight = reader.ReadDouble();
					Max = reader.ReadInt();
					Inc = reader.ReadInt();
				}
				break;
			}
		}
	}
}
