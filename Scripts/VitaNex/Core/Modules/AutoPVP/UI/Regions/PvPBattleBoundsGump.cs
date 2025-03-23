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
using Server.Gumps;

using VitaNex.Network;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPBattleBoundsGump : Rect3DListGump
	{
		private RegionExtUtility.PreviewRegion _SpectatePreview;

		public PvPBattle Battle { get; set; }

		public PvPBattleBoundsGump(Mobile user, PvPBattle battle, Gump parent = null)
			: base(
				user,
				parent,
				list: battle.Options.Locations.BattleBounds,
				emptyText: "There are no bounds in the list.",
				title: "Battle Region Bounds")
		{
			Battle = battle;

			InputMap = Battle.Map;

			Preview = true;
			PreviewHue = TextHue;
			PreviewEffect = 3259;
			PreviewName = String.Format("Battle Region Preview: {0} ({1})", Battle.Name, Battle.Serial);

			ForceRecompile = true;
		}

		public override List<Rectangle3D> GetExternalList()
		{
			return Battle.Options.Locations.BattleBounds;
		}

		protected override bool OnBeforeListAdd()
		{
			if (!base.OnBeforeListAdd())
			{
				return false;
			}

			if (InputRect != null && InputRect.Value.ToRectangle2D()
											  .EnumeratePoints()
											  .Any(Battle.Options.Locations.SpectateBounds.Contains))
			{
				User.SendMessage(ErrorHue, "Bounds can not overlap Spectate region.");
				return false;
			}

			return true;
		}

		protected override void HandleApplyChanges()
		{
			base.HandleApplyChanges();

			Battle.Map = InputMap;
			Battle.InvalidateBattleRegion();
		}

		public override void ClearPreview()
		{
			base.ClearPreview();

			if (_SpectatePreview == null)
			{
				return;
			}

			_SpectatePreview.Unregister();
			_SpectatePreview = null;
		}

		public override void DisplayPreview()
		{
			base.DisplayPreview();

			if (!Preview || InputMap == null || InputMap == Map.Internal || Battle.SpectateRegion == null ||
				Battle.SpectateRegion.Area.Length == 0)
			{
				if (_SpectatePreview != null)
				{
					_SpectatePreview.Unregister();
					_SpectatePreview = null;
				}

				return;
			}

			if (_SpectatePreview != null)
			{
				if (_SpectatePreview.Map == InputMap &&
					_SpectatePreview.Area.GetBoundsHashCode() == Battle.SpectateRegion.Area.GetBoundsHashCode())
				{
					_SpectatePreview.Refresh();
					return;
				}

				_SpectatePreview.Unregister();
				_SpectatePreview = null;
			}

			_SpectatePreview = Battle.SpectateRegion.DisplayPreview(ErrorHue, PreviewEffect, EffectRender.Darken);
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			var regions = Region
				.Regions.Not(r => r == null || !r.Registered || r is PvPRegion || r is RegionExtUtility.PreviewRegion)
				.Where(r => r.Contains(User.Location, User.Map))
				.ToArray();

			if (regions.Length > 0)
			{
				var opts = new MenuGumpOptions();

				regions.ForEach(
					r => opts.AppendEntry(
						new ListGumpEntry(
							r.Name,
							() =>
							{
								ClearPreview();

								var prev = Preview;

								Preview = false;
								r.Area.ForEach(AddToList);
								Preview = prev;

								DisplayPreview();
							})));

				list.AppendEntry(new ListGumpEntry("Use Region...", b => Send(new MenuGump(User, Refresh(), opts, b))));
			}
			else
			{
				list.RemoveEntry("Use Region...");
			}

			base.CompileMenuOptions(list);
		}
	}
}