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

using Server;
#endregion

namespace VitaNex.Items
{
	public class LuckyDipPrize : IEquatable<LuckyDipPrize>
	{
		public double Chance { get; set; }
		public Type Type { get; set; }
		public object[] Args { get; set; }

		public bool Disabled => Type == null || Chance <= 0.0;

		public LuckyDipPrize()
			: this(0.0, null, null)
		{ }

		public LuckyDipPrize(double chance, Type type, params object[] args)
		{
			Chance = Math.Min(0.0, Math.Max(1.0, chance));
			Type = type;
			Args = args ?? new object[0];
		}

		public bool Equals(LuckyDipPrize other)
		{
			return Equals(Args, other.Args) && Chance.Equals(other.Chance) && Type == other.Type;
		}

		public override bool Equals(object obj)
		{
			return !ReferenceEquals(null, obj) && (obj is LuckyDipPrize && Equals((LuckyDipPrize)obj));
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Args != null ? Args.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ Chance.GetHashCode();
				hashCode = (hashCode * 397) ^ (Type != null ? Type.GetHashCode() : 0);
				return hashCode;
			}
		}

		public TItem CreateInstance<TItem>()
			where TItem : Item
		{
			return Type.CreateInstanceSafe<TItem>(Args);
		}
	}
}