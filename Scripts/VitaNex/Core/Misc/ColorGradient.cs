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
#endregion

namespace VitaNex
{
	public class ColorGradient : List<Color>, IDisposable
	{
		private static readonly Color[] _EmptyColors = new Color[0];
		private static readonly int[] _EmptySizes = new int[0];

		public static ColorGradient CreateInstance(params Color[] colors)
		{
			return CreateInstance(0, colors);
		}

		public static ColorGradient CreateInstance(int fade, params Color[] colors)
		{
			if (fade <= 0 || colors.Length < 2)
			{
				return new ColorGradient(colors);
			}

			var gradient = new ColorGradient(colors.Length + ((fade - 1) * colors.Length));

			Color c1, c2;
			double f;

			for (var i = 0; colors.InBounds(i); i++)
			{
				c1 = colors[i];

				if (!colors.InBounds(i + 1))
				{
					gradient.Add(c1);
					break;
				}

				c2 = colors[i + 1];

				for (f = 0; f < fade; f++)
				{
					gradient.Add(c1.Interpolate(c2, f / fade));
				}
			}

			gradient.Free(false);

			return gradient;
		}

		public bool IsDisposed { get; private set; }

		public ColorGradient(int capacity)
			: base(capacity)
		{ }

		public ColorGradient(params Color[] colors)
			: base(colors.Ensure())
		{ }

		public ColorGradient(IEnumerable<Color> colors)
			: base(colors.Ensure())
		{ }

		~ColorGradient()
		{
			Dispose();
		}

		public void ForEachSegment(int size, Action<int, int, Color> action)
		{
			if (size <= 0 || action == null)
			{
				return;
			}

			GetSegments(size, out var colors, out var sizes, out var count);

			if (count <= 0)
			{
				return;
			}

			Color c;
			int s;

			for (int i = 0, o = 0; i < count; i++)
			{
				c = colors[i];
				s = sizes[i];

				if (!c.IsEmpty && c != Color.Transparent && s > 0)
				{
					action(o, s, c);
				}

				o += s;
			}
		}

		public void GetSegments(int size, out Color[] colors, out int[] sizes, out int count)
		{
			if (IsDisposed)
			{
				count = 0;
				colors = _EmptyColors;
				sizes = _EmptySizes;

				return;
			}

			count = Count;

			if (count == 0)
			{
				colors = _EmptyColors;
				sizes = _EmptySizes;

				return;
			}

			colors = new Color[count];
			sizes = new int[count];

			var chunk = (int)Math.Ceiling(size / (double)count);

			for (var i = 0; i < count; i++)
			{
				colors[i] = this[i];
				sizes[i] = chunk;
			}

			var total = sizes.Sum();

			if (total > size)
			{
				var diff = total - size;

				while (diff > 0)
				{
					var share = (int)Math.Ceiling(diff / (double)count);

					for (var i = count - 1; i >= 0; i--)
					{
						sizes[i] -= share;

						diff -= share;

						if (diff <= 0)
						{
							break;
						}
					}
				}
			}
			else if (total < size)
			{
				var diff = size - total;

				while (diff > 0)
				{
					var share = (int)Math.Ceiling(diff / (double)count);

					for (var i = 0; i < count; i++)
					{
						sizes[i] += share;

						diff -= share;

						if (diff <= 0)
						{
							break;
						}
					}
				}
			}
		}

		public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			IsDisposed = true;

			this.Free(true);
		}
	}
}