#region References
using System;

using Server.Accounting;
using Server.Network;
#endregion

namespace Server.Items
{
	public class BankBox : Container
	{
		private static readonly Type _GoldType = ScriptCompiler.FindTypeByFullName("Server.Items.Gold");
		private static readonly Type _CheckType = ScriptCompiler.FindTypeByFullName("Server.Items.BankCheck");

		public static bool SendDeleteOnClose { get; set; }

		private Mobile m_Owner;
		private bool m_Open;

		public override int DefaultMaxWeight => 0; 

		public override bool IsVirtualItem => true; 

		public Mobile Owner => m_Owner; 

		public bool Opened => m_Open; 

		public BankBox(Mobile owner)
			: base(0xE7C)
		{
			m_Owner = owner;

			Movable = false;
			Layer = Layer.Bank;
		}

		public BankBox(Serial serial)
			: base(serial)
		{ }
		
		public void Open()
		{
			if (m_Owner != null && m_Owner.NetState != null)
			{
			m_Open = true;

				m_Owner.PrivateOverheadMessage(
					MessageType.Regular,
					0x3B2,
					true,
					String.Format("Bank container has {0} items, {1} stones", TotalItems, TotalWeight),
					m_Owner.NetState);

				m_Owner.Send(new EquipUpdate(this));

				DisplayTo(m_Owner);
			}
		}

		public void Close()
		{
			m_Open = false;

			if (m_Owner != null && SendDeleteOnClose)
			{
				m_Owner.Send(RemovePacket);
			}
		}

		public override void OnDoubleClick(Mobile from)
		{ }

		public override DeathMoveResult OnParentDeath(Mobile parent)
		{
			return DeathMoveResult.RemainEquiped;
		}

		public override bool IsAccessibleTo(Mobile check)
		{
			if ((check == m_Owner && m_Open) || check.AccessLevel >= AccessLevel.GameMaster)
			{
				return base.IsAccessibleTo(check);
			}

			return false;
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if ((from == m_Owner && m_Open) || from.AccessLevel >= AccessLevel.GameMaster)
			{
				return base.OnDragDrop(from, dropped);
			}

			return false;
		}

		public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
		{
			if ((from == m_Owner && m_Open) || from.AccessLevel >= AccessLevel.GameMaster)
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

		public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
		{
			Type type = item.GetType();

			if (AccountGold.Enabled && Owner != null && Owner.Account != null && (type.IsAssignableFrom(_GoldType) || type.IsAssignableFrom(_CheckType)))
			{
				return true;
			}

			return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version

			writer.Write(m_Owner);
			writer.Write(m_Open);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();

			m_Owner = reader.ReadMobile();
			m_Open = reader.ReadBool();

			if (ItemID == 0xE41)
			{
				ItemID = 0xE7C;
			}

			if (m_Owner == null)
			{
				Delete();
			}
		}
	}
}
