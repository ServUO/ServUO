#region References
using System;

using Server.Accounting;
using Server.Network;
#endregion

namespace Server.Items
{
	public class BankBox : Container
	{
		public static bool SendDeleteOnClose { get; set; }

		private static Type _GoldType;
		private static Type _CheckType;

		public static bool IsGoldType(Item item)
		{
			if (item == null)
			{
				return false;
			}

			if (_GoldType == null)
			{
				_GoldType = ScriptCompiler.FindTypeByFullName("Server.Items.Gold");
			}

			if (_CheckType == null)
			{
				_CheckType = ScriptCompiler.FindTypeByFullName("Server.Items.BankCheck");
			}

			var type = item.GetType();

			return (_GoldType != null && type.IsAssignableFrom(_GoldType)) || (_CheckType != null && type.IsAssignableFrom(_CheckType));
		}

		public Mobile Owner { get; private set; }

		public bool Opened { get; private set; }

		public override int DefaultMaxWeight => 0;

		public override bool IsVirtualItem => true;

		public BankBox(Mobile owner)
			: base(0xE7C)
		{
			Owner = owner;

			Movable = false;
			Layer = Layer.Bank;
		}

		public BankBox(Serial serial)
			: base(serial)
		{ }

		public void Open()
		{
			if (Owner != null && Owner.NetState != null)
			{
				Opened = true;

				Owner.PrivateOverheadMessage(MessageType.Regular, 0x3B2, true, String.Format("Bank container has {0} items, {1} stones", TotalItems, TotalWeight), Owner.NetState);

				Owner.Send(new EquipUpdate(this));

				DisplayTo(Owner);
			}
		}

		public void Close()
		{
			Opened = false;

			if (Owner != null && SendDeleteOnClose)
			{
				Owner.Send(RemovePacket);
			}
		}

		public override void OnSingleClick(Mobile from)
		{ }

		public override void OnDoubleClick(Mobile from)
		{ }

		public override DeathMoveResult OnParentDeath(Mobile parent)
		{
			return DeathMoveResult.RemainEquiped;
		}

		public override bool IsAccessibleTo(Mobile check)
		{
			if ((check == Owner && Opened) || check.AccessLevel >= AccessLevel.GameMaster)
			{
				return base.IsAccessibleTo(check);
			}

			return false;
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if ((from == Owner && Opened) || from.AccessLevel >= AccessLevel.GameMaster)
			{
				return base.OnDragDrop(from, dropped);
			}

			return false;
		}

		public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
		{
			if ((from == Owner && Opened) || from.AccessLevel >= AccessLevel.GameMaster)
			{
				return base.OnDragDropInto(from, item, p);
			}

			return false;
		}

		public override int GetTotal(TotalType type)
		{
			if (AccountGold.Enabled && Owner != null && Owner.Account != null && type == TotalType.Gold)
			{
				return Owner.Account.TotalGold;
			}

			return base.GetTotal(type);
		}

		public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, bool checkWeight, int plusItems, int plusWeight)
		{
			if (AccountGold.Enabled && AccountGold.ConvertOnBank && IsGoldType(item))
			{
				return true;
			}

			return base.CheckHold(m, item, message, checkItems, checkWeight, plusItems, plusWeight);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(Owner);
			writer.Write(Opened);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();

			Owner = reader.ReadMobile();
			Opened = reader.ReadBool();

			if (ItemID == 0xE41)
			{
				ItemID = 0xE7C;
			}

			if (Owner == null)
			{
				Delete();
			}
		}
	}
}
