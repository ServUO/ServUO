#region Header
// **********
// ServUO - GumpEntry.cs
// **********
#endregion

#region References
using Server.Network;
#endregion

namespace Server.Gumps
{
	public abstract class GumpEntry : IGumpComponent
	{
		private IGumpContainer _Container;

		public IGumpContainer Container
		{
			get { return _Container; }
			set
			{
				if (_Container == value)
				{
					return;
				}

				if (_Container != null)
				{
					_Container.Remove(this);
				}

				_Container = value;

				if (_Container != null)
				{
					_Container.Add(this);
				}
			}
		}

		//Legacy Support
		public Gump Parent { get { return Container as Gump; } set { Container = value; } }

		public virtual int X { get; set; }
		public virtual int Y { get; set; }

		public abstract string Compile();

		public abstract void AppendTo(IGumpWriter disp);

		protected internal void AssignID()
		{
			if (this is IInputEntry && ((IInputEntry)this).EntryID < 0 && _Container != null && _Container.RootParent != null)
			{
				((IInputEntry)this).EntryID = _Container.RootParent.NewID();
			}
		}

		protected void Delta<T>(ref T field, T val)
		{
			if (ReferenceEquals(field, val) || field.Equals(val))
			{
				return;
			}

			field = val;

			if (_Container != null)
			{
				_Container.Invalidate();
			}
		}
	}
}