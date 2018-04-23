using System;
using Server;

namespace Knives.Chat3
{
    public class MailGump : GumpPlus
    {
        private Mobile c_Target;

        public Mobile Current { get { return (c_Target == null ? Owner : c_Target); } }

        public MailGump(Mobile m, Mobile targ)
            : base(m, 100, 100)
        {
            c_Target = targ;
        }

        public MailGump(Mobile m)
            : this(m, null)
        {
        }

        protected override void BuildGump()
        {
            int width = 300;
            int y = 10;

            AddHtml(0, y, width, "<CENTER>" + General.Local(217));
            AddImage(width / 2 - 100, y + 2, 0x39);
            AddImage(width / 2 + 70, y + 2, 0x3B);

            if (c_Target != null)
                AddHtml(0, y += 25, width, "<CENTER>" + General.Local(224) + " " + c_Target.RawName);

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(58));
            AddButton(width / 2 - 120, y, Data.GetData(Current).SevenDays ? 0x2343 : 0x2342, "Seven Days", new GumpCallback(SevenDays));
            AddButton(width / 2 + 100, y, Data.GetData(Current).SevenDays ? 0x2343 : 0x2342, "Seven Days", new GumpCallback(SevenDays));
            AddHtml(0, y += 20, width, "<CENTER>" + General.Local(259));
            AddButton(width / 2 - 120, y, Data.GetData(Current).WhenFull ? 0x2343 : 0x2342, "When Full", new GumpCallback(WhenFull));
            AddButton(width / 2 + 100, y, Data.GetData(Current).WhenFull ? 0x2343 : 0x2342, "When Full", new GumpCallback(WhenFull));
            AddHtml(0, y += 20, width, "<CENTER>" + General.Local(24));
            AddButton(width / 2 - 120, y, Data.GetData(Current).FriendsOnly ? 0x2343 : 0x2342, "Friends Only", new GumpCallback(FriendsOnly));
            AddButton(width / 2 + 100, y, Data.GetData(Current).FriendsOnly ? 0x2343 : 0x2342, "Friends Only", new GumpCallback(FriendsOnly));
            AddHtml(0, y += 20, width, "<CENTER>" + General.Local(25));
            AddButton(width / 2 - 120, y, Data.GetData(Current).ByRequest ? 0x2343 : 0x2342, "Friend Request", new GumpCallback(FriendRequest));
            AddButton(width / 2 + 100, y, Data.GetData(Current).ByRequest ? 0x2343 : 0x2342, "Friend Request", new GumpCallback(FriendRequest));
            AddHtml(0, y += 20, width, "<CENTER>" + General.Local(30));
            AddButton(width / 2 - 120, y, Data.GetData(Current).FriendAlert ? 0x2343 : 0x2342, "Friend Alert", new GumpCallback(FriendAlert));
            AddButton(width / 2 + 100, y, Data.GetData(Current).FriendAlert ? 0x2343 : 0x2342, "Friend Alert", new GumpCallback(FriendAlert));
            AddHtml(0, y += 20, width, "<CENTER>" + General.Local(196));
            AddButton(width / 2 - 120, y, Data.GetData(Current).ReadReceipt ? 0x2343 : 0x2342, "Read Receipt", new GumpCallback(ReadReceipt));
            AddButton(width / 2 + 100, y, Data.GetData(Current).ReadReceipt ? 0x2343 : 0x2342, "Read Receipt", new GumpCallback(ReadReceipt));

            AddHtml(0, y += 25, width, "<CENTER>" + General.Local(27));
            AddButton(width / 2 - 120, y, Data.GetData(Current).MsgSound ? 0x2343 : 0x2342, "Message Sound", new GumpCallback(MessageSound));
            AddButton(width / 2 + 100, y, Data.GetData(Current).MsgSound ? 0x2343 : 0x2342, "Message Sound", new GumpCallback(MessageSound));

            if (Data.GetData(Current).MsgSound)
            {
                AddHtml(0, y += 30, width, "<CENTER>" + General.Local(28));
                AddTextField(width/2-25, y, 50, 21, 0x480, 0xBBA, "Sound", Data.GetData(Current).DefaultSound.ToString());
                AddButton(width/2+30, y + 3, 0x15E1, 0x15E5, "Play Sound", new GumpCallback(PlaySound));
                AddButton(width/2-40, y, 0x983, "Sound Up", new GumpCallback(SoundUp));
                AddButton(width/2-40, y + 10, 0x985, "Sound Down", new GumpCallback(SoundDown));
            }

            AddBackgroundZero(0, 0, width, y + 40, Data.GetData(Current).DefaultBack);
        }

        private void SevenDays()
        {
            Data.GetData(Current).SevenDays = !Data.GetData(Current).SevenDays;
            NewGump();
        }

        private void WhenFull()
        {
            Data.GetData(Current).WhenFull = !Data.GetData(Current).WhenFull;
            NewGump();
        }

        private void FriendsOnly()
        {
            Data.GetData(Current).FriendsOnly = !Data.GetData(Current).FriendsOnly;
            NewGump();
        }

        private void FriendRequest()
        {
            Data.GetData(Current).ByRequest = !Data.GetData(Current).ByRequest;
            NewGump();
        }

        private void FriendAlert()
        {
            Data.GetData(Current).FriendAlert = !Data.GetData(Current).FriendAlert;
            NewGump();
        }

        private void ReadReceipt()
        {
            Data.GetData(Current).ReadReceipt = !Data.GetData(Current).ReadReceipt;
            NewGump();
        }

        private void MessageSound()
        {
            Data.GetData(Current).MsgSound = !Data.GetData(Current).MsgSound;
            NewGump();
        }

        private void PlaySound()
        {
            Data.GetData(Current).DefaultSound = GetTextFieldInt("Sound");
            Owner.SendSound(Data.GetData(Current).DefaultSound);
            NewGump();
        }

        private void SoundUp()
        {
            Data.GetData(Current).DefaultSound = Data.GetData(Current).DefaultSound + 1;
            NewGump();
        }

        private void SoundDown()
        {
            Data.GetData(Current).DefaultSound = Data.GetData(Current).DefaultSound - 1;
            NewGump();
        }
    }
}