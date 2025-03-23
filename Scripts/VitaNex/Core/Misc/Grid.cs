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

using VitaNex.Collections;
#endregion

namespace VitaNex
{
	public class Grid<T> : IEnumerable<T>
	{
		private List<List<T>> _InternalGrid;

		private Size _Size = Size.Empty;

		public Action<GenericWriter, T, int, int> OnSerializeContent { get; set; }
		public Func<GenericReader, Type, int, int, T> OnDeserializeContent { get; set; }

		public virtual T DefaultValue { get; set; }

		public virtual int Width { get => _Size.Width; set => Resize(value, _Size.Height); }
		public virtual int Height { get => _Size.Height; set => Resize(_Size.Width, value); }

		public virtual T this[int x, int y]
		{
			get
			{
				var val = DefaultValue;

				if (InBounds(x, y))
				{
					val = _InternalGrid[x][y];
				}

				return val;
			}
			set
			{
				if (InBounds(x, y))
				{
					_InternalGrid[x][y] = value;
				}
			}
		}

		public int Count => _InternalGrid.SelectMany(e => e).Count(e => e != null);

		public int Capacity => Width * Height;

		public Grid()
			: this(0, 0)
		{
			DefaultValue = default(T);
		}

		public Grid(Grid<T> grid)
			: this(grid.Width, grid.Height)
		{
			SetAllContent(grid.GetContent);
		}

		public Grid(int width, int height)
		{
			_InternalGrid = ListPool<List<T>>.AcquireObject();

			Resize(width, height);
		}

		public Grid(GenericReader reader)
		{
			_InternalGrid = ListPool<List<T>>.AcquireObject();

			Deserialize(reader);
		}

		~Grid()
		{
			if (!_InternalGrid.IsNullOrEmpty())
			{
				Clear();
			}

			ObjectPool.Free(ref _InternalGrid);
		}

		public void Clear()
		{
			foreach (var col in _InternalGrid)
			{
				col.Clear();
			}

			_InternalGrid.Clear();
		}

		public void TrimExcess()
		{
			foreach (var col in _InternalGrid)
			{
				col.TrimExcess();
			}

			_InternalGrid.TrimExcess();
		}

		public void Free(bool clear)
		{
			if (clear)
			{
				Clear();
			}

			TrimExcess();
		}

		public void TrimOverflow()
		{
			for (var x = Width - 1; x >= 0; x--)
			{
				if (FindCells(x, 0, 1, Height).All(e => e == null))
				{
					RemoveColumn(x);
				}
			}

			for (var y = Height - 1; y >= 0; y--)
			{
				if (FindCells(0, y, Width, 1).All(e => e == null))
				{
					RemoveRow(y);
				}
			}

			TrimExcess();
		}

