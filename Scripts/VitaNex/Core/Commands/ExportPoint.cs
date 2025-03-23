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
using System.IO;

using Server;
using Server.Mobiles;

using VitaNex.IO;
using VitaNex.Targets;
#endregion

namespace VitaNex.Commands
{
	public static class ExportPointCommand
	{
		public static void Initialize()
		{
			CommandUtility.Register(
				"ExportPoint2D",
				AccessLevel.GameMaster,
				e =>
				{
					if (e != null && e.Mobile != null)
					{
						OnExportPoint2D(e.Mobile, e.GetString(0), e.GetString(1));
					}
				});
			CommandUtility.RegisterAlias("ExportPoint2D", "XP2D");

			CommandUtility.Register(
				"ExportPoint3D",
				AccessLevel.GameMaster,
				e =>
				{
					if (e != null && e.Mobile != null)
					{
						OnExportPoint3D(e.Mobile, e.GetString(0), e.GetString(1));
					}
				});
			CommandUtility.RegisterAlias("ExportPoint3D", "XP3D");
		}

		public static void OnExportPoint2D(Mobile m, string speech, string comment)
		{
			if (m == null || m.Deleted || !(m is PlayerMobile))
			{
				return;
			}

			if (String.IsNullOrWhiteSpace(speech))
			{
				speech = "Points";
			}

			GenericSelectTarget<IPoint2D>.Begin(
				m,
				(from, p) =>
					IOUtility
						.EnsureFile(VitaNexCore.DataDirectory + "/Export/Points/2D/" + IOUtility.GetSafeFileName(speech) + ".txt")
						.AppendText(false, String.Format("new Point2D({0}, {1}), //{2}", p.X, p.Y, comment ?? String.Empty)),
				null,
				-1,
				true);
		}

		public static void OnExportPoint3D(Mobile m, string speech, string comment)
		{
			if (m == null || m.Deleted || !(m is PlayerMobile))
			{
				return;
			}

			if (String.IsNullOrWhiteSpace(speech))
			{
				speech = "Points";
			}

			GenericSelectTarget<IPoint3D>.Begin(
				m,
				(from, p) =>
					IOUtility
						.EnsureFile(VitaNexCore.DataDirectory + "/Export/Points/3D/" + IOUtility.GetSafeFileName(speech) + ".txt")
						.AppendText(false, String.Format("new Point3D({0}, {1}, {2}), //{3}", p.X, p.Y, p.Z, comment ?? String.Empty)),
				null,
				-1,
				true);
		}
	}
}