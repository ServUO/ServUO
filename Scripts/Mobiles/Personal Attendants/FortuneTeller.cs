using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    public class AttendantFortuneTeller : PersonalAttendant
    { 
        [Constructable]
        public AttendantFortuneTeller()
            : this(false)
        {
        }

        [Constructable]
        public AttendantFortuneTeller(bool female)
            : base("the Fortune Teller")
        {
            this.Female = female;
        }

        public AttendantFortuneTeller(Serial serial)
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
                list.Add(new AttendantUseEntry(this, 6245));

            base.AddCustomContextEntries(from, list);
        }

        public override void InitBody()
        {
            this.SetStr(50, 60);
            this.SetDex(20, 30);
            this.SetInt(100, 110);

            this.Name = NameList.RandomName("female");
            this.Female = true;
            this.Race = Race.Human;
            this.Hue = this.Race.RandomSkinHue();
			
            Utility.AssignRandomHair(this, Utility.RandomHairHue());
        }

        public override void InitOutfit()
        {
            this.AddItem(new Shoes(Utility.RandomPinkHue()));
            this.AddItem(new Shirt(Utility.RandomPinkHue()));
            this.AddItem(new SkullCap(Utility.RandomPinkHue()));

            if (Utility.RandomBool())
                this.AddItem(new Kilt(Utility.RandomPinkHue()));
            else
                this.AddItem(new Skirt(Utility.RandomPinkHue()));
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
            private readonly AttendantFortuneTeller m_Teller;
            public InternalGump(AttendantFortuneTeller teller)
                : base(200, 200)
            {
                this.m_Teller = teller;

                this.AddPage(0);

                this.AddBackground(0, 0, 291, 155, 0x13BE);
                this.AddImageTiled(5, 6, 280, 20, 0xA40);
                this.AddHtmlLocalized(9, 8, 280, 20, 1075994, 0x7FFF, false, false); // Fortune Teller
                this.AddImageTiled(5, 31, 280, 91, 0xA40);
                this.AddHtmlLocalized(9, 35, 280, 40, 1076025, 0x7FFF, false, false); // Ask your question
                this.AddImageTiled(9, 55, 270, 20, 0xDB0);
                this.AddImageTiled(10, 55, 270, 2, 0x23C5);
                this.AddImageTiled(9, 55, 2, 20, 0x23C3);
                this.AddImageTiled(9, 75, 270, 2, 0x23C5);
                this.AddImageTiled(279, 55, 2, 22, 0x23C3);

                this.AddTextEntry(12, 56, 263, 17, 0xA28, 15, "");

                this.AddButton(5, 129, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(40, 131, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL

                this.AddButton(190, 129, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(225, 131, 100, 20, 1006044, 0x7FFF, false, false); // OK
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Teller == null || this.m_Teller.Deleted)
                    return;

                if (info.ButtonID == 1)
                {
                    TextRelay text = info.GetTextEntry(15);

                    if (text != null && !String.IsNullOrEmpty(text.Text))
                    {
                        sender.Mobile.CloseGump(typeof(FortuneGump));
                        sender.Mobile.SendGump(new FortuneGump(text.Text));
                    }
                    else
                        sender.Mobile.SendGump(this);
                }
            }
        }

        private class FortuneGump : Gump
        { 
            public FortuneGump(string text)
                : base(200, 200)
            { 
                this.AddPage(0);

                this.AddImage(0, 0, 0x7724);

                int one, two, three;

                one = Utility.RandomMinMax(1, 19);
                two = Utility.RandomMinMax(one + 1, 20);
                three = Utility.RandomMinMax(0, one - 1);

                this.AddImageTiled(28, 140, 115, 180, 0x7725 + one);				
                this.AddTooltip(this.GetTooltip(one));
                this.AddHtmlLocalized(28, 115, 125, 20, 1076079, 0x7FFF, false, false); // The Past
                this.AddImageTiled(171, 140, 115, 180, 0x7725 + two);
                this.AddTooltip(this.GetTooltip(two));
                this.AddHtmlLocalized(171, 115, 125, 20, 1076081, 0x7FFF, false, false); // The Question
                this.AddImageTiled(314, 140, 115, 180, 0x7725 + three);
                this.AddTooltip(this.GetTooltip(three));
                this.AddHtmlLocalized(314, 115, 125, 20, 1076080, 0x7FFF, false, false); // The Future

                this.AddHtml(30, 32, 400, 25, text, true, false);
            }

            private int GetTooltip(int number)
            {
                if (number > 9)
                    return 1076015 + number - 10;

                switch ( number )
                {
                    case 0:
                        return 1076063;
                    case 1:
                        return 1076060;
                    case 2:
                        return 1076061;
                    case 3:
                        return 1076057;
                    case 4:
                        return 1076062;
                    case 5:
                        return 1076059;
                    case 6:
                        return 1076058;
                    case 7:
                        return 1076065;
                    case 8:
                        return 1076064;
                    case 9:
                        return 1076066;
                }

                return 1052009; // I have seen the error of my ways!
            }
        }
    }
}