#region Header
// **********
// ServUO - Toolbar.cs
// **********
#endregion

#region References
using System;

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;

using Services.Toolbar.Core;
#endregion

namespace Services.Toolbar.Gumps
{
	public class ToolbarGump : Gump
	{
		/*******************
        *	BUTTON ID'S
        * 0 - Close
        * 1 - Edit
        *******************/

		private readonly ToolbarInfo _Info;

		public int InitOptsW, InitOptsH;

		public ToolbarGump(ToolbarInfo info)
			: base(0, 28)
		{
			_Info = info;

			if (_Info.Lock)
			{
				Closable = false;
				Disposable = false;
			}

			int offset = GumpIDs.Misc[(int)GumpIDs.MiscIDs.ButtonOffset].Content[_Info.Skin, 0];
			int bx = ((offset * 2) + (_Info.Rows * 110)), by = ((offset * 2) + (_Info.Collumns * 24)), byx = by, cy = 0;

			SetCoords(offset);

			if (_Info.Reverse)
			{
				cy = InitOptsH;
				by = 0;
			}

			AddPage(0);
			AddPage(1);

			if (_Info.Stealth)
			{
				AddMinimized(by, offset);
				AddPage(2);
			}

			AddInitOpts(by, offset);

			AddBackground(0, cy, bx, byx, GumpIDs.Misc[(int)GumpIDs.MiscIDs.Background].Content[_Info.Skin, 0]);

			string font = GumpIDs.Fonts[_Info.Font];

			if (_Info.Phantom)
			{
				font += "<BASEFONT COLOR=#FFFFFF>";
			}

			int temp = 0, x, y;

			for (int i = 0; i < _Info.Rows * _Info.Collumns; i++)
			{
				x = offset + ((i % _Info.Rows) * 110);
				y = offset + (int)(Math.Floor((double)(i / _Info.Rows)) * 24) + cy;

				AddButton(x + 1, y, 2445, 2445, temp + 10, GumpButtonType.Reply, 0);
				AddBackground(x, y, 110, 24, GumpIDs.Misc[(int)GumpIDs.MiscIDs.Buttonground].Content[_Info.Skin, 0]);

				if (_Info.Phantom)
				{
					AddImageTiled(x + 2, y + 2, 106, 20, 2624); // Alpha Area 1_1
					AddAlphaRegion(x + 2, y + 2, 106, 20); // Alpha Area 1_2
				}

				AddHtml(x + 5, y + 3, 100, 20, String.Format("<center>{0}{1}", font, _Info.Entries[temp]), false, false);
				//AddLabelCropped(x + 5, y + 3, 100, 20, GumpIDs.Misc[(int)GumpIDs.MiscIDs.Color].Content[p_Skin,0], Commands[temp]); 

				if (i % _Info.Rows == _Info.Rows - 1)
				{
					temp += 9 - _Info.Rows;
				}

				++temp;
			}

			/*TEST---
            0%5 == 0
            1%5 == 1
            2%5 == 2
            3%5 == 3
            4%5 == 4
            5%5 == 0
            END TEST---*/

			if (_Info.Stealth)
			{
				return;
			}

			AddPage(2);
			AddMinimized(by, offset);
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			Mobile mob = sender.Mobile;

			switch (info.ButtonID)
			{
				case 0: // Close
					break;
				case 1: // Edit
					{
						mob.SendGump(this);
						mob.CloseGump(typeof(ToolbarEdit));
						mob.SendGump(new ToolbarEdit(_Info));
					}
					break;
				default: // Command
					{
						mob.SendGump(this);

                        if (_Info.Entries.Count <= 0)
                            return;

                        if (info.ButtonID - 10 <= 0)
                            return;

                        if (_Info.Entries[info.ButtonID - 10].StartsWith(CommandSystem.Prefix))
						{
							mob.SendMessage(_Info.Entries[info.ButtonID - 10]);
							CommandSystem.Handle(mob, _Info.Entries[info.ButtonID - 10]);
						}
						else
						{
							//SpeechEventArgs args = new SpeechEventArgs( mob, Commands[info.ButtonID - 10], MessageType.Regular, mob.SpeechHue, null );
							mob.DoSpeech(_Info.Entries[info.ButtonID - 10], new int[0], MessageType.Regular, mob.SpeechHue);
							//mob.Say(Commands[info.ButtonID - 10]);
						}
					}
					break;
			}
		}

		/// <summary>
		///     Sets coordinates and sizes.
		/// </summary>
		public void SetCoords(int offset)
		{
			InitOptsW = 50 + (offset * 2) + GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Minimize].Content[_Info.Skin, 2] + 5 +
						GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Customize].Content[_Info.Skin, 2];
			InitOptsH = (offset * 2) + GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Minimize].Content[_Info.Skin, 3];

			if (GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Customize].Content[_Info.Skin, 3] + (offset * 2) > InitOptsH)
			{
				InitOptsH = GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Customize].Content[_Info.Skin, 3] + (offset * 2);
			}
		}

		/// <summary>
		///     Adds initial options.
		/// </summary>
		public void AddInitOpts(int y, int offset)
		{
			AddBackground(0, y, InitOptsW, InitOptsH, GumpIDs.Misc[(int)GumpIDs.MiscIDs.Background].Content[_Info.Skin, 0]);
			AddButton(
				offset,
				y + offset,
				GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Minimize].Content[_Info.Skin, 0],
				GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Minimize].Content[_Info.Skin, 1],
				0,
				GumpButtonType.Page,
				_Info.Stealth ? 1 : 2);
			AddButton(
				offset + GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Minimize].Content[_Info.Skin, 2] + 5,
				y + offset,
				GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Customize].Content[_Info.Skin, 0],
				GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Customize].Content[_Info.Skin, 1],
				1,
				GumpButtonType.Reply,
				0); // 1 Edit
		}

		/// <summary>
		///     Adds minimized page.
		/// </summary>
		public void AddMinimized(int y, int offset)
		{
			AddBackground(0, y, InitOptsW, InitOptsH, GumpIDs.Misc[(int)GumpIDs.MiscIDs.Background].Content[_Info.Skin, 0]);
			AddButton(
				offset,
				y + offset,
				GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Maximize].Content[_Info.Skin, 0],
				GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Maximize].Content[_Info.Skin, 1],
				0,
				GumpButtonType.Page,
				_Info.Stealth ? 2 : 1);
			AddButton(
				offset + GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Minimize].Content[_Info.Skin, 2] + 5,
				y + offset,
				GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Customize].Content[_Info.Skin, 0],
				GumpIDs.Buttons[(int)GumpIDs.ButtonIDs.Customize].Content[_Info.Skin, 1],
				1,
				GumpButtonType.Reply,
				0); // 1 Edit
		}
	}
}