using Server.Gumps;
using Server.Mobiles;

namespace Server.Services.TownCryer
{
    public class BaseTownCryerGump : BaseGump
    {
        public TownCrier Cryer { get; private set; }

        public BaseTownCryerGump(PlayerMobile pm, TownCrier cryer)
            : base(pm, 50, 50)
        {
            Cryer = cryer;
        }

        public override void AddGumpLayout()
        {
            AddPage(0);

            AddBackground(0, 0, 854, 700, 0x24AE);
            AddImage(156, 35, 0x266C);

            AddBackground(40, 130, 770, 470, 0x2486);
        }
    }
}