using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class AttendantLuckyDealer : PersonalAttendant
    {
        private DateTime m_NextUse;
        private int m_Count;
        public AttendantLuckyDealer()
            : base("the Lucky Dealer")
        {
        }

        public AttendantLuckyDealer(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Alive && IsOwner(from))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
                base.OnDoubleClick(from);
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive && IsOwner(from))
                list.Add(new AttendantUseEntry(this, 6244));

            base.AddCustomContextEntries(from, list);
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

        private class InternalGump : Gump
        {
            private readonly AttendantLuckyDealer m_Dealer;
            public InternalGump(AttendantLuckyDealer dealer)
                : this(dealer, 1, 4)
            {
            }

            public InternalGump(AttendantLuckyDealer dealer, int dice, int faces)
                : base(60, 36)
            {
                m_Dealer = dealer;

                AddHtmlLocalized(14, 12, 273, 20, 1075995, 0x7FFF, false, false); // Lucky Dealer

                AddPage(0);

                AddBackground(0, 0, 273, 324, 0x13BE);
                AddImageTiled(10, 10, 253, 20, 0xA40);
                AddImageTiled(10, 40, 253, 244, 0xA40);
                AddImageTiled(10, 294, 253, 20, 0xA40);
                AddAlphaRegion(10, 10, 253, 304);
                AddButton(10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 294, 80, 20, 1060051, 0x7FFF, false, false); // CANCEL
                AddButton(130, 294, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(165, 294, 80, 20, 1076002, 0x7FFF, false, false); // Roll

                AddHtmlLocalized(14, 50, 120, 20, 1076000, 0x7FFF, false, false); // Number of dice
                AddGroup(0);
                AddRadio(14, 70, 0xD2, 0xD3, dice == 1, 1001);
                AddLabel(44, 70, 0x481, "1");
                AddRadio(14, 100, 0xD2, 0xD3, dice == 2, 1002);
                AddLabel(44, 100, 0x481, "2");
                AddRadio(14, 130, 0xD2, 0xD3, dice == 3, 1003);
                AddLabel(44, 130, 0x481, "3");
                AddRadio(14, 160, 0xD2, 0xD3, dice == 4, 1004);
                AddLabel(44, 160, 0x481, "4");

                AddHtmlLocalized(130, 50, 120, 20, 1076001, 0x7FFF, false, false); // Number of faces
                AddGroup(1);
                AddRadio(130, 70, 0xD2, 0xD3, faces == 4, 4);
                AddLabel(160, 70, 0x481, "4");
                AddRadio(130, 100, 0xD2, 0xD3, faces == 6, 6);
                AddLabel(160, 100, 0x481, "6");
                AddRadio(130, 130, 0xD2, 0xD3, faces == 8, 8);
                AddLabel(160, 130, 0x481, "8");
                AddRadio(130, 160, 0xD2, 0xD3, faces == 10, 10);
                AddLabel(160, 160, 0x481, "10");
                AddRadio(130, 190, 0xD2, 0xD3, faces == 12, 12);
                AddLabel(160, 190, 0x481, "12");
                AddRadio(130, 220, 0xD2, 0xD3, faces == 20, 20);
                AddLabel(160, 220, 0x481, "20");
                AddRadio(130, 250, 0xD2, 0xD3, faces == 100, 100);
                AddLabel(160, 250, 0x481, "100");
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Dealer == null || m_Dealer.Deleted)
                    return;

                if (info.ButtonID == 1)
                {
                    int dice = 1;
                    int faces = 4;

                    if (info.Switches.Length == 2)
                    {
                        dice = info.Switches[0] - 1000;
                        faces = info.Switches[1];
                    }

                    if (m_Dealer.m_NextUse < DateTime.UtcNow)
                    {
                        if (dice > 0 && faces > 0)
                        {
                            int sum = 0;
                            string text = string.Empty;

                            for (int i = 0; i < dice; i++)
                            {
                                int roll = Utility.Random(faces) + 1;
                                text = string.Format("{0}{1}{2}", text, i > 0 ? " " : "", roll);
                                sum += roll;
                            }

                            m_Dealer.Say(1076071, string.Format("{0}\t{1}\t{2}\t{3}\t{4}", sender.Mobile.Name, dice, faces, text, sum)); // ~1_NAME~ rolls ~2_DICE~d~3_FACES~: ~4_ROLLS~ (Total: ~5_TOTAL~)
                        }

                        if (m_Dealer.m_Count > 0 && DateTime.UtcNow - m_Dealer.m_NextUse < TimeSpan.FromSeconds(m_Dealer.m_Count))
                            m_Dealer.m_NextUse = DateTime.UtcNow + TimeSpan.FromSeconds(3);

                        if (m_Dealer.m_Count++ == 5)
                        {
                            m_Dealer.m_NextUse = DateTime.UtcNow;
                            m_Dealer.m_Count = 0;
                        }
                    }
                    else
                        sender.Mobile.SendLocalizedMessage(501789); // You must wait before trying again.

                    sender.Mobile.SendGump(new InternalGump(m_Dealer, dice, faces));
                }
            }
        }
    }

    public class AttendantMaleLuckyDealer : AttendantLuckyDealer
    {
        [Constructable]
        public AttendantMaleLuckyDealer()
            : base()
        {
        }

        public AttendantMaleLuckyDealer(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            SetStr(50, 60);
            SetDex(20, 30);
            SetInt(100, 110);

            Name = NameList.RandomName("male");
            Female = false;
            Race = Race.Human;
            Hue = Race.RandomSkinHue();

            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            AddItem(new Boots());
            AddItem(new ShortPants());
            AddItem(new JesterHat());
            AddItem(new JesterSuit());
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
    }

    public class AttendantFemaleLuckyDealer : AttendantLuckyDealer
    {
        [Constructable]
        public AttendantFemaleLuckyDealer()
            : base()
        {
        }

        public AttendantFemaleLuckyDealer(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            SetStr(50, 60);
            SetDex(20, 30);
            SetInt(100, 110);

            Name = NameList.RandomName("female");
            Female = true;
            Race = Race.Elf;
            Hue = Race.RandomSkinHue();

            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots());
            AddItem(new ElvenPants());
            AddItem(new ElvenShirt());
            AddItem(new JesterHat());
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
    }
}