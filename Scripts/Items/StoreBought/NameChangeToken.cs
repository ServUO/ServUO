using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using System;

namespace Server.Items
{
    public class NameChangeToken : Item, IPromotionalToken
    {
        public override int LabelNumber => 1070997;  // a promotional token
        public TextDefinition ItemName => 1075247;  // name change

        public Type GumpType => typeof(NameChangeConfirmGump);

        [Constructable]
        public NameChangeToken()
            : base(0x2AAA)
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
            else if (from is PlayerMobile)
            {
                from.CloseGump(typeof(NameChangeConfirmGump));
                BaseGump.SendGump(new NameChangeConfirmGump((PlayerMobile)from, this));
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1070998, ItemName.ToString()); // Use this to redeem<br>Your ~1_PROMO~ : name change
        }

        public NameChangeToken(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class NameChangeConfirmGump : BaseGump
    {
        public NameChangeToken Token { get; private set; }

        public NameChangeConfirmGump(PlayerMobile pm, NameChangeToken token)
            : base(pm, 100, 100)
        {
            Token = token;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 291, 179, 9200);
            AddImageTiled(5, 5, 281, 20, 2702);
            AddImageTiled(5, 30, 281, 120, 2702);

            AddHtmlLocalized(8, 5, 279, 20, 1075241, 0x7FFF, false, false); // Change your character's name

            AddHtmlLocalized(8, 30, 279, 16, 1075242, 0x7FFF, false, false); // Enter your new name (16 characters max, English characters only):
            AddImageTiled(8, 50, 276, 20, 0xDB0);
            AddTextEntry(9, 50, 275, 20, 0, 0, "");
            AddHtmlLocalized(8, 70, 279, 80, 1075561, 0x7FFF, false, false); // Clicking OK will permanently change your character's name. Reversing this requires the purchase of an additional name change token. For more details, <A HREF="http://store2.origin.com/store/ea/en_US/DisplayProductDetailsPage/productID.244034900">visit our web site</A>.

            AddButton(5, 152, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 152, 100, 20, 1011012, 0x7FFF, false, false); // CANCEL

            AddButton(126, 152, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(160, 152, 120, 20, 1075243, 0x7FFF, false, false); // Change my name!
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1 && Token.IsChildOf(User.Backpack))
            {
                TextRelay relay = info.GetTextEntry(0);

                if (relay != null)
                {
                    string text = relay.Text;

                    if (string.IsNullOrEmpty(text))
                    {
                        User.SendLocalizedMessage(1075245); // Your name cannot be blank.
                    }
                    else if (text.Length > 16)
                    {
                        User.SendLocalizedMessage(1075244); // That name is too long.
                    }
                    else if (!NameVerification.Validate(text, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote))
                    {
                        User.SendLocalizedMessage(1075246); // That name is not valid.
                    }
                    else
                    {
                        User.Name = text;
                        Token.Delete();

                        User.SendMessage("You have successfully changed your name."); // TODO: Message?
                    }
                }
            }
        }
    }
}
