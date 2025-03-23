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
using System.Text;

using Server;

using VitaNex.Text;
#endregion

namespace VitaNex.Modules.AutoDonate
{
	public sealed class DonationWebFormOptions : PropertyObject
	{
		private static readonly Dictionary<string, object> _DefOptions = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
		{
			{"test", false},
			{"command", "_donations"},
			{"business", String.Empty},
			{"notifyUrl", String.Empty},
			{"returnUrl", String.Empty},
			{"verifyUrl", String.Empty},
			{"currency", "USD"},
			{"itemName", "Gold Coins"},
			{"itemType", "Gold"},
			{"itemValue", 1.00},
			{"amountDef", 25.00},
			{"amountMin", 5.00},
			{"amountMax", 500.00},
			{"amountInc", 1.00},
			{"buttonName", "Donate"},
			{"bannerUrl", String.Empty},
			{"bannerImg", String.Empty},
			{"shard", String.Empty}
		};

		private readonly Dictionary<string, object> _Options = new Dictionary<string, object>(_DefOptions, StringComparer.OrdinalIgnoreCase);

		[CommandProperty(AutoDonate.Access)]
		public bool Test { get => (bool)_Options["test"]; set => _Options["test"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public string Command { get => (string)_Options["command"]; set => _Options["command"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public string Business { get => (string)_Options["business"]; set => _Options["business"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public string NotifyUrl { get => (string)_Options["notifyUrl"]; set => _Options["notifyUrl"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public string ReturnUrl { get => (string)_Options["returnUrl"]; set => _Options["returnUrl"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public string VerifyUrl { get => (string)_Options["verifyUrl"]; set => _Options["verifyUrl"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public string Currency { get => (string)_Options["currency"]; set => _Options["currency"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public string ItemName { get => (string)_Options["itemName"]; set => _Options["itemName"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public string ItemType { get => (string)_Options["itemType"]; set => _Options["itemType"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public double ItemValue { get => (double)_Options["itemValue"]; set => _Options["itemValue"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public double AmountDef { get => (double)_Options["amountDef"]; set => _Options["amountDef"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public double AmountMin { get => (double)_Options["amountMin"]; set => _Options["amountMin"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public double AmountMax { get => (double)_Options["amountMax"]; set => _Options["amountMax"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public double AmountInc { get => (double)_Options["amountInc"]; set => _Options["amountInc"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public string ButtonName { get => (string)_Options["buttonName"]; set => _Options["buttonName"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public string BannerUrl { get => (string)_Options["bannerUrl"]; set => _Options["bannerUrl"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public string BannerImg { get => (string)_Options["bannerImg"]; set => _Options["bannerImg"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public string Shard { get => (string)_Options["shard"]; set => _Options["shard"] = value; }

		[CommandProperty(AutoDonate.Access)]
		public bool Enabled { get; set; }

		public Action<StringBuilder> GenerationHandler { get; set; }

		public DonationWebFormOptions()
		{ }

		public DonationWebFormOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			Enabled = false;

			foreach (var kv in _DefOptions)
			{
				_Options[kv.Key] = kv.Value;
			}
		}

		public override void Reset()
		{
			Enabled = false;

			foreach (var kv in _DefOptions)
			{
				_Options[kv.Key] = kv.Value;
			}
		}

		public string GetJsonOptions()
		{
			return Json.Encode(_Options);
		}

		public void SetJsonOptions(string json)
		{
			if (!Json.Decode(json, out var obj, out var e))
			{
				e.ToConsole();
				return;
			}

			if (obj is Dictionary<string, object>)
			{
				foreach (var kv in (Dictionary<string, object>)obj)
				{
					_Options[kv.Key] = kv.Value;
				}
			}
		}

		public string Generate()
		{
			var html = new StringBuilder();

			html.AppendLine("<!DOCTYPE html>");
			html.AppendLine("<html>");
			html.AppendLine("\t<head>");
			html.AppendLine("\t\t<title>{0} - Donate</title>", Shard);
			html.AppendLine("\t\t<link rel='stylesheet' type='text/css' href='http://www.vita-nex.com/js/inc/index.css' />");
			html.AppendLine("\t\t<script type='text/javascript' src='http://www.vita-nex.com/js/mod/donateForm.js'></script>");
			html.AppendLine("\t</head>");
			html.AppendLine("\t<body>");
			html.AppendLine("\t\t<div id='index'>");
			html.AppendLine("\t\t\t<form class='donate-form' data-options='{0}'></form>", GetJsonOptions());
			html.AppendLine("\t\t</div>");
			html.AppendLine("\t</body>");
			html.AppendLine("</html>");

			if (GenerationHandler != null)
			{
				GenerationHandler(html);
			}

			return html.ToString();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(Enabled);

			writer.Write(GetJsonOptions());
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Enabled = reader.ReadBool();

			SetJsonOptions(reader.ReadString());
		}
	}
}