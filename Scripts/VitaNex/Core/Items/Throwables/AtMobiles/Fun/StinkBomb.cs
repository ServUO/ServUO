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
using Server.Items;
using Server.Targeting;

using VitaNex.FX;
#endregion

namespace VitaNex.Items
{
	[Flipable(10248, 10249)]
	public class ThrowableStinkBomb : BaseThrowableAtMobile<Mobile>
	{
		private static readonly PollTimer _InternalTimer;

		public static Dictionary<Mobile, DateTime> Stinky { get; private set; }

		static ThrowableStinkBomb()
		{
			Stinky = new Dictionary<Mobile, DateTime>();

			_InternalTimer = PollTimer.FromSeconds(5.0, InternalCallback, () => Stinky.Count > 0);
		}

		private static void InternalCallback()
		{
			Stinky.RemoveKeyRange(m => !CheckStinky(m));

			var now = DateTime.UtcNow;

			var e = Enumerable.Empty<Tuple<Mobile, TimeSpan>>();
			e = Stinky.Aggregate(e, (c, kv) => c.With(DoStinkEffect(kv.Key).Select(t => Tuple.Create(t, kv.Value - now))));

			foreach (var t in e.Where(t => t.Item2.TotalSeconds > 0))
			{
				MakeStinky(t.Item1, t.Item2);
			}
		}

		public static bool CheckStinky(Mobile m)
		{
			return Stinky.ContainsKey(m) && Stinky[m] >= DateTime.UtcNow;
		}

		public static void MakeStinky(Mobile m, TimeSpan duration)
		{
			Stinky[m] = DateTime.UtcNow + duration;
		}

		public static IEnumerable<Mobile> DoStinkEffect(Mobile m)
		{
			if (!CheckStinky(m) || m.Hidden || !m.Alive)
			{
				return Enumerable.Empty<Mobile>();
			}

			Effects.PlaySound(m.Location, m.Map, 1064);

			new PoisonExplodeEffect(m, m.Map, 1)
			{
				EffectMutator = e =>
				{
					if (e.ProcessIndex == 0)
					{
						e.SoundID = 1064;
					}
				},
				EffectHandler = e =>
				{
					if (e.ProcessIndex != 0)
					{
						return;
					}

					foreach (var t in e.Source.FindMobilesInRange(e.Map, 0)
									   .Where(t => t != null && !t.Deleted && t != m && !t.Hidden && t.Alive && t.Body.IsHuman))
					{
						Effects.PlaySound(t.Location, t.Map, Utility.RandomList(1065, 1066, 1067));
					}
				}
			}.Send();

			return m.FindMobilesInRange(m.Map, 1)
					.Where(t => t != null && !t.Deleted && t != m && !t.Hidden && t.Alive && t.Body.IsHuman);
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int ExplosionRange { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan StinkyDuration { get; set; }

		[Constructable]
		public ThrowableStinkBomb()
			: this(1)
		{ }

		[Constructable]
		public ThrowableStinkBomb(int amount)
			: base(Utility.RandomList(10248, 10249), amount)
		{
			Name = "Stink Bomb";
			Usage = "Throw To Unleash A Terrible Smell!";
			Hue = 1270;
			Weight = 1.0;
			Stackable = true;

			StinkyDuration = TimeSpan.FromSeconds(30.0);

			TargetFlags = TargetFlags.None;
			Delivery = ThrowableAtMobileDelivery.None;

			ThrowSound = 1491;
			ImpactSound = 1064;

			EffectID = ItemID;
			EffectHue = Hue;

			ThrowRecovery = TimeSpan.FromSeconds(60.0);

			ExplosionRange = 5;

			RequiredSkill = SkillName.Alchemy;
			RequiredSkillValue = 25.0;
		}

		public ThrowableStinkBomb(Serial serial)
			: base(serial)
		{ }

		protected override void OnThrownAt(Mobile from, Mobile target)
		{
			if (from == null || from.Deleted || target == null)
			{
				return;
			}

			for (int layer = 0, range = ExplosionRange; layer < ExplosionRange && range >= 0; layer++, range--)
			{
				new PoisonExplodeEffect(target.Clone3D(0, 0, layer * 10), target.Map, range, 0, null, ExplosionStink).Send();
			}

			base.OnThrownAt(from, target);
		}

		public virtual void ExplosionStink(EffectInfo info)
		{
			Effects.PlaySound(info.Source.Location, info.Map, ImpactSound);

			foreach (var m in info.Source.FindMobilesInRange(info.Map, 0)
								  .Not(t => t == null || t.Deleted || t.Hidden || (!t.Alive && !AllowDeadTarget)))
			{
				Effects.PlaySound(m.Location, m.Map, Utility.RandomList(1065, 1066, 1067));

				if (StinkyDuration > TimeSpan.Zero)
				{
					MakeStinky(m, StinkyDuration);
				}
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(ExplosionRange);
					writer.Write(StinkyDuration);
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
					ExplosionRange = reader.ReadInt();
					StinkyDuration = reader.ReadTimeSpan();
				}
				break;
			}

			if (StinkyDuration <= TimeSpan.Zero)
			{
				StinkyDuration = TimeSpan.FromSeconds(30);
			}
		}
	}
}