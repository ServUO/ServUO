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
	public class PvPSpectateBoundsGump : Rect3DListGump
	{
		public PvPBattle Battle { get; set; }

		private RegionExtUtility.PreviewRegion _BattlePreview;

		public PvPSpectateBoundsGump(Mobile user, PvPBattle battle, Gump parent = null)
			: base(
				user,
				parent,
				list: battle.Options.Locations.SpectateBounds,
				emptyText: "There are no bounds in the list.",
				title: "Spectate Region Bounds")
		{
			Battle = battle;

			InputMap = Battle.Map;

			Preview = true;
			PreviewHue = HighlightHue;
			PreviewEffect = 3259;
			PreviewName = String.Format("Spectate Region Preview: {0} ({1})", Battle.Name, Battle.Serial);

			ForceRecompile = true;
		}

		public override List<Rectangle3D> GetExternalList()
		{
			return Battle.Options.Locations.SpectateBounds;
		}

		protected override bool OnBeforeListAdd()
		{
			if (!base.OnBeforeListAdd())
			{
				return false;
			}

			if (InputRect != null && InputRect.Value.ToRectangle2D()
											  .EnumeratePoints()
											  .Any(Battle.Options.Locations.BattleBounds.Contains))
			{
				User.SendMessage(ErrorHue, "Bounds can not overlap Battle region.");
				return false;
			}

			return true;
		}

		protected override void HandleApplyChanges()
		{
			base.HandleApplyChanges();

			Battle.Map = InputMap;
			Battle.InvalidateSpectateRegion();
		}

		public override void ClearPreview()
		{
			base.ClearPreview();

			if (_BattlePreview == null)
			{
				return;
			}

			_BattlePreview.Unregister();
			_BattlePreview = null;
		}

		public override void DisplayPreview()
		{
			base.DisplayPreview();

			if (!Preview || InputMap == null || InputMap == Map.Internal || Battle.BattleRegion == null ||
				Battle.BattleRegion.Area.Length == 0)
			{
				if (_BattlePreview != null)
				{
					_BattlePreview.Unregister();
					_BattlePreview = null;
				}

				return;
			}

			if (_BattlePreview != null)
			{
				if (_BattlePreview.Map == InputMap &&
					_BattlePreview.Area.GetBoundsHashCode() == Battle.BattleRegion.Area.GetBoundsHashCode())
				{
					_BattlePreview.Refresh();
					return;
				}

				_BattlePreview.Unregister();
				_BattlePreview = null;
			}

			_BattlePreview = Battle.BattleRegion.DisplayPreview(ErrorHue, PreviewEffect, EffectRender.Darken);
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