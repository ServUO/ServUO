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
	public abstract class GumpEntry
	{
		private Gump _Parent;

		public Gump Parent
		{
			get { return _Parent; }
			set
			{
				if (_Parent == value)
				{
					return;
				}

				if (_Parent != null)
				{
					_Parent.Remove(this);
				}

				_Parent = value;

				if (_Parent != null)
				{
					_Parent.Add(this);
				}
			}
		}

		protected void Delta(ref int var, int val)
		{
			if (var == val)
			{
				return;
			}

			var = val;

			if (_Parent != null)
			{
				_Parent.Invalidate();
			}
		}

		protected void Delta(ref bool var, bool val)
		{
			if (var == val)
			{
				return;
			}

			var = val;

			if (_Parent != null)
			{
				_Parent.Invalidate();
			}
		}

		protected void Delta(ref string var, string val)
		{
			if (var == val)
			{
				return;
			}

			var = val;

			if (_Parent != null)
			{
				_Parent.Invalidate();
			}
		}

		public abstract string Compile();
		public abstract void AppendTo(IGumpWriter disp);
	}
}