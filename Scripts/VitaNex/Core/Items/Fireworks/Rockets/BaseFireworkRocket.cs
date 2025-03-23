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
using System.Linq;

using Server;
using Server.Engines.Craft;
using Server.Items;

using VitaNex.FX;
using VitaNex.Network;
using VitaNex.SuperCrafts;
#endregion

namespace VitaNex.Items
{
	public abstract class BaseFireworkRocket : BaseFirework
	{
		#region Launch Effect
		public virtual int DefLaunchID => ItemID;
		public virtual int DefLaunchHue => Hue;
		public virtual int DefLaunchSpeed => 8;
		public virtual EffectRender DefLaunchRender => EffectRender.Normal;
		public virtual int DefLaunchSound => 551;
		public virtual int DefLaunchRangeMin => 1;
		public virtual int DefLaunchRangeMax => 3;
		public virtual int DefLaunchHeightMin => 60;
		public virtual int DefLaunchHeightMax => 80;

		[CommandProperty(AccessLevel.GameMaster)]
		public int LaunchID { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int LaunchHue { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int LaunchSpeed { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public EffectRender LaunchRender { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int LaunchSound { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int LaunchHeightMin { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int LaunchHeightMax { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int LaunchRangeMin { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int LaunchRangeMax { get; set; }
		#endregion

		#region Trail Effect
		public virtual int DefTrailID => 14120;
		public virtual int DefTrailHue => 0;
		public virtual int DefTrailSpeed => 10;
		public virtual int DefTrailDuration => 10;
		public virtual EffectRender DefTrailRender => EffectRender.SemiTransparent;
		public virtual int DefTrailSound => 856;

		[CommandProperty(AccessLevel.GameMaster)]
		public int TrailID { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int TrailHue { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int TrailSpeed { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int TrailDuration { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public EffectRender TrailRender { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int TrailSound { get; set; }
		#endregion

		#region Explode Effect
		public virtual int DefExplodeID => 14000;
		public virtual int DefExplodeHue => 0;
		public virtual int DefExplodeSpeed => 10;
		public virtual int DefExplodeDuration => 13;
		public virtual EffectRender DefExplodeRender => EffectRender.LightenMore;
		public virtual int DefExplodeSound => 776;

		[CommandProperty(AccessLevel.GameMaster)]
		public int ExplodeID { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ExplodeHue { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ExplodeSpeed { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ExplodeDuration { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public EffectRender ExplodeRender { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int ExplodeSound { get; set; }
		#endregion

		#region Stars Effect
		public virtual FireworkStars DefStarsEffect => FireworkStars.Peony;
		public virtual int DefStarsHue => 0;
		public virtual int DefStarsSound => 776;
		public virtual int DefStarsRangeMin => 5;
		public virtual int DefStarsRangeMax => 10;

		public virtual int[] DefStars => new[] { 14170, 14155, 14138, 10980, 10296, 10297, 10298, 10299, 10300, 10301 };

		public virtual int[] DefStarHues => new int[0];

		[CommandProperty(AccessLevel.GameMaster)]
		public FireworkStars StarsEffect { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int StarsHue { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int StarsSound { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int StarsRangeMin { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int StarsRangeMax { get; set; }

		public List<int> Stars { get; set; }
		public List<int> StarHues { get; set; }
		#endregion

		public BaseFireworkRocket(int itemID, int hue)
			: base(itemID, hue)
		{
			LaunchID = DefLaunchID;
			LaunchHue = DefLaunchHue;
			LaunchSpeed = DefLaunchSpeed;
			LaunchRender = DefLaunchRender;
			LaunchSound = DefLaunchSound;
			LaunchHeightMin = DefLaunchHeightMin;
			LaunchHeightMax = DefLaunchHeightMax;
			LaunchRangeMin = DefLaunchRangeMin;
			LaunchRangeMax = DefLaunchRangeMax;

			TrailID = DefTrailID;
			TrailHue = DefTrailHue;
			TrailSpeed = DefTrailSpeed;
			TrailDuration = DefTrailDuration;
			TrailRender = DefTrailRender;
			TrailSound = DefTrailSound;

			ExplodeID = DefExplodeID;
			ExplodeHue = DefExplodeHue;
			ExplodeSpeed = DefExplodeSpeed;
			ExplodeDuration = DefExplodeDuration;
			ExplodeRender = DefExplodeRender;
			ExplodeSound = DefExplodeSound;

			StarsEffect = DefStarsEffect;
			StarsHue = DefStarsHue;
			StarsSound = DefStarsSound;
			StarsRangeMin = DefStarsRangeMin;
			StarsRangeMax = DefStarsRangeMax;
			Stars = new List<int>(DefStars);
			StarHues = new List<int>(DefStarHues);
		}

		public BaseFireworkRocket(Serial serial)
			: base(serial)
		{ }

		protected virtual Point3D[] GetPath()
		{
			var start = GetWorldLocation();
			var end = start.GetRandomPoint2D(Utility.RandomMinMax(LaunchRangeMin, LaunchRangeMax));

			return start.GetLine3D(end.ToPoint3D(Utility.RandomMinMax(LaunchHeightMin, LaunchHeightMax)), Map, false);
		}

		protected override bool OnFuseBurned(Mobile m)
		{
			return Launch(m);
		}

		protected bool Launch(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			var path = GetPath();

			if (path == null || path.Length == 0)
			{
				return false;
			}

			var start = path.First();
			var end = path.Last();

			if (!OnLaunch(m, start, end))
			{
				return false;
			}

			var q = new MovingEffectQueue(() => Explode(m, end));

			for (var i = 0; i < path.Length; i++)
			{
				var p = path[i];

				if (i + 1 >= path.Length)
				{
					continue;
				}

				var pn = path[i + 1];

				q.Add(
					new MovingEffectInfo(
						p,
						pn,
						Map,
						LaunchID,
						LaunchHue,
						LaunchSpeed,
						LaunchRender,
						TimeSpan.Zero,
						() => LaunchTrail(pn)));
			}

			q.Process();

			if (LaunchSound > 0)
			{
				Effects.PlaySound(GetWorldLocation(), Map, LaunchSound);
			}

			Visible = false;
			return true;
		}

		protected virtual bool OnLaunch(Mobile m, Point3D start, Point3D end)
		{
			return true;
		}

		protected void LaunchTrail(Point3D p)
		{
			if (TrailID > 0)
			{
				var fx = new EffectInfo(p, Map, TrailID, TrailHue, TrailSpeed, TrailDuration, TrailRender);

				fx.Send();
			}

			if (TrailSound > 0)
			{
				Effects.PlaySound(p, Map, TrailSound);
			}
		}

		protected void Explode(Mobile m, Point3D p)
		{
			if (m == null || m.Deleted)
			{
				return;
			}

			if (!OnExplode(m, p))
			{
				return;
			}

			if (ExplodeID > 0)
			{
				var fx = new EffectInfo(p, Map, ExplodeID, ExplodeHue, ExplodeSpeed, ExplodeDuration, ExplodeRender);

				fx.Send();
			}

			if (ExplodeSound >= 0)
			{
				Effects.PlaySound(p, Map, ExplodeSound);
			}

			ExplodeStars(
				m,
				StarsEffect,
				p,
				Utility.RandomMinMax(StarsRangeMin, StarsRangeMax),
				StarsSound,
				Stars.ToArray(),
				StarsHue > 0 ? new[] { StarsHue } : StarHues.ToArray());

			Movable = false;

			Timer.DelayCall(
				TimeSpan.FromSeconds(1.0),
				() =>
				{
					Movable = true;
					Consume();
				});
		}

		protected virtual bool OnExplode(Mobile m, Point3D p)
		{
			return true;
		}

		protected virtual void ExplodeStars(
			Mobile m,
			FireworkStars fx,
			Point3D p,
			int radius,
			int sound,
			int[] stars,
			int[] hues)
		{
			if (m != null && !m.Deleted && fx != FireworkStars.None && stars != null && stars.Length != 0 && !Deleted)
			{
				fx.DoStarsEffect(p, Map, radius, sound, stars, hues);
			}
		}

		public override void OnDelete()
		{
			base.OnDelete();

			Stars.Clear();
			Stars.TrimExcess();

			StarHues.Clear();
			StarHues.TrimExcess();
		}

		public override int OnCraft(
			int quality,
			bool makersMark,
			Mobile m,
			CraftSystem craftSystem,
			Type typeRes,
			ITool tool,
			CraftItem craftItem,
			int resHue)
		{
			if (craftSystem is Pyrotechnics && craftItem != null)
			{
				var stars = new CraftRes[craftItem.Resources.Count];

				stars.SetAll(i => craftItem.Resources.GetAt(i));
				stars = stars.Where(res => res.ItemType.IsEqualOrChildOf<BaseFireworkStar>()).ToArray();

				if (stars.Length > 0)
				{
					StarsRangeMin = 1 + stars.Sum(s => s.Amount);
					StarsRangeMax = StarsRangeMin * 2;

					LaunchHeightMin = Math.Max(50, Math.Min(100, StarsRangeMin * 5));
					LaunchHeightMax = Math.Max(50, Math.Min(100, StarsRangeMax * 5));

					StarHues.AddRange(BaseFireworkStar.GetEffectHues(typeRes ?? stars[0].ItemType));

					if (stars.Length > 1)
					{
						StarHues.AddRange(stars.Skip(1).SelectMany(s => BaseFireworkStar.GetEffectHues(s.ItemType)));
					}
				}
			}

			return base.OnCraft(quality, makersMark, m, craftSystem, typeRes, tool, craftItem, resHue);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(LaunchID);
					writer.Write(LaunchHue);
					writer.Write(LaunchSpeed);
					writer.WriteFlag(LaunchRender);
					writer.Write(LaunchSound);
					writer.Write(LaunchRangeMin);
					writer.Write(LaunchRangeMax);
					writer.Write(LaunchHeightMin);
					writer.Write(LaunchHeightMax);

					writer.Write(TrailID);
					writer.Write(TrailHue);
					writer.Write(TrailSpeed);
					writer.Write(TrailDuration);
					writer.WriteFlag(TrailRender);
					writer.Write(TrailSound);

					writer.Write(ExplodeID);
					writer.Write(ExplodeHue);
					writer.Write(ExplodeSpeed);
					writer.Write(ExplodeDuration);
					writer.WriteFlag(ExplodeRender);
					writer.Write(ExplodeSound);

					writer.WriteFlag(StarsEffect);
					writer.Write(StarsHue);
					writer.Write(StarsSound);
					writer.Write(StarsRangeMin);
					writer.Write(StarsRangeMax);
					writer.WriteList(Stars, writer.Write);
					writer.WriteList(StarHues, writer.Write);
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
				case 0:
				{
					LaunchID = reader.ReadInt();
					LaunchHue = reader.ReadInt();
					LaunchSpeed = reader.ReadInt();
					LaunchRender = reader.ReadFlag<EffectRender>();
					LaunchSound = reader.ReadInt();
					LaunchRangeMin = reader.ReadInt();
					LaunchRangeMax = reader.ReadInt();
					LaunchHeightMin = reader.ReadInt();
					LaunchHeightMax = reader.ReadInt();

					TrailID = reader.ReadInt();
					TrailHue = reader.ReadInt();
					TrailSpeed = reader.ReadInt();
					TrailDuration = reader.ReadInt();
					TrailRender = reader.ReadFlag<EffectRender>();
					TrailSound = reader.ReadInt();

					ExplodeID = reader.ReadInt();
					ExplodeHue = reader.ReadInt();
					ExplodeSpeed = reader.ReadInt();
					ExplodeDuration = reader.ReadInt();
					ExplodeRender = reader.ReadFlag<EffectRender>();
					ExplodeSound = reader.ReadInt();

					StarsEffect = reader.ReadFlag<FireworkStars>();
					StarsHue = reader.ReadInt();
					StarsSound = reader.ReadInt();
					StarsRangeMin = reader.ReadInt();
					StarsRangeMax = reader.ReadInt();
					Stars = reader.ReadList(reader.ReadInt);
					StarHues = reader.ReadList(reader.ReadInt);
				}
				break;
			}
		}
	}
}
