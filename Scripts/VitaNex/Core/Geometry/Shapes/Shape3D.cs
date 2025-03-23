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

using Server;
#endregion

namespace VitaNex.Geometry
{
	[PropertyObject]
	public abstract class Shape3D : DynamicWireframe, IPoint3D
	{
		private bool _InitialRender;

		public override List<Block3D> Blocks
		{
			get
			{
				if (!_InitialRender)
				{
					Render();
				}

				return base.Blocks;
			}
			set => base.Blocks = value;
		}

		public override int Volume
		{
			get
			{
				if (!_InitialRender)
				{
					Render();
				}

				return base.Volume;
			}
		}

		protected Point3D _Center;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public Point3D Center
		{
			get => _Center;
			set
			{
				if (_Center == value)
				{
					return;
				}

				_Center = value;
				Render();
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int X
		{
			get => _Center.X;
			set
			{
				if (_Center.X == value)
				{
					return;
				}

				_Center.X = value;
				Render();
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Y
		{
			get => _Center.Y;
			set
			{
				if (_Center.Z == value)
				{
					return;
				}

				_Center.Y = value;
				Render();
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Z
		{
			get => _Center.Z;
			set
			{
				if (_Center.Z == value)
				{
					return;
				}

				_Center.Z = value;
				Render();
			}
		}

		public Shape3D()
			: this(Point3D.Zero)
		{ }

		public Shape3D(IPoint3D center)
		{
			_Center = center.Clone3D();
		}

		public Shape3D(GenericReader reader)
			: base(reader)
		{ }

		public void Render()
		{
			if (Rendering)
			{
				return;
			}

			_InitialRender = Rendering = true;

			Clear();
			OnRender();

			Rendering = false;
		}

		protected abstract void OnRender();

		public override void Add(Block3D item)
		{
			if (!_InitialRender)
			{
				Render();
			}

			base.Add(item);
		}

		public override void AddRange(IEnumerable<Block3D> collection)
		{
			if (!_InitialRender)
			{
				Render();
			}

			base.AddRange(collection);
		}

		public override bool Remove(Block3D item)
		{
			if (!_InitialRender)
			{
				Render();
			}

			return base.Remove(item);
		}

		public override int RemoveAll(Predicate<Block3D> match)
		{
			if (!_InitialRender)
			{
				Render();
			}

			return base.RemoveAll(match);
		}

		public override void RemoveAt(int index)
		{
			if (!_InitialRender)
			{
				Render();
			}

			base.RemoveAt(index);
		}

		public override void RemoveRange(int index, int count)
		{
			if (!_InitialRender)
			{
				Render();
			}

			base.RemoveRange(index, count);
		}

		public override void ForEach(Action<Block3D> action)
		{
			if (!_InitialRender)
			{
				Render();
			}

			base.ForEach(action);
		}

		public override IEnumerator<Block3D> GetEnumerator()
		{
			if (!_InitialRender)
			{
				Render();
			}

			return base.GetEnumerator();
		}

		public override Block3D this[int index]
		{
			get
			{
				if (!_InitialRender)
				{
					Render();
				}

				return base[index];
			}
			set
			{
				if (!_InitialRender)
				{
					Render();
				}

				base[index] = value;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			if (!_InitialRender)
			{
				Render();
			}

			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(_Center);
		}

		public override void Deserialize(GenericReader reader)
		{
			_InitialRender = true;

			base.Deserialize(reader);

			reader.GetVersion();

			_Center = reader.ReadPoint3D();
		}
	}
}