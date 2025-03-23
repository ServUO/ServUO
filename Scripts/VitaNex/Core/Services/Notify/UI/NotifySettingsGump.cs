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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Server;
using Server.Commands.Generic;
using Server.Gumps;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Notify
{
	public class NotifySettingsGump : SuperGumpList<NotifySettings>
	{
		public NotifySettingsGump(Mobile user)
			: base(user)
		{
			EntriesPerPage = 5;
		}

		protected override void CompileList(List<NotifySettings> list)
		{
			list.Clear();
			list.AddRange(Notify.Settings.Values.Where(o => User.AccessLevel >= o.Access).OrderByNatural());

			base.CompileList(list);
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			const int width = 400, height = 600;

			var sup = SupportsUltimaStore;
			var pad = sup ? 15 : 10;
			var bgID = sup ? 40000 : 9270;

			layout.Add("bg", () => AddBackground(0, 0, width, height, bgID));

			layout.Add(
				"title",
				() =>
				{
					var title = "Notification Settings";

					title = title.WrapUOHtmlBig();
					title = title.WrapUOHtmlCenter();
					title = title.WrapUOHtmlColor(Color.Gold, false);

					AddHtml(pad, pad, width - (pad * 2), 40, title, false, false);
				});

			layout.Add(
				"opts",
				() =>
				{
					var xx = pad;
					var yy = pad + 30;
					var ww = width - (pad * 2);
					var hh = height - ((pad * 2) + 60);

					var r = GetListRange();
					var o = AddAccordion(xx, yy, ww, hh, r.Values, CompileEntryLayout);

					if (o == null || o.Item1 == null)
					{
						if (o != null)
						{
							CompileEmptyLayout(xx, yy + o.Item2, ww, hh - o.Item2);
						}
						else
						{
							CompileEmptyLayout(xx, yy, ww, hh);
						}
					}
				});

			layout.Add(
				"pages",
				() =>
				{
					var xx = pad;
					var yy = (height - 25) - pad;
					var ww = width - (pad * 2);

					if (HasPrevPage)
					{
						if (sup)
						{
							AddButton(xx, yy, 40016, 40026, PreviousPage);
						}
						else
						{
							AddButton(xx, yy, 9766, 9767, PreviousPage);
						}
					}

					xx += 35;
					ww -= 70;

					var page = String.Format("Page {0:#,0} / {1:#,0}", Page + 1, PageCount);

					page = page.WrapUOHtmlCenter();
					page = page.WrapUOHtmlColor(Color.PaleGoldenrod, false);

					AddHtml(xx, yy + 2, ww, 40, page, false, false);

					xx += ww;

					if (HasNextPage)
					{
						if (sup)
						{
							AddButton(xx, yy, 40017, 40027, NextPage);
						}
						else
						{
							AddButton(xx, yy, 9762, 9763, NextPage);
						}
					}
				});
		}

		protected virtual void CompileEntryLayout(int x, int y, int w, int h, NotifySettings o)
		{
			var sup = SupportsUltimaStore;
			var pad = sup ? 15 : 10;
			var off = sup ? 35 : 10;
			var bgID = sup ? 40000 : 9270;
			var btnNormal = sup ? 40017 : 9762;
			var btnSelected = sup ? 40027 : 9763;
			var chkNormal = sup ? 40014 : 9722;
			var chkSelected = sup ? 40015 : 9723;

			var s = o.EnsureState(User.Account);

			string label;

			if (User.AccessLevel >= Notify.Access)
			{
				AddButton(
					x,
					y,
					btnNormal,
					btnSelected,
					b =>
					{
						Refresh();

						if (User.AccessLevel >= Notify.Access)
						{
							User.SendGump(new PropertiesGump(User, o));
						}
					});

				label = Notify.Access.ToString().ToUpper();
				label = label.WrapUOHtmlBold();
				label = label.WrapUOHtmlColor(Color.PaleGoldenrod, false);

				AddHtml(x + 35, y + 2, w - 35, 40, label, false, false);

				y += 30;
				h -= 30;
			}

			if (w * h <= 0)
			{
				return;
			}

			if (o.CanIgnore)
			{
				AddCheck(x, y, chkNormal, chkSelected, s.Ignore, (c, v) => s.Ignore = v);

				label = "IGNORE";
				label = label.WrapUOHtmlBold();
				label = label.WrapUOHtmlColor(Color.PaleGoldenrod, false);
			}
			else
			{
				AddImage(x, y, chkNormal, 900);

				label = "IGNORE";
				label = label.WrapUOHtmlBold();
				label = label.WrapUOHtmlColor(Color.Gray, false);
			}

			AddHtml(x + 30, y + 2, (w / 2) - 30, 40, label, false, false);

			if (o.CanAutoClose)
			{
				AddCheck(x + (w / 2), y, chkNormal, chkSelected, s.AutoClose, (c, v) => s.AutoClose = v);

				label = "AUTO CLOSE";
				label = label.WrapUOHtmlBold();
				label = label.WrapUOHtmlColor(Color.PaleGoldenrod, false);
			}
			else
			{
				AddImage(x + (w / 2), y, chkNormal, 900);

				label = "AUTO CLOSE";
				label = label.WrapUOHtmlBold();
				label = label.WrapUOHtmlColor(Color.Gray, false);
			}

			AddHtml(x + (w / 2) + 30, y + 2, (w / 2) - 30, 40, label, false, false);

			y += 30;
			h -= 30;

			if (w * h <= 0)
			{
				return;
			}

			AddCheck(x, y, chkNormal, chkSelected, s.Animate, (c, v) => s.Animate = v);

			label = "ANIMATE";
			label = label.WrapUOHtmlBold();
			label = label.WrapUOHtmlColor(Color.PaleGoldenrod, false);

			AddHtml(x + 30, y + 2, (w / 2) - 30, 40, label, false, false);

			AddCheck(x + (w / 2), y, chkNormal, chkSelected, s.TextOnly, (c, v) => s.TextOnly = v);

			label = "TEXT ONLY";
			label = label.WrapUOHtmlBold();
			label = label.WrapUOHtmlColor(Color.PaleGoldenrod, false);

			AddHtml(x + (w / 2) + 30, y + 2, (w / 2) - 30, 40, label, false, false);

			y += 30;
			h -= 30;

			if (w * h <= 0)
			{
				return;
			}

			var track = (w / 2) - 60;

			AddScrollbarH(
				x,
				y + 4,
				200,
				s.Speed,
				p =>
				{
					s.Speed = (byte)Math.Max(0, s.Speed - 10);

					Refresh(true);
				},
				n =>
				{
					s.Speed = (byte)Math.Min(200, s.Speed + 10);

					Refresh(true);
				},
				new Rectangle(30, 0, track, 13),
				new Rectangle(0, 0, 28, 13),
				new Rectangle(track + 32, 0, 28, 13));

			label = String.Format("{0}% SPEED", Math.Max((byte)1, s.Speed));
			label = label.WrapUOHtmlBold();
			label = label.WrapUOHtmlColor(Color.PaleGoldenrod, false);

			AddHtml(x + track + 70, y + 2, w - (track + 60), 40, label, false, false);

			y += 30;
			h -= 30;

			if (w * h <= 0)
			{
				return;
			}

			if (String.IsNullOrWhiteSpace(o.Desc))
			{
				return;
			}

			x -= pad;
			w += pad * 2;

			AddImage(x, y, bgID);
			AddImageTiled(x + off, y, w - (off * 2), off, bgID + 1);
			AddImage(x + off + (w - (off * 2)), y, bgID + 2);

			x += pad;
			w -= pad * 2;
			y += pad;
			h -= pad;

			label = "Description";
			label = label.WrapUOHtmlBig();
			label = label.WrapUOHtmlCenter();
			label = label.WrapUOHtmlColor(Color.Gold, false);

			AddHtml(x, y, w, 40, label, false, false);

			x += 5;
			w -= 5;
			y += 20;
			h -= 10;

			if (w * h <= 0)
			{
				return;
			}

			label = o.Desc;
			label = label.WrapUOHtmlColor(Color.PaleGoldenrod, false);

			AddHtml(x, y, w, h, label, false, true);
		}

		protected virtual void CompileEmptyLayout(int x, int y, int w, int h)
		{
			var sup = SupportsUltimaStore;
			var pad = sup ? 15 : 10;
			var off = sup ? 35 : 10;
			var bgID = sup ? 40000 : 9270;
			var btnNormal = sup ? 40017 : 9762;
			var btnSelected = sup ? 40027 : 9763;
			var chkNormal = sup ? 40014 : 9722;
			var chkSelected = sup ? 40015 : 9723;

			x -= pad;
			w += pad * 2;

			AddImage(x, y, bgID);
			AddImageTiled(x + off, y, w - (off * 2), off, bgID + 1);
			AddImage(x + off + (w - (off * 2)), y, bgID + 2);

			x += pad;
			w -= pad * 2;
			y += pad;
			h -= pad;

			var label = "All Notifications";
			label = label.WrapUOHtmlBig();
			label = label.WrapUOHtmlCenter();
			label = label.WrapUOHtmlColor(Color.Gold, false);

			AddHtml(x, y + 2, w, 40, label, false, false);

			y += 30;
			h -= 30;

			if (User.AccessLevel >= Notify.Access)
			{
				AddButton(
					x,
					y,
					btnNormal,
					btnSelected,
					b =>
					{
						Refresh();

						if (User.AccessLevel >= Notify.Access)
						{
							var cols = new[] { "Object" };
							var list = new ArrayList(List);

							User.SendGump(new InterfaceGump(User, cols, list, 0, null));
						}
					});

				label = Notify.Access.ToString().ToUpper();
				label = label.WrapUOHtmlBold();
				label = label.WrapUOHtmlColor(Color.PaleGoldenrod, false);

				AddHtml(x + 35, y + 2, w - 35, 40, label, false, false);

				y += 30;
				h -= 30;
			}

			if (w * h <= 0)
			{
				return;
			}

			var states = List.Select(o => o.EnsureState(User.Account));

			var vals = new int[5, 2];

			foreach (var o in states)
			{
				if (o.Settings.CanIgnore)
				{
					if (o.Ignore)
					{
						++vals[0, 0];
					}
					else
					{
						++vals[0, 1];
					}
				}

				if (o.Settings.CanAutoClose)
				{
					if (o.AutoClose)
					{
						++vals[1, 0];
					}
					else
					{
						++vals[1, 1];
					}
				}

				if (o.Animate)
				{
					++vals[2, 0];
				}
				else
				{
					++vals[2, 1];
				}

				if (o.TextOnly)
				{
					++vals[3, 0];
				}
				else
				{
					++vals[3, 1];
				}

				vals[4, 0] += o.Speed;
				++vals[4, 1];
			}

			var ignore = vals[0, 0] >= vals[0, 1];

			AddButton(
				x,
				y,
				ignore ? chkSelected : chkNormal,
				ignore ? chkNormal : chkSelected,
				b =>
				{
					foreach (var s in List.Where(o => o.CanIgnore).Select(o => o.EnsureState(User.Account)))
					{
						s.Ignore = !ignore;
					}

					Refresh(true);
				});

			label = "IGNORE";
			label = label.WrapUOHtmlBold();
			label = label.WrapUOHtmlColor(Color.PaleGoldenrod, false);

			AddHtml(x + 30, y + 2, (w / 2) - 30, 40, label, false, false);

			var autoClose = vals[1, 0] >= vals[1, 1];

			AddButton(
				x + (w / 2),
				y,
				autoClose ? chkSelected : chkNormal,
				autoClose ? chkNormal : chkSelected,
				b =>
				{
					foreach (var s in List.Where(o => o.CanAutoClose).Select(o => o.EnsureState(User.Account)))
					{
						s.AutoClose = !autoClose;
					}

					Refresh(true);
				});

			label = "AUTO CLOSE";
			label = label.WrapUOHtmlBold();
			label = label.WrapUOHtmlColor(Color.PaleGoldenrod, false);

			AddHtml(x + (w / 2) + 30, y + 2, (w / 2) - 30, 40, label, false, false);

			y += 30;
			h -= 30;

			if (w * h <= 0)
			{
				return;
			}

			var animate = vals[2, 0] >= vals[2, 1];

			AddButton(
				x,
				y,
				animate ? chkSelected : chkNormal,
				animate ? chkNormal : chkSelected,
				b =>
				{
					foreach (var s in List.Select(o => o.EnsureState(User.Account)))
					{
						s.Animate = !animate;
					}

					Refresh(true);
				});

			label = "ANIMATE";
			label = label.WrapUOHtmlBold();
			label = label.WrapUOHtmlColor(Color.PaleGoldenrod, false);

			AddHtml(x + 30, y + 2, (w / 2) - 30, 40, label, false, false);

			var textOnly = vals[3, 0] >= vals[4, 1];

			AddButton(
				x + (w / 2),
				y,
				textOnly ? chkSelected : chkNormal,
				textOnly ? chkNormal : chkSelected,
				b =>
				{
					foreach (var s in List.Select(o => o.EnsureState(User.Account)))
					{
						s.TextOnly = !textOnly;
					}

					Refresh(true);
				});

			label = "TEXT ONLY";
			label = label.WrapUOHtmlBold();
			label = label.WrapUOHtmlColor(Color.PaleGoldenrod, false);

			AddHtml(x + (w / 2) + 30, y + 2, (w / 2) - 30, 40, label, false, false);

			y += 30;
			h -= 30;

			if (w * h <= 0)
			{
				return;
			}

			var track = (w / 2) - 60;

			var speed = vals[4, 0] / Math.Max(1, vals[4, 1]);

			AddScrollbarH(
				x,
				y + 4,
				200,
				speed,
				p =>
				{
					foreach (var s in List.Where(o => o.CanIgnore).Select(o => o.EnsureState(User.Account)))
					{
						s.Speed = (byte)Math.Max(0, speed - 10);
					}

					Refresh(true);
				},
				n =>
				{
					foreach (var s in List.Where(o => o.CanIgnore).Select(o => o.EnsureState(User.Account)))
					{
						s.Speed = (byte)Math.Min(200, speed + 10);
					}

					Refresh(true);
				},
				new Rectangle(30, 0, track, 13),
				new Rectangle(0, 0, 28, 13),
				new Rectangle(track + 32, 0, 28, 13));

			label = String.Format("{0}% SPEED", Math.Max(1, speed));
			label = label.WrapUOHtmlBold();
			label = label.WrapUOHtmlColor(Color.PaleGoldenrod, false);

			AddHtml(x + track + 70, y + 2, w - (track + 60), 40, label, false, false);
		}
	}
}