using System;
using Server;
using Server.Gumps;

namespace Knives.Chat3
{
    public class ProfileGump : GumpPlus
    {
        private Mobile c_Target;
        
        public ProfileGump(Mobile m, Mobile targ)
            : base(m, 100, 100)
        {
            Override = true;

            c_Target = targ;
        }

        protected override void BuildGump()
        {
            int width = 300;
            int y = 10;

            AddImage(10, y, 0x589);
            Avatar av = Avatar.GetAvatar(c_Target);
            if (av.Id < 100000)
                AddImage(10 + av.X, y + av.Y, av.Id);
            else
                AddItem(10 + av.X, y + av.Y, av.Id - 100000);

            AddHtml(95, y, width-95, Server.Misc.Titles.ComputeTitle(Owner, c_Target));

            if (Owner.AccessLevel != AccessLevel.Player)
                AddHtml(95, y += 20, width - 95, "Access: " + c_Target.AccessLevel);
            else if (c_Target.AccessLevel != AccessLevel.Player)
                AddHtml(95, y += 20, width - 95, "" + c_Target.AccessLevel);
            else
            {
                if (c_Target.Guild != null)
                    AddHtml(95, y += 20, width - 95, "[" + c_Target.Guild.Abbreviation + "] " + c_Target.GuildTitle);
                if (General.IsInFaction(c_Target))
                    AddHtml(95, y += 20, width - 95, General.FactionName(c_Target) + " " + General.FactionTitle(c_Target));
            }

            if (y < 89)
                y = 89;

            if (Owner == c_Target)
            {
                AddButton(32, y, 0x2626, 0x2627, "Avatar Down", new GumpCallback(AvatarDown));
                AddButton(52, y, 0x2622, 0x2623, "Avatar Up", new GumpCallback(AvatarUp));
            }

            AddHtml(0, y+=20, width, "<CENTER>" + General.Local(253) + " " + Data.GetData(c_Target).Karma);

            if (Owner == c_Target)
            {
                AddHtml(20, y += 25, 100, General.Local(247));
                AddTextField(20, y+=25, width - 40, 65, 0x480, 0xBBC, "Signature", Data.GetData(c_Target).Signature);
                AddHtml(width - 125, y += 65, 50, General.Local(244));
                AddHtml(width - 65, y, 50, General.Local(245));
                AddButton(width - 140, y + 3, 0x2716, "Clear Sig", new GumpCallback(ClearSig));
                AddButton(width - 80, y + 3, 0x2716, "Submit Sig", new GumpCallback(SubmitSig));
            }
            else
            {
                AddHtml(20, y += 25, width - 40, 65, "'" + Data.GetData(c_Target).Signature + "'", false, false);
                y += 65;
            }

            if (Owner != c_Target)
                ViewOptions(width);

            AddBackgroundZero(0, 0, width, y+40, Data.GetData(c_Target).DefaultBack);
        }

