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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using Server;
using Server.Gumps;
using Server.Items;

using Ultima;

using VitaNex.SuperGumps.UI;
using VitaNex.Text;

using MultiComponentList = Server.MultiComponentList;
#endregion

namespace VitaNex.SuperGumps
{
	public abstract partial class SuperGump
	{
		public void AddPage()
		{
			AddPage(IndexedPage + 1);
		}

		public virtual void AddPageBackButton(int x, int y, int normalID, int pressedID)
		{
			AddPageBackButton(x, y, normalID, pressedID, IndexedPage);
		}

		public virtual void AddPageNextButton(int x, int y, int normalID, int pressedID)
		{
			AddPageNextButton(x, y, normalID, pressedID, IndexedPage);
		}

		public virtual void AddPageButton(int x, int y, int normalID, int pressedID)
		{
			AddPageButton(x, y, normalID, pressedID, IndexedPage);
		}

		public virtual void AddPageBackButton(int x, int y, int normalID, int pressedID, int page)
		{
			AddPageButton(x, y, normalID, pressedID, page - 1);
			AddTooltip(1011067); // Previous Page
		}

		public virtual void AddPageNextButton(int x, int y, int normalID, int pressedID, int page)
		{
			AddPageButton(x, y, normalID, pressedID, page + 1);
			AddTooltip(1011066); // Next Page
		}

		public virtual void AddPageButton(int x, int y, int normalID, int pressedID, int page)
		{
			AddButton(x, y, normalID, pressedID, -1, GumpButtonType.Page, page);
		}

		public virtual void AddModalRegion(int x, int y, int w, int h)
		{
			Add(new GumpModal(x, y, w, h, 2624));
		}

		public virtual void AddModalRegion(int x, int y, int w, int h, int gumpID)
		{
			Add(new GumpModal(x, y, w, h, gumpID));
		}

		public virtual Size AddCursor(int x, int y, UOCursor cursor)
		{
			GumpCursor c;

			Add(c = new GumpCursor(x, y, cursor));

			return new Size(c.Width, c.Height);
		}

		public virtual Size AddCursor(int x, int y, UOCursor cursor, int hue)
		{
			GumpCursor c;

			Add(c = new GumpCursor(x, y, cursor, hue));

			return new Size(c.Width, c.Height);
		}

		public virtual Size AddCursor(int x, int y, UOCursor cursor, int hue, int bgID)
		{
			GumpCursor c;

			Add(c = new GumpCursor(x, y, cursor, hue, bgID));

			return new Size(c.Width, c.Height);
		}

		public Size AddCursorButton(int x, int y, UOCursor cursor, Action<GumpButton> handler)
		{
			return AddCursorButton(x, y, cursor, 0, 2624, handler);
		}

		public Size AddCursorButton(int x, int y, UOCursor cursor, int hue, Action<GumpButton> handler)
		{
			return AddCursorButton(x, y, cursor, hue, 2624, handler);
		}

		public Size AddCursorButton(int x, int y, UOCursor cursor, int hue, int bgID, Action<GumpButton> handler)
		{
			var s = ArtExtUtility.GetImageSize((int)cursor);

			AddColoredButton(x, y, s.Width, s.Height, Color.Transparent, Color.Transparent, 0, handler);

			if (bgID > 0)
			{
				AddImageTiled(x, y, s.Width, s.Height, bgID);
			}

			return AddCursor(x, y, cursor, hue, bgID);
		}

		public virtual void AddPixel(int x, int y, Color color)
		{
			Add(new GumpPixel(x, y, color));
		}

		public virtual void AddRectangle(int x, int y, int w, int h, Color color)
		{
			Add(new GumpRectangle(x, y, w, h, color));
		}

		public virtual void AddRectangle(int x, int y, int w, int h, Color color, bool filled)
		{
			Add(new GumpRectangle(x, y, w, h, color, filled));
		}

		public virtual void AddRectangle(int x, int y, int w, int h, Color color, int borderSize)
		{
			Add(new GumpRectangle(x, y, w, h, color, borderSize));
		}

		public virtual void AddRectangle(int x, int y, int w, int h, Color fill, Color border, int borderSize)
		{
			Add(new GumpRectangle(x, y, w, h, fill, border, borderSize));
		}

		public virtual void AddRectangle(Rectangle bounds, Color color)
		{
			Add(new GumpRectangle(bounds, color));
		}

		public virtual void AddRectangle(Rectangle bounds, Color color, bool filled)
		{
			Add(new GumpRectangle(bounds, color, filled));
		}

		public virtual void AddRectangle(Rectangle bounds, Color color, int borderSize)
		{
			Add(new GumpRectangle(bounds, color, borderSize));
		}

		public virtual void AddRectangle(Rectangle bounds, Color fill, Color border, int borderSize)
		{
			Add(new GumpRectangle(bounds, fill, border, borderSize));
		}

		public virtual void AddRectangle(Rectangle2D bounds, Color color)
		{
			Add(new GumpRectangle(bounds, color));
		}

		public virtual void AddRectangle(Rectangle2D bounds, Color color, bool filled)
		{
			Add(new GumpRectangle(bounds, color, filled));
		}

		public virtual void AddRectangle(Rectangle2D bounds, Color color, int borderSize)
		{
			Add(new GumpRectangle(bounds, color, borderSize));
		}

		public virtual void AddRectangle(Rectangle2D bounds, Color fill, Color border, int borderSize)
		{
			Add(new GumpRectangle(bounds, fill, border, borderSize));
		}

		public virtual void AddLine(IPoint2D start, IPoint2D end, Color color)
		{
			Add(new GumpLine(start, end, color, 1));
		}

		public virtual void AddLine(IPoint2D start, IPoint2D end, Color color, int size)
		{
			Add(new GumpLine(start, end, color, size));
		}

		public virtual void AddLine(IPoint2D start, Angle angle, int length, Color color)
		{
			Add(new GumpLine(start, angle, length, color, 1));
		}

		public virtual void AddLine(IPoint2D start, Angle angle, int length, Color color, int size)
		{
			Add(new GumpLine(start.X, start.Y, angle, length, color, size));
		}

		public virtual void AddLine(int x, int y, Angle angle, int length, Color color)
		{
			Add(new GumpLine(x, y, angle, length, color, 1));
		}

		public virtual void AddLine(int x, int y, Angle angle, int length, Color color, int size)
		{
			Add(new GumpLine(x, y, angle, length, color, size));
		}

		public virtual void AddGradient(int x, int y, int w, int h, Direction45 to, ColorGradient g)
		{
			Add(new GumpGradient(x, y, w, h, to, g));
		}

		public virtual void AddItemShadow(int x, int y, int itemID, int itemHue, Angle shadowAngle, int shadowOffset)
		{
			Add(new GumpItemShadow(x, y, itemID, itemHue, shadowAngle, shadowOffset));
		}

		public virtual void AddItemShadow(int x, int y, int itemID)
		{
			Add(new GumpItemShadow(x, y, itemID));
		}

		public virtual void AddItemShadow(int x, int y, int itemID, int itemHue)
		{
			Add(new GumpItemShadow(x, y, itemID, itemHue));
		}

		public virtual void AddItemShadow(int x, int y, int itemID, int itemHue, Angle shadowAngle)
		{
			Add(new GumpItemShadow(x, y, itemID, itemHue, shadowAngle));
		}

		public virtual void AddItemShadow(
			int x,
			int y,
			int itemID,
			int itemHue,
			Angle shadowAngle,
			int shadowOffset,
			int shadowHue)
		{
			Add(new GumpItemShadow(x, y, itemID, itemHue, shadowAngle, shadowOffset, shadowHue));
		}

		public virtual void AddImageShadow(int x, int y, int itemID, int itemHue, Angle shadowAngle, int shadowOffset)
		{
			Add(new GumpImageShadow(x, y, itemID, itemHue, shadowAngle, shadowOffset));
		}

		public virtual void AddImageShadow(int x, int y, int itemID)
		{
			Add(new GumpImageShadow(x, y, itemID));
		}

		public virtual void AddImageShadow(int x, int y, int itemID, int itemHue)
		{
			Add(new GumpImageShadow(x, y, itemID, itemHue));
		}

		public virtual void AddImageShadow(int x, int y, int itemID, int itemHue, Angle shadowAngle)
		{
			Add(new GumpImageShadow(x, y, itemID, itemHue, shadowAngle));
		}

		public virtual void AddImageShadow(
			int x,
			int y,
			int itemID,
			int itemHue,
			Angle shadowAngle,
			int shadowOffset,
			int shadowHue)
		{
			Add(new GumpImageShadow(x, y, itemID, itemHue, shadowAngle, shadowOffset, shadowHue));
		}

		public virtual void AddProgress(
			int x,
			int y,
			int w,
			int h,
			double progress,
			Direction dir = Direction.Right,
			Color? background = null,
			Color? foreground = null,
			Color? border = null,
			int borderSize = 0)
		{
			Add(new GumpProgress(x, y, w, h, progress, dir, background, foreground, border, borderSize));
		}

		public virtual void AddPaperdoll(int x, int y, bool props, Mobile m)
		{
			Add(new GumpPaperdoll(x, y, props, m));
		}

		public virtual void AddPaperdoll(
			int x,
			int y,
			bool props,
			List<Item> items,
			Body body,
			int bodyHue,
			int solidHue,
			int hairID,
			int hairHue,
			int facialHairID,
			int facialHairHue)
		{
			Add(new GumpPaperdoll(x, y, props, items, body, bodyHue, solidHue, hairID, hairHue, facialHairID, facialHairHue));
		}

