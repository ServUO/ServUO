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
using System.Linq;

using Server;
using Server.Items;

using VitaNex.Crypto;
using VitaNex.Items;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public abstract class BaseTrashHandler
	{
		public static Type[] DefaultAcceptList = { typeof(Item) };

		public static Type[] DefaultIgnoredList =
		{
			typeof(Gold), typeof(Bandage), typeof(Bottle), typeof(BaseReagent), typeof(IVendorToken), typeof(BaseTrashContainer)
		};

		private int _BonusTokens;
		private int _BonusTokensChance;

		private bool _Enabled = true;

		private TrashPriority _Priority;

		[CommandProperty(TrashCollection.Access)]
		public int BonusTokensChance
		{
			get => _BonusTokensChance;
			set => _BonusTokensChance = Math.Max(0, Math.Min(100, value));
		}

		public string UID { get; private set; }

		[CommandProperty(TrashCollection.Access)]
		public bool Enabled
		{
			get
			{
				if (!TrashCollection.CMOptions.ModuleEnabled)
				{
					return false;
				}

				return _Enabled;
			}
			set => _Enabled = value;
		}

		[CommandProperty(TrashCollection.Access)]
		public TrashPriority Priority
		{
			get => _Priority;
			set
			{
				if (_Priority == value)
				{
					return;
				}

				_Priority = value;
				TrashCollection.InvalidateHandlers();
			}
		}

		[CommandProperty(TrashCollection.Access)]
		public int BonusTokens { get => _BonusTokens; set => _BonusTokens = Math.Max(0, value); }

		public List<Type> Accepted { get; protected set; }
		public List<Type> Ignored { get; protected set; }

		[CommandProperty(TrashCollection.Access)]
		public bool IgnoreBlessed { get; set; }

		[CommandProperty(TrashCollection.Access)]
		public bool IgnoreInsured { get; set; }

		public BaseTrashHandler()
			: this(true)
		{ }

		public BaseTrashHandler(
			bool enabled,
			TrashPriority priority = TrashPriority.Normal,
			IEnumerable<Type> accepts = null,
			IEnumerable<Type> ignores = null)
		{
			UID = CryptoGenerator.GenString(CryptoHashType.MD5, GetType().FullName);

			Accepted = new List<Type>(accepts ?? DefaultAcceptList);
			Ignored = new List<Type>(ignores ?? DefaultIgnoredList);

			Enabled = enabled;
			Priority = priority;
		}

		public BaseTrashHandler(GenericReader reader)
		{
			Deserialize(reader);
		}

		public bool Trash(Mobile from, Item trashed, bool message = true)
		{
			var tokens = 0;

			return Trash(from, trashed, ref tokens, message);
		}

		public bool Trash(Mobile from, Item trashed, ref int tokens, bool message = true)
		{
			tokens = Math.Max(0, tokens);

			if (!Enabled || from == null || trashed == null || trashed.Deleted)
			{
				return false;
			}

			var andThis = true;
			var multiple = false;

			if (trashed is Container)
			{
				if (trashed.Items.Count > 0)
				{
					var i = trashed.Items.Count;

					while (--i >= 0)
					{
						if (!trashed.Items.InBounds(i))
						{
							continue;
						}

						if (Trash(from, trashed.Items[i], ref tokens, false))
						{
							multiple = true;
						}
						else
						{
							andThis = false;
						}
					}
				}

				if (trashed.Items.Count > 0)
				{
					andThis = false;
				}
			}

			if (multiple && message)
			{
				from.SendMessage(0x55, "You trashed multiple items, check your profile history for more information.");
			}

			if (!CanTrash(from, trashed))
			{
				andThis = false;
			}

			if (!andThis)
			{
				OnTrashRejected(from, trashed, message);
				return false;
			}

			GetTrashTokens(from, trashed, ref tokens);
			OnTrashed(from, trashed, ref tokens, message);

			var e = new ItemTrashedEventArgs(this, from, trashed, tokens, message);

			VitaNexCore.TryCatch(e.Invoke, TrashCollection.CMOptions.ToConsole);

			if (tokens != e.Tokens)
			{
				tokens = Math.Max(0, e.Tokens);
			}

			message = e.Message;

			if (!e.HandledTokens)
			{
				TrashCollection.EnsureProfile(from).TransferTokens(from, trashed, tokens, message);
			}

			if (!multiple && from.Backpack != null && TrashCollection.CMOptions.UseTrashedProps)
			{
				from.Backpack.InvalidateProperties<ITrashTokenProperties>();
			}

			trashed.Delete();
			return true;
		}

		public virtual bool IsAccepted(Mobile from, Type trash)
		{
			return Accepted.Any(trash.IsEqualOrChildOf);
		}

		public virtual bool IsIgnored(Mobile from, Type trash)
		{
			return Ignored.Any(trash.IsEqualOrChildOf);
		}

		public virtual bool CanTrash(Mobile from, Item trash, bool message = true)
		{
			if (!Enabled || trash == null || trash.Deleted || !trash.Movable || !trash.IsAccessibleTo(from))
			{
				return false;
			}

			if (IgnoreInsured && trash.Insured)
			{
				return false;
			}

			if (IgnoreBlessed && (trash.LootType == LootType.Blessed || trash.BlessedFor != null))
			{
				return false;
			}

			var iType = trash.GetType();

			return IsAccepted(from, iType) && !IsIgnored(from, iType);
		}

		protected virtual void OnTrashed(Mobile from, Item trashed, ref int tokens, bool message = true)
		{
			tokens = Math.Max(0, tokens);
		}

		protected virtual void OnTrashRejected(Mobile from, Item trashed, bool message = true)
		{ }

		protected virtual void GetTrashTokens(Mobile from, Item trashed, ref int tokens, bool message = true)
		{
			tokens = Math.Max(0, tokens);

			if (!Enabled || from == null || trashed == null || trashed.Deleted)
			{
				return;
			}

			var amount = trashed.GetAttributeCount();

			if (trashed.Stackable)
			{
				amount *= trashed.Amount;
			}

			if (TrashCollection.CMOptions.GiveBonusTokens && BonusTokens > 0 &&
				Utility.RandomDouble() <= BonusTokensChance / 100.0)
			{
				amount += BonusTokens;
			}

			if (amount > 0)
			{
				tokens += amount;
			}
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				{
					writer.Write(IgnoreBlessed);
					writer.Write(IgnoreInsured);
				}
				goto case 0;
				case 0:
				{
					writer.Write(Enabled);
					writer.Write(UID);
					writer.WriteFlag(_Priority);
					writer.Write(_BonusTokens);
					writer.Write(_BonusTokensChance);
					writer.WriteList(Accepted, t => writer.WriteType(t));
					writer.WriteList(Ignored, t => writer.WriteType(t));
				}
				break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 1:
				{
					IgnoreBlessed = reader.ReadBool();
					IgnoreInsured = reader.ReadBool();
				}
				goto case 0;
				case 0:
				{
					Enabled = reader.ReadBool();
					UID = reader.ReadString();
					_Priority = reader.ReadFlag<TrashPriority>();
					_BonusTokens = reader.ReadInt();
					_BonusTokensChance = reader.ReadInt();
					Accepted = reader.ReadList(reader.ReadType);
					Ignored = reader.ReadList(reader.ReadType);
				}
				break;
			}

			if (version < 1)
			{
				IgnoreBlessed = true;
				IgnoreInsured = true;
			}
		}
	}
}