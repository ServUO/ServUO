using System;
using System.Drawing;

using Server.Gumps;
using Server.Network;

namespace Server
{
	public sealed class VirtualCheck : Item
	{
		public static bool UseEditGump = false;

		public override bool IsVirtualItem { get { return true; } }

		public override double DefaultWeight { get { return 0; } }

		public override string DefaultName { get { return "Offer Of Currency"; } }

		private int _Plat;

		[CommandProperty(AccessLevel.Administrator)]
		public int Plat
		{
			get
			{
				return _Plat;
			}
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
				new EditGump(from, this).Send();
			}
			else
			{
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

			public Mobile User { get; private set; }
			public VirtualCheck Check { get; private set; }

			public EditGump(Mobile user, VirtualCheck check)
				: base(0, 0)
			{
				User = user;
				Check = check;

				Closable = true;
				Disposable = true;
				Dragable = true;
				Resizable = false;

				Close();
				CompileLayout();
			}

			public void Close()
			{
				User.CloseGump(GetType());
			}

			public void Send()
			{
				User.SendGump(this);
			}

			public void Refresh(bool recompile)
			{
				if (recompile)
				{
					CompileLayout();
				}

				Close();
				Send();
			}

			private void CompileLayout()
			{
				Entries.ForEach(e => e.Parent = null);
				Entries.Clear();

				AddPage(0);

				AddBackground(0, 0, 400, 160, 3500);

				// Title
				AddImageTiled(25, 35, 350, 3, 96);
				AddImage(10, 8, 113);
				AddImage(360, 8, 113);

				string title = String.Format(
					"<BASEFONT COLOR=#{0:X}><CENTER>TREASURY OF {1}</CENTER>",
					Color.DarkSlateGray.ToArgb(),
					User.RawName);

				AddHtml(40, 15, 320, 20, title, false, false);

				// Platinum Row
				AddBackground(15, 60, 175, 20, 9300);
				AddBackground(20, 45, 165, 30, 9350);
				AddItem(20, 45, 3826); // Plat
				AddLabel(60, 50, 0, User.Account.TotalPlat.ToString("#,0"));

				AddButton(195, 50, 95, 95, (int)Buttons.AllPlat, GumpButtonType.Reply, 0); // ->

				AddBackground(210, 60, 175, 20, 9300);
				AddBackground(215, 45, 165, 30, 9350);
				AddTextEntry(225, 50, 145, 20, 0, 0, Check.Plat.ToString());

				// Gold Row
				AddBackground(15, 100, 175, 20, 9300);
				AddBackground(20, 85, 165, 30, 9350);
				AddItem(20, 85, 3823); // Gold
				AddLabel(60, 90, 0, User.Account.TotalGold.ToString("#,0"));

				AddButton(195, 90, 95, 95, (int)Buttons.AllGold, GumpButtonType.Reply, 0); // ->

				AddBackground(210, 100, 175, 20, 9300);
				AddBackground(215, 85, 165, 30, 9350);
				AddTextEntry(225, 90, 145, 20, 0, 1, Check.Gold.ToString());

				// Buttons
				AddButton(20, 128, 12006, 12007, (int)Buttons.Close, GumpButtonType.Reply, 0);
				AddButton(215, 128, 12003, 12004, (int)Buttons.Clear, GumpButtonType.Reply, 0);
				AddButton(305, 128, 12000, 12002, (int)Buttons.Accept, GumpButtonType.Reply, 0);
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				if (sender.Mobile != User)
				{
					Close();
					return;
				}

				var platText = info.GetTextEntry(0).Text;
				var goldText = info.GetTextEntry(1).Text;

				int plat, gold;
				bool refresh = false;

				if (Int32.TryParse(platText, out plat))
				{
					if (plat <= User.Account.TotalPlat)
					{
						Check.Plat = plat;
					}
					else
					{
						Check.Plat = User.Account.TotalPlat;
						User.SendMessage("You do not have that much platinum.");
						refresh = true;
					}
				}
				else
				{
					User.SendMessage("That is not a valid amount of platinum.");
					refresh = true;
				}

				if (Int32.TryParse(goldText, out gold))
				{
					if (gold <= User.Account.TotalGold)
					{
						Check.Gold = gold;
					}
					else
					{
						Check.Gold = User.Account.TotalGold;
						User.SendMessage("You do not have that much gold.");
						refresh = true;
					}
				}
				else
				{
					User.SendMessage("That is not a valid amount of gold.");
					refresh = true;
				}

				if (refresh)
				{
					Refresh(true);
				}
				else
				{
					User.SendMessage("Your offer has been updated.");
					Close();
				}
			}
		}
	}
}