		public virtual void AddClock(
			int x,
			int y,
			DateTime time,
			bool background = true,
			int backgroundHue = 0,
			bool face = true,
			int faceHue = 0,
			bool numerals = true,
			bool numbers = true,
			Color? numbersColor = null,
			bool hours = true,
			Color? hoursColor = null,
			bool minutes = true,
			Color? minutesColor = null,
			bool seconds = true,
			Color? secondsColor = null)
		{
			Add(
				new GumpClock(
					x,
					y,
					time,
					background,
					backgroundHue,
					face,
					faceHue,
					numerals,
					numbers,
					numbersColor,
					hours,
					hoursColor,
					minutes,
					minutesColor,
					seconds,
					secondsColor));
		}

		public virtual void AddImageNumber(int x, int y, int value, int hue = 0, Axis centering = Axis.None)
		{
			Add(new GumpImageNumber(x, y, value, hue, centering));
		}

		public virtual void AddImageTime(int x, int y, TimeSpan value, int hue = 0, Axis centering = Axis.None)
		{
			Add(new GumpImageTime(x, y, value, hue, centering));
		}

		public virtual void AddEnumSelect<TEnum>(
			int x,
			int y,
			int normalID,
			int pressedID,
			int labelXOffset,
			int labelYOffset,
			int labelWidth,
			int labelHeight,
			int labelHue,
			TEnum selected,
			Action<TEnum> onSelect)
			where TEnum : struct, IComparable, IFormattable, IConvertible
		{
			AddEnumSelect(
				x,
				y,
				normalID,
				pressedID,
				labelXOffset,
				labelYOffset,
				labelWidth,
				labelHeight,
				labelHue,
				selected,
				onSelect,
				true);
		}

		public virtual void AddEnumSelect<TEnum>(
			int x,
			int y,
			int normalID,
			int pressedID,
			int labelXOffset,
			int labelYOffset,
			int labelWidth,
			int labelHeight,
			int labelHue,
			TEnum selected,
			Action<TEnum> onSelect,
			bool resolveMenuPos)
			where TEnum : struct, IComparable, IFormattable, IConvertible
		{
			if (!typeof(TEnum).IsEnum)
			{
				return;
			}

			var opts = new MenuGumpOptions();

			ListGumpEntry? def = null;

			foreach (var o in (default(TEnum) as Enum).EnumerateValues<TEnum>(false))
			{
				var v = o;
				var e = new ListGumpEntry(v.ToString(), b => onSelect(v));

				opts.AppendEntry(e);

				if (Equals(v, selected))
				{
					def = e;
				}
			}

			if (def != null)
			{
				AddMenuButton(
					x,
					y,
					normalID,
					pressedID,
					labelXOffset,
					labelYOffset,
					labelWidth,
					labelHeight,
					labelHue,
					opts,
					def.Value,
					resolveMenuPos);
			}
		}

		public virtual void AddMenuButton(
			int x,
			int y,
			int normalID,
			int pressedID,
			int labelXOffset,
			int labelYOffset,
			int labelWidth,
			int labelHeight,
			int labelHue,
			MenuGumpOptions opts,
			ListGumpEntry defSelection)
		{
			AddMenuButton(
				x,
				y,
				normalID,
				pressedID,
				labelXOffset,
				labelYOffset,
				labelWidth,
				labelHeight,
				labelHue,
				opts,
				defSelection,
				true);
		}

		public virtual void AddMenuButton(
			int x,
			int y,
			int normalID,
			int pressedID,
			int labelXOffset,
			int labelYOffset,
			int labelWidth,
			int labelHeight,
			int labelHue,
			MenuGumpOptions opts,
			ListGumpEntry defSelection,
			bool resolveMenuPos)
		{
			AddInputEC();

			AddButton(x, y, normalID, pressedID, b => Send(new MenuGump(User, Refresh(), opts, resolveMenuPos ? b : null)));
			AddLabel(x + labelXOffset, y + labelYOffset, labelHue, defSelection.Label ?? String.Empty);

			AddInputEC();
		}

		public virtual void AddScrollbar(
			Axis axis, int x, int y, int range, int value,
			Action<GumpButton> prev, Action<GumpButton> next,
			int trackX, int trackY, int trackW, int trackH,
			int trackBackgroundID, int trackForegroundID,
			int prevX, int prevY, int prevW, int prevH,
			int prevDisplayID, int prevPressedID, int prevDisabledID,
			int nextX, int nextY, int nextW, int nextH,
			int nextDisplayID, int nextPressedID, int nextDisabledID,
			bool toolTips = true)
		{
			AddScrollbarM(
				axis, x, y, range, value,
				(b, n) => prev(b), (b, n) => next(b),
				trackX, trackY, trackW, trackH,
				trackBackgroundID, trackForegroundID,
				prevX, prevY, prevW, prevH,
				prevDisplayID, prevPressedID, prevDisabledID,
				nextX, nextY, nextW, nextH,
				nextDisplayID, nextPressedID, nextDisabledID,
				toolTips);
		}

		public virtual void AddScrollbar(
			Axis axis, int x, int y, int range, int value,
			Action<GumpButton> prev, Action<GumpButton> next,
			Rectangle trackBounds, Rectangle prevBounds, Rectangle nextBounds,
			Tuple<int, int> trackIDs = null,
			Tuple<int, int, int> prevIDs = null,
			Tuple<int, int, int> nextIDs = null,
			bool toolTips = true)
		{
			AddScrollbarM(
				axis, x, y, range, value,
				(b, n) => prev(b), (b, n) => next(b),
				trackBounds, prevBounds, nextBounds,
				trackIDs, prevIDs, nextIDs,
				toolTips);
		}

		public virtual void AddScrollbarM(
			Axis axis, int x, int y, int range, int value,
			Action<GumpButton, int> prev, Action<GumpButton, int> next,
			int trackX, int trackY, int trackW, int trackH,
			int trackBackgroundID, int trackForegroundID,
			int prevX, int prevY, int prevW, int prevH,
			int prevDisplayID, int prevPressedID, int prevDisabledID,
			int nextX, int nextY, int nextW, int nextH,
			int nextDisplayID, int nextPressedID, int nextDisabledID,
			bool toolTips = true)
		{
			AddScrollbarM(
				axis, x, y, range, value,
				prev, next,
				new Rectangle(trackX, trackY, trackW, trackH),
				new Rectangle(prevX, prevY, prevW, prevH),
				new Rectangle(nextX, nextY, nextW, nextH),
				Tuple.Create(trackBackgroundID, trackForegroundID),
				Tuple.Create(prevDisplayID, prevPressedID, prevDisabledID),
				Tuple.Create(nextDisplayID, nextPressedID, nextDisabledID),
				toolTips);
		}

		public virtual void AddScrollbarM(Axis axis, int x, int y, int range, int value,
			Action<GumpButton, int> prev, Action<GumpButton, int> next,
			Rectangle trackBounds, Rectangle prevBounds, Rectangle nextBounds,
			Tuple<int, int> trackIDs = null,
			Tuple<int, int, int> prevIDs = null,
			Tuple<int, int, int> nextIDs = null,
			bool toolTips = true)
		{
			switch (axis)
			{
				case Axis.Vertical:
					AddScrollbarVM(x, y, range, value, prev, next, trackBounds, prevBounds, nextBounds, trackIDs, prevIDs, nextIDs, toolTips);
					break;
				case Axis.Horizontal:
					AddScrollbarHM(x, y, range, value, prev, next, trackBounds, prevBounds, nextBounds, trackIDs, prevIDs, nextIDs, toolTips);
					break;
			}
		}

		public virtual void AddScrollbarV(
			int x, int y, int h, int range, int value,
			Action<GumpButton> prev, Action<GumpButton> next,
			int trackBackgroundID, int trackForegroundID,
			int prevDisplayID, int prevPressedID, int prevDisabledID,
			int nextDisplayID, int nextPressedID, int nextDisabledID,
			bool toolTips = true)
		{
			AddScrollbarVM(
				x, y, h, range, value,
				(b, n) => prev(b), (b, n) => next(b),
				trackBackgroundID, trackForegroundID,
				prevDisplayID, prevPressedID, prevDisabledID,
				nextDisplayID, nextPressedID, nextDisabledID,
				toolTips);
		}

		public virtual void AddScrollbarV(
			int x, int y, int h, int range, int value,
			Action<GumpButton> prev, Action<GumpButton> next,
			Tuple<int, int> trackIDs = null,
			Tuple<int, int, int> prevIDs = null,
			Tuple<int, int, int> nextIDs = null,
			bool toolTips = true)
		{
			AddScrollbarVM(
				x, y, h, range, value,
				(b, n) => prev(b), (b, n) => next(b),
				trackIDs, prevIDs, nextIDs,
				toolTips);
		}

		public virtual void AddScrollbarV(
			int x, int y, int range, int value,
			Action<GumpButton> prev, Action<GumpButton> next,
			Rectangle trackBounds, Rectangle prevBounds, Rectangle nextBounds,
			Tuple<int, int> trackIDs = null,
			Tuple<int, int, int> prevIDs = null,
			Tuple<int, int, int> nextIDs = null,
			bool toolTips = true)
		{
			AddScrollbarVM(
				x, y, range, value,
				(b, n) => prev(b), (b, n) => next(b),
				trackBounds, prevBounds, nextBounds,
				trackIDs, prevIDs, nextIDs,
				toolTips);
		}

		public virtual void AddScrollbarVM(
			int x, int y, int h, int range, int value,
			Action<GumpButton, int> prev, Action<GumpButton, int> next,
			int trackBackgroundID, int trackForegroundID,
			int prevDisplayID, int prevPressedID, int prevDisabledID,
			int nextDisplayID, int nextPressedID, int nextDisabledID,
			bool toolTips = true)
		{
			AddScrollbarVM(
				x, y, h, range, value,
				prev, next,
				Tuple.Create(trackBackgroundID, trackForegroundID),
				Tuple.Create(prevDisplayID, prevPressedID, prevDisabledID),
				Tuple.Create(nextDisplayID, nextPressedID, nextDisabledID),
				toolTips);
		}

