#region Header
// **********
// ServUO - Gumpling.cs
// **********
#endregion

#region References
using System.Collections.Generic;
#endregion

namespace Server.Gumps
{
	public abstract class Gumpling : IGumpContainer, IGumpComponent
	{
		private int _X;
		private int _Y;

		public int X
		{
			get { return _X; }
			set
			{
				int offset = value - X;
				_X = value;

				foreach (GumpEntry g in _Entries)
				{
					g.X += offset;
				}
			}
		}

		public int Y
		{
			get { return _Y; }
			set
			{
				int offset = value - Y;
				_Y = value;

				foreach (GumpEntry g in _Entries)
				{
					g.Y += offset;
				}
			}
		}

		private readonly List<IGumpComponent> _Entries;

		private IGumpContainer _MContainer;

		public IGumpContainer Container
		{
			get { return _MContainer; }
			set
			{
				if (_MContainer != value)
				{
					if (_MContainer != null)
					{
						_MContainer.Remove(this);
					}

					_MContainer = value;

					if (_MContainer != null)
					{
						_MContainer.Add(this);
					}
				}
			}
		}

		public Gumpling(int x, int y)
		{
			_X = x;
			_Y = y;

			_Entries = new List<IGumpComponent>();
		}

		public Gump RootParent { get { return Container.RootParent; } }

		public void Add(IGumpComponent g)
		{
			if (g.Container == null)
			{
				g.Container = this;
			}

			if (!_Entries.Contains(g))
			{
				g.X += _X;
				g.Y += _Y;

				_Entries.Add(g);
				Invalidate();
			}
		}

		public void Remove(IGumpComponent g)
		{
			_Entries.Remove(g);
			g.Container = null;
			Invalidate();
		}

		public virtual void Invalidate()
		{ }

		public void AddToGump(Gump gump)
		{
			foreach (IGumpComponent g in _Entries)
			{
				gump.Add(g);
			}
		}

		public void RemoveFromGump(Gump gump)
		{
			foreach (IGumpComponent g in _Entries)
			{
				gump.Remove(g);
			}
		}
	}
}