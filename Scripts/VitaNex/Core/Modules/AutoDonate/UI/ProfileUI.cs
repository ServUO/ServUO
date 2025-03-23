#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Server;
using Server.Accounting;
using Server.Gumps;

using VitaNex.Collections;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
using VitaNex.Text;
#endregion

namespace VitaNex.Modules.AutoDonate
{
	public class DonationProfileUI : TreeGump
	{
		public static void DisplayTo(Mobile user, DonationProfile profile, DonationTransaction trans)
		{
			DisplayTo(user, profile, false, trans);
		}

		public static void DisplayTo(Mobile user, DonationProfile profile, bool refreshOnly, DonationTransaction trans)
		{
			var node = trans.IsPending
				? ("Transactions|Pending|" + trans.ID)
				: trans.IsProcessed //
					? ("Transactions|Claim|" + trans.ID)
					: !trans.Hidden //
						? ("History|" + trans.FullPath)
						: "History";

			DisplayTo(user, profile, refreshOnly, node);
		}

		public static void DisplayTo(Mobile user)
		{
			DisplayTo(user, null);
		}

		public static void DisplayTo(Mobile user, DonationProfile profile)
		{
			DisplayTo(user, profile, String.Empty);
		}

		public static void DisplayTo(Mobile user, DonationProfile profile, string node)
		{
			DisplayTo(user, profile, false, node);
		}

		public static void DisplayTo(Mobile user, DonationProfile profile, bool refreshOnly)
		{
			DisplayTo(user, profile, refreshOnly, String.Empty);
		}

		public static void DisplayTo(Mobile user, DonationProfile profile, bool refreshOnly, string node)
		{
			var info = EnumerateInstances<DonationProfileUI>(user).FirstOrDefault(g => g != null && !g.IsDisposed && g.IsOpen);

			if (info == null)
			{
				if (refreshOnly)
				{
					return;
				}

				info = new DonationProfileUI(user, profile);
			}
			else if (profile != null)
			{
				info.Profile = profile;
			}

			if (!String.IsNullOrWhiteSpace(node))
			{
				info.SelectedNode = node;
			}

			info.Refresh(true);
		}

		private bool _Admin;

		private int _DonationTier;
		private int _DonationCount;
		private double _DonationValue;
		private long _DonationCredit;

		private int[,] _Indicies;
		private List<DonationTransaction> _Transactions;

		public DonationProfile Profile { get; set; }

		public DonationProfileUI(Mobile user, DonationProfile profile = null)
			: base(user, null, null, null, null, null, "Donations")
		{
			_Indicies = new int[6, 2];

			Profile = profile;

			CanMove = true;
			CanClose = true;
			CanDispose = true;
			CanResize = false;

			Width = 900;
			Height = 500;

			ForceRecompile = true;
		}

		public override void AssignCollections()
		{
			base.AssignCollections();

			if (_Transactions == null)
			{
				ObjectPool.Acquire(out _Transactions);
			}
		}

		protected override void MainButtonHandler(GumpButton b)
		{
			if (_Admin)
			{
				new DonationAdminUI(User, Hide()).Send();
				return;
			}

			base.MainButtonHandler(b);
		}

		protected override void Compile()
		{
			_Admin = User.AccessLevel >= AutoDonate.Access;

			if (Profile == null || Profile.Account == null || (!_Admin && User.AccessLevel <= Profile.Account.AccessLevel &&
															   !Profile.Account.IsSharedWith(User.Account)))
			{
				Profile = AutoDonate.EnsureProfile(User.Account);
			}

			if (Profile != null)
			{
				_DonationTier = Profile.Tier;
				_DonationValue = Profile.TotalValue;
				_DonationCredit = Profile.TotalCredit;
				_DonationCount = _Admin ? Profile.Transactions.Count : Profile.Visible.Count();

				Title = "Donations: " + Profile.Account;
			}

			base.Compile();
		}