		public virtual void AddScrollbarVM(
			int x, int y, int h, int range, int value,
			Action<GumpButton, int> prev, Action<GumpButton, int> next,
			Tuple<int, int> trackIDs = null,
			Tuple<int, int, int> prevIDs = null,
			Tuple<int, int, int> nextIDs = null,
			bool toolTips = true)
		{
			trackIDs = trackIDs ?? new Tuple<int, int>(87, 9394);
			prevIDs = prevIDs ?? new Tuple<int, int, int>(2361, 2362, 2360);
			nextIDs = nextIDs ?? new Tuple<int, int, int>(2361, 2362, 2360);

			Func<Size[], Size> evalW = o => o.Highest(s => s.Width);
			Func<Size[], Size> evalH = o => o.Highest(s => s.Height);

			var sizes = new[]
			{
				new[]
				{
					GumpsExtUtility.GetImageSize(trackIDs.Item1),
					GumpsExtUtility.GetImageSize(trackIDs.Item2)
				},
				new[]
				{
					GumpsExtUtility.GetImageSize(prevIDs.Item1),
					GumpsExtUtility.GetImageSize(prevIDs.Item2),
					GumpsExtUtility.GetImageSize(prevIDs.Item3)
				},
				new[]
				{
					GumpsExtUtility.GetImageSize(nextIDs.Item1),
					GumpsExtUtility.GetImageSize(nextIDs.Item2),
					GumpsExtUtility.GetImageSize(nextIDs.Item3)
				}
			};

			var ts = evalH(sizes[0]);
			var ps = evalH(sizes[1]);
			var ns = evalH(sizes[2]);

			var ms = evalW(new[] { ps, ns });

			if (ts.Width > ms.Width)
			{
				ts.Width = ms.Width;
			}

			h -= ps.Height + ns.Height;

			var trackBounds = new Rectangle((ms.Width - ts.Width) / 2, ps.Height, ts.Width, h);
			var prevBounds = new Rectangle((ms.Width - ps.Width) / 2, 0, ps.Width, ps.Height);
			var nextBounds = new Rectangle((ms.Width - ns.Width) / 2, ps.Height + h, ns.Width, ns.Height);

			AddScrollbarVM(x, y, range, value, prev, next, trackBounds, prevBounds, nextBounds, trackIDs, prevIDs, nextIDs, toolTips);
		}

		public virtual void AddScrollbarVM(
			int x, int y, int range, int value,
			Action<GumpButton, int> prev, Action<GumpButton, int> next,
			Rectangle trackBounds, Rectangle prevBounds, Rectangle nextBounds,
			Tuple<int, int> trackIDs = null,
			Tuple<int, int, int> prevIDs = null,
			Tuple<int, int, int> nextIDs = null,
			bool toolTips = true)
		{
			trackIDs = trackIDs ?? new Tuple<int, int>(87, 9394);
			prevIDs = prevIDs ?? new Tuple<int, int, int>(2361, 2362, 2360);
			nextIDs = nextIDs ?? new Tuple<int, int, int>(2361, 2362, 2360);

			range = Math.Max(1, range);
			value = Math.Max(0, Math.Min(range - 1, value));

			var bh = Math.Min(trackBounds.Height, Math.Max(13, trackBounds.Height / (double)range));
			var by = Math.Min(trackBounds.Height, Math.Max(bh, trackBounds.Height * ((value + 1) / (double)range))) - bh;

			var barBounds = new Rectangle(trackBounds.X, trackBounds.Y + (int)by, trackBounds.Width, (int)bh);

			if (value > 0)
			{
				AddButton(x + prevBounds.X, y + prevBounds.Y, prevIDs.Item1, prevIDs.Item2, b => prev(b, 1));

				if (toolTips)
				{
					AddTooltip(1011067);
				}
			}
			else if (prevIDs.Item3 > 0)
			{
				AddImage(x + prevBounds.X, y + prevBounds.Y, prevIDs.Item3);
			}

			if (range > 1)
			{
				AddInputEC();

				var ps = GetImageSize(prevIDs.Item1);
				var ph = barBounds.Y + (barBounds.Height / 2);

				AddTileButton(x + trackBounds.X, y + trackBounds.Y, trackBounds.Width, ph, prevIDs.Item1, prevIDs.Item1, b =>
				{
					var offset = (b.Y - (y + trackBounds.Y)) / (double)(trackBounds.Height - ps.Height);

					var index = (int)Math.Round(offset * range);

					if (index < value)
					{
						prev(b, value - index);
					}
					else
					{
						Refresh();
					}
				});

				var ns = GetImageSize(nextIDs.Item1);
				var nh = trackBounds.Height - ph;

				AddTileButton(x + trackBounds.X, y + trackBounds.Y + ph, trackBounds.Width, nh, nextIDs.Item1, nextIDs.Item1, b =>
				{
					var offset = (b.Y - (y + trackBounds.Y)) / (double)(trackBounds.Height - ns.Height);

					var index = (int)Math.Round(offset * range);

					if (index > value)
					{
						next(b, index - value);
					}
					else
					{
						Refresh();
					}
				});

				AddImageTiled(x + trackBounds.X, y + trackBounds.Y, trackBounds.Width, trackBounds.Height, trackIDs.Item1);

				AddInputEC();
			}
			else
			{
				AddImageTiled(x + trackBounds.X, y + trackBounds.Y, trackBounds.Width, trackBounds.Height, trackIDs.Item1);
			}

			if (range > 1)
			{
				AddImageTiled(x + barBounds.X, y + barBounds.Y, barBounds.Width, barBounds.Height, trackIDs.Item2);
			}

			if (value + 1 < range)
			{
				AddButton(x + nextBounds.X, y + nextBounds.Y, nextIDs.Item1, nextIDs.Item2, b => next(b, 1));

				if (toolTips)
				{
					AddTooltip(1011066);
				}
			}
			else if (nextIDs.Item3 > 0)
			{
				AddImage(x + nextBounds.X, y + nextBounds.Y, nextIDs.Item3);
			}
		}

		public virtual void AddScrollbarH(
			int x, int y, int w, int range, int value,
			Action<GumpButton> prev, Action<GumpButton> next,
			int trackBackgroundID, int trackForegroundID,
			int prevDisplayID, int prevPressedID, int prevDisabledID,
			int nextDisplayID, int nextPressedID, int nextDisabledID,
			bool toolTips = true)
		{
			AddScrollbarHM(
				x, y, w, range, value,
				(b, n) => prev(b), (b, n) => next(b),
				trackBackgroundID, trackForegroundID,
				prevDisplayID, prevPressedID, prevDisabledID,
				nextDisplayID, nextPressedID, nextDisabledID,
				toolTips);
		}

		public virtual void AddScrollbarH(
			int x, int y, int w, int range, int value,
			Action<GumpButton> prev, Action<GumpButton> next,
			Tuple<int, int> trackIDs = null,
			Tuple<int, int, int> prevIDs = null,
			Tuple<int, int, int> nextIDs = null,
			bool toolTips = true)
		{
			AddScrollbarHM(
				x, y, w, range, value,
				(b, n) => prev(b), (b, n) => next(b),
				trackIDs, prevIDs, nextIDs,
				toolTips);
		}

		public virtual void AddScrollbarH(
			int x, int y, int range, int value,
			Action<GumpButton> prev, Action<GumpButton> next,
			Rectangle trackBounds, Rectangle prevBounds, Rectangle nextBounds,
			Tuple<int, int> trackIDs = null,
			Tuple<int, int, int> prevIDs = null,
			Tuple<int, int, int> nextIDs = null,
			bool toolTips = true)
		{
			AddScrollbarHM(
				x, y, range, value,
				(b, n) => prev(b), (b, n) => next(b),
				trackBounds, prevBounds, nextBounds,
				trackIDs, prevIDs, nextIDs,
				toolTips);
		}

		public virtual void AddScrollbarHM(
			int x, int y, int w, int range, int value,
			Action<GumpButton, int> prev, Action<GumpButton, int> next,
			int trackBackgroundID, int trackForegroundID,
			int prevDisplayID, int prevPressedID, int prevDisabledID,
			int nextDisplayID, int nextPressedID, int nextDisabledID,
			bool toolTips = true)
		{
			AddScrollbarHM(
				x, y, w, range, value,
				prev, next,
				Tuple.Create(trackBackgroundID, trackForegroundID),
				Tuple.Create(prevDisplayID, prevPressedID, prevDisabledID),
				Tuple.Create(nextDisplayID, nextPressedID, nextDisabledID),
				toolTips);
		}

		public virtual void AddScrollbarHM(
			int x, int y, int w, int range, int value,
			Action<GumpButton, int> prev, Action<GumpButton, int> next,
			Tuple<int, int> trackIDs = null,
			Tuple<int, int, int> prevIDs = null,
			Tuple<int, int, int> nextIDs = null,
			bool toolTips = true)
		{
			trackIDs = trackIDs ?? new Tuple<int, int>(87, 9394);
			prevIDs = prevIDs ?? new Tuple<int, int, int>(2361, 2362, 2360);
			nextIDs = nextIDs ?? new Tuple<int, int, int>(2361, 2362, 2360);

			Func<Size[], Size> evalW = o => o.Highest(s => s.Width);
			Func<Size[], Size> evalH = o => o.Highest(s => s.Height);

			var sizes = new[]
			{
				new[]
				{
					GumpsExtUtility.GetImageSize(trackIDs.Item1),
					GumpsExtUtility.GetImageSize(trackIDs.Item2)
				},
				new[]
				{
					GumpsExtUtility.GetImageSize(prevIDs.Item1),
					GumpsExtUtility.GetImageSize(prevIDs.Item2),
					GumpsExtUtility.GetImageSize(prevIDs.Item3)
				},
				new[]
				{
					GumpsExtUtility.GetImageSize(nextIDs.Item1),
					GumpsExtUtility.GetImageSize(nextIDs.Item2),
					GumpsExtUtility.GetImageSize(nextIDs.Item3)
				}
			};

			var ts = evalW(sizes[0]);
			var ps = evalW(sizes[1]);
			var ns = evalW(sizes[2]);

			var ms = evalH(new[] { ps, ns });

			if (ts.Height > ms.Height)
			{
				ts.Height = ms.Height;
			}

			w -= ps.Width + ns.Width;

			var trackBounds = new Rectangle(ps.Width, (ms.Height - ts.Height) / 2, w, ts.Height);
			var prevBounds = new Rectangle(0, (ms.Height - ps.Height) / 2, ps.Width, ps.Height);
			var nextBounds = new Rectangle(ps.Width + w, (ms.Height - ns.Height) / 2, ns.Width, ns.Height);

			AddScrollbarHM(x, y, range, value, prev, next, trackBounds, prevBounds, nextBounds, trackIDs, prevIDs, nextIDs, toolTips);
		}

