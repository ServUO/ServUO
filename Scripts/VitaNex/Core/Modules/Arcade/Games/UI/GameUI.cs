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
using System.Drawing;
using System.Text;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.Games
{
	public abstract class GameUI<TEngine> : SuperGump, IGameUI
		where TEngine : class, IGameEngine
	{
		public bool IsDisposing { get; private set; }

		public TEngine Engine { get; private set; }

		public GameUI(TEngine engine)
			: base(engine.User)
		{
			Engine = engine;

			RandomButtonID = true;
			ForceRecompile = true;
		}

		public bool Validate()
		{
			return !IsDisposed && !IsDisposing && Engine != null && !Engine.IsDisposed && User != null && !User.Deleted;
		}

		protected override void OnDispose()
		{
			if (IsDisposed || IsDisposing)
			{
				return;
			}

			IsDisposing = true;

			base.OnDispose();
		}

		protected override void OnDisposed()
		{
			base.OnDisposed();

			IsDisposing = false;
		}

		protected override void OnClosed(bool all)
		{
			base.OnClosed(all);

			if (!all)
			{
				OnQuit();
			}
			else
			{
				Engine.Dispose();
			}
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add(
				"window",
				() =>
				{
					AddBackground(0, 0, 650, 490, 2620);

					AddImage(5, 5, 9001);
					AddImage(5, 5, 9002, 901);
				});

			layout.Add(
				"window/title",
				() =>
				{
					var title = GetTitle();

					AddImage(10, 10, 2440, 901);
					AddHtml(10, 12, 166, 40, title, false, false);
				});

			layout.Add(
				"window/score",
				() =>
				{
					var points = GetPoints();

					AddImage(180, 10, 2440, 901);
					AddHtml(180, 12, 166, 40, points, false, false);
				});

			layout.Add("window/quit", () => { AddButton(560, 5, 5514, 5515, Close); });
		}

		protected virtual void OnQuit()
		{ }

		public virtual string GetPoints()
		{
			if (Engine == null)
			{
				return String.Empty;
			}

			var p = String.Format("Points: {0:#,0.##}", Engine.Points);

			var t = new StringBuilder();

			var c1 = Color.Gold;
			var c2 = Color.PaleGoldenrod;

			for (var i = 0; i < p.Length; i++)
			{
				if (!Char.IsWhiteSpace(p, i))
				{
					t.Append(p[i].ToString().WrapUOHtmlColor(c1.Interpolate(c2, i / (p.Length - 1.0)), false));
				}
				else
				{
					t.Append(p[i]);
				}
			}

			return t.ToString().WrapUOHtmlBig().WrapUOHtmlCenter();
		}

		public virtual string GetTitle()
		{
			if (Engine == null || Engine.Game == null)
			{
				return String.Empty;
			}

			var n = Engine.Game.Name.Trim();

			var t = new StringBuilder();

			var c1 = Color.Gold;
			var c2 = Color.PaleGoldenrod;

			for (var i = 0; i < n.Length; i++)
			{
				if (!Char.IsWhiteSpace(n, i))
				{
					t.Append(n[i].ToString().WrapUOHtmlColor(c1.Interpolate(c2, i / (n.Length - 1.0)), false));
				}
				else
				{
					t.Append(n[i]);
				}
			}

			return t.ToString().WrapUOHtmlBig().WrapUOHtmlCenter();
		}

		#region Explicit Impl
		IGameEngine IGameUI.Engine => Engine;
		#endregion
	}
}