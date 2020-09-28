using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public enum AnniversaryType
    {
        ShadowItems,
        CrystalItems
    }

    public class AnniversaryPromotionalToken : Item
    {
        public override int LabelNumber
        {
            get
            {
                int cliloc = 0;

                switch (Type)
                {
                    case AnniversaryType.ShadowItems:
                        cliloc = 1076594; // A Shadow Token
                        break;
                    case AnniversaryType.CrystalItems:
                        cliloc = 1076592; // A Crystal Token
                        break;
                }

                return cliloc;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AnniversaryType Type { get; set; }

        [Constructable]
        public AnniversaryPromotionalToken(AnniversaryType type)
            : base(0x2AAA)
        {
            Type = type;

            if (type == AnniversaryType.CrystalItems)
                ItemID = 0x3678;
            else if (type == AnniversaryType.ShadowItems)
                ItemID = 0x3679;

            LootType = LootType.Blessed;
            Light = LightType.Circle300;
            Weight = 5.0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int cliloc = 0;

            switch (Type)
            {
                case AnniversaryType.ShadowItems:
                    cliloc = 1076717;
                    break;
                case AnniversaryType.CrystalItems:
                    cliloc = 1076716;
                    break;
            }

            list.Add(1070998, string.Format("#{0}", cliloc)); // Use this to redeem<br>your ~1_PROMO~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
            else
            {
                from.CloseGump(typeof(AnniversaryRewardGump));
                from.SendGump(new AnniversaryRewardGump(this));
            }
        }

        public AnniversaryPromotionalToken(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Type = (AnniversaryType)reader.ReadInt();
        }

        private class AnniversaryRewardGump : Gump
        {
            private readonly AnniversaryPromotionalToken Token;

            public AnniversaryRewardGump(AnniversaryPromotionalToken token)
                : base(200, 200)
            {
                Token = token;

                AddHtmlLocalized(0, 0, 0, 0, 1015313, false, false); // <center></center>
                AddHtmlLocalized(0, 0, 0, 0, 1049004, false, false); // Confirm
                AddHtmlLocalized(0, 0, 0, 0, 1076597, false, false); // Clicking "OK" will create the items in your backpack if there is room.  Otherwise it will be created in your bankbox.
                AddHtmlLocalized(0, 0, 0, 0, 1011036, false, false); // OKAY
                AddHtmlLocalized(0, 0, 0, 0, 1011012, false, false); // CANCEL

                AddPage(0);

                AddBackground(0, 0, 291, 159, 0x13BE);

                AddImageTiled(5, 6, 280, 20, 0xA40);
                AddHtmlLocalized(9, 8, 280, 20, 1049004, 0x7FFF, false, false); // Confirm

                AddImageTiled(5, 31, 280, 100, 0xA40);
                AddHtmlLocalized(9, 35, 272, 100, 1076597, 0x7FFF, false, false); // Clicking "OK" will create the items in your backpack if there is room.  Otherwise it will be created in your bankbox.

                AddButton(190, 133, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(225, 135, 90, 20, 1006044, 0x7FFF, false, false); // OK

                AddButton(5, 133, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40, 135, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 1)
                {
                    Mobile m = sender.Mobile;
                    Item item = null;

                    switch (Token.Type)
                    {
                        case AnniversaryType.CrystalItems:
                            {
                                item = new BoxOfCrystalItems();
                                break;
                            }
                        case AnniversaryType.ShadowItems:
                            {
                                item = new BoxOfShadowItems();
                                break;
                            }
                    }

                    if (item != null && Token != null)
                    {
                        if (!m.AddToBackpack(item))
                        {
                            if (m.BankBox.TryDropItem(m, item, false))
                                item.MoveToWorld(m.Location, m.Map);
                        }

                        Token.Delete();
                    }
                }
            }
        }
    }
}
