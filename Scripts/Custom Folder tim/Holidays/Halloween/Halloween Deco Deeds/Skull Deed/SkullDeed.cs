using System; 
using Server; 
using Server.Gumps; 
using Server.Network; 

namespace Server.Items 
{ 

public class SkullDeed : Item 
{ 

[Constructable] 
public SkullDeed() : this( null ) 
{ 
} 

[Constructable] 
public SkullDeed ( string name ) : base ( 0x14F0 ) 
{ 
Name = "Skull Deed"; 
LootType = LootType.Blessed; 
Hue = 1153; 
}

    public SkullDeed(Serial serial)
        : base(serial) 
{ 
} 

public override void OnDoubleClick( Mobile from ) 
{ 
if ( !IsChildOf( from.Backpack ) ) 
{ 
from.SendLocalizedMessage( 1042001 ); 
} 
else 
{
    from.SendGump(new SkullGump(from, this)); 
} 
} 

public override void Serialize ( GenericWriter writer) 
{ 
base.Serialize ( writer ); 

writer.Write ( (int) 0); 
} 

public override void Deserialize( GenericReader reader ) 
{ 
base.Deserialize ( reader ); 

int version = reader.ReadInt(); 
} 
} 
}
