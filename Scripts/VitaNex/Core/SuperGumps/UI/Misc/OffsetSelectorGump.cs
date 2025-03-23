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

using Server;
using Server.Accounting;
using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class OffsetSelectorGump : SuperGump
	{
		public static Point DefaultOffset = Point.Empty;

		private Point _Value;

		public Point Value
		{
			get => _Value;
			set
			{
				if (_Value == value)
				{
					return;
				}

				var oldValue = _Value;

				_Value = value;

				OnValueChanged(oldValue);
			}
		}

		public int Cols { get; private set; }
		public int Rows { get; private set; }

		public virtual Action<OffsetSelectorGump, Point> ValueChanged { get; set; }

		public OffsetSelectorGump(
			Mobile user,
			Gump parent = null,
			Point? value = null,
			Action<OffsetSelectorGump, Point> valueChanged = null)
			: base(user, parent, 0, 0)
		{
			ForceRecompile = true;

			CanMove = false;
			CanClose = true;
			CanDispose = true;

			_Value = value ?? DefaultOffset;
			ValueChanged = valueChanged;

			var hi = User.Account is Account ? ((Account)User.Account).HardwareInfo : null;

			Cols = (hi != null ? hi.ScreenWidth : 1920) / 20;
			Rows = (hi != null ? hi.ScreenHeight : 1080) / 20;
		}

		public override void AssignCollections()
		{
			var capacity = Cols * Rows;

			if (Entries != null && Entries.Capacity < 0x20 + capacity)
			{
				Entries.Capacity = 0x20 + capacity;
			}

			if (Buttons == null)
			{
				Buttons = new Dictionary<GumpButton, Action<GumpButton>>(capacity);
			}

			base.AssignCollections();
		}

		protected override bool OnBeforeSend()
		{
			User.SendMessage(0x55, "Generating Offset Selection Interface, please wait...");

			return base.OnBeforeSend();
		}

		protected virtual void OnValueChanged(Point oldValue)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, oldValue);
			}

			Refresh();
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add(
				"buttongrid/base",
				() =>
				{
					int x, y;

					for (y = 0; y < Rows; y++)
					{
						for (x = 0; x < Cols; x++)
						{
							AddButton(x * 20, y * 20, 9028, 9021, OnSelectPoint);
						}
					}
				});

			layout.Add("image/marker", () => AddImage(Value.X + 5, Value.Y, 9009));
		}

		protected virtual void OnSelectPoint(GumpButton b)
		{
			Value = new Point(b.X, b.Y);
		}
	}
}