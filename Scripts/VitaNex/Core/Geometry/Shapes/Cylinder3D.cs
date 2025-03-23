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
	public class Cylinder3D : Shape3D
	{
		private int _Radius;
		private bool _Hollow;
		private bool _EndCaps;

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

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public bool EndCaps
		{
			get => _EndCaps;
			set
			{
				if (_EndCaps == value)
				{
					return;
				}

				_EndCaps = value;
				Render();
			}
		}

		public Cylinder3D(int radius)
			: this(Point3D.Zero, radius)
		{ }

		public Cylinder3D(IPoint3D center, int radius)
			: this(center, radius, false)
		{ }

		public Cylinder3D(IPoint3D center, int radius, bool hollow)
			: this(center, radius, hollow, true)
		{
			Hollow = hollow;
		}

		public Cylinder3D(IPoint3D center, int radius, bool hollow, bool endCaps)
			: base(center)
		{
			_Radius = radius;
			_Hollow = hollow;
			_EndCaps = endCaps;
		}

		public Cylinder3D(GenericReader reader)
			: base(reader)
		{ }

		protected override void OnRender()
		{
			const int h = 5;

			for (var z = -Radius; z <= Radius; z++)
			{
				if (Hollow && !EndCaps && (z == -Radius || z == Radius))
				{
					continue;
				}

				for (var x = -Radius; x <= Radius; x++)
				{
					for (var y = -Radius; y <= Radius; y++)
					{
						var dist = (int)Math.Sqrt(x * x + y * y);

						if ((!Hollow || z == -Radius || z == Radius || dist >= Radius) && dist <= Radius)
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
			writer.Write(_EndCaps);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			_Radius = reader.ReadInt();
			_Hollow = reader.ReadBool();
			_EndCaps = reader.ReadBool();
		}
	}
}