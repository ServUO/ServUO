using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Gumps
{
    public class HonorSelf : Gump
    {
        readonly PlayerMobile m_from;
        public HonorSelf(PlayerMobile from)
            : base(150, 50)
        {
            this.m_from = from;
            this.AddBackground(0, 0, 245, 145, 9250);
            this.AddButton(157, 101, 247, 248, 1, GumpButtonType.Reply, 0);
            this.AddButton(81, 100, 241, 248, 0, GumpButtonType.Reply, 0);
            this.AddHtml(21, 20, 203, 70, @"Are you sure you want to use honor points on yourself?", true, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                HonorVirtue.ActivateEmbrace(this.m_from);
            }
            else
            {
                return;
            }
        }
    }
}