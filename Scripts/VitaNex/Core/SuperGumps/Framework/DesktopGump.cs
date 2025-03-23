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
using System.Drawing;
using System.Linq;

using Server;
using Server.Gumps;
using Server.Network;

using VitaNex.Notify;
using VitaNex.Targets;
#endregion

namespace VitaNex.SuperGumps
{
	public sealed class DesktopGumpEntry : SuperGumpEntry
	{
		private string _Compiled;
		private byte[] _Buffer;

		private int _TypeID;
		private Action<NetState, RelayInfo> _Handler;

		public DesktopGump Desktop { get; private set; }

		public Dictionary<int, int> ButtonMap { get; private set; }
		public Dictionary<int, int> SwitchMap { get; private set; }
		public Dictionary<int, int> TextMap { get; private set; }

		public DesktopGumpEntry(DesktopGump dt, Gump gump, bool focus)
		{
			Desktop = dt;

			_TypeID = gump.TypeID;
			_Handler = gump.OnResponse;

			if (focus)
			{
				ButtonMap = new Dictionary<int, int>();
				SwitchMap = new Dictionary<int, int>();
				TextMap = new Dictionary<int, int>();
			}

			var entries = gump.Entries.Not(e => e is GumpModal).ToList();

			var esc = false;

			foreach (var e in entries)
			{
				bool pos;

				if (e.TryGetPosition(out var x, out var y))
				{
					pos = true;
					e.TrySetPosition(gump.X + x, gump.Y + y);
				}
				else
				{
					pos = false;
				}

				e.Parent = Desktop;

				try
				{
					if (e is GumpButton)
					{
						var b = (GumpButton)e;

						if (focus && b.Type == GumpButtonType.Reply)
						{
							ButtonMap[Desktop.NewButtonID()] = b.ButtonID;

							b.ButtonID = ButtonMap.GetKey(b.ButtonID);
							_Compiled += b.Compile();
							b.ButtonID = ButtonMap[b.ButtonID];
						}
						else
						{
							_Compiled += new GumpImage(b.X, b.Y, b.NormalID)
							{
								Parent = Desktop
							}.Compile();
						}
					}
					else if (e is GumpImageTileButton)
					{
						var b = (GumpImageTileButton)e;

						if (focus && b.Type == GumpButtonType.Reply)
						{
							ButtonMap[Desktop.NewButtonID()] = b.ButtonID;

							b.ButtonID = ButtonMap.GetKey(b.ButtonID);
							_Compiled += b.Compile();
							b.ButtonID = ButtonMap[b.ButtonID];
						}
						else
						{
							_Compiled += new GumpImage(b.X, b.Y, b.NormalID)
							{
								Parent = Desktop
							}.Compile();
						}
					}
					else if (e is GumpCheck)
					{
						var c = (GumpCheck)e;

						if (focus)
						{
							SwitchMap[Desktop.NewSwitchID()] = c.SwitchID;

							c.SwitchID = SwitchMap.GetKey(c.SwitchID);
							_Compiled += c.Compile();
							c.SwitchID = SwitchMap[c.SwitchID];
						}
						else
						{
							_Compiled += new GumpImage(c.X, c.Y, c.InitialState ? c.ActiveID : c.InactiveID)
							{
								Parent = Desktop
							}.Compile();
						}
					}
					else if (e is GumpRadio)
					{
						var r = (GumpRadio)e;

						if (focus)
						{
							SwitchMap[Desktop.NewSwitchID()] = r.SwitchID;

							r.SwitchID = SwitchMap.GetKey(r.SwitchID);
							_Compiled += r.Compile();
							r.SwitchID = SwitchMap[r.SwitchID];
						}
						else
						{
							_Compiled += new GumpImage(r.X, r.Y, r.InitialState ? r.ActiveID : r.InactiveID)
							{
								Parent = Desktop
							}.Compile();
						}
					}
					else if (e is GumpTextEntry)
					{
						var t = (GumpTextEntry)e;

						if (focus)
						{
							TextMap[Desktop.NewTextEntryID()] = t.EntryID;

							t.EntryID = TextMap.GetKey(t.EntryID);
							_Compiled += t.Compile();
							t.EntryID = TextMap[t.EntryID];
						}
						else
						{
							_Compiled += new GumpLabelCropped(t.X, t.Y, t.Width, t.Height, t.Hue, t.InitialText)
							{
								Parent = Desktop
							}.Compile();
						}
					}
					else if (e is GumpTextEntryLimited)
					{
						var t = (GumpTextEntryLimited)e;

						if (focus)
						{
							TextMap[Desktop.NewTextEntryID()] = t.EntryID;

							t.EntryID = TextMap.GetKey(t.EntryID);
							t.Parent = Desktop;
							_Compiled += t.Compile();
							t.EntryID = TextMap[t.EntryID];
						}
						else
						{
							_Compiled += new GumpLabelCropped(t.X, t.Y, t.Width, t.Height, t.Hue, t.InitialText)
							{
								Parent = Desktop
							}.Compile();
						}
					}
					else if (e is GumpPage)
					{
						var p = (GumpPage)e;

						if (p.Page > 0)
						{
							esc = true;
						}
					}
					else
					{
						_Compiled += e.Compile();
					}
				}
				catch
				{ }

				e.Parent = gump;

				if (pos)
				{
					e.TrySetPosition(x, y);
				}

				if (esc)
				{
					break;
				}
			}

			entries.Free(true);

			if (String.IsNullOrWhiteSpace(_Compiled))
			{
				_Compiled = "{{ gumptooltip -1 }}";
			}

			_Buffer = Gump.StringToBuffer(_Compiled);
		}

