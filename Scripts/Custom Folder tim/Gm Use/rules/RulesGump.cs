using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Mobiles;

namespace Server
{
    public class RulesGump : Gump
    {
        public RulesGump()
            : base(0, 0)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(154, 66, 499, 479, 5120);
            this.AddImageTiled(165, 76, 477, 459, 2624);
            this.AddPage(1);
            this.AddLabel(313, 84, 67, @"Welcome to Vanquishing Uo");
            this.AddAlphaRegion(175, 109, 456, 379);
            this.AddHtml(175, 110, 456, 38, @"<BODY><BASEFONT COLOR=Green><u>Before you start playing in our server we need to fill you in about our home.</BODY>", (bool)false, (bool)false);
            this.AddHtml(178, 158, 450, 328, @"<BODY><BASEFONT COLOR=#B3FFE8>1-)The Game Starts. You will be asked to submit a email adress. Please do not worry, the email only will be used to fill you personally in on major events., you will be ABLE to Rent and Buy any House or Shop.<br><br>2-) We are a PVP,Pvm and Pure Peaceful RP server. This is a RPVM server which is based on straight Rules. You have to ask a Player to PVP with them before you attack them, not as you are about to attack or attacking. (This doesn't include fel areas. in these areas, murder nad theft can happen.)<br><br>3-) You will be able to own Three Houses per Account. if you go inactive for Four-Weeks straight without giving information, we will DELETE your house and open it for another Player. Also your Items will be DEPOSITED at your Bank Box.<br><br>4-) Every Account has 6 character slot per one and we want to note that, you CANNOT have a Second Account without first asking.Some people have others from the same house hold that like to join and this is fine but needs to be announced. If you go against this, It will result in Actions being taken.<br><br>5-) We want our players to NOT Play-Dirty in Politics. This doesnt mean you cant make conspiracy about other character, we just want you to NOT play dirty.<br><br>6-) We want our players to NOT disrespect us and fellow players, we are here to help you and provide you a better server, we are working for you almost 24 Hours, Everyday of Week. If you want something or have questions in your mind, please feel free and tell us what is it without any insults or inappropriate words. And please, also keep this in your mind, be nice to other players.<br><br>7-) Servers language is English. So please make sure you are using English in Public. You can speak any language when you are with your friends IN PRIVATE, but you are FORCED use English in PUBLIC.8- ) Please Enjoy your stay.</BODY>", (bool)false, (bool)true);
            this.AddLabel(182, 502, 37, @"www.URL.com");
            this.AddImage(105, 42, 10400);
            this.AddImage(619, 449, 10412);
            this.AddButton(584, 499, 2152, 2153, (int)Buttons.Button1, GumpButtonType.Reply, 0);
            this.AddLabel(538, 503, 67, @"Close!");

        }

        public enum Buttons
        {
            Button1,
        }
    }
}
