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
using System.Collections.Generic;

using Server;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPBattleLocations : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual int BattlePriority { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual Map Map { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual Point3D SpectateJoin { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual MapPoint Eject { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual MapPoint SpectateGate { get; set; }

		public virtual List<Rectangle3D> BattleBounds { get; set; }
		public virtual List<Rectangle3D> SpectateBounds { get; set; }

		public Point3D BattleFixedPoint => GetBattleFixedPoint();
		public Point3D SpectateFixedPoint => GetSpectateFixedPoint();

		public PvPBattleLocations()
		{
			BattlePriority = Region.DefaultPriority;

			Map = Map.Internal;
			Eject = MapPoint.Empty;

			SpectateGate = MapPoint.Empty;
			SpectateJoin = Point3D.Zero;

			BattleBounds = new List<Rectangle3D>();
			SpectateBounds = new List<Rectangle3D>();
		}

		public PvPBattleLocations(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Battle Locations";
		}

		public override void Clear()
		{
			Map = Map.Internal;
			BattlePriority = Region.DefaultPriority;
			BattleBounds.Free(true);
			SpectateBounds.Free(true);
			SpectateJoin = Point3D.Zero;
			Eject = MapPoint.Empty;
		}

		public override void Reset()
		{
			Map = Map.Internal;
			BattlePriority = Region.DefaultPriority;
			BattleBounds.Free(true);
			SpectateBounds.Free(true);
			SpectateJoin = Point3D.Zero;
			Eject = MapPoint.Empty;
		}

		public Point3D GetBattleFixedPoint()
		{
			if (BattleBounds == null || BattleBounds.Count == 0)
			{
				return Point3D.Zero;
			}

			var p = BattleBounds[0].Start;

			return p.ToPoint3D(Map.GetAverageZ(p.X, p.Y));
		}

		public Point3D GetSpectateFixedPoint()
		{
			if (SpectateBounds == null || SpectateBounds.Count == 0)
			{
				return Point3D.Zero;
			}

			var p = SpectateBounds[0].Start;

			return p.ToPoint3D(Map.GetAverageZ(p.X, p.Y));
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					SpectateGate.Serialize(writer);
					goto case 0;
				case 0:
				{
					writer.Write(Map);
					writer.Write(BattlePriority);

					Eject.Serialize(writer);

					writer.Write(SpectateJoin);

					writer.WriteBlockList(BattleBounds, (w, b) => w.Write(b));
					writer.WriteBlockList(SpectateBounds, (w, b) => w.Write(b));
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 1:
					SpectateGate = new MapPoint(reader);
					goto case 0;
				case 0:
				{
					Map = reader.ReadMap();
					BattlePriority = reader.ReadInt();

					Eject = new MapPoint(reader);

					SpectateJoin = reader.ReadPoint3D();

					BattleBounds = reader.ReadBlockList(r => r.ReadRect3D());
					SpectateBounds = reader.ReadBlockList(r => r.ReadRect3D());
				}
				break;
			}
		}
	}
}