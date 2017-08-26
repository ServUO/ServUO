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

		public ToolbarGump(ToolbarInfo info, Mobile m)
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

            NetState ns = m.NetState;

            for (int i = 0; i < _Info.Rows * _Info.Collumns; i++)
			{
				x = offset + ((i % _Info.Rows) * 110);
				y = offset + (int)(Math.Floor((double)(i / _Info.Rows)) * 24) + cy;

                if (ns.IsEnhancedClient)
                {
                    AddButton(x + 4, y + 5, 2103, 2104, temp + 10, GumpButtonType.Reply, 0);//4005, 4007
                }
                else
                {
                    AddButton(x + 1, y, 2445, 2445, temp + 10, GumpButtonType.Reply, 0);
                }

                AddBackground(x, y, 110, 24, GumpIDs.Misc[(int)GumpIDs.MiscIDs.Buttonground].Content[_Info.Skin, 0]);

				if (_Info.Phantom)
				{
					AddImageTiled(x + 2, y + 2, 106, 20, 2624); // Alpha Area 1_1
					AddAlphaRegion(x + 2, y + 2, 106, 20); // Alpha Area 1_2
				}

                if (ns.IsEnhancedClient)
                {
                    AddHtml(x + 30, y + 3, 100, 20, String.Format("<center>{0}{1}", font, _Info.Entries[temp]), false, false);
                }
                else
                {
                    AddHtml(x + 5, y + 3, 100, 20, String.Format("<center>{0}{1}", font, _Info.Entries[temp]), false, false);
                }

                if (i % _Info.Rows == _Info.Rows - 1)
				{
					temp += 9 - _Info.Rows;
				}

				++temp;
			}

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

						int buttonPressedIndex = info.ButtonID - 10;

                        if (buttonPressedIndex >= _Info.Entries.Count)
                            return;

                        String buttonText = _Info.Entries[buttonPressedIndex];

                        if (buttonText.StartsWith(CommandSystem.Prefix))
						{
							mob.SendMessage(buttonText);
							CommandSystem.Handle(mob, buttonText);
						}
						else
						{
							mob.DoSpeech(buttonText, new int[0], MessageType.Regular, mob.SpeechHue);
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
