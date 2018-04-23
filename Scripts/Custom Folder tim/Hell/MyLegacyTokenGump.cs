using System; 
using System.Net; 
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Accounting; 
using Server.Gumps; 
using Server.Items; 
using Server.Mobiles; 
using Server.Network;

namespace Server.Gumps
{
    public class MyLegacyTokenGump : Gump
    {
        private Mobile m_Mobile;
        private Item m_Deed;

        public MyLegacyTokenGump(Mobile from, Item deed)
            : base(30, 20)
        {
            m_Mobile = from;
            m_Deed = deed;

            Closable = true;
            Disposable = false;
            Dragable = true;
            Resizable = false;
            AddPage(1);
      
            Account a = from.Account as Account;

            AddImage(128, 1, 1418);
            AddImage(119, 255, 2620);
            AddImage(408, 255, 2622);
            AddImage(139, 255, 2621);
            AddImage(406, 273, 2625);
            AddImage(141, 279, 2624);
            AddImage(119, 273, 2623);
            AddImage(408, 483, 2628);
            AddImage(139, 483, 2627);
            AddImage(119, 483, 2626);
            AddImage(141, 272, 2624);
            AddImage(396, 154, 10441);
            AddImage(68, 154, 10440);
            AddLabel(176, 267, 1153, "Please Select An Ethereal Mount");
            AddButton(138, 309, 4005, 4006, 1, GumpButtonType.Reply, 1);
            AddLabel(192, 309, 1153, "Ethereal Llama");
            AddButton(138, 369, 4005, 4006, 2, GumpButtonType.Reply, 2);
            AddLabel(192, 369, 1153, "Ethereal Horse");
            AddButton(138, 429, 4005, 4006, 3, GumpButtonType.Reply, 3);
            AddLabel(192, 429, 1153, "Ethereal Ostard");
            
                    
}


        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0:  
                    {
                        from.CloseGump(typeof(MyLegacyTokenGump));
                        break;
                    }
                case 1: 
                    {
                        Item item = new EtherealLlama();
                        from.AddToBackpack(item);
                        from.CloseGump(typeof(MyLegacyTokenGump));
                        m_Deed.Delete();
                        break;
                    }
                case 2: 
                    {
                        Item item = new EtherealHorse();
                        from.AddToBackpack(item);
                        from.CloseGump(typeof(MyLegacyTokenGump));
                        m_Deed.Delete();
                        break;
                    }
                case 3: 
                    {
                        Item item = new EtherealOstard();
                        from.AddToBackpack(item);
                        from.CloseGump(typeof(MyLegacyTokenGump));
                        m_Deed.Delete();
                        break;
                    }
                
              }
        }
    }
}
