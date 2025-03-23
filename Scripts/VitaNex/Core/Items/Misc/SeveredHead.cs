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
using Server.Items;

using VitaNex.FX;
using VitaNex.Network;
#endregion

namespace VitaNex.Items
{
	public delegate bool DecapitateHandler(Mobile from, Mobile target, Func<Mobile, Item> createHead = null);

	public class SeveredHead : Item
	{
		public static DecapitateHandler Decapitate = HandleDecapitate;

		public static bool HandleDecapitate(Mobile from, Mobile target, Func<Mobile, Item> createHead = null)
		{
			if (from == null || target == null)
			{
				return false;
			}

			var map = target.Map;

			target.Freeze(TimeSpan.FromSeconds(1.0));

			var range = Utility.RandomMinMax(5, 7);
			var zOffset = target.Mounted ? 20 : 10;

			var src = target.Location.Clone3D(0, 0, zOffset);
			var points = src.GetAllPointsInRange(map, range, range);

			Effects.PlaySound(target.Location, map, 0x19C);

			target.Send(VNScreenLightFlash.Instance);

			Timer.DelayCall(
				TimeSpan.FromMilliseconds(100),
				() =>
				{
					foreach (var trg in points)
					{
						var bloodID = Utility.RandomMinMax(4650, 4655);

						new MovingEffectInfo(src, trg.Clone3D(0, 0, 2), from.Map, bloodID).MovingImpact(
							info =>
							{
								new Blood(bloodID).MoveToWorld(info.Target.Location, info.Map);
								Effects.PlaySound(info.Target, info.Map, 0x028);
							});
					}
				});

			target.Damage(target.HitsMax, from);

			Timer.DelayCall(
				TimeSpan.FromMilliseconds(100),
				() =>
				{
					var corpse = target.Corpse as Corpse;

					if (corpse != null && !corpse.Deleted)
					{
						corpse.TurnToBones();
					}
				});

			var head = createHead != null ? createHead(target) : null;

			int headID;
			int headHue;

			if (head != null)
			{
				headID = head.ItemID;
				headHue = head.Hue;
			}
			else
			{
				headID = 7393;
				headHue = target.SolidHueOverride >= 0 ? target.SolidHueOverride : target.Hue;
			}

			new MovingEffectInfo(src, src.Clone3D(0, 0, 40), map, headID, headHue).MovingImpact(
				info => new MovingEffectInfo(
					info.Target,
					info.Source.Clone3D(Utility.RandomMinMax(-1, 1), Utility.RandomMinMax(-1, 1), 2),
					info.Map,
					headID,
					headHue).MovingImpact(
					hInfo =>
					{
						if (head != null && !head.Deleted)
						{
							head.MoveToWorld(hInfo.Target.Location, info.Map);
						}
					}));

			return true;
		}

		public override string DefaultName => "a severed head";

		[Constructable]
		public SeveredHead()
			: this(String.Empty)
		{ }

		[Constructable]
		public SeveredHead(Mobile m)
			: this(m == null || String.IsNullOrWhiteSpace(m.RawName) ? String.Empty : "the severed head of " + m.RawName)
		{ }

		[Constructable]
		public SeveredHead(string name)
			: base(7393)
		{
			Name = !String.IsNullOrWhiteSpace(name) ? name : DefaultName;
		}

		public SeveredHead(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}