		public virtual void AddScrollbarHM(
			int x, int y, int range, int value,
			Action<GumpButton, int> prev, Action<GumpButton, int> next,
			Rectangle trackBounds, Rectangle prevBounds, Rectangle nextBounds,
			Tuple<int, int> trackIDs = null,
			Tuple<int, int, int> prevIDs = null,
			Tuple<int, int, int> nextIDs = null,
			bool toolTips = true)
		{
			trackIDs = trackIDs ?? new Tuple<int, int>(87, 9394);
			prevIDs = prevIDs ?? new Tuple<int, int, int>(2361, 2362, 2360);
			nextIDs = nextIDs ?? new Tuple<int, int, int>(2361, 2362, 2360);

			range = Math.Max(1, range);
			value = Math.Max(0, Math.Min(range, value));

			var bw = Math.Min(trackBounds.Width, Math.Max(13, trackBounds.Width / (double)range));
			var bx = Math.Min(trackBounds.Width, Math.Max(bw, trackBounds.Width * ((value + 1) / (double)range))) - bw;

			var barBounds = new Rectangle(trackBounds.X + (int)bx, trackBounds.Y, (int)bw, trackBounds.Height);

			if (value > 0)
			{
				AddButton(x + prevBounds.X, y + prevBounds.Y, prevIDs.Item1, prevIDs.Item2, b => prev(b, 1));

				if (toolTips)
				{
					AddTooltip(1011067);
				}
			}
			else
			{
				AddImage(x + prevBounds.X, y + prevBounds.Y, prevIDs.Item3);
			}

			if (range > 1)
			{
				AddInputEC();

				var ps = GetImageSize(prevIDs.Item1);
				var pw = barBounds.X + (barBounds.Width / 2);

				AddTileButton(x + trackBounds.X, y + trackBounds.Y, pw, trackBounds.Height, prevIDs.Item1, prevIDs.Item1, b =>
				{
					var offset = (b.X - (x + trackBounds.X)) / (double)(trackBounds.Width - ps.Width);

					var index = (int)Math.Round(offset * range);

					if (index < value)
					{
						prev(b, value - index);
					}
					else
					{
						Refresh();
					}
				});

				var ns = GetImageSize(nextIDs.Item1);
				var nw = trackBounds.Width - pw;

				AddTileButton(x + trackBounds.X + pw, y + trackBounds.Y, nw, trackBounds.Height, nextIDs.Item1, nextIDs.Item1, b =>
				{
					var offset = (b.X - (x + trackBounds.X)) / (double)(trackBounds.Width - ns.Width);

					var index = (int)Math.Round(offset * range);

					if (index > value)
					{
						next(b, index - value);
					}
					else
					{
						Refresh();
					}
				});

				AddImageTiled(x + trackBounds.X, y + trackBounds.Y, trackBounds.Width, trackBounds.Height, trackIDs.Item1);

				AddInputEC();
			}
			else
			{
				AddImageTiled(x + trackBounds.X, y + trackBounds.Y, trackBounds.Width, trackBounds.Height, trackIDs.Item1);
			}

			if (range > 1)
			{
				AddImageTiled(x + barBounds.X, y + barBounds.Y, barBounds.Width, barBounds.Height, trackIDs.Item2);
			}

			if (value + 1 < range)
			{
				AddButton(x + nextBounds.X, y + nextBounds.Y, nextIDs.Item1, nextIDs.Item2, b => next(b, 1));

				if (toolTips)
				{
					AddTooltip(1011066);
				}
			}
			else
			{
				AddImage(x + nextBounds.X, y + nextBounds.Y, nextIDs.Item3);
			}
		}

		private static readonly Regex _FontTagsRegex = new Regex(
			@"</?basefont[^>]*>",
			RegexOptions.IgnoreCase | RegexOptions.Singleline);

		private static readonly Regex _FontColorRegex = new Regex(
			@"<basefont color[^#\w]*(?<val>#\w*)[^>]*>",
			RegexOptions.IgnoreCase | RegexOptions.Singleline);

		public new void AddHtml(int x, int y, int w, int h, string label, bool bg, bool scroll)
		{
			if (IsEnhancedClient && !String.IsNullOrWhiteSpace(label))
			{
				var color = Color.Empty;

				try
				{
					var match = _FontColorRegex.Match(label);

					if (match.Success)
					{
						var val = match.Groups["val"].Value.Replace("\"", String.Empty).Trim();
						var hex = val.IndexOf("#", StringComparison.Ordinal) >= 0;

						if (hex)
						{
							val = val.Replace("#", String.Empty);


							if (Int32.TryParse(val, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var argb))
							{
								color = Color.FromArgb(argb);
							}
						}
						else if (val.Any(Char.IsNumber))
						{

							if (Int32.TryParse(val, out var argb))
							{
								color = Color.FromArgb(argb);
							}
						}
						else
						{

							if (Enum.TryParse(val, true, out KnownColor kcol))
							{
								color = Color.FromKnownColor(kcol);
							}
						}
					}
				}
				finally
				{
					if (color.IsEmpty || color == Color.Transparent)
					{
						color = DefaultHtmlColor;
					}

					label = _FontTagsRegex.Replace(label, String.Empty);
					label = label.WrapUOHtmlColor(color, false);
				}
			}

			base.AddHtml(x, y, w, h, label, bg, scroll);
		}

		public void AddHtml(int x, int y, int w, int h, string label, Color labelColor, Color fillColor)
		{
			AddHtml(x, y, w, h, label, labelColor, fillColor, Color.Empty, 0);
		}

		public void AddHtml(
			int x,
			int y,
			int w,
			int h,
			string label,
			Color labelColor,
			Color fillColor,
			Color borderColor,
			int borderSize)
		{
			if (w * h <= 0)
			{
				return;
			}

			if (labelColor.IsEmpty)
			{
				labelColor = DefaultHtmlColor;
			}

			if (borderColor.IsEmpty)
			{
				borderSize = 0;
			}

			AddRectangle(x, y, w, h, fillColor, borderColor, borderSize);

			if (borderSize > 0)
			{
				x += borderSize;
				y += borderSize;
				w -= borderSize * 2;
				h -= borderSize * 2;
			}

			if (w * h > 0 && !String.IsNullOrWhiteSpace(label))
			{
				var f = UOFont.Unicode[1];
				var t = label.StripHtmlBreaks(true).StripHtml();

				var s = Size.Empty;

				foreach (var line in t.Split('\n'))
				{
					var ls = f.GetSize(line);

					var count = (int)Math.Ceiling(ls.Width / (double)w);

					if (count > 1 && Regex.IsMatch(line, @"[\D\W\s]+", RegexOptions.Compiled))
					{
						ls.Height *= count;
					}

					s.Width = Math.Max(s.Width, Math.Min(w, ls.Width));
					s.Height += ls.Height;
				}

				if (s.Height < h)
				{
					ComputeCenter(ref y, h - s.Height);
				}

				h = Math.Max(40, h);

				AddHtml(x, y, w, h, label.WrapUOHtmlColor(labelColor, false), false, false);
			}
		}

		private static readonly int[,] _HtmlButtonSizes =
			{{87, 16}, {11280, 20}, {10460, 30}, {2240, 44}, {1464, 50}, {7000, 70}, {1417, 80}};

		public void AddHtmlButton(
			int x,
			int y,
			int w,
			int h,
			Action<GumpButton> handler,
			string label,
			Color labelColor,
			Color fillColor)
		{
			AddHtmlButton(x, y, w, h, handler, label, labelColor, fillColor, Color.Empty, 0);
		}

		public void AddHtmlButton(
			int x,
			int y,
			int w,
			int h,
			Action<GumpButton> handler,
			string label,
			Color labelColor,
			Color fillColor,
			Color borderColor,
			int borderSize)
		{
			if (w * h <= 0)
			{
				return;
			}

			if (labelColor.IsEmpty)
			{
				labelColor = DefaultHtmlColor;
			}

			if (fillColor.IsEmpty)
			{
				fillColor = Color.Black;
			}

			if (borderColor.IsEmpty)
			{
				borderSize = 0;
			}

			var bi = 87;
			var bs = 16;

			if (!IsEnhancedClient)
			{
				for (var i = 0; i < _HtmlButtonSizes.GetLength(0); i++)
				{
					if (w < _HtmlButtonSizes[i, 1] || h < _HtmlButtonSizes[i, 1])
					{
						break;
					}

					bi = _HtmlButtonSizes[i, 0];
					bs = _HtmlButtonSizes[i, 1];
				}
			}

			w = Math.Max(bs, w);
			h = Math.Max(bs, h);

			var cols = (int)Math.Ceiling(w / (double)bs);
			var rows = (int)Math.Ceiling(h / (double)bs);

			int c = 0, r = 0, xo = x, yo = y, wo = x + w, ho = y + h;

			AddInputEC();

			while (c++ < cols)
			{
				if (xo + bs > wo)
				{
					xo = wo - bs;
				}

				if (handler != null)
				{
					AddButton(xo, yo, bi, bi, handler);
				}

				xo += bs;

				if (c % cols == 0)
				{
					xo = x;
					yo += bs;

					if (yo + bs > ho)
					{
						yo = ho - bs;
					}

					if (r++ < rows)
					{
						c = 0;
					}
					else
					{
						break;
					}
				}
			}

			AddHtml(x, y, w, h, label, labelColor, fillColor, borderColor, borderSize);

			AddInputEC();
		}

