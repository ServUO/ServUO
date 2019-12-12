using System;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public interface IPromotionalToken
    {
        TextDefinition ItemName { get; }
        Type GumpType { get; }
    }

	public abstract class PromotionalToken : Item, IPromotionalToken
	{
		public PromotionalToken()
			: base(0x2AAA)
		{
			LootType = LootType.Blessed;
			Light = LightType.Circle300;
			Weight = 5.0;
		}

		public PromotionalToken(Serial serial)
			: base(serial)
		{
		}

		public abstract TextDefinition ItemName { get; }
		public abstract TextDefinition ItemReceiveMessage { get; }
		public abstract TextDefinition ItemGumpName { get; }

        public Type GumpType { get { return typeof(PromotionalTokenGump); } }
        public virtual bool PlaceInBank { get { return true; } }

		public override int LabelNumber
		{
			get
			{
				return 1070997;
			}
		}// A promotional token
		public abstract Item CreateItemFor(Mobile from);

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1070998, ItemName.ToString()); // Use this to redeem<br>your ~1_PROMO~
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!IsChildOf(from.Backpack))
			{
				from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
			}
			else
			{
				from.CloseGump(typeof(PromotionalTokenGump));
				from.SendGump(new PromotionalTokenGump(this));
			}
		}

		public override void OnRemoved(object parent)
		{
			Mobile m = null;

			if (parent is Item)
				m = ((Item)parent).RootParent as Mobile;
			else if (parent is Mobile)
				m = (Mobile)parent;

			if (m != null)
				m.CloseGump(typeof(PromotionalTokenGump));
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}

		private class PromotionalTokenGump : Gump
		{
			private readonly PromotionalToken m_Token;
			public PromotionalTokenGump(PromotionalToken token)
				: base(10, 10)
			{
				m_Token = token;

				AddPage(0);

				AddBackground(0, 0, 240, 135, 0x2422);
				AddHtmlLocalized(15, 15, 210, 75, 1070972, 0x0, true, false); // Click "OKAY" to redeem the following promotional item:
				TextDefinition.AddHtmlText(this, 15, 60, 210, 75, m_Token.ItemGumpName, false, false);

				AddButton(160, 95, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0);	//Okay
				AddButton(90, 95, 0xF2, 0xF1, 0, GumpButtonType.Reply, 0);	//Cancel
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				if (info.ButtonID != 1)
					return;

				Mobile from = sender.Mobile;

				if (!m_Token.IsChildOf(from.Backpack))
				{
					from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
				}
				else
				{
					Item i = m_Token.CreateItemFor(from);

					if (i != null)
					{
                        if (m_Token.PlaceInBank || from.Backpack == null || !from.Backpack.TryDropItem(from, i, false))
                        {
                            from.BankBox.AddItem(i);
                        }

						TextDefinition.SendMessageTo(from, m_Token.ItemReceiveMessage);
						m_Token.Delete();
					}
				}
			}
		}
	}
}
