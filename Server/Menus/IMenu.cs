#region Header
// **********
// ServUO - IMenu.cs
// **********
#endregion

#region References
using Server.Network;
#endregion

namespace Server.Menus
{
	public interface IMenu
	{
		int Serial { get; }
		int EntryLength { get; }
		void SendTo(NetState state);
		void OnCancel(NetState state);
		void OnResponse(NetState state, int index);
	}
}