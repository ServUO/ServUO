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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;

using VitaNex.IO;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
using VitaNex.Text;
#endregion

namespace VitaNex.Modules.AutoDonate
{
	public class DonationAdminUI : SuperGump
	{
		public class OptionEntry
		{
			public string Label { get; set; }
			public string Info { get; set; }

			public Func<object> GetValue { get; set; }
			public Action<object> SetValue { get; set; }

			public object Value { get => GetValue(); set => SetValue(value); }

			public Type ValueType
			{
				get
				{
					var value = Value;

					return value != null ? value.GetType() : typeof(object);
				}
			}

			public OptionEntry(string label, string info, Func<object> getValue, Action<object> setValue)
			{
				Label = label;
				Info = info;

				GetValue = getValue;
				SetValue = setValue;
			}
		}

		private static readonly OptionEntry[] _CoreSettings =
		{
			new OptionEntry(
				"Enabled",
				"Enable or Disable the Donation Service.\n\n" +
				"All features of the service will be enabled or disabled by changing this value.",
				() => AutoDonate.CMOptions.ModuleEnabled,
				o => AutoDonate.CMOptions.ModuleEnabled = (bool)o),
			new OptionEntry(
				"Debug Mode",
				"Enable or Disable debug mode (extended output and error handling).",
				() => AutoDonate.CMOptions.ModuleDebug,
				o => AutoDonate.CMOptions.ModuleDebug = (bool)o),
			new OptionEntry(
				"Quiet Mode",
				"Enable or Disable quiet mode (limited output, simple logs).",
				() => AutoDonate.CMOptions.ModuleQuiet,
				o => AutoDonate.CMOptions.ModuleQuiet = (bool)o),
			new OptionEntry(
				"Business *",
				"Defines the business to use when verifying the recipient of a transaction.\n\n" +
				"Supported Values:\n'email@domain.com' (PayPal Email)\n'ABCDEF0123' (Merchant ID)",
				() => AutoDonate.CMOptions.Business,
				o => AutoDonate.CMOptions.Business = (string)o),
			new OptionEntry(
				"Money Symbol *",
				"The symbol that represents your currency.\n\nExample; USD is represented by the Dollar symbol '$'.",
				() => AutoDonate.CMOptions.MoneySymbol,
				o => AutoDonate.CMOptions.MoneySymbol = (char)o),
			new OptionEntry(
				"Money Abbr *",
				"The abbreviation that represents your currency.\n\nExample; 'USD' represents the US Dollar.",
				() => AutoDonate.CMOptions.MoneyAbbr,
				o => AutoDonate.CMOptions.MoneyAbbr = (string)o),
			new OptionEntry(
				"Currency Name *",
				"The display name of the currency that is given to donators when they claim a transaction.",
				() => AutoDonate.CMOptions.CurrencyName,
				o => AutoDonate.CMOptions.CurrencyName = (string)o),
			new OptionEntry(
				"Currency Type *",
				"The Type name of the currency that is given to donators when they claim a transaction.\n" +
				"This Type must be a constructable Item type with zero parameters.",
				() => AutoDonate.CMOptions.CurrencyType.TypeName,
				o => AutoDonate.CMOptions.CurrencyType = (string)o),
			new OptionEntry(
				"Currency Price *",
				"The unit price for the currency that is given to donators when they claim a transaction.\n" +
				"Example; 1 (one) unit of currency may be worth $0.50 (fifty cents) if this value is set to '0.50'.",
				() => AutoDonate.CMOptions.CurrencyPrice,
				o => AutoDonate.CMOptions.CurrencyPrice = (double)o),
			new OptionEntry(
				"Fallback Account",
				"Defines the fallback account to use when an account can not be found for a transaction.\n\n" +
				"If the given account is not set, the first valid Owner account will be used.",
				() => AutoDonate.CMOptions.FallbackUsername,
				o => AutoDonate.CMOptions.FallbackUsername = (string)o),
			new OptionEntry(
				"Tier Factor",
				"The Tier Factor is used as a factor to determine a donator's 'level' when computing credit bonuses, etc.\n\n" +
				"For use with custom implementations.",
				() => AutoDonate.CMOptions.TierFactor,
				o => AutoDonate.CMOptions.TierFactor = (double)o),
			new OptionEntry(
				"Credit Bonus",
				"The Credit Bonus is used as a factor to increase the amount of credit received at the point of claiming the transaction.\n\n" +
				"This value represents a percentage with the ratio of 0.01 : 1% (1.0 : 100%)",
				() => AutoDonate.CMOptions.CreditBonus,
				o => AutoDonate.CMOptions.CreditBonus = (double)o),
			new OptionEntry(
				"Show History",
				"When enabled, allows donators to browse their transaction history.\n\n" +
				"When disabled, all transactions will be hidden with the exception of transactions waiting to be claimed.",
				() => AutoDonate.CMOptions.ShowHistory,
				o => AutoDonate.CMOptions.ShowHistory = (bool)o)
		};