        private void ViewOptions(int x)
        {
            int y = 10;
            int width = 150;

            AddHtml(x, y += 10, width, "<CENTER>" + General.Local(0));
            AddButton(x + width / 2 - 60, y, Data.GetData(Owner).Friends.Contains(c_Target) ? 0x2343 : 0x2342, "Friend", new GumpCallback(Friend));
            AddButton(x + width / 2 + 40, y, Data.GetData(Owner).Friends.Contains(c_Target) ? 0x2343 : 0x2342, "Friend", new GumpCallback(Friend));

            AddHtml(x, y += 20, width, "<CENTER>" + General.Local(2));
            AddButton(x + width / 2 - 60, y, Data.GetData(Owner).Ignores.Contains(c_Target) ? 0x2343 : 0x2342, "Ignore", new GumpCallback(Ignore));
            AddButton(x + width / 2 + 40, y, Data.GetData(Owner).Ignores.Contains(c_Target) ? 0x2343 : 0x2342, "Ignore", new GumpCallback(Ignore));

            if (Chat3.Message.CanMessage(Owner, c_Target))
            {
                AddHtml(x, y += 20, width, "<CENTER>" + General.Local(13));
                AddButton(x + width / 2 - 60, y + 3, 0x2716, "Send Message", new GumpCallback(Message));
                AddButton(x + width / 2 + 50, y + 3, 0x2716, "Send Message", new GumpCallback(Message));
            }

            if (Owner.AccessLevel >= AccessLevel.Administrator)
            {
                if (Owner.AccessLevel > c_Target.AccessLevel)
                {
                    AddHtml(x, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(14), false);
                    AddButton(x + width / 2 - 60, y + 3, 0x2716, "Become User", new GumpCallback(BecomeUser));
                    AddButton(x + width / 2 + 50, y + 3, 0x2716, "Become User", new GumpCallback(BecomeUser));
                }

                if (c_Target.AccessLevel < AccessLevel.Administrator && c_Target.AccessLevel != AccessLevel.Player)
                {
                    AddHtml(x, y += 20, width, HTML.LightPurple + "<CENTER>" + General.Local(4), false);
                    AddButton(x + width / 2 - 60, y, Data.GetData(c_Target).GlobalAccess ? 0x2343 : 0x2342, "Global Access", new GumpCallback(GlobalAccess));
                    AddButton(x + width / 2 + 40, y, Data.GetData(c_Target).GlobalAccess ? 0x2343 : 0x2342, "Global Access", new GumpCallback(GlobalAccess));
                }
            }

            if (Owner.AccessLevel >= AccessLevel.GameMaster && c_Target.AccessLevel == AccessLevel.Player)
            {
                AddHtml(x, y += 20, width, HTML.Red + "<CENTER>" + General.Local(6), false);
                AddButton(x + width / 2 - 60, y, Data.GetData(c_Target).Banned ? 0x2343 : 0x2342, "Ban", new GumpCallback(Ban));
                AddButton(x + width / 2 + 40, y, Data.GetData(c_Target).Banned ? 0x2343 : 0x2342, "Ban", new GumpCallback(Ban));
            }

            if (Data.GetData(Owner).GlobalAccess)
            {
                y += 20;

                if (Data.GetData(Owner).Global)
                {
                    AddHtml(x, y += 20, width, HTML.Red + "<CENTER>" + General.Local(8), false);
                    AddButton(x + width / 2 - 60, y, Data.GetData(Owner).GIgnores.Contains(c_Target) ? 0x2343 : 0x2342, "Global Ignore", new GumpCallback(GIgnore));
                    AddButton(x + width / 2 + 40, y, Data.GetData(Owner).GIgnores.Contains(c_Target) ? 0x2343 : 0x2342, "Global Ignore", new GumpCallback(GIgnore));
                }
                else
                {
                    AddHtml(x, y += 20, width, HTML.Red + "<CENTER>" + General.Local(10), false);
                    AddButton(x + width / 2 - 60, y, Data.GetData(Owner).GListens.Contains(c_Target) ? 0x2343 : 0x2342, "Global Listen", new GumpCallback(GListen));
                    AddButton(x + width / 2 + 40, y, Data.GetData(Owner).GListens.Contains(c_Target) ? 0x2343 : 0x2342, "Global Listen", new GumpCallback(GListen));
                }
            }

            if (Owner.AccessLevel >= AccessLevel.GameMaster && c_Target.NetState != null)
            {
                AddHtml(x, y += 20, width, HTML.Red + "<CENTER>" + General.Local(15), false);
                AddButton(x + width / 2 - 60, y + 3, 0x2716, "Client", new GumpCallback(Client));
                AddButton(x + width / 2 + 50, y + 3, 0x2716, "Client", new GumpCallback(Client));

                AddHtml(x, y += 20, width, HTML.Red + "<CENTER>" + General.Local(16), false);
                AddButton(x + width / 2 - 60, y + 3, 0x2716, "Goto", new GumpCallback(Goto));
                AddButton(x + width / 2 + 50, y + 3, 0x2716, "Goto", new GumpCallback(Goto));
            }

            if (Data.GetData(Owner).MsgSound)
            {
                AddHtml(x, y += 25, width, "<CENTER>" + General.Local(17));
                AddImageTiled(x + width / 2 - 25, y += 25, 50, 21, 0xBBA);
                AddTextField(x + width / 2 - 25, y, 50, 21, 0x480, 0xBBA, "Sound", Data.GetData(Owner).GetSound(c_Target).ToString());
                AddButton(x + width / 2 + 30, y + 3, 0x15E1, 0x15E5, "Play Sound", new GumpCallback(PlaySound));
                AddButton(x + width / 2 - 40, y, 0x983, "Sound Up", new GumpCallback(SoundUp));
                AddButton(x + width / 2 - 40, y + 10, 0x985, "Sound Down", new GumpCallback(SoundDown));
            }

            AddBackgroundZero(x, 0, width, y + 40, Data.GetData(c_Target).DefaultBack, false);
        }

