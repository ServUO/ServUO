using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;

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
            if (from.Alive && this.IsOwner(from))
            {
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
            }
            else
                base.OnDoubleClick(from);
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive && this.IsOwner(from))
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
                this.m_Dealer = dealer;

                this.AddHtmlLocalized(14, 12, 273, 20, 1075995, 0x7FFF, false, false); // Lucky Dealer

                this.AddPage(0);

                this.AddBackground(0, 0, 273, 324, 0x13BE);
                this.AddImageTiled(10, 10, 253, 20, 0xA40);
                this.AddImageTiled(10, 40, 253, 244, 0xA40);
                this.AddImageTiled(10, 294, 253, 20, 0xA40);
                this.AddAlphaRegion(10, 10, 253, 304);
                this.AddButton(10, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(45, 294, 80, 20, 1060051, 0x7FFF, false, false); // CANCEL
                this.AddButton(130, 294, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(165, 294, 80, 20, 1076002, 0x7FFF, false, false); // Roll

                this.AddHtmlLocalized(14, 50, 120, 20, 1076000, 0x7FFF, false, false); // Number of dice
                this.AddGroup(0);
                this.AddRadio(14, 70, 0xD2, 0xD3, dice == 1, 1001);
                this.AddLabel(44, 70, 0x481, "1");
                this.AddRadio(14, 100, 0xD2, 0xD3, dice == 2, 1002);
                this.AddLabel(44, 100, 0x481, "2");
                this.AddRadio(14, 130, 0xD2, 0xD3, dice == 3, 1003);
                this.AddLabel(44, 130, 0x481, "3");
                this.AddRadio(14, 160, 0xD2, 0xD3, dice == 4, 1004);
                this.AddLabel(44, 160, 0x481, "4");

                this.AddHtmlLocalized(130, 50, 120, 20, 1076001, 0x7FFF, false, false); // Number of faces
                this.AddGroup(1);
                this.AddRadio(130, 70, 0xD2, 0xD3, faces == 4, 4);
                this.AddLabel(160, 70, 0x481, "4"); 
                this.AddRadio(130, 100, 0xD2, 0xD3, faces == 6, 6);
                this.AddLabel(160, 100, 0x481, "6");
                this.AddRadio(130, 130, 0xD2, 0xD3, faces == 8, 8);
                this.AddLabel(160, 130, 0x481, "8");
                this.AddRadio(130, 160, 0xD2, 0xD3, faces == 10, 10);
                this.AddLabel(160, 160, 0x481, "10");
                this.AddRadio(130, 190, 0xD2, 0xD3, faces == 12, 12);
                this.AddLabel(160, 190, 0x481, "12");
                this.AddRadio(130, 220, 0xD2, 0xD3, faces == 20, 20);
                this.AddLabel(160, 220, 0x481, "20");
                this.AddRadio(130, 250, 0xD2, 0xD3, faces == 100, 100);
                this.AddLabel(160, 250, 0x481, "100");
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Dealer == null || this.m_Dealer.Deleted)
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

                    if (this.m_Dealer.m_NextUse < DateTime.UtcNow)
                    {
                        if (dice > 0 && faces > 0)
                        {
                            int sum = 0;
                            string text = String.Empty;
							
                            for (int i = 0; i < dice; i++)
                            {
                                int roll = Utility.Random(faces) + 1;
                                text = String.Format("{0}{1}{2}", text, i > 0 ? " " : "", roll);
                                sum += roll;
                            }

                            this.m_Dealer.Say(1076071, String.Format("{0}\t{1}\t{2}\t{3}\t{4}", sender.Mobile.Name, dice, faces, text, sum)); // ~1_NAME~ rolls ~2_DICE~d~3_FACES~: ~4_ROLLS~ (Total: ~5_TOTAL~)
                        }

                        if (this.m_Dealer.m_Count > 0 && DateTime.UtcNow - this.m_Dealer.m_NextUse < TimeSpan.FromSeconds(this.m_Dealer.m_Count))
                            this.m_Dealer.m_NextUse = DateTime.UtcNow + TimeSpan.FromSeconds(3);

                        if (this.m_Dealer.m_Count++ == 5)
                        {
                            this.m_Dealer.m_NextUse = DateTime.UtcNow;
                            this.m_Dealer.m_Count = 0;
                        }
                    }
                    else
                        sender.Mobile.SendLocalizedMessage(501789); // You must wait before trying again.

                    sender.Mobile.SendGump(new InternalGump(this.m_Dealer, dice, faces));
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
            this.SetStr(50, 60);
            this.SetDex(20, 30);
            this.SetInt(100, 110);

            this.Name = NameList.RandomName("male");
            this.Female = false;
            this.Race = Race.Human;
            this.Hue = this.Race.RandomSkinHue();

            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            this.AddItem(new Boots());
            this.AddItem(new ShortPants());
            this.AddItem(new JesterHat());
            this.AddItem(new JesterSuit());
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
            this.SetStr(50, 60);
            this.SetDex(20, 30);
            this.SetInt(100, 110);

            this.Name = NameList.RandomName("female");
            this.Female = true;
            this.Race = Race.Elf;
            this.Hue = this.Race.RandomSkinHue();

            this.HairItemID = this.Race.RandomHair(this.Female);
            this.HairHue = this.Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots());
            this.AddItem(new ElvenPants());
            this.AddItem(new ElvenShirt());
            this.AddItem(new JesterHat());
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