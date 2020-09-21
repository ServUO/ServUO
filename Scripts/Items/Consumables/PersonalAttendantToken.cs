using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class PersonalAttendantToken : Item
    {
        [Constructable]
        public PersonalAttendantToken()
            : base(0x2AAA)
        {
            LootType = LootType.Blessed;
            Weight = 5.0;
        }

        public PersonalAttendantToken(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1070997;// A promotional token
        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1070998, string.Format("#{0}", 1075997));  // Use this to redeem<br>Personal Attendant
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        public class InternalGump : Gump
        {
            private readonly PersonalAttendantToken m_Token;
            public InternalGump(PersonalAttendantToken token)
                : base(200, 200)
            {
                m_Token = token;

                AddPage(0);

                AddBackground(0, 0, 291, 159, 0x13BE);
                AddImageTiled(5, 6, 280, 20, 0xA40);
                AddHtmlLocalized(9, 8, 280, 20, 1049004, 0x7FFF, false, false); // Confirm
                AddImageTiled(5, 31, 280, 100, 0xA40);
                AddHtmlLocalized(9, 35, 272, 100, 1076052, 0x7FFF, false, false); // Clicking "OK" will create a Personal Attendant contract bound to you. You will not be able to trade it to another player, and only you will be able to use it.

                AddButton(190, 133, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(225, 135, 90, 20, 1006044, 0x7FFF, false, false); // OK

                AddButton(5, 133, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40, 135, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Token == null || m_Token.Deleted)
                    return;

                if (info.ButtonID == 1)
                {
                    sender.Mobile.AddToBackpack(new PersonalAttendantDeed(sender.Mobile));
                    m_Token.Delete();
                }
            }
        }
    }
}