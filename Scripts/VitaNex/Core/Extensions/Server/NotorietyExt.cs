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
using System.Drawing;
#endregion

namespace Server
{
	public enum NotorietyType
	{
		None = 0,
		Innocent = Notoriety.Innocent,
		Ally = Notoriety.Ally,
		CanBeAttacked = Notoriety.CanBeAttacked,
		Criminal = Notoriety.Criminal,
		Enemy = Notoriety.Enemy,
		Murderer = Notoriety.Murderer,
		Invulnerable = Notoriety.Invulnerable
	}

	public static class NotorietyExtUtility
	{
		public static int GetHue(this NotorietyType noto)
		{
			return Notoriety.GetHue((int)noto);
		}

		public static Color GetColor(this NotorietyType noto)
		{
			switch ((int)noto)
			{
				case Notoriety.Innocent:
					return Color.SkyBlue;
				case Notoriety.Ally:
					return Color.LawnGreen;
				case Notoriety.CanBeAttacked:
				case Notoriety.Criminal:
					return Color.Silver;
				case Notoriety.Enemy:
					return Color.Orange;
				case Notoriety.Murderer:
					return Color.IndianRed;
				case Notoriety.Invulnerable:
					return Color.Yellow;
			}

			return Color.White;
		}
	}
}