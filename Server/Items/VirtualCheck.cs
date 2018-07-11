#region Header
// **********
// ServUO - VirtualCheck.cs
// **********
#endregion

#region References
using System;
using System.Drawing;

using Server.Accounting;
using Server.Gumps;
using Server.Network;
#endregion

namespace Server
{
	public sealed class VirtualCheck : Item
	{
		public static bool UseEditGump = false;

		public override bool IsVirtualItem { get { return true; } }

		public override bool DisplayWeight { get { return false; } }
		public override bool DisplayLootType { get { return false; } }

		public override double DefaultWeight { get { return 0; } }

		public override string DefaultName { get { return "Offer Of Currency"; } }

		public EditGump Editor { get; private set; }

		private int _Plat;

		[CommandProperty(AccessLevel.Administrator)]
		public int Plat
		{
			get { return _Plat; }
			set
			{
				_Plat = value;
				InvalidateProperties();
			}
		}

		private int _Gold;

		[CommandProperty(AccessLevel.Administrator)]
		public int Gold
		{
			get { return _Gold; }
			set
			{
				_Gold = value;
				InvalidateProperties();
			}
		}

		public VirtualCheck()
			: this(0, 0)
		{ }

		public VirtualCheck(int plat, int gold)
			: base(0x14F0)
		{
			Plat = plat;
			Gold = gold;

			Movable = false;
		}

		public VirtualCheck(Serial serial)
			: base(serial)
		{ }

		public override bool IsAccessibleTo(Mobile check)
		{
			var c = GetSecureTradeCont();

			if (check == null || c == null)
			{
				return base.IsAccessibleTo(check);
			}

			return c.RootParent == check && IsChildOf(c);
		}

		public override void OnDoubleClickSecureTrade(Mobile from)
		{
			if (UseEditGump && IsAccessibleTo(from))
			{
				if (Editor == null || Editor.Check == null || Editor.Check.Deleted)
				{
					Editor = new EditGump(from, this);
					Editor.Send();
				}
				else
				{
					Editor.Refresh(true);
				}
			}
			else
			{
				if (Editor != null)
				{
					Editor.Close();
					Editor = null;
				}

				base.OnDoubleClickSecureTrade(from);
			}
		}

		public override void OnSingleClick(Mobile from)
		{
			LabelTo(from, "Offer: {0:#,0} platinum, {1:#,0} gold", Plat, Gold);
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1060738, String.Format("{0:#,0} platinum, {1:#,0} gold", Plat, Gold)); // value: ~1_val~
		}

		public void UpdateTrade(Mobile user)
		{
			var c = GetSecureTradeCont();

			if (c == null || c.Trade == null)
			{
				return;
			}

			if (user == c.Trade.From.Mobile)
			{
				c.Trade.UpdateFromCurrency();
			}
			else if (user == c.Trade.To.Mobile)
			{
				c.Trade.UpdateToCurrency();
			}

			c.ClearChecks();
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if (Editor != null)
			{
				Editor.Close();
				Editor = null;
			}
		}

		public override void Serialize(GenericWriter writer)
		{ }

		public override void Deserialize(GenericReader reader)
		{
			Delete();
		}

		public class EditGump : Gump
		{
			public enum Buttons
			{
				Close,
				Clear,
				Accept,
				AllPlat,
				AllGold
			}

			private int _Plat, _Gold;

			public Mobile User { get; private set; }
			public VirtualCheck Check { get; private set; }

			public EditGump(Mobile user, VirtualCheck check)
				: base(50, 50)
			{
				User = user;
				Check = check;

				_Plat = Check.Plat;
				_Gold = Check.Gold;

				Closable = true;
				Disposable = true;
				Dragable = true;
				Resizable = false;

				User.CloseGump(GetType());

				CompileLayout();
			}

			public override void OnServerClose(NetState owner)
			{
				base.OnServerClose(owner);

				if (Check != null && !Check.Deleted)
				{
					Check.UpdateTrade(User);
				}
			}

			public void Close()
			{
				User.CloseGump(GetType());

				if (Check != null && !Check.Deleted)
				{
					Check.UpdateTrade(User);
				}
				else
				{
					Check = null;
				}
			}

			public void Send()
			{
				if (Check != null && !Check.Deleted)
				{
					User.SendGump(this);
				}
				else
				{
					Close();
				}
			}