        private void ClearSig()
        {
            Data.GetData(Owner).Signature = "";
            NewGump();
        }

        private void SubmitSig()
        {
            Data.GetData(Owner).Signature = GetTextField("Signature");
            NewGump();
        }

        private void Friend()
        {
            if (Data.GetData(c_Target).ByRequest && !Data.GetData(Owner).Friends.Contains(c_Target))
            {
                if (!TrackSpam.LogSpam(Owner, "Request " + c_Target.Name, TimeSpan.FromHours(Data.RequestSpam)))
                {
                    TimeSpan ts = TrackSpam.NextAllowedIn(Owner, "Request " + c_Target.Name, TimeSpan.FromHours(Data.RequestSpam));
                    string txt = (ts.Days != 0 ? ts.Days + " " + General.Local(170) + " " : "") + (ts.Hours != 0 ? ts.Hours + " " + General.Local(171) + " " : "") + (ts.Minutes != 0 ? ts.Minutes + " " + General.Local(172) + " " : "");

                    Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(96) + " " + txt);
                    NewGump();
                    return;
                }

                Data.GetData(c_Target).AddMessage(new Message(Owner, General.Local(84), General.Local(85), MsgType.Invite));

                Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(86) + " " + c_Target.Name);

                NewGump();
              
                return;
            }

            if (Data.GetData(Owner).Friends.Contains(c_Target))
                Data.GetData(Owner).RemoveFriend(c_Target);
            else
                Data.GetData(Owner).AddFriend(c_Target);

            NewGump();
        }

        private void Ignore()
        {
            if (Data.GetData(Owner).Ignores.Contains(c_Target))
                Data.GetData(Owner).RemoveIgnore(c_Target);
            else
                Data.GetData(Owner).AddIgnore(c_Target);

            NewGump();
        }

        private void Message()
        {
            NewGump();

            if (Chat3.Message.CanMessage(Owner, c_Target))
                new SendMessageGump(Owner, c_Target, "", null, MsgType.Normal);
        }

        private void GlobalAccess()
        {
            Data.GetData(c_Target).GlobalAccess = !Data.GetData(c_Target).GlobalAccess;

            if (Data.GetData(c_Target).GlobalAccess)
                Owner.SendMessage(Data.GetData(Owner).SystemC, c_Target.Name + " " + General.Local(75));
            else
                Owner.SendMessage(Data.GetData(Owner).SystemC, c_Target.Name + " " + General.Local(76));

            NewGump();
        }

        private void Ban()
        {
            if (Data.GetData(c_Target).Banned)
            {
                Data.GetData(c_Target).RemoveBan();
                Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(78) + " " + c_Target.Name);
                NewGump();
            }
            else
                new BanGump(c_Target, this);
        }

        private void GIgnore()
        {
            if (Data.GetData(Owner).GIgnores.Contains(c_Target))
                Data.GetData(Owner).RemoveGIgnore(c_Target);
            else
                Data.GetData(Owner).AddGIgnore(c_Target);

            NewGump();
        }

        private void GListen()
        {
            if (Data.GetData(Owner).GListens.Contains(c_Target))
                Data.GetData(Owner).RemoveGListen(c_Target);
            else
                Data.GetData(Owner).AddGListen(c_Target);

            NewGump();
        }

        private void Client()
        {
            NewGump();

            if (c_Target.NetState == null)
                Owner.SendMessage(Data.GetData(Owner).SystemC, c_Target.Name + " " + General.Local(83));
            else
                Owner.SendGump(new ClientGump(Owner, c_Target.NetState));
        }

        private void Goto()
        {
            if (c_Target.NetState == null)
                Owner.SendMessage(Data.GetData(Owner).SystemC, c_Target.Name + " " + General.Local(83));
            else
            {
                Owner.Location = c_Target.Location;
                Owner.Map = c_Target.Map;
            }

            NewGump();
        }

        private void BecomeUser()
        {
            NewGump();

            General.List(Owner, c_Target);
        }

        private void PlaySound()
        {
            Data.GetData(Owner).SetSound(c_Target, GetTextFieldInt("Sound"));
            Owner.SendSound(Data.GetData(Owner).GetSound(c_Target));

            NewGump();
        }

        private void SoundUp()
        {
            Data.GetData(Owner).SetSound(c_Target, Data.GetData(Owner).GetSound(c_Target) + 1);

            NewGump();
        }

        private void SoundDown()
        {
            Data.GetData(Owner).SetSound(c_Target, Data.GetData(Owner).GetSound(c_Target) - 1);

            NewGump();
        }

        private void AvatarUp()
        {
            Data.GetData(c_Target).AvatarUp();

            NewGump();
        }

        private void AvatarDown()
        {
            Data.GetData(c_Target).AvatarDown();

            NewGump();
        }


        private class BanGump : GumpPlus
        {
            private GumpPlus c_Gump;
            private Mobile c_Target;

            public BanGump(Mobile m, GumpPlus g) : base(g.Owner, 100, 100)
            {
                c_Gump = g;
                c_Target = m;
            }

            protected override void BuildGump()
            {
                int width = 150;
                int y = 10;

                AddHtml(0, y, width, "<CENTER>" + General.Local(160));

                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(161));
                AddButton(width / 2 - 50, y + 3, 0x2716, "30 minutes", new GumpStateCallback(BanTime), TimeSpan.FromMinutes(30));
                AddButton(width / 2 + 40, y + 3, 0x2716, "30 minutes", new GumpStateCallback(BanTime), TimeSpan.FromMinutes(30));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(162));
                AddButton(width / 2 - 50, y + 3, 0x2716, "1 hour", new GumpStateCallback(BanTime), TimeSpan.FromHours(1));
                AddButton(width / 2 + 40, y + 3, 0x2716, "1 hour", new GumpStateCallback(BanTime), TimeSpan.FromHours(1));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(163));
                AddButton(width / 2 - 50, y + 3, 0x2716, "12 hours", new GumpStateCallback(BanTime), TimeSpan.FromHours(12));
                AddButton(width / 2 + 40, y + 3, 0x2716, "12 hours", new GumpStateCallback(BanTime), TimeSpan.FromHours(12));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(164));
                AddButton(width / 2 - 50, y + 3, 0x2716, "1 day", new GumpStateCallback(BanTime), TimeSpan.FromDays(1));
                AddButton(width / 2 + 40, y + 3, 0x2716, "1 day", new GumpStateCallback(BanTime), TimeSpan.FromDays(1));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(165));
                AddButton(width / 2 - 50, y + 3, 0x2716, "1 week", new GumpStateCallback(BanTime), TimeSpan.FromDays(7));
                AddButton(width / 2 + 40, y + 3, 0x2716, "1 week", new GumpStateCallback(BanTime), TimeSpan.FromDays(7));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(166));
                AddButton(width / 2 - 50, y + 3, 0x2716, "1 month", new GumpStateCallback(BanTime), TimeSpan.FromDays(30));
                AddButton(width / 2 + 40, y + 3, 0x2716, "1 month", new GumpStateCallback(BanTime), TimeSpan.FromDays(30));
                AddHtml(0, y += 20, width, "<CENTER>" + General.Local(167));
                AddButton(width / 2 - 50, y + 3, 0x2716, "1 year", new GumpStateCallback(BanTime), TimeSpan.FromDays(365));
                AddButton(width / 2 + 40, y + 3, 0x2716, "1 year", new GumpStateCallback(BanTime), TimeSpan.FromDays(365));

                AddBackgroundZero(0, 0, width, y + 40, Data.GetData(c_Target).DefaultBack);
            }

            private void BanTime(object o)
            {
                if (!(o is TimeSpan))
                    return;

                Data.GetData(c_Target).Ban((TimeSpan)o);
                Owner.SendMessage(Data.GetData(Owner).SystemC, General.Local(77) + " " + c_Target.Name);

                c_Gump.NewGump();
            }

            protected override void OnClose()
            {
                c_Gump.NewGump();
            }
        }
    }
}