		protected override void CompileNodes(Dictionary<TreeGumpNode, Action<Rectangle, int, TreeGumpNode>> list)
		{
			list.Clear();

			list["Transactions"] = CompileTransactions;
			list["Transactions|Pending"] = CompileTransactions;
			list["Transactions|Claim"] = CompileTransactions;

			var trans = _Admin ? Profile.Transactions.Values : Profile.Visible;

			foreach (var t in trans)
			{
				if (t.IsPending)
				{
					list["Transactions|Pending|" + t.ID] = CompileTransaction;
				}
				else if (t.IsProcessed)
				{
					list["Transactions|Claim|" + t.ID] = CompileTransaction;
				}

				TreeGumpNode n = t.FullPath;

				list["History|" + n.FullName] = CompileTransaction;

				foreach (var p in n.GetParents())
				{
					list["History|" + p] = CompileTransactions;
				}
			}

			base.CompileNodes(list);
		}

		protected override void CompileNodeLayout(
			SuperGumpLayout layout,
			int x,
			int y,
			int w,
			int h,
			int index,
			TreeGumpNode node)
		{
			base.CompileNodeLayout(layout, x, y, w, h, index, node);

			if (Nodes == null || !Nodes.ContainsKey(node))
			{
				CompileEmptyNodeLayout(layout, x, y, w, h, index, node);
			}
		}

		protected override void CompileEmptyNodeLayout(
			SuperGumpLayout layout,
			int x,
			int y,
			int w,
			int h,
			int index,
			TreeGumpNode node)
		{
			base.CompileEmptyNodeLayout(layout, x, y, w, h, index, node);

			layout.Add("node/page/" + index, () => CompileOverview(new Rectangle(x, y, w, h), index, node));
		}

		protected virtual void CompileOverview(Rectangle bounds, int index, TreeGumpNode node)
		{
			var info = new StringBuilder();

			info.AppendLine(
				"Welcome to the Donation Exchange, {0}!",
				User.RawName.WrapUOHtmlColor(User.GetNotorietyColor(), HtmlColor));
			info.AppendLine();
			info.AppendLine("Select a category on the left to browse your transactions.");
			info.AppendLine();
			info.AppendLine("MY EXCHANGE".WrapUOHtmlBold().WrapUOHtmlColor(Color.Gold, false));
			info.AppendLine();

			GetProfileOverview(info);

			AddHtml(
				bounds.X + 5,
				bounds.Y,
				bounds.Width - 5,
				bounds.Height,
				info.ToString().WrapUOHtmlColor(HtmlColor),
				false,
				true);
		}

		protected virtual void CompileTransactions(Rectangle b, int index, TreeGumpNode node)
		{
			_Transactions.Clear();

			int idx;

			var trans = _Admin ? Profile.Transactions.Values : Profile.Visible;

			switch (node.Name)
			{
				case "Transactions":
					idx = 0;
					break;
				case "Claim":
				{
					trans = trans.Where(t => t.IsProcessed);

					idx = 1;
				}
				break;
				case "Pending":
				{
					trans = trans.Where(t => t.IsPending);

					idx = 2;
				}
				break;
				default:
				{
					switch (node.Depth)
					{
						default: // History
							idx = 3;
							break;
						case 1: // Year
						{
							var y = Utility.ToInt32(node.Name);

							trans = trans.Where(t => t.Time.Value.Year == y);

							idx = 4;
						}
						break;
						case 2: // Month
						{
							var y = Utility.ToInt32(node.Parent.Name);
							var m = (Months)Enum.Parse(typeof(Months), node.Name);

							trans = trans.Where(t => t.Time.Value.Year == y);
							trans = trans.Where(t => t.Time.Value.GetMonth() == m);

							idx = 5;
						}
						break;
					}
				}
				break;
			}

			_Transactions.AddRange(trans);
			_Transactions.Sort();

			_Indicies[idx, 1] = _Transactions.Count;
			_Indicies[idx, 0] = Math.Max(0, Math.Min(_Indicies[idx, 1] - 1, _Indicies[idx, 0]));

			_Transactions.TrimStart(_Indicies[idx, 0]);
			_Transactions.TrimEndTo((b.Height / 24) - 1);

			// ID | Date | Recipient | Value | Credit | State
			var cols = new[] { -1, -1, -1, 80, 80, 80 };

			AddTable(b.X, b.Y, b.Width - 25, b.Height, true, cols, _Transactions, 24, Color.Empty, 0, RenderTransaction);

			_Transactions.Clear();

			AddBackground(b.X + (b.Width - 25), b.Y, 28, b.Height, SupportsUltimaStore ? 40000 : 9260);

			AddScrollbarV(
				b.X + (b.Width - 24),
				b.Y,
				b.Height,
				_Indicies[idx, 1],
				_Indicies[idx, 0],
				p =>
				{
					--_Indicies[idx, 0];
					Refresh(true);
				},
				n =>
				{
					++_Indicies[idx, 0];
					Refresh(true);
				});
		}