		public virtual void ForEach(Action<int, int> action)
		{
			if (action == null)
			{
				return;
			}

			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					action(x, y);
				}
			}
		}

		public virtual void ForEach(Action<int, int, T> action)
		{
			if (action == null)
			{
				return;
			}

			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					action(x, y, this[x, y]);
				}
			}
		}

		public virtual void ForEachIndex(Action<int, int, int> action)
		{
			if (action == null)
			{
				return;
			}

			var i = 0;

			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					action(x, y, i++);
				}
			}
		}

		public virtual void Resize(int width, int height)
		{
			width = Math.Max(0, width);
			height = Math.Max(0, height);

			if (width * height > 0)
			{
				while (Height != height)
				{
					if (Height < height)
					{
						AppendRow();
					}
					else if (Height > height)
					{
						RemoveRow(Height - 1);
					}
				}

				while (Width != width)
				{
					if (Width < width)
					{
						AppendColumn();
					}
					else if (Width > width)
					{
						RemoveColumn(Width - 1);
					}
				}
			}
			else
			{
				Clear();
			}

			_Size = new Size(width, height);
		}

		public virtual Point GetLocaton(T content)
		{
			for (var x = 0; x < Width; x++)
			{
				for (var y = 0; y < Height; y++)
				{
					if (Equals(this[x, y], content))
					{
						return new Point(x, y);
					}
				}
			}

			return new Point(-1, -1);
		}

		public virtual T[] GetCells()
		{
			return GetCells(0, 0, Width, Height);
		}

		public virtual T[] GetCells(int x, int y, int w, int h)
		{
			return FindCells(x, y, w, h).ToArray();
		}

		public virtual IEnumerable<T> FindCells(int x, int y, int w, int h)
		{
			for (var col = x; col < x + w; col++)
			{
				if (!_InternalGrid.InBounds(col))
				{
					continue;
				}

				for (var row = y; row < y + h; row++)
				{
					if (_InternalGrid[col].InBounds(row))
					{
						yield return _InternalGrid[col][row];
					}
				}
			}
		}

		public virtual T[][] SelectCells(Rectangle2D bounds)
		{
			return SelectCells(bounds.X, bounds.Y, bounds.Width, bounds.Height);
		}

		public virtual T[][] SelectCells(int x, int y, int w, int h)
		{
			var list = new T[w][];

			for (int xx = 0, col = x; col < x + w; col++, xx++)
			{
				list[xx] = new T[h];

				for (int yy = 0, row = y; row < y + h; row++, yy++)
				{
					if (col >= Width || row >= Height)
					{
						list[xx][yy] = DefaultValue;
					}
					else
					{
						list[xx][yy] = this[col, row];
					}
				}
			}

			return list;
		}

		public virtual void PrependColumn()
		{
			InsertColumn(0);
		}

		public virtual void AppendColumn()
		{
			InsertColumn(Width);
		}

		public virtual void InsertColumn(int x)
		{
			x = Math.Max(0, x);

			var col = ListPool<T>.AcquireObject();

			col.Capacity = Height;
			col.SetAll(DefaultValue);

			if (x >= Width)
			{
				_InternalGrid.Add(col);
			}
			else
			{
				_InternalGrid.Insert(x, col);
			}

			_Size = new Size(_InternalGrid.Count, _Size.Height);
		}

		public virtual void RemoveColumn(int x)
		{
			if (x >= 0 && x < Width)
			{
				if (_InternalGrid.InBounds(x))
				{
					ObjectPool.Free(_InternalGrid[x]);

					_InternalGrid.RemoveAt(x);
				}
			}

			_Size = new Size(_InternalGrid.Count, _Size.Height);
		}

		public virtual void PrependRow()
		{
			InsertRow(0);
		}

		public virtual void AppendRow()
		{
			InsertRow(Height);
		}

		public virtual void InsertRow(int y)
		{
			y = Math.Max(0, y);

			foreach (var col in _InternalGrid)
			{
				if (y >= Height)
				{
					col.Add(DefaultValue);
				}
				else
				{
					col.Insert(y, DefaultValue);
				}
			}

			_Size = new Size(_InternalGrid.Count, Math.Max(0, _Size.Height + 1));
		}

		public virtual void RemoveRow(int y)
		{
			if (y >= 0 && y < Height)
			{
				foreach (var col in _InternalGrid)
				{
					if (col.InBounds(y))
					{
						col.RemoveAt(y);
					}
				}
			}

			_Size = new Size(_InternalGrid.Count, Math.Max(0, _Size.Height - 1));
		}

		public virtual T GetContent(int x, int y)
		{
			return this[x, y];
		}

		public virtual void SetContent(int x, int y, T content)
		{
			if (x >= Width || y >= Height)
			{
				Resize(Width, Height);
			}

			this[x, y] = content;
		}

		public virtual void SetAllContent(T content, Predicate<T> replace)
		{
			ForEach(
				(x, y, c) =>
				{
					if (replace == null || replace(c))
					{
						this[x, y] = content;
					}
				});
		}

		public virtual void SetAllContent(Func<int, int, T> resolve)
		{
			if (resolve != null)
			{
				ForEach((x, y, c) => this[x, y] = resolve(x, y));
			}
		}

		public virtual void SetAllContent(Func<int, int, T, T> resolve)
		{
			if (resolve != null)
			{
				ForEach((x, y, c) => this[x, y] = resolve(x, y, c));
			}
		}

		public virtual void SetAllContent(Func<int, int, int, T> resolve)
		{
			if (resolve != null)
			{
				ForEachIndex((x, y, i) => this[x, y] = resolve(x, y, i));
			}
		}

		public bool InBounds(int gx, int gy)
		{
			return _InternalGrid.InBounds(gx) && _InternalGrid[gx].InBounds(gy);
		}

		public virtual IEnumerator<T> GetEnumerator()
		{
			return _InternalGrid.SelectMany(x => x).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(Width);
					writer.Write(Height);

					ForEach(
						(x, y, c) =>
						{
							if (c == null)
							{
								writer.Write(false);
							}
							else
							{
								writer.Write(true);
								writer.WriteType(c);

								SerializeContent(writer, c, x, y);
							}
						});
				}
				break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					var width = reader.ReadInt();
					var height = reader.ReadInt();

					Resize(width, height);

					ForEach(
						(x, y, c) =>
						{
							if (!reader.ReadBool())
							{
								return;
							}

							var type = reader.ReadType();

							this[x, y] = DeserializeContent(reader, type, x, y);
						});
				}
				break;
			}
		}

		public virtual void SerializeContent(GenericWriter writer, T content, int x, int y)
		{
			if (OnSerializeContent != null)
			{
				OnSerializeContent(writer, content, x, y);
			}
		}

		public virtual T DeserializeContent(GenericReader reader, Type type, int x, int y)
		{
			return OnDeserializeContent != null ? OnDeserializeContent(reader, type, x, y) : DefaultValue;
		}

		public static implicit operator Rectangle2D(Grid<T> grid)
		{
			return new Rectangle2D(0, 0, grid.Width, grid.Height);
		}

		public static implicit operator Grid<T>(Rectangle2D rect)
		{
			return new Grid<T>(rect.Width, rect.Height);
		}
	}
}