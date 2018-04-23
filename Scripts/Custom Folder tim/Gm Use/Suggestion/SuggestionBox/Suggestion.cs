using System;
using Server;
using System.IO;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Accounting;
using System.Collections;
using Server.Commands;
using System.Text;
namespace Server.Gumps
{
    public class Suggestion : Gump
    {
        public Suggestion()
            : base(0, 0)
        {
            Closable = false;
            Disposable = false;
            Dragable = true;

            AddPage(0);
            AddBackground(241, 189, 288, 276, 9300);
			AddImageTiled(254, 210, 260, 11, 50);
            AddImageTiled(254, 451, 260, 11, 50);
            AddImageTiled(245, 190, 11, 274, 50);
            AddImageTiled(512, 189, 11, 273, 50);
            AddImage(481, 426, 9005, 231);
            AddImage(255, 426, 9005, 231);
            AddImage(474, 435, 95, 231);
            AddImage(289, 434, 97, 231);
            AddButton(424, 405, 69, 70, 1, GumpButtonType.Reply, 0); // Close Button
            AddButton(292, 405, 69, 70, 0, GumpButtonType.Reply, 0); // Okay Button
            AddLabel(317, 194, 0, @"Suggestion Box"); // Suggestion Box Label 1
			AddTextEntry(257, 223, 254, 400, 0, 0, @"");
            AddLabel(300, 433, 0, @"Okay"); // Label 2
            AddLabel(432, 433, 0, @"Close"); // Label 3
        }
        public enum Buttons
        {
            TextEntry1,
        }
        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            Account acct = (Account)from.Account;

            switch (info.ButtonID)
            {

                case 0:
                    string tudo = (string)info.GetTextEntry((int)Buttons.TextEntry1).Text;

                    Console.WriteLine("");
                    Console.WriteLine("{0} From Account {1} Sent a suggestion", from.Name, acct.Username);//from.Name of account send a suggestion
                    Console.WriteLine("");

                    if (!Directory.Exists("suggestion")) //create directory
                        Directory.CreateDirectory("suggestion");

                    using (StreamWriter op = new StreamWriter("suggestion/suggestion.txt", true))
                    {
                        op.WriteLine("");
                        op.WriteLine("Name Of Character: {0}, Account:{1}", from.Name, acct.Username);
                        op.WriteLine("Message: {0}", tudo);
                        op.WriteLine("");
                    }


                    from.SendMessage("Thanks for your suggestion!");//thanks to send your suggestion

                    break;

                case 1:
                    from.CloseGump(typeof(Suggestion));
                    break;
            }
        }
    }
}
