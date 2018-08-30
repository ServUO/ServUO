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
public class SpiderWebGump : Gump 
{ 
private Mobile m_Mobile; 
private Item m_Deed;


    public SpiderWebGump(Mobile from, Item deed)
        : base(30, 20) 
{ 
m_Mobile = from; 
m_Deed = deed; 

Closable=true;
Disposable=false;
Dragable=true;
Resizable=false;
AddPage( 1 ); 

AddBackground( 0, 0, 300, 400, 3000 ); 
AddBackground( 8, 8, 284, 384, 5054 );

AddLabel(40, 12, 37, "SpiderWebDeed"); 

Account a = from.Account as Account; 


AddLabel( 52, 40, 0, "Spider Web 1" ); 
AddButton( 14, 40, 4005, 4007, 1, GumpButtonType.Reply, 1 ); 
AddLabel( 52, 60, 0, "Spider Web 2" ); 
AddButton( 14, 60, 4005, 4007, 2, GumpButtonType.Reply, 2 );
AddLabel(52, 80, 0, "Spider Web 3"); 
AddButton( 14, 80, 4005, 4007, 3, GumpButtonType.Reply, 3 );
AddLabel(52, 100, 0, "Spider Web 4");
AddButton(14, 100, 4005, 4007, 4, GumpButtonType.Reply, 4); 


} 


public override void OnResponse( NetState state, RelayInfo info ) 
{ 
Mobile from = state.Mobile; 

switch ( info.ButtonID ) 
{ 
case 0: //Close Gump 
{
    from.CloseGump(typeof(SpiderWebGump)); 
break; 
}
    case 1: //SmallWeb1AddonDeed 
{
    Item item = new SmallWeb1AddonDeed();
item.LootType = LootType.Blessed; 
from.AddToBackpack( item );
from.CloseGump(typeof(SpiderWebGump));
m_Deed.Delete();
break; 
}
    case 2: // SmallWeb2AddonDeed
{
    Item item = new SmallWeb2AddonDeed();
item.LootType = LootType.Blessed; 
from.AddToBackpack( item );
from.CloseGump(typeof(SpiderWebGump));
m_Deed.Delete();
break; 
}
    case 3: //SmallWeb3AddonDeed 
{
    Item item = new SmallWeb3AddonDeed();
item.LootType = LootType.Blessed; 
from.AddToBackpack( item );
from.CloseGump(typeof(SpiderWebGump));
m_Deed.Delete();
break; 
}
    case 4: //SmallWeb4AddonDeed
{
    Item item = new SmallWeb4AddonDeed();
    item.LootType = LootType.Blessed;
    from.AddToBackpack(item);
    from.CloseGump(typeof(SpiderWebGump));
    m_Deed.Delete();
    break;
}
} 
} 
} 
}
