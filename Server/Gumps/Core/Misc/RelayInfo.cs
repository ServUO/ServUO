#region Header
// **********
// ServUO - RelayInfo.cs
// **********
#endregion

#region References
using System.Linq;
#endregion

namespace Server.Gumps
{
	public class TextRelay
	{
		private readonly int _EntryID;
		private readonly string _Text;

		public TextRelay(int entryID, string text)
		{
			_EntryID = entryID;
			_Text = text;
		}

		public int EntryID { get { return _EntryID; } }
		public string Text { get { return _Text; } }
	}

	public class RelayInfo
	{
		private readonly int _ButtonID;
		private readonly int[] _Switches;
		private readonly TextRelay[] _TextEntries;

		public RelayInfo(int buttonID, int[] switches, TextRelay[] textEntries)
		{
			_ButtonID = buttonID;
			_Switches = switches;
			_TextEntries = textEntries;
		}

		public int ButtonID { get { return _ButtonID; } }
		public int[] Switches { get { return _Switches; } }
		public TextRelay[] TextEntries { get { return _TextEntries; } }

		public bool IsSwitched(int switchID)
		{
			return _Switches.Any(t => t == switchID);
		}

		public TextRelay GetTextEntry(int entryID)
		{
			return _TextEntries.FirstOrDefault(t => t.EntryID == entryID);
		}
	}
}