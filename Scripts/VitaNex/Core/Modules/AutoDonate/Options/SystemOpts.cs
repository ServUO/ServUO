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
using System.Linq;

using Server;
using Server.Accounting;
#endregion

namespace VitaNex.Modules.AutoDonate
{
	public sealed class DonationOptions : CoreModuleOptions
	{
		[CommandProperty(AutoDonate.Access)]
		public DonationStatistics Info { get; set; }

		[CommandProperty(AutoDonate.Access)]
		public bool ShowHistory { get; set; }

		[CommandProperty(AutoDonate.Access)]
		public char MoneySymbol { get; set; }

		[CommandProperty(AutoDonate.Access)]
		public double TierFactor { get; set; }

		[CommandProperty(AutoDonate.Access)]
		public double CreditBonus { get; set; }

		[CommandProperty(AutoDonate.Access)]
		public DonationWebFormOptions WebForm { get; set; }

		[CommandProperty(AutoDonate.Access)]
		public string Business { get => WebForm.Business; set => WebForm.Business = value; }

		[CommandProperty(AutoDonate.Access)]
		public string MoneyAbbr { get => WebForm.Currency; set => WebForm.Currency = value; }

		[CommandProperty(AutoDonate.Access)]
		public double CurrencyPrice { get => WebForm.ItemValue; set => WebForm.ItemValue = value; }

		[CommandProperty(AutoDonate.Access)]
		public string CurrencyName { get => WebForm.ItemName; set => WebForm.ItemName = value; }

		private ItemTypeSelectProperty _CurrencyType = new ItemTypeSelectProperty();

		[CommandProperty(AutoDonate.Access)]
		public ItemTypeSelectProperty CurrencyType
		{
			get
			{
				if (_CurrencyType.TypeName != WebForm.ItemType)
				{
					_CurrencyType.TypeName = WebForm.ItemType;
				}

				return _CurrencyType;
			}
			set
			{
				if (value != null)
				{
					_CurrencyType = value;
				}
				else
				{
					_CurrencyType.TypeName = String.Empty;
				}

				WebForm.ItemType = _CurrencyType.TypeName;
			}
		}

		private IAccount _FallbackAccount;

		public IAccount FallbackAccount
		{
			get
			{
				ValidateFallbackAccount(ref _FallbackAccount);
				return _FallbackAccount;
			}
			set
			{
				_FallbackAccount = value;
				ValidateFallbackAccount(ref _FallbackAccount);
			}
		}

		[CommandProperty(AutoDonate.Access)]
		public string FallbackUsername
		{
			get => FallbackAccount.Username;
			set => FallbackAccount = Accounts.GetAccount(value);
		}

		private static void ValidateFallbackAccount(ref IAccount acc)
		{
			if (acc == null)
			{
				acc = Accounts.GetAccounts().FirstOrDefault(a => a.AccessLevel == AccessLevel.Owner);
			}
		}

		public DonationOptions()
			: base(typeof(AutoDonate))
		{
			WebForm = new DonationWebFormOptions();

			MoneySymbol = '$';
			ShowHistory = false;
			TierFactor = 0.0;
			CreditBonus = 0.0;

			Info = new DonationStatistics();
		}

		public DonationOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			base.Clear();

			MoneySymbol = ' ';
			ShowHistory = false;
			TierFactor = 0.0;
			CreditBonus = 0.0;
		}

		public override void Reset()
		{
			base.Reset();

			MoneySymbol = '$';
			ShowHistory = false;
			TierFactor = 100.0;
			CreditBonus = 0.0;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(4);

			switch (version)
			{
				case 4:
					writer.Write(FallbackAccount);
					goto case 3;
				case 3:
					writer.Write(CreditBonus);
					goto case 2;
				case 2:
					WebForm.Serialize(writer);
					goto case 1;
				case 1:
					writer.Write(TierFactor);
					goto case 0;
				case 0:
				{
					writer.Write(ShowHistory);
					writer.Write(MoneySymbol);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			if (version < 2)
			{
				WebForm = new DonationWebFormOptions();
			}

			switch (version)
			{
				case 4:
					FallbackAccount = reader.ReadAccount();
					goto case 3;
				case 3:
					CreditBonus = reader.ReadDouble();
					goto case 2;
				case 2:
					WebForm = new DonationWebFormOptions(reader);
					goto case 1;
				case 1:
					TierFactor = reader.ReadDouble();
					goto case 0;
				case 0:
				{
					if (version < 2)
					{
						#region MySQL
						reader.ReadInt();
						reader.ReadInt();
						reader.ReadString();
						reader.ReadShort();
						reader.ReadString();
						reader.ReadString();
						reader.ReadString();
						reader.ReadByte();
						reader.ReadInt();
						reader.ReadString();
						reader.ReadString();
						#endregion

						_CurrencyType = new ItemTypeSelectProperty(reader); // CurrencyType

						reader.ReadString(); // TableName
					}

					ShowHistory = reader.ReadBool();

					if (version < 2)
					{
						CurrencyPrice = reader.ReadDouble(); // UnitPrice
					}

					MoneySymbol = reader.ReadChar();

					if (version < 2)
					{
						MoneyAbbr = reader.ReadString(); // MoneyAbbr
						reader.ReadBool(); // GiftingEnabled
					}
				}
				break;
			}

			Info = new DonationStatistics();
		}
	}
}