			public void Refresh(bool recompile)
			{
				if (Check == null || Check.Deleted)
				{
					Close();
					return;
				}

				if (recompile)
				{
					CompileLayout();
				}

				Close();
				Send();
			}

			private void CompileLayout()
			{
				if (Check == null || Check.Deleted)
				{
					return;
				}

				Entries.ForEach(e => e.Parent = null);
				Entries.Clear();

				AddPage(0);

				AddBackground(0, 0, 400, 160, 3500);

				// Title
				AddImageTiled(25, 35, 350, 3, 96);
				AddImage(10, 8, 113);
				AddImage(360, 8, 113);

				var title = String.Format(
					"<BASEFONT COLOR=#{0:X6}><CENTER>BANK OF {1}</CENTER>",
					Color.DarkSlateGray.ToArgb() & 0x00FFFFFF,
					User.RawName.ToUpper());

				AddHtml(40, 15, 320, 20, title, false, false);

				// Platinum Row
				AddBackground(15, 60, 175, 20, 9300);
				AddBackground(20, 45, 165, 30, 9350);
				AddItem(20, 45, 3826); // Plat
				AddLabel(60, 50, 0, User.Account.TotalPlat.ToString("#,0"));

				AddButton(195, 50, 95, 95, (int)Buttons.AllPlat, GumpButtonType.Reply, 0); // ->

				AddBackground(210, 60, 175, 20, 9300);
				AddBackground(215, 45, 165, 30, 9350);
				AddTextEntry(225, 50, 145, 20, 0, 0, _Plat.ToString(), User.Account.TotalPlat.ToString().Length);

				// Gold Row
				AddBackground(15, 100, 175, 20, 9300);
				AddBackground(20, 85, 165, 30, 9350);
				AddItem(20, 85, 3823); // Gold
				AddLabel(60, 90, 0, User.Account.TotalGold.ToString("#,0"));

				AddButton(195, 90, 95, 95, (int)Buttons.AllGold, GumpButtonType.Reply, 0); // ->

				AddBackground(210, 100, 175, 20, 9300);
				AddBackground(215, 85, 165, 30, 9350);
				AddTextEntry(225, 90, 145, 20, 0, 1, _Gold.ToString(), User.Account.TotalGold.ToString().Length);

				// Buttons
				AddButton(20, 128, 12006, 12007, (int)Buttons.Close, GumpButtonType.Reply, 0);
				AddButton(215, 128, 12003, 12004, (int)Buttons.Clear, GumpButtonType.Reply, 0);
				AddButton(305, 128, 12000, 12002, (int)Buttons.Accept, GumpButtonType.Reply, 0);
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				if (Check == null || Check.Deleted || sender.Mobile != User)
				{
					Close();
					return;
				}

				bool refresh = false, updated = false;

				switch ((Buttons)info.ButtonID)
				{
					case Buttons.Close:
						break;
					case Buttons.Clear:
					{
						_Plat = _Gold = 0;
						refresh = true;
					}
						break;
					case Buttons.Accept:
					{
						var platText = info.GetTextEntry(0).Text;
						var goldText = info.GetTextEntry(1).Text;

						if (!Int32.TryParse(platText, out _Plat))
						{
							User.SendMessage("That is not a valid amount of platinum.");
							refresh = true;
						}
						else if (!Int32.TryParse(goldText, out _Gold))
						{
							User.SendMessage("That is not a valid amount of gold.");
							refresh = true;
						}
						else
						{
							var cur = User.Account.TotalCurrency;
							var off = _Plat + (_Gold / Math.Max(1.0, AccountGold.CurrencyThreshold));

							if (off > cur)
							{
								_Plat = User.Account.TotalPlat;
								_Gold = User.Account.TotalGold;
								User.SendMessage("You do not have that much currency.");
								refresh = true;
							}
							else
							{
								Check.Plat = _Plat;
								Check.Gold = _Gold;
								updated = true;
							}
						}
					}
						break;
					case Buttons.AllPlat:
					{
						_Plat = User.Account.TotalPlat;
						refresh = true;
					}
						break;
					case Buttons.AllGold:
					{
						_Gold = User.Account.TotalGold;
						refresh = true;
					}
						break;
				}

				if (updated)
				{
					User.SendMessage("Your offer has been updated.");
				}

				if (refresh && Check != null && !Check.Deleted)
				{
					Refresh(true);
					return;
				}

				Close();
			}
		}
	}
}