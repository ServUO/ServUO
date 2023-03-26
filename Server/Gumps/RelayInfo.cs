namespace Server.Gumps
{
	public class TextRelay
	{
		public int EntryID { get; }
		public string Text { get; }

		public TextRelay(int entryID, string text)
		{
			EntryID = entryID;
			Text = text;
		}
	}

	public class RelayInfo
	{
		public int ButtonID { get; }

		public int[] Switches { get; }

		public TextRelay[] TextEntries { get; }

		public RelayInfo(int buttonID, int[] switches, TextRelay[] textEntries)
		{
			ButtonID = buttonID;
			Switches = switches;
			TextEntries = textEntries;
		}

		public bool IsSwitched(int switchID)
		{
			for (var i = 0; i < Switches.Length; ++i)
			{
				if (Switches[i] == switchID)
				{
					return true;
				}
			}

			return false;
		}

		public TextRelay GetTextEntry(int entryID)
		{
			for (var i = 0; i < TextEntries.Length; ++i)
			{
				if (TextEntries[i].EntryID == entryID)
				{
					return TextEntries[i];
				}
			}

			return null;
		}
	}
}