		public void AddColoredButton(int x, int y, int w, int h, Color fillColor, Action<GumpButton> handler)
		{
			AddColoredButton(x, y, w, h, fillColor, Color.Empty, 0, handler);
		}

		public void AddColoredButton(
			int x,
			int y,
			int w,
			int h,
			Color fillColor,
			Color borderColor,
			int borderSize,
			Action<GumpButton> handler)
		{
			if (w * h <= 0)
			{
				return;
			}

			if (fillColor.IsEmpty)
			{
				fillColor = Color.Black;
			}

			if (borderColor.IsEmpty)
			{
				borderSize = 0;
			}

			var bi = 87;
			var bs = 16;

			if (!IsEnhancedClient)
			{
				for (var i = 0; i < _HtmlButtonSizes.GetLength(0); i++)
				{
					if (w < _HtmlButtonSizes[i, 1] || h < _HtmlButtonSizes[i, 1])
					{
						break;
					}

					bi = _HtmlButtonSizes[i, 0];
					bs = _HtmlButtonSizes[i, 1];
				}
			}

			w = Math.Max(bs, w);
			h = Math.Max(bs, h);

			var cols = (int)Math.Ceiling(w / (double)bs);
			var rows = (int)Math.Ceiling(h / (double)bs);

			int c = 0, r = 0, xo = x, yo = y, wo = x + w, ho = y + h;

			AddInputEC();

			while (c++ < cols)
			{
				if (xo + bs > wo)
				{
					xo = wo - bs;
				}

				AddButton(xo, yo, bi, bi, handler);

				xo += bs;

				if (c % cols == 0)
				{
					xo = x;
					yo += bs;

					if (yo + bs > ho)
					{
						yo = ho - bs;
					}

					if (r++ < rows)
					{
						c = 0;
					}
					else
					{
						break;
					}
				}
			}

			AddRectangle(x, y, w, h, fillColor, borderColor, borderSize);

			AddInputEC();
		}

		public void AddTileButton(int x, int y, int w, int h, Action<GumpButton> handler)
		{
			AddTileButton(x, y, w, h, 87, handler);
		}

		public void AddTileButton(int x, int y, int w, int h, int imageID, Action<GumpButton> handler)
		{
			AddTileButton(x, y, w, h, imageID, imageID, handler);
		}

		public void AddTileButton(int x, int y, int w, int h, int nID, int pID, Action<GumpButton> handler)
		{
			if (w * h <= 0)
			{
				return;
			}

			var bs = GetImageSize(nID);

			if (bs.IsEmpty)
			{
				return;
			}

			int bw = bs.Width, bh = bs.Height;

			w = Math.Max(bw, w);
			h = Math.Max(bh, h);

			var cols = (int)Math.Ceiling(w / (double)bw);
			var rows = (int)Math.Ceiling(h / (double)bh);

			int c = 0, r = 0, xo = x, yo = y, wo = x + w, ho = y + h;

			while (c++ < cols)
			{
				if (xo + bw > wo)
				{
					xo = wo - bw;
				}

				AddButton(xo, yo, nID, pID, handler);

				xo += bw;

				if (c % cols == 0)
				{
					xo = x;
					yo += bh;

					if (yo + bh > ho)
					{
						yo = ho - bh;
					}

					if (r++ < rows)
					{
						c = 0;
					}
					else
					{
						break;
					}
				}
			}
		}

		private Dictionary<int, object> _Controls;

		public TEnum? GetSelection<TEnum>(IEnumerable<TEnum> renderer)
			where TEnum : struct, IComparable, IFormattable, IConvertible
		{
			return _Controls.GetValue(renderer.GetHashCode()) as TEnum?;
		}

		public void AddSelection<T>(
			int x,
			int y,
			int w,
			int h,
			T initValue,
			ICollection<T> values,
			Action<T> onSelect,
			Func<T, string> getLabel)
		{
			AddSelection(x, y, w, h, initValue, values, onSelect, getLabel, true, true, false, Color.White, Color.Black);
		}

		public void AddSelection<T>(
			int x,
			int y,
			int w,
			int h,
			T initValue,
			ICollection<T> values,
			Action<T> onSelect,
			Func<T, string> getLabel,
			Color color,
			Color fill)
		{
			AddSelection(x, y, w, h, initValue, values, onSelect, getLabel, true, true, false, color, fill);
		}

		public void AddSelection<T>(
			int x,
			int y,
			int w,
			int h,
			T initValue,
			ICollection<T> values,
			Action<T> onSelect,
			Func<T, string> getLabel,
			bool left,
			bool right,
			bool cycle,
			Color color,
			Color fill)
		{
			if (w * h <= 0 || onSelect == null)
			{
				return;
			}

			if (values == null || values.Count == 0)
			{
				return;
			}

			if (!left && !right && !cycle)
			{
				return;
			}

			var hash = onSelect.GetHashCode();
			var value = _Controls.GetValue(hash);

			if (!(value is T) || !values.Contains((T)value))
			{
				value = initValue;
			}

			var idx = values.IndexOf((T)value);
			var prev = cycle ? ((idx <= 0 ? values.Count : idx) - 1) : Math.Max(0, idx - 1);
			var next = cycle ? ((idx + 1) % values.Count) : Math.Min(idx, values.Count - 1);

			var bp = new Action<GumpButton>(
				b =>
				{
					var v = values.ElementAtOrDefault(prev);

					_Controls[hash] = (value = v) ?? (v = initValue);

					onSelect(v);

					Refresh(true);
				});

			var bn = new Action<GumpButton>(
				b =>
				{
					var v = values.ElementAtOrDefault(next);

					_Controls[hash] = (value = v) ?? (v = initValue);

					onSelect(v);

					Refresh(true);
				});

			if (left)
			{
				if (values.Count > 1 && idx != prev)
				{
					var t = UniGlyph.TriLeftFill.ToString().WrapUOHtmlCenter();

					AddHtmlButton(x, y, 20, h, bp, t, color, fill);
				}
				else
				{
					var t = UniGlyph.TriLeftEmpty.ToString().WrapUOHtmlCenter();

					AddHtml(x, y, 20, h, t, color, fill);
				}
			}

			if (right)
			{
				if (values.Count > 1 && idx != next)
				{
					var t = UniGlyph.TriRightFill.ToString().WrapUOHtmlCenter();

					AddHtmlButton(x + (w - 20), y, 20, h, bn, t, color, fill);
				}
				else
				{
					var t = UniGlyph.TriRightEmpty.ToString().WrapUOHtmlCenter();

					AddHtml(x + (w - 20), y, 20, h, t, color, fill);
				}
			}

			if (left)
			{
				x += 20;
				w -= 20;
			}

			if (right)
			{
				w -= 20;
			}

			var l = value.ToString();

			if (getLabel != null)
			{
				l = getLabel((T)value);
			}
			else if (value is Mobile)
			{
				l = ((Mobile)value).Name;
			}
			else if (value is Item)
			{
				l = ((Item)value).ResolveName(User);
			}
			else if (value is Enum)
			{
				l = ((Enum)value).ToString(true);
			}

			l = l.WrapUOHtmlCenter();

			if (!left && !right && values.Count > 1 && idx != next)
			{
				AddHtmlButton(x, y, w, h, bn, l, color, fill);
			}
			else
			{
				AddHtml(x, y, w, h, l, color, fill);
			}
		}

		public bool HasAccordion<T>(Action<int, int, int, int, T> renderer)
		{
			return _Controls.ContainsKey(renderer.GetHashCode());
		}

