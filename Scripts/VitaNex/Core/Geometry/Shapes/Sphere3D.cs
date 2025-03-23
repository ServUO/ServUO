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

using Server;
#endregion

namespace VitaNex.Geometry
{
	public class Sphere3D : Shape3D
	{
		private int _Radius;
		private bool _Hollow;

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public int Radius
		{
			get => _Radius;
			set
			{
				if (_Radius == value)
				{
					return;
				}

				_Radius = value;
				Render();
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool Hollow
		{
			get => _Hollow;
			set
			{
				if (_Hollow == value)
				{
					return;
				}

				_Hollow = value;
				Render();
			}
		}

		public Sphere3D(int radius)
			: this(Point3D.Zero, radius)
		{ }

		public Sphere3D(IPoint3D center, int radius)
			: this(center, radius, false)
		{ }

		public Sphere3D(IPoint3D center, int radius, bool hollow)
			: base(center)
		{
			_Radius = radius;
			_Hollow = hollow;
		}

		public Sphere3D(GenericReader reader)
			: base(reader)
		{ }

		protected override void OnRender()
		{
			const int h = 5;

			var layers = Radius * 2;

			for (var z = -layers; z <= layers; z++)
			{
				var p = z / Math.Max(1.0, layers);

				var r = 2 * Radius;

				if (p < 0.5)
				{
					r = (int)Math.Ceiling(r * p);
				}
				else if (p > 0.5)
				{
					r = (int)Math.Ceiling(r - (r * p));
				}
				else
				{
					r = Radius;
				}

				for (var x = -r; x <= r; x++)
				{
					for (var y = -r; y <= r; y++)
					{
						var dist = (int)Math.Sqrt(x * x + y * y);

						if ((!Hollow || z == -layers || z == layers || dist >= r) && dist <= r)
						{
							Add(new Block3D(Center.Clone3D(x, y, z * h), h));
						}
					}
				}
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(_Radius);
			writer.Write(_Hollow);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			_Radius = reader.ReadInt();
			_Hollow = reader.ReadBool();
		}
	}
}