		protected virtual void RenderTransaction(int x, int y, int w, int h, DonationTransaction t, int r, int c)
		{
			var bgCol = r % 2 != 0 ? Color.DarkSlateGray : Color.Black;
			var fgCol = r % 2 != 0 ? Color.White : Color.WhiteSmoke;

			ApplyPadding(ref x, ref y, ref w, ref h, 2);

			if (r < 0) // headers
			{
				var label = String.Empty;

				switch (c)
				{
					case -1:
						AddHtml(x, y, w, h, label, fgCol, bgCol);
						break;
					case 0:
						label = "ID";
						goto case -1;
					case 1:
						label = "Date";
						goto case -1;
					case 2:
						label = "Recipient";
						goto case -1;
					case 3:
						label = AutoDonate.CMOptions.MoneySymbol + AutoDonate.CMOptions.MoneyAbbr;
						goto case -1;
					case 4:
						label = AutoDonate.CMOptions.CurrencyName;
						goto case -1;
					case 5:
						label = "Status";
						goto case -1;
				}
			}
			else if (t != null)
			{
				switch (c)
				{
					case 0: // ID
						AddHtml(x, y, w, h, t.ID, fgCol, bgCol);
						break;
					case 1: // Date
					{
						AddHtml(x, y, w, h, t.Time.Value.ToSimpleString("m/d/y"), fgCol, bgCol);
						AddTooltip(t.Time.Value.ToSimpleString());
					}
					break;
					case 2: // Recipient
					{
						AddHtml(x, y, w, h, t.DeliveredTo ?? String.Empty, fgCol, bgCol);

						if (!String.IsNullOrWhiteSpace(t.DeliveredTo))
						{
							AddTooltip(t.DeliveryTime.Value.ToSimpleString());
						}
					}
					break;
					case 3: // Value
						AddHtml(x, y, w, h, AutoDonate.CMOptions.MoneySymbol + t.Total.ToString("#,0.00"), fgCol, bgCol);
						break;
					case 4: // Credit
					{
						if (t.Bonus > 0)
						{
							AddHtml(x, y, w, h, String.Format("{0:#,0} +{1:#,0}", t.Credit, t.Bonus), fgCol, bgCol);
						}
						else
						{
							AddHtml(x, y, w, h, t.Credit.ToString("#,0"), fgCol, bgCol);
						}
					}
					break;
					case 5: // Status
					{
						string node;

						if (t.IsPending)
						{
							node = "Transactions|Pending|" + t.ID;
						}
						else if (t.IsProcessed)
						{
							node = "Transactions|Claim|" + t.ID;
						}
						else
						{
							node = "History|" + t.FullPath;
						}

						var label = String.Empty;
						var color = fgCol;

						switch (t.State)
						{
							case TransactionState.Voided:
							{
								label = UniGlyph.CircleX.ToString();
								color = Color.IndianRed;
							}
							break;
							case TransactionState.Pending:
							{
								label = UniGlyph.Coffee.ToString();
								color = Color.Yellow;
							}
							break;
							case TransactionState.Processed:
							{
								label = UniGlyph.StarEmpty.ToString();
								color = Color.SkyBlue;
							}
							break;
							case TransactionState.Claimed:
							{
								label = UniGlyph.StarFill.ToString();
								color = Color.LawnGreen;
							}
							break;
						}

						label = label.WrapUOHtmlColor(color, fgCol);
						label += " " + t.State.ToString(true);

						AddHtmlButton(x, y, w, h, b => SelectNode(node), label, fgCol, bgCol);
					}
					break;
				}
			}
		}