		public T GetAccordion<T>(Action<int, int, int, int, T> renderer)
		{
			var o = _Controls.GetValue(renderer.GetHashCode());

			if (o is T)
			{
				return (T)o;
			}

			return default(T);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddAccordion<T>(Rectangle o, Action<int, int, int, int, T> onRender)
		{
			return AddAccordion(o.X, o.Y, o.Width, o.Height, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddAccordion<T>(int x, int y, int w, int h, Action<int, int, int, int, T> onRender)
		{
			return AddAccordion(x, y, w, h, null, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddAccordion<T>(Rectangle o, T initValue, Action<int, int, int, int, T> onRender)
		{
			return AddAccordion(o.X, o.Y, o.Width, o.Height, initValue, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddAccordion<T>(int x, int y, int w, int h, T initValue, Action<int, int, int, int, T> onRender)
		{
			return AddAccordion(x, y, w, h, initValue, null, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddAccordion<T>(
			Rectangle o,
			int pad,
			int bgID,
			int btnNormal,
			int btnSelected,
			Color txtNormal,
			Color txtSelected,
			T initValue,
			Action<int, int, int, int, T> onRender)
		{
			return AddAccordion(
				o.X,
				o.Y,
				o.Width,
				o.Height,
				pad,
				bgID,
				btnNormal,
				btnSelected,
				txtNormal,
				txtSelected,
				initValue,
				onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddAccordion<T>(
			int x,
			int y,
			int w,
			int h,
			int pad,
			int bgID,
			int btnNormal,
			int btnSelected,
			Color txtNormal,
			Color txtSelected,
			T initValue,
			Action<int, int, int, int, T> onRender)
		{
			return AddAccordion(
				x,
				y,
				w,
				h,
				pad,
				bgID,
				btnNormal,
				btnSelected,
				txtNormal,
				txtSelected,
				initValue,
				null,
				onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddAccordion<T>(Rectangle o, ICollection<T> values, Action<int, int, int, int, T> onRender)
		{
			return AddAccordion(o.X, o.Y, o.Width, o.Height, values, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddAccordion<T>(
			int x,
			int y,
			int w,
			int h,
			ICollection<T> values,
			Action<int, int, int, int, T> onRender)
		{
			return AddAccordion(x, y, w, h, default(T), values, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddAccordion<T>(
			Rectangle o,
			T initValue,
			ICollection<T> values,
			Action<int, int, int, int, T> onRender)
		{
			return AddAccordion(o.X, o.Y, o.Width, o.Height, initValue, values, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddAccordion<T>(
			int x,
			int y,
			int w,
			int h,
			T initValue,
			ICollection<T> values,
			Action<int, int, int, int, T> onRender)
		{
			var sup = SupportsUltimaStore;
			var pad = sup ? 15 : 10;
			var bgID = sup ? 40000 : 9270;
			var btnNormal = sup ? 40016 : 9909;
			var btnSelected = sup ? 40027 : 9904;

			return AddAccordion(
				x,
				y,
				w,
				h,
				pad,
				bgID,
				btnNormal,
				btnSelected,
				Color.White,
				Color.Gold,
				initValue,
				values,
				onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddAccordion<T>(
			Rectangle o,
			int pad,
			int bgID,
			int btnNormal,
			int btnSelected,
			Color txtNormal,
			Color txtSelected,
			T initValue,
			ICollection<T> values,
			Action<int, int, int, int, T> onRender)
		{
			return AddAccordion(
				o.X,
				o.Y,
				o.Width,
				o.Height,
				pad,
				bgID,
				btnNormal,
				btnSelected,
				txtNormal,
				txtSelected,
				initValue,
				values,
				onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddAccordion<T>(
			int x,
			int y,
			int w,
			int h,
			int pad,
			int bgID,
			int btnNormal,
			int btnSelected,
			Color txtNormal,
			Color txtSelected,
			T initValue,
			ICollection<T> values,
			Action<int, int, int, int, T> onRender)
		{
			if (w * h <= 0 || pad * 2 >= w || pad * 2 >= h || onRender == null)
			{
				return null;
			}

			if (values == null && typeof(T).IsEnum)
			{
				values = Enum.GetValues(typeof(T)).CastToArray<T>();
			}

			if (values == null || values.Count == 0)
			{
				return null;
			}

			var hash = onRender.GetHashCode();
			var value = _Controls.GetValue(hash);

			if (!(value is T) || !values.Contains((T)value))
			{
				value = initValue;
			}

			if (btnNormal < 0)
			{
				btnNormal = SupportsUltimaStore ? 40016 : 9909;
			}

			if (btnSelected < 0)
			{
				btnSelected = SupportsUltimaStore ? 40027 : 9904;
			}

			var btnSize = GumpsExtUtility.GetImageSize(btnNormal);

			var titleX = pad + btnSize.Width + 5;
			var titleH = btnSize.Height + (pad * 2);

			var titleC = values.Count(v => v != null);

			var panelH = h - (titleC * titleH);

			var hh = 0;

			foreach (var val in values.Where(v => v != null))
			{
				var v = val;
				var s = Equals(value, v);
				var l = ResolveLabel(v).WrapUOHtmlBig().WrapUOHtmlColor(s ? txtSelected : txtNormal, false);

				if (bgID > 0)
				{
					AddBackground(x, y, w, s ? titleH + panelH : titleH, bgID);
				}

				AddInputEC();

				AddButton(
					x + pad,
					y + pad,
					s ? btnSelected : btnNormal,
					s ? btnNormal : btnSelected,
					b =>
					{
						_Controls[hash] = s ? initValue : v;

						Refresh(true);
					});

				AddHtml(x + titleX, ComputeCenter(y + pad, (titleH - (pad * 2))) - 10, w - titleX, 40, l, false, false);

				AddInputEC();

				if (s)
				{
					y += titleH;
					hh += titleH;

					onRender(x + pad, y, w - (pad * 2), panelH - (pad * 2), v);

					y += panelH;
					hh += panelH;
				}
				else
				{
					y += titleH;
					hh += titleH;
				}
			}

			if (value is T)
			{
				return Tuple.Create((T)value, hh);
			}

			return Tuple.Create(default(T), hh);
		}

		public bool HasTabs<T>(Action<int, int, int, int, T> renderer)
		{
			return _Controls.ContainsKey(renderer.GetHashCode());
		}

		public T GetTabs<T>(Action<int, int, int, int, T> renderer)
		{
			var o = _Controls.GetValue(renderer.GetHashCode());

			if (o is T)
			{
				return (T)o;
			}

			return default(T);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddTabs<T>(Rectangle o, Action<int, int, int, int, T> onRender)
		{
			return AddTabs(o.X, o.Y, o.Width, o.Height, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddTabs<T>(int x, int y, int w, int h, Action<int, int, int, int, T> onRender)
		{
			return AddTabs(x, y, w, h, null, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddTabs<T>(Rectangle o, T initValue, Action<int, int, int, int, T> onRender)
		{
			return AddTabs(o.X, o.Y, o.Width, o.Height, initValue, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddTabs<T>(int x, int y, int w, int h, T initValue, Action<int, int, int, int, T> onRender)
		{
			return AddTabs(x, y, w, h, initValue, null, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddTabs<T>(
			Rectangle o,
			int pad,
			int bgID,
			int btnNormal,
			int btnSelected,
			Color txtNormal,
			Color txtSelected,
			T initValue,
			Action<int, int, int, int, T> onRender)
		{
			return AddTabs(
				o.X,
				o.Y,
				o.Width,
				o.Height,
				pad,
				bgID,
				btnNormal,
				btnSelected,
				txtNormal,
				txtSelected,
				initValue,
				onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddTabs<T>(
			int x,
			int y,
			int w,
			int h,
			int pad,
			int bgID,
			int btnNormal,
			int btnSelected,
			Color txtNormal,
			Color txtSelected,
			T initValue,
			Action<int, int, int, int, T> onRender)
		{
			return AddTabs(x, y, w, h, pad, bgID, btnNormal, btnSelected, txtNormal, txtSelected, initValue, null, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddTabs<T>(Rectangle o, ICollection<T> values, Action<int, int, int, int, T> onRender)
		{
			return AddTabs(o.X, o.Y, o.Width, o.Height, values, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddTabs<T>(
			int x,
			int y,
			int w,
			int h,
			ICollection<T> values,
			Action<int, int, int, int, T> onRender)
		{
			return AddTabs(x, y, w, h, default(T), values, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddTabs<T>(
			Rectangle o,
			T initValue,
			ICollection<T> values,
			Action<int, int, int, int, T> onRender)
		{
			return AddTabs(o.X, o.Y, o.Width, o.Height, initValue, values, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddTabs<T>(
			int x,
			int y,
			int w,
			int h,
			T initValue,
			ICollection<T> values,
			Action<int, int, int, int, T> onRender)
		{
			var sup = SupportsUltimaStore;
			var pad = sup ? 15 : 10;
			var bgID = sup ? 40000 : 9270;
			var btnNormal = sup ? 40016 : 9909;
			var btnSelected = sup ? 40027 : 9904;

			return AddTabs(x, y, w, h, pad, bgID, btnNormal, btnSelected, Color.White, Color.Gold, initValue, values, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddTabs<T>(
			Rectangle o,
			int pad,
			int bgID,
			int btnNormal,
			int btnSelected,
			Color txtNormal,
			Color txtSelected,
			T initValue,
			ICollection<T> values,
			Action<int, int, int, int, T> onRender)
		{
			return AddTabs(
				o.X,
				o.Y,
				o.Width,
				o.Height,
				pad,
				bgID,
				btnNormal,
				btnSelected,
				txtNormal,
				txtSelected,
				initValue,
				values,
				onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj)
		/// </summary>
		public Tuple<T, int> AddTabs<T>(
			int x,
			int y,
			int w,
			int h,
			int pad,
			int bgID,
			int btnNormal,
			int btnSelected,
			Color txtNormal,
			Color txtSelected,
			T initValue,
			ICollection<T> values,
			Action<int, int, int, int, T> onRender)
		{
			if (w * h <= 0 || pad * 2 >= w || pad * 2 >= h || onRender == null)
			{
				return null;
			}

			if (values == null && typeof(T).IsEnum)
			{
				values = Enum.GetValues(typeof(T)).CastToArray<T>();
			}

			if (values == null || values.Count == 0)
			{
				return null;
			}

			var hash = onRender.GetHashCode();
			var value = _Controls.GetValue(hash);

			if (!(value is T) || !values.Contains((T)value))
			{
				value = initValue;
			}

			if (btnNormal < 0)
			{
				btnNormal = SupportsUltimaStore ? 40016 : 9909;
			}

			if (btnSelected < 0)
			{
				btnSelected = SupportsUltimaStore ? 40027 : 9904;
			}

			var btnSize = GumpsExtUtility.GetImageSize(btnNormal);

			var titleX = pad + btnSize.Width + 5;
			var titleH = btnSize.Height + 5;

			var titleC = values.Count(v => v != null);

			var font = UOFont.Unicode[0];

			var maxW = values.Where(v => v != null)
							 .Select(v => v.ToString().SpaceWords())
							 .Aggregate(pad, (c, o) => Math.Max(c, font.GetWidth(o)));

			var tabW = titleX + maxW;
			var tabH = titleH;

			var tabC = w / tabW;
			var tabR = (int)Math.Ceiling(titleC / (double)tabC);

			if (tabW * tabC < w - (pad * 2))
			{
				tabW += (int)(((w - (pad * 2)) - (tabW * tabC)) / (double)tabC);
			}

			var tabsH = (pad * 2) + (tabR * tabH);
			var panelH = h - tabsH;

			var txtWidth = tabW - titleX;

			var px = x;
			var py = y + tabsH;
			var hh = 0;
			var i = 0;

			if (bgID > 0)
			{
				AddBackground(x, y, w, tabsH, bgID);
			}

			foreach (var val in values.Where(v => v != null))
			{
				var v = val;
				var s = Equals(value, v);
				var l = ResolveLabel(v).WrapUOHtmlBig().WrapUOHtmlColor(s ? txtSelected : txtNormal, false);

				AddInputEC();

				AddButton(
					x + pad,
					y + pad,
					s ? btnSelected : btnNormal,
					s ? btnNormal : btnSelected,
					b =>
					{
						_Controls[hash] = s ? initValue : v;

						Refresh(true);
					});

				AddHtml(x + titleX, ComputeCenter(y + pad, tabH) - 10, txtWidth, 40, l, false, false);

				AddInputEC();

				if (s)
				{
					if (bgID > 0)
					{
						AddBackground(px, py, w, panelH, bgID);
					}

					onRender(px + pad, py + pad, w - (pad * 2), panelH - (pad * 2), v);

					hh += panelH;
				}

				if (++i % tabC == 0)
				{
					x = px;
					y += tabH;
					hh += tabH;
				}
				else
				{
					x += tabW;
				}
			}

			if (value is T)
			{
				return Tuple.Create((T)value, hh);
			}

			return Tuple.Create(default(T), hh);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj, row, col)
		/// </summary>
		public void AddTable<T>(
			Rectangle o,
			bool headers,
			IEnumerable<int> cols,
			IEnumerable<T> rows,
			int rowHeight,
			Action<int, int, int, int, T, int, int> onRender)
		{
			AddTable(o.X, o.Y, o.Width, o.Height, headers, cols, rows, rowHeight, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj, row, col)
		/// </summary>
		public void AddTable<T>(
			int x,
			int y,
			int w,
			int h,
			bool headers,
			IEnumerable<int> cols,
			IEnumerable<T> rows,
			int rowHeight,
			Action<int, int, int, int, T, int, int> onRender)
		{
			AddTable(x, y, w, h, headers, cols, rows, rowHeight, Color.White, 1, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj, row, col)
		/// </summary>
		public int AddTable<T>(
			Rectangle o,
			bool headers,
			IEnumerable<int> cols,
			IEnumerable<T> rows,
			int rowHeight,
			Color border,
			int borderSize,
			Action<int, int, int, int, T, int, int> onRender)
		{
			return AddTable(o.X, o.Y, o.Width, o.Height, headers, cols, rows, rowHeight, border, borderSize, onRender);
		}

		/// <summary>
		///     onRender: (x, y, w, h, obj, row, col)
		/// </summary>
		public int AddTable<T>(
			int x,
			int y,
			int w,
			int h,
			bool headers,
			IEnumerable<int> cols,
			IEnumerable<T> rows,
			int rowHeight,
			Color border,
			int borderSize,
			Action<int, int, int, int, T, int, int> onRender)
		{
			if (w * h <= 0 || onRender == null)
			{
				return 0;
			}

			if (cols == null || rows == null)
			{
				return 0;
			}

			var spans = cols.ToArray();

			if (spans.Length == 0)
			{
				return 0;
			}

			var auto = spans.Count(s => s < 0);

			if (auto > 0)
			{
				var stat = spans.Sum(s => Math.Max(0, s));
				var delta = (w - stat) / auto;

				if (delta > 0)
				{
					for (var i = 0; i < spans.Length; i++)
					{
						var s = spans[i];

						if (s < 0)
						{
							spans[i] = delta;
						}
					}
				}
			}

			var tw = spans.Sum();

			if (tw < w)
			{
				var wd = (int)((w - tw) / (double)spans.Length);

				spans.SetAll((i, v) => v + wd);
			}
			else if (tw > w)
			{
				var wd = (int)((tw - w) / (double)spans.Length);

				spans.SetAll((i, v) => v - wd);
			}

			var rh = rowHeight;
			var bs = borderSize;

			var cx = x;
			var cy = y;

			var pad = border.IsEmpty ? 1 : bs;
			var pad2 = pad * 2;

			var entries = rows.ToArray();

			var ho = headers ? rh : 0;

			for (var r = headers ? -1 : 0; r < entries.Length; r++)
			{
				for (var c = 0; c < spans.Length; c++)
				{
					onRender(cx + pad, cy + pad, spans[c] - pad2, rh - pad2, r < 0 ? default(T) : entries[r], r, c);

					cx += spans[c];
				}

				cx = x;
				cy += rh;
			}

			if (entries.Length > 0 && !border.IsEmpty && bs > 0)
			{
				cx = x;

				for (var c = 0; c < spans.Length - 1; c++)
				{
					AddRectangle(cx + (spans[c] - (bs / 2)), y + ho, bs, cy - (y + ho), border, true);

					cx += spans[c];
				}

				for (var r = 1; r < entries.Length; r++)
				{
					AddRectangle(x, y + ho + ((r * rh) - (bs / 2)), w, bs, border, true);
				}

				AddRectangle(x, y + ho, w, cy - (y + ho), border, bs);
			}

			return cy - y;
		}

		public void AddSelect<T>(
			int x,
			int y,
			int w,
			int h,
			string text,
			T selected,
			ICollection<T> values,
			Action<T> onSelect)
		{
			AddSelect(x, y, w, h, text, selected, values, onSelect, true, true, false, Color.White, Color.Black);
		}

		public void AddSelect<T>(
			int x,
			int y,
			int w,
			int h,
			string text,
			T selected,
			ICollection<T> values,
			Action<T> onSelect,
			Color color,
			Color fill)
		{
			AddSelect(x, y, w, h, text, selected, values, onSelect, true, true, false, color, fill);
		}

		public void AddSelect<T>(
			int x,
			int y,
			int w,
			int h,
			string text,
			T selected,
			ICollection<T> values,
			Action<T> onSelect,
			bool left,
			bool right,
			bool cycle,
			Color color,
			Color fill)
		{
			if (w * h <= 0 || onSelect == null)
			{
				return;
			}

			if (values == null || values.Count == 0)
			{
				return;
			}

			if (!left && !right && !cycle)
			{
				return;
			}

			var idx = values.IndexOf(selected);
			var prev = cycle ? ((idx <= 0 ? values.Count : idx) - 1) : Math.Max(0, idx - 1);
			var next = cycle ? ((idx + 1) % values.Count) : Math.Min(idx + 1, values.Count - 1);

			var bp = new Action<GumpButton>(
				b =>
				{
					if (idx >= 0)
					{
						onSelect(values.ElementAtOrDefault(prev));
					}

					Refresh(true);
				});

			var bn = new Action<GumpButton>(
				b =>
				{
					if (idx >= 0)
					{
						onSelect(values.ElementAtOrDefault(next));
					}

					Refresh(true);
				});

			if (left)
			{
				if (idx >= 0 && values.Count > 1 && idx != prev)
				{
					var t = UniGlyph.TriLeftFill.ToString().WrapUOHtmlCenter();

					AddHtmlButton(x, y, 20, h, bp, t, color, fill);
				}
				else
				{
					var t = UniGlyph.TriLeftEmpty.ToString().WrapUOHtmlCenter();

					AddHtml(x, y, 20, h, t, color, fill);
				}
			}

			if (right)
			{
				if (idx >= 0 && values.Count > 1 && idx != next)
				{
					var t = UniGlyph.TriRightFill.ToString().WrapUOHtmlCenter();

					AddHtmlButton(x + (w - 20), y, 20, h, bn, t, color, fill);
				}
				else
				{
					var t = UniGlyph.TriRightEmpty.ToString().WrapUOHtmlCenter();

					AddHtml(x + (w - 20), y, 20, h, t, color, fill);
				}
			}

			if (left)
			{
				x += 20;
				w -= 20;
			}

			if (right)
			{
				w -= 20;
			}

			if (String.IsNullOrWhiteSpace(text))
			{
				if (selected is Mobile)
				{
					text = ((Mobile)(object)selected).Name;
				}
				else if (selected is Item)
				{
					text = ((Item)(object)selected).ResolveName(User);
				}
				else if (selected is Enum)
				{
					text = ((Enum)(object)selected).ToString(true);
				}
				else
				{
					text = selected.ToString();
				}
			}

			text = text.WrapUOHtmlCenter();

			if (idx >= 0 && !left && !right && values.Count > 1 && idx != next)
			{
				AddHtmlButton(x, y, w, h, bn, text, color, fill);
			}
			else
			{
				AddHtml(x, y, w, h, text, color, fill);
			}
		}

		public Size AddBackgroundSep(int x, int y, int w, int h, int bgID, Axis dir)
		{
			switch (dir)
			{
				case Axis.None:
					return Size.Empty;
				case Axis.Both:
				{
					var s1 = AddBackgroundSep(x, y, w, h, bgID, Axis.Horizontal);
					var s2 = AddBackgroundSep(x, y, w, h, bgID, Axis.Vertical);

					return new Size(Math.Max(s1.Width, s2.Width), Math.Max(s1.Height, s2.Height));
				}
			}

			int t, l, c, r, b;

			var bg = Enumerable.Range(bgID, 9).ToArray();
			var sz = bg.Select(GumpsExtUtility.GetImageSize).ToArray();

			if (w <= 0)
			{
				w = sz.Min(s => s.Width);

				if (dir == Axis.Vertical)
				{
					w *= 2;
				}
			}

			if (h <= 0)
			{
				h = sz.Min(s => s.Height);

				if (dir == Axis.Horizontal)
				{
					h *= 2;
				}
			}

			switch (dir)
			{
				case Axis.Horizontal:
				{
					h /= 2;

					l = sz[6].Width;
					c = w - (sz[6].Width + sz[8].Width);
					r = sz[8].Width;

					AddImageTiled(x, y, l, h, bg[6]);
					AddImageTiled(x + l, y, c, h, bg[7]);
					AddImageTiled(x + l + c, y, r, h, bg[8]);

					y += h;

					l = sz[0].Width;
					c = w - (sz[0].Width + sz[1].Width);
					r = sz[1].Width;

					AddImageTiled(x, y, l, h, bg[0]);
					AddImageTiled(x + l, y, c, h, bg[1]);
					AddImageTiled(x + l + c, y, r, h, bg[2]);

					y -= h;
					h *= 2;
				}
				break;
				case Axis.Vertical:
				{
					w /= 2;

					t = sz[2].Height;
					c = h - (sz[2].Height + sz[8].Height);
					b = sz[8].Height;

					AddImageTiled(x, y, w, t, bg[2]);
					AddImageTiled(x, y + t, w, c, bg[5]);
					AddImageTiled(x, y + t + c, w, b, bg[8]);

					x += w;

					t = sz[0].Height;
					c = h - (sz[0].Height + sz[6].Height);
					b = sz[6].Height;

					AddImageTiled(x, y, w, t, bg[0]);
					AddImageTiled(x, y + t, w, c, bg[3]);
					AddImageTiled(x, y + t + c, w, b, bg[6]);

					x -= w;
					w *= 2;
				}
				break;
			}

			return new Size(w, h);
		}

		public void AddLink(int x, int y, int w, int h, string text, string uri)
		{
			if (w > 0 && h > 0 && !String.IsNullOrWhiteSpace(text) && Uri.IsWellFormedUriString(uri, UriKind.Absolute))
			{
				text = text.WrapUOHtmlUrl(uri);

				AddHtml(x, y, w, h, text, false, false);
			}
		}

		public void AddLink(string uri)
		{
			if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute) || Entries.Count <= 0)
			{
				return;
			}

			var html = Entries[Entries.Count - 1] as GumpHtml;

			if (html != null)
			{
				html.Text = html.Text.WrapUOHtmlUrl(uri);
			}
		}

		public void AddItem(int x, int y, int itemID, bool offset, Axis centering)
		{
			if (offset)
			{
				var o = GetItemOffset(itemID);

				x += o.X;
				y += o.Y;
			}

			var s = GetImageSize(itemID);

			ComputeCenter(ref x, ref y, s.Width, s.Height, centering);

			AddItem(x, y, itemID);
		}

		public void AddItem(int x, int y, int itemID, int hue, bool offset, Axis centering)
		{
			if (offset)
			{
				var o = GetItemOffset(itemID);

				x += o.X;
				y += o.Y;
			}

			var s = GetImageSize(itemID);

			ComputeCenter(ref x, ref y, s.Width, s.Height, centering);

			AddItem(x, y, itemID, hue);
		}

		public void AddImage(int x, int y, int gumpID, Axis centering)
		{
			var s = GetImageSize(gumpID);

			ComputeCenter(ref x, ref y, s.Width, s.Height, centering);

			AddImage(x, y, gumpID);
		}

		public void AddImage(int x, int y, int gumpID, int hue, Axis centering)
		{
			var s = GetImageSize(gumpID);

			ComputeCenter(ref x, ref y, s.Width, s.Height, centering);

			AddImage(x, y, hue, gumpID);
		}

		public virtual void AddCross(Rectangle bounds, int size, Color color)
		{
			Add(new GumpCross(bounds, size, color));
		}

		public virtual void AddCross(Rectangle bounds, int size, Color color, bool filled)
		{
			Add(new GumpCross(bounds, size, color, filled));
		}

		public virtual void AddCross(Rectangle bounds, int size, Color color, int borderSize)
		{
			Add(new GumpCross(bounds, size, color, borderSize));
		}

		public virtual void AddCross(Rectangle bounds, int size, Color color, Color border, int borderSize)
		{
			Add(new GumpCross(bounds, size, color, border, borderSize));
		}

		public virtual void AddCross(Rectangle2D bounds, int size, Color color)
		{
			Add(new GumpCross(bounds, size, color));
		}

		public virtual void AddCross(Rectangle2D bounds, int size, Color color, bool filled)
		{
			Add(new GumpCross(bounds, size, color, filled));
		}

		public virtual void AddCross(Rectangle2D bounds, int size, Color color, int borderSize)
		{
			Add(new GumpCross(bounds, size, color, borderSize));
		}

		public virtual void AddCross(Rectangle2D bounds, int size, Color color, Color border, int borderSize)
		{
			Add(new GumpCross(bounds, size, color, border, borderSize));
		}

		public virtual void AddCross(int x, int y, int w, int h, int size, Color color)
		{
			Add(new GumpCross(x, y, w, h, size, color));
		}

		public virtual void AddCross(int x, int y, int w, int h, int size, Color color, bool filled)
		{
			Add(new GumpCross(x, y, w, h, size, color, filled));
		}

		public virtual void AddCross(int x, int y, int w, int h, int size, Color color, int borderSize)
		{
			Add(new GumpCross(x, y, w, h, size, color, borderSize));
		}

		public virtual void AddCross(int x, int y, int w, int h, int size, Color color, Color border, int borderSize)
		{
			Add(new GumpCross(x, y, w, h, size, color, border, borderSize));
		}

		public virtual void AddMulti(int x, int y, BaseMulti multi)
		{
			Add(new GumpMulti(x, y, multi));
		}

		public virtual void AddMulti(int x, int y, int multiID)
		{
			Add(new GumpMulti(x, y, multiID));
		}

		public virtual void AddMulti(int x, int y, int hue, int multiID)
		{
			Add(new GumpMulti(x, y, hue, multiID));
		}

		public virtual void AddMulti(int x, int y, MultiComponentList mcl)
		{
			Add(new GumpMulti(x, y, mcl));
		}

		public virtual void AddMulti(int x, int y, int hue, MultiComponentList mcl)
		{
			Add(new GumpMulti(x, y, hue, mcl));
		}

		public virtual void AddInputEC()
		{
			if (IsEnhancedClient)
			{
				Add(new GumpInputEC());
			}
		}

		public Rectangle AddPanel(Rectangle o, int margin, int pad, int bgID, int fillID)
		{
			return AddPanel(o.X, o.Y, o.Width, o.Height, margin, pad, bgID, fillID);
		}

		public Rectangle AddPanel(int x, int y, int w, int h, int margin, int pad, int bgID, int fillID)
		{
			var margin2 = margin * 2;
			var pad2 = pad * 2;

			var o = new Rectangle(margin + x, margin + y, w - margin2, h - margin2);
			var r = new Rectangle(o.X + pad, o.Y + pad, o.Width - pad2, o.Height - pad2);

			if (o.Width * o.Height <= 0 || r.Width * r.Height <= 0)
			{
				return new Rectangle(x, y, w, h);
			}

			if (bgID > 0)
			{
				AddBackground(o.X, o.Y, o.Width, o.Height, bgID);

				if (fillID > 0)
				{
					AddImageTiled(r.X, r.Y, r.Width, r.Height, fillID);

					if (bgID == 40000)
					{
						AddImageTiled(r.X, r.Y - 3, r.Width, 3, fillID);
						AddImageTiled(r.X - 3, r.Y, 3, r.Height, fillID);
						AddImageTiled(r.X, r.Y + r.Height, r.Width, 3, fillID);
						AddImageTiled(r.X + r.Width, r.Y, 3, r.Height, fillID);
					}
				}
			}
			else if (fillID > 0)
			{
				AddImageTiled(o.X, o.Y, o.Width, o.Height, fillID);
			}

			return r;
		}

		public void Add(SuperGumpEntry e)
		{
			if (OnBeforeAdd(e))
			{
				base.Add(e);

				OnAdded(e);
			}
		}

		protected virtual bool OnBeforeAdd(SuperGumpEntry e)
		{
			return e != null;
		}

		protected virtual void OnAdded(SuperGumpEntry e)
		{ }

		public void ApplyPadding(ref int o, ref int s, int pad)
		{
			o += pad;
			s -= pad * 2;
		}

		public void ApplyPadding(ref int x, ref int y, ref int w, ref int h, int pad)
		{
			ApplyPadding(ref x, ref w, pad);
			ApplyPadding(ref y, ref h, pad);
		}

		public Point ComputeCenter(Point p, Size s, Axis centering)
		{
			switch (centering)
			{
				case Axis.None:
					return p;
				case Axis.Both:
					return ComputeCenter(p, s);
			}

			if (centering.HasFlag(Axis.Horizontal))
			{
				return new Point(ComputeCenter(p.X, s.Width), p.Y);
			}

			if (centering.HasFlag(Axis.Vertical))
			{
				return new Point(p.X, ComputeCenter(p.Y, s.Height));
			}

			return ComputeCenter(p, s);
		}

		public void ComputeCenter(ref int x, ref int y, int w, int h, Axis centering)
		{
			if (centering != Axis.None)
			{
				if (centering.HasFlag(Axis.Horizontal))
				{
					ComputeCenter(ref x, w);
				}

				if (centering.HasFlag(Axis.Vertical))
				{
					ComputeCenter(ref y, h);
				}
			}
		}

		public Point ComputeCenter(Point p, Size s)
		{
			return new Point(ComputeCenter(p.X, s.Width), ComputeCenter(p.Y, s.Height));
		}

		public void ComputeCenter(ref Point p, Size s)
		{
			p = new Point(ComputeCenter(p.X, s.Width), ComputeCenter(p.Y, s.Height));
		}

		public Point ComputeCenter(int x, int y, int w, int h)
		{
			ComputeCenter(ref x, ref y, w, h);

			return new Point(x, y);
		}

		public void ComputeCenter(ref int x, ref int y, int w, int h)
		{
			x = ComputeCenter(x, w);
			y = ComputeCenter(y, h);
		}

		public void ComputeCenter(ref int o, int s)
		{
			o = ComputeCenter(o, s);
		}

		public int ComputeCenter(int o, int s)
		{
			return o + (s / 2);
		}

		public string ResolveLabel(object o)
		{
			if (o is Enum)
			{
				return ((Enum)o).ToString(true);
			}

			if (o is Item)
			{
				return ((Item)o).ResolveName(User);
			}

			if (o is Mobile)
			{
				return ((Mobile)o).Name;
			}

			return o.ToString();
		}
	}
}
