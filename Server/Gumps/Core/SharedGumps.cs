#region Header
// **********
// ServUO - SharedGumps.cs
// **********
#endregion

#region References
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server.Gumps
{
	public partial class Gump
	{
		public List<Mobile> Users
		{
			get { return _Users; }
			set
			{
				if (_Users == value)
				{
					return;
				}

				_Users = value;
				Invalidate();
			}
		}

		public List<Mobile> Viewers
		{
			get { return _Viewers; }
			set
			{
				if (_Viewers == value)
				{
					return;
				}

				_Viewers = value;
				Invalidate();
			}
		}

		public bool RemoveUser(Mobile from)
		{
			Invalidate();

			bool result = false;

			if (_Users.Contains(from))
			{
				_Users.Remove(from);
				result = true;

				if (_Users.FirstOrDefault() == null && _Viewers.FirstOrDefault() == null)
				{
					SharedGump = false;
				}
			}

			return result;
		}

		public bool AddViewer(Mobile from)
		{
			Invalidate();

			if (_Viewers == null)
			{
				_Viewers = new List<Mobile>();
			}

			if (_Viewers.Contains(from))
			{
				return false;
			}

			_Viewers.Add(from);

			_SharedGump = true;

			return true;
		}

		public bool RemoveViewer(Mobile from)
		{
			Invalidate();

			bool result = false;

			if (_Viewers.Remove(from))
			{
				result = true;

				if (_Viewers.FirstOrDefault() == null && _Users.FirstOrDefault() == null)
				{
					SharedGump = false;
				}
			}

			return result;
		}
	}
}