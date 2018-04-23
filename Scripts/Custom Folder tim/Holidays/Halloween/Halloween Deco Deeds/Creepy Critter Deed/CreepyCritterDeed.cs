using System; 
using Server; 
using Server.Gumps; 
using Server.Network; 

namespace Server.Items 
{ 

public class CreepyCritterDeed : Item 
{ 

[Constructable] 
public CreepyCritterDeed() : this( null ) 
{ 
} 

[Constructable] 
public CreepyCritterDeed ( string name ) : base ( 0x14F0 ) 
{ 
Name = "Creepy Critter Deed"; 
LootType = LootType.Blessed; 
Hue = 53; 
}

    public CreepyCritterDeed(Serial serial)
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
    from.SendGump(new CreepyCritterGump(from, this)); 
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
