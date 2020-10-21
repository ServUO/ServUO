using Server.Mobiles;
using Server.Network;
using Server.Services.Virtues;

namespace Server.Gumps
{
    public class HonorSelf : Gump
    {
        readonly PlayerMobile m_from;
        public HonorSelf(PlayerMobile from)
            : base(150, 50)
        {
            m_from = from;
            AddBackground(0, 0, 245, 145, 9250);
            AddButton(157, 101, 247, 248, 1, GumpButtonType.Reply, 0);
            AddButton(81, 100, 241, 248, 0, GumpButtonType.Reply, 0);
            AddHtml(21, 20, 203, 70, "Are you sure you want to use honor points on yourself?", true, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                HonorVirtue.ActivateEmbrace(m_from);
            }
            else
            {
                return;
            }
        }
    }
}