		public override string Compile()
		{
			return _Compiled;
		}

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(_Buffer);
		}

		public bool OnResponse(NetState ns, RelayInfo info)
		{
			if (_Handler == null || Desktop == null || Desktop.Viewed == null || Desktop.Viewed.NetState == null ||
				Desktop.Viewed.NetState.Gumps == null)
			{
				return false;
			}

			var buttonID = info.ButtonID;

			if (!ButtonMap.ContainsKey(buttonID))
			{
				return false;
			}

			buttonID = ButtonMap[buttonID];

			var switches = info.Switches.Where(SwitchMap.ContainsKey).Select(SwitchMap.GetValue).ToArray();
			var texts = info.TextEntries.Where(tr => TextMap.ContainsKey(tr.EntryID))
							.Select(tr => new TextRelay(TextMap[tr.EntryID], tr.Text))
							.ToArray();

			Desktop.Viewed.NetState.Gumps.RemoveAll(g => g.TypeID == _TypeID);
			Desktop.Viewed.NetState.Send(new CloseGump(_TypeID, 0));

			_Handler(Desktop.Viewed.NetState, new RelayInfo(buttonID, switches, texts));
			return true;
		}

		public override void Dispose()
		{
			ButtonMap.Clear();
			ButtonMap = null;

			SwitchMap.Clear();
			SwitchMap = null;

			TextMap.Clear();
			TextMap = null;

			Desktop = null;

			_Compiled = null;
			_Buffer = null;

			_TypeID = -1;
			_Handler = null;

			base.Dispose();
		}
	}

	public class DesktopGump : SuperGumpList<Gump>
	{
		public static void Initialize()
		{
			CommandUtility.Register("ViewDesktop", AccessLevel.GameMaster, e => BeginDesktopTarget(e.Mobile));
		}

		public static void BeginDesktopTarget(Mobile m)
		{
			if (m != null && !m.Deleted && m.IsOnline())
			{
				MobileSelectTarget.Begin(m, DisplayDesktop, null);
			}
		}

		public static void DisplayDesktop(Mobile viewer, Mobile viewed)
		{
			if (viewer == null || viewed == null || viewer == viewed)
			{
				return;
			}

			if (!viewed.IsOnline())
			{
				viewer.SendMessage(0x22, "You can't view the desktop of an offline player!");
				return;
			}

			Send(new DesktopGump(viewer, viewed));
		}

		private static IEnumerable<Gump> FilterGumps(IEnumerable<Gump> list)
		{
			return list.Where(g => g != null && !(g is DesktopGump) && !(g is NotifyGump))
					   .Where(
						   g =>
						   {
							   if (g is SuperGump)
							   {
								   var sg = (SuperGump)g;

								   if (sg.IsDisposed || !sg.Compiled || !sg.IsOpen)
								   {
									   return false;
								   }
							   }

							   return true;
						   })
					   .Reverse()
					   .TakeUntil(g => g is SuperGump && ((SuperGump)g).Modal)
					   .Reverse();
		}

		private long _NextUpdate;

		public Mobile Viewed { get; set; }

		public DesktopGump(Mobile viewer, Mobile viewed)
			: base(viewer, null, 0, 0)
		{
			Viewed = viewed;

			CanMove = false;
			CanResize = false;
			CanDispose = false;
			CanClose = true;

			Modal = true;

			BlockMovement = true;
			BlockSpeech = true;

			AutoRefreshRate = TimeSpan.FromSeconds(30.0);
			AutoRefresh = true;
		}

		protected override bool CanAutoRefresh()
		{
			if (IsDisposed || !IsOpen || !AutoRefresh || HasChildren || Viewed == null || Viewed.NetState == null)
			{
				return base.CanAutoRefresh();
			}

			if (VitaNexCore.Ticks > _NextUpdate)
			{
				_NextUpdate = VitaNexCore.Ticks + 1000;

				if (!List.ContentsEqual(FilterGumps(Viewed.NetState.Gumps), false))
				{
					return true;
				}
			}

			return base.CanAutoRefresh();
		}

		protected override void CompileList(List<Gump> list)
		{
			list.Clear();

			if (Viewed != null && Viewed.NetState != null && Viewed.NetState.Gumps != null)
			{
				list.AddRange(FilterGumps(Viewed.NetState.Gumps));
			}

			base.CompileList(list);
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			var i = 0;
			var l = List.LastOrDefault();

			foreach (var g in List)
			{
				CompileEntryLayout(layout, i++, g, g == l);
			}

			layout.Add(
				"cpanel",
				() =>
				{
					const int x = 620, y = 0, w = 200, h = 60;

					AddImageTiled(x, y, w, h, 2624);
					AddAlphaRegion(x, y, w, h);

					var title = "DESKTOP: ".WrapUOHtmlBold();
					title = title.WrapUOHtmlColor(Color.Gold, false);
					title = title + Viewed.RawName.WrapUOHtmlColor(User.GetNotorietyColor(Viewed), false);
					title = title.WrapUOHtmlCenter();

					AddHtml(x + 5, y + 5, w - 10, h - 30, title, false, false);

					AddButton(x + 5, h - 30, 4017, 4019, Close);
					AddTooltip(3000363); // Close

					AddButton(x + 45, h - 30, 4014, 4016, Refresh);
					AddTooltip(1015002); // Refresh

					AddRectangle(x, y, w, h, Color.Empty, Color.Gold, 2);
				});
		}

		protected void CompileEntryLayout(SuperGumpLayout layout, int index, Gump gump, bool focus)
		{
			layout.Add(
				"gumps/" + index,
				() =>
				{
					//Console.WriteLine("Layout Entry {0}: {1}", index, gump);

					Add(new DesktopGumpEntry(this, gump, focus));
					AddRectangle(gump.GetBounds(), Color.Empty, Color.LawnGreen, 2);
				});
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (IsDisposed)
			{
				return;
			}

			if (info.ButtonID > 0 && GetButtonEntry(info.ButtonID) == null && GetTileButtonEntry(info.ButtonID) == null)
			{
				var handled = VitaNexCore.TryCatchGet(
					() =>
					{
						if (GetEntries<DesktopGumpEntry>().Any(e => e.OnResponse(sender, info)))
						{
							User.SendMessage("Response injection successful!");
							return true;
						}

						return false;
					});

				if (handled)
				{
					Refresh(true);
					return;
				}
			}

			base.OnResponse(sender, info);
		}
	}
}