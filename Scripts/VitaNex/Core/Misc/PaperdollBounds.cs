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

using Server;
#endregion

namespace VitaNex
{
	public struct PaperdollBounds : IEquatable<PaperdollBounds>, IEquatable<Layer>, IEnumerable<Rectangle2D>
	{
		public static readonly PaperdollBounds Empty = new PaperdollBounds();

		public static readonly PaperdollBounds MainHand = new PaperdollBounds(
			new Rectangle2D(20, 25, 40, 90),
			new Rectangle2D(20, 115, 20, 20),
			new Rectangle2D(20, 135, 30, 40));

		public static readonly PaperdollBounds OffHand = new PaperdollBounds(
			new Rectangle2D(120, 90, 40, 25),
			new Rectangle2D(120, 120, 20, 15),
			new Rectangle2D(120, 135, 40, 30));

		public static readonly PaperdollBounds Arms = new PaperdollBounds(
			new Rectangle2D(55, 80, 25, 40),
			new Rectangle2D(100, 80, 25, 40));

		public static readonly PaperdollBounds Backpack = new PaperdollBounds(new Rectangle2D(115, 180, 50, 50));

		public static readonly PaperdollBounds Bracelet = new PaperdollBounds(
			new Rectangle2D(50, 110, 10, 15),
			new Rectangle2D(125, 115, 10, 15));

		public static readonly PaperdollBounds Cloak = new PaperdollBounds(new Rectangle2D(45, 135, 100, 100));

		public static readonly PaperdollBounds Earrings = new PaperdollBounds(
			new Rectangle2D(70, 65, 10, 10),
			new Rectangle2D(105, 70, 10, 10));

		public static readonly PaperdollBounds Gloves = new PaperdollBounds(
			new Rectangle2D(35, 105, 20, 20),
			new Rectangle2D(130, 115, 20, 20));

		public static readonly PaperdollBounds Helm = new PaperdollBounds(new Rectangle2D(80, 55, 20, 20));

		public static readonly PaperdollBounds InnerLegs = new PaperdollBounds(
			new Rectangle2D(70, 135, 20, 30),
			new Rectangle2D(65, 165, 20, 30),
			new Rectangle2D(90, 135, 20, 60));

		public static readonly PaperdollBounds InnerTorso = new PaperdollBounds(new Rectangle2D(70, 85, 45, 75));
		public static readonly PaperdollBounds MiddleTorso = new PaperdollBounds(new Rectangle2D(70, 85, 45, 75));
		public static readonly PaperdollBounds Neck = new PaperdollBounds(new Rectangle2D(80, 75, 20, 15));

		public static readonly PaperdollBounds OneHanded = new PaperdollBounds(
			new Rectangle2D(20, 25, 40, 90),
			new Rectangle2D(20, 115, 20, 20),
			new Rectangle2D(20, 135, 30, 40));

		public static readonly PaperdollBounds OuterLegs = new PaperdollBounds(
			new Rectangle2D(70, 135, 20, 30),
			new Rectangle2D(65, 165, 20, 30),
			new Rectangle2D(90, 135, 20, 60));

		public static readonly PaperdollBounds OuterTorso = new PaperdollBounds(new Rectangle2D(60, 80, 60, 140));

		public static readonly PaperdollBounds Pants = new PaperdollBounds(
			new Rectangle2D(70, 135, 20, 30),
			new Rectangle2D(65, 165, 20, 30),
			new Rectangle2D(90, 135, 20, 60));

		public static readonly PaperdollBounds Ring = new PaperdollBounds(new Rectangle2D(40, 110, 5, 5));
		public static readonly PaperdollBounds Shirt = new PaperdollBounds(new Rectangle2D(70, 85, 45, 45));

		public static readonly PaperdollBounds Shoes = new PaperdollBounds(
			new Rectangle2D(55, 190, 20, 35),
			new Rectangle2D(90, 190, 20, 35));

		public static readonly PaperdollBounds Talisman = new PaperdollBounds(new Rectangle2D(130, 70, 35, 35));

		public static readonly PaperdollBounds TwoHanded = new PaperdollBounds(
			new Rectangle2D(20, 25, 40, 90),
			new Rectangle2D(20, 115, 20, 20),
			new Rectangle2D(20, 135, 30, 40),
			new Rectangle2D(120, 90, 40, 25),
			new Rectangle2D(120, 120, 20, 15),
			new Rectangle2D(120, 135, 40, 30));

		public static readonly PaperdollBounds Waist = new PaperdollBounds(new Rectangle2D(70, 110, 45, 25));

		private static readonly Dictionary<Layer, PaperdollBounds> _LayerBounds = new Dictionary<Layer, PaperdollBounds>
		{
			{Layer.Arms, Arms},
			{Layer.Backpack, Backpack},
			{Layer.Bracelet, Bracelet},
			{Layer.Cloak, Cloak},
			{Layer.Earrings, Earrings},
			{Layer.Gloves, Gloves},
			{Layer.Helm, Helm},
			{Layer.InnerLegs, InnerLegs},
			{Layer.InnerTorso, InnerTorso},
			{Layer.MiddleTorso, MiddleTorso},
			{Layer.Neck, Neck},
			{Layer.OneHanded, OneHanded},
			{Layer.OuterLegs, OuterLegs},
			{Layer.OuterTorso, OuterTorso},
			{Layer.Pants, Pants},
			{Layer.Ring, Ring},
			{Layer.Shirt, Shirt},
			{Layer.Shoes, Shoes},
			{Layer.Talisman, Talisman},
			{Layer.TwoHanded, TwoHanded},
			{Layer.Waist, Waist}
		};

		public static PaperdollBounds Find(Layer layer)
		{
			if (!_LayerBounds.TryGetValue(layer, out var b))
			{
				b = Empty;
			}

			return b;
		}

		public Rectangle2D[] Bounds { get; private set; }

		private PaperdollBounds(params Rectangle2D[] bounds)
			: this()
		{
			Bounds = bounds ?? new Rectangle2D[0];
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return Bounds.Aggregate(Bounds.Length, (h, b) => (h * 397) ^ b.GetHashCode());
			}
		}

		public override bool Equals(object obj)
		{
			return (obj is PaperdollBounds && Equals((PaperdollBounds)obj)) || (obj is Layer && Equals((Layer)obj));
		}

		public bool Equals(Layer layer)
		{
			return _LayerBounds.ContainsKey(layer) && Equals(_LayerBounds[layer]);
		}

		public bool Equals(PaperdollBounds bounds)
		{
			return GetHashCode() == bounds.GetHashCode();
		}

		public IEnumerator<Rectangle2D> GetEnumerator()
		{
			return Bounds.GetEnumerator<Rectangle2D>();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Bounds.GetEnumerator();
		}

		public static bool operator ==(PaperdollBounds l, PaperdollBounds r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(PaperdollBounds l, PaperdollBounds r)
		{
			return !l.Equals(r);
		}

		public static bool operator ==(PaperdollBounds l, Layer r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(PaperdollBounds l, Layer r)
		{
			return !l.Equals(r);
		}

		public static bool operator ==(Layer l, PaperdollBounds r)
		{
			return r.Equals(l);
		}

		public static bool operator !=(Layer l, PaperdollBounds r)
		{
			return !r.Equals(l);
		}

		public static implicit operator Rectangle2D[](PaperdollBounds b)
		{
			return b.Bounds;
		}

		public static implicit operator PaperdollBounds(Layer l)
		{
			return Find(l);
		}
	}
}