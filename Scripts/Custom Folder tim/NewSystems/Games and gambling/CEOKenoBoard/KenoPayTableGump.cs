using System;
using Server.Network;

namespace Server.Gumps
{
    public class KenoPayTableGump : Gump
    {
        public enum PayMethod { Standard, QuickPick, BottomHalf, TopHalf, RightHalf, LeftHalf, Odd, Even, Edges, Kool20, EZBucks, BancoSpecial, Millionare10 }
        private readonly PayMethod m_PayMethod;

        public KenoPayTableGump(int PayTable, float[] PayOutTable, int Cost, int Selected)    
            : base(15, 25)
        {
            m_PayMethod = (PayMethod)PayTable;
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
            AddPage(0);
            int selected = Selected > 20 ? 21 : Selected + 1;
            int length =  selected * 20;
            AddBackground(0, 0, 250, 105 + length, 9250);
            string text = String.Format("Ticket Cost: {0}", Cost );
            AddLabel(70, 10, 1149, "Keno Payout Table");
            AddLabel(15, 30, 2213, text);
            text = String.Format("Method: {0}", DisplayMethod(m_PayMethod));
            AddLabel(15, 45, 2213, text);
            AddLabel(15, 70, 1149, "# Hit");
            AddLabel(180, 70, 1149, "Payout");
            for (int i = 0; i < selected; i++)
            {
                text = String.Format("{0} of {1}", i, Selected);
                AddLabel(15, 90 + (i * 20), ( i % 2 == 0 ? 1149 : 1150), text);
                text = String.Format("{0:##,###,##0}", (int)(Cost * PayOutTable[i]));
                AddLabel(160, 90 + (i * 20), (i % 2 == 0 ? 1149 : 1150), text);
            }
        }

        private string DisplayMethod(PayMethod m_PayTable)
        {
            switch (m_PayTable)
            {
                case PayMethod.Standard:
                    return "Standard";
                case PayMethod.QuickPick:
                    return "QuickPick";
                case PayMethod.BottomHalf:
                    return "Bottom";
                case PayMethod.TopHalf:
                    return "Top";
                case PayMethod.RightHalf:
                    return "Right";
                case PayMethod.LeftHalf:
                    return "Left";
                case PayMethod.Odd:
                    return "Odd";
                case PayMethod.Even:
                    return "Even";
                case PayMethod.Edges:
                    return "Edges";
                case PayMethod.Kool20:
                    return "Kool 20";
                case PayMethod.EZBucks:
                    return "EZ Bucks";
                case PayMethod.BancoSpecial:
                    return "Banco Special";
                case PayMethod.Millionare10:
                    return "Millionare 10";
            }
            return "Unknown!";
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (from == null)
                return;
            if (info.ButtonID == 0)
            {
                return;
            }
            
        }
    }
}