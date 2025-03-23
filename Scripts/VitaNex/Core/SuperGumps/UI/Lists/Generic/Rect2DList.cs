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
using Server.Gumps;

using VitaNex.Network;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public abstract class Rect2DListGump : GenericListGump<Rectangle2D>
	{
		public bool Preview { get; set; }
		public int PreviewHue { get; set; }
		public int PreviewEffect { get; set; }
		public EffectRender PreviewRender { get; set; }
		public string PreviewName { get; set; }

		public Rectangle2D? InputRect { get; set; }
		public Map InputMap { get; set; }

		private RegionExtUtility.PreviewRegion _Preview;

		public Rect2DListGump(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			IEnumerable<Rectangle2D> list = null,
			string emptyText = null,
			string title = null,
			IEnumerable<ListGumpEntry> opts = null,
			bool canAdd = true,
			bool canRemove = true,
			bool canClear = true,
			Action<Rectangle2D> addCallback = null,
			Action<Rectangle2D> removeCallback = null,
			Action<List<Rectangle2D>> applyCallback = null,
			Action clearCallback = null)
			: base(
				user,
				parent,
				x,
				y,
				list,
				emptyText,
				title,
				opts,
				canAdd,
				canRemove,
				canClear,
				addCallback,
				removeCallback,
				applyCallback,
				clearCallback)
		{
			InputMap = User.Map;

			Preview = false;
			PreviewHue = TextHue;
			PreviewEffect = 1801;
			PreviewRender = EffectRender.SemiTransparent;
			PreviewName = "Preview Region";

			ForceRecompile = true;
		}

		protected override int GetLabelHue(int index, int pageIndex, Rectangle2D entry)
		{
			return PreviewHue;
		}

		protected override string GetLabelText(int index, int pageIndex, Rectangle2D entry)
		{
			return entry.Start + " -> " + entry.End;
		}

		public override string GetSearchKeyFor(Rectangle2D key)
		{
			return key.Start + " -> " + key.End;
		}

		protected override bool OnBeforeListAdd()
		{
			if (InputRect != null)
			{
				return true;
			}

			Minimize();

			BoundingBoxPicker.Begin(
				User,
				(from, map, start, end, state) =>
				{
					InputMap = map;
					InputRect = new Rectangle2D(start, end.Clone2D(1, 1));
					HandleAdd();
					InputRect = null;

					Maximize();
				},
				null);

			return false;
		}

		public override Rectangle2D GetListAddObject()
		{
			return InputRect ?? default(Rectangle2D);
		}

		protected override void OnSend()
		{
			base.OnSend();

			DisplayPreview();
		}

		protected override void OnRefreshed()
		{
			base.OnRefreshed();

			DisplayPreview();
		}

		protected override void OnClosed(bool all)
		{
			base.OnClosed(all);

			ClearPreview();
		}

		protected override void OnDispose()
		{
			base.OnDispose();

			ClearPreview();
		}

		protected override void CompileEntryOptions(MenuGumpOptions opts, Rectangle2D entry)
		{
			opts.AppendEntry("Go To", () => User.MoveToWorld(entry.Start.GetWorldTop(InputMap), InputMap), HighlightHue);

			base.CompileEntryOptions(opts, entry);
		}

		public virtual void ClearPreview()
		{
			if (_Preview == null)
			{
				return;
			}

			_Preview.Unregister();
			_Preview = null;
		}

		public virtual void DisplayPreview()
		{
			if (!Preview || InputMap == null || InputMap == Map.Internal || List.Count == 0)
			{
				if (_Preview != null)
				{
					_Preview.Unregister();
					_Preview = null;
				}

				return;
			}

			if (_Preview != null)
			{
				if (_Preview.Map == InputMap && _Preview.Area.GetBoundsHashCode() == List.GetBoundsHashCode())
				{
					_Preview.Refresh();
					return;
				}

				_Preview.Unregister();
			}

			_Preview = RegionExtUtility.DisplayPreview(
				PreviewName,
				InputMap,
				PreviewHue,
				PreviewEffect,
				PreviewRender,
				List.ToArray());
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			if (!Preview)
			{
				list.Replace(
					"Disable Preview",
					"Enable Preview",
					() =>
					{
						Preview = true;
						DisplayPreview();
						Refresh();
					},
					HighlightHue);
			}
			else
			{
				list.Replace(
					"Enable Preview",
					"Disable Preview",
					() =>
					{
						Preview = false;
						ClearPreview();
						Refresh();
					},
					ErrorHue);
			}

			base.CompileMenuOptions(list);
		}
	}
}