		private static readonly OptionEntry[] _FormSettings =
		{
			new OptionEntry(
				"Enabled",
				"Enables or Disables the web form service.\n\n" +
				"When enabled, the Donation service will serve http requests for 'donate/form' with html data that provides a complete web-form for donators to use.\nThe web form is loaded in to the page using JavaScript located at <a href=\"http://www.vita-nex.com/js/mod/donateForm.js\">donateForm.js</a>\n\n" +
				"When disabled, the Donation service will serve http requests for '/donate/form' with empty data.",
				() => AutoDonate.CMOptions.WebForm.Enabled,
				o => AutoDonate.CMOptions.WebForm.Enabled = (bool)o),
			new OptionEntry(
				"Test",
				"Enables or Disables the web form service transaction testing.\n\n" +
				"When enabled, PayPal sandbox is used to verify transactions.\n" +
				"All test transactions will be Voided so they can not be claimed.",
				() => AutoDonate.CMOptions.WebForm.Test,
				o => AutoDonate.CMOptions.WebForm.Test = (bool)o),
			new OptionEntry(
				"Command *",
				"Defines the command to use when submitting the web-form.\n\nPayPal Button Variable: 'cmd'\n\n" +
				"Supported Values:\n'_xclick' (Buy Now Button)\n'_donations' (Donate Button)\n\n" +
				"<a href=\"https://developer.paypal.com/docs/classic/paypal-payments-standard/integration-guide/Appx_websitestandard_htmlvariables\">Documentation</a>",
				() => AutoDonate.CMOptions.WebForm.Command,
				o => AutoDonate.CMOptions.WebForm.Command = (string)o),
			new OptionEntry(
				"Business *",
				"Defines the business to use when submitting the web-form.\n\nPayPal Button Variable: 'business'\n\n" +
				"Supported Values:\n'email@domain.com' (PayPal Email)\n'ABCDEF0123' (Merchant ID)\n\n" +
				"<a href=\"https://developer.paypal.com/docs/classic/paypal-payments-standard/integration-guide/Appx_websitestandard_htmlvariables\">Documentation</a>",
				() => AutoDonate.CMOptions.WebForm.Business,
				o => AutoDonate.CMOptions.WebForm.Business = (string)o),
			new OptionEntry(
				"Notify URL *",
				"Defines the IPN handler Url to use when submitting the web-form.\n\nPayPal Button Variable: 'notify_url'\n\n" +
				"Format: 'http://shard.domain.com/donate/ipn' or 'http://255.255.255.255/donate/ipn'\n" +
				"This Url should resolve to your shard's IP address and should end with '/donate/ipn' " +
				"in order for the Donation service to handle the transactions.\n" +
				"If you use a custom IPN handler, you may specify that Url, but the Donation Service will not handle your transactions.\n\n" +
				"<a href=\"https://developer.paypal.com/docs/classic/paypal-payments-standard/integration-guide/Appx_websitestandard_htmlvariables\">Documentation</a>",
				() => AutoDonate.CMOptions.WebForm.NotifyUrl,
				o => AutoDonate.CMOptions.WebForm.NotifyUrl = (string)o),
			new OptionEntry(
				"Return URL",
				"Defines the return Url to use when submitting the web-form.\n\nPayPal Button Variable: 'return'\n\n" +
				"Format: 'http://shard.domain.com/donate/form' or 'http://127.0.0.1/donate/form'\n" +
				"This Url is where your donators will visit after they successfully complete a transaction.\n\n" +
				"<a href=\"https://developer.paypal.com/docs/classic/paypal-payments-standard/integration-guide/Appx_websitestandard_htmlvariables\">Documentation</a>",
				() => AutoDonate.CMOptions.WebForm.ReturnUrl,
				o => AutoDonate.CMOptions.WebForm.ReturnUrl = (string)o),
			new OptionEntry(
				"Verify URL",
				"Defines the Url to use to verify whether an account exists when submitting the web-form.\n\n" +
				"Format: 'http://shard.domain.com/donate/acc' or 'http://127.0.0.1/donate/acc'\n" +
				"This Url should target a resource that outputs one of two responses; 'VALID' or 'INVALID' " +
				"and should parse the value of the query key 'username' to verify whether the account exists.",
				() => AutoDonate.CMOptions.WebForm.VerifyUrl,
				o => AutoDonate.CMOptions.WebForm.VerifyUrl = (string)o),
			new OptionEntry(
				"Default Amount",
				"The default amount to use when rendering the web-form's slider component.\n" +
				"The default amount a donator can donate in a single transaction.\nFormat: '12.34'",
				() => AutoDonate.CMOptions.WebForm.AmountDef,
				o => AutoDonate.CMOptions.WebForm.AmountDef = (double)o),
			new OptionEntry(
				"Minimum Amount",
				"The minimum amount to use when rendering the web-form's slider component.\n" +
				"The minimum amount a donator can donate in a single transaction.\n\nFormat: '12.34'",
				() => AutoDonate.CMOptions.WebForm.AmountMin,
				o => AutoDonate.CMOptions.WebForm.AmountMin = (double)o),
			new OptionEntry(
				"Maximum Amount",
				"The maximum amount to use when rendering the web-form's slider component.\n" +
				"The maximum amount a donator can donate in a single transaction.\nFormat: '12.34'",
				() => AutoDonate.CMOptions.WebForm.AmountMax,
				o => AutoDonate.CMOptions.WebForm.AmountMax = (double)o),
			new OptionEntry(
				"Increment Amount",
				"The incremental amount to use when rendering the web-form's slider component.\n" +
				"The incremental amount used when moving the slider.\nFormat: '12.34'",
				() => AutoDonate.CMOptions.WebForm.AmountInc,
				o => AutoDonate.CMOptions.WebForm.AmountInc = (double)o),
			new OptionEntry(
				"Button Name",
				"The name to display when rendering the web-form's button component.",
				() => AutoDonate.CMOptions.WebForm.ButtonName,
				o => AutoDonate.CMOptions.WebForm.ButtonName = (string)o),
			new OptionEntry(
				"Banner Image",
				"Optional.\nThe image Url to use when rendering the web-form's banner component.",
				() => AutoDonate.CMOptions.WebForm.BannerImg,
				o => AutoDonate.CMOptions.WebForm.BannerImg = (string)o),
			new OptionEntry(
				"Banner Url",
				"Optional.\nThe Url to use when rendering the web-form's banner component.\n" +
				"This Url will be visited by donators who click on the banner image.",
				() => AutoDonate.CMOptions.WebForm.BannerUrl,
				o => AutoDonate.CMOptions.WebForm.BannerUrl = (string)o),
			new OptionEntry(
				"Shard Name",
				"Optional.\nThe shard name to use when rendering the web-form's header component.",
				() => AutoDonate.CMOptions.WebForm.Shard,
				o => AutoDonate.CMOptions.WebForm.Shard = (string)o)
		};

		public int CoreSettingsPage { get; set; }
		public int CoreSettingsPageCount { get; private set; }

		public int FormSettingsPage { get; set; }
		public int FormSettingsPageCount { get; private set; }

		public int ProfilesPage { get; set; }
		public int ProfilesPageCount { get; private set; }

		public int TransactionsPage { get; set; }
		public int TransactionsPageCount { get; private set; }

		public int InfoPanelTab { get; set; }
		public int InfoPanelTabsCount { get; private set; }

		public string ProfilesSearch { get; set; }
		public string TransactionsSearch { get; set; }

		public bool CoreSettingsMinimized { get; set; }
		public bool FormSettingsMinimized { get; set; }
		public bool InfoPanelMinimized { get; set; }

		public bool Minimized
		{
			get => CoreSettingsMinimized && FormSettingsMinimized && InfoPanelMinimized;
			set => CoreSettingsMinimized = FormSettingsMinimized = InfoPanelMinimized = value;
		}

		public int WidthMin { get; set; }
		public int WidthMax { get; set; }

		public int HeightMin { get; set; }
		public int HeightMax { get; set; }

		public int Width { get; set; }
		public int Height { get; set; }

		public int PanelWidth { get; private set; }
		public int PanelHeight { get; private set; }

		public int PanelRows { get; private set; }

		public DonationAdminUI(Mobile user, Gump parent = null)
			: base(user, parent)
		{
			WidthMin = 1024;
			WidthMax = 1280;

			HeightMin = 600;
			HeightMax = 720;

			Width = WidthMax;
			Height = HeightMax;

			CoreSettingsMinimized = FormSettingsMinimized = true;
		}

		protected void SetValue(OptionEntry entry, object value)
		{
			entry.Value = Convert.ChangeType(value, entry.ValueType);
		}

		protected string GetValueString(OptionEntry e)
		{
			var val = e.Value;

			if (val == null)
			{
				return "(~null~)";
			}

			if (val is bool)
			{
				return (bool)val ? "True" : "False";
			}

			if (val is sbyte || val is short || val is int || val is long)
			{
				return ((long)val).ToString("#,0");
			}

			if (val is byte || val is ushort || val is uint || val is ulong)
			{
				return ((ulong)val).ToString("#,0");
			}

			if (val is float || val is decimal || val is double)
			{
				return ((double)val).ToString("#,0.00");
			}

			return val.ToString();
		}

		protected override void Compile()
		{
			Width = Math.Max(WidthMin, Math.Min(WidthMax, Width));
			Height = Math.Max(HeightMin, Math.Min(HeightMax, Height));

			PanelWidth = Width / 2;
			PanelHeight = Height / 2;

			PanelRows = (int)Math.Floor((PanelHeight - 55) / 30.0);

			CoreSettingsPageCount = (int)Math.Ceiling(_CoreSettings.Length / (double)PanelRows);
			CoreSettingsPage = Math.Min(CoreSettingsPageCount, CoreSettingsPage);

			FormSettingsPageCount = (int)Math.Ceiling(_FormSettings.Length / (double)PanelRows);
			FormSettingsPage = Math.Min(FormSettingsPageCount, FormSettingsPage);

			InfoPanelTabsCount = 2;
			InfoPanelTab = Math.Min(InfoPanelTabsCount, InfoPanelTab);

			TransactionsPageCount = (int)Math.Ceiling(AutoDonate.Transactions.Count / (double)(PanelRows - 2));
			TransactionsPage = Math.Min(TransactionsPageCount, TransactionsPage);

			ProfilesPageCount = (int)Math.Ceiling(AutoDonate.Profiles.Count / (double)(PanelRows - 1));
			ProfilesPage = Math.Min(ProfilesPageCount, ProfilesPage);

			base.Compile();
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			CompileStatisticsLayout(layout, 0, 0, PanelWidth, PanelHeight); // Top-Left

			if (User.AccessLevel >= AutoDonate.Access)
			{
				CompileInfoPanelLayout(layout); // Bottom-Left

				CompileCoreSettingsLayout(layout, PanelWidth, 0, PanelWidth, PanelHeight); // Top-Right
				CompileFormSettingsLayout(layout, PanelWidth, PanelHeight, PanelWidth, PanelHeight); // Bottom-Right
			}
		}

		protected virtual void CompileInfoPanelLayout(SuperGumpLayout layout)
		{
			switch (InfoPanelTab)
			{
				case 0:
					CompileTransactionsLayout(layout, 0, PanelHeight, PanelWidth, PanelHeight);
					break;
				case 1:
					CompileProfilesLayout(layout, 0, PanelHeight, PanelWidth, PanelHeight);
					break;
			}
		}

		public virtual void Minimize(GumpButton b)
		{
			Minimize();
		}

		public virtual void Minimize()
		{
			Minimized = true;

			Refresh(true);
		}

		public virtual void Maximize(GumpButton b)
		{
			Maximize();
		}

		public virtual void Maximize()
		{
			Minimized = false;

			Refresh(true);
		}

		public virtual void Resize(GumpButton b)
		{
			Resize();
		}

		public virtual void Resize()
		{
			if ((Width - WidthMin) / (double)(WidthMax - WidthMin) >= 1.0)
			{
				Width = WidthMin;
			}
			else
			{
				Width += (int)(WidthMax * 0.10);
			}

			if ((Height - HeightMin) / (double)(HeightMax - HeightMin) >= 1.0)
			{
				Height = HeightMin;
			}
			else
			{
				Height += (int)(HeightMax * 0.10);
			}

			Refresh(true);
		}

		protected virtual void CompileStatisticsLayout(SuperGumpLayout layout, int x, int y, int w, int h)
		{
			layout.Add(
				"panels/statistics",
				() =>
				{
					AddBackground(x, y, w, h, 9270);

					AddButton(x + 15, y + 15, 2708, 2709, Close);

					if (Minimized)
					{
						AddButton(x + 40, y + 15, 2711, 2710, Maximize);
					}
					else
					{
						AddButton(x + 40, y + 15, 2710, 2711, Minimize);
					}

					if (CanResize)
					{
						AddButton(x + 65, y + 15, 2714, 2715, Resize);
					}
					else
					{
						AddImage(x + 65, y + 15, 2715, 900);
					}

					var text = "STATISTICS".WrapUOHtmlBold().WrapUOHtmlCenter().WrapUOHtmlColor(Color.PaleGoldenrod, false);

					AddHtml(x + 15, y + 15, w - 30, 40, text, false, false);

					var stats = new StringBuilder();

					if (AutoDonate.Transactions.Count > 0)
					{
						double start = Double.MaxValue, end = Double.MinValue;

						foreach (var t in AutoDonate.Transactions.Values)
						{
							start = Math.Min(start, t.Time.Stamp);
							end = Math.Max(end, t.Time.Stamp);
						}

						if (start <= end)
						{
							var startDate = TimeStamp.FromSeconds(start).Value.ToSimpleString("t@h:m:s@ D d M y");
							var endDate = TimeStamp.FromSeconds(end).Value.ToSimpleString("t@h:m:s@ D d M y");

							stats.AppendLine("Period Start: {0}", startDate.WrapUOHtmlColor(Color.LawnGreen, Color.Gold));
							stats.AppendLine("Period End: {0}", endDate.WrapUOHtmlColor(Color.LawnGreen, Color.Gold));
							stats.AppendLine();
						}
					}

					stats.AppendLine(
						"Total Income: " + "{0}{1:#,0.00} {2}".WrapUOHtmlColor(Color.LawnGreen, Color.Gold),
						AutoDonate.CMOptions.MoneySymbol,
						AutoDonate.CMOptions.Info.TotalIncome,
						AutoDonate.CMOptions.MoneyAbbr);
					stats.AppendLine(
						"Total Credit: " + "{0:#,0} {1}".WrapUOHtmlColor(Color.LawnGreen, Color.Gold),
						AutoDonate.CMOptions.Info.TotalCredit,
						AutoDonate.CMOptions.CurrencyName);

					stats.AppendLine();
					stats.AppendLine(
						"Profiles: " + "{0:#,0}".WrapUOHtmlColor(Color.LawnGreen, Color.Gold),
						AutoDonate.CMOptions.Info.ProfileCount);
					stats.AppendLine(
						"Min. Donator Tier: " + "{0:#,0}".WrapUOHtmlColor(Color.LawnGreen, Color.Gold),
						AutoDonate.CMOptions.Info.TierMin);
					stats.AppendLine(
						"Max. Donator Tier: " + "{0:#,0}".WrapUOHtmlColor(Color.LawnGreen, Color.Gold),
						AutoDonate.CMOptions.Info.TierMax);
					stats.AppendLine(
						"Avg. Donator Tier: " + "{0:#,0.##}".WrapUOHtmlColor(Color.LawnGreen, Color.Gold),
						AutoDonate.CMOptions.Info.TierAverage);

					stats.AppendLine();
					stats.AppendLine(
						"Transactions: " + "{0:#,0}".WrapUOHtmlColor(Color.LawnGreen, Color.Gold),
						AutoDonate.CMOptions.Info.TransCount);
					stats.AppendLine(
						"Min. Transaction Value: " + "{0}{1:#,0.00} {2}".WrapUOHtmlColor(Color.LawnGreen, Color.Gold),
						AutoDonate.CMOptions.MoneySymbol,
						AutoDonate.CMOptions.Info.TransMin,
						AutoDonate.CMOptions.MoneyAbbr);
					stats.AppendLine(
						"Max. Transaction Value: " + "{0}{1:#,0.00} {2}".WrapUOHtmlColor(Color.LawnGreen, Color.Gold),
						AutoDonate.CMOptions.MoneySymbol,
						AutoDonate.CMOptions.Info.TransMax,
						AutoDonate.CMOptions.MoneyAbbr);
					stats.AppendLine(
						"Avg. Transaction Value: " + "{0}{1:#,0.00} {2}".WrapUOHtmlColor(Color.LawnGreen, Color.Gold),
						AutoDonate.CMOptions.MoneySymbol,
						AutoDonate.CMOptions.Info.TransAverage,
						AutoDonate.CMOptions.MoneyAbbr);
					stats.AppendLine(
						"Min. Transaction Credit: " + "{0:#,0} {1}".WrapUOHtmlColor(Color.LawnGreen, Color.Gold),
						AutoDonate.CMOptions.Info.CreditMin,
						AutoDonate.CMOptions.CurrencyName);
					stats.AppendLine(
						"Max. Transaction Credit: " + "{0:#,0} {1}".WrapUOHtmlColor(Color.LawnGreen, Color.Gold),
						AutoDonate.CMOptions.Info.CreditMax,
						AutoDonate.CMOptions.CurrencyName);
					stats.AppendLine(
						"Avg. Transaction Credit: " + "{0:#,0.##} {1}".WrapUOHtmlColor(Color.LawnGreen, Color.Gold),
						AutoDonate.CMOptions.Info.CreditAverage,
						AutoDonate.CMOptions.CurrencyName);

					text = stats.ToString().WrapUOHtmlColor(Color.Gold, false);

					AddHtml(x + 15, y + 40, w - 30, h - 55, text, false, true);
				});
		}

		protected virtual void CompileCoreSettingsLayout(SuperGumpLayout layout, int x, int y, int w, int h)
		{
			layout.Add(
				"panels/coresettings",
				() =>
				{
					if (CoreSettingsMinimized)
					{
						AddButton(
							x - 25,
							y,
							4502,
							4502,
							b =>
							{
								CoreSettingsMinimized = false;
								Refresh(true);
							});
						return;
					}

					AddBackground(x, y, w, h, 9270);

					AddButton(
						x - 25,
						y,
						4506,
						4506,
						b =>
						{
							CoreSettingsMinimized = true;
							Refresh(true);
						});

					var text = "CORE SETTINGS".WrapUOHtmlBold().WrapUOHtmlCenter().WrapUOHtmlColor(Color.PaleGoldenrod, false);

					AddHtml(x + 15, y + 15, w - 30, 40, text, false, false);

					int xx, yy, ww = (w - 40) / 2, i = 0;

					foreach (var o in _CoreSettings.Skip(CoreSettingsPage * PanelRows).Take(PanelRows))
					{
						var opt = o;

						text = opt.Label;

						var len = text.Length;

						if (text.EndsWith("*"))
						{
							text = text.TrimEnd('*');
							text += "*".WrapUOHtmlColor(Color.OrangeRed, false);
						}

						if (len > 20)
						{
							text = text.WrapUOHtmlSmall();
						}

						text = text.WrapUOHtmlRight().WrapUOHtmlColor(Color.Gold, false);

						xx = x + 15;
						yy = y + 40 + (i * 30);

						AddBackground(xx, yy, ww, 30, 5120);
						AddButton(xx + 5, yy + 3, 4012, 4013, b => OnOptionInfo(opt)); // Info
						AddHtml(xx + 45, yy + 5, ww - 55, 40, text, false, false);

						text = GetValueString(opt);

						if (text.Length > 20)
						{
							text = text.WrapUOHtmlSmall();
						}

						text = text.WrapUOHtmlColor(Color.Gold, false);

						xx += ww + 10;

						AddBackground(xx, yy, ww, 30, 5120);
						AddButton(xx + 5, yy + 3, 4027, 4028, b => OnOptionEdit(opt)); // Edit
						AddHtml(xx + 45, yy + 5, ww - 55, 40, text, false, false);

						++i;
					}

					AddImageTiled(x + 10, y + (h - 20), w - 20, 10, 9277);

					AddScrollbar(
						Axis.Horizontal,
						x + 12,
						y + (h - 20),
						CoreSettingsPageCount,
						CoreSettingsPage,
						p =>
						{
							--CoreSettingsPage;
							Refresh(true);
						},
						n =>
						{
							++CoreSettingsPage;
							Refresh(true);
						},
						34,
						0,
						w - 88,
						13,
						10740,
						10741,
						0,
						0,
						28,
						13,
						10731,
						10732,
						10730,
						w - 50,
						0,
						28,
						13,
						10711,
						10712,
						10710);
				});
		}

		protected virtual void CompileTransactionsLayout(SuperGumpLayout layout, int x, int y, int w, int h)
		{
			layout.Add(
				"panels/transactions",
				() =>
				{
					if (InfoPanelMinimized)
					{
						AddButton(
							x,
							y - 25,
							4504,
							4504,
							b =>
							{
								InfoPanelMinimized = false;
								Refresh(true);
							});
						return;
					}

					AddBackground(x, y, w, h, 9270);

					AddButton(
						x,
						y - 25,
						4500,
						4500,
						b =>
						{
							InfoPanelMinimized = true;
							Refresh(true);
						});

					var text = "TRANSACTIONS".WrapUOHtmlBold();

					AddHtmlButton(
						x + 15,
						y + 15,
						(w - 30) / 2,
						30,
						b =>
						{
							InfoPanelTab = 0;
							Refresh(true);
						},
						text,
						Color.PaleGoldenrod,
						Color.Black,
						Color.PaleGoldenrod,
						2);

					text = "PROFILES".WrapUOHtmlBold();

					AddHtmlButton(
						x + 15 + ((w - 30) / 2),
						y + 15,
						(w - 30) / 2,
						30,
						b =>
						{
							InfoPanelTab = 1;
							Refresh(true);
						},
						text,
						Color.Silver,
						Color.Black,
						Color.Silver,
						2);

					int xx, yy, ww = (w - 40) / 2, i = 0;

					IEnumerable<DonationTransaction> list = AutoDonate.Transactions.Values;

					if (!String.IsNullOrWhiteSpace(TransactionsSearch))
					{
						var pat = new Regex(TransactionsSearch);

						list = list.Where(o => pat.IsMatch(GetSearchKey(o)));
					}

					list = list.OrderByDescending(o => o.Time);

					foreach (var o in list.Skip(TransactionsPage * (PanelRows - 2)).Take(PanelRows - 2))
					{
						var trans = o;

						text = trans.ID + " [" + trans.State + "]";

						if (text.Length > 20)
						{
							text = text.WrapUOHtmlSmall();
						}

						text = text.WrapUOHtmlRight().WrapUOHtmlColor(Color.Gold, false);

						xx = x + 15;
						yy = y + 50 + (i * 30);

						AddBackground(xx, yy, ww, 30, 5120);
						AddButton(xx + 5, yy + 3, 4018, 4019, b => OnTransactionDelete(trans)); // Delete
						AddHtml(xx + 45, yy + 5, ww - 55, 40, text, false, false);

						text = String.Format(
							"{0}{1:#,0.00} {2}",
							AutoDonate.CMOptions.MoneySymbol,
							trans.Total,
							AutoDonate.CMOptions.MoneyAbbr);

						if (text.Length > 20)
						{
							text = text.WrapUOHtmlSmall();
						}

						text = text.WrapUOHtmlColor(Color.Gold, false);

						xx += ww + 10;

						AddBackground(xx, yy, ww, 30, 5120);
						AddButton(xx + 5, yy + 3, 4012, 4013, b => OnTransactionInfo(trans)); // Info
						AddHtml(xx + 45, yy + 5, ww - 55, 40, text, false, false);

						++i;
					}

					xx = x + 15;
					yy = y + 50 + (i * 30);

					AddBackground(xx, yy, ww, 30, 5120);

					if (String.IsNullOrWhiteSpace(TransactionsSearch))
					{
						text = "Search...";
						text = text.WrapUOHtmlRight().WrapUOHtmlColor(Color.Gold, false);

						AddButton(xx + 5, yy + 3, 4006, 4007, b => OnTransactionSearch(false)); // Search
						AddHtml(xx + 45, yy + 5, ww - 55, 40, text, false, false);
					}
					else
					{
						text = TransactionsSearch;

						if (text.Length > 20)
						{
							text = text.WrapUOHtmlSmall();
						}

						text = text.WrapUOHtmlRight().WrapUOHtmlColor(Color.Gold, false);

						AddButton(xx + 5, yy + 3, 4021, 4022, b => OnTransactionSearch(true)); // Clear Search
						AddHtml(xx + 45, yy + 5, ww - 55, 40, text, false, false);
					}

					xx += ww + 10;

					AddBackground(xx, yy, ww, 30, 5120);

					ww /= 2;

					text = "Add";
					text = text.WrapUOHtmlColor(Color.Gold, false);

					AddButton(xx + 5, yy + 3, 4030, 4031, b => OnTransactionAdd()); // Add
					AddHtml(xx + 45, yy + 5, ww - 55, 40, text, false, false);

					xx += 45 + (ww - 55);

					text = "Import";
					text = text.WrapUOHtmlColor(Color.Gold, false);

					AddButton(xx + 5, yy + 3, 4030, 4031, b => OnTransactionImport()); // Import
					AddHtml(xx + 45, yy + 5, ww - 55, 40, text, false, false);

					AddImageTiled(x + 10, y + (h - 20), w - 20, 10, 9277);

					AddScrollbar(
						Axis.Horizontal,
						x + 12,
						y + (h - 20),
						TransactionsPageCount,
						TransactionsPage,
						p =>
						{
							--TransactionsPage;
							Refresh(true);
						},
						n =>
						{
							++TransactionsPage;
							Refresh(true);
						},
						34,
						0,
						w - 88,
						13,
						10740,
						10741,
						0,
						0,
						28,
						13,
						10731,
						10732,
						10730,
						w - 50,
						0,
						28,
						13,
						10711,
						10712,
						10710);
				});
		}

		protected virtual void CompileProfilesLayout(SuperGumpLayout layout, int x, int y, int w, int h)
		{
			layout.Add(
				"panels/profiles",
				() =>
				{
					if (InfoPanelMinimized)
					{
						AddButton(
							x,
							y - 25,
							4504,
							4504,
							b =>
							{
								InfoPanelMinimized = false;
								Refresh(true);
							});
						return;
					}

					AddBackground(x, y, w, h, 9270);

					AddButton(
						x,
						y - 25,
						4500,
						4500,
						b =>
						{
							InfoPanelMinimized = true;
							Refresh(true);
						});

					var text = "TRANSACTIONS".WrapUOHtmlBold();

					AddHtmlButton(
						x + 15,
						y + 15,
						(w - 30) / 2,
						30,
						b =>
						{
							InfoPanelTab = 0;
							Refresh(true);
						},
						text,
						Color.Silver,
						Color.Black,
						Color.Silver,
						2);

					text = "PROFILES".WrapUOHtmlBold();

					AddHtmlButton(
						x + 15 + ((w - 30) / 2),
						y + 15,
						(w - 30) / 2,
						30,
						b =>
						{
							InfoPanelTab = 1;
							Refresh(true);
						},
						text,
						Color.PaleGoldenrod,
						Color.Black,
						Color.PaleGoldenrod,
						2);

					int xx, yy, ww = (w - 40) / 2, i = 0;

					IEnumerable<DonationProfile> list = AutoDonate.Profiles.Values;

					if (!String.IsNullOrWhiteSpace(ProfilesSearch))
					{
						var pat = new Regex(ProfilesSearch);

						list = list.Where(o => pat.IsMatch(GetSearchKey(o)));
					}

					list = list.OrderByDescending(p => p.TotalValue);

					foreach (var o in list.Skip(ProfilesPage * (PanelRows - 2)).Take(PanelRows - 2))
					{
						var pro = o;

						text = pro.Account == null ? "(~unlinked~)" : pro.Account.Username;

						if (text.Length > 20)
						{
							text = text.WrapUOHtmlSmall();
						}

						text = text.WrapUOHtmlRight().WrapUOHtmlColor(Color.Gold, false);

						xx = x + 15;
						yy = y + 50 + (i * 30);

						AddBackground(xx, yy, ww, 30, 5120);
						AddButton(xx + 5, yy + 3, 4012, 4013, b => OnProfileInfo(pro)); // Info
						AddHtml(xx + 45, yy + 5, ww - 55, 40, text, false, false);

						text = String.Format(
							"{0}{1:#,0.00} {2} (Tier {3:#,0})",
							AutoDonate.CMOptions.MoneySymbol,
							pro.TotalValue,
							AutoDonate.CMOptions.MoneyAbbr,
							pro.Tier);

						if (text.Length > 20)
						{
							text = text.WrapUOHtmlSmall();
						}

						text = text.WrapUOHtmlColor(Color.Gold, false);

						xx += ww + 10;

						AddBackground(xx, yy, ww, 30, 5120);
						AddHtml(xx + 5, yy + 5, ww - 10, 40, text, false, false);

						++i;
					}

					xx = x + 15;
					yy = y + 50 + (i * 30);

					AddBackground(xx, yy, ww, 30, 5120);

					if (String.IsNullOrWhiteSpace(ProfilesSearch))
					{
						text = "Search...";
						text = text.WrapUOHtmlRight().WrapUOHtmlColor(Color.Gold, false);

						AddButton(xx + 5, yy + 3, 4006, 4007, b => OnProfileSearch(false)); // Search
						AddHtml(xx + 45, yy + 5, ww - 55, 40, text, false, false);
					}
					else
					{
						text = ProfilesSearch;

						if (text.Length > 20)
						{
							text = text.WrapUOHtmlSmall();
						}

						text = text.WrapUOHtmlRight().WrapUOHtmlColor(Color.Gold, false);

						AddButton(xx + 5, yy + 3, 4021, 4022, b => OnProfileSearch(true)); // Clear Search
						AddHtml(xx + 45, yy + 5, ww - 55, 40, text, false, false);
					}

					//xx += ww + 10;

					AddImageTiled(x + 10, y + (h - 20), w - 20, 10, 9277);

					AddScrollbar(
						Axis.Horizontal,
						x + 12,
						y + (h - 20),
						ProfilesPageCount,
						ProfilesPage,
						p =>
						{
							--ProfilesPage;
							Refresh(true);
						},
						n =>
						{
							++ProfilesPage;
							Refresh(true);
						},
						34,
						0,
						w - 88,
						13,
						10740,
						10741,
						0,
						0,
						28,
						13,
						10731,
						10732,
						10730,
						w - 50,
						0,
						28,
						13,
						10711,
						10712,
						10710);
				});
		}

		protected virtual void CompileFormSettingsLayout(SuperGumpLayout layout, int x, int y, int w, int h)
		{
			layout.Add(
				"panels/formsettings",
				() =>
				{
					if (FormSettingsMinimized)
					{
						AddButton(
							x - 25,
							y - 25,
							4503,
							4503,
							b =>
							{
								FormSettingsMinimized = false;
								Refresh(true);
							});
						return;
					}

					AddBackground(x, y, w, h, 9270);

					AddButton(
						x - 25,
						y - 25,
						4507,
						4507,
						b =>
						{
							FormSettingsMinimized = true;
							Refresh(true);
						});

					var text = "FORM SETTINGS".WrapUOHtmlBold().WrapUOHtmlCenter().WrapUOHtmlColor(Color.PaleGoldenrod, false);

					AddHtml(x + 15, y + 15, w - 30, 40, text, false, false);

					int xx, yy, ww = (w - 40) / 2, i = 0;

					foreach (var o in _FormSettings.Skip(FormSettingsPage * PanelRows).Take(PanelRows))
					{
						var opt = o;

						text = opt.Label;

						var len = text.Length;

						if (text.EndsWith("*"))
						{
							text = text.TrimEnd('*');
							text += "*".WrapUOHtmlColor(Color.OrangeRed, false);
						}

						if (len > 20)
						{
							text = text.WrapUOHtmlSmall();
						}

						text = text.WrapUOHtmlRight().WrapUOHtmlColor(Color.Gold, false);

						xx = x + 15;
						yy = y + 40 + (i * 30);

						AddBackground(xx, yy, ww, 30, 5120);
						AddButton(xx + 5, yy + 3, 4012, 4013, b => OnOptionInfo(opt)); // Info
						AddHtml(xx + 45, yy + 5, ww - 55, 40, text, false, false);

						text = GetValueString(opt);

						if (text.Length > 20)
						{
							text = text.WrapUOHtmlSmall();
						}

						text = text.WrapUOHtmlColor(Color.Gold, false);

						xx += ww + 10;

						AddBackground(xx, yy, ww, 30, 5120);
						AddButton(xx + 5, yy + 3, 4027, 4028, b => OnOptionEdit(opt)); // Edit
						AddHtml(xx + 45, yy + 5, ww - 55, 40, text, false, false);

						++i;
					}

					AddImageTiled(x + 10, y + (h - 20), w - 20, 10, 9277);

					AddScrollbar(
						Axis.Horizontal,
						x + 12,
						y + (h - 20),
						FormSettingsPageCount,
						FormSettingsPage,
						p =>
						{
							--FormSettingsPage;
							Refresh(true);
						},
						n =>
						{
							++FormSettingsPage;
							Refresh(true);
						},
						34,
						0,
						w - 88,
						13,
						10740,
						10741,
						0,
						0,
						28,
						13,
						10731,
						10732,
						10730,
						w - 50,
						0,
						28,
						13,
						10711,
						10712,
						10710);
				});
		}

		protected virtual void OnOptionInfo(OptionEntry entry)
		{
			new NoticeDialogGump(User, Refresh())
			{
				Title = entry.Label,
				Html = entry.Info
			}.Send();
		}

		protected virtual void OnOptionEdit(OptionEntry entry)
		{
			var val = entry.Value;

			if (val is bool)
			{
				OnOptionEditBool(entry, (bool)val);
			}
			else if (val is sbyte || val is short || val is int || val is long)
			{
				OnOptionEditNumber(entry, (long)val);
			}
			else if (val is byte || val is ushort || val is uint || val is ulong)
			{
				OnOptionEditNumber(entry, (ulong)val);
			}
			else if (val is float || val is decimal || val is double)
			{
				OnOptionEditNumber(entry, (double)val);
			}
			else if (val is string)
			{
				OnOptionEditString(entry, (string)val);
			}
			else if (val is char)
			{
				OnOptionEditChar(entry, (char)val);
			}
			else
			{
				OnOptionEditObject(entry, val);
			}
		}

		protected virtual void OnOptionEditBool(OptionEntry entry, bool value)
		{
			entry.Value = !value;

			Refresh(true);
		}

		protected virtual void OnOptionEditNumber(OptionEntry entry, long value)
		{
			new InputDialogGump(User, Refresh())
			{
				Title = entry.Label,
				Html = entry.Info,
				InputText = value.ToString(),
				Callback = (b, v) =>
				{
					if (v.TryParse(out value))
					{
						SetValue(entry, value);
					}

					Refresh(true);
				}
			}.Send();
		}

		protected virtual void OnOptionEditNumber(OptionEntry entry, ulong value)
		{
			new InputDialogGump(User, Refresh())
			{
				Title = entry.Label,
				Html = entry.Info,
				InputText = value.ToString(),
				Callback = (b, v) =>
				{
					if (v.TryParse(out value))
					{
						SetValue(entry, value);
					}

					Refresh(true);
				}
			}.Send();
		}

		protected virtual void OnOptionEditNumber(OptionEntry entry, double value)
		{
			new InputDialogGump(User, Refresh())
			{
				Title = entry.Label,
				Html = entry.Info,
				InputText = value.ToString("F"),
				Callback = (b, v) =>
				{
					if (v.TryParse(out value))
					{
						SetValue(entry, value);
					}

					Refresh(true);
				}
			}.Send();
		}

		protected virtual void OnOptionEditString(OptionEntry entry, string value)
		{
			new InputDialogGump(User, Refresh())
			{
				Title = entry.Label,
				Html = entry.Info,
				InputText = value,
				Callback = (b, v) =>
				{
					SetValue(entry, v);

					Refresh(true);
				}
			}.Send();
		}

		protected virtual void OnOptionEditChar(OptionEntry entry, char value)
		{
			new InputDialogGump(User, Refresh())
			{
				Limit = 1,
				Title = entry.Label,
				Html = entry.Info,
				InputText = value.ToString(),
				Callback = (b, v) =>
				{
					SetValue(entry, v);

					Refresh(true);
				}
			}.Send();
		}

		protected virtual void OnOptionEditObject(OptionEntry entry, object value)
		{
			new InputDialogGump(User, Refresh())
			{
				Title = entry.Label,
				Html = entry.Info,
				InputText = value.ToString(),
				Callback = (b, v) =>
				{
					if (v.TryParse(out value))
					{
						SetValue(entry, value);
					}

					Refresh(true);
				}
			}.Send();
		}

		protected virtual void OnProfileSearch(bool clear)
		{
			if (clear)
			{
				ProfilesSearch = String.Empty;
				Refresh(true);
				return;
			}

			new InputDialogGump(User, Refresh())
			{
				Title = "Profile Search",
				Html = "Filter profiles that match the search query.",
				Callback = (b, t) =>
				{
					ProfilesSearch = t;
					Refresh(true);
				}
			}.Send();
		}

		protected virtual string GetSearchKey(DonationProfile profile)
		{
			return String.Format("{0}", profile.Account);
		}

		protected virtual void OnProfileInfo(DonationProfile profile)
		{
			Refresh(true);

			DonationProfileUI.DisplayTo(User, profile, false);
		}

		protected virtual void OnTransactionDelete(DonationTransaction trans)
		{
			new ConfirmDialogGump(User, Refresh())
			{
				Title = "Delete Transaction",
				Html = "Click OK to delete this transaction.\nThis action can not be undone!",
				AcceptHandler = b =>
				{
					if (!trans.Deleted)
					{
						trans.Delete();

						User.SendMessage(0x55, "Transaction '{0}' has been deleted.", trans.ID);
					}

					Refresh(true);
				},
				CancelHandler = Refresh
			}.Send();
		}

		protected virtual void OnTransactionInfo(DonationTransaction trans)
		{
			Refresh(true);

			var profile = AutoDonate.EnsureProfile(trans.Account);

			DonationProfileUI.DisplayTo(User, profile, trans);
		}

		protected virtual void OnTransactionSearch(bool clear)
		{
			if (clear)
			{
				TransactionsSearch = String.Empty;
				Refresh(true);
				return;
			}

			new InputDialogGump(User, Refresh())
			{
				Title = "Transaction Search",
				Html = "Filter transactions that match the search query.",
				Callback = (b, t) =>
				{
					TransactionsSearch = t;
					Refresh(true);
				}
			}.Send();
		}

		protected virtual string GetSearchKey(DonationTransaction trans)
		{
			return String.Format(
				"{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
				trans.ID,
				trans.State,
				trans.Time,
				trans.Account,
				trans.Email,
				trans.Notes,
				trans.Extra,
				trans.DeliveredTo ?? String.Empty);
		}

		protected virtual void OnTransactionImport()
		{
			var input = new InputDialogGump(User, Hide())
			{
				Title = "Import Transactions",
				Html = "Import transactions from a file.\n\n" + //
					   "File Formats: \n*.json\n\n" + //
					   "Root Directory: " + Core.BaseDirectory
			};

			input.AcceptHandler = b =>
			{
				input.InputText = IOUtility.GetSafeFilePath(input.InputText, true);

				var path = Path.Combine(Core.BaseDirectory, input.InputText);

				if (!Insensitive.EndsWith(path, ".json"))
				{
					input.InputText = String.Empty;
					input.User.SendMessage("File not supported: {0}", path);
					input.Refresh(true);
					return;
				}

				if (!File.Exists(path))
				{
					input.InputText = String.Empty;
					input.User.SendMessage("File not found: {0}", path);
					input.Refresh(true);
					return;
				}

				OnTransactionImport(new FileInfo(path));
			};

			input.CancelHandler = Refresh;

			input.Send();
		}

		protected virtual void OnTransactionImport(FileInfo file)
		{
			var o = Json.Decode(file.ReadAllText(), out JsonException e);

			if (e != null)
			{
				User.SendMessage(e.ToString());

				Refresh(true);
				return;
			}

			var list = new List<DonationTransaction>();

			DonationTransaction trans;

			string id, email, notes, extra;
			double total, time;
			long credit;
			int version;
			TransactionState state;
			IAccount account;

			var stateType = typeof(TransactionState);

			if (o is IEnumerable<object>)
			{
				var col = ((IEnumerable<object>)o).OfType<IDictionary<string, object>>();

				foreach (var obj in col)
				{
					/*try
					{*/

					id = (string)obj.GetValue("id");
					state = (TransactionState)Enum.Parse(stateType, (string)obj.GetValue("state"), true);
					account = Accounts.GetAccount((string)obj.GetValue("account"));
					email = (string)obj.GetValue("email");
					total = (double)obj.GetValue("total");
					credit = (long)(double)obj.GetValue("credit");
					time = (double)obj.GetValue("time");
					version = (int)(double)obj.GetValue("version");
					notes = (string)obj.GetValue("notes");
					extra = (string)obj.GetValue("extra");

					trans = CreateTransaction(id, state, account, email, total, credit, time, version, notes, extra);
					/*}
					catch
					{
						trans = null;
					}*/

					if (trans != null)
					{
						list.Add(trans);
					}
				}
			}
			else if (o is IDictionary<string, object>)
			{
				var obj = (IDictionary<string, object>)o;

				/*try
				{*/
				id = (string)obj.GetValue("id");
				state = (TransactionState)Enum.Parse(stateType, (string)obj.GetValue("state"), true);
				account = Accounts.GetAccount((string)obj.GetValue("account"));
				email = (string)obj.GetValue("email");
				total = (double)obj.GetValue("total");
				credit = (long)(double)obj.GetValue("credit");
				time = (double)obj.GetValue("time");
				version = (int)(double)obj.GetValue("version");
				notes = (string)obj.GetValue("notes");
				extra = (string)obj.GetValue("extra");

				trans = CreateTransaction(id, state, account, email, total, credit, time, version, notes, extra);
				/*}
				catch
				{
					trans = null;
				}*/

				if (trans != null)
				{
					list.Add(trans);
				}
			}

			var count = 0;

			if (list.Count > 0)
			{
				DonationProfile p;
				DonationTransaction ot;

				foreach (var g in list.Where(t => t.Account != null).ToLookup(t => t.Account, t => t))
				{
					p = AutoDonate.EnsureProfile(g.Key);

					if (p == null)
					{
						continue;
					}

					foreach (var t in g)
					{
						ot = p[t.ID];

						if (ot == null || ot.Version <= t.Version)
						{
							p.Add(t);

							++count;
						}
					}
				}
			}

			if (count > 0)
			{
				User.SendMessage("Imported {0:#,0} transactions!", count);
			}
			else
			{
				User.SendMessage("No transactions to import.");
			}

			Refresh(true);
		}

		protected virtual DonationTransaction CreateTransaction(
			string id,
			TransactionState state,
			IAccount account,
			string email,
			double total,
			long credit,
			double time,
			int version,
			string notes,
			string extra)
		{
			using (var stream = new MemoryStream())
			{
				var writer = stream.GetBinaryWriter();

				writer.SetVersion(0);

				writer.Write(id);
				writer.WriteFlag(state);
				writer.Write(account);
				writer.Write(email);
				writer.Write(total);
				writer.Write(credit);
				writer.Write(time);
				writer.Write(version);
				writer.Write(0);

				writer.Write(notes);
				writer.Write(extra);

				var mobiles = account.FindMobiles<PlayerMobile>().OrderByDescending(m => m.GameTime.Ticks);
				var recipient = mobiles.FirstOrDefault();

				writer.Write(recipient);
				writer.Write(recipient);
				writer.Write(0.0);

				writer.Flush();

				stream.Position = 0;

				var reader = stream.GetBinaryReader();

				var t = new DonationTransaction(reader);

				writer.Close();
				reader.Close();

				return t;
			}
		}

		protected virtual void OnTransactionAdd()
		{
			var state = new TransactionAddState
			{
				Account = User.Account,
				Email = User.Account.Email,
				ID = String.Empty,
				Value = 0.0
			};

			OnTransactionAdd1(state);
		}

		protected virtual void OnTransactionAdd1(TransactionAddState state)
		{
			var ui = new InputDialogGump(User, Refresh())
			{
				Title = "Add Transaction: Step 1",
				Html = "Manually add a transaction.\n\n" + //
					   "Account Name:\nEnter the account name of the recipient for this transaction.\n\n" + //
					   "Click OK to continue or Cancel to exit.", //
				InputText = User.Account.Username,
				CancelHandler = Refresh
			};

			ui.Callback = (b, username) => OnTransactionAdd1(state, ui);

			ui.Send();
		}

		protected virtual void OnTransactionAdd1(TransactionAddState state, InputDialogGump ui)
		{
			IAccount acc;

			if (String.IsNullOrWhiteSpace(ui.InputText) || (acc = Accounts.GetAccount(ui.InputText)) == null)
			{
				User.SendMessage(0x22, "Account '{0}' could not be found.", ui.InputText);
				ui.Refresh();
				return;
			}

			state.Account = acc;
			state.Email = acc.Email;

			OnTransactionAdd2(state);
		}

		protected virtual void OnTransactionAdd2(TransactionAddState state)
		{
			var ui = new InputDialogGump(User, Refresh())
			{
				Title = "Add Transaction: Step 2",
				Html = "Manually add a transaction.\n\n" + //
					   "Email Address:\nEnter the Email Address for this transaction.\n\n" + //
					   "Click OK to continue or Cancel to exit.", //
				InputText = state.Email,
				CancelHandler = Refresh
			};

			ui.Callback = (b, email) => OnTransactionAdd2(state, ui);

			ui.Send();
		}

		protected virtual void OnTransactionAdd2(TransactionAddState state, InputDialogGump ui)
		{
			if (String.IsNullOrWhiteSpace(ui.InputText) || ui.InputText.Count(c => c == '@') != 1 ||
				ui.InputText.LastIndexOf('.') < ui.InputText.LastIndexOf('@') ||
				ui.InputText.Any(c => c != '.' && c != '@' && c != '-' && c != '_' && !Char.IsLetterOrDigit(c)))
			{
				User.SendMessage(0x22, "Email '{0}' is not valid.", ui.InputText);
				ui.Refresh();
				return;
			}

			state.Email = ui.InputText.Trim();

			OnTransactionAdd3(state);
		}

		protected virtual void OnTransactionAdd3(TransactionAddState state)
		{
			var ui = new InputDialogGump(User, Refresh())
			{
				Title = "Add Transaction: Step 3",
				Html = "Manually add a transaction.\n\n" + //
					   "Transaction ID:\nEnter the unique ID for this transaction.\n\n" + //
					   "Click OK to continue or Cancel to exit.", //
				InputText = state.ID,
				CancelHandler = Refresh
			};

			ui.Callback = (b, id) => OnTransactionAdd3(state, ui);

			ui.Send();
		}

		protected virtual void OnTransactionAdd3(TransactionAddState state, InputDialogGump ui)
		{
			if (String.IsNullOrWhiteSpace(ui.InputText) || AutoDonate.Transactions.ContainsKey(ui.InputText))
			{
				User.SendMessage(0x22, "Transaction ID '{0}' is not valid.", ui.InputText);
				ui.Refresh();
				return;
			}

			state.ID = ui.InputText.Trim();

			OnTransactionAdd4(state);
		}

		protected virtual void OnTransactionAdd4(TransactionAddState state)
		{
			var ui = new InputDialogGump(User, Refresh())
			{
				Title = "Add Transaction: Step 4",
				Html = "Manually add a transaction.\n\n" + //
					   "Value:\nEnter the " + AutoDonate.CMOptions.MoneySymbol + " " + AutoDonate.CMOptions.MoneyAbbr +
					   " value for this transaction.\n\n" + //
					   "Click OK to continue or Cancel to exit.", //
				InputText = state.Value.ToString("F2"),
				CancelHandler = Refresh
			};

			ui.Callback = (b, val) => OnTransactionAdd4(state, ui);

			ui.Send();
		}

		protected virtual void OnTransactionAdd4(TransactionAddState state, InputDialogGump ui)
		{
			if (String.IsNullOrWhiteSpace(ui.InputText) || !Double.TryParse(ui.InputText, out var value))
			{
				User.SendMessage(0x22, "Value '{0}' is not valid.", ui.InputText);
				ui.Refresh();
				return;
			}

			state.Value = value;

			OnTransactionAdd(state);
		}

		protected virtual void OnTransactionAdd(TransactionAddState state)
		{
			DonationTransaction trans = null;

			var profile = AutoDonate.EnsureProfile(state.Account);

			if (profile != null)
			{
				trans = new DonationTransaction(
					state.ID,
					state.Account,
					state.Email,
					state.Value,
					(long)(state.Value / AutoDonate.CMOptions.CurrencyPrice),
					String.Empty,
					"{MANUAL ADD}");

				profile.Add(trans);

				if (trans.Process())
				{
					AutoDonate.SpotCheck(trans.Account);
				}
			}

			Refresh(true);

			if (trans != null)
			{
				DonationProfileUI.DisplayTo(User, profile, trans);
			}
		}

		protected sealed class TransactionAddState
		{
			public IAccount Account { get; set; }
			public string Email { get; set; }
			public string ID { get; set; }
			public double Value { get; set; }
		}
	}
}