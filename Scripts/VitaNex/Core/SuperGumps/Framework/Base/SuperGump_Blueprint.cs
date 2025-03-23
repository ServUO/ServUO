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

using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps
{
	public abstract partial class SuperGump
	{
		private SuperGumpLayout _Layout;

		public SuperGumpLayout Layout { get => _Layout; protected set => _Layout = value; }

		public virtual bool CanMove { get => Dragable; set => Dragable = value; }
		public virtual bool CanClose { get => Closable; set => Closable = value; }
		public virtual bool CanDispose { get => Disposable; set => Disposable = value; }
		public virtual bool CanResize { get => Resizable; set => Resizable = value; }

		public virtual int ModalXOffset { get; set; }
		public virtual int ModalYOffset { get; set; }
		public virtual int XOffset { get; set; }
		public virtual int YOffset { get; set; }

		private long _SizeTick = -1;
		private Size _Size = Size.Empty;

		public Size OuterSize
		{
			get
			{
				lock (_InstanceLock)
				{
					// Prevent computing size if called successive times on the same tick
					var tick = VitaNexCore.Tick;

					if (tick == _SizeTick)
					{
						return _Size;
					}

					_SizeTick = tick;

					int x1 = 0, y1 = 0, x2 = 0, y2 = 0;

					foreach (var e in Entries.Not(e => e is GumpModal))
					{

						if (!e.TryGetBounds(out var ex, out var ey, out var ew, out var eh))
						{
							continue;
						}

						x1 = Math.Min(x1, ex);
						y1 = Math.Min(y1, ey);

						x2 = Math.Max(x2, ex + ew);
						y2 = Math.Max(y2, ey + eh);
					}

					_Size.Width = Math.Max(0, x2 - x1);
					_Size.Height = Math.Max(0, y2 - y1);

					return _Size;
				}
			}
		}

		public int OuterWidth => OuterSize.Width;
		public int OuterHeight => OuterSize.Height;

		public void InvalidateSize()
		{
			_SizeTick = -1;
		}

		private void InvalidateOffsets()
		{
			Entries.ForEachReverse(
				e =>
				{
					var x = XOffset;
					var y = YOffset;

					if (Modal && (!(e is SuperGumpEntry) || !((SuperGumpEntry)e).IgnoreModalOffset))
					{
						x += ModalXOffset;
						y += ModalYOffset;
					}

					if (x != 0 || y != 0)
					{
						e.TryOffsetPosition(x, y);
					}
				});
		}
	}
}