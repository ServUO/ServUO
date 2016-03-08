using System;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public abstract class PromotionalToken : Item
	{
		public PromotionalToken()
			: base(0x2AAA)
		{
			this.LootType = LootType.Blessed;
			this.Light = LightType.Circle300;
			this.Weight = 5.0;
		}

		public PromotionalToken(Serial serial)
			: base(serial)
		{
		}

		public abstract TextDefinition ItemName { get; }
		public abstract TextDefinition ItemReceiveMessage { get; }
		public abstract TextDefinition ItemGumpName { get; }
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

			list.Add(1070998, this.ItemName.ToString()); // Use this to redeem<br>your ~1_PROMO~
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!this.IsChildOf(from.Backpack))
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
				this.m_Token = token;

				this.AddPage(0);

				this.AddBackground(0, 0, 240, 135, 0x2422);
				this.AddHtmlLocalized(15, 15, 210, 75, 1070972, 0x0, true, false); // Click "OKAY" to redeem the following promotional item:
				TextDefinition.AddHtmlText(this, 15, 60, 210, 75, this.m_Token.ItemGumpName, false, false);

				this.AddButton(160, 95, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0);	//Okay
				this.AddButton(90, 95, 0xF2, 0xF1, 0, GumpButtonType.Reply, 0);	//Cancel
			}

			public override void OnResponse(NetState sender, RelayInfo info)
			{
				if (info.ButtonID != 1)
					return;

				Mobile from = sender.Mobile;

				if (!this.m_Token.IsChildOf(from.Backpack))
				{
					from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
				}
				else
				{
					Item i = this.m_Token.CreateItemFor(from);

					if (i != null)
					{
						from.BankBox.AddItem(i);
						TextDefinition.SendMessageTo(from, this.m_Token.ItemReceiveMessage);
						this.m_Token.Delete();
					}
				}
			}
		}
	}

	public class SoulstoneFragmentToken : PromotionalToken
	{
		[Constructable]
		public SoulstoneFragmentToken()
			: base()
		{
		}

		public SoulstoneFragmentToken(Serial serial)
			: base(serial)
		{
		}

		public override TextDefinition ItemGumpName
		{
			get
			{
				return 1070999;
			}
		}// <center>Soulstone Fragment</center>
		public override TextDefinition ItemName
		{
			get
			{
				return 1071000;
			}
		}//soulstone fragment
		public override TextDefinition ItemReceiveMessage
		{
			get
			{
				return 1070976;
			}
		}// A soulstone fragment has been created in your bank box.
		public override Item CreateItemFor(Mobile from)
		{
			if (from != null && from.Account != null)

				return new SoulstoneFragment(from.Account.ToString());
			else
				return null;
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
	}

	public class SoulstoneToken : PromotionalToken
	{
		[Constructable]
		public SoulstoneToken()
			: base()
		{
		}

		public SoulstoneToken(Serial serial)
			: base(serial)
		{
		}

		public override TextDefinition ItemGumpName
		{
			get
			{
				return 1030903;
			}
		}// <center>Soulstone</center>
		public override TextDefinition ItemName
		{
			get
			{
				return 1030899;
			}
		}//soulstone
		public override TextDefinition ItemReceiveMessage
		{
			get
			{
				return 1070743;
			}
		}// A soulstone has been created in your bank box.
		public override Item CreateItemFor(Mobile from)
		{
			if (from != null && from.Account != null)
			{
				return new SoulStone(from.Account.ToString()) { LastUserName = @from.RawName };
			}
			else
				return null;
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
	}

}