		protected virtual void CompileTransaction(Rectangle b, int index, TreeGumpNode node)
		{
			var trans = Profile[node.Name];

			if (trans == null)
			{
				CompileOverview(b, index, node);
				return;
			}

			var cpHeight = _Admin ? 60 : 30;

			var html = new StringBuilder();

			GetTransactionOverview(trans, html, true, true, true);

			AddHtml(
				b.X + 5,
				b.Y,
				b.Width - 5,
				b.Height - cpHeight,
				html.ToString().WrapUOHtmlColor(Color.White, false),
				false,
				true);

			var bw = b.Width;
			var bh = cpHeight;

			if (_Admin)
			{
				bh /= 2;
			}

			switch (trans.State)
			{
				case TransactionState.Voided:
				{
					AddHtmlButton(
						b.X,
						b.Y + (b.Height - bh),
						bw,
						bh,
						o => OnVoidedTransaction(trans),
						"[VOIDED]",
						Color.OrangeRed,
						Color.Black,
						Color.OrangeRed,
						2);
				}
				break;
				case TransactionState.Pending:
				{
					AddHtmlButton(
						b.X,
						b.Y + (b.Height - bh),
						bw,
						bh,
						o => OnPendingTransaction(trans),
						"[PENDING]",
						Color.Yellow,
						Color.Black,
						Color.Yellow,
						2);
				}
				break;
				case TransactionState.Processed:
				{
					AddHtmlButton(
						b.X,
						b.Y + (b.Height - bh),
						bw,
						bh,
						o => OnClaimTransaction(trans),
						"[CLAIM]",
						Color.SkyBlue,
						Color.Black,
						Color.SkyBlue,
						2);
				}
				break;
				case TransactionState.Claimed:
				{
					AddHtmlButton(
						b.X,
						b.Y + (b.Height - bh),
						bw,
						bh,
						o => OnClaimedTransaction(trans),
						"[CLAIMED]",
						Color.LawnGreen,
						Color.Black,
						Color.LawnGreen,
						2);
				}
				break;
			}

			if (_Admin)
			{
				bw /= 2;

				AddHtmlButton(
					b.X,
					b.Y + (b.Height - (bh * 2)),
					bw,
					bh,
					o => OnTransactionEdit(trans),
					"[EDIT]",
					Color.Gold,
					Color.Black,
					Color.Gold,
					2);

				AddHtmlButton(
					b.X + bw,
					b.Y + (b.Height - (bh * 2)),
					bw,
					bh,
					o => OnTransactionTransfer(trans),
					"[TRANSFER]",
					Color.Gold,
					Color.Black,
					Color.Gold,
					2);
			}
		}

		protected virtual void GetTransactionOverview(
			DonationTransaction trans,
			StringBuilder info,
			bool details,
			bool exchange,
			bool status)
		{
			if (details)
			{
				info.AppendLine();
				info.AppendLine("Details");
				info.AppendLine();
				info.AppendLine("ID: {0}", trans.ID);
				info.AppendLine("Date: {0}", trans.Time.Value);

				if (_Admin)
				{
					info.AppendLine();
					info.AppendLine("Notes: {0}", trans.Notes);
					info.AppendLine();
					info.AppendLine("Extra: {0}", trans.Extra);
				}
			}

			if (exchange)
			{
				info.AppendLine();
				info.AppendLine("Exchange");
				info.AppendLine();
				info.AppendLine(
					"Value: {0}{1:#,0.00} {2}",
					AutoDonate.CMOptions.MoneySymbol,
					trans.Total,
					AutoDonate.CMOptions.MoneyAbbr);

				var total = trans.GetCredit(Profile, out var credit, out var bonus);

				info.AppendLine("Credit: {0:#,0} {1}", credit, AutoDonate.CMOptions.CurrencyName);
				info.AppendLine("Bonus: {0:#,0} {1}", bonus, AutoDonate.CMOptions.CurrencyName);
				info.AppendLine("Total: {0:#,0} {1}", total, AutoDonate.CMOptions.CurrencyName);
			}

			if (status)
			{
				info.AppendLine();
				info.AppendLine("Status");
				info.AppendLine();
				info.AppendLine("State: {0}", trans.State);

				switch (trans.State)
				{
					case TransactionState.Voided:
						info.AppendLine("Transaction has been voided.");
						break;
					case TransactionState.Pending:
						info.AppendLine("Transaction is pending verification.");
						break;
					case TransactionState.Processed:
						info.AppendLine("Transaction is complete and can be claimed.");
						break;
					case TransactionState.Claimed:
					{
						info.AppendLine("Transaction has been delivered.");
						info.AppendLine();
						info.AppendLine("Date: {0}", trans.DeliveryTime.Value);

						if (trans.DeliveredTo != null)
						{
							info.AppendLine("Recipient: {0}", trans.DeliveredTo);
						}
					}
					break;
				}
			}
		}

		public void GetProfileOverview(StringBuilder info)
		{
			var ms = AutoDonate.CMOptions.MoneySymbol;
			var ma = AutoDonate.CMOptions.MoneyAbbr;
			var cn = AutoDonate.CMOptions.CurrencyName;

			var val = _DonationTier.ToString("#,0").WrapUOHtmlColor(Color.LawnGreen, HtmlColor);
			info.AppendLine("Donation Tier: {0}", val);

			val = _DonationValue.ToString("#,0.00").WrapUOHtmlColor(Color.LawnGreen, HtmlColor);
			info.AppendLine("Donations Total: {0}{1} {2}", ms, val, ma);

			val = _DonationCredit.ToString("#,0").WrapUOHtmlColor(Color.LawnGreen, HtmlColor);
			info.AppendLine("Donations Claimed: {0} {1}", val, cn);
		}

		protected virtual void OnTransactionEdit(DonationTransaction trans)
		{
			Refresh();

			if (_Admin)
			{
				User.SendGump(new PropertiesGump(User, trans));
			}
		}

		protected virtual void OnTransactionTransfer(DonationTransaction trans)
		{
			new InputDialogGump(User, Refresh())
			{
				Title = "Transfer Transaction",
				Html = "Enter the account name of the recipient for the transfer.",
				InputText = trans.Account != null ? trans.Account.Username : String.Empty,
				Callback = (b, a) =>
				{
					if (User.AccessLevel >= AutoDonate.Access)
					{
						var acc = Accounts.GetAccount(a);

						if (acc == null)
						{
							User.SendMessage(34, "The account '{0}' does not exist.", a);
						}
						else if (trans.Account == acc)
						{
							User.SendMessage(34, "The transaction is already bound to '{0}'", a);
						}
						else
						{
							trans.SetAccount(acc);
							User.SendMessage(85, "The transaction has been transferred to '{0}'", a);
						}
					}

					Refresh(true);
				}
			}.Send();
		}

		protected virtual void OnVoidedTransaction(DonationTransaction trans)
		{
			var html = new StringBuilder();

			GetTransactionOverview(trans, html, false, false, true);

			new NoticeDialogGump(User, Refresh())
			{
				Title = "Transaction Voided",
				Html = html.ToString()
			}.Send();
		}

		protected virtual void OnPendingTransaction(DonationTransaction trans)
		{
			var html = new StringBuilder();

			GetTransactionOverview(trans, html, false, false, true);

			new NoticeDialogGump(User, Refresh())
			{
				Title = "Transaction Pending",
				Html = html.ToString()
			}.Send();
		}

		protected virtual void OnClaimTransaction(DonationTransaction trans)
		{
			var html = new StringBuilder();

			GetTransactionOverview(trans, html, false, true, false);

			html.AppendLine();
			html.AppendLine("Click OK to claim this transaction!");

			new ConfirmDialogGump(User, Refresh())
			{
				Title = "Reward Claim",
				Html = html.ToString(),
				AcceptHandler = b =>
				{
					if (trans.Claim(User))
					{
						SelectedNode = "Transactions|" + trans.FullPath;

						User.SendMessage(85, "You claimed the transaction!");
					}

					Refresh(true);
				}
			}.Send();
		}

		protected virtual void OnClaimedTransaction(DonationTransaction trans)
		{
			var info = new StringBuilder();

			GetTransactionOverview(trans, info, false, false, true);

			new NoticeDialogGump(User, Refresh())
			{
				Title = "Reward Delivered",
				Html = info.ToString()
			}.Send();
		}

		protected override void OnDisposed()
		{
			_Indicies = null;

			ObjectPool.Free(ref _Transactions);

			base.OnDisposed();
		}
	}
}
