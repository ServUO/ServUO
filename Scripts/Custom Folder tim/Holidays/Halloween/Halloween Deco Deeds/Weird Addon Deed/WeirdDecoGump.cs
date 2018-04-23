using System; 
using System.Net; 
using Server; 
using Server.Accounting; 
using Server.Gumps; 
using Server.Items; 
using Server.Mobiles; 
using Server.Network;

namespace Server.Gumps
{
    public class WeirdDecoGump : Gump
    {
        private Mobile m_Mobile;
        private Item m_Deed;


        public WeirdDecoGump(Mobile from, Item deed)
            : base(30, 20)
        {
            m_Mobile = from;
            m_Deed = deed;

            Closable = true;
            Disposable = false;
            Dragable = true;
            Resizable = false;
            AddPage(1);

            AddBackground(0, 0, 300, 400, 3000);
            AddBackground(8, 8, 284, 384, 5054);

            AddLabel(40, 12, 37, "Weird Deco Gump");

            Account a = from.Account as Account;


            AddLabel(52, 40, 0, "Corpse");
            AddButton(12, 40, 4005, 4007, 1, GumpButtonType.Reply, 1);
            AddLabel(52, 60, 0, "Lil Coffin");
            AddButton(12, 60, 4005, 4007, 2, GumpButtonType.Reply, 2);
            AddLabel(52, 80, 0, "Lil Iron Maiden");
            AddButton(12, 80, 4005, 4007, 3, GumpButtonType.Reply, 3);



        }


        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            switch (info.ButtonID)
            {
                case 0: //Close Gump 
                    {
                        from.CloseGump(typeof(WeirdDecoGump));
                        break;
                    }
                case 1: //CorpseAddonDeed
                    {
                        Item item = new CorpseAddonDeed();
                        item.LootType = LootType.Blessed;
                        from.AddToBackpack(item);
                        from.CloseGump(typeof(WeirdDecoGump));
                        m_Deed.Delete();
                        break;
                    }
                case 2: // LilCoffinSouthAddonDeed
                    {
                        Item item = new LilCoffinSouthAddonDeed();
                        item.LootType = LootType.Blessed;
                        from.AddToBackpack(item);
                        from.CloseGump(typeof(WeirdDecoGump));
                        m_Deed.Delete();
                        break;
                    }
                case 3: // LilIronMaidenAddonDeed
                    {
                        Item item = new LilIronMaidenAddonDeed();
                        item.LootType = LootType.Blessed;
                        from.AddToBackpack(item);
                        from.CloseGump(typeof(WeirdDecoGump));
                        m_Deed.Delete();
                        break;
                    }
            }
        }
    }
}
