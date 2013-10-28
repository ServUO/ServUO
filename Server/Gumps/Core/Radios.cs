#region Header
// **********
// ServUO - Radios.cs
// **********
#endregion

#region References
using System.Linq;
#endregion

namespace Server.Gumps
{
	public partial class Gump
	{
		public void AddRadio(int x, int y, int inactiveID, int activeID, bool initialState, int switchID, string name = "")
		{
			Add(new GumpRadio(x, y, inactiveID, activeID, initialState, switchID, null, name));
		}

		public void AddRadio(
			int x, int y, int inactiveID, int activeID, bool initialState, int switchID, GumpResponse callback, string name = "")
		{
			Add(new GumpRadio(x, y, inactiveID, activeID, initialState, switchID, callback, name));
		}

		public void AddRadio(
			int x, int y, int inactiveID, int activeID, bool initialState, GumpResponse callback, string name = "")
		{
			Add(new GumpRadio(x, y, inactiveID, activeID, initialState, NewID(), callback, name));
		}

		public bool GetCheck(int id)
		{
			foreach (GumpCheck entry in _Entries.OfType<GumpCheck>().Where(entry => entry.EntryID == id))
			{
				return entry.InitialState;
			}

			return false;
		}

		public bool GetCheck(string name)
		{
			foreach (GumpCheck entry in _Entries.OfType<GumpCheck>().Where(entry => entry.Name == name))
			{
				return entry.InitialState;
			}

			return false;
		}
	}
}