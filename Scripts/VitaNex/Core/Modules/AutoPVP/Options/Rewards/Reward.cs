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

using Server;
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public enum PvPRewardDeliveryMethod
	{
		None,
		Custom,
		Backpack,
		Bank
	}

	public enum PvPRewardClass
	{
		None,
		Custom,
		Item
	}

	public class PvPReward : PropertyObject
	{
		private class InternalTypeSelect : ItemTypeSelectProperty
		{
			private readonly PvPReward _Owner;

			[CommandProperty(AutoPvP.Access)]
			public override string TypeName
			{
				get => base.TypeName;
				set
				{
					base.TypeName = value;

					if (_Owner == null)
					{
						return;
					}

					if (InternalType != null)
					{
						if (InternalType.IsConstructableFrom<Item>())
						{
							_Owner.Class = PvPRewardClass.Item;
							_Owner.DeliveryMethod = PvPRewardDeliveryMethod.Backpack;
						}
						else
						{
							_Owner.Class = PvPRewardClass.Custom;
							_Owner.DeliveryMethod = PvPRewardDeliveryMethod.Custom;
						}
					}
					else
					{
						_Owner.Class = PvPRewardClass.None;
						_Owner.DeliveryMethod = PvPRewardDeliveryMethod.None;
					}
				}
			}

			public InternalTypeSelect(PvPReward owner, GenericReader reader)
				: base(reader)
			{
				_Owner = owner;
			}

			public InternalTypeSelect(PvPReward owner, string type)
				: base(type)
			{
				_Owner = owner;
			}

			public override void Serialize(GenericWriter writer)
			{
				base.Serialize(writer);

				writer.SetVersion(0);
			}

			public override void Deserialize(GenericReader reader)
			{
				base.Deserialize(reader);

				reader.GetVersion();
			}
		}

		private InternalTypeSelect _InternalTypeSelect;

		private PvPRewardDeliveryMethod _DeliveryMethod = PvPRewardDeliveryMethod.Backpack;

		[CommandProperty(AutoPvP.Access)]
		public virtual bool Enabled { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int Amount { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual string TypeName
		{
			get => _InternalTypeSelect.TypeName;
			set => _InternalTypeSelect.TypeName = value;
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPRewardDeliveryMethod DeliveryMethod
		{
			get => _DeliveryMethod;
			set
			{
				if (value != PvPRewardDeliveryMethod.None && Class == PvPRewardClass.Custom)
				{
					value = PvPRewardDeliveryMethod.Custom;
				}

				_DeliveryMethod = value;
			}
		}

		[CommandProperty(AutoPvP.Access, true)]
		public PvPRewardClass Class { get; private set; }

		public PvPReward()
			: this(String.Empty)
		{ }

		public PvPReward(string type)
		{
			_InternalTypeSelect = new InternalTypeSelect(this, type);

			Enabled = false;
			Amount = 1;
			Class = PvPRewardClass.None;
			_DeliveryMethod = PvPRewardDeliveryMethod.None;
		}

		public PvPReward(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			_InternalTypeSelect.Clear();

			Enabled = false;
			Amount = 1;
			Class = PvPRewardClass.None;
			_DeliveryMethod = PvPRewardDeliveryMethod.None;
		}

		public override void Reset()
		{
			_InternalTypeSelect.Reset();

			Enabled = false;
			Amount = 1;
			Class = PvPRewardClass.None;
			_DeliveryMethod = PvPRewardDeliveryMethod.Backpack;
		}

		public virtual List<Item> GiveReward(PlayerMobile pm)
		{
			var items = new List<Item>();

			if (pm == null || pm.Deleted || !Enabled)
			{
				return items;
			}

			var amount = Amount;

			while (amount > 0)
			{
				var reward = _InternalTypeSelect.CreateInstance();

				if (reward == null)
				{
					return items;
				}

				items.Add(reward);

				if (reward.Stackable)
				{
					reward.Amount = Math.Min(60000, amount);
				}

				amount -= reward.Amount;
			}

			items.ForEach(
				item =>
				{
					switch (_DeliveryMethod)
					{
						case PvPRewardDeliveryMethod.Custom:
							break;
						case PvPRewardDeliveryMethod.Bank:
							item.GiveTo(pm, GiveFlags.Bank | GiveFlags.Delete);
							break;
						case PvPRewardDeliveryMethod.Backpack:
							item.GiveTo(pm, GiveFlags.Pack | GiveFlags.Delete);
							break;
						default:
							item.GiveTo(pm, GiveFlags.PackBankDelete);
							break;
					}
				});

			items.RemoveAll(i => i == null || i.Deleted);

			return items;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					_InternalTypeSelect.Serialize(writer);

					writer.Write(Amount);
					writer.Write(Enabled);
					writer.WriteFlag(Class);
					writer.WriteFlag(_DeliveryMethod);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					_InternalTypeSelect = new InternalTypeSelect(this, reader);

					Amount = reader.ReadInt();
					Enabled = reader.ReadBool();
					Class = reader.ReadFlag<PvPRewardClass>();
					_DeliveryMethod = reader.ReadFlag<PvPRewardDeliveryMethod>();
				}
				break;
			}

			if (_InternalTypeSelect == null)
			{
				_InternalTypeSelect = new InternalTypeSelect(this, String.Empty);
			}

			if (Amount < 1)
			{
				Amount = 1;
			}
		}
	}
}