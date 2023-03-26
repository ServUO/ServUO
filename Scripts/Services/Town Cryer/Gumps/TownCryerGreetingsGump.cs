using System;
using System.Collections.Generic;

using Server.Gumps;
using Server.Mobiles;

namespace Server.Services.TownCryer
{
	public class TownCryerGreetingsGump : BaseTownCryerGump
	{
		public int Page { get; private set; }
		public int Pages => TownCryerSystem.GreetingsEntries.Count;

		public TownCryerGreetingEntry Entry { get; private set; }

		public TownCryerGreetingsGump(PlayerMobile pm, TownCrier cryer, int page = 0)
			: base(pm, cryer)
		{
			Page = page;
		}

		public override void AddGumpLayout()
		{
			base.AddGumpLayout();

			var list = new List<TownCryerGreetingEntry>(TownCryerSystem.GreetingsEntries);
			list.Sort();

			Entry = list[0];

			if (Page >= 0 && Page < list.Count)
			{
				Entry = list[Page];
			}

			var y = 150;

			if (Entry?.Title.IsEmpty == false)
			{
				if (Entry.Title.Number > 0)
				{
					AddHtmlLocalized(78, y, 700, 400, CenterLoc, Entry.Title.ToString(), C32216(0x080808), false, false);
				}
				else
				{
					AddHtml(78, y, 700, 400, Entry.Title.ToString(), false, false);
				}

				y += 40;
			}

			// For now, we're only supporting a cliloc (hard coded greetings per EA) or string (Custom) entries. Not both.
			// Html tags will needed to be added when creating the entry, this will not auto format it for you.
			if (Entry?.Body1.Number > 0)
			{
				AddHtmlLocalized(78, y, 700, 400, Entry.Body1.Number, false, false);
			}
			else if (!String.IsNullOrEmpty(Entry?.Body1.String))
			{
				var str = Entry.Body1.String;

				if (!String.IsNullOrEmpty(Entry.Body2))
				{
					if (!str.EndsWith("<br>"))
					{
						str += " ";
					}

					str += Entry.Body2;
				}

				if (!String.IsNullOrEmpty(Entry.Body3))
				{
					if (!str.EndsWith("<br>"))
					{
						str += " ";
					}

					str += Entry.Body3;
				}

				AddHtml(78, y, 700, 400, str, false, false);
			}

			if (Entry?.Expires != DateTime.MinValue)
			{
				AddHtmlLocalized(50, 550, 200, 20, 1060658, $"Created\t{Entry.Created.ToShortDateString()}", 0, false, false);
				AddHtmlLocalized(50, 570, 200, 20, 1060659, $"Expires\t{Entry.Expires.ToShortDateString()}", 0, false, false);
			}

			AddButton(350, 570, 0x605, 0x606, 1, GumpButtonType.Reply, 0);
			AddButton(380, 570, 0x609, 0x60A, 2, GumpButtonType.Reply, 0);
			AddButton(430, 570, 0x607, 0x608, 3, GumpButtonType.Reply, 0);
			AddButton(455, 570, 0x603, 0x604, 4, GumpButtonType.Reply, 0);

			AddHtml(395, 570, 35, 20, Center($"{Page + 1}/{Pages}"), false, false);

			AddButton(525, 625, 0x5FF, 0x600, 5, GumpButtonType.Reply, 0);
			AddHtmlLocalized(550, 625, 300, 20, 1158386, false, false); // Close and do not show this version again

			if (Entry?.Link != null)
			{
				if (!String.IsNullOrEmpty(Entry.LinkText))
				{
					AddHtml(50, 490, 745, 40, $"<a href=\"{Entry.Link}\">{Entry.LinkText}</a>", false, false);
				}
				else
				{
					AddHtml(50, 490, 745, 40, $"<a href=\"{Entry.Link}\">{Entry.Link}</a>", false, false);
				}
			}
			/*
			if (TownCryerSystem.HasCustomEntries())
            {
                AddButton(40, 615, 0x603, 0x604, 6, GumpButtonType.Reply, 0);
                AddHtmlLocalized(68, 615, 300, 20, 1060660, $"Sort By\t{Sort}", 0, false, false);
            }
			*/
			if (User.AccessLevel >= AccessLevel.Administrator)
			{
				if (Entry?.CanEdit == true)
				{
					AddButton(40, 601, 0x603, 0x604, 7, GumpButtonType.Reply, 0);
					AddHtml(68, 601, 300, 20, "Edit Greeting", false, false);
				}

				AddButton(40, 623, 0x603, 0x604, 8, GumpButtonType.Reply, 0);
				AddHtml(68, 623, 300, 20, "New Greeting", false, false);

				AddButton(40, 645, 0x603, 0x604, 9, GumpButtonType.Reply, 0);
				AddHtml(68, 645, 300, 20, "Entry Props", false, false);
			}

			ColUtility.Free(list);
		}

		public override void OnResponse(RelayInfo info)
		{
			var button = info.ButtonID;

			switch (button)
			{
				case 0: break;
				case 1: // <<
					Page = 0;
					Refresh();
					break;
				case 2: // <
					Page = Math.Max(0, Page - 1);
					Refresh();
					break;
				case 3: // >
					Page = Math.Min(Pages - 1, Page + 1);
					Refresh();
					break;
				case 4: // >>
					Page = Pages - 1;
					Refresh();
					break;
				case 5: // No Show
					TownCryerSystem.AddExempt(User);
					break;
				case 7:
					if (Entry != null)
					{
						_ = SendGump(new CreateGreetingEntryGump(User, Cryer, Entry));
					}
					else
					{
						Refresh();
					}
					break;
				case 8:
					_ = SendGump(new CreateGreetingEntryGump(User, Cryer));
					break;
				case 9:
					Refresh();

					if (Entry != null)
					{
						_ = User.SendGump(new PropertiesGump(User, Entry));
					}
					break;
			}
		}
	}
}
