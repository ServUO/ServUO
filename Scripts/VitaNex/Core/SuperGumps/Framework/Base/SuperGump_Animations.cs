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
using System.Linq;
#endregion

namespace VitaNex.SuperGumps
{
	public abstract partial class SuperGump
	{
		public static TimeSpan DefaultAnimationRate = TimeSpan.FromMilliseconds(100.0);

		private bool? _OldForceRecompile;
		private bool? _OldAutoRefresh;
		private TimeSpan _OldRefreshRate;

		public bool IsAnimated => GetEntries<GumpAnimation>().Any(e => !e.Entries.IsNullOrEmpty() && e.State != null);

		public bool IsAnimating => GetEntries<GumpAnimation>()
					.Where(e => !e.Entries.IsNullOrEmpty() && e.State != null)
					.Any(e => e.State.Sequencing || e.State.Animating || e.State.Waiting);

		public TimeSpan AnimationRate { get; set; }

		public void SetAnimationBreak()
		{
			Add(new GumpAnimationBreak());
		}

		public void QueueAnimation(
			Action<GumpAnimation> handler,
			string name,
			int take = -1,
			long delay = 0,
			long duration = 0,
			bool repeat = false,
			bool wait = false,
			params object[] args)
		{
			Add(new GumpAnimation(this, name, take, delay, duration, repeat, wait, args, handler));
		}

		private void InvalidateAnimations()
		{
			if (_OldForceRecompile == null)
			{
				_OldForceRecompile = ForceRecompile;
			}

			if (_OldAutoRefresh == null)
			{
				_OldForceRecompile = ForceRecompile;
				_OldAutoRefresh = AutoRefresh;
				_OldRefreshRate = AutoRefreshRate;
			}

			if (IsAnimating)
			{
				ForceRecompile = true;

				AutoRefresh = true;
				AutoRefreshRate = AnimationRate;
			}
			else
			{
				if (_OldForceRecompile != null)
				{
					ForceRecompile = _OldForceRecompile.Value;

					_OldForceRecompile = null;
				}

				if (_OldAutoRefresh != null)
				{
					AutoRefresh = _OldAutoRefresh.Value;
					AutoRefreshRate = _OldRefreshRate;

					_OldAutoRefresh = null;
				}
			}

			var idx = Entries.Count;

			while (--idx >= 0)
			{
				if (!Entries.InBounds(idx))
				{
					continue;
				}

				var e = Entries[idx];

				if (e is GumpAnimation)
				{
					var a = (GumpAnimation)e;

					if (a.State.Waiting)
					{
						Entries.TrimEndTo(idx + 1);
						break;
					}

					a.Animate();
				}
			}
		}

		public virtual void OnAnimate(GumpAnimation a)
		{ }
	}
}