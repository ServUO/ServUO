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
using System.Linq;

using VitaNex.Collections;
#endregion

namespace Server
{
	public interface IWireframe : IEnumerable<Block3D>
	{
		bool Intersects(IBlock3D b);
		bool Intersects(int x, int y, int z);
		bool Intersects(int x, int y, int z, int h);
		bool Intersects(IWireframe frame);
		Rectangle3D GetBounds();
		IEnumerable<Block3D> Offset(int x = 0, int y = 0, int z = 0, int h = 0);
	}

	public sealed class Wireframe : IEquatable<Wireframe>, IWireframe
	{
		public static readonly Wireframe Empty = new Wireframe(0);

		public Block3D[] Blocks { get; private set; }

		public int Volume => Blocks.Length;

		public int Length => Blocks.Length;

		public Wireframe(params IBlock3D[] blocks)
			: this(blocks.Ensure().Select(b => new Block3D(b)))
		{ }

		public Wireframe(IEnumerable<IBlock3D> blocks)
			: this(blocks.Ensure().Select(b => new Block3D(b)))
		{ }

		public Wireframe(Wireframe frame)
			: this(frame.Blocks)
		{ }

		public Wireframe(int capacity)
		{
			Blocks = new Block3D[capacity];
		}

		public Wireframe(params Block3D[] blocks)
		{
			Blocks = blocks.Ensure().ToArray();
		}

		public Wireframe(IEnumerable<Block3D> blocks)
		{
			Blocks = blocks.Ensure().ToArray();
		}

		public bool Intersects(IPoint3D p)
		{
			return Intersects(p.X, p.Y, p.Z);
		}

		public bool Intersects(IPoint3D p, int h)
		{
			return Intersects(p.X, p.Y, p.Z, h);
		}

		public bool Intersects(IBlock3D b)
		{
			return Intersects(b.X, b.Y, b.Z, b.H);
		}

		public bool Intersects(int x, int y, int z)
		{
			return Intersects(x, y, z, 0);
		}

		public bool Intersects(int x, int y, int z, int h)
		{
			return Blocks.Any(b => b.Intersects(x, y, z, h));
		}

		public bool Intersects(IWireframe frame)
		{
			return frame != null && Blocks.Any(b => frame.Intersects(b));
		}

		public Rectangle3D GetBounds()
		{
			Point3D min = Point3D.Zero, max = Point3D.Zero;

			foreach (var b in Blocks)
			{
				min.X = Math.Min(min.X, b.X);
				min.Y = Math.Min(min.Y, b.Y);
				min.Z = Math.Min(min.Z, b.Z);

				max.X = Math.Max(max.X, b.X);
				max.Y = Math.Max(max.Y, b.Y);
				max.Z = Math.Max(max.Z, b.Z + b.H);
			}

			return new Rectangle3D(min, max);
		}

		public IEnumerable<Block3D> Offset(int x = 0, int y = 0, int z = 0, int h = 0)
		{
			return Blocks.Select(b => b.Offset(x, y, z, h));
		}

		public IEnumerator<Block3D> GetEnumerator()
		{
			return Blocks.AsEnumerable().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Block3D this[int index]
		{
			get
			{
				if (index < 0 || index >= Blocks.Length)
				{
					return Block3D.Empty;
				}

				return Blocks[index];
			}
			set
			{
				if (index >= 0 && index < Blocks.Length)
				{
					Blocks[index] = value;

					_Hash = null;
				}
			}
		}

		private int? _Hash;

		public int HashCode => _Hash ?? (_Hash = Blocks.GetContentsHashCode(true)).Value;

		public override int GetHashCode()
		{
			return HashCode;
		}

		public override bool Equals(object obj)
		{
			return obj is Wireframe && Equals((Wireframe)obj);
		}

		public bool Equals(Wireframe other)
		{
			return !ReferenceEquals(other, null) && (ReferenceEquals(this, other) || GetHashCode() == other.GetHashCode());
		}

		public override string ToString()
		{
			return String.Format("Wireframe ({0:#,0} blocks)", Length);
		}

		public static bool operator ==(Wireframe l, Wireframe r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(Wireframe l, Wireframe r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}
	}

	public class DynamicWireframe : IEquatable<DynamicWireframe>, IWireframe
	{
		public bool Rendering { get; protected set; }

		public virtual List<Block3D> Blocks { get; set; }

		public virtual int Volume => Blocks.Count;

		public int Count => Blocks.Count;

		public DynamicWireframe(params IBlock3D[] blocks)
			: this(blocks.Ensure().Select(b => new Block3D(b)))
		{ }

		public DynamicWireframe(IEnumerable<IBlock3D> blocks)
			: this(blocks.Ensure().Select(b => new Block3D(b)))
		{ }

		public DynamicWireframe(Wireframe frame)
			: this(frame.Blocks)
		{ }

		public DynamicWireframe()
			: this(0x100)
		{ }

		public DynamicWireframe(int capacity)
		{
			Blocks = new List<Block3D>(capacity);
		}

		public DynamicWireframe(params Block3D[] blocks)
		{
			Blocks = blocks.Ensure().ToList();
		}

		public DynamicWireframe(IEnumerable<Block3D> blocks)
		{
			Blocks = blocks.Ensure().ToList();
		}

		public DynamicWireframe(GenericReader reader)
		{
			Rendering = true;
			Deserialize(reader);
			Rendering = false;
		}

		public void Flatten()
		{
			var list = ListPool<Block3D>.AcquireObject();

			list.AddRange(this);

			Clear();

			AddRange(list.Flatten());

			ObjectPool.Free(ref list);
		}

		public bool Intersects(IPoint3D p)
		{
			return Intersects(p.X, p.Y, p.Z);
		}

		public bool Intersects(IBlock3D b)
		{
			return Intersects(b.X, b.Y, b.Z, b.H);
		}

		public bool Intersects(int x, int y, int z)
		{
			return Intersects(x, y, z, 0);
		}

		public bool Intersects(int x, int y, int z, int h)
		{
			return Blocks.Any(b => b.Intersects(x, y, z, h));
		}

		public bool Intersects(IWireframe frame)
		{
			return frame != null && Blocks.Any(b => frame.Intersects(b));
		}

		public Rectangle3D GetBounds()
		{
			Point3D min = Point3D.Zero, max = Point3D.Zero;

			foreach (var b in Blocks)
			{
				min.X = Math.Min(min.X, b.X);
				min.Y = Math.Min(min.Y, b.Y);
				min.Z = Math.Min(min.Z, b.Z);

				max.X = Math.Max(max.X, b.X);
				max.Y = Math.Max(max.Y, b.Y);
				max.Z = Math.Max(max.Z, b.Z + b.H);
			}

			return new Rectangle3D(min, max);
		}

		public IEnumerable<Block3D> Offset(int x = 0, int y = 0, int z = 0, int h = 0)
		{
			return Blocks.Select(b => b.Offset(x, y, z, h));
		}

		public virtual void Clear()
		{
			Blocks.Free(true);

			_Hash = null;
		}

		public virtual void Add(Block3D item)
		{
			if (item != null)
			{
				Blocks.Add(item);

				_Hash = null;
			}
		}

		public virtual void AddRange(IEnumerable<Block3D> collection)
		{
			if (collection != null)
			{
				Blocks.AddRange(collection);

				_Hash = null;
			}
		}

		public virtual bool Remove(Block3D item)
		{
			if (item == null)
			{
				return false;
			}

			var success = Blocks.Remove(item);

			Blocks.Free(false);

			_Hash = null;

			return success;
		}

		public virtual int RemoveAll(Predicate<Block3D> match)
		{
			if (match == null)
			{
				return 0;
			}

			var success = Blocks.RemoveAll(match);

			Blocks.Free(false);

			_Hash = null;

			return success;
		}

		public virtual void RemoveAt(int index)
		{
			if (!Blocks.InBounds(index))
			{
				return;
			}

			Blocks.RemoveAt(index);
			Blocks.Free(false);

			_Hash = null;
		}

		public virtual void RemoveRange(int index, int count)
		{
			if (!Blocks.InBounds(index))
			{
				return;
			}

			Blocks.RemoveRange(index, count);
			Blocks.Free(false);

			_Hash = null;
		}

		public virtual void ForEach(Action<Block3D> action)
		{
			Blocks.ForEach(action);
		}

		public virtual IEnumerator<Block3D> GetEnumerator()
		{
			return Blocks.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public virtual Block3D this[int index]
		{
			get
			{
				if (index < 0 || index >= Blocks.Count)
				{
					return Block3D.Empty;
				}

				return Blocks[index];
			}
			set
			{
				if (index >= 0 && index < Blocks.Count)
				{
					Blocks[index] = value;

					_Hash = null;
				}
			}
		}

		private int? _Hash;

		public int HashCode => _Hash ?? (_Hash = Blocks.GetContentsHashCode(true)).Value;

		public override int GetHashCode()
		{
			return HashCode;
		}

		public override bool Equals(object obj)
		{
			return obj is DynamicWireframe && Equals((DynamicWireframe)obj);
		}

		public bool Equals(DynamicWireframe other)
		{
			return !ReferenceEquals(other, null) && (ReferenceEquals(this, other) || GetHashCode() == other.GetHashCode());
		}

		public override string ToString()
		{
			return String.Format("DynamicWireframe ({0:#,0} blocks)", Count);
		}

		public virtual void Serialize(GenericWriter writer)
		{
			writer.SetVersion(0);

			writer.WriteList(Blocks, (w, b) => w.WriteBlock3D(b));
		}

		public virtual void Deserialize(GenericReader reader)
		{
			reader.GetVersion();

			Blocks = reader.ReadList(r => reader.ReadBlock3D());
		}

		public static bool operator ==(DynamicWireframe l, DynamicWireframe r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(DynamicWireframe l, DynamicWireframe r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